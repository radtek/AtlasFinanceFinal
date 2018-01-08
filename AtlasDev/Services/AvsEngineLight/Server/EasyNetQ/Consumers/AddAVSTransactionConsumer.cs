using System;

using AvsEngineLight.DB;
using BankVerification.EasyNetQ;
using Atlas.Enumerators;
using Atlas.Common.Extensions;
using Atlas.Common.Interface;


namespace AvsEngineLight.EasyNetQ.Consumers
{
  internal class AddAVSTransactionConsumer
  {
    public static AVSResponse Handle(ILogging log, AddAVSRequest message)
    {
      log.Information("AddAVSTransactionConsumer- {@Message}", message);
      
      var responseMessage = new AVSResponse
      {
        CreatedAt = DateTime.Now
      
      };

      try
      {        
        if (!ValidateMessage(message))
        {
          log.Error("Invalid message parameters: {@Message}", message);
          responseMessage.Error = true;
          return responseMessage;
        }

        responseMessage.WaitingReply = true;

        /*if (message.ForceAVS ?? false)
        {
          responseMessage.TransactionId = AvsDbRepository.SaveNewAVSRequest(message, serviceToUse);
          bus.MessageContext<AddAVSTransaction>().Respond(responseMessage);
          return;
        }*/

        var transactionId = AvsDbRepository.CheckForDuplicateTrans(message.Bank, message.AccountNo, message.BranchCode,
          message.IdNumber, message.LastName, message.Initials);
        if (transactionId <= 0)
        {
          var serviceToUse = AvsDbRepository.DecideAVSSupplier(message.Bank, message.Host, message.BranchCode, message.IdNumber, log);
          transactionId = AvsDbRepository.SaveNewAVSRequest(message, serviceToUse);
        }

        var transaction = AvsDbRepository.FindAVSTransactionId(transactionId);

        responseMessage.TransactionId = transactionId;
        responseMessage.AccountExists = (string.Compare(transaction.ResponseAccountNumber, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
        responseMessage.IdNumberMatch = (string.Compare(transaction.ResponseIdNumber, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
        responseMessage.InitialsMatch = (string.Compare(transaction.ResponseInitials, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
        responseMessage.LastNameMatch = (string.Compare(transaction.ResponseLastName, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
        responseMessage.AccountOpen = (string.Compare(transaction.ResponseAccountOpen, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
        responseMessage.AccountAcceptsCredits = (string.Compare(transaction.ResponseAcceptsCredit, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
        responseMessage.AccountAcceptsDebits = (string.Compare(transaction.ResponseAcceptsDebit, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
        responseMessage.AccountOpen90days = (string.Compare(transaction.ResponseOpenThreeMonths, AVS.ResponseCode.Match.ToStringEnum(), true) == 0);
        responseMessage.FinalResult = transaction.Result == null ? (AVS.Result?)null : transaction.Result.Type;
        responseMessage.WaitingReply = transaction.Result == null;                
        
        log.Information("AddAVSTransactionConsumer- {@Response}", responseMessage);
        return responseMessage;
      }
      catch (Exception err)
      {
        log.Error(err, "AddAVSTransactionConsumer.Handle");
        responseMessage.Error = true;
        return responseMessage;
      }
    }


    #region Private methods

    /// <summary>
    /// Perform basic message validation
    /// </summary>
    /// <param name="avsMessage"></param>
    /// <returns></returns>
    private static bool ValidateMessage(AddAVSRequest avsMessage)
    {
      if ((int)avsMessage.Bank <= 0)
        return false;
      if ((int)avsMessage.Host <= 0)
        return false;
      if (string.IsNullOrEmpty(avsMessage.Initials))
        return false;
      if (string.IsNullOrEmpty(avsMessage.LastName))
        return false;
      if (string.IsNullOrEmpty(avsMessage.IdNumber))
        return false;
      if (string.IsNullOrEmpty(avsMessage.BranchCode))
        return false;
      if (string.IsNullOrEmpty(avsMessage.AccountNo))
        return false;

      return true;
    }

    #endregion

  }
}
