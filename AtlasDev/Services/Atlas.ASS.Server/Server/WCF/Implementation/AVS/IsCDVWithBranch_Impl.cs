using System;

using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;


namespace Atlas.Server.WCF.Implementation.AVS
{
  internal static class IsCDVWithBranch_Impl
  {
    internal static bool Execute(ILogging log, long bankId, long bankAccountTypeId, string bankAccountNo, string branchCode)
    {
      var methodName = "IsCDVWithBranch";
      log.Information("{MethodName} started- {BankId}, {BankAccountTypeId}, {BankAccountNo}, {BranchCode}",
        methodName, bankId, bankAccountTypeId, bankAccountNo, branchCode);
      
      string errorMessage;
      int result;
      var avs = new CDV.AVS(log);
      var isCDV = avs.CDV_Perform(branchCode, bankAccountNo, (int)bankAccountTypeId, out errorMessage, out result);

      log.Information("{MethodName} completed- {BankId}, {BankAccountTypeId}, {BankAccountNo}, {branchCode}: {Result}",
        methodName, bankId, bankAccountTypeId, bankAccountNo, branchCode, isCDV);

      return isCDV;
    }

  }
}
