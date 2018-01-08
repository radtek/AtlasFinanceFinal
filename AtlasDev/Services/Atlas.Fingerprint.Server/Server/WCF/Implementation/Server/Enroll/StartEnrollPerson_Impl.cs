using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.Domain.Model.Biometric;
using Atlas.Domain.Security;
using Atlas.Enumerators;
using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.Common;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Server.Enroll
{
  public static class StartEnrollPerson_Impl
  {
    public static int StartEnrollPerson(ILogging log, SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions,
      Int64 personId,
      out Int64 startEnrollRef, out string errorMessage)
    {
      var methodName = "StartEnrollPerson";
      startEnrollRef = 0;

      try
      {
        log.Information("{MethodName} starting: {@Request}, {@Reference}, {@Scanner}, {@ScannerOptions}", 
          methodName, sourceRequest, startEnrollRef, scanner, scannerOptions);

        #region Check parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning("StartEnrollPerson", new Exception(errorMessage));
          return (int)General.WCFCallResult.BadParams;
        }

        if (personId <= 0)
        {
          errorMessage = "Parameter personId cannot be empty";
          log.Warning("StartEnrollPerson", new Exception(errorMessage));
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        using (var unitOfWork = new UnitOfWork())
        {
          #region Ensure person exists
          if (!unitOfWork.Query<PER_Person>().Any(s => s.PersonId == personId))
          {
            errorMessage = "Person does not exist";
            log.Warning("StartEnrollPerson", new Exception(errorMessage));
            return (int)General.WCFCallResult.BadParams;
          }
          #endregion

          #region Delete any existing session for this machine
          var session = unitOfWork.Query<BIO_UploadSession>().FirstOrDefault(s => s.Machine.MachineId == machine.Id);
          if (session != null)
          {
            // Delete bitmaps
            var bitmaps = unitOfWork.Query<BIO_UploadBitmap>().Where(s => s.FPUploadSession == session);
            foreach (var bitmap in bitmaps)
            {
              bitmap.Delete();
            }

            // Delete templates
            var templates = unitOfWork.Query<BIO_UploadTemplate>().Where(s => s.FPUploadSession == session);
            foreach (var template in templates)
            {
              template.Delete();
            }

            session.Delete();
            unitOfWork.CommitChanges();
          }
          #endregion

          #region Start a new session
          var newSession = new BIO_UploadSession(unitOfWork)
          {
            Machine = unitOfWork.Query<COR_Machine>().First(s => s.MachineId == machine.Id),
            StartDate = DateTime.Now,
            PersonId = personId,
            UserPersonId = sourceRequest.UserPersonId,
            AdminPersonId = sourceRequest.AdminPersonId,
          };

          unitOfWork.CommitChanges();
          startEnrollRef = newSession.FPUploadSessionId;          
          #endregion

          log.Information("{MethodName} completed successfully", methodName);
          return (int)General.WCFCallResult.OK;
        }
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
