using Atlas.RabbitMQ.Messages.Coupon;
using Atlas.RabbitMQ.Messages.Notification;
using Atlas.RabbitMQ.Messages.Online;
using Magnum.StateMachine;
using MassTransit;
using MassTransit.Saga;
using System;
using Serilog;

namespace Atlas.Colony.Integration.Service.Saga
{
  public sealed class SmsSendSaga : SagaStateMachine<SmsSendSaga>,
    ISaga
  {
    #region Public Properties

    public IServiceBus Bus { get; set; }

    public Guid CorrelationId { get; set; }

    #endregion

    #region Events

    public static Event<EurocomSMSRequestMessage> EurocomSMSRequestMessage { get; set; }

    public static Event<EurocomSMSCompletedRequestMessage> EurocomSMSCompletedRequestMessage { get; set; }

    #endregion

    #region Logger

    private static readonly ILogger _log = Log.Logger.ForContext<SmsSendSaga>();

    #endregion

    #region States

    public static State Initial { get; set; }
    public static State Completed { get; set; }
    public static State SendingSMSMessage { get; set; }

    #endregion

    #region Constructor

    public SmsSendSaga(Guid correlationId)
    {
      CorrelationId = correlationId;
    }

    public SmsSendSaga()
    {
    }


    #endregion

    #region Static

    static SmsSendSaga()
    {
      Define(() =>
      {
        Initially(
          When(EurocomSMSRequestMessage)
            .Then((saga, message) =>
              saga.Step1(message))
            .TransitionTo(SendingSMSMessage)
          );

        During(SendingSMSMessage, When(EurocomSMSCompletedRequestMessage)
          .Then((saga, message) => saga.Step2
            (message)).Complete()
          );

      });
    }


    #endregion

    #region Saga Methods

    public void Step1(EurocomSMSRequestMessage message)
    {
      _log.Information("Processing {0} EurocomSMSRequestMessage...", message.CorrelationId);

      Bus.Publish<EurocomSMSStartRequestMessage>(AutoMapper.Mapper.Map<EurocomSMSRequestMessage, EurocomSMSStartRequestMessage>(message));

      _log.Information("End {0} EurocomSMSRequestMessage.", message.CorrelationId);
    }

    private void Step2(EurocomSMSCompletedRequestMessage message)
    {
      _log.Information("Transitioning {0} to EurocomSMSCompletedRequestMessage...", message.CorrelationId);

      // Sink

      _log.Information("End {0} EurocomSMSCompletedRequestMessage...", message.CorrelationId);
    }

    #endregion
  }
}