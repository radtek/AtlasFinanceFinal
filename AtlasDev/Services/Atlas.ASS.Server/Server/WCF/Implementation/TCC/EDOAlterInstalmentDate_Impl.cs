using System;

using Atlas.Server.WCF.Utils;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;


namespace Atlas.Server.WCF.Implementation.TCC
{
  public static class EDOAlterInstalmentDate_Impl
  {
    internal static bool Execute(ILogging log, IConfigSettings config, ICacheServer cache,  
      string atlasBranchNumber, int transactionId, string edoType, 
      int installment, DateTime newDate, string frequency, out string errorMessage)
    {
      var methodName = "EDOAddContract_Impl.Execute";
      log.Information("{MethodName}- Starting: {@Request}", methodName, new { atlasBranchNumber, transactionId, edoType, installment, newDate });
      errorMessage = null;

      try
      {
        #region Check parameters
        atlasBranchNumber = atlasBranchNumber.Trim();
        if (string.IsNullOrEmpty(atlasBranchNumber) || atlasBranchNumber.Length > 3)
        {
          errorMessage = string.Format("EDOAddContract called with invalid atlasBranchNumber '{0}'", atlasBranchNumber);
          return false;
        }
        atlasBranchNumber = atlasBranchNumber.PadLeft(3, '0');

        int frequencyVal;
        switch (frequency)
        {
          case "W":
            frequencyVal = 1;
            break;

          case "F":
            frequencyVal = 2;
            break;

          case "M":
            frequencyVal = 3;
            break;

          default:
            errorMessage = "Invalid value for frequency parameter- must be 'W', 'F' or 'M'";
            return false;
        }
        #endregion

        return EDOUtils.AlterInstalmentDate(log, atlasBranchNumber, transactionId, edoType, installment, newDate, frequencyVal, out errorMessage);
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
