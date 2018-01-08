using System;

using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.Common;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  public static class FPC_CheckForNewRequest_Impl
  {
    /// <summary>
    /// Local Fingerprint GUI Client- check for new requests from other system(s)
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="requestId">Request ID (GUID)</param>
    /// <param name="requestType">Returns the request type</param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static int Execute(ILogging log, SourceRequest sourceRequest, out string requestId,
      out FPRequestType requestType, out Int64 personId, out Int64 userPersonId, out Int64 adminPersonId,
      out string message1, out string message2, out string message3, out int timeoutSecs, out string errorMessage)
    {
      var methodName = "FPC_CheckForNewRequest";
      //_log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      requestType = FPRequestType.NotSet;
      errorMessage = null;
      requestId = null;
      message1 = null;
      message2 = null;
      message3 = null;
      timeoutSecs = 0;
      personId = 0;
      userPersonId = 0;
      adminPersonId = 0;
      try
      {
        #region Check parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), "{MethodName}- {@Request}", methodName, sourceRequest);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }
        #endregion

        #region Return first pending request for this machine
        var requests = Atlas.WCF.FPServer.ClientState.LMSGuiState.LmsGuiGetPendingRequests(machine.Id);
        var newStatus = Atlas.Enumerators.Biometric.RequestStatus.None;
        foreach (var request in requests)
        {
          switch (request.Status)
          {
            case Atlas.Enumerators.Biometric.RequestStatus.EnrollmentRequested:
              requestType = FPRequestType.Enroll;
              newStatus = Atlas.Enumerators.Biometric.RequestStatus.EnrollmentPending;
              break;

            case Atlas.Enumerators.Biometric.RequestStatus.VerifyRequested:
              requestType = FPRequestType.Verify;
              newStatus = Atlas.Enumerators.Biometric.RequestStatus.VerifyPending;
              break;

            case Atlas.Enumerators.Biometric.RequestStatus.IdentificationRequested:
              requestType = FPRequestType.Identification;
              newStatus = Atlas.Enumerators.Biometric.RequestStatus.IdentificationPending;
              break;
          }

          if (requestType != FPRequestType.NotSet)
          {
            requestId = request.RequestId;
            message1 = request.Message1;
            message2 = request.Message2;
            message3 = request.Message3;
            timeoutSecs = request.TimeoutSecs;
            personId = request.PersonId;
            userPersonId = request.UserPersonId;
            adminPersonId = request.AdminPersonId;

            Atlas.WCF.FPServer.ClientState.LMSGuiState.LmsGuiSetCommStatus(machine.Id, requestId, newStatus);

            break;
          }
        }
        #endregion
        if (requestType != FPRequestType.NotSet)
        {
          //log.Information("{MethodName} completed successfully with result: {@Result}", methodName,
          //  new { requestId, requestType, personId, userPersonId, adminPersonId, message1, message2, message3, timeoutSecs, errorMessage });
        }

        return (int)Enumerators.General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = FPActivation.SERVER_ERR_UNEXPECTED;
        return (int)Enumerators.General.WCFCallResult.ServerError;
      }
    }

  }
}
