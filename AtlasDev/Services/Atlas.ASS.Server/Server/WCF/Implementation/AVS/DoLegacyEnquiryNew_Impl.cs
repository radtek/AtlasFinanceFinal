using System;

using Atlas.Common.Interface;
using Atlas.WCF.Interface;
using Atlas.Enumerators;
using Atlas.Server.MessageBus.Avs;
using Atlas.Cache.Interfaces;
using Atlas.Cache.DataUtils;
using Atlas.Data.Repository;

namespace Atlas.Server.WCF.Implementation.AVS
{
  internal static class DoLegacyEnquiryNew_Impl
  {
    internal static AVSReply Execute(ILogging log, IConfigSettings config, ICacheServer cache,  
      string initials, string lastName, string idNo, string accountNo, General.BankName bankName,
      string branchCode, string legacyBranchCode, General.Host host, bool forceCheck)
    {
      var methodName = "DoLegacyEnquiryNew";
      try
      {
        log.Information("{MethodName} starting: {Initials}, {LastName}, {IdNo}, {AccountNo}, {BankName}, {BranchCode}, {LegacyBranchCode}, " +
          "{Host}, {ForceCheck}", methodName, initials, lastName, idNo, accountNo, bankName, branchCode, legacyBranchCode, host, forceCheck);

        var companyId = AtlasData.FindBranch(legacyBranchCode)?.BranchId;
        var avsReply = AvsDistCommUtils.AddAVSWithResponse(new BankVerification.EasyNetQ.AddAVSRequest
        { Initials = initials, LastName = lastName, IdNumber = idNo, AccountNo = accountNo, Bank = bankName, BranchCode = branchCode, CompanyId = companyId, Host = host });

        if (avsReply != null)
        {
          log.Information("{MethodName} completed- {@AVSReply}", methodName, avsReply);
        }
        else
        {
          log.Warning("DoLegacyEnquiryNew completed with no result");
        }

        return avsReply;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        throw new Exception("Unexpected server error");
      }
    }

  }
}
