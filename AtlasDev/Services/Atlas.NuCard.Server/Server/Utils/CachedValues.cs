#region Using

using Serilog;
using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;

#endregion

namespace Atlas.Server.NuCard.Utils
{
  /// <summary>
  /// Class to cache values to avoid hitting the DB unnecessarily
  /// </summary>
  public static class CachedValues
  {
    /// <summary>
    /// Get the Tutuka endpoint
    /// </summary>
    public static string TutukaEndpoint
    {
      get
      {
        lock (_access)
        {
          if (MemoryCache.Default[Tutuka_ENDPOINT_KEY] == null)
          {
            var endpoint = ConfigurationManager.AppSettings["NuCardEndpoint"] ?? "http://voucherengine.tutuka.com/handlers/remote/profilexmlrpc.cfm";
            MemoryCache.Default.Add(Tutuka_ENDPOINT_KEY, endpoint, new DateTimeOffset(DateTime.Now.AddMinutes(CACHE_DEFAULT_VALIDITY_MINUTES)));

            Log.Information("System now Using cached Tutuka endpoint: {0}", (string)MemoryCache.Default[Tutuka_ENDPOINT_KEY]);
          }

          return (string)MemoryCache.Default[Tutuka_ENDPOINT_KEY];
        }
      }
    }


    /// <summary>
    /// Determines if a transaction is currently in progress and adds if not
    /// </summary>
    /// <param name="transactionId">The unique transaction id to check</param>
    /// <param name="expireAfter">The amount of time to expire the transaction</param>
    /// <returns>true if given transaction id is currently in progress, false if no record of transaction id</returns>
    public static bool TransInProgress(string transactionId, TimeSpan expireAfter)
    {
      if (!_trackTransIds.ContainsKey(transactionId))
      {
        _trackTransIds.TryAdd(transactionId, DateTime.Now);
        return false;
      }
      else // exists, check for transaction expiry
      {
        var added = _trackTransIds[transactionId];
        if (DateTime.Now.Subtract(added) > expireAfter) // has expired?
        {
          _trackTransIds[transactionId] = DateTime.Now;
          return false;
        }

        return true;
      }
    }


    /// <summary>
    /// Sets that the specific transaction id has completed
    /// </summary>
    /// <param name="transactionId">The unique transaction id to mark as complete</param>
    public static void TransCompleted(string transactionId)
    {
      DateTime value;
      _trackTransIds.TryRemove(transactionId, out value);
    }


    /// <summary>
    /// Determines if adding a new card number is in progress and adds if not
    /// </summary>
    /// <param name="transactionId">The unique transaction id to check</param>
    /// <param name="expireAfter">The amount of time to expire the transaction</param>
    /// <returns>true if given transaction id is currently in progress, false if no record of transaction id</returns>
    public static bool CardImportInProgres(string cardNum, TimeSpan expireAfter)
    {
      if (!_trackCardNums.ContainsKey(cardNum))
      {
        _trackCardNums.TryAdd(cardNum, DateTime.Now);
        return false;
      }
      else // exists, check for transaction expiry
      {
        var added = _trackCardNums[cardNum];
        if (DateTime.Now.Subtract(added) > expireAfter) // has expired?
        {
          _trackCardNums[cardNum] = DateTime.Now;
          return false;
        }

        return true;
      }
    }


    /// <summary>
    /// Sets that the specific card number add has completed
    /// </summary>
    /// <param name="transactionId">The unique transaction id to mark as complete</param>
    public static void CardImportCompleted(string cardNum)
    {
      DateTime value;
      _trackCardNums.TryRemove(cardNum, out value);
    }


    #region Private consts

    private static object _access = new object();

    /// <summary>
    /// The unique key to use for the Tutuka endpoint Cache item
    /// </summary>
    private const string Tutuka_ENDPOINT_KEY = "TutukaEndpoint";

    /// <summary>
    /// Default cache item validity in minutes- 120 minutes
    /// </summary>
    private const int CACHE_DEFAULT_VALIDITY_MINUTES = 120;

    private static ConcurrentDictionary<string, DateTime> _trackTransIds = new ConcurrentDictionary<string, DateTime>();

    private static ConcurrentDictionary<string, DateTime> _trackCardNums = new ConcurrentDictionary<string, DateTime>();

    #endregion

  }
}
