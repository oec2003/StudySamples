using IdentityModel.Client;
using Newtonsoft.Json;
using Ocelot.Configuration.File;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();
        private static async Task MainAsync()
        {
            //需要修改的配置
            var configuration = new FileConfiguration
            {
                ReRoutes = new List<FileReRoute>
                   {
                       new FileReRoute
                       {
                           DownstreamPathTemplate = "/api/values",
                           DownstreamHostAndPorts = new List<FileHostAndPort>
                           {
                               new FileHostAndPort
                               {
                                   Host ="localhost",
                                   Port = 10001,
                               },
                               new FileHostAndPort
                               {
                                   Host ="localhost",
                                   Port = 10002,
                               }
                           },
                           DownstreamScheme = "http",
                           UpstreamPathTemplate = "/c/api/values",
                           UpstreamHttpMethod = new List<string> { "Get","Post" }
                       }
                   },
                GlobalConfiguration = new FileGlobalConfiguration
                {
                    BaseUrl = "http://localhost:10000/"
                }
            };

            // 从元数据中发现客户端
            var disco = await DiscoveryClient.GetAsync("http://localhost:9500");

            // 请求令牌
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("s2api");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            HttpContent content = new StringContent(JsonConvert.SerializeObject(configuration));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync("http://localhost:10000/admin/configuration", content);

            Console.WriteLine("update ocelot config sucess");
            Console.ReadLine();
        }
    }
}
