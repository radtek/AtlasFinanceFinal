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
  public sealed class AccountCreationSaga : SagaStateMachine<AccountCreationSaga>, ISaga
  {
    #region Public Properties

    public IServiceBus Bus { get; set; }

    public Guid CorrelationId { get; set; }

    #endregion

    #region Events

    public static Event<AccountCreationRequestMessage> AccountCreationRequestMessage { get; set; }

    public static Event<AccountCreationCompletedRequestMessage> AccountCreationCompletedRequestMessage { get; set; }

    #endregion

    #region Logger

    private static readonly ILogger _log = Log.Logger.ForContext<AccountCreationSaga>();

    #endregion

    #region States
    public static State Initial { get; set; }
    public static State Completed { get; set; }
    public static State CreatingNewAccount { get; set; }

    #endregion

    #region Constructor

    public AccountCreationSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

    public AccountCreationSaga()
    {
    }

    #endregion

    #region Static

    static AccountCreationSaga()
    {
      Define(() =>
        {
          Initially(
              When(AccountCreationRequestMessage)
                .Then((saga, message) =>
                    saga.Step1(message))
                        .TransitionTo(CreatingNewAccount)
                  );

          During(CreatingNewAccount, When(AccountCreationCompletedRequestMessage)
                .Then((saga, message) => saga.Step2(message)).Complete()
                );

        });
    }


    #endregion

    #region Saga Methods

    public void Step1(AccountCreationRequestMessage message)
    {
      _log.Information("Transitioning {0} to AccountCreationRequestMessage...", message.CorrelationId);

      Bus.Publish<AccountCreationStartRequestMessage>(new AccountCreationStartRequestMessage()
      {
        CorrelationId = message.CorrelationId,
        PersonId = message.PersonId
      });

      _log.Information("End {0} AccountCreationRequestMessage.", message.CorrelationId);

    }

    private void Step2(AccountCreationCompletedRequestMessage message)
    {
      _log.Information("Transitioning {0} to AccountCreationCompletedRequestMessage...", message.CorrelationId);
      
      Bus.Publish<FraudPreventionRequestMessage>(new FraudPreventionRequestMessage()
      {
          CorrelationId = message.CorrelationId,
          PersonId = message.PersonId
      });

      _log.Information("End {0} AccountCreationCompletedRequestMessage.", message.CorrelationId);
    }
    #endregion
  }
}
