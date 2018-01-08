using System;


namespace Atlas.RabbitMQ.Messages.DebitOrder
{
  /// <summary>
  /// This will Stop all debit Orders for a specified control
  /// </summary>
  public class StopDebitOrder : IMessage
  {
    public StopDebitOrder(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.Now;
    }


    public long? MessageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CorrelationId { get; set; }
    public long ControlId { get; set; }
    /// <summary>
    /// If this is true, then new transactions for this control will be stopped.
    ///   If there is a transaction that has already been batched and sent off to the banks, an error will be thrown - control and transactions will not be stopped or cancelled
    /// if this is false and there are any transactions new or pending, the control will not be stopped and an error will be thrown
    /// </summary>
    public bool CancelTransactions { get; set; } 

  }
}
