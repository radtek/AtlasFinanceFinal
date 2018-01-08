using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Enumerators;
using Autofac;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Serilog;

namespace Falcon.Moluccan.Hubs
{
  [HubName("general")]
  public class GeneralHub : Hub
  {
    private static readonly ConcurrentDictionary<WebSocket.SubscriptionChannel?, ConcurrentDictionary<Guid, Guid?>> _connections = new ConcurrentDictionary<WebSocket.SubscriptionChannel?, ConcurrentDictionary<Guid, Guid?>>();
    private readonly ILogger _logger;
    private readonly ILifetimeScope _hubLife;

    public GeneralHub(ILifetimeScope lifetime)
    {
      _hubLife = lifetime.BeginLifetimeScope();
      _logger = _hubLife.Resolve<ILogger>();
    }

    public override Task OnConnected()
    {
      _logger.Information("Client connected {connectionId} at {time}", Context.ConnectionId, DateTime.Now);

      return base.OnConnected();
    }

    public override Task OnDisconnected(bool stopCalled)
    {
      var subs = _connections.ToList();

      foreach (var sub in subs)
      {
        Guid? connectionId;
        sub.Value.TryRemove(new Guid(Context.ConnectionId), out connectionId);

        if (connectionId != null)
          _logger.Information("Stale connection {connectionId} from channel {channel} has been removed.", Context.ConnectionId, sub.Key.ToString());
      }

      return base.OnDisconnected(stopCalled);
    }

    /// <summary>
    /// Add user to channel subscription
    /// </summary>
    public void SubscribeChannel(Guid userId, Guid connectionId, WebSocket.SubscriptionChannel subscriptionChannel)
    {
      ConcurrentDictionary<Guid, Guid?> channel;

      _connections.TryGetValue(subscriptionChannel, out channel);

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
          _connections.AddOrUpdate(subscriptionChannel, connnection, (s, i) => connnection);
          _logger.Information("Connection {connectionId} has suscribed to channel {channelsub}", connectionId, subscriptionChannel);
        }
      }
      else
      {
        _connections.TryAdd(subscriptionChannel, AddConnection(connectionId, userId));
        _logger.Information("Connection {connectionId} has suscribed to channel {channelsub}", connectionId, subscriptionChannel);
      }
    }

    /// <summary>
    /// Remove a particular subscribe from a socket channel
    /// </summary>
    public void UnsubscribeChannel(Guid userId, Guid connectionId, WebSocket.SubscriptionChannel subscriptionChannel)
    {
      ConcurrentDictionary<Guid, Guid?> channel;

      _connections.TryGetValue(subscriptionChannel, out channel);

      if (channel != null)
      {
        Guid? id;
        channel.TryGetValue(connectionId, out id);

        if (id != null)
        {
          if (channel.TryRemove(connectionId, out id))
            _logger.Information("Connection {connectionId} has unsuscribed from channel {channelsub}", connectionId, subscriptionChannel);
        }
      }
    }

    public void Notify(WebSocket.SubscriptionChannel channel, string broadcastSubscriber, Guid userId, string data)
    {
      var connectionPool = _connections.FirstOrDefault(p => p.Key == channel);

      if (connectionPool.Key != null)
      {
        var connections = connectionPool.Value.Where(p => p.Value != null && p.Value.Value == userId);
        connections.ToList().ForEach(e =>
        {
          Clients.Client(e.Key.ToString()).notify(channel, broadcastSubscriber, data);
        });
      }
    }

    /// <summary>
    /// Checks to see if the client is still busy.
    /// 
    /// Use:
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public bool IsBusy(WebSocket.SubscriptionChannel channel, Guid userId)
    {
      //var connectionPool = _connections.FirstOrDefault(p => p.Key == channel);

      //if(connectionPool.Key != null)
      //{
      //  // See if we have a user listening for this particular channel.
      //  var connection = connectionPool.Value.FirstOrDefault(p => p.Value.Value == userId);

      //  if (connection.Key != null)
      //  {
      //    // Check to see if the client has started eventing busy statuses.
      //    var busy = _busyConnections.FirstOrDefault(p => p.Key == connection.Key);
      //    if(busy.Key == null)
      //    {
      //      // Tell the client to start publishing is busy statuses to the server.
      //      Clients.Client(connection.Key.ToString()).is_busy(channel, userId);
      //    }

      //  }

      //}
      return false;
    }


    /// <summary>
    /// Subscribe a busy event.
    /// </summary>
    public void BusyEvent(WebSocket.SubscriptionChannel channel, Guid userId, bool busy)
    {
      //ConcurrentDictionary<Guid, Guid?> channel = null;

      //_connections.TryGetValue(channel, out channel);

      //if (channel != null)
      //{
      //  Guid? id = null;
      //  channel.TryGetValue(userId, out id);

      //  if (id == null)
      //  {
      //    //var _connnection = AddConnection(connectionId, userId);
      //    channel.AddOrUpdate(channel, busy, (s, i) => busy);

      //  }
      //}
      //// Check to see if the client has started eventing busy statuses.
      //var _busy = _busyConnections.FirstOrDefault(p => p.Key == connection.Key);

      //_busyConnections.AddOrUpdate(userId, busy, (s, i) => busy);
      //_logger.Information("[BusyEvent] - Updated existing event to {status} for user {user}", userId, userId);
    }

    /// <summary>
    /// Get a list of subscribed users.
    /// </summary>
    /// <returns></returns>
    public Dictionary<WebSocket.SubscriptionChannel, List<Guid>> GetSubscribedUsers()
    {
      Dictionary<WebSocket.SubscriptionChannel, List<Guid>> socketConnections = new Dictionary<WebSocket.SubscriptionChannel, List<Guid>>();
      var subscribers = _connections.ToList();
      foreach (var sub in subscribers)
      {
        var connectionsPerSubscriptionChannel = sub.Value.Select(p => p.Value).Distinct();
        foreach (var connectionPerSub in connectionsPerSubscriptionChannel)
        {
          if (sub.Key != null && !socketConnections.ContainsKey((WebSocket.SubscriptionChannel)sub.Key))
          {
            socketConnections.Add((WebSocket.SubscriptionChannel)sub.Key, new List<Guid>());
            if (connectionPerSub != null)
              socketConnections[(WebSocket.SubscriptionChannel)sub.Key].Add((Guid)connectionPerSub);
          }
          else
          {
            if (sub.Key != null)
              if (connectionPerSub != null)
                socketConnections[(WebSocket.SubscriptionChannel)sub.Key].Add((Guid)connectionPerSub);
          }
        }
      }

      return socketConnections;
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
