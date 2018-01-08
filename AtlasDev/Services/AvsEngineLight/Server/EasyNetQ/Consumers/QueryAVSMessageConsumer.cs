using System;


using Atlas.Common.Extensions;
using AvsEngineLight.DB;
using BankVerification.EasyNetQ;
using Atlas.Enumerators;
using Atlas.Common.Interface;

namespace AvsEngineLight.EasyNetQ.Consumers
{
  internal class QueryAVSMessageConsumer
  {
    public static AVSResponse Handle(ILogging log, CheckAVSRequest message)
    {
      log.Information("QueryAVSMessageConsumer- {@Message}", message);

      var responseMessage = new AVSResponse
      {      
        CreatedAt = DateTime.Now,
      };

      try
      {
        var transaction = AvsDbRepository.FindAVSTransactionId(message.TransactionId);
        if (transaction == null)
        {
          log.Error("Failed to locate TransactionId- {TransactionId}", message.TransactionId);
          responseMessage.Error = true;
        }
        else
        {
          responseMessage.TransactionId = message.TransactionId;
          responseMessage.AccountExists = (string.Compare(transaction.ResponseAccountNumber, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
          responseMessage.IdNumberMatch = (string.Compare(transaction.ResponseIdNumber, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
          responseMessage.InitialsMatch = (string.Compare(transaction.ResponseInitials, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
          responseMessage.LastNameMatch = (string.Compare(transaction.ResponseLastName, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
          responseMessage.AccountOpen = (string.Compare(transaction.ResponseAccountOpen, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
          responseMessage.AccountAcceptsCredits = (string.Compare(transaction.ResponseAcceptsCredit, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
          responseMessage.AccountAcceptsDebits = (string.Compare(transaction.ResponseAcceptsDebit, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
          responseMessage.AccountOpen90days = (string.Compare(transaction.ResponseOpenThreeMonths, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
          responseMessage.FinalResult = transaction.Result?.Type ?? null;
          responseMessage.WaitingReply = transaction.Result == null;
        }

        log.Information("QueryAVSMessageConsumer- {@Response}", responseMessage);
        return responseMessage;
      }
      catch (Exception err)
      {
        log.Error(err, "QueryAVSMessageConsumer.Handle");
        responseMessage.Error = true;
        return responseMessage;
      }
    }
    

  }
}
