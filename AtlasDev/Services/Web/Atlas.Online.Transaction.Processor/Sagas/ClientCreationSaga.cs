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
  public sealed class ClientCreationSaga : SagaStateMachine<ClientCreationSaga>, 
                                           ISaga
  {
    #region Public Properties

    public IServiceBus Bus { get; set; }

    public Guid CorrelationId { get; set; }

    public long ClientId { get; set; }
    public string IdNo { get; set; }

    #endregion

    #region Events

    public static Event<ClientCreationRequestMessage> ClientCreationRequestMessage { get; set; }

    public static Event<ClientCreationCompletedRequestMessage> ClientCreationCompletedRequestMessage { get; set; }

    #endregion

    #region Logger

    private static readonly ILogger _log = Log.Logger.ForContext<ClientCreationSaga>();
    
    #endregion

    #region States
    public static State Initial { get; set; }
    public static State Completed { get; set; }
    public static State CreatingNewClient { get; set; }
    #endregion

    #region Constructor

    public ClientCreationSaga(Guid correlationId)
		{
			CorrelationId = correlationId;
		}

    public ClientCreationSaga()
    {
    }
  

    #endregion

    #region Static

    static ClientCreationSaga()
    {
      Define(() =>
        {
          Initially(
              When(ClientCreationRequestMessage)
                .Then((saga, message) =>
                    saga.Step1(message))
                        .TransitionTo(CreatingNewClient)
                  );

          During(CreatingNewClient, When(ClientCreationCompletedRequestMessage)
                .Then((saga, message) => saga.Step2
                               (message)).Complete()
                );

        });
    }


    #endregion

    #region Saga Methods

    public void Step1(ClientCreationRequestMessage message)
    {
      _log.Information("Processing {0} ClientCreationRequestMessage...", message.CorrelationId);

      Bus.Publish<ClientCreationStartRequestMessage>(new ClientCreationStartRequestMessage()
      {
        CorrelationId = message.CorrelationId,
        Id = 123L
      });

      _log.Information("End {0} ClientCreationRequestMessage.", message.CorrelationId);
    }

    private void Step2(ClientCreationCompletedRequestMessage message)
    {
      _log.Information("Transitioning {0} to ClientCreationCompletedRequestMessage...", message.CorrelationId);

      var meassage = new AccountCreationRequestMessage()
      {
        CorrelationId = message.CorrelationId,
        PersonId = 12312312L
      };

      _log.Information(string.Format("End {0} ClientCreationCompletedRequestMessage...", message.CorrelationId));

      Bus.Publish(meassage);
    }

    #endregion
  }
}
