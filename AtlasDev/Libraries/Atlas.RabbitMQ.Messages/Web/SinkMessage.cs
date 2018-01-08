using System;


namespace Atlas.RabbitMQ.Messages.Online
{
  /// <summary>
  /// Sink class used to denote an end of cycle process.
  /// </summary>
  public class SinkMessage
  {
    public string Sink { get; set; }
  }
}
