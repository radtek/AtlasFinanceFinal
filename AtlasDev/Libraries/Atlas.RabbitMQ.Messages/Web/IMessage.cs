using System;
using Atlas.Enumerators;


namespace Atlas.RabbitMQ.Messages.Online
{
  public interface IMessage 
  {
    long? RouteHistoryId { get; set; }
    long? MessageId { get; set; }
    NodeType.Nodes Source { get; set; }
    NodeType.Nodes Destination { get; set; }
    DateTime CreatedAt { get; set; }
  }
}
