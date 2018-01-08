using System.Runtime.CompilerServices;
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
  public sealed class CouponIssueSaga : SagaStateMachine<CouponIssueSaga>,
    ISaga
  {
    #region Public Properties

    public IServiceBus Bus { get; set; }

    public Guid CorrelationId { get; set; }

    public DateTime CreatDate = DateTime.Now;

    #endregion

    #region Events

    public static Event<CouponIssueRequestMessage> CouponIssueRequestMessage { get; set; }

    public static Event<CouponIssueCompletedRequestMessage> CouponIssueCompletedRequestMessage { get; set; }

    public static Event<CouponIssueIgnoreRequestMessage> CouponIssueIgnoreRequestMessage { get; set; }

    #endregion

    #region Logger

    private static readonly ILogger _log = Log.Logger.ForContext<CouponIssueSaga>();

    #endregion

    #region States

    public static State Initial { get; set; }
    public static State Completed { get; set; }
    public static State SendingClientInformation { get; set; }
    public static State IgnoreClientInformation { get; set; }

    #endregion

    #region Constructor

    public CouponIssueSaga(Guid correlationId)
    {
      CorrelationId = correlationId;
    }

    public CouponIssueSaga()
    {
    }


    #endregion

    #region Static

    static CouponIssueSaga()
    {
      Define(() =>
      {
        Initially(
          When(CouponIssueRequestMessage)
            .Then((saga, message) =>
              saga.Step1(message))
            .TransitionTo(SendingClientInformation)
          );

        During(SendingClientInformation, When(CouponIssueCompletedRequestMessage)
          .Then((saga, message) => saga.Step2
            (message)).Complete()
          );

        During(IgnoreClientInformation, When(CouponIssueIgnoreRequestMessage)
          .Complete());

      });
    }


    #endregion

    #region Saga Methods

    public void Step1(CouponIssueRequestMessage message)
    {
      _log.Information("Processing {0} CouponIssueRequestMessage...", message.CorrelationId);

      Bus.Publish<CouponIssueStartRequestMessage>(AutoMapper.Mapper.Map<CouponIssueRequestMessage, CouponIssueStartRequestMessage>(message));

      _log.Information("End {0} CouponIssueRequestMessage.", message.CorrelationId);
    }

    private void Step2(CouponIssueCompletedRequestMessage message)
    {
      _log.Information("Transitioning {0} to CouponIssueCompletedRequestMessage...", message.CorrelationId);

      Bus.Publish<EurocomSMSRequestMessage>(new EurocomSMSRequestMessage()
      {
        CorrelationId = Magnum.CombGuid.Generate(),
        CampaignId = message.CampaignId,
        CellNo = message.CellNo,
        Message = message.Message
      });

      _log.Information("End {0} CouponIssueCompletedRequestMessage...", message.CorrelationId);
    }

    #endregion
  }
}