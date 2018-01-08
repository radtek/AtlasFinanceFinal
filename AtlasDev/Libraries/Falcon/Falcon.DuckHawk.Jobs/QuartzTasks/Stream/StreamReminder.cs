using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using Falcon.Common.Interfaces.Jobs;
using Falcon.DuckHawk.Jobs.Attributes;
using Microsoft.AspNet.SignalR.Client;
using Quartz;
using Serilog;
using StackExchange.Redis;
using Stream.Framework.Repository;
using LongPollingTransport = Microsoft.AspNet.SignalR.Client.Transports.LongPollingTransport;
using WebSocket = Atlas.Enumerators.WebSocket;

namespace Falcon.DuckHawk.Jobs.QuartzTasks.Stream
{
  //[DisableJob]
  [DisallowConcurrentExecution]
  [JobName("StreamReminderJob")]
  [TriggerName("StreamReminderJob")]
  [ScheduleBuilder]
  public sealed class StreamReminder : IStreamReminder
  {
    public IScheduleBuilder Schedule
    {
      get
      {
        return SimpleScheduleBuilder.Create().WithIntervalInSeconds(15).RepeatForever().WithMisfireHandlingInstructionFireNow();
      }
    }

    private readonly IDatabase _redis;
    private readonly ILogger _logger;
    private readonly HubConnection _hubConnection;
    private readonly IStreamRepository _streamRepository;

    public StreamReminder(IDatabase redis, ILogger logger, HubConnection hubConnection, IStreamRepository streamRepository)
    {
      _logger = logger;
      _redis = redis;
      _hubConnection = hubConnection;
      _streamRepository = streamRepository;
    }

    /// <summary>
    /// Empty constructor for Instance Activator
    /// </summary>
    public StreamReminder()
    {
    }

    public void Execute(IJobExecutionContext context)
    {
      try
      {
        var proxy = _hubConnection.CreateHubProxy("general");
        _hubConnection.Start(new LongPollingTransport()).Wait();

        var subscribedGroupCollection = proxy.Invoke<Dictionary<WebSocket.SubscriptionChannel, List<Guid>>>("GetSubscribedUsers").Result;

        if (subscribedGroupCollection.Count == 0)
        {
          _hubConnection.Dispose();
          return;
        }

        foreach (var group in subscribedGroupCollection)
        {
          var groupType = group.Key == WebSocket.SubscriptionChannel.Collections ? global::Stream.Framework.Enumerators.Stream.GroupType.Collections : global::Stream.Framework.Enumerators.Stream.GroupType.Sales;

          var caseSummaries = groupType == global::Stream.Framework.Enumerators.Stream.GroupType.Collections ?
            _streamRepository.GetWorkItemsSummary(groupType, group.Value.Select(c=>c.ToString()).ToList(), global::Stream.Framework.Enumerators.Stream.StreamType.New, global::Stream.Framework.Enumerators.Stream.StreamType.PTPBroken,
            global::Stream.Framework.Enumerators.Stream.StreamType.FollowUp, global::Stream.Framework.Enumerators.Stream.StreamType.PTP) :
            _streamRepository.GetWorkItemsSummary(groupType, group.Value.Select(c => c.ToString()).ToList(), global::Stream.Framework.Enumerators.Stream.StreamType.New, global::Stream.Framework.Enumerators.Stream.StreamType.FollowUp,
            global::Stream.Framework.Enumerators.Stream.StreamType.PTC, global::Stream.Framework.Enumerators.Stream.StreamType.PTCBroken);
          
          foreach (var user in group.Value)
          {
            _logger.Information("[StreamReminder] - Retrieving items for user {user} in channel {channel}", user, groupType);

            if (caseSummaries.Count > 0)
            {
              if (groupType == global::Stream.Framework.Enumerators.Stream.GroupType.Collections)
                proxy.Invoke("Notify", WebSocket.SubscriptionChannel.Collections, "collections-notification-popup", user, caseSummaries[user.ToString()].ToJSON());
              else
                proxy.Invoke("Notify", WebSocket.SubscriptionChannel.Sales, "sales-notification-popup", user, caseSummaries[user.ToString()].ToJSON());
            }
            else
            {
              if (groupType == global::Stream.Framework.Enumerators.Stream.GroupType.Collections)
                proxy.Invoke("Notify", WebSocket.SubscriptionChannel.Collections, "collections-no-tasks", user, "No pending tasks");
              else
                proxy.Invoke("Notify", WebSocket.SubscriptionChannel.Sales, "sales-no-tasks", user, "No pending tasks");

            }
          }
          _logger.Information("Stream Job - Sent work...");
        }
      }
      catch (Exception ex)
      {
        _logger.Fatal(ex.Message);
      }

      _hubConnection.Dispose();
    }
  }
}