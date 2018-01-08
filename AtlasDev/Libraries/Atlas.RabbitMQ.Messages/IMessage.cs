using System;

namespace Atlas.RabbitMQ.Messages
{
  public interface IMessage
  {
    long? MessageId { get; set; }
    DateTime CreatedAt { get; set; }
  }
}
