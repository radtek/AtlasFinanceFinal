using System;
using System.Collections.Generic;
using System.Linq;

using MassTransit;
using Serilog;

using Atlas.RabbitMQ.Messages.Event.FingerPrint;


namespace Falcon.Gyrkin.ESB
{
  public sealed class FingerPrintEventMessageConsumer : Consumes<FingerPrintEventMessage>.All, IBusService
  {
    #region Private Properties

    private ILogger _log = Log.Logger.ForContext<FingerPrintEventMessageConsumer>();

    #endregion


    #region Public Properties

    public IServiceBus bus;
    private UnsubscribeAction unsubscribeAction;
    public Guid CorrelationId { get; set; }

    #endregion


    public void Consume(FingerPrintEventMessage message)
    {
    }

    public void Dispose()
    {
      // Do nothing
    }

    public void Start(IServiceBus _bus)
    {
      bus = _bus;
      unsubscribeAction = bus.SubscribeConsumer<FingerPrintEventMessageConsumer>();
    }

    public void Stop()
    {
      unsubscribeAction();
    }
  }
}