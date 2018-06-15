using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Microex.All.ControllerInsideProxy
{
    public static class Extensions
    {
        public static async void Proxy(this HttpContext context)
        {
            var _httpClient = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            if (!string.Equals(context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(context.Request.Method, "HEAD", StringComparison.OrdinalIgnoreCase) &&
                (!string.Equals(context.Request.Method, "DELETE", StringComparison.OrdinalIgnoreCase) &&
                 !string.Equals(context.Request.Method, "TRACE", StringComparison.OrdinalIgnoreCase)))
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

            request.Headers.Host = $"{context.Request.Host}";
            string uriString =
                $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}";
            request.RequestUri = new Uri(uriString);
            request.Method = new HttpMethod(context.Request.Method);
            HttpResponseMessage responseMessage = null;
            try
            {
                responseMessage = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                    context.RequestAborted);
                context.Response.StatusCode = (int) responseMessage.StatusCode;
                foreach (KeyValuePair<string, IEnumerable<string>> header in responseMessage.Headers)
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                foreach (KeyValuePair<string, IEnumerable<string>> header in responseMessage.Content.Headers)
                    context.Response.Headers[header.Key] = header.Value.ToArray();
                context.Response.Headers.Remove("transfer-encoding");
                await responseMessage.Content.CopyToAsync(context.Response.Body);
            }
            catch (Exception e)
            {
            }
            finally
            {
                responseMessage?.Dispose();
            }
        }
    }
}
