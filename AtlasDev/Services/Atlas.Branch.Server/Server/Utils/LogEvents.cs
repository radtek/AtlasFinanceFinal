/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Thread-safe, in-memory logging
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *    
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;


namespace ASSSyncClient.Utils
{
  /// <summary>
  /// Class used to log events to server
  /// </summary>
  public static class LogEvents
  {
    /// <summary>
    /// Log a new event to the queue- thread-safe
    /// </summary>
    /// <param name="raisedDT">Event raised date/time</param>
    /// <param name="task">The task that caused the event</param>
    /// <param name="eventMessage">The message</param>
    /// <param name="severity">The severity</param>
    public static void Log(DateTime raisedDT, string task, string eventMessage, int severity)
    {
      // Limit the message portion to 100 chars, else slows down server unnecessarily- useful info should be contained within first 1000 chars
      if (eventMessage.Length > 100)
      {
        eventMessage = eventMessage.Substring(0, 100);
      }

      EventsPending.Enqueue(new EventToLog() { RaisedDT = raisedDT, Task = task, EventMessage = eventMessage, Severity = severity });

      // Avoid using too much memory/server error overflows
      while (EventsPending.Count > 100)
      {
        EventToLog tempItem;
        EventsPending.TryDequeue(out tempItem);
      }
    }


    /// <summary>
    /// Logs a list of events to the queue- thread-safe
    /// </summary>
    /// <param name="eventsToLog"></param>
    public static void Log(List<EventToLog> eventsToLog)
    {
      foreach (var eventToLog in eventsToLog)
      {
        Log(eventToLog.RaisedDT, eventToLog.Task, eventToLog.EventMessage, eventToLog.Severity);
      }
    }


    /// <summary>
    /// Get pending events- limited to first 100 events
    /// </summary>
    /// <param name="maxEvents">Maximum number of events to remove from queue and return- default is 100</param>
    /// <returns>List of EventLog- limited to first 100 events to avoid overflow when sending to WCF server</returns>
    public static List<EventToLog> GetPendingEvents(int maxEvents = 100)
    {
      var result = new List<EventToLog>();
      EventToLog eventToLog;
      while (EventsPending.TryDequeue(out eventToLog) && result.Count < maxEvents)
      {
        result.Add(eventToLog);
      }

      return (result.Count > 0) ? result : null;
    }
    

    public class EventToLog
    {
      public DateTime RaisedDT { get; set; }
      public string Task { get; set; }
      public string EventMessage { get; set; }
      public int Severity { get; set; }      
    }


    #region Private members

    private static readonly ConcurrentQueue<EventToLog> EventsPending = new ConcurrentQueue<EventToLog>();

    #endregion

  }
}