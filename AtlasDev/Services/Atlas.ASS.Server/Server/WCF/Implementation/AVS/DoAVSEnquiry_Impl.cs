using System;

using Atlas.Common.Interface;
using Atlas.WCF.Interface;
using Atlas.Enumerators;
using Atlas.Cache.Interfaces;
using Atlas.Server.MessageBus.Avs;


namespace Atlas.Server.WCF.Implementation.AVS
{
  internal static class DoAVSEnquiry_Impl
  {
    internal static AVSReply Execute(ILogging log,
      string initials, string lastName, string idNo, string accountNo, General.BankName bankName, string branchCode)
    {
      var methodName = "DoAVSEnquiry";
      try
      {
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
