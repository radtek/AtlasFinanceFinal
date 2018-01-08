using System;


namespace Atlas.RabbitMQ.Messages.Workflow
{
  [Serializable]
  public class StepWorkflowProcessMessage : IMessage
  {
    public StepWorkflowProcessMessage(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }
    public int CurrentProcessStepId { get; set; }
    public Enumerators.Workflow.WorkflowProcess Workflow { get; set; }
    public Enumerators.Workflow.WorkflowDirection Direction { get; set; }
    // Used for jumping
    public int? JumpToProcessStepId { get; set; }

    // The process thats getting this message needs to know what type it is
    public dynamic Data { get; set; }

  }
}