  using Atlas.Common.Utils;
using Atlas.Online.Data.Models.Definitions;
using DevExpress.Xpo;
using MassTransit;
using Ninject.Extensions.Logging;
using System;
using System.Timers;
using System.Linq;
using Atlas.Enumerators;

namespace Atlas.Online.Node.Core
{

  public abstract class AbstractService<T, V, E> : IService
  {
    public readonly ILogger _logger;
    public Timer _timer;

    Object _lock = new Object();

    public AbstractService(ILogger ilogger)
    {
      _logger = ilogger;
    }

    public virtual void Start()
    {     

    }

    public virtual void Stop()
    {
      _timer.Dispose();
    }
    // Handles subscribed message
    public abstract void Handle(V message);

    // publish to consumer
    public abstract void Publish(E message);

    public virtual long AddRouteHistory(dynamic message)
    {
      _logger.Info(":: Route History End - (Source:{0}), (Destination:{1}), (messageId:{2})", message.Source, message.Destination, message.MessageId);

      using (var uow = new UnitOfWork())
      {
        long messageId = message.MessageId;

        var route = new RouteHistory(uow)
        {
          Source = Enum.GetName(typeof(NodeType.Nodes), message.Source),
          Destination = Enum.GetName(typeof(NodeType.Nodes), message.Destination),
          MsgData = Compression.Compress(Xml.Serialize((object)message)),
          Message = new XPQuery<Message>(uow).FirstOrDefault(m => m.MessageId == messageId),
          CreateDate = DateTime.Now
        };

        uow.CommitChanges();

        return route.RouteHistoryId;
      }
      _logger.Info(":: Route History End - (Source:{0}), (Destination:{1}), (messageId:{2})", message.Source, message.Destination, message.MessageId);
    }
  }
}