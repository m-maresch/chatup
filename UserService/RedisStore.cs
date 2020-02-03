using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace UserService
{
    public static class RedisStore
    {
        private static readonly Lazy<ConnectionMultiplexer> cacheConnection;

        static RedisStore()
        {
            var cacheConfigurationOptions = new ConfigurationOptions
            {
                EndPoints = { Startup.Configuration["cacheUrl"] }
            };
            
            cacheConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(cacheConfigurationOptions));
        }

        public static ConnectionMultiplexer CacheConnection => cacheConnection.Value;

        public static IDatabase Cache => CacheConnection.GetDatabase();
    }
}
