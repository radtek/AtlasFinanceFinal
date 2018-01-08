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
  public sealed class FraudPreventionSaga : SagaStateMachine<FraudPreventionSaga>, ISaga
  {
    #region Public Properties

    public IServiceBus Bus { get; set; }

    public Guid CorrelationId { get; set; }

    #endregion

    #region Events

    public static Event<FraudPreventionRequestMessage> FraudPreventionRequestMessage { get; set; }

    public static Event<FraudPreventionCompletedRequestMessage> FraudPreventionCompletedRequestMessage { get; set; }

    #endregion

    #region Logger

    private static readonly ILogger _log = Log.Logger.ForContext<FraudPreventionSaga>();

    #endregion

    #region States
    public static State Initial { get; set; }
    public static State Completed { get; set; }
    public static State PerformingFraudVerification { get; set; }

    #endregion

    #region Constructor

    public FraudPreventionSaga(Guid correlationId)
    {
      CorrelationId = correlationId;
    }

    public FraudPreventionSaga()
    {
    }

    #endregion

    #region Static

    static FraudPreventionSaga()
    {
      Define(() =>
        {
          Initially(
              When(FraudPreventionRequestMessage)
                .Then((saga, message) =>
                    saga.Step1(message))
                        .TransitionTo(PerformingFraudVerification)
                  );

          During(PerformingFraudVerification, When(FraudPreventionCompletedRequestMessage)
                .Then((saga, message) => saga.Step2(message)).Complete()
                );

        });
    }


    #endregion

    #region Saga Methods

    public void Step1(FraudPreventionRequestMessage message)
    {
      _log.Information("Transitioning {0} to FraudPreventionRequestMessage...", message.CorrelationId);

      Bus.Publish<FraudPreventionStartRequestMessage>(new FraudPreventionStartRequestMessage()
      {
        CorrelationId = message.CorrelationId,
        PersonId = message.PersonId
      });

      _log.Information("End {0} FraudPreventionRequestMessage.", message.CorrelationId);

    }

    private void Step2(FraudPreventionCompletedRequestMessage message)
    {
      _log.Information("Transitioning {0} to FraudPreventionCompletedRequestMessage...", message.CorrelationId);

      Bus.Publish<CreditCheckRequestMessage>(new CreditCheckRequestMessage()
      {
        CorrelationId = message.CorrelationId,
        PersonId = message.PersonId
      });

      _log.Information("End {0} FraudPreventionCompletedRequestMessage.", message.CorrelationId);
    }
    #endregion
  }
}
