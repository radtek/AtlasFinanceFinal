using System;

using Atlas.Common.Interface;
using Atlas.Enumerators;
using Atlas.WCF.FPServer.Comms;
using Atlas.WCF.FPServer.WCF.Implementation.Server.Admin;


namespace Atlas.WCF.FPServer.WCF.Implementation.Server.Enroll
{
  internal static class UnEnrollPerson_Impl
  {
    internal static int UnEnrollPerson(ILogging log, Security.Interface.SourceRequest sourceRequest, long personId, out string errorMessage)
    {
      var methodName = "UnEnrollPerson";
      log.Information("{MethodName}", methodName);
      try
      {
        #region Check parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning("UnEnrollPerson", new Exception(errorMessage));
          return (int)General.WCFCallResult.BadParams;
        }

        if (personId <= 0)
        {
          errorMessage = "Parameter personId cannot be empty";
          log.Warning("UnEnrollPerson", new Exception(errorMessage));
          return (int)General.WCFCallResult.BadParams;
        }
        #endregion
        
        #region Remove from mongodb
        var deleteBitmaps = DeleteData_Impl.DeleteData(log, sourceRequest, personId, Interface.FPDataType.Bitmap, out errorMessage);
        var deleteTemplates = DeleteData_Impl.DeleteData(log, sourceRequest, personId, Interface.FPDataType.Template, out errorMessage);          
        #endregion

        // Notify all distributed identifier endpoints
        if (deleteTemplates == (int)General.WCFCallResult.OK)
        {
          DistCommUtils.PublishDeletedFingerprint(personId);
          // TODO: Broadcast to all socket endpoints?
        }
        
        return (int)General.WCFCallResult.OK;
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}", methodName);
        errorMessage = "Unexpected server error";
        return (int)General.WCFCallResult.ServerError;
      }
    }
  }
}
