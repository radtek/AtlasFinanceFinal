using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;

namespace Atlas.ThirdParty.CompuScan.Enquiry
{
  [Serializable]
  [DataContract]
  public sealed class ResponseResult : IResponseResult
  {
    [DataMember]
    public List<FPM> FPM { get; set; }
    [DataMember]
    public List<FPM_AddressDetail> FPMAddressDetails { get; set; }
    [DataMember]
    public List<FPM_BankDetail> FPMBankDetails { get; set; }
    [DataMember]
    public List<FPM_EmploymentDetail> FPMEmploymentDetails { get; set; }
    [DataMember]
    public List<FPM_IncidentDetail> FPMIncidentDetails { get; set; }
    [DataMember]
    public List<FPM_AliasDetail> FPMAliasDetails { get; set; }
    [DataMember]
    public List<FPM_OtherIdDetail> FPMOtherIdDetails { get; set; }
    [DataMember]
    public List<FPM_PersonalDetail> FPMPersonalDetails { get; set; }
    [DataMember]
    public List<FPM_TelephoneDetail> FPMTelephoneDetails { get; set; }
    [DataMember]
    public List<FPM_CaseDetail> FPMCaseDetails { get; set; }
    [DataMember]
    public bool WasSucess { get; set; }
    [DataMember]
    public string Error { get; set; }
    [DataMember]
    public string ErrorDescription { get; set; }
    [DataMember]
    public List<string> Files { get; set; }
    [DataMember]
    public string Score { get; set; }
    [DataMember]
    public string RiskType { get; set; }
    [DataMember]
    public List<Account> Accounts { get; set; }
    [DataMember]
    public List<Telephone> Telephone { get; set; }
    [DataMember]
    public string SequestrationReason { get; set; }
    [DataMember]
    public string NLREnquiryReferenceNo { get; set; }
    [DataMember]
    public List<string> Reasons { get; set; }
    [DataMember]
    public XmlDocument OriginalResponse { get; set; }
    [DataMember]
    public byte[] CompuscanResponse { get; set; }
    [DataMember]
    public byte[] SummaryFile { get; set; }
    [DataMember]
    public List<Atlas.Enumerators.Risk.Policy> Policies { get; set; }
    [DataMember]
    public int R { get; set; }
    [DataMember]
    public int G { get; set; }
    [DataMember]
    public int B { get; set; }

  }

  [Serializable]
  [DataContract]
  public sealed class ResponseResultV2 : IResponseResult
  {
    [DataMember]
    public List<FPM> FPM { get; set; }
    [DataMember]
    public List<FPM_AddressDetail> FPMAddressDetails { get; set; }
    [DataMember]
    public List<FPM_BankDetail> FPMBankDetails { get; set; }
    [DataMember]
    public List<FPM_EmploymentDetail> FPMEmploymentDetails { get; set; }
    [DataMember]
    public List<FPM_IncidentDetail> FPMIncidentDetails { get; set; }
    [DataMember]
    public List<FPM_AliasDetail> FPMAliasDetails { get; set; }
    [DataMember]
    public List<FPM_OtherIdDetail> FPMOtherIdDetails { get; set; }
    [DataMember]
    public List<FPM_PersonalDetail> FPMPersonalDetails { get; set; }
    [DataMember]
    public List<FPM_TelephoneDetail> FPMTelephoneDetails { get; set; }
    [DataMember]
    public List<FPM_CaseDetail> FPMCaseDetails { get; set; }
    [DataMember]
    public bool WasSucess { get; set; }
    [DataMember]
    public string Error { get; set; }
    [DataMember]
    public string ErrorDescription { get; set; }
    [DataMember]
    public List<string> Files { get; set; }
    [DataMember]
    public string Score { get; set; }
    [DataMember]
    public string RiskType { get; set; }
    [DataMember]
    public List<Account> Accounts { get; set; }
    [DataMember]
    public List<Telephone> Telephone { get; set; }
    [DataMember]
    public string SequestrationReason { get; set; }
    [DataMember]
    public string NLREnquiryReferenceNo { get; set; }
    [DataMember]
    public List<string> Reasons { get; set; }
    [DataMember]
    public XmlDocument OriginalResponse { get; set; }
    [DataMember]
    public byte[] CompuscanResponse { get; set; }
    [DataMember]
    public byte[] SummaryFile { get; set; }
    [DataMember]
    public List<Atlas.Enumerators.Risk.Policy> Policies { get; set; }
    [DataMember]
    public int R { get; set; }
    [DataMember]
    public int G { get; set; }
    [DataMember]
    public int B { get; set; }
    [DataMember]
    public List<Product> Products { get; set; }
  }

  public sealed class FPM
  {
    public string SubjectNo { get; set; }
    public string Category { get; set; }
    public string CategoryNo { get; set; }
    public string SubCategory { get; set; }
    public string Subject { get; set; }
    public string Passport { get; set; }
    public string IncidentDate { get; set; }
    public bool Victim { get; set; }
  }

  public sealed class FPM_AddressDetail
  {
    public string Type { get; set; }
    public string Street { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string From { get; set; }
    public string To { get; set; }
  }

  public sealed class FPM_BankDetail
  {
    public string AccountNo { get; set; }
    public string AccountType { get; set; }
    public string Bank { get; set; }
    public string From { get; set; }
    public string To { get; set; }
  }

  public sealed class FPM_EmploymentDetail
  {
    public string Name { get; set; }
    public string Telephone { get; set; }
    public string RegisteredName { get; set; }
    public string CompanyNo { get; set; }
    public string Occupation { get; set; }
    public string From { get; set; }
    public string To { get; set; }
  }

  public sealed class FPM_IncidentDetail
  {
    public bool Victim { get; set; }
    public string MembersReference { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public string IncidentDate { get; set; }
    public string SubRole { get; set; }
    public string City { get; set; }
    public string Detail { get; set; }
    public string Forensic { get; set; }
  }

  public sealed class FPM_OtherIdDetail
  {
    public string IDNo { get; set; }
    public string Type { get; set; }
    public string IssueDate { get; set; }
    public string Country { get; set; }
  }

  public sealed class FPM_PersonalDetail
  {
    public string Title { get; set; }
    public string Surname { get; set; }
    public string Firstname { get; set; }
    public string ID { get; set; }
    public string Passport { get; set; }
    public string DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
  }

  public sealed class FPM_TelephoneDetail
  {
    public string Type { get; set; }
    public string No { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
  }

  public sealed class FPM_AliasDetail
  {
    public string FirstName { get; set; }
    public string Surname { get; set; }
  }

  public sealed class FPM_CaseDetail
  {
    public string CaseNo { get; set; }
    public string ReportDate { get; set; }
    public string Officer { get; set; }
    public string CreatedBy { get; set; }
    public string Station { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
    public string Reason { get; set; }
    public string ReasonExtension { get; set; }
    public string ContactNo { get; set; }
    public string Email { get; set; }
    public string Fax { get; set; }
    public string Details { get; set; }
  }

  public sealed class Reason
  {
    public string Description { get; set; }
  }
  public sealed class Product
  {
    public string Description { get; set; }
    public string Outcome { get; set; }
    public List<Reason> Reasons { get; set; }
  }
}