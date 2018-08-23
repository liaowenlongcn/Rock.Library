using Rock.Library.Helper.Http;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Rock.Library.Helper
{
    public class HttpHelper
    {
        #region Get请求

        public static HttpResponse Get(string url, Dictionary<string, object> headers = null, AuthenticationHeaderValue authorization=null)
        {
            HttpResponse result = new HttpResponse();
            var client = GetHttpClient(headers);
            client.DefaultRequestHeaders.Authorization = authorization;
            var response = client.GetAsync(new Uri(url)).Result;
            var task = response.Content.ReadAsStringAsync();
            result.StatusCode = response.StatusCode;
            result.Result = task.Result;
            response.Dispose();
            Close(client);
            return result;
        }
        #endregion

        #region Post请求
        public static HttpResponse Post(string url, string data, Dictionary<string, object> headers = null, AuthenticationHeaderValue authorization = null)
        {
            HttpResponse result = new HttpResponse();
            var client = GetHttpClient(headers);
            if (String.IsNullOrEmpty(data)) data = "{}";
            HttpContent stringContent = new StringContent(data);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            client.DefaultRequestHeaders.Authorization = authorization;
            var response = client.PostAsync(new Uri(url), stringContent).Result;
            var task = response.Content.ReadAsStringAsync();
            result.StatusCode = response.StatusCode;
            result.Result = task.Result;
            response.Dispose();
            Close(client);
            return result;
        }

        #endregion

        #region 创建HttpClient

        /// <summary>
        /// 获取一个用于请求的HttpClient
        /// </summary>
        public static HttpClient GetHttpClient(Dictionary<string, object> headers)
        {
            var client = HttpClientPool.Instance.Open(headers);
            return client;
        }

        #endregion 

        #region 关闭HttpClient

        public static void Close(HttpClient client)
        {
            var httpClients = HttpClientPool.Instance.HttpClients;
            if (httpClients.Count < 10)
            {
                HttpClientPool.Instance.HttpClients.Add(client);
            }
            else
            {
                client.Dispose();
            }
        }

        #endregion
    }
}
