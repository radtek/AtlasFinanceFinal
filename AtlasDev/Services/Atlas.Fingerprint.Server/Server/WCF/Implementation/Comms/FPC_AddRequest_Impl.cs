using System;
using System.Linq;

using DevExpress.Xpo;
using Newtonsoft.Json;

using Atlas.Domain.Model;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.Common;
using Atlas.WCF.FPServer.FPSocketServer;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  public static class FPC_AddRequest_Impl
  {
    public static int Execute(ILogging log, SourceRequest sourceRequest, FPRequestType requestType,
      string trackingId, Int64 personId, Int64 userPersonId, Int64 adminPersonId,
      string message1, string message2, string message3, int timeoutSecs,
      out string errorMessage)
    {
      var methodName = "FPC_AddRequest";
      //log.Information("{MethodName} starting: {@Request}", methodName, new
      //{
      //  sourceRequest,
      //  requestType,
      //  trackingId,
      //  personId,
      //  userPersonId,
      //  adminPersonId,
      //  message1,
      //  message2,
      //  message3,
      //  timeoutSecs
      //});

      errorMessage = string.Empty;

      try
      {
        #region Check parameters
        Guid gui;
        if (string.IsNullOrEmpty(trackingId) || !Guid.TryParse(trackingId, out gui))
        {
          errorMessage = "trackingId must be set";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }

        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }

        if (requestType == FPRequestType.NotSet)
        {
          errorMessage = "RequestType must be set";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }

        if (timeoutSecs <= 0)
        {
          errorMessage = string.Format("Invalid tineoutSecs parameter: {0}", timeoutSecs);
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }

        if (requestType != FPRequestType.Identification)
        {
          // Person must exist if not identification
          if (personId <= 0)
          {
            errorMessage = string.Format("Invalid personId parameter: {0}", personId);
            log.Warning(new Exception(errorMessage), methodName);
            return (int)Enumerators.General.WCFCallResult.BadParams;
          }

          using (var unitOfWork = new UnitOfWork())
          {
            var person = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == personId);
            if (person == null)
            {
              errorMessage = "Person could not be located";
              log.Warning(new Exception(errorMessage), methodName);
              return (int)Enumerators.General.WCFCallResult.BadParams;
            }
          }
        }
        else
        {
          // Person must not be specified, this will be filled in by the identification process
          if (personId > 0)
          {
            errorMessage = "Person id cannot be specified for an identification";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)Enumerators.General.WCFCallResult.BadParams;
          }
        }

        FpTcpUtils.FpFunctions messageType;
        var requestStatus = Atlas.Enumerators.Biometric.RequestStatus.None;
        switch (requestType)
        {
          case FPRequestType.Enroll:
            messageType = FpTcpUtils.FpFunctions.Enroll;
            requestStatus = Atlas.Enumerators.Biometric.RequestStatus.EnrollmentRequested;

            #region Ensure we do not already have templates for this person
            var fingersEnrolled = Atlas.WCF.FPServer.Comms.DistCommUtils.GetFingersEnrolled(personId);
            if (fingersEnrolled != null || fingersEnrolled.Any())
            {
              errorMessage = "Person has already been enrolled with fingerprints";
              log.Warning(new Exception(errorMessage), methodName);
              return (int)Enumerators.General.WCFCallResult.BadParams;
            }
            #endregion

            break;

          case FPRequestType.Verify:
            messageType = FpTcpUtils.FpFunctions.Verify;
            requestStatus = Atlas.Enumerators.Biometric.RequestStatus.VerifyRequested;

            #region Ensure we have templates for this person
            var fingersEnrolledV = Atlas.WCF.FPServer.Comms.DistCommUtils.GetFingersEnrolled(personId);
            if (fingersEnrolledV == null || !fingersEnrolledV.Any())
            {
              errorMessage = string.Format("Person has not been enrolled with fingerprints (id: {0})", personId);
              log.Warning(new Exception(errorMessage), methodName);
              return (int)Enumerators.General.WCFCallResult.BadParams;
            }
            #endregion

            break;

          case FPRequestType.Identification:
            messageType = FpTcpUtils.FpFunctions.Identify;
            requestStatus = Atlas.Enumerators.Biometric.RequestStatus.IdentificationRequested;

            break;

          default:
            errorMessage = "Request type not handled";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)Enumerators.General.WCFCallResult.BadParams;
        }

        #endregion

        if (user != null && user != null && userPersonId == 0)
        {
          userPersonId = user.PersonId;
        }

        Atlas.WCF.FPServer.ClientState.LMSGuiState.LmsGuiAddRequest(machine.Id, trackingId, requestStatus, personId, userPersonId, adminPersonId, message1, message2, message3, timeoutSecs, true);

        FpTcpClientComms.SendClientMessage(machine.HardwareKey, messageType,
         JsonConvert.SerializeObject(new ClientActivateInfo
          {
            MessageId = trackingId,
            PersonId = personId,
            TimeoutSeconds = timeoutSecs,
            UserPersonId = userPersonId,
            AdminPersonId = adminPersonId,
            Message1 = message1,
            Message2 = message2,
            Message3 = message3
          }));

        //log.Information("{MethodName} completed successfully", methodName);
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
