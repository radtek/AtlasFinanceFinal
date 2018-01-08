using System;

using Atlas.Server.WCF.Utils;
using Atlas.Common.Interface;
using Atlas.Server.Classes.CustomException;


namespace AtlasServer.WCF.Implementation
{
  internal static class CancelContract_Impl
  {
    internal static bool Execute(ILogging log, 
      string contractNo, string edoType, string edoReference, out string errorMessage)
    {
      var methodName = "CancelContract";
      errorMessage = string.Empty;     
      log.Information("{MethodName} starting, {@Params}", methodName, new { contractNo, edoType, edoReference });
      try
      {
        #region Check parameters
        if (string.IsNullOrEmpty(contractNo))
        {
          throw new BadParamException("Parameter 'contractNo' not specified");
        }

        if (string.IsNullOrEmpty(edoType) || (edoType != "N" && edoType != "A"))
        {
          throw new BadParamException("Parameter 'edoType' not specified/invalid");
        }

        if (string.IsNullOrEmpty(edoReference))
        {
          throw new BadParamException("Parameter 'edoReference' not specified");
        }
        #endregion

        errorMessage = EDOUtils.CancelAllPendingEDOTransactionsFor(log, edoType, contractNo, Int64.Parse(edoReference), null);
        return string.IsNullOrEmpty(errorMessage);
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = (err is BadParamException) ? err.Message : "Unexpected server error";
        return false;
      }      
    }

  }
}
