using System;
using System.Net.Http;

namespace Office2PDF.Common
{
    public class APIHelper
    {
        public static string RunApiPost(string apiRootUrl,string requestUri, HttpContent context)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiRootUrl);

                var result = client.PostAsync(requestUri, context).Result.Content.ReadAsStringAsync().Result;
                return result;
            }
        }

        public static string RunApiGet(string apiRootUrl,string requestUrl)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiRootUrl);
                var result = client.GetStringAsync(requestUrl).Result;
                return result;
            }
        }
    }
}
