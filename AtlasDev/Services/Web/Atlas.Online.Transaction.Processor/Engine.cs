using Ninject;
using System;
using Atlas.Online.Node.Core;
using MassTransit;
using MassTransit.Saga;
using Atlas.Online.Transaction.Processor.Sagas;
using System.Collections.Generic;
using System.Linq;
using Serilog;

namespace Atlas.Online.Transaction.Processor
{
  public sealed class Engine : IService
  {

    #region Injection Points

    [Inject]
    public IServiceBus Bus { get; set; }
    [Inject]
    public IKernel Kernel { get; set; }
    [Inject]
    public ISagaRepository<ClientCreationSaga> ClientCreationSaga { get; set; }
    [Inject]
    public ISagaRepository<AccountCreationSaga> AccountCreationSaga { get; set; }
    [Inject]
    public ISagaRepository<FraudPreventionSaga> FraudPreventionSaga { get; set; }
    [Inject]
    public ISagaRepository<CreditCheckSaga> CreditCheckSaga { get; set; }
    [Inject]
    public ISagaRepository<AffordabilitySaga> AffordabilitySaga { get; set; }
    [Inject]
    public ISagaRepository<AccountVerificationSaga> AccountVerificationSaga { get; set; }
    [Inject]
    public ISagaRepository<DecisionSaga> DecisionSaga { get; set; }

    #endregion

    #region Logger

    public ILogger Logger = Log.Logger.ForContext<Engine>();

    #endregion

    #region Unsubscribe Actions
    private List <UnsubscribeAction> _unsubscribeAction = new List<UnsubscribeAction>();

    #endregion

    public Engine()
    {
      
    }

    public void Start()
    {
      Logger.Information("[Engine] - Starting Engine");

      Logger.Information("[Engine] - Engine Started");
      try
      {
        Logger.Information("[Saga] - Subscribing to saga registrations...");

        _unsubscribeAction.Add(Bus.SubscribeSaga(ClientCreationSaga));
        _unsubscribeAction.Add(Bus.SubscribeSaga(AccountCreationSaga));
        _unsubscribeAction.Add(Bus.SubscribeSaga(FraudPreventionSaga));
        _unsubscribeAction.Add(Bus.SubscribeSaga(CreditCheckSaga));
        _unsubscribeAction.Add(Bus.SubscribeSaga(AffordabilitySaga));
        _unsubscribeAction.Add(Bus.SubscribeSaga(AccountVerificationSaga));
        _unsubscribeAction.Add(Bus.SubscribeSaga(DecisionSaga));

        Logger.Information("[Saga] - Subscribing complete.");

        Logger.Information("[Consumers] - Subscribing consumers...");

        SubscribeConsumers();

        Logger.Information("[Consumers] - Subscribing consumers completed.");
      }
      catch (Exception err)
      {
        Logger.Fatal("[Engine] -Failed to load 'Config' service", err);
      }
    }

    /// <summary>
    /// Processes binding context of container in order to retrieve consumers that impleemnt Consumes<>.All
    /// </summary>
    private void SubscribeConsumers()
    {
      var consumers = Kernel.GetAll<IBusService>();

      if (consumers.ToList().Count == 0)
        Logger.Warning("[SubscribeConsumers] - No consumers were detected in assembly");

      foreach (var consumer in consumers)
        consumer.Start(Kernel.Get<IServiceBus>());
    }

    public void Stop()
    {
      //foreach (var unsub in _unsubscribeAction)
      //  unsub();

      Bus.Dispose();
      Logger.Information("[Online] - Shutting Down.");
    }
  }
}
