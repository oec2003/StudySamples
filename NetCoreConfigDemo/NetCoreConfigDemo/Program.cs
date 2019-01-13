using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
namespace NetCoreConfigDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var dic = new Dictionary<string, string>() { { "name","oec2003"}};
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection(dic)
                .AddJsonFile("App.json")
                .AddCommandLine(args);
            
            var configration = builder.Build();
            Console.WriteLine($"name:{configration["name"]}");

            Console.ReadLine();
        }
    }
}
