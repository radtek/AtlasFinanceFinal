using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using DevExpress.Xpo;
using Falcon.Service.Business;
using Falcon.Service.Controllers;
using Falcon.Service.Core;
using Falcon.Service.Structures;
using Quartz;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Falcon.Service.Tasks
{
  [DisallowConcurrentExecution]
  public class AvsAnalyticsTask : IJob
  {
    private static readonly ILogger _log = Log.Logger.ForContext<AvsAnalyticsTask>();
    private static readonly AnalyticsController avsStat = new AnalyticsController();

    public void Execute(IJobExecutionContext context)
    {
      _log.Information("[FalconService][Task] {Job} Executing...", context.JobDetail.Key.Name);

      var redisAvsKey = string.Format("falcon.avs.query.{0}.{1}", DateTime.Today.ToString("ddMMyyyy"), DateTime.Today.ToString("ddMMyyyy"));
      var lastStatusDate = DateTime.Today;

      // get transactions from Redis
      var cachedTransactions = RedisConnection.GetObjectFromString<Tuple<List<AvsStatistics>, List<AvsTransactions>>>(redisAvsKey);
      // get last status date
      if (cachedTransactions != null)
        lastStatusDate = cachedTransactions.Item2.Max(t => t.LastStatusDate).AddMinutes(-1);

      // Get new transactions from DB
      var avsutility = new AvsUtility();
      var transactions = avsutility.GetAvsTransactions(null, lastStatusDate, DateTime.Today, null, null, null);
      if (transactions.Count > 0)
      {
        if (cachedTransactions != null && cachedTransactions.Item2.Count > 0)
        {
          var uniqueTransactions = cachedTransactions.Item2.Where(c => !transactions.Select(t => t.TransactionId).Contains(c.TransactionId));
          transactions.AddRange(uniqueTransactions);
          uniqueTransactions = null;
        }

        // Update redis cache
        RedisConnection.SetStringFromObject<Tuple<List<AvsStatistics>, List<AvsTransactions>>>(redisAvsKey, new Tuple<List<AvsStatistics>, List<AvsTransactions>>(avsutility.GetStats(transactions), transactions), new TimeSpan(12, 0, 0));

        avsStat.SendForceRefresh();
      }
      avsutility = null;
      transactions = null;
      cachedTransactions = null;

      _log.Information("[FalconService][Task] {Job} Finished...", context.JobDetail.Key.Name);
    }
  }
}