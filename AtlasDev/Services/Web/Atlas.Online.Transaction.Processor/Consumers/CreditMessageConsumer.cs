using Atlas.RabbitMQ.Messages.Online;
using MassTransit;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Atlas.Online.Transaction.Processor.Consumers
{
  public sealed class CreditMessageConsumer : Consumes<CreditCheckStartRequestMessage>.All, IBusService
  {
    public IServiceBus bus;

    private UnsubscribeAction unsubscribeAction;

    public Guid CorrelationId { get;set;}
    public void Consume(CreditCheckStartRequestMessage message)
    {

      this.Context().Bus.Publish<CreditCheckedCompletedRequestMessage>(new CreditCheckedCompletedRequestMessage()
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
      unsubscribeAction = bus.SubscribeConsumer<CreditMessageConsumer>();
    }

    public void Stop()
    {
      unsubscribeAction();
    }
  }
}
