using System;


namespace Atlas.RabbitMQ.Messages.Workflow
{
  [Serializable]
  public class StartWorflowProcessMessage : IMessage
  {
    public StartWorflowProcessMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }
    public Enumerators.Workflow.WorkflowProcess Workflow { get; set; }

    // The process thats getting this message needs to know what type it is
    public dynamic Data { get; set; }

  }
}