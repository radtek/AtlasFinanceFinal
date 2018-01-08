using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.RabbitMQ.Messages.Push;
using Falcon.Service.Controllers;
using Falcon.Service.Helpers;
using Serilog;

namespace Falcon.Service.Core
{
  public static class RabbitMessageHandler
  {
    private static readonly ILogger _log = Log.Logger;

    public static void PushMessageHandler(PushMessage message)
    {
      Func<Dictionary<string, object>, string[], bool> validate = (parameters, keys) => keys.All(parameters.ContainsKey);

      switch (message.Type)
      {
        case PushMessage.PushType.AVS:
          if (validate(message.Parameters, new[] {"AccountId"}))
          {
            long accountId;
            if (long.TryParse(message.Parameters["AccountId"].ToString(), out accountId))
            {
              var accountDetail = new AccountDetail();
              accountDetail.SendAvsUpdate(accountId);
            }
          }
          break;
        case PushMessage.PushType.NAEDO:
          if (validate(message.Parameters, new[] {"AccountId"}))
          {
            long accountId;
            if (long.TryParse(message.Parameters["AccountId"].ToString(), out accountId))
            {
              var accountDetail = new AccountDetail();
              accountDetail.SendNaedoUpdate(accountId);
            }
          }
          break;
        case PushMessage.PushType.Payout:
          if (validate(message.Parameters, new[] {"AccountId"}))
          {
            long accountId;
            if (long.TryParse(message.Parameters["AccountId"].ToString(), out accountId))
            {
              var accountDetail = new AccountDetail();
              accountDetail.SendPayoutUpdate(accountId);
            }
          }
          break;
        case PushMessage.PushType.FingerPrint:
          if (validate(message.Parameters, new[] {"Authenticated", "TrackingId", "Checksum", "HasError", "Error"}))
          {
            _log.Information("[Push Message] - FingerPrint - Validating parameters...");

            var authenticated = Boolean.Parse(message.Parameters["Authenticated"].ToString());
            var trackingId = message.Parameters["TrackingId"].ToString();
            var checkSum = message.Parameters["Checksum"].ToString();
            var hasError = Boolean.Parse(message.Parameters["HasError"].ToString());


            var hash = Hashing.GetSHA256(string.Format("{0}{1}{2}{3}{4}", authenticated, trackingId, hasError, string.Empty,
                            "12312312312"));

            if (hash != checkSum)
            {
              _log.Warning(
                string.Format(
                  "[Push Message] - FingerPrint - Checksum mismatch expected {checksum} got {checksumcomputed}",
                  checkSum, hash));
              break;
            }
            _log.Information("[Push Message] - FingerPrint - Checksum match");

            var fp = new FingerPrint();
            // Update the web ui
            fp.SendPushMessageResult(Guid.Parse(trackingId), message);

            //long accountId = 0;
            //if (long.TryParse(message.Parameters["AccountId"].ToString(), out accountId))
            //{
            //  Falcon.Service.Controllers.AccountDetail accountDetail = new Controllers.AccountDetail();
            //  accountDetail.SendPayoutUpdate(accountId);
            //}
          }
          break;
      }
    }
  }
}