using System;
using System.Web;
using Thinktecture.IdentityModel.Web;

namespace Falcon.Gyrkin.Library.Security.TokenCache.Providers
{
  /// <summary>
  /// ASP.NET Session based implementation for SessionSecurityTokenCache
  /// </summary>
  public class SessionTokenCacheRepository : ITokenCacheRepository
  {
    public SessionTokenCacheRepository()
    {

    }

    public void AddOrUpdate(TokenCacheItem item)
    {
      HttpContext.Current.Session[item.Key] = item;
    }

    public TokenCacheItem Get(string key)
    {
      return HttpContext.Current.Session != null ? HttpContext.Current.Session[key] as TokenCacheItem : null;
    }

    public void Remove(string key)
    {
      HttpContext.Current.Session.Remove(key);
    }

    public void RemoveAllBefore(DateTime date)
    {
      //not possible to implement for session based cache
    }
  }
}