using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microex.All.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

namespace Microex.All.PathMapProxy
{
    public class PortProxyMiddleware:IMiddleware
    {
        private readonly ILogger<PortProxyMiddleware> _logger;
        private List<string> _portMappings;

        public PortProxyMiddleware(PathMapProxyOptions options,ILogger<PortProxyMiddleware> logger)
        {
            _logger = logger;
            this._portMappings = options.PortMappings;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var _httpClient = new HttpClient(new HttpClientHandler(){AllowAutoRedirect = false});
            string curPort = "";
            if (context.Request.Query.TryGetValue("port", out StringValues forcedPort))
            {
                context.Response.Cookies.Delete("port");
                context.Response.Cookies.Append("port", forcedPort);
                curPort = forcedPort;
            }
            else
            {
                if (context.Request.Cookies.TryGetValue("port", out string cachedPort))
                {
                    curPort = cachedPort;
                }
            }

            if (!_portMappings.Contains(curPort))
            {
                await next.Invoke(context);
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
                $"http://localhost:{ curPort}{ context.Request.PathBase}{ context.Request.Path}{ context.Request.QueryString}";
            request.RequestUri = new Uri(uriString);
            request.Method = new HttpMethod(context.Request.Method);
            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
                // 处理重定向
                if (responseMessage.StatusCode == HttpStatusCode.MovedPermanently)
                {
                    context.Response.Redirect(responseMessage.Headers.Location.ToString());
                }
                context.Response.StatusCode = (int)responseMessage.StatusCode;
                foreach (KeyValuePair<string, IEnumerable<string>> header in responseMessage.Headers)
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                foreach (KeyValuePair<string, IEnumerable<string>> header in responseMessage.Content.Headers)
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                context.Response.Headers.Remove("transfer-encoding");
                //// 避免重定向引起的页面缓存
                //if (context.Response.Headers.TryGetValue("Cache-Control",out _))
                //{
                //    context.Response.Headers.Remove("Cache-Control");
                //}
                //context.Response.Headers.Add("Cache-Control", "must-revalidate");
                await responseMessage.Content.CopyToAsync(context.Response.Body);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "代理访问调用失败,request:{request}", request.ToJson());
                await next.Invoke(context);
            }
            finally
            {
                responseMessage?.Dispose();
            }

        }
    }
}
