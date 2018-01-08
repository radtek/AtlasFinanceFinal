using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using Thinktecture.IdentityModel.Web;

namespace Falcon.Web.Security.TokenCache.Providers
{
    /// <summary>
    /// MongoDB implementation for SessionSecurityTokenCache
    /// </summary>
    public class MongoTokenCacheRepository : ITokenCacheRepository
    {
        protected MongoDatabase Db;
        protected readonly MongoCollection<TokenCacheItem> Collection;
        public MongoTokenCacheRepository()
        {
            var connectionString = CommonConfig.MongoDBTokenCacheConnectionString;
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            Db = server.GetDatabase(new MongoUrl(connectionString).DatabaseName);
            Collection = Db.GetCollection<TokenCacheItem>(typeof(TokenCacheItem).Name);
        }
        public void AddOrUpdate(TokenCacheItem item)
        {
            Collection.Update(Query.EQ("key", item.Key), Update.Replace(item), UpdateFlags.Upsert);
        }
        public TokenCacheItem Get(string key)
        {
            return Collection.FindOneById(key);
        }
        public void Remove(string key)
        {
            Collection.Remove(Query.EQ("key", key));
        }

        public void RemoveAllBefore(DateTime date)
        {
            Collection.Remove(Query.LTE("Expires", date));
        }
    }
}