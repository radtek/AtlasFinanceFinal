using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.Model.Biometric;
using Atlas.Enumerators;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.Common;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Server.Enroll
{
  public static class CancelEnrollPerson_Impl
  {    public static int CancelEnrollPerson(ILogging log, SourceRequest sourceRequest, Int64 startEnrollRef, out string errorMessage)
    {
      var methodName = "CancelEnrollPerson";
      errorMessage = null;
      try
      {
        log.Warning("{MethodName} starting: {@Request}, {Reference}", methodName, sourceRequest, startEnrollRef);

        #region Validate parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion

        using (var unitOfWork = new UnitOfWork())
        {
          var session = unitOfWork.Query<BIO_UploadSession>().FirstOrDefault(s => s.FPUploadSessionId == startEnrollRef);
          if (session == null)
          {
            errorMessage = "No session found!";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)General.WCFCallResult.BadParams;
          }

          if (session.Machine.MachineId != machine.Id)
          {
            errorMessage = "Invalid session- session belongs to another machine";
            log.Warning(new Exception(errorMessage), methodName);
            return (int)General.WCFCallResult.BadParams;
          }

          var bitmaps = unitOfWork.Query<BIO_UploadBitmap>().Where(s => s.FPUploadSession == session);
          if (bitmaps != null)
          {
            foreach (var bitmap in bitmaps)
            {
              bitmap.Delete();
            }

          }
          var templates = unitOfWork.Query<BIO_UploadTemplate>().Where(s => s.FPUploadSession == session);
          if (templates != null)
          {
            foreach (var template in templates)
            {
              template.Delete();
            }
          }

          session.Delete();

          unitOfWork.CommitChanges();
        }

        log.Warning("{MethodName} completed successfully", methodName);
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
