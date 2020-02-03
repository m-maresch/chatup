using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace NotificationService
{
    public static class RedisStore
    {
        private static readonly Lazy<ConnectionMultiplexer> userCacheConnection;
        private static readonly Lazy<ConnectionMultiplexer> messageCacheConnection;

        static RedisStore()
        {
            var userCacheConfigurationOptions = new ConfigurationOptions
            {
                EndPoints = { Startup.Configuration["userCacheUrl"] }
            };

            var messageCacheConfigurationOptions = new ConfigurationOptions
            {
                EndPoints = { Startup.Configuration["messageCacheUrl"] }
            };
            
            userCacheConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(userCacheConfigurationOptions));

            messageCacheConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(messageCacheConfigurationOptions));
        }

        public static ConnectionMultiplexer UserCacheConnection => userCacheConnection.Value;

        public static IDatabase UserCache => UserCacheConnection.GetDatabase();

        public static ConnectionMultiplexer MessageCacheConnection => messageCacheConnection.Value;

        public static IDatabase MessageCache => MessageCacheConnection.GetDatabase();


    }
}
