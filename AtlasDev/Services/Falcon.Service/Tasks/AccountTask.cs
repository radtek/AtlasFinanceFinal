using System;
using System.Configuration;
using System.Text.RegularExpressions;
using Atlas.Common.Utils;
using Falcon.Service.Business;
using Falcon.Service.Core;
using Falcon.Common.Structures;
using Quartz;
using Serilog;

namespace Falcon.Service.Tasks
{
  [DisallowConcurrentExecution]
  public class AccountTask : IJob
  {
    private static readonly ILogger _log = Log.Logger.ForContext<AccountTask>();
    private static readonly string _accountCacheUpdate = ConfigurationManager.AppSettings["Account-Cache-Update"];
    private static readonly int _touchRedisAccountTtl = 5;
    private static readonly TimeSpan _redisAccountTtl = new TimeSpan(5, 0, 0);

    public void Execute(IJobExecutionContext context)
    {
      _log.Information("[FalconService][Task] {Job} Executing...", context.JobDetail.Key.Name);

      var redisAccountKey = string.Format("falcon.account.{0}", "*");
      var server = RedisConnection.GetServer();

      // Ideally only key scan replica or slave server, but due to our redis size,
      // we will accept the minor perfomance hit.
      var regex = new Regex(@"\d+");

      foreach (var key in server.Keys(pattern: redisAccountKey))
      {
        var result = RedisConnection.Current.GetDatabase().StringGet(key);

        if (!result.IsNullOrEmpty)
        {
          _log.Information("[FalconService][Task] {Job} Found Key [{Key}] in redis...", context.JobDetail.Key.Name, key);
          var match = regex.Match(key);
          if (match.Success)
          {
            var decompressedResult = GZipUtils.DecompressString(result);
            var accountCache = Xml.DeSerialize<AccountCache>(decompressedResult) as AccountCache;

            // Only bother updating the cache if its below the ttl in redis, otherwise let redis evict it.
            if (accountCache != null && DateTime.Now.Subtract(accountCache.TouchDate).Hours <= _touchRedisAccountTtl)
            {
              if (DateTime.Now.Subtract(accountCache.LastUpdateDate).Minutes >= int.Parse(_accountCacheUpdate))
              {
                _log.Information("[FalconService][Task] {Job} Key [{Key}] requires updating...",
                  context.JobDetail.Key.Name, key);
                var accountDetail = AccountUtility.GetAccountDetail(Int64.Parse(match.Value), true);

                if (accountDetail == null)
                {
                  _log.Warning(
                    "[FalconService][Task] {Job} Key [{Key}] object is missing from datastore, removing key.",
                    context.JobDetail.Key.Name, key);
                  RedisConnection.Current.GetDatabase().KeyDelete(key);
                  return;
                }
                accountDetail.CacheDate = DateTime.Now;
                var cache = new AccountCache(accountDetail, DateTime.Now, accountCache.TouchDate);
                var serialized = Xml.Serialize(cache);
                RedisConnection.Current.GetDatabase()
                  .StringSet(key, GZipUtils.CompressString(serialized), _redisAccountTtl);
                _log.Information("[FalconService][Task] {Job} Key [{Key}] Updated.", context.JobDetail.Key.Name, key);
              }
              else
              {
                _log.Information("[FalconService][Task] {Job} Key [{Key}] is up to date.", context.JobDetail.Key.Name,
                  key);
              }
            }
          }
        }
      }
      _log.Information("[FalconService][Task] {Job} Finished...", context.JobDetail.Key.Name);
    }
  }
}