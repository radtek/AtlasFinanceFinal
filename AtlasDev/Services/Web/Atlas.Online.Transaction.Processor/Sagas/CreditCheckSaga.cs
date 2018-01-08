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
  public sealed class CreditCheckSaga : SagaStateMachine<CreditCheckSaga>, ISaga
  {
    #region Public Properties

    public IServiceBus Bus { get; set; }

    public Guid CorrelationId { get; set; }

    #endregion

    #region Events

    public static Event<CreditCheckRequestMessage> CreditCheckRequestMessage { get; set; }

    public static Event<ClientCreationCompletedRequestMessage> ClientCreationCompletedRequestMessage { get; set; }

    #endregion

    #region Logger

    private static readonly ILogger _log = Log.Logger.ForContext<CreditCheckSaga>();

    #endregion

    #region States
    public static State Initial { get; set; }
    public static State Completed { get; set; }
    public static State PerformingCreditVerification { get; set; }

    #endregion

    #region Constructor

    public CreditCheckSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

    public CreditCheckSaga()
    {
    }

    #endregion

    #region Static

    static CreditCheckSaga()
    {
      Define(() =>
        {
          Initially(
              When(CreditCheckRequestMessage)
                .Then((saga, message) =>
                    saga.Step1(message))
                        .TransitionTo(PerformingCreditVerification)
                  );

          During(PerformingCreditVerification, When(ClientCreationCompletedRequestMessage)
                .Then((saga, message) => saga.Step2(message)).Complete()
                );

        });
    }


    #endregion

    #region Saga Methods

    public void Step1(CreditCheckRequestMessage message)
    {
      _log.Information("Transitioning {0} to CreditCheckRequestMessage...", message.CorrelationId);

      Bus.Publish<CreditCheckStartRequestMessage>(new CreditCheckStartRequestMessage()
      {
        CorrelationId = message.CorrelationId,
        PersonId = message.PersonId
      });

      _log.Information("End {0} CreditCheckRequestMessage.", message.CorrelationId);

    }

    private void Step2(ClientCreationCompletedRequestMessage message)
    {
      _log.Information("Transitioning {0} to ClientCreationCompletedRequestMessage...", message.CorrelationId);

      Bus.Publish<AffordabilityCalculationRequestMessage>(new AffordabilityCalculationRequestMessage()
      {
        CorrelationId = message.CorrelationId
      });

      _log.Information("End {0} ClientCreationCompletedRequestMessage.", message.CorrelationId);
    }
    #endregion
  }
}
