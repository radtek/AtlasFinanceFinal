/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Bank Verification server (Proxy)
 * 
 * 
 *  Author:
 *  ------------------
 *     Lee Venkatsamy / Keith Blows (Proxy version)
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-10-23 - Initial Version
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using Atlas.Enumerators;
using System;
using System.Runtime.Serialization;
using System.ServiceModel;


namespace Atlas.WCF.Interface
{
  [ServiceContract(Namespace = "Atlas.Bank.Verification.Server")]
  public interface IVerificationServer
  {
    [OperationContract]
    bool IsCDV(long bankId, long bankAccountTypeId, string bankAccountNo);

    [OperationContract]
    bool IsCDVWithBranch(long bankId, long bankAccountTypeId, string bankAccountNo, string branchCode);


    #region AVS Related

    [OperationContract]
    AVSReply DoAVSEnquiry(string initials, string lastName, string idNo, string accountNo, 
      General.BankName bankName, string branchCode);

    [OperationContract]
    AVSReply DoEnquiry(string initials, string lastName, string idNo, string accountNo, 
      General.BankName bankName, string branchCode, bool forceCheck);

    [OperationContract]
    AVSReply DoLegacyEnquiry(string initials, string lastName, string idNo, string accountNo, 
      General.BankName bankName, string branchCode, string legacyBranchCode, bool forceCheck);

    [OperationContract]
    AVSReply DoLegacyEnquiryNew(string initials, string lastName, string idNo, string accountNo, 
      General.BankName bankName, string branchCode, string legacyBranchCode,
      General.Host host, bool forceCheck);

    [OperationContract]
    AVSResponse GetAVSResponse(long transactionId);

    #endregion

  }


  [DataContract(Name="AVSResponse", Namespace="http://schemas.datacontract.org/2004/07/Atlas.Bank.Verification.Server.Structures")]
   
  public class AVSResponse
  {
    [DataMember]
    public bool WaitingReply { get; set; }

    [DataMember]
    public long TransactionId { get; set; }

    [DataMember]
    public AVS.Result? FinalResult { get; set; }

    [DataMember]
    public bool AccountExists { get; set; }

    [DataMember]
    public bool IdNumberMatch { get; set; }

    [DataMember]
    public bool InitialsMatch { get; set; }

    [DataMember]
    public bool LastNameMatch { get; set; }

    [DataMember]
    public bool AccountOpen { get; set; }

    [DataMember]
    public bool AccountAcceptsCredits { get; set; }

    [DataMember]
    public bool AccountAcceptsDebits { get; set; }

    [DataMember]
    public bool AccountOpen90days { get; set; }
  }


  [DataContract(Name = "AVSReply", Namespace = "http://schemas.datacontract.org/2004/07/Atlas.Bank.Verification.Server.Structures")]
    
  public class AVSReply
  {
    [DataMember]
    public string Initials { get; set; }

    [DataMember]
    public string Lastname { get; set; }

    [DataMember]
    public string IdNumber { get; set; }

    [DataMember]
    public General.BankName Bank { get; set; }

    [DataMember]
    public string BankAccountNo { get; set; }

    [DataMember]
    public string BranchCode { get; set; }

    [DataMember]
    public bool WaitingReply { get; set; }

    [DataMember]
    public long TransactionId { get; set; }

    [DataMember]
    public AVS.Result? FinalResult { get; set; }
        
    // Account specific
    [DataMember]
    public bool AccountExists { get; set; }

    [DataMember]
    public bool IdNumberMatch { get; set; }

    [DataMember]
    public bool InitialsMatch { get; set; }

    [DataMember]
    public bool LastNameMatch { get; set; }

    [DataMember]
    public bool AccountOpen { get; set; }

    [DataMember]
    public bool AccountAcceptsCredits { get; set; }

    [DataMember]
    public bool AccountAcceptsDebits { get; set; }

    [DataMember]
    public bool AccountOpen90days { get; set; }
  }

}
