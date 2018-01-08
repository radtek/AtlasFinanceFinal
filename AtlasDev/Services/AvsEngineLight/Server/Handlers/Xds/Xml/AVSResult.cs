using System;
using System.Xml.Serialization;


namespace AvsEngineLight.Handlers.Xds.Xml
{
  public class AVSResult
  {    
    [XmlTypeAttribute(AnonymousType = true)]
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class AccountVerificationResult
    {      
      public AccountVerificationResultResultFile ResultFile { get; set; }

      
      public AccountVerificationResultSubscriberInputDetails SubscriberInputDetails { get; set; }
    }

    
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class AccountVerificationResultResultFile
    {      
      public long RECORDINDICATOR { get; set; }

      
      public string SEQUENCENUMBER { get; set; }

      
      public string BRANCHNUMBER { get; set; }

      
      public string ACCOUNTNUMBER { get; set; }

      
      public string ACCOUNTTYPE { get; set; }

      
      public string IDNUMBER { get; set; }

      
      public string IDTYPE { get; set; }

      
      public string INITIALS { get; set; }

      
      public string SURNAME { get; set; }

      
      public string TAXREFERENCENUMBER { get; set; }

      
      public string CLIENTUSERREFERENCE { get; set; }

      
      public string SUBBILLINGID { get; set; }

      
      public string ERRORCONDITIONNUMBER { get; set; }

      
      public string ACCOUNTFOUND { get; set; }

      
      public string IDNUMBERMATCH { get; set; }

      
      public string INITIALSMATCH { get; set; }

      
      public string SURNAMEMATCH { get; set; }

      
      [XmlElementAttribute("ACCOUNT-OPEN")]
      public string ACCOUNTOPEN { get; set; }

      
      public string ACCOUNTDORMANT { get; set; }

      
      public string ACCOUNTOPENFORATLEASTTHREEMONTHS { get; set; }

      
      public string ACCOUNTACCEPTSDEBITS { get; set; }

      
      public string ACCOUNTACCEPTSCREDITS { get; set; }

      
      public string TAXREFERENCEMATCH { get; set; }

      
      public string ACCOUNTISSUER { get; set; }

      
      public string ACCOUNTTYPERETURN { get; set; }
    }

    
    [XmlTypeAttribute(AnonymousType = true)]
    public partial class AccountVerificationResultSubscriberInputDetails
    {

      
      public DateTime EnquiryDate { get; set; }

      
      public string EnquiryType { get; set; }

      
      public string SubscriberName { get; set; }

      
      public string SubscriberUserName { get; set; }

      
      public string EnquiryInput { get; set; }

      
      public string EnquiryStatus { get; set; }

      
      public string XDsRefNo { get; set; }

      
      public string ExternalRef { get; set; }

      
      public string IDNo { get; set; }

      
      public string SurName { get; set; }

      
      public string Initials { get; set; }

      
      public string AccountNo { get; set; }

      
      public string BankName { get; set; }

      
      public string BranchCode { get; set; }

      
      public string AccountType { get; set; }

      
      public object EmailAddress { get; set; }

      
      public object RequesterFirstName { get; set; }

      
      public object RequesterSurName { get; set; }

      
      public string refnumber { get; set; }
    }

  }
}
