using System;
using Microsoft.Extensions.Configuration;

namespace NetCoreRedisConfigDemo
{

    public static class RedisConfigExtension
    {
        public static IConfigurationBuilder AddRedisConfig(this IConfigurationBuilder builder)
        {
            return builder.Add(new RedisConfigSource());
        }
    }
}
