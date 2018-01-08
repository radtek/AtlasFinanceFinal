using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model.Biometric;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.WCF.Implementation.Comms.Utils;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  public static class FPC_CheckForOpenSession_Impl
  {
    /// <summary>
    /// Used for fingerprint enrollmemt recovery
    /// </summary>
    /// <param name="log"></param>
    /// <param name="sourceRequest"></param>
    /// <returns></returns>
    public static RecoveryInfoDTO Execute(ILogging log, SourceRequest sourceRequest)
    {
      RecoveryInfoDTO result = null;
      var methodName = "FPC_CheckForOpenSession";
      try
      {
        #region check parameters
        Machine machine;
        User user;
        Int64 branchId;
        string errorMessage;

        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return null;
        }
        #endregion

        #region Get pending enrolment request for this machine, which has not timed out
        var requests = Atlas.WCF.FPServer.ClientState.LMSGuiState.LmsGuiGetPendingRequests(machine.Id, 6000 /* Large- we check timeout further down... */);
        if (requests == null)
        {
          return null;
        }

        // Find pending enroll requests which have not timed out...
        var enrollRequest = requests.FirstOrDefault(s => 
          (s.Status == Enumerators.Biometric.RequestStatus.EnrollmentPending || 
          s.Status == Enumerators.Biometric.RequestStatus.EnrollmentRequested) && 
          DateTime.Now.Subtract(s.Started).TotalSeconds < s.TimeoutSecs);
        if (enrollRequest == null)
        {
          return null;
        }
        #endregion

        #region Get FPs done from SQL DB session- match person and machine, to be sure there is no session confusion...
        string fpsCsvDoneList;
        Int64 sessionId = 0;
        BasicPersonDetailsDTO person = null;
        if (!WCFCommsUtils.PersonToBasicPersonDetailsDTO(Atlas.WCF.FPServer.WCF.Implementation.Comms.Utils.WCFCommsUtils.FindPersonByField.ByPersonId, 
          enrollRequest.PersonId.ToString(), out person))
        {
          errorMessage = string.Format("Unable to locate person with personId: {0}", enrollRequest.PersonId);
          log.Warning(new Exception(errorMessage), methodName);
          return null;
        }

        using (var unitOfWork = new UnitOfWork())
        {
          var session = unitOfWork.Query<BIO_UploadSession>()
            .FirstOrDefault(s => s.Machine.MachineId == machine.Id && s.PersonId == enrollRequest.PersonId);
          if (session == null)
          {
            return null;
          }
          sessionId = session.FPUploadSessionId;
          
          var fpsDone = unitOfWork.Query<BIO_UploadTemplate>().Where(s => s.FPUploadSession.FPUploadSessionId == sessionId);
          if (!fpsDone.Any())
          {
            return null;
          }
          var fps = fpsDone.Select(s => s.FingerId).Distinct().ToList();
          fpsCsvDoneList = string.Join(",", fps.Select(s => s.ToString()));
        }
        #endregion

        result = new RecoveryInfoDTO(){
          CSVFingersDone = fpsCsvDoneList,
          Message1 = enrollRequest.Message1, 
          Message2 = enrollRequest.Message2,
          Message3 = enrollRequest.Message3,
          PersonId = enrollRequest.PersonId,
          UserPersonId = enrollRequest.UserPersonId,
          RequestId = enrollRequest.RequestId,
          TimeoutSecs = enrollRequest.TimeoutSecs - (int)DateTime.Now.Subtract(enrollRequest.Started).TotalSeconds,
          StartEnrollRef = sessionId,
          Person = person
        };

        return result;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        return null;
      }      
    }

  }
}
