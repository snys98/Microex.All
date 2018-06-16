using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microex.All.Extensions;
using Microex.All.IdentityServer;
using Microex.All.PathMappedProxy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using HttpHeaders = Aliyun.OSS.Util.HttpHeaders;

namespace Microex.All.Common
{
    public class PortProxyMiddleware:IMiddleware
    {
        private readonly ILogger<PortProxyMiddleware> _logger;
        private HttpClient _httpClient;
        private List<string> _portMappings;

        public PortProxyMiddleware(PathMapProxyOptions options,ILogger<PortProxyMiddleware> logger)
        {
            _logger = logger;
            this._portMappings = options.PortMappings;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var session = context.Session;
            _httpClient = new HttpClient();
            foreach (var portMapping in this._portMappings)
            {
                if (context.Request.Query.TryGetValue("port",out StringValues port) && port == portMapping)
                {
                    session.SetString("port", portMapping);
                }
                else if(session.TryGetValue("port", out var portValue) && Convert.ToString(portValue) == portMapping)
                {
                }
                else
                {
                    await next.Invoke(context);
                }

                var curPort = session.GetString("port");

                if (curPort != portMapping)
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
                request.Headers.Host = $"localhost:{curPort}";
                string uriString =
                    $"{ context.Request.Scheme}://localhost:{ curPort}{ context.Request.PathBase}{ context.Request.Path}{ context.Request.QueryString}";
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
                    _logger.LogWarning(e, "代理访问调用失败,request:{request}", request.ToJson(formated:true));
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
