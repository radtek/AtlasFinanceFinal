using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Saga;
using Atlas.RabbitMQ.Messages.Online;
using Magnum.StateMachine;
using Serilog;

namespace Atlas.Online.Transaction.Processor.Sagas
{
  public sealed class AffordabilitySaga : SagaStateMachine<AffordabilitySaga>, ISaga
  {
    #region Public Properties

    public IServiceBus Bus { get; set; }

    public Guid CorrelationId { get; set; }

    #endregion

    #region Events

    public static Event<AffordabilityCalculationRequestMessage> AffordabilityCalculationRequestMessage { get; set; }

    public static Event<AffordabilityCalculationCompletedRequestMessage> AffordabilityCalculationCompletedRequestMessage { get; set; }

    #endregion

    #region Logger

    private static readonly ILogger _log = Log.Logger.ForContext<AffordabilitySaga>();

    #endregion

    #region States
    public static State Initial { get; set; }
    public static State Completed { get; set; }
    public static State PerformingAffordabilityCalculation { get; set; }

    #endregion

    #region Constructor

    public AffordabilitySaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

    public AffordabilitySaga()
    {
    }

    #endregion

    #region Static

    static AffordabilitySaga()
    {
      Define(() =>
        {
          Initially(
              When(AffordabilityCalculationRequestMessage)
                .Then((saga, message) =>
                    saga.Step1(message))
                        .TransitionTo(PerformingAffordabilityCalculation)
                  );

          During(PerformingAffordabilityCalculation, When(AffordabilityCalculationCompletedRequestMessage)
                .Then((saga, message) => saga.Step2(message)).Complete()
                );

        });
    }


    #endregion

    #region Saga Methods

    public void Step1(AffordabilityCalculationRequestMessage message)
    {
      _log.Information("Transitioning {0} to AffordabilityCalculationRequestMessage...", message.CorrelationId);

      Bus.Publish<AffordabilityCalculationStartRequestMessage>(new AffordabilityCalculationStartRequestMessage()
      {
        CorrelationId = message.CorrelationId
      });

      _log.Information("End {0} AffordabilityCalculationRequestMessage.", message.CorrelationId);

    }

    private void Step2(AffordabilityCalculationCompletedRequestMessage message)
    {
      _log.Information("Transitioning {0} to AffordabilityCalculationCompletedRequestMessage...", message.CorrelationId);

      Bus.Publish<AccountVerificationStartRequestMessage>(new AccountVerificationStartRequestMessage()
      {
        CorrelationId = message.CorrelationId
      });

      _log.Information("End {0} AffordabilityCalculationCompletedRequestMessage.", message.CorrelationId);
    }
    #endregion
  }
}
