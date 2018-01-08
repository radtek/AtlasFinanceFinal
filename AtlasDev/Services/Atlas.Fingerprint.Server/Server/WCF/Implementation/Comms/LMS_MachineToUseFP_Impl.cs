using System;
using System.Linq;

using DevExpress.Xpo;

using Atlas.WCF.FPServer.Common;
using Atlas.Domain.Model.Biometric;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  public static class LMS_MachineToUseFP_Impl
  {
    public static int Execute(ILogging log, SourceRequest sourceRequest, out bool isEnabled, out string errorMessage)
    {      
      var methodName = "LMS_MachineToUseFP";
      log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest); 

      errorMessage = string.Empty;
      isEnabled = false;
      try
      {
        #region Check parameters
        Machine machine;
        User user;
        Int64 branchId;
        if (!WCFUtils.CheckSourceRequest(log, sourceRequest, out branchId, out machine, out user, out errorMessage))
        {
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }

        if (branchId <= 0)
        {
          errorMessage = string.Format("Unable to locate branch '{0}'", sourceRequest.BranchCode);
          log.Warning(new Exception(errorMessage), methodName);
          return (int)Enumerators.General.WCFCallResult.BadParams;
        }
        #endregion

        #region Find setting in Bio Config
        using (var unitOfWork = new UnitOfWork())
        {
          #region Check if machine enabled
          var machineEnabled = unitOfWork.Query<BIO_Setting>()
            .FirstOrDefault(s =>
              s.AppliesTo == Enumerators.Biometric.AppliesTo.Station
              && s.Entity == machine.Id
              && s.SettingType == Enumerators.Biometric.SettingType.FPEnabled);

          if (machineEnabled != null)
          {
            isEnabled = machineEnabled.Value.ToUpper().Trim() == "Y" || machineEnabled.Value.Trim() == "1" || machineEnabled.Value.ToUpper().Trim() == "T";
            log.Information("{MethodName} completed successfully with result (machine): {IsEnabled}", methodName, isEnabled);
            return (int)Enumerators.General.WCFCallResult.OK;
          }
          #endregion

          #region Check if branch enabled
          var branchEnabled = unitOfWork.Query<BIO_Setting>()
            .FirstOrDefault(s =>
              s.AppliesTo == Enumerators.Biometric.AppliesTo.Branch
              && s.Entity == branchId
              && s.SettingType == Enumerators.Biometric.SettingType.FPEnabled);

          if (branchEnabled != null)
          {
            isEnabled = branchEnabled.Value.ToUpper().Trim() == "Y" || branchEnabled.Value.Trim() == "1" || branchEnabled.Value.ToUpper().Trim() == "T";
            log.Information("{MethodName} completed successfully with result (branch): {IsEnabled}", methodName, isEnabled);
            return (int)Enumerators.General.WCFCallResult.OK;
          }
          #endregion
        }
        #endregion

        log.Information("{MethodName} completed successfully with result (default): {IsEnabled}", methodName, isEnabled);
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