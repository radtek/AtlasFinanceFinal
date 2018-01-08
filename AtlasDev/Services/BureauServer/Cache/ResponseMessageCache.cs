using System;
using System.Collections.Generic;
using System.Linq;

using Atlas.Common.Utils;


namespace Atlas.Bureau.Server.Cache
{
  public static class ResponseMessageCache
  {
    private static readonly Dictionary<Guid, Dictionary<DateTime, dynamic>> cache = new Dictionary<Guid, Dictionary<DateTime, dynamic>>();

    private static readonly object synclock = new object();

    public static void Add(dynamic msg)
    {
      lock (synclock)
      {
        if (cache.ContainsKey(msg.CorrelationId))
        {
          cache[msg.CorrelationId].Add(DateTime.Now, msg);
        }
        else
        {
          var dictItem = new Dictionary<DateTime, dynamic>();
          dictItem.Add(DateTime.Now, msg);
          cache.Add(msg.CorrelationId, dictItem);
        }
      }
    }

    public static dynamic CheckItem(Guid guid)
    {
      lock (synclock)
      {
        if (cache.ContainsKey(guid))
          return cache[guid].FirstOrDefault().Value;
        return null;
      }
    }

    public static int Report()
    {
      lock (synclock)
      {
        return cache.Count();
      }
    }

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

    public static void Remove(Guid guid)
    {
      lock(synclock)
      {
        cache.Remove(guid);
      }
    }
  }
}
