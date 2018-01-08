using System;
using System.Linq;

using Atlas.Domain.Model;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.Common;
using Atlas.Common.Interface;

using DevExpress.Xpo;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  public static class LMS_CheckRequestStatus_Impl
  {
    public static int Execute(ILogging log, SourceRequest sourceRequest, string requestId,
      out FPRequestStatus status, out BasicPersonDetailsDTO person,
      out string errorMessage)
    {
      var methodName = "LMS_CheckRequestStatus";
      //_log.Information("{MethodName} starting: {@Request}, {RequestId}", methodName, sourceRequest, requestId);

      errorMessage = string.Empty;
      status = FPRequestStatus.NotSet;
      person = null;

      try
      {
        #region check parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }

        if (string.IsNullOrEmpty(requestId))
        {
          errorMessage = "requestId missing";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }
        #endregion

        var request = Atlas.WCF.FPServer.ClientState.LMSGuiState.GetGUICommStatus(machine.Id, requestId);
        if (request != null)
        {
          errorMessage = request.ErrorMessage;

          switch (request.Status)
          {
            case Atlas.Enumerators.Biometric.RequestStatus.IdentificationSuccessful:
              if (request.PersonId > 0)
              {
                using (var unitofWork = new UnitOfWork())
                {
                  person = AutoMapper.Mapper.Map<BasicPersonDetailsDTO>(unitofWork.Query<PER_Person>().First(s => s.PersonId == request.PersonId));
                }

                status = FPRequestStatus.Successful;
              }
              else
              {
                status = FPRequestStatus.Failed;
              }

              break;

            case Atlas.Enumerators.Biometric.RequestStatus.VerifySuccessful:
            case Atlas.Enumerators.Biometric.RequestStatus.EnrollmentSuccessful:
              status = FPRequestStatus.Successful;
              break;

            case Atlas.Enumerators.Biometric.RequestStatus.EnrollmentCancelled:
              errorMessage = request.ErrorMessage ?? "Enrollment was cancelled";
              status = FPRequestStatus.Failed;
              break;

            case Atlas.Enumerators.Biometric.RequestStatus.EnrollmentFailed:
              errorMessage = request.ErrorMessage ?? "Enrollment failed";
              status = FPRequestStatus.Failed;
              break;

            case Atlas.Enumerators.Biometric.RequestStatus.VerifyFailed:
              errorMessage = request.ErrorMessage ?? "Verification failed";
              status = FPRequestStatus.Failed;
              break;

            case Atlas.Enumerators.Biometric.RequestStatus.VerifyCancelled:
              errorMessage = request.ErrorMessage ?? "Verification was cancelled";
              status = FPRequestStatus.Failed;
              break;

            case Atlas.Enumerators.Biometric.RequestStatus.IdentificationFailed:
              errorMessage = request.ErrorMessage ?? "Identification failed";
              status = FPRequestStatus.Failed;
              break;

            case Atlas.Enumerators.Biometric.RequestStatus.IdentificationCancelled:
              errorMessage = request.ErrorMessage ?? "Identification was cancelled";
              status = FPRequestStatus.Failed;
              break;

            default:
              status = FPRequestStatus.NotSet;

              break;
          }

          if (status != FPRequestStatus.NotSet)
          {
            //log.Information("{MethodName} completed with result: {@Person}, {Status}, {Error} ", methodName, person, status, errorMessage);
          }
          return (int)Enumerators.General.WCFCallResult.OK;
        }
        else
        {
          errorMessage = "Request could not be located";
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }
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
