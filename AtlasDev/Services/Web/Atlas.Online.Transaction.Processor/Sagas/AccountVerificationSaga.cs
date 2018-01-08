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
  public sealed class AccountVerificationSaga : SagaStateMachine<AccountVerificationSaga>, ISaga
  {
    #region Public Properties

    public IServiceBus Bus { get; set; }

    public Guid CorrelationId { get; set; }

    #endregion

    #region Events

    public static Event<AccountVerificationRequestMessage> AccountVerificationRequestMessage { get; set; }

    public static Event<AccountVerificationCompletedRequestMessage> AccountVerificationCompletedRequestMessage { get; set; }

    #endregion

    #region Logger

    private static readonly ILogger _log = Log.Logger.ForContext<AccountVerificationSaga>();

    #endregion

    #region States
    public static State Initial { get; set; }
    public static State Completed { get; set; }
    public static State PerformingAccountVerification { get; set; }

    #endregion

    #region Constructor

    public AccountVerificationSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

    public AccountVerificationSaga()
    {
    }

    #endregion

    #region Static

    static AccountVerificationSaga()
    {
      Define(() =>
        {
          Initially(
              When(AccountVerificationRequestMessage)
                .Then((saga, message) =>
                    saga.Step1(message))
                        .TransitionTo(PerformingAccountVerification)
                  );

          During(PerformingAccountVerification, When(AccountVerificationCompletedRequestMessage)
                .Then((saga, message) => saga.Step2(message)).Complete()
                );

        });
    }


    #endregion

    #region Saga Methods

    public void Step1(AccountVerificationRequestMessage message)
    {
      _log.Information("Transitioning {0} to AccountVerificationRequestMessage...", message.CorrelationId);

      Bus.Publish<AccountVerificationStartRequestMessage>(new AccountVerificationStartRequestMessage()
      {
        CorrelationId = message.CorrelationId
      });

      _log.Information("End {0} AccountVerificationRequestMessage.", message.CorrelationId);

    }

    private void Step2(AccountVerificationCompletedRequestMessage message)
    {
      _log.Information("Transitioning {0} to AccountVerificationCompletedRequestMessage...", message.CorrelationId);

      Bus.Publish<DecisionRequestMessage>(new DecisionRequestMessage()
      {
        CorrelationId = message.CorrelationId
      });

      _log.Information("End {0} AccountVerificationCompletedRequestMessage.", message.CorrelationId);
    }
    #endregion
  }
}
