using BookSleeve;
using Falcon.Service.Core;
using Falcon.Service.Structures;
using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Falcon.Service.Hubs
{
  public sealed class AvsAnalyticsHub //: Hub
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(AvsAnalyticsHub));
    private static object _lock = new object();

    //public override Task OnConnected()
    //{
    //  if (!ConnectionPool.Connections.ContainsKey(Context.ConnectionId))
    //  {
    //    lock (ConnectionPool.Connections)
    //    {
    //      ConnectionPool.Connections.Add(Context.ConnectionId, new ConnectionItem()
    //      {
    //        Hub = this,
    //        Created = DateTime.Now
    //      });
    //    }
    //  }

    //  return base.OnConnected();
    //}
    //public override Task OnDisconnected()
    //{
    //  if (ConnectionPool.Connections.ContainsKey(Context.ConnectionId))
    //  {
    //    lock (ConnectionPool.Connections)
    //    {
    //      ConnectionPool.Connections.Remove(Context.ConnectionId);
    //    }
    //  }
    //  return base.OnDisconnected();
    //}

    /// <summary>
    /// Subscribe to analytics updates for avs
    /// </summary>
    /// <param name="groupName">Branch for the avs analytics</param>
    public void Subscribe(string groupName)
    {
     // _log.Info(string.Format("Connection {0} has subscribed to updates for Branch {1}", Context.ConnectionId, groupName));

      //lock (ConnectionPool.Connections)
      //{
      //  if (ConnectionPool.Connections.ContainsKey(Context.ConnectionId))
      //  {
      //    lock (_lock)
      //    {
      //      Groups.Add(Context.ConnectionId, groupName);
      //    }
      //  }
      //  _log.Info(string.Format("Publishing immediate analytics results to connection {0} for branch {1} avs", Context.ConnectionId, groupName));
      //  this.Publish(groupName);
      //}
    }

    /// <summary>
    /// Publish immediate result back to calling signalR client.
    /// </summary>
    /// <param name="groupName">Branch for the avs analytics</param>
    private void Publish(string groupName)
    {
      var conn = BookSleeveConnection.Connection();
 
      //_log.Info(string.Format("[FalconService][AvsAnalyticsHub] - Getting statsics for {0}", string.Format("avs.analytics.branch.{0}", groupName)));

      string queryKey = string.Empty;
      
      if (groupName == "Avs-1")
        queryKey = "unassigned";

      var result = conn.Strings.GetInt64(0, (string.Format("avs.analytics.branch.{0}", queryKey)));

      if (result.Result.HasValue)
        _log.Info(string.Format("[FalconService][AvsAnalyticsHub] - Finished getting statsics for {0} with result {1}", string.Format("avs.analytics.branch.{0}", queryKey), Convert.ToString(result.Result)));
      else
        _log.Info(string.Format("[FalconService][AvsAnalyticsHub] - Finished getting statsics for {0} with result {1}", string.Format("avs.analytics.branch.{0}", queryKey), "0"));

      //Clients.Group(groupName).update(Convert.ToString(result.Result));
    }
  }
}