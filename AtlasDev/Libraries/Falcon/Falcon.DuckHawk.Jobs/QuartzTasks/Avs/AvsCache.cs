using System;
using System.Collections.Generic;
using System.Linq;
using Falcon.Common.Interfaces.Jobs;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Structures.AVS;
using Falcon.Common.Structures.Avs;
using Falcon.DuckHawk.Jobs.Attributes;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using StackExchange.Redis;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.Avs
{
  //[DisableJob]
  [DisallowConcurrentExecution]
  [JobName("AvsCache")]
  [TriggerName("AvsCache")]
  [CronExpression("0 0/2 5-21 ? * MON-SAT *")] // Mon-Sat 07:00 to 19:00, every 2 minutes (No online- no need for after-hours)
  public class AvsCache : IAvsCacheJob
  {
    private readonly IDatabase _redis;
    private readonly IAvsRepository _avsRepository;
    private readonly ILogger _logger;

    public AvsCache(IDatabase redis, IAvsRepository avsRepository, ILogger logger)
    {
      _redis = redis;
      _avsRepository = avsRepository;
      _logger = logger;
    }

    public void Execute(IJobExecutionContext context)
    {
      _logger.Information("Started AVS Job");
      var redisAvsKey = string.Format("falcon.avs.query.{0}.{1}", DateTime.Today.ToString("ddMMyyyy"), DateTime.Today.ToString("ddMMyyyy"));
      var lastStatusDate = DateTime.Today;

      // get transactions from Redis
      var result = new RedisValue();
      _logger.Information("AvsCache() - Execute - Key {key}", redisAvsKey);
      try
      {
        result = _redis.StringGet(redisAvsKey, CommandFlags.HighPriority);
      }
      catch (TimeoutException ex)
      {
        _logger.Fatal("AvsCache() - Redis Query - Key {key} - Exception {ex}", redisAvsKey, ex.StackTrace);
      }

      Tuple<List<AvsStatistics>, List<AvsTransactions>> cachedTransactions = null;

      if (!result.IsNull)
        cachedTransactions = JsonConvert.DeserializeObject<Tuple<List<AvsStatistics>, List<AvsTransactions>>>(result);

      // get last status date
      if (cachedTransactions != null)
        lastStatusDate = cachedTransactions.Item2.Max(t => t.LastStatusDate).AddMinutes(-1);

      // Get new transactions from DB
      var transactions = _avsRepository.GetTransactions(null, lastStatusDate, DateTime.Today, null, null, null);

      if (transactions.Count > 0)
      {
        // Exclude transactions already in the cache
        if (cachedTransactions != null && cachedTransactions.Item2.Count > 0)
        {
          var transactionsTemp = transactions;
          var uniqueTransactions = cachedTransactions.Item2.Where(c => !transactionsTemp.Select(t => t.TransactionId).Contains(c.TransactionId));
          transactions.AddRange(uniqueTransactions);
        }

        _logger.Information("AvsCache() - Execute - Key {key}, Set Cache", redisAvsKey);
        _redis.StringSet(redisAvsKey, JsonConvert.SerializeObject(new Tuple<List<IAvsStatistics>, List<IAvsTransactions>>(_avsRepository.GetStats(transactions), transactions)));
        _redis.KeyExpire(redisAvsKey, new TimeSpan(12, 0, 0));
      }

      _logger.Information("Finished AVS Job");
    }
  }
}
