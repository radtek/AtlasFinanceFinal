using System;

using Atlas.WCF.Interface;
using Atlas.Enumerators;


namespace Atlas.Server.Training
{
  internal class BankVerification_Impl: IVerificationServer
  {
    public bool IsCDV(long bankId, long bankAccountTypeId, string bankAccountNo)
    {
      return true;
    }


    public bool IsCDVWithBranch(long bankId, long bankAccountTypeId, string bankAccountNo, string branchCode)
    {
      return true;
    }


    public AVSReply DoAVSEnquiry(string initials, string lastName, string idNo, string accountNo, General.BankName bankName, string branchCode)
    {
      return new AVSReply
      {        
        TransactionId = new Random().Next(int.MaxValue),
        WaitingReply = true
      };
    }


    public AVSReply DoEnquiry(string initials, string lastName, string idNo, string accountNo, 
      General.BankName bankName, string branchCode, bool forceCheck)
    {
      return new AVSReply
      {
        AccountAcceptsCredits = true,
        AccountAcceptsDebits = true,
        AccountExists = true,
        AccountOpen = true,
        AccountOpen90days = true,
        Bank = bankName,
        BankAccountNo = accountNo,
        BranchCode = branchCode,
        FinalResult = AVS.Result.Passed,
        IdNumber = idNo,
        IdNumberMatch = true,
        Initials = initials,
        InitialsMatch = true,
        Lastname = lastName,
        LastNameMatch = true,
        TransactionId = new Random().Next(int.MaxValue),
        WaitingReply = true
      };
    }


    public AVSReply DoLegacyEnquiry(string initials, string lastName, string idNo, string accountNo, 
      General.BankName bankName,  string branchCode, string legacyBranchCode, bool forceCheck)
    {
      return new AVSReply
      {
        AccountAcceptsCredits = true,
        AccountAcceptsDebits = true,
        AccountExists = true,
        AccountOpen = true,
        AccountOpen90days = true,
        Bank = bankName,
        BankAccountNo = accountNo,
        BranchCode = branchCode,
        FinalResult = AVS.Result.Passed,
        IdNumber = idNo,
        IdNumberMatch = true,
        Initials = initials,
        InitialsMatch = true,
        Lastname = lastName,
        LastNameMatch = true,
        TransactionId = new Random().Next(int.MaxValue),
        WaitingReply = true
      };
    }


    public AVSReply DoLegacyEnquiryNew(string initials, string lastName, string idNo, string accountNo, 
      General.BankName bankName,  string branchCode, string legacyBranchCode, General.Host host, bool forceCheck)
    {
      return new AVSReply
      {
        AccountAcceptsCredits = true,
        AccountAcceptsDebits = true,
        AccountExists = true,
        AccountOpen = true,
        AccountOpen90days = true,
        Bank = bankName,
        BankAccountNo = accountNo,
        BranchCode = branchCode,
        FinalResult = AVS.Result.Passed,
        IdNumber = idNo,
        IdNumberMatch = true,
        Initials = initials,
        InitialsMatch = true,
        Lastname = lastName,
        LastNameMatch = true,
        TransactionId = new Random().Next(int.MaxValue),
        WaitingReply = true
      };
    }


    public AVSResponse GetAVSResponse(long transactionId)
    {
      return new AVSResponse
      {
        AccountAcceptsCredits = true,
        AccountAcceptsDebits = true,
        AccountExists = true,
        AccountOpen = true,
        AccountOpen90days = true,      
        FinalResult = AVS.Result.Passed,      
        IdNumberMatch = true,      
        InitialsMatch = true,        
        LastNameMatch = true,
        TransactionId = transactionId,
        WaitingReply = false
      };
    }
  }

}
