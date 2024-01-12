using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace apitude_meta.Repositories
{
    /// <summary>
    /// Asynchronous http client repository
    /// </summary>
    public class HttpRepository
    {

        private readonly int _defaultTimeOut = 6000000;
        private readonly HttpClient _client;
        private HttpRepository()
        {
            _client = new HttpClient();
        }

        public static HttpRepository Instance { get; } = new HttpRepository();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request">Http request parameters</param>
        /// <returns>Http response, in case of invokation failure, returns with Status Code 408 and non null Exception property value</returns>
        public async Task<DXHttpResponse> ExecuteRequest(DXHttpRequest request)
        {
            var stopWatch = Stopwatch.StartNew();

            using (var requestMessage = new HttpRequestMessage(new HttpMethod(request.RequestType), request.Url))
            {
                var isRequestCompressed =
                    requestMessage.Headers.TryAddWithoutValidation("Content-Encoding", new List<string>() { "gzip" });

                requestMessage.Content = (isRequestCompressed)
                    ? (HttpContent)new StreamContent(
                        new GZipStream(new MemoryStream(Encoding.UTF8.GetBytes(request.Body)),
                            CompressionLevel.Fastest))
                    : new StringContent(request.Body);

                //Add headers
                if (request.Headers != null)
                {
                    foreach (var key in request.Headers.Keys)
                    {
                        var val = request.Headers[key];
                        switch (key.ToLower())
                        {
                            case "authorization":
                                var spl = val.Split(' ');
                                if (spl.Length == 2)
                                {
                                    requestMessage.Headers.Authorization =
                                        new AuthenticationHeaderValue(spl[0], spl[1]);
                                }
                                else
                                {
                                    requestMessage.Headers.TryAddWithoutValidation("Authorization", val);
                                }

                                break;
                            case "connection":
                                requestMessage.Headers.Connection.Add(val);
                                break;
                            case "accept":
                                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(val));
                                break;
                            case "content-type":
                                requestMessage.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(val);
                                break;
                            default:
                                requestMessage.Headers.TryAddWithoutValidation(key, request.Headers[key]);
                                break;
                        }
                    }
                }

                var isResponseCompressed =
                    requestMessage.Headers.TryAddWithoutValidation("Accept-Encoding", new List<string>() { "gzip" });

                //Check if request has valid timeout value, use it otherwise use default timeout
                var cancellationToken =
                    new CancellationTokenSource((request.Timeout != -1) ? request.Timeout : _defaultTimeOut);
                try
                {
                    using (var responseMessage = await _client.SendAsync(requestMessage, cancellationToken.Token))
                    {
                        stopWatch.Stop();

                        return new DXHttpResponse
                        {
                            StatusCode = (int)responseMessage.StatusCode,
                            Headers = (from item in responseMessage.Headers
                                       select new
                                       {
                                           Key = item.Key,
                                           Value = string.Join(" ", item.Value)
                                       }).ToDictionary(i => i.Key, i => i.Value),
                            Body = (isResponseCompressed && responseMessage.Content.Headers.Any(x => x.Key.Equals("content-encoding", StringComparison.OrdinalIgnoreCase) && x.Value.Any(y => y.Equals("gzip", StringComparison.OrdinalIgnoreCase))))
                                ? await (Decompress(await responseMessage.Content.ReadAsStreamAsync()))
                                : await responseMessage.Content.ReadAsStringAsync()
                        };
                    }
                }
                catch (TaskCanceledException e)
                {
                    stopWatch.Stop();

                    return new DXHttpResponse()
                    {
                        StatusCode = 408,
                        Exception = e
                    };
                }
            }
        }

        static async Task<string> Decompress(Stream stream)
        {
            using (var mStream = new GZipStream(stream, CompressionMode.Decompress))
            {
                using (var reader = new StreamReader(mStream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ~HttpRepository()
        {
            _client.Dispose();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DXHttpRequest
    {
        /// <summary>
        /// POST, GET etc.
        /// </summary>
        public string RequestType { get; set; }
        /// <summary>
        /// Valid URL string incuding http/https
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Timeout in milliseconds if there is, otherwise use -1
        /// </summary>
        public int Timeout { get; set; } = -1;
        /// <summary>
        /// Initialize before adding headers
        /// </summary>
        public Dictionary<string, string> Headers;
        /// <summary>
        /// 
        /// </summary>
        public string ClientName { get; set; } = "default";
        /// <summary>
        /// 
        /// </summary>
        public string Body { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DXHttpResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Exception Exception { get; set; }
    }
}
