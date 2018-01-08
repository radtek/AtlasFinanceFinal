using Atlas.RabbitMQ.Messages.Online;
using MassTransit;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Transaction.Processor.Consumers
{
  public sealed class DecisionMessageConsumer : Consumes<DecisionStartRequestMessage>.All, IBusService
  {
    public IServiceBus bus;

    private UnsubscribeAction unsubscribeAction;

    public Guid CorrelationId { get;set;}
    public void Consume(DecisionStartRequestMessage message)
    {
     // Update DB
      this.Context().Bus.Publish<DecisionRequestCompletedMessage>(new DecisionRequestCompletedMessage()
      {
        CorrelationId = message.CorrelationId
      });
    }

    public void Dispose()
    {
      // Do nothing
    }

    public void Start(IServiceBus _bus)
    {
      bus = _bus;
      unsubscribeAction = bus.SubscribeConsumer<DecisionMessageConsumer>();
    }

    public void Stop()
    {
      unsubscribeAction();
    }
  }
}
