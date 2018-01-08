using System;

using Atlas.Common.Interface;

using Atlas.Enumerators;
using AtlasServer.WCF.Interface;
using Atlas.Server.MessageBus.Notification;
using Atlas.Server.Classes.CustomException;


namespace Atlas.Server.WCF.Implementation.ASS
{
  internal static class SendSMS_Impl
  {
    internal static int Execute(ILogging log, 
      SourceRequest sourceRequest, string[] cellNumbers, string[] messages, out string errorMessage)
    {      
      errorMessage = string.Empty;
      var methodName = "SendSMS";
      try
      {
        #region Check params
        if (string.IsNullOrEmpty(sourceRequest.BranchCode))
        {
          throw new BadParamException("Missing branch number");
        }

        if (string.IsNullOrEmpty(sourceRequest.UserIDOrPassport))
        {
          throw new BadParamException("Missing operator ID");
        }

        if (string.IsNullOrEmpty(sourceRequest.MachineUniqueID))
        {
          throw new BadParamException("Missing machine fingerprint");
        }

        if (string.IsNullOrEmpty(sourceRequest.AppVer))
        {
          throw new BadParamException("Missing application version");
        }

        if (cellNumbers == null || messages == null)
        {
          throw new BadParamException("Missing cellNumbers/messages value");
        }

        if (cellNumbers.Length != messages.Length)
        {
          throw new BadParamException("The 'cellNumbers' and 'messages' parameters must contain the same amount of elements");
        }
        #endregion

        for (var i = 0; i < cellNumbers.Length; i++)
        {
          log.Information("Sending message to {0}- '{1}'", cellNumbers[i], messages[i]);
          NotificationDistCommUtils.SendSMS(cellNumbers[i], messages[i]);
          log.Information("Message placed on queue");
        }

        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = (err is BadParamException) ? err.Message : "Unexpected server error";
        return (err is BadParamException) ? (int)General.WCFCallResult.BadParams : (int)General.WCFCallResult.ServerError;
      }
    }
    
  }
}
