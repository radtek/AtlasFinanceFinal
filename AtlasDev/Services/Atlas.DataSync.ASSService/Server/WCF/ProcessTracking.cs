using System;
using System.Runtime.Caching;


namespace ASSServer.WCF
{  
  /// <summary>
  /// Tracks status of uploads, using MemoryCache
  /// </summary>
  public static class ProcessTracking
  {
    /// <summary>
    /// Adds/updates a Transaction
    /// </summary>
    /// <param name="transactionId">The Transaction GUID</param>
    /// <param name="busy">Are we still busy with this transaction?</param>
    /// <param name="errorMessage">Any error associated with this transaction?</param>
    public static void SetTransactionState(string transactionId, CurrentStatus status, string errorMessage = null, string fileName = null)
    {
      var current = _cache[transactionId] as TransactionStatus;
      if (current == null)
      {
        var cp = new CacheItemPolicy() { AbsoluteExpiration = DateTime.UtcNow.Add(TimeSpan.FromMinutes(90)) };
        var newItem = new CacheItem(transactionId, new TransactionStatus() { Status = status, ErrorMessage = errorMessage, Filename = fileName });
        if (!string.IsNullOrEmpty(errorMessage))
        {
          current.ErrorMessage = errorMessage;
        }
        _cache.Add(newItem, cp);
      }
      else
      {
        if (status != CurrentStatus.NotSet)
        {
          current.Status = status;
        }

        if (!string.IsNullOrEmpty(errorMessage))
        {
          current.ErrorMessage = errorMessage;
        }
        if (!string.IsNullOrEmpty(fileName))
        {
          current.Filename = fileName;
        }
        _cache[transactionId] = current;
      }
    }


    /// <summary>
    /// Gets the state of a transaction
    /// </summary>
    /// <param name="transactionId">The transaction GUID</param>
    /// <param name="busy">Are we still busy this transaction</param>
    /// <param name="errorMessage">Was there any error with this transaction</param>
    /// <returns>true if transactionId was found, else false</returns>
    public static bool GetTransactionState(string transactionId, out CurrentStatus status, out string errorMessage, out string fileName)
    {
      status = CurrentStatus.NotSet;
      errorMessage = null;
      fileName = null;

      var current = _cache[transactionId] as TransactionStatus;
      if (current == null)
      {
        errorMessage = "Unable to locate transaction";
        return false;
      }
      else
      {
        status = current.Status;
        errorMessage = current.ErrorMessage;
        fileName = current.Filename;

        return true;
      }
    }


    public enum CurrentStatus
    {
      Completed,
      Failed,
      NotSet,
      Started
    };


    /// <summary>
    /// Class used to track status requests
    /// </summary>
    private class TransactionStatus
    {
      public CurrentStatus Status { get; set; }
      public string ErrorMessage { get; set; }
      public string Filename { get; set; }
    }

    /// <summary>
    /// The memory cache to store transaction status
    /// </summary>
    private static readonly MemoryCache _cache = new MemoryCache("UploadTransactions");

  }
}
