using System;

using Atlas.Common.Interface;
using Atlas.WCF.Interface;
using Atlas.Enumerators;
using Atlas.Server.MessageBus.Avs;
using Atlas.Cache.Interfaces;


namespace Atlas.Server.WCF.Implementation.AVS
{
  internal static class DoEnquiry_Impl
  {
    internal static AVSReply Execute(ILogging log, 
      string initials, string lastName, string idNo, string accountNo, General.BankName bankName, string branchCode, bool forceCheck)
    {
      var methodName = "DoEnquiry";
      try
      {
        log.Information("{MethodName} starting- {Initials}, {LastName}, {IdNo}, {AccountNo}, {BankName}, {BranchCode}, {ForceCheck}",
          methodName, initials, lastName, idNo, accountNo, bankName, branchCode, forceCheck);

        return AvsDistCommUtils.AddAVSWithResponse(new BankVerification.EasyNetQ.AddAVSRequest
        { Initials = initials, LastName = lastName, IdNumber = idNo, AccountNo = accountNo, Bank = bankName, BranchCode = branchCode, Host = General.Host.ASS });

      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        throw new Exception("Unexpected server error");
      }
    }

  }
}
