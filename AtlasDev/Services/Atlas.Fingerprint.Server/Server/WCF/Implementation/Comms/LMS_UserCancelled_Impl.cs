using System;

using Newtonsoft.Json;

using Atlas.WCF.FPServer.FPSocketServer;
using Atlas.WCF.FPServer.ClientState;
using Atlas.Enumerators;
using Atlas.WCF.FPServer.Common;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  public class LMS_UserCancelled_Impl
  {
    public static int Execute(ILogging log, SourceRequest sourceRequest, string requestId, out string errorMessage)
    {
      errorMessage = string.Empty;

      var methodName = "LMS_UserCancelled";
      //log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      try
      {
        #region Check parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          return (int)General.WCFCallResult.BadParams;
        }

        if (string.IsNullOrEmpty(requestId))
        {
          errorMessage = "Missing value for requestId";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }

        var request = LMSGuiState.GetGUICommStatus(machine.Id, requestId);
        if (request == null)
        {
          errorMessage = "Request could not be located";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        #region Update status
        switch (request.Status)
        {
          case Biometric.RequestStatus.EnrollmentPending:
          case Biometric.RequestStatus.EnrollmentRequested:
            LMSGuiState.LmsGuiSetCommStatus(machine.Id, requestId, Biometric.RequestStatus.EnrollmentCancelled);
            break;

          case Biometric.RequestStatus.IdentificationPending:
          case Biometric.RequestStatus.IdentificationRequested:
            LMSGuiState.LmsGuiSetCommStatus(machine.Id, requestId, Biometric.RequestStatus.IdentificationCancelled);
            break;

          case Biometric.RequestStatus.VerifyPending:
          case Biometric.RequestStatus.VerifyRequested:
            LMSGuiState.LmsGuiSetCommStatus(machine.Id, requestId, Biometric.RequestStatus.VerifyCancelled);
            break;

          default:
            errorMessage = "Nothing to do";
            return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        FpTcpClientComms.SendClientMessage(machine.HardwareKey, FpTcpUtils.FpFunctions.Abort,
          JsonConvert.SerializeObject(new ClientActivateInfo { MessageId = requestId }));

        //log.Information("{MethodName} completed successfully", methodName);
        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = FPActivation.SERVER_ERR_UNEXPECTED;
        return (int)General.WCFCallResult.ServerError;
      }
    }

  }
}
