using System;

using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.Common;
using Atlas.Common.Interface;


namespace Atlas.WCF.FPServer.WCF.Implementation.Comms
{
  public static class LMS_IsMachineFPReady_Impl
  {
    public static int Execute(ILogging log, SourceRequest sourceRequest, out bool isReady, out string errorMessage)
    {
      var methodName = "LMS_IsMachineFPReady";
      //log.Information("{MethodName} starting: {@Request}", methodName, sourceRequest);

      isReady = false;
      errorMessage = string.Empty;
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
        #endregion

        var hwInfo = Atlas.WCF.FPServer.ClientState.LMSGuiState.LmsGuiGetHWStatus(sourceRequest.MachineUniqueID);
        isReady = hwInfo != null && hwInfo.FPDeviceCount > 0;

        //log.Information("{MethodName} completed successfully with result: {IsReady}", methodName, isReady);
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
