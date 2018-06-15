using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microex.All.IdentityServer;
using Microex.All.PathMappedProxy;
using Microex.All.PathMapProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using HttpHeaders = Aliyun.OSS.Util.HttpHeaders;

namespace Microex.All.Common
{
    public class PathMapProxyMiddleware:IMiddleware
    {
        private HttpClient _httpClient;
        private List<PathMap> _segment2PortMapping;
        private PathMap _curMap;

        public PathMapProxyMiddleware(PathMapProxyOptions options)
        {
            this._segment2PortMapping = options.PathMaps;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            _httpClient = new HttpClient();
            foreach (var seg2port in this._segment2PortMapping)
            {
                if (context.Request.Path.StartsWithSegments(seg2port.Path))
                {
                    _curMap = seg2port;
                }

                if (_curMap?.Path != seg2port.Path)
                {
                    continue;
                }
                HttpRequestMessage request = new HttpRequestMessage();
                if (!string.Equals(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) && !string.Equals(context.Request.Method, "HEAD", StringComparison.OrdinalIgnoreCase) && (!string.Equals(context.Request.Method, "DELETE", StringComparison.OrdinalIgnoreCase) && !string.Equals(context.Request.Method, "TRACE", StringComparison.OrdinalIgnoreCase)))
                {
                    StreamContent streamContent = new StreamContent(context.Request.Body);
                    request.Content = streamContent;
                }
                foreach (KeyValuePair<string, StringValues> header in context.Request.Headers)
                {
                    HttpRequestHeaders headers1 = request.Headers;
                    string key1 = header.Key;
                    StringValues stringValues = header.Value;
                    string[] array1 = stringValues.ToArray();
                    if (!headers1.TryAddWithoutValidation(key1, array1) && request.Content != null)
                    {
                        HttpContent content = request.Content;
                        if (content != null)
                        {
                            HttpContentHeaders headers2 = content.Headers;
                            string key2 = header.Key;
                            stringValues = header.Value;
                            string[] array2 = stringValues.ToArray();
                            headers2.TryAddWithoutValidation(key2, array2);
                        }
                    }
                }
                request.Headers.Host = $"{_curMap.ProxyAddress.Host}:{_curMap.ProxyAddress.Port}";
                string uriString =
                    $"{ _curMap.ProxyAddress.Scheme}://{ _curMap.ProxyAddress.Host}:{ _curMap.ProxyAddress.Port}{ context.Request.PathBase}{ context.Request.Path}{ context.Request.QueryString}";
                request.RequestUri = new Uri(uriString);
                request.Method = new HttpMethod(context.Request.Method);
                HttpResponseMessage responseMessage = null;
                try
                {
                    responseMessage = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
                    if (!responseMessage.IsSuccessStatusCode)
                    {
                        await next.Invoke(context);
                    }
                    context.Response.StatusCode = (int)responseMessage.StatusCode;
                    foreach (KeyValuePair<string, IEnumerable<string>> header in responseMessage.Headers)
                        context.Response.Headers[header.Key] = header.Value.ToArray();
                    foreach (KeyValuePair<string, IEnumerable<string>> header in responseMessage.Content.Headers)
                        context.Response.Headers[header.Key] = header.Value.ToArray();
                    context.Response.Headers.Remove("transfer-encoding");
                    await responseMessage.Content.CopyToAsync(context.Response.Body);
                }
                catch (Exception e)
                {
                    await next.Invoke(context);
                }
                finally
                {
                    responseMessage?.Dispose();
                }
            }
        }
    }
}
