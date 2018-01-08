/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Bank Verification server
 * 
 * 
 *  Author:
 *  ------------------
 *     Lee Venkatsamy
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-10-23 - Initial Version
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */
using System.ServiceModel;

using Atlas.Enumerators;


namespace Atlas.WCF.Interface
{
  [ServiceContract(Namespace = "Atlas.Bank.Verification.Server")]
  public interface IVerificationServer
  {
    [OperationContract]
    bool IsCDV(long bankId, long bankAccountTypeId, string bankAccountNo);

    [OperationContract]
    bool IsCDVWithBranch(long bankId, long bankAccountTypeId, string bankAccountNo, string branchCode);
      
    [OperationContract]
    AVSReply DoAVSEnquiry(string initials, string lastName, string idNo, string accountNo, General.BankName bankName, string branchCode);

    [OperationContract]
    AVSReply DoAVSEnquiryWithHost(long? personId, string initials, string lastName, string idNo, string accountNo, General.BankName bankName, string branchCode, General.Host host, General.BankPeriod bankPeriod);

    [OperationContract]
    AVSReply DoEnquiry(string initials, string lastName, string idNo, string accountNo, General.BankName bankName, string branchCode, bool forceCheck);

    [OperationContract]
    AVSReply DoLegacyEnquiry(string initials, string lastName, string idNo, string accountNo, General.BankName bankName, string branchCode, string legacyBranchCode, bool forceCheck);
    
    [OperationContract]
    AVSResponse GetAVSResponse(long transactionId);

  }
}