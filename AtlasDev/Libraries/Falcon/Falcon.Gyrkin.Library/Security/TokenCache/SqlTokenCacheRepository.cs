using System;
using System.Data.Entity;
using System.Linq;
using Thinktecture.IdentityModel.Web;

namespace IdentityServer.Demo.Common.Security.TokenCache.Providers
{
    /// <summary>
    /// SQL Database implementation for SessionSecurityTokenCache
    /// </summary>
public class SqlTokenCacheRepository : ITokenCacheRepository
{
    public void AddOrUpdate(TokenCacheItem item)
    {
        using (LmsTokenCacheDataContext db = new LmsTokenCacheDataContext())
        {
            DbSet<TokenCacheItem> items = db.Set<TokenCacheItem>();
            var dbItem = items.Find(item.Key);
            if (dbItem == null)
            {
                dbItem = new TokenCacheItem();
                dbItem.Key = item.Key;
                items.Add(dbItem);
            }
            dbItem.Token = item.Token;
            dbItem.Expires = item.Expires;
            db.SaveChanges();
        }
    }

    public TokenCacheItem Get(string key)
    {
        using (LmsTokenCacheDataContext db = new LmsTokenCacheDataContext())
        {
            DbSet<TokenCacheItem> items = db.Set<TokenCacheItem>();
            return items.Find(key);
        }
    }

    public void Remove(string key)
    {
        using (LmsTokenCacheDataContext db = new LmsTokenCacheDataContext())
        {
            DbSet<TokenCacheItem> items = db.Set<TokenCacheItem>();
            var item = items.Find(key);
            if (item != null)
            {
                items.Remove(item);
                db.SaveChanges();
            }
        }
    }

    public void RemoveAllBefore(DateTime date)
    {
        using (LmsTokenCacheDataContext db = new LmsTokenCacheDataContext())
        {
            DbSet<TokenCacheItem> items = db.Set<TokenCacheItem>();
            var query =
                from item in items
                where item.Expires <= date
                select item;
            foreach (var item in query)
            {
                items.Remove(item);
            }
            db.SaveChanges();
        }
    }
}
}