using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;

namespace WebAPIGetway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();

            Console.ReadLine();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                //add ocelot json config file
                .ConfigureAppConfiguration((hostingContext, builder) =>
                {
                    builder
                    .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("Ocelot.json")
                    .AddEnvironmentVariables();
                })
                .UseStartup<Startup>()
                .UseUrls("http://*:10000")
                .Build();
    }
}
