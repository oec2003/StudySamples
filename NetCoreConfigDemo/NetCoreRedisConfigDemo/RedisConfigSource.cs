using System;
using Microsoft.Extensions.Configuration;

namespace NetCoreRedisConfigDemo
{
    public class RedisConfigSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new RedisConfigProvider();
        }
    }
}
