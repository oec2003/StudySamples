using System;
using Microsoft.Extensions.Configuration;

namespace NetCoreRedisConfigDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddRedisConfig();
            var configration = builder.Build();
            Console.WriteLine($"HostName:{configration["HostName"]}");
            Console.WriteLine($"Port:{configration["Port"]}");
            Console.WriteLine($"Password:{configration["Password"]}");
            Console.ReadLine();
        }
    }
}
