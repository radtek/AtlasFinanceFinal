using Atlas.Common.Utils;
using Atlas.RabbitMQ.Messages.Credit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlas.ThirdParty.CompuScan
{
  public static class EnquiryCache
  {
    private static Dictionary<Guid, Dictionary<DateTime, Dictionary<Guid, CreditResponse>>> cache
      = new Dictionary<Guid, Dictionary<DateTime, Dictionary<Guid, CreditResponse>>>();

    private static object synclock = new object();

    public static void Add(CreditRequest msg)
    {
      lock (synclock)
      {
        Dictionary<DateTime, Dictionary<Guid, CreditResponse>> dictItem = new Dictionary<DateTime, Dictionary<Guid, CreditResponse>>();
        Dictionary<Guid, CreditResponse> subDictItem = new Dictionary<Guid, CreditResponse>();
        subDictItem.Add(msg.CorrelationId, null);
        dictItem.Add(DateTime.Now, subDictItem);
        cache.Add(msg.CorrelationId, dictItem);
      }
    }

    /// <summary>
    /// Updates cache with response.
    /// </summary>
    public static void Update(Guid guid, CreditRequest req, CreditResponse resp)
    {
      lock (synclock)
      {
        if (cache.ContainsKey(guid))
        {
          cache[guid][cache[guid].FirstOrDefault().Key][req.CorrelationId] = resp;
        }
      }
    }

    /// <summary>
    /// Checks if item exists in cache.
    /// </summary>
    public static DateTime? CheckItem(Guid guid)
    {
      lock (synclock)
      {
        if (cache.ContainsKey(guid))
          return cache[guid].FirstOrDefault().Key;
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
