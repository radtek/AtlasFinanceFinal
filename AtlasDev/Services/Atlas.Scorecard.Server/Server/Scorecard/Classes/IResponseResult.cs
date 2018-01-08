using Atlas.Enumerators;
using System.Collections.Generic;
using System.Xml;


namespace Atlas.ThirdParty.CS.Enquiry
{
  public interface IResponseResult
  {
    List<FPM> FPM { get; set; }
    List<FPM_AddressDetail> FPMAddressDetails { get; set; }
    List<FPM_BankDetail> FPMBankDetails { get; set; }
    List<FPM_EmploymentDetail> FPMEmploymentDetails { get; set; }
    List<FPM_IncidentDetail> FPMIncidentDetails { get; set; }
    List<FPM_AliasDetail> FPMAliasDetails { get; set; }
    List<FPM_OtherIdDetail> FPMOtherIdDetails { get; set; }
    List<FPM_PersonalDetail> FPMPersonalDetails { get; set; }
    List<FPM_TelephoneDetail> FPMTelephoneDetails { get; set; }
    List<FPM_CaseDetail> FPMCaseDetails { get; set; }
    bool WasSucess { get; set; }
    string Error { get; set; }
    string ErrorDescription { get; set; }
    List<string> Files { get; set; }
    string Score { get; set; }
    string RiskType { get; set; }
    List<Account> Accounts { get; set; }
    List<Telephone> Telephone { get; set; }
    string SequestrationReason { get; set; }
    string NLREnquiryReferenceNo { get; set; }
    List<string> Reasons { get; set; }
    XmlDocument OriginalResponse { get; set; }
    byte[] CompuscanResponse { get; set; }
    byte[] SummaryFile { get; set; }
    List<Risk.Policy> Policies { get; set; }
    int R { get; set; }
    int G { get; set; }
    int B { get; set; }
  }
}
