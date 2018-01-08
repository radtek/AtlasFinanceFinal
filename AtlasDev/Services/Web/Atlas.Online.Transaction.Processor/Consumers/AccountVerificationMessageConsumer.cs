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
  public sealed class AccountVerificationMessageConsumer : Consumes<AccountVerificationStartRequestMessage>.All, IBusService
  {
    public IServiceBus bus;

    private UnsubscribeAction _unsubscribeAction;

    public Guid CorrelationId { get;set;}
    public void Consume(AccountVerificationStartRequestMessage message)
    {
      this.Context().Bus.Publish<AccountVerificationCompletedRequestMessage>(new AccountVerificationCompletedRequestMessage()
      {
        CorrelationId = message.CorrelationId,
      });
    }

    public void Dispose()
    {
      // Do nothing
    }

    public void Start(IServiceBus _bus)
    {
      bus = _bus;
      _unsubscribeAction = bus.SubscribeConsumer<AccountVerificationMessageConsumer>();
    }

    public void Stop()
    {
      _unsubscribeAction();
    }
  }
}
