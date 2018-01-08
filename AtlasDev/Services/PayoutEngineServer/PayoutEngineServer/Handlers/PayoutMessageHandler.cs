using Atlas.RabbitMQ.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Payout.Engine.Handlers
{
  public class PayoutMessageHandler
  {
    public object _lock { get; set; }

    public PayoutMessageHandler()
    {
      _lock = new object();
    }

    public void HandleMessage(InitiatePayoutMessage payoutMessage)
    {
      lock (_lock)
      {
        // Do Validation

        // Do Import
 
        // Do Processing

        // Close Completed Batches
      }
    }
  }
}
