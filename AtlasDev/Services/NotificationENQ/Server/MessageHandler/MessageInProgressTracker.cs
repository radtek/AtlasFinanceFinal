using System;
using System.Collections.Concurrent;


namespace NotificationServerENQ.MessageHandler
{
  /// <summary>
  /// Implements "message sending in progress" tracking, using a ConcurrentDictionary
  /// Replace with Redis for distributed solution
  /// </summary>
  internal class MessageInProgressTracker : IMessageInProgressTracker
  {
    public bool IsInProgress(long recId)
    {
      return _busyWith.ContainsKey(recId);
    }


    public void SetInProgress(long recId, bool inProgress)
    {
      if (inProgress)
      {
        _busyWith.TryAdd(recId, DateTime.Now);
      }
      else
      {
        DateTime dummy;
        _busyWith.TryRemove(recId, out dummy);
      }
    }


    private static readonly ConcurrentDictionary<long, DateTime> _busyWith = new ConcurrentDictionary<long, DateTime>();

  }

}
