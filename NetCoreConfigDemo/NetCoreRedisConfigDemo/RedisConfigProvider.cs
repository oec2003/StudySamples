using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
namespace NetCoreRedisConfigDemo
{
    public class RedisConfigProvider:ConfigurationProvider
    {
        public RedisConfigProvider()
        {
        }
        public override void Load()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                {"HostName","localhost"},
                {"Port","6379"},
                {"Password","123456"}
            };

            Data = dic;
        }
    }
}
