using System;
using System.Threading.Tasks;
using Atlas.Common.Utils;
using Falcon.Service.Business;
using Falcon.Service.Core;
using Falcon.Common.Structures;
using StackExchange.Redis;

namespace Falcon.Service.Helpers
{
  public static class AccountCacher
  {
    public static Tuple<bool, AccountDetail> Read(long accountId)
    {
      var instance = RedisConnection.Current.GetDatabase();
      var keyValue = instance.StringGet(string.Format("falcon.account.{0}", accountId));

      if (keyValue.IsNullOrEmpty)
        return new Tuple<bool, AccountDetail>(false, null);

      var decompressedResult = GZipUtils.DecompressString(keyValue);
      var accountCache = Xml.DeSerialize<AccountCache>(decompressedResult) as AccountCache;

      return new Tuple<bool, AccountDetail>(true, accountCache == null ? null : accountCache.AccountDetail);
    }

    public static void Cache(AccountDetail accountDetail)
    {
      var instance = RedisConnection.Current.GetDatabase();

      var keyValue = instance.StringGet(string.Format("falcon.account.{0}", accountDetail.AccountId));

      if (keyValue.IsNullOrEmpty)
      {
        accountDetail.CacheDate = DateTime.Now;

        var cache = new AccountCache(accountDetail, DateTime.Now, DateTime.Now);
        var serialized = Xml.Serialize(cache);

        instance.StringSet(string.Format("falcon.account.{0}", accountDetail.AccountId), GZipUtils.CompressString(serialized), new TimeSpan(5, 0, 0));
      }
    }

    public static async Task<bool> ForceUpdate(long accountId)
    {
      var redisKey = string.Format("falcon.account.{0}", accountId);

      var cachedAccount = await RedisConnection.Current.GetDatabase().StringGetAsync(redisKey);

      if (!cachedAccount.IsNullOrEmpty)
      {
        var cacheAccount = GetCache(cachedAccount);
        // Get latest account information
        var accountDetail = AccountUtility.GetAccountDetail(accountId, true);
        // Update cache date.
        accountDetail.CacheDate = DateTime.Now;
        // Create new cache object for storage
        AccountCache cache = new AccountCache(accountDetail, DateTime.Now, cacheAccount.TouchDate);
        // Serial.
        var serialized = Xml.Serialize(cache);
        // Store with ttl 5 hours
        return await RedisConnection.Current.GetDatabase().StringSetAsync(redisKey, GZipUtils.CompressString(serialized), new TimeSpan(5, 0, 0));
      }
      return true;
    }

    private static AccountCache GetCache(RedisValue redisValue)
    {
      var decompressedResult = GZipUtils.DecompressString(redisValue);
      return Xml.DeSerialize<AccountCache>(decompressedResult) as AccountCache;
    }
  }
}
