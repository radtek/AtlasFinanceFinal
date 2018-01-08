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
  public sealed class DecisionSaga : SagaStateMachine<DecisionSaga>, ISaga
  {
    #region Public Properties

    public IServiceBus Bus { get; set; }

    public Guid CorrelationId { get; set; }

    #endregion

    #region Events

    public static Event<DecisionRequestMessage> DecisionRequestMessage { get; set; }

    public static Event<DecisionRequestCompletedMessage> DecisionRequestCompletedMessage { get; set; }

    #endregion

    #region Logger

    private static readonly ILogger _log = Log.Logger.ForContext<AccountCreationSaga>();

    #endregion

    #region States
    public static State Initial { get; set; }
    public static State Completed { get; set; }
    public static State UpdateDecision { get; set; }

    #endregion

    #region Constructor

    public DecisionSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

    public DecisionSaga()
    {
    }

    #endregion

    #region Static

    static DecisionSaga()
    {
      Define(() =>
        {
          Initially(
              When(DecisionRequestMessage)
                .Then((saga, message) =>
                    saga.Step1(message))
                        .TransitionTo(UpdateDecision)
                  );

          During(UpdateDecision, When(DecisionRequestCompletedMessage)
                .Then((saga, message) => saga.Step2(message)).Complete()
                );

        });
    }


    #endregion

    #region Saga Methods

    public void Step1(DecisionRequestMessage message)
    {
      _log.Information("Transitioning {0} to DecisionRequestMessage...", message.CorrelationId);

      // Update Account / Email Client
      Bus.Publish<DecisionStartRequestMessage>(new DecisionStartRequestMessage()
      {
        CorrelationId = message.CorrelationId
      });

      _log.Information("End {0} DecisionRequestMessage.", message.CorrelationId);
    }

    private void Step2(DecisionRequestCompletedMessage message)
    {
      _log.Information("Transitioning {0} to DecisionRequestCompletedMessage...", message.CorrelationId);

      //  Nothing

      _log.Information("End {0} DecisionRequestCompletedMessage.", message.CorrelationId);
    }
    #endregion
  }
}
