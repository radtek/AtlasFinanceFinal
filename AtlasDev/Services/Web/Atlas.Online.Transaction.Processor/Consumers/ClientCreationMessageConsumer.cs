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
  public sealed class ClientCreationMessageConsumer : Consumes<ClientCreationStartRequestMessage>.All, IBusService
  {
    public IServiceBus bus;

    private UnsubscribeAction unsubscribeAction;

    public Guid CorrelationId { get;set;}
    public void Consume(ClientCreationStartRequestMessage message)
    {

      this.Context().Bus.Publish<ClientCreationCompletedRequestMessage>(new ClientCreationCompletedRequestMessage()
      {
        CorrelationId = message.CorrelationId,
        Id = 455454L
      });
    }
    public void Dispose()
    {
      // Do nothing
    }

    public void Start(IServiceBus _bus)
    {
      bus = _bus;
      unsubscribeAction = bus.SubscribeConsumer<ClientCreationMessageConsumer>();
    }

    public void Stop()
    {
      unsubscribeAction();
    }
  }
}
