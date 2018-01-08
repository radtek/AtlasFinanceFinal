using Atlas.Common.Utils;
using Atlas.RabbitMQ.Messages.Fraud;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.ThirdParty.Fraud.Cache
{
  public static class EnquiryCache
  {
    private static Dictionary<Guid, Dictionary<DateTime, Dictionary<FraudRequest, FraudResponse>>> cache
      = new Dictionary<Guid, Dictionary<DateTime, Dictionary<FraudRequest, FraudResponse>>>();

    private static object synclock = new object();

    public static void Add(FraudRequest msg)
    {
      lock (synclock)
      {
        Dictionary<DateTime, Dictionary<FraudRequest, FraudResponse>> dictItem = new Dictionary<DateTime, Dictionary<FraudRequest, FraudResponse>>();
        Dictionary<FraudRequest, FraudResponse> subDictItem = new Dictionary<FraudRequest, FraudResponse>();
        subDictItem.Add(msg, null);
        dictItem.Add(DateTime.Now, subDictItem);
        cache.Add(msg.CorrelationId, dictItem);
      }
    }

    /// <summary>
    /// Updates cache with response.
    /// </summary>
    public static void Update(Guid guid, FraudRequest req, FraudResponse resp)
    {
      lock (synclock)
      {
        if (cache.ContainsKey(guid))
        {
          cache[guid][cache[guid].FirstOrDefault().Key][req] = resp;
        }
      }
    }

    /// <summary>
    /// Checks if item exists in cache.
    /// </summary>
    public static FraudResponse CheckItem(Guid guid)
    {
      lock (synclock)
      {
        if (cache.ContainsKey(guid))
          return cache[guid][cache[guid].FirstOrDefault().Key].FirstOrDefault().Value;
        return null;
      }
    }

    /// <summary>
    /// Performs basic reporting on amount of items in cache
    /// </summary>
    public static int Report()
    {
      lock (synclock)
      {
        return cache.Count();
      }
    }

    /// <summary>
    /// House cleaning function to remove objects older than two days.
    /// </summary>
    public static void Clean()
    {
      lock (synclock)
      {
        var itemsOlder = cache.Where(_ => new DateDifference(_.Value.FirstOrDefault().Key, DateTime.Now).Days >= 2).ToList();

        foreach (var itm in itemsOlder)
        {
          cache.Remove(itm.Key);
        }
      }
    }
  }
}
