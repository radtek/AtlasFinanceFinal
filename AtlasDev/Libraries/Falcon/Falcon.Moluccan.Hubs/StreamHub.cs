using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Common.Extensions;
using Autofac;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Serilog;

namespace Falcon.Moluccan.Hubs
{

  [HubName("stream")]
  public class StreamHub : Hub
  {
    private static readonly ConcurrentDictionary<HubType?, ConcurrentDictionary<Guid, Guid?>> _connections = new ConcurrentDictionary<HubType?, ConcurrentDictionary<Guid, Guid?>>();

    private readonly ILogger _logger;
    private readonly ILifetimeScope _hubLife;
    internal enum Dialog
    {
      [Description("allocateDialog")]
      AllocateDialog,
      [Description("reminderDialog")]
      ReminderDialog
    }

    public enum HubType
    {
      [Description("Collections")]
      Collections = 1,
      [Description("Sales")]
      Sales = 2,
      [Description("Management")]
      Management = 3
    }

    public StreamHub(ILifetimeScope lifetime)
    {
      _hubLife = lifetime.BeginLifetimeScope();
      _logger = _hubLife.Resolve<ILogger>();
    }

    public override Task OnConnected()
    {
      _logger.Information("Client connected {connectionId} at {time}", Context.ConnectionId, DateTime.Now);

      return base.OnConnected();
    }

    public void Subscribe(Guid userId, Guid connectionId, HubType hubType)
    {
      ConcurrentDictionary<Guid, Guid?> channel;

      _connections.TryGetValue(hubType, out channel);

      if (channel != null)
      {
        Guid? id;
        channel.TryGetValue(connectionId, out id);

        if (id == null)
        {
          // Clean up dictionary for stagnant users in connection pool.
          var stagnantConnections = channel.Where(p => p.Value == userId && p.Key != connectionId);

          stagnantConnections.ToList().ForEach(e =>
          {
            Guid? stagId;
            channel.TryRemove(e.Key, out stagId);
          });

          // Add new connection to pool.
          var connnection = AddConnection(connectionId, userId);
          _connections.AddOrUpdate(hubType, connnection, (s, i) => connnection);
        }
      }
      else
      {
        _connections.TryAdd(hubType, AddConnection(connectionId, userId));
      }
    }

    public void Notify(string time)
    {
      _logger.Information("Stream Hub: Notify for user {0}", time);
      Clients.All.alert("test");
    }

    /// <summary>
    /// Get a list of subscribed users.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Guid> GetSubscribedUsers(long hubType)
    {
      var subscribers = _connections.FirstOrDefault(p => p.Key == ((HubType)hubType));
      return subscribers.Key != null ? subscribers.Value.Select(p => p.Value).OfType<Guid>().Distinct() : new List<Guid>();
    }

    public void ClientAlert(int hubType, Guid userId, string caseStreamAction)
    {
      _logger.Information("[StreamHub][ClientAlert] Notification for {alertType} for user {user}", ((HubType)hubType).ToStringEnum(), userId);

      var connectionPool = _connections.FirstOrDefault(p => p.Key == ((HubType)hubType));

      if(connectionPool.Key != null)
      {
        var connections = connectionPool.Value.Where(p => p.Value != null && p.Value.Value == userId);
        connections.ToList().ForEach(e =>
        {
          Clients.Client(e.Key.ToString()).notify_dialog(caseStreamAction, Dialog.AllocateDialog.ToStringEnum());
        });
      }
    }

    /// <summary>
    /// Alert user they have no pending reminders.
    /// </summary>
    /// <param name="hubType"></param>
    /// <param name="userId"></param>
    public void NoPendingTasks(long hubType, Guid userId)
    {
      _logger.Information("StreamHub: No tasks for user {user}...", userId);
       var connectionPool = _connections.FirstOrDefault(p => p.Key == ((HubType)hubType));

      if(connectionPool.Key != null)
      {
        var connections = connectionPool.Value.Where(p => p.Value != null && p.Value.Value == userId);
        connections.ToList().ForEach(e =>
        {
          Clients.Client(e.Key.ToString()).no_tasks(true);
        });
      }
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && _hubLife != null)
        _hubLife.Dispose();
      base.Dispose(disposing);
    }


    #region Internal Methods

    /// <summary>
    /// Add the user to the pool
    /// </summary>
    internal ConcurrentDictionary<Guid, Guid?> AddConnection(Guid connectionId, Guid userId)
    {
      var item = new ConcurrentDictionary<Guid, Guid?>();
      item.TryAdd(connectionId, userId);
      return item;
    }

    #endregion
  }
}
