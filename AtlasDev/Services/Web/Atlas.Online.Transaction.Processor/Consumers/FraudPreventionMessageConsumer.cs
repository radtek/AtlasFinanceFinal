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
  public sealed class FraudPreventionMessageConsumer : Consumes<FraudPreventionStartRequestMessage>.All, IBusService
  {
    public IServiceBus bus;

    private UnsubscribeAction unsubscribeAction;

    public Guid CorrelationId { get;set;}
    public void Consume(FraudPreventionStartRequestMessage message)
    {
      this.Context().Bus.Publish<FraudPreventionCompletedRequestMessage>(new FraudPreventionCompletedRequestMessage()
      {
        CorrelationId = message.CorrelationId,
        PersonId = message.PersonId
      });
    }

    public void Dispose()
    {
      // Do nothing
    }

    public void Start(IServiceBus _bus)
    {
      bus = _bus;
      unsubscribeAction = bus.SubscribeConsumer<FraudPreventionMessageConsumer>();
    }

    public void Stop()
    {
      unsubscribeAction();
    }
  }
}
