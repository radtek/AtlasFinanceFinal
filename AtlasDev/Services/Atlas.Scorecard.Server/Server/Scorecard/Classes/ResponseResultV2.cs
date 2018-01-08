using System;
using System.Collections.Generic;
using System.Xml;


namespace Atlas.ThirdParty.CS.Enquiry
{  
  [Serializable] 
  public sealed class ResponseResultV2 : IResponseResult
  {    
    public List<FPM> FPM { get; set; }
    
    public List<FPM_AddressDetail> FPMAddressDetails { get; set; }
    
    public List<FPM_BankDetail> FPMBankDetails { get; set; }
    
    public List<FPM_EmploymentDetail> FPMEmploymentDetails { get; set; }
    
    public List<FPM_IncidentDetail> FPMIncidentDetails { get; set; }
    
    public List<FPM_AliasDetail> FPMAliasDetails { get; set; }
    
    public List<FPM_OtherIdDetail> FPMOtherIdDetails { get; set; }
    
    public List<FPM_PersonalDetail> FPMPersonalDetails { get; set; }
    
    public List<FPM_TelephoneDetail> FPMTelephoneDetails { get; set; }
    
    public List<FPM_CaseDetail> FPMCaseDetails { get; set; }
    
    public bool WasSucess { get; set; }
    
    public string Error { get; set; }
    
    public string ErrorDescription { get; set; }
    
    public List<string> Files { get; set; }
    
    public string Score { get; set; }
    
    public string RiskType { get; set; }
    
    public List<Account> Accounts { get; set; }
    
    public List<Telephone> Telephone { get; set; }
    
    public string SequestrationReason { get; set; }
    
    public string NLREnquiryReferenceNo { get; set; }
    
    public List<string> Reasons { get; set; }
    
    public XmlDocument OriginalResponse { get; set; }
    
    public byte[] CompuscanResponse { get; set; }
    
    public byte[] SummaryFile { get; set; }
    
    public List<Atlas.Enumerators.Risk.Policy> Policies { get; set; }
    
    public int R { get; set; }
    
    public int G { get; set; }
    
    public int B { get; set; }
    
    public List<Product> Products { get; set; }
  }

}