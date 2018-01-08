using System;
using System.Linq;

using DevExpress.Xpo;
using Newtonsoft.Json;

using Atlas.Domain.Model;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.Common;
using Atlas.WCF.FPServer.FPSocketServer;
using Atlas.WCF.FPServer.ClientState;
using Atlas.Common.Interface;
using Atlas.Domain.Model.Biometric;
using Atlas.Enumerators;
using Atlas.Domain.Security;

namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  public static class LMS_AddRequest_Impl
  {
    public static int Execute(ILogging log, SourceRequest sourceRequest, FPRequestType requestType,
      Int64 personId, Int64 userPersonId, Int64 adminPersonId,
      string message1, string message2, string message3, int timeoutSecs,
      out string requestId, out string errorMessage)
    {
      var methodName = "LMS_AddRequest";
      //log.Information("{MethodName} starting: {@Request}", methodName, new
      //{
      //  sourceRequest,
      //  requestType,
      //  personId,
      //  userPersonId,
      //  adminPersonId,
      //  message1,
      //  message2,
      //  message3,
      //  timeoutSecs
      //});

      errorMessage = string.Empty;
      requestId = Guid.NewGuid().ToString("N");

      try
      {
        #region Check parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }

        if (requestType == FPRequestType.NotSet)
        {
          errorMessage = "RequestType must be set";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }

        if (timeoutSecs <= 0)
        {
          errorMessage = string.Format("Invalid tineoutSecs parameter: {0}", timeoutSecs);
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }

        if (requestType != FPRequestType.Identification)
        {
          // Person must exist if not identification
          if (personId <= 0)
          {
            errorMessage = string.Format("Invalid personId parameter: {0}", personId);
            log.Warning(new Exception(errorMessage), methodName);
            return (int)General.WCFCallResult.BadParams;
          }

          using (var unitOfWork = new UnitOfWork())
          {
            var person = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == personId);
            if (person == null)
            {
              errorMessage = string.Format("Person with ID '{0}' could not be located", personId);
              log.Warning(new Exception(errorMessage), methodName);
              return (int)General.WCFCallResult.BadParams;
            }

            // Find user
            if (user == null && userPersonId > 0)
            {
              var userDb = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == userPersonId);
              if (userDb == null)
              {
                errorMessage = string.Format("User person with ID '{0}' could not be located", userPersonId);
                log.Warning(new Exception(errorMessage), methodName);
                return (int)General.WCFCallResult.BadParams;
              }
              user = AutoMapper.Mapper.Map<User>(userDb);
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
            return (int)General.WCFCallResult.BadParams;
          }
        }
        #endregion

        FpTcpUtils.FpFunctions messageType;
        var requestStatus = Biometric.RequestStatus.None;
        switch (requestType)
        {
          case FPRequestType.Enroll:
            messageType = FpTcpUtils.FpFunctions.Enroll;
            requestStatus = Biometric.RequestStatus.EnrollmentRequested;

            #region Ensure we do not already have templates for this person
            var fingersEnrolledE = Atlas.WCF.FPServer.Comms.DistCommUtils.GetFingersEnrolled(personId);
            if (fingersEnrolledE != null && fingersEnrolledE.Any())
            {
              errorMessage = "Person has already been enrolled with fingerprints";
              log.Warning(new Exception(errorMessage), methodName);
              return (int)General.WCFCallResult.BadParams;
            }
            #endregion

            #region Ensure user id provided
            if (user == null)
            {
              errorMessage = string.Format("Operator '{0}' not found or not specified", sourceRequest.UserIDOrPassport);
              log.Warning(new Exception(errorMessage), methodName);
              return (int)General.WCFCallResult.BadParams;
            }
            #endregion

            break;

          case FPRequestType.Verify:
            #region Is there a verification override for this person?
            string overrideForPerson = null;
            using (var unitOfWork = new UnitOfWork())
            {
              overrideForPerson = unitOfWork.Query<BIO_Setting>().FirstOrDefault(s =>
                s.AppliesTo == Biometric.AppliesTo.PersonSpecific &&
                s.Entity == personId &&
                s.SettingType == Biometric.SettingType.FPClientEnabled)?.Value;
              
              if (overrideForPerson == "N") // FP verification over-ride- assume success
              {
                new BIO_LogRequest(unitOfWork)
                {
                  BiometricClass = Biometric.BiometricClass.Fingerprint,
                  Machine = unitOfWork.Query<COR_Machine>().FirstOrDefault(s => s.MachineId == machine.Id),
                  Person = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == personId),
                  UserPerson = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == userPersonId),
                  StartDT = DateTime.Now,
                  EndDT = DateTime.Now,
                  Error = "Person has verification over-ride- physical verification was skipped",
                  RequestId = requestId,
                  TimeoutSecs = timeoutSecs,
                  FPResult = Biometric.RequestStatus.VerifySuccessful,
                };

                unitOfWork.CommitChanges();

                LMSGuiState.LmsGuiAddRequest(machine.Id, requestId, Biometric.RequestStatus.VerifySuccessful,
                  personId, userPersonId, adminPersonId, message1, message2, message3, timeoutSecs, false);

                return (int)General.WCFCallResult.OK;
              }              
            }
            #endregion

            messageType = FpTcpUtils.FpFunctions.Verify;
            requestStatus = Biometric.RequestStatus.VerifyRequested;

            #region Ensure we have templates for this person
            var fingersEnrolledV = Atlas.WCF.FPServer.Comms.DistCommUtils.GetFingersEnrolled(personId);
            if (fingersEnrolledV == null || !fingersEnrolledV.Any())
            {
              errorMessage = string.Format("Person has not been enrolled with fingerprints (id: {0})", personId);
              log.Warning(new Exception(errorMessage), methodName);
              return (int)General.WCFCallResult.BadParams;
            }
            #endregion

            break;

          case FPRequestType.Identification:
            messageType = FpTcpUtils.FpFunctions.Identify;
            requestStatus = Biometric.RequestStatus.IdentificationRequested;

            break;

          default:
            errorMessage = "Request type not handled";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)General.WCFCallResult.BadParams;
        }

        if (user != null && userPersonId == 0)
        {
          userPersonId = user.PersonId;
        }

        // Store the request for server tracking
        LMSGuiState.LmsGuiAddRequest(machine.Id, requestId, requestStatus, personId, userPersonId, adminPersonId, message1, message2, message3, timeoutSecs, false);

        // Activate the client via socket
        FpTcpClientComms.SendClientMessage(machine.HardwareKey, messageType,
          JsonConvert.SerializeObject(new ClientActivateInfo
          {
            AdminPersonId = adminPersonId,
            Message1 = message1,
            Message2 = message2,
            Message3 = message3,
            PersonId = personId,
            TimeoutSeconds = timeoutSecs,
            MessageId = requestId,
            UserPersonId = userPersonId
          }));

        //log.Information("{MethodName} completed successfully", methodName);
        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = FPActivation.SERVER_ERR_UNEXPECTED;
        requestId = null;
        return (int)General.WCFCallResult.ServerError;
      }
    }
  }
}
