using System;
using System.ServiceModel;

using Atlas.Enumerators;
using Atlas.WCF.Interface;
using Atlas.Common.Interface;
using Atlas.Cache.Interfaces;
using Atlas.Server.WCF.Implementation.AVS;


namespace Atlas.WCF.Implementation
{
  /// <summary>
  /// ASS Proxy WCF service for Atlas Bank Verification server
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class VerificationServer : IVerificationServer
  {
    public VerificationServer(ILogging log, IConfigSettings config, ICacheServer cache)
    {
      _log = log;
      _config = config;
      _cache = cache;
    }


    public bool IsCDV(long bankId, long bankAccountTypeId, string bankAccountNo)
    {
      return false;
    }


    public bool IsCDVWithBranch(long bankId, long bankAccountTypeId, string bankAccountNo, string branchCode)
    {
      return IsCDVWithBranch_Impl.Execute(_log, bankId, bankAccountTypeId, bankAccountNo, branchCode);
    }


    public AVSReply DoAVSEnquiry(string initials, string lastName, string idNo, string accountNo, General.BankName bankName, string branchCode)
    {
      return DoAVSEnquiry_Impl.Execute(_log, initials, lastName, idNo, accountNo, bankName, branchCode);
    }


    public AVSReply DoEnquiry(string initials, string lastName, string idNo, string accountNo, General.BankName bankName, string branchCode, bool forceCheck)
    {
      return DoEnquiry_Impl.Execute(_log, initials, lastName, idNo, accountNo, bankName, branchCode, forceCheck);
    }


    public AVSReply DoLegacyEnquiry(string initials, string lastName, string idNo, string accountNo,
      General.BankName bankName, string branchCode,
      string legacyBranchCode, bool forceCheck)
    {
      return DoLegacyEnquiry_Impl.Execute(_log, _cache, initials, lastName, idNo, accountNo, bankName, branchCode, legacyBranchCode, forceCheck);
    }


    public AVSResponse GetAVSResponse(long transactionId)
    {
      return GetAVSResponse_Impl.Execute(_log, transactionId);
    }


    public AVSReply DoLegacyEnquiryNew(string initials, string lastName, string idNo, string accountNo,
      General.BankName bankName, string branchCode, string legacyBranchCode,
      General.Host host, bool forceCheck)
    {
      return DoLegacyEnquiryNew_Impl.Execute(_log, _config, _cache, initials, lastName, idNo, accountNo, bankName, branchCode, legacyBranchCode, host, forceCheck);
    }

    public AVSReply DoAVSEnquiryWithHost(long? personId, string initials, string lastName, string idNo, string accountNo, General.BankName bankName, string branchCode, General.Host host, General.BankPeriod bankPeriod)
    {
      throw new NotImplementedException();
    }

    
    private readonly ILogging _log;
    private readonly ICacheServer _cache;
    private readonly IConfigSettings _config;

  }
}
