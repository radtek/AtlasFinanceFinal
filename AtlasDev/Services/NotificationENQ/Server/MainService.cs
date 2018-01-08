/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2016 Atlas Finance (Pty) Ltd.
*
*  Description:
*  ------------------
*    Main service events
*
*
*  Author:
*  ------------------
*     Keith Blows
*
*
*  Revision history:
*  ------------------
*     2016-03-24  Created
*
*
* ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;

using Atlas.Common.Interface;
using NotificationServerENQ.Senders;
using Atlas.NotificationENQ.Dto;
using NotificationServerENQ.Db;
using NotificationServerENQ.MessageHandler;


namespace NotificationServerENQ
{
  internal class MainService
  {
    public MainService(ILogging log, IMessageBusHandler messageBus, IConfigSettings config,
      ISmsSender smsSender, IEmailSender emailSender, IDbRepository dbRepos, IMessageInProgressTracker tracker)
    {
      _config = config;
      _log = log;
      _messageBus = messageBus;
      _smsSender = smsSender;
      _emailSender = emailSender;
      _dbRepos = dbRepos;
      _tracker = tracker;
    }


    internal bool Start()
    {
      try
      {
        _log.Information("Starting...");

        // Start message bus
        _messageBus.Start();

        // Sending thread
        _checkPending = new Thread(() => HandlePending()) { IsBackground = true };
        _checkPending.Start();

        _log.Information("Started");

        return true;
      }
      catch (Exception err)
      {
        _log.Error(err, "Start()");
        return false;
      }
    }


    internal bool Stop()
    {
      try
      {
        _log.Information("Stopping...");

        _terminated.Set();
        _messageBus.Stop();
        Thread.Sleep(3000); // Give pending thread some time...

        _log.Information("Stopped");
      }
      catch (Exception err)
      {
        _log.Error(err, "Stop()");
      }

      return true;
    }


    /// <summary>
    /// Handles all unsent messages with Id overlap checking (IMessageInProgressTracker).
    /// Spawns a thread for each request.
    /// NOTE: The message bus will try send immediately from message bus handler- *** this is for retries ***
    /// </summary>
    /// 
    /// ISSUE: This is not scalable for multiple instances of this server- if multiple instances, 
    /// will have to cache (with persistence) the NotificationId's.
    /// 
    /// To scale, use Redis implementation for IMessageInProgressTracker _tracker with:
    /// (http://www.codeproject.com/Articles/1076448/Creating-an-in-memory-L-Cache-for-StackExchange-Re)
    /// 
    private void HandlePending()
    {
      while (!_terminated.Wait(30000))
      {
        #region Send pending emails
        try
        {
          var expireOlderThan = TimeSpan.FromHours(24);
          var pendingEmail = _dbRepos.GetAllPendingEmail(expireOlderThan);

          if (pendingEmail?.Count > 0)
          {
            _log.Information("Pending: Found {EMail} pending emails", pendingEmail.Count);
            var threads = new Thread[Math.Min(Environment.ProcessorCount, pendingEmail.Count)];
            var queue = new BlockingCollection<SendEmailMessageRequest>(pendingEmail.Count);
            foreach (var message in pendingEmail.Where(s => !_tracker.IsInProgress(s.RecId)))
            {
              queue.Add(message);
            }

            for (var i = 0; i < threads.Length; i++)
            {
              var thread = threads[i] = new Thread(async () =>
              {
                SendEmailMessageRequest message;
                if (queue.TryTake(out message))
                {
                  if (!_tracker.IsInProgress(message.RecId))
                  {
                    _tracker.SetInProgress(message.RecId, true);
                    try
                    {
                      var status = await _emailSender.Send(_log, _config, message).ConfigureAwait(false);
                      _dbRepos.UpdateStatus(message.RecId, status.Item1, expireOlderThan, status.Item1 ? 1 : 0, status.Item2);
                    }
                    finally
                    {
                      _tracker.SetInProgress(message.RecId, false);
                    }
                  }
                }
              });

              thread.Start(queue);
            }
          }
        }
        catch (Exception err)
        {
          _log.Error(err, "CheckCached() Email");
        }
        #endregion

        #region Send pending SMS        
        try
        {
          var expireOlderThan = TimeSpan.FromMinutes(15);
          var pendingSms = _dbRepos.GetAllPendingSms(expireOlderThan);

          if (pendingSms?.Count > 0)
          {
            _log.Information("Pending: Found {SMS} pending SMS", pendingSms.Count);
            var threads = new Thread[Math.Min(Environment.ProcessorCount, pendingSms.Count)];
            var queue = new BlockingCollection<SendSmsMessageRequest>(pendingSms.Count);
            foreach (var message in pendingSms.Where(s => !_tracker.IsInProgress(s.RecId)))
            {
              queue.Add(message);
            }

            for (var i = 0; i < threads.Length; i++)
            {
              var thread = threads[i] = new Thread(async (_) =>
              {
                SendSmsMessageRequest message;
                if (queue.TryTake(out message))
                {
                  if (!_tracker.IsInProgress(message.RecId))
                  {
                    _tracker.SetInProgress(message.RecId, true);
                    try
                    {
                      var status = await _smsSender.Send(_log, _config, message).ConfigureAwait(false);
                      _dbRepos.UpdateStatus(message.RecId, status.Item1 > 0, expireOlderThan, status.Item1, status.Item2);
                    }
                    finally
                    {
                      _tracker.SetInProgress(message.RecId, false);
                    }
                  }
                }
              });

              thread.Start(queue);
            }
          }
        }
        catch (Exception err)
        {
          _log.Error(err, "CheckCached() Sms");
        }
        #endregion
      }
    }

    /// <summary>
    /// Thread to retry failed messages
    /// </summary>
    private Thread _checkPending;

    /// <summary>
    /// Indicate to thread we want to terminate
    /// </summary>
    private readonly ManualResetEventSlim _terminated = new ManualResetEventSlim();


    // Injected
    private readonly ILogging _log;
    private readonly IMessageBusHandler _messageBus;
    private readonly ISmsSender _smsSender;
    private readonly IEmailSender _emailSender;
    private readonly IDbRepository _dbRepos;
    private readonly IConfigSettings _config;
    private readonly IMessageInProgressTracker _tracker;

  }
}