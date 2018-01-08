using System;

using Atlas.Server.WCF.Utils;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;


namespace Atlas.Server.WCF.Implementation.TCC

{
  public static class EDOGetFutureTrans_Impl
  {
    internal static bool Execute(ILogging log, IConfigSettings config, ICacheServer cache, 
      string contractRef, string edoType, 
      out DateTime nextInstalmentDate, out decimal nextInstalmentVal, out string errorMessage)
    {
      var methodName = "EDOGetFutureTrans_Impl.Execute";

      nextInstalmentDate = DateTime.MinValue;
      nextInstalmentVal = 0;
      errorMessage = string.Empty;

      try
      {
        log.Information("{MethodName}- Starting: {@Params}", methodName, new { contractRef, edoType });
        
        return EDOUtils.GetFutureTrans(contractRef, edoType, out nextInstalmentDate, out nextInstalmentVal, out errorMessage);
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}", methodName);
        errorMessage = "Unexpected server error";
        return false;
      }
    }

  }
}
