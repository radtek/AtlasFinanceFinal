using System;
using System.Configuration;
using System.Linq;

using DevExpress.Xpo;

using Atlas.RabbitMQ.Messages.Coupon;
using Atlas.Domain.Model;


namespace Atlas.WCF.Implementation
{
  public static class MessagingBus
  {
    /// <summary>
    /// Start the RabbitMQ bus
    /// </summary>
    public static void StartBus()
    {
      /*
      var RABBITMQ_ADDRESS = ConfigurationManager.AppSettings["rabbitmq-server-address"];
      var RABBITMQ_SENDCOUPON_BINDING = ConfigurationManager.AppSettings["rabbitmq-coupon-binding"];

      _bus = ServiceBusFactory.New(config =>
      {
        config.UseRabbitMq(
        r => r.ConfigureHost(new Uri(string.Format("rabbitmq://{0}/{1}", RABBITMQ_ADDRESS, RABBITMQ_SENDCOUPON_BINDING)),
          h =>
          {
            h.SetUsername(ConfigurationManager.AppSettings["rabbitmq-username"]);
            h.SetPassword(ConfigurationManager.AppSettings["rabbitmq-password"]);
            //#if !DEBUG
            h.SetRequestedHeartbeat(125);
            //#endif
          }));

        config.ReceiveFrom(string.Format("rabbitmq://{0}/{1}", RABBITMQ_ADDRESS, RABBITMQ_SENDCOUPON_BINDING));
        //config.UseControlBus();
        //config.EnableRemoteIntrospection();
        //config.EnableMessageTracing();
      });
      */
    }


    /// <summary>
    /// Deliver a SMS using the message queue
    /// </summary>
    /// <param name="cellNum"></param>
    /// <param name="smsMessage"></param>
    public static void SendCoupon(string firstName, string surname, string idNum, string cellNum, string branch)
    {
      /*
      var methodName = "SendCoupon";

      try
      {
        #region Legacy branch to more comprehensive branch/region details
        string fullBranch;
        string fullRegion;
        int region;
        using (var unitOfWork = new UnitOfWork())
        {
          var branchDb = unitOfWork.Query<BRN_Branch>().FirstOrDefault(s => s.LegacyBranchNum.PadLeft(3, '0') == branch.PadLeft(3, '0'));
          if (branchDb == null || branchDb.Region == null || branchDb.Company == null)
          {
            _log.Error(new Exception(string.Format("Failed to locate branch/region/company details for branch: '{0}'", branch)), methodName);
            return;
          }

          fullBranch = string.Format("{0}- ({1})", branchDb.Company.Name, branchDb.LegacyBranchNum);
          fullRegion = string.Format("{0}- ({1})", branchDb.Region.Description, branchDb.Region.LegacyRegionCode);
          region = int.Parse(branchDb.Region.LegacyRegionCode);
        }
        #endregion

        #region Publish
        if (_bus != null)
        {
          var message = new CouponIssueRequestMessage
            {
              CorrelationId = Magnum.CombGuid.Generate(),              
              FirstName = firstName,
              LastName = surname,
              IDNo = idNum,
              BranchNo = branch,
              BranchDescription = fullBranch,
              RegionNo = region,
              RegionDescription = fullRegion,
              CellNo = cellNum
            };
          _log.Information("{MethodName}- Sending to message bus: {@Message}", methodName, message);
          _bus.Publish<CouponIssueRequestMessage>(message);
        }
        else
        {
          _log.Error(new NullReferenceException("SendCoupon called while static _bus is null"), methodName);
        }
        #endregion

      }
      catch (Exception err)
      {
        _log.Error(err, methodName);
      }
      */
    }


    /// <summary>
    /// Shut-down the service bus
    /// </summary>
    public static void ShutdownBus()
    {
      /*
      if (_bus != null)
      {
        _bus.Dispose();
        _bus = null;
      }
      */
    }

    /// <summary>
    /// String to send back to client when an unexpected server error occurs
    /// </summary>
    public static readonly string SERVER_ERR_UNEXPECTED = "Unexpected server error";


    /// <summary>
    /// The log4Net logging
    /// </summary>
    //private static readonly ILogger _log = Log.Logger;

    /// <summary>
    /// Our entry point, to be able to send messages to endpoints
    /// </summary>
    //private static IServiceBus _bus = null;

  }
}
