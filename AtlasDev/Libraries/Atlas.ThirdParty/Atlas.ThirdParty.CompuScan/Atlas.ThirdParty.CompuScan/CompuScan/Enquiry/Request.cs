using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Atlas.ThirdParty.CompuScan.Enquiry
{
  [DataContract]  
  public sealed class Request
  {    
    [DataMember]
    public string Firstname  {   get;  set;   }
    [DataMember]
    public string Surname  {  get; set; }
    [DataMember]
    public string IdentityNo { get; set; }
    [DataMember]
    public Enumerators.General.Gender Gender { get;set;}
    [DataMember]
    public DateTime DateOfBirth { get; set;}
    [DataMember]
    public string AddressLine1 {get;set;}
    [DataMember]
    public string AddressLine2 { get;set;}
    [DataMember]
    public string Suburb{get;set;}
    [DataMember]
    public string City {get;set;}
    [DataMember]
    public string PostalCode{get;set;}
    [DataMember]
    public string HomeTelCode { get; set; }
    [DataMember]
    public string HomeTelNo { get; set; }
    [DataMember]
    public string WorkTelCode { get; set; }
    [DataMember]
    public string WorkTelNo { get; set; }
    [DataMember]
    public string CellTelNo { get; set; }
    [DataMember]
    public Enumerators.Risk.CreditCheckDestination Destination {get; set;}
    [DataMember]
    public string CCEnquiry{get;set;}
    [DataMember]
    public string NLREnquiry{get;set;}
    [DataMember]
    public string CCPlusCPAEnquiry {get;set;}
    [DataMember]
    public string RunCodix{get;set;}
    [DataMember]
    public string IsIDPassportNo{get;set;}
    [DataMember]
    public string AddressMandatory{get;set;}
    [DataMember]
    public Enumerators.Risk.ResponseFormat ResponseFormat { get;set;   }
    [DataMember]
    public Enumerators.Risk.EnquiryPurpose EnquiryPurpose { get; set; }
    [DataMember]
    public string RunCompuScore { get; set; }
  }
}
