using System;
using System.Xml.Serialization;
using System.Xml.Schema;


namespace Atlas.ThirdParty.CompuScan.Batch.XML
{
  [Serializable]
  [XmlType(AnonymousType = true)]
  public partial class NLR_LOANCLOSE
  {
    private string reference_noField;

    private string trans_created_dateField;

    private string trans_created_timeField;

    private string country_of_originField;

    private string nlr_loan_reg_noField;

    private string nlr_loan_close_codeField;

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Reference_no
    {
      get
      {
        return this.reference_noField;
      }
      set
      {
        this.reference_noField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Trans_created_date
    {
      get
      {
        return this.trans_created_dateField;
      }
      set
      {
        this.trans_created_dateField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Trans_created_time
    {
      get
      {
        return this.trans_created_timeField;
      }
      set
      {
        this.trans_created_timeField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Country_of_origin
    {
      get
      {
        return this.country_of_originField;
      }
      set
      {
        this.country_of_originField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Nlr_loan_reg_no
    {
      get
      {
        return this.nlr_loan_reg_noField;
      }
      set
      {
        this.nlr_loan_reg_noField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Nlr_loan_close_code
    {
      get
      {
        return this.nlr_loan_close_codeField;
      }
      set
      {
        this.nlr_loan_close_codeField = value;
      }
    }
  }
}