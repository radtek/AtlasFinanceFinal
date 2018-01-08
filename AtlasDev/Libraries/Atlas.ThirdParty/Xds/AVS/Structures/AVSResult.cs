using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.ThirdParty.Xds.AVS.Structures
{
  public class AVSResult
  {
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class AccountVerificationResult
    {

      private AccountVerificationResultResultFile resultFileField;

      private AccountVerificationResultSubscriberInputDetails subscriberInputDetailsField;

      /// <remarks/>
      public AccountVerificationResultResultFile ResultFile
      {
        get
        {
          return this.resultFileField;
        }
        set
        {
          this.resultFileField = value;
        }
      }

      /// <remarks/>
      public AccountVerificationResultSubscriberInputDetails SubscriberInputDetails
      {
        get
        {
          return this.subscriberInputDetailsField;
        }
        set
        {
          this.subscriberInputDetailsField = value;
        }
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AccountVerificationResultResultFile
    {

      private long rECORDINDICATORField;

      private string sEQUENCENUMBERField;

      private string bRANCHNUMBERField;

      private string aCCOUNTNUMBERField;

      private string aCCOUNTTYPEField;

      private string iDNUMBERField;

      private string iDTYPEField;

      private string iNITIALSField;

      private string sURNAMEField;

      private string tAXREFERENCENUMBERField;

      private string cLIENTUSERREFERENCEField;

      private string sUBBILLINGIDField;

      private string eRRORCONDITIONNUMBERField;

      private string aCCOUNTFOUNDField;

      private string iDNUMBERMATCHField;

      private string iNITIALSMATCHField;

      private string sURNAMEMATCHField;

      private string aCCOUNTOPENField;

      private string aCCOUNTDORMANTField;

      private string aCCOUNTOPENFORATLEASTTHREEMONTHSField;

      private string aCCOUNTACCEPTSDEBITSField;

      private string aCCOUNTACCEPTSCREDITSField;

      private string tAXREFERENCEMATCHField;

      private string aCCOUNTISSUERField;

      private string aCCOUNTTYPERETURNField;

      /// <remarks/>
      public long RECORDINDICATOR
      {
        get
        {
          return this.rECORDINDICATORField;
        }
        set
        {
          this.rECORDINDICATORField = value;
        }
      }

      /// <remarks/>
      public string SEQUENCENUMBER
      {
        get
        {
          return this.sEQUENCENUMBERField;
        }
        set
        {
          this.sEQUENCENUMBERField = value;
        }
      }

      /// <remarks/>
      public string BRANCHNUMBER
      {
        get
        {
          return this.bRANCHNUMBERField;
        }
        set
        {
          this.bRANCHNUMBERField = value;
        }
      }

      /// <remarks/>
      public string ACCOUNTNUMBER
      {
        get
        {
          return this.aCCOUNTNUMBERField;
        }
        set
        {
          this.aCCOUNTNUMBERField = value;
        }
      }

      /// <remarks/>
      public string ACCOUNTTYPE
      {
        get
        {
          return this.aCCOUNTTYPEField;
        }
        set
        {
          this.aCCOUNTTYPEField = value;
        }
      }

      /// <remarks/>
      public string IDNUMBER
      {
        get
        {
          return this.iDNUMBERField;
        }
        set
        {
          this.iDNUMBERField = value;
        }
      }

      /// <remarks/>
      public string IDTYPE
      {
        get
        {
          return this.iDTYPEField;
        }
        set
        {
          this.iDTYPEField = value;
        }
      }

      /// <remarks/>
      public string INITIALS
      {
        get
        {
          return this.iNITIALSField;
        }
        set
        {
          this.iNITIALSField = value;
        }
      }

      /// <remarks/>
      public string SURNAME
      {
        get
        {
          return this.sURNAMEField;
        }
        set
        {
          this.sURNAMEField = value;
        }
      }

      /// <remarks/>
      public string TAXREFERENCENUMBER
      {
        get
        {
          return this.tAXREFERENCENUMBERField;
        }
        set
        {
          this.tAXREFERENCENUMBERField = value;
        }
      }

      /// <remarks/>
      public string CLIENTUSERREFERENCE
      {
        get
        {
          return this.cLIENTUSERREFERENCEField;
        }
        set
        {
          this.cLIENTUSERREFERENCEField = value;
        }
      }

      /// <remarks/>
      public string SUBBILLINGID
      {
        get
        {
          return this.sUBBILLINGIDField;
        }
        set
        {
          this.sUBBILLINGIDField = value;
        }
      }

      /// <remarks/>
      public string ERRORCONDITIONNUMBER
      {
        get
        {
          return this.eRRORCONDITIONNUMBERField;
        }
        set
        {
          this.eRRORCONDITIONNUMBERField = value;
        }
      }

      /// <remarks/>
      public string ACCOUNTFOUND
      {
        get
        {
          return this.aCCOUNTFOUNDField;
        }
        set
        {
          this.aCCOUNTFOUNDField = value;
        }
      }

      /// <remarks/>
      public string IDNUMBERMATCH
      {
        get
        {
          return this.iDNUMBERMATCHField;
        }
        set
        {
          this.iDNUMBERMATCHField = value;
        }
      }

      /// <remarks/>
      public string INITIALSMATCH
      {
        get
        {
          return this.iNITIALSMATCHField;
        }
        set
        {
          this.iNITIALSMATCHField = value;
        }
      }

      /// <remarks/>
      public string SURNAMEMATCH
      {
        get
        {
          return this.sURNAMEMATCHField;
        }
        set
        {
          this.sURNAMEMATCHField = value;
        }
      }

      /// <remarks/>
      [System.Xml.Serialization.XmlElementAttribute("ACCOUNT-OPEN")]
      public string ACCOUNTOPEN
      {
        get
        {
          return this.aCCOUNTOPENField;
        }
        set
        {
          this.aCCOUNTOPENField = value;
        }
      }

      /// <remarks/>
      public string ACCOUNTDORMANT
      {
        get
        {
          return this.aCCOUNTDORMANTField;
        }
        set
        {
          this.aCCOUNTDORMANTField = value;
        }
      }

      /// <remarks/>
      public string ACCOUNTOPENFORATLEASTTHREEMONTHS
      {
        get
        {
          return this.aCCOUNTOPENFORATLEASTTHREEMONTHSField;
        }
        set
        {
          this.aCCOUNTOPENFORATLEASTTHREEMONTHSField = value;
        }
      }

      /// <remarks/>
      public string ACCOUNTACCEPTSDEBITS
      {
        get
        {
          return this.aCCOUNTACCEPTSDEBITSField;
        }
        set
        {
          this.aCCOUNTACCEPTSDEBITSField = value;
        }
      }

      /// <remarks/>
      public string ACCOUNTACCEPTSCREDITS
      {
        get
        {
          return this.aCCOUNTACCEPTSCREDITSField;
        }
        set
        {
          this.aCCOUNTACCEPTSCREDITSField = value;
        }
      }

      /// <remarks/>
      public string TAXREFERENCEMATCH
      {
        get
        {
          return this.tAXREFERENCEMATCHField;
        }
        set
        {
          this.tAXREFERENCEMATCHField = value;
        }
      }

      /// <remarks/>
      public string ACCOUNTISSUER
      {
        get
        {
          return this.aCCOUNTISSUERField;
        }
        set
        {
          this.aCCOUNTISSUERField = value;
        }
      }

      /// <remarks/>
      public string ACCOUNTTYPERETURN
      {
        get
        {
          return this.aCCOUNTTYPERETURNField;
        }
        set
        {
          this.aCCOUNTTYPERETURNField = value;
        }
      }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class AccountVerificationResultSubscriberInputDetails
    {

      private System.DateTime enquiryDateField;

      private string enquiryTypeField;

      private string subscriberNameField;

      private string subscriberUserNameField;

      private string enquiryInputField;

      private string enquiryStatusField;

      private string xDsRefNoField;

      private string externalRefField;

      private string iDNoField;

      private string surNameField;

      private string initialsField;

      private string accountNoField;

      private string bankNameField;

      private string branchCodeField;

      private string accountTypeField;

      private object emailAddressField;

      private object requesterFirstNameField;

      private object requesterSurNameField;

      private string refnumberField;

      /// <remarks/>
      public System.DateTime EnquiryDate
      {
        get
        {
          return this.enquiryDateField;
        }
        set
        {
          this.enquiryDateField = value;
        }
      }

      /// <remarks/>
      public string EnquiryType
      {
        get
        {
          return this.enquiryTypeField;
        }
        set
        {
          this.enquiryTypeField = value;
        }
      }

      /// <remarks/>
      public string SubscriberName
      {
        get
        {
          return this.subscriberNameField;
        }
        set
        {
          this.subscriberNameField = value;
        }
      }

      /// <remarks/>
      public string SubscriberUserName
      {
        get
        {
          return this.subscriberUserNameField;
        }
        set
        {
          this.subscriberUserNameField = value;
        }
      }

      /// <remarks/>
      public string EnquiryInput
      {
        get
        {
          return this.enquiryInputField;
        }
        set
        {
          this.enquiryInputField = value;
        }
      }

      /// <remarks/>
      public string EnquiryStatus
      {
        get
        {
          return this.enquiryStatusField;
        }
        set
        {
          this.enquiryStatusField = value;
        }
      }

      /// <remarks/>
      public string XDsRefNo
      {
        get
        {
          return this.xDsRefNoField;
        }
        set
        {
          this.xDsRefNoField = value;
        }
      }

      /// <remarks/>
      public string ExternalRef
      {
        get
        {
          return this.externalRefField;
        }
        set
        {
          this.externalRefField = value;
        }
      }

      /// <remarks/>
      public string IDNo
      {
        get
        {
          return this.iDNoField;
        }
        set
        {
          this.iDNoField = value;
        }
      }

      /// <remarks/>
      public string SurName
      {
        get
        {
          return this.surNameField;
        }
        set
        {
          this.surNameField = value;
        }
      }

      /// <remarks/>
      public string Initials
      {
        get
        {
          return this.initialsField;
        }
        set
        {
          this.initialsField = value;
        }
      }

      /// <remarks/>
      public string AccountNo
      {
        get
        {
          return this.accountNoField;
        }
        set
        {
          this.accountNoField = value;
        }
      }

      /// <remarks/>
      public string BankName
      {
        get
        {
          return this.bankNameField;
        }
        set
        {
          this.bankNameField = value;
        }
      }

      /// <remarks/>
      public string BranchCode
      {
        get
        {
          return this.branchCodeField;
        }
        set
        {
          this.branchCodeField = value;
        }
      }

      /// <remarks/>
      public string AccountType
      {
        get
        {
          return this.accountTypeField;
        }
        set
        {
          this.accountTypeField = value;
        }
      }

      /// <remarks/>
      public object EmailAddress
      {
        get
        {
          return this.emailAddressField;
        }
        set
        {
          this.emailAddressField = value;
        }
      }

      /// <remarks/>
      public object RequesterFirstName
      {
        get
        {
          return this.requesterFirstNameField;
        }
        set
        {
          this.requesterFirstNameField = value;
        }
      }

      /// <remarks/>
      public object RequesterSurName
      {
        get
        {
          return this.requesterSurNameField;
        }
        set
        {
          this.requesterSurNameField = value;
        }
      }

      /// <remarks/>
      public string refnumber
      {
        get
        {
          return this.refnumberField;
        }
        set
        {
          this.refnumberField = value;
        }
      }
    }
  }
}
