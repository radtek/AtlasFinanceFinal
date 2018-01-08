using System.Collections.Generic;
using System.Linq;
using Atlas.Colony.Integration.Service.Saga;
using MassTransit;
using MassTransit.Saga;
using Ninject;
using System;
using System.IO;
using Serilog;


namespace Atlas.Colony.Integration.Service
{
  public sealed class Engine
  {
    private static ILogger _logger = Log.Logger.ForContext<Engine>();
    private static IKernel _kernal = null;
 
    private ISagaRepository<CouponIssueSaga> _couponIssueSaga { get; set; }
    private ISagaRepository<SmsSendSaga> _smsSendSaga { get; set; }

    private IServiceBus _bus = null;
    
    #region Unsubscribe Actions
    private List<UnsubscribeAction> _unsubscribeAction = new List<UnsubscribeAction>();

    #endregion
    public Engine(IKernel ikernal,IServiceBus bus, ISagaRepository<CouponIssueSaga> couponIssueSaga, ISagaRepository<SmsSendSaga> smsSendSaga)
    {
      _couponIssueSaga = couponIssueSaga;
      _smsSendSaga = smsSendSaga;
      _bus = bus;
      _kernal = ikernal;
    }

    public void Start()
    {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.Write(File.ReadAllText("load.asc") + "\n\r");
      Console.ForegroundColor = ConsoleColor.White;

      _unsubscribeAction.Add(_bus.SubscribeSaga(_couponIssueSaga));
      _unsubscribeAction.Add(_bus.SubscribeSaga(_smsSendSaga));

      SubscribeConsumers();
    }

    private void SubscribeConsumers()
    {
      var consumers = _kernal.GetAll<IBusService>();

      if (consumers.ToList().Count == 0)
        _logger.Warning("[SubscribeConsumers] - No consumers were detected in assembly");

      foreach (var consumer in consumers)
        consumer.Start(_kernal.Get<IServiceBus>());
    }

    public void Stop()
    {
      _kernal.Dispose();
    }
  }
}
