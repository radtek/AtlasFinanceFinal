using Microsoft.ApplicationServer.Caching;
using System;
using System.Diagnostics;
using Thinktecture.IdentityModel.Web;

namespace Falcon.Web.Security.TokenCache.Providers
{
    /// <summary>
    /// AppFabric implementation for SessionSecurityTokenCache
    /// </summary>
    public class AppFabricTokenCacheRepository : ITokenCacheRepository
    {
        private DataCache m_cache = null;
        public AppFabricTokenCacheRepository()
        {
            m_cache = CacheUtil.GetCache();
        }
        public void AddOrUpdate(TokenCacheItem item)
        {
            m_cache.Put(item.Key, item, item.Expires - DateTime.Now);
        }
        public TokenCacheItem Get(string key)
        {
            return m_cache.Get(key) as TokenCacheItem;
        }
        public void Remove(string key)
        {
            m_cache.Remove(key);
        }

        public void RemoveAllBefore(DateTime date)
        {
            //no need to implement this method i think as AppFabric will take care of deletion
            Debug.Print("Deleting tokens");
            //m_cache.RemoveRegion(date.ToString());
        }
    }
}