using BookSleeve;
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
  public sealed class NotificationHub //: Hub
  {
    //private static readonly ILog _log = LogManager.GetLogger(typeof(NotificationHub));
    //private static List<ConnectedClient> _connectedClients = new List<ConnectedClient>();

    //public void Connect(string userName)
    //{
    //  var id = Context.ConnectionId;

    //  if (_connectedClients.Count(x => x.ConnectionId == id) == 0)
    //  {
    //    _connectedClients.Add(new ConnectedClient { ConnectionId = id, Name = userName });
    //    //// send to caller
    //    //Clients.Caller.onConnected(id, userName, ConnectedUsers, CurrentMessage);

    //    //// send to all except caller client
    //    //Clients.AllExcept(id).onNewUserConnected(id, userName);
    //  }
    

    //public override System.Threading.Tasks.Task OnDisconnected()
    //{
    //  var item = _connectedClients.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
    //  if (item != null)
    //  {
    //    _connectedClients.Remove(item);
    //    var id = Context.ConnectionId;
    //  }

    //  return base.OnDisconnected();
    //}

    /// <summary>
    /// Publish immediate result back to calling signalR client.
    /// </summary>
    /// <param name="groupName">Branch for the avs analytics</param>
    private void Publish(string groupName)
    {
      
    }
  }
}