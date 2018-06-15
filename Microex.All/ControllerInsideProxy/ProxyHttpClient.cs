//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;
//using Microex.All.RestHttpClient;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Primitives;

//namespace Microex.All.ControllerInsideProxy
//{
//    public class ProxyHttpClient
//    {
//        private readonly HttpContext _context;
//        private readonly HttpClient _client = new HttpClient();


//        public ProxyHttpClient(HttpContext context)
//        {
//            _context = context;
//        }

//        public Task<string> GetStringAsync(string uri)
//        {
//            throw new NotImplementedException();
//        }

//        public Task<HttpResponseMessage> PostAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null,
//            string authorizationMethod = "Bearer")
//        {
//            throw new NotImplementedException();
//        }

//        public Task<HttpResponseMessage> DeleteAsync(string uri, string authorizationToken = null, string requestId = null,
//            string authorizationMethod = "Bearer")
//        {
//            throw new NotImplementedException();
//        }

//        public Task<HttpResponseMessage> PutAsync<T>(string uri, T item, string authorizationToken = null, string requestId = null,
//            string authorizationMethod = "Bearer")
//        {
//            throw new NotImplementedException();
//        }

//        public async void ProxyRequest(string url,string method)
//        {
//            HttpRequestMessage request = new HttpRequestMessage();
//            if (!string.Equals(_context.Request.Method, "GET", StringComparison.OrdinalIgnoreCase) &&
//                !string.Equals(_context.Request.Method, "HEAD", StringComparison.OrdinalIgnoreCase) &&
//                (!string.Equals(_context.Request.Method, "DELETE", StringComparison.OrdinalIgnoreCase) &&
//                 !string.Equals(_context.Request.Method, "TRACE", StringComparison.OrdinalIgnoreCase)))
//            {
//                StreamContent streamContent = new StreamContent(_context.Request.Body);
//                request.Content = streamContent;
//            }

//            foreach (KeyValuePair<string, StringValues> header in _context.Request.Headers)
//            {
//                HttpRequestHeaders headers1 = request.Headers;
//                string key1 = header.Key;
//                StringValues stringValues = header.Value;
//                string[] array1 = stringValues.ToArray();
//                if (!headers1.TryAddWithoutValidation(key1, array1) && request.Content != null)
//                {
//                    HttpContent content = request.Content;
//                    if (content != null)
//                    {
//                        HttpContentHeaders headers2 = content.Headers;
//                        string key2 = header.Key;
//                        stringValues = header.Value;
//                        string[] array2 = stringValues.ToArray();
//                        headers2.TryAddWithoutValidation(key2, array2);
//                    }
//                }
//            }

//            request.Headers.Host = $"{_context.Request.Host}";
//            string uriString =
//                $"{_context.Request.Scheme}://{_context.Request.Host}{_context.Request.PathBase}{_context.Request.Path}{_context.Request.QueryString}";
//            request.RequestUri = new Uri(uriString);
//            request.Method = new HttpMethod(_context.Request.Method);
//            HttpResponseMessage responseMessage = null;
//            try
//            {
//                responseMessage = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
//                    _context.RequestAborted);
//                _context.Response.StatusCode = (int)responseMessage.StatusCode;
//                foreach (KeyValuePair<string, IEnumerable<string>> header in responseMessage.Headers)
//                    _context.Response.Headers[header.Key] = header.Value.ToArray();
//                foreach (KeyValuePair<string, IEnumerable<string>> header in responseMessage.Content.Headers)
//                    _context.Response.Headers[header.Key] = header.Value.ToArray();
//                _context.Response.Headers.Remove("transfer-encoding");
//                await responseMessage.Content.CopyToAsync(_context.Response.Body);
//            }
//            catch (Exception e)
//            {
//            }
//            finally
//            {
//                responseMessage?.Dispose();
//            }
//        }
//    }
//}
