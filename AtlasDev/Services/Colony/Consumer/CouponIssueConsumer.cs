using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Atlas.Colony.Integration.Service.Eurocom;
using Atlas.Common.Utils;
using Atlas.RabbitMQ.Messages.Coupon;
using MassTransit;
using NHibernate.Mapping;
using RestSharp;
using Serilog;
using Atlas.Common.Extensions;

namespace Atlas.Colony.Integration.Service.Consumer
{
  public sealed class CouponIssueConsumer : Consumes<CouponIssueStartRequestMessage>.All, IBusService
  {
    private ILogger _log = Log.Logger.ForContext<CouponIssueConsumer>();
    private List<int> _regions = new List<int>();
    private int CAMPAIGN_ID = Convert.ToInt32(ConfigurationManager.AppSettings["euro.campaign.id"]);
    private string CAMPAIGN_MSG = ConfigurationManager.AppSettings["euro.campaign.sms.msg"];
    public IServiceBus bus;

    private UnsubscribeAction unsubscribeAction;

    public Guid CorrelationId { get; set; }

    public void Consume(CouponIssueStartRequestMessage message)
    {
      _log.Information("[CouponIssueConsumer] - Message Received : {msg}", message.ToJSON());
      _regions.Clear();

      // Process Coupon Request
      bool success = false;

      using (var eurocom = new EurocomImpl())
      {
        IDValidator identity = new IDValidator(message.IDNo);
        string gender = "m";

        if (identity.isValid())
          gender = identity.IsFemale() ? "f" : "m";

        var cell = Regex.Replace(message.CellNo, @"[^.0-9]", String.Empty);
        message.CellNo = Regex.Replace(cell, @"^\+?0", "27");

        if (eurocom.InjectProfile(CAMPAIGN_ID, string.Format("{0} {1}", message.FirstName, message.LastName),
          message.IDNo,
          message.CellNo, gender, message.BranchNo.ToString(), message.RegionNo.ToString()))
        {
          if (eurocom.ActivateLoyalty(CAMPAIGN_ID, message.IDNo))
          {

            var replyMessage =
              AutoMapper.Mapper.Map<CouponIssueStartRequestMessage, CouponIssueCompletedRequestMessage>(message);

            replyMessage.IsSuccess = true;
            replyMessage.Message = CAMPAIGN_MSG;
            replyMessage.CampaignId = CAMPAIGN_ID;

            this.Context().Bus.Publish(replyMessage);
          }
        }
      }
    }

    public void Dispose()
    {
      // Do nothing
    }

    public void Start(IServiceBus _bus)
    {
      bus = _bus;
      unsubscribeAction = bus.SubscribeConsumer<CouponIssueConsumer>();
    }

    public void Stop()
    {
      unsubscribeAction();
    }
  }
}
