using System;
using System.Collections.Generic;
using System.Linq;

using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using Serilog;

using Atlas.RabbitMQ.Messages.Event.FingerPrint;


namespace Falcon.Gyrkin.ESB.Saga
{
  public sealed class FingerPrintEventMessageSaga : SagaStateMachine<FingerPrintEventMessageSaga>,
     ISaga
  {
    #region Public Properties

    public IServiceBus Bus { get; set; }

    public Guid CorrelationId { get; set; }

    public DateTime CreatDate = DateTime.Now;

    #endregion


    #region Events

    public static Event<FingerPrintEventMessage> FingerPrintEventMessage { get; set; }

    #endregion


    #region Logger

    private static readonly ILogger _log = Log.Logger.ForContext<FingerPrintEventMessageSaga>();

    #endregion


    #region States

    public static State Initial { get; set; }
    public static State Completed { get; set; }
    public static State SendingClientInformation { get; set; }
    public static State IgnoreClientInformation { get; set; }

    #endregion


    #region Constructor

    public FingerPrintEventMessageSaga(Guid correlationId)
    {
      CorrelationId = correlationId;
    }

    public FingerPrintEventMessageSaga()
    {
    }

    #endregion


    #region Static

    static FingerPrintEventMessageSaga()
    {
      Define(() =>
      {
        Initially(
          When(FingerPrintEventMessage)
            .Then((saga, message) =>
              saga.Step1(message))
            .Complete());

      });
    }

    #endregion


    #region Saga Methods

    public void Step1(FingerPrintEventMessage message)
    {
      Console.WriteLine(message.CorrelationId);
    }

    #endregion

  }
}