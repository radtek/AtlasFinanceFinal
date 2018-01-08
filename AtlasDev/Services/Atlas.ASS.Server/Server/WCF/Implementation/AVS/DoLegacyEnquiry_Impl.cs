using System;

using Atlas.Common.Interface;
using Atlas.WCF.Interface;
using Atlas.Enumerators;
using Atlas.Server.MessageBus.Avs;
using Atlas.Cache.Interfaces;
using Atlas.Data.Repository;


namespace Atlas.Server.WCF.Implementation.AVS
{
  internal static class DoLegacyEnquiry_Impl
  {
    internal static AVSReply Execute(ILogging log, ICacheServer cache,  
      string initials, string lastName, string idNo, string accountNo, General.BankName bankName, string branchCode,
      string legacyBranchCode, bool forceCheck)
    {
      var methodName = "DoLegacyEnquiry";
      log.Information("{MethodName}- {Initials}, {LastName}, {IdNo}, {AccountNo}, {BankName}, {BranchCode}, {LegacyBranchCode}, {ForceCheck}",
        methodName, initials, lastName, idNo, accountNo, bankName, branchCode, legacyBranchCode, forceCheck);

      try
      {
        var companyId = AtlasData.FindBranch(legacyBranchCode)?.Company.CompanyId;
        
        var id = AvsDistCommUtils.AddAVS(new BankVerification.EasyNetQ.AddAVSRequest
        { Initials = initials, LastName = lastName, IdNumber = idNo, AccountNo = accountNo, Bank = bankName,
          BranchCode = branchCode, CompanyId = companyId, Host = General.Host.ASS });

        return (id > 0) ? new AVSReply { WaitingReply = true, TransactionId = id } : null;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        throw new Exception("Unexpected server error");
      }
    }

  }
}
