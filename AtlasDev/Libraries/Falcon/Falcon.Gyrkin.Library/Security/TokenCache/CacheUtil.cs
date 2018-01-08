using Microsoft.ApplicationServer.Caching;
using System.Collections.Generic;

namespace Falcon.Web.Security.TokenCache.Providers
{
    /// <summary>
    /// http://www.hanselman.com/blog/InstallingConfiguringAndUsingWindowsServerAppFabricAndTheVelocityMemoryCacheIn10Minutes.aspx
    /// http://msdn.microsoft.com/en-us/library/ee790981(v=azure.10).aspx
    /// http://jamesbroo.me/automatically-starting-the-windows-server-appfabric-caching-service/
    /// </summary>
    public class CacheUtil
    {
        private static DataCacheFactory _factory = null;
        private static DataCache _cache = null;

        public static DataCache GetCache()
        {
            if (_cache != null)
                return _cache;

            //-------------------------
            // Configure Cache Client 
            //-------------------------

            //Define Array for 1 Cache Host
            List<DataCacheServerEndpoint> servers = new List<DataCacheServerEndpoint>(1);

            //Specify Cache Host Details 
            servers.Add(new DataCacheServerEndpoint(CommonConfig.AppFabricCacheServerName, CommonConfig.AppFabricCachePort));

            //Create cache configuration
            DataCacheFactoryConfiguration configuration = new DataCacheFactoryConfiguration();

            //Set the cache host(s)
            configuration.Servers = servers;

            //Set default properties for local cache (local cache disabled)
            var properties = new DataCacheLocalCacheProperties();
            configuration.LocalCacheProperties = properties;
            //Disable tracing to avoid informational/verbose messages on the web page
            DataCacheClientLogManager.ChangeLogLevel(System.Diagnostics.TraceLevel.Off);

            //Pass configuration settings to cacheFactory constructor
            _factory = new DataCacheFactory(configuration);
            _cache = _factory.GetCache(CommonConfig.AppFabricCacheName);

            return _cache;
        }
    }
}
