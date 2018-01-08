using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Colony.Integration.Service.Eurocom;
using Atlas.Enumerators;
using Atlas.RabbitMQ.Messages.Coupon;
using Atlas.RabbitMQ.Messages.Notification;
using MassTransit;
using RestSharp;
using Serilog;

namespace Atlas.Colony.Integration.Service.Consumer
{
  public sealed class SmsSendConsumer : Consumes<EurocomSMSStartRequestMessage>.All, IBusService
  {
    private ILogger _log = Log.Logger.ForContext<CouponIssueConsumer>();
    

    public IServiceBus bus;

    private UnsubscribeAction unsubscribeAction;

    public Guid CorrelationId { get; set; }
    public void Consume(EurocomSMSStartRequestMessage message)
    {
      using (var eurocom = new EurocomImpl())
      {
        Notification.NotificationStatus status = eurocom.SendSMS(message.CampaignId, message.CellNo, message.Message)
          ? Notification.NotificationStatus.Sent
          : Notification.NotificationStatus.Failed;

        if (message.NotificationId != null)
        {
          this.Context()
            .Bus.Publish<SMSNotifyUpdateWithStatus>(new SMSNotifyUpdateWithStatus(Magnum.CombGuid.Generate())
            {
              NotificationId = (long)message.NotificationId,
              Status = status
            });
        }
      }

      this.Context().Bus.Publish<EurocomSMSCompletedRequestMessage>(AutoMapper.Mapper.Map<EurocomSMSStartRequestMessage, EurocomSMSCompletedRequestMessage>(message));
    }

    public void Dispose()
    {
      // Do nothing
    }

    public void Start(IServiceBus _bus)
    {
      bus = _bus;
      unsubscribeAction = bus.SubscribeConsumer<SmsSendConsumer>();
    }

    public void Stop()
    {
      unsubscribeAction();
    }
  }
}
