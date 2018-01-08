using Atlas.RabbitMQ.Messages.Online;
using MassTransit;
using System;

namespace Atlas.Online.Transaction.Processor.Consumers
{
  public sealed class AffordabilityMessageConsumer : Consumes<AffordabilityCalculationStartRequestMessage>.All, IBusService
  {
    public IServiceBus bus;

    private UnsubscribeAction unsubscribeAction;

    public Guid CorrelationId { get;set;}
    public void Consume(AffordabilityCalculationStartRequestMessage message)
    {
      this.Context().Bus.Publish<AffordabilityCalculationCompletedRequestMessage>(new FraudPreventionCompletedRequestMessage()
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
