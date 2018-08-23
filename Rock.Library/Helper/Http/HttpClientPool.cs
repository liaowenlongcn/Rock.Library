using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Library.Helper.Http
{
    public class HttpClientPool
    {
        private static readonly HttpClientPool instance = new HttpClientPool();
        private ConcurrentBag<HttpClient> httpClients = null;

        private HttpClientPool()
        {
            httpClients = new ConcurrentBag<HttpClient>();
        }
        public static HttpClientPool Instance
        {
            get { return instance; }
        }

        public ConcurrentBag<HttpClient> HttpClients
        {
            get { return httpClients; }
        }

        public HttpClient Open(Dictionary<string, object> headers)
        {
            HttpClient httpClient = null;
            if (httpClients.Count > 0)
            {
                httpClients.TryTake(out httpClient);
            }
            else
            {
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            }
            if (headers == null) return httpClient;
            foreach (var header in headers)
            {
                if (httpClient.DefaultRequestHeaders.Contains(header.Key))
                {
                    httpClient.DefaultRequestHeaders.Remove(header.Key);
                }
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value.ToJson());
            }
            return httpClient;
        }
    }
}
