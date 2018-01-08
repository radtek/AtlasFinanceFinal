using System;
using System.Xml.Serialization;
using System.Xml.Schema;


namespace Atlas.ThirdParty.CompuScan.Batch.XML
{
  [Serializable]
  [XmlType(AnonymousType = true)]
  public partial class CSUPD_LOAN
  {
    private string reference_noField;

    private string trans_created_dateField;

    private string trans_created_timeField;

    private string country_of_originField;

    private string identity_numberField;

    private string loan_ref_noField;

    private string loan_statusField;

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
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
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
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
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
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
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
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Identity_number
    {
      get
      {
        return this.identity_numberField;
      }
      set
      {
        this.identity_numberField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Loan_ref_no
    {
      get
      {
        return this.loan_ref_noField;
      }
      set
      {
        this.loan_ref_noField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Loan_status
    {
      get
      {
        return this.loan_statusField;
      }
      set
      {
        this.loan_statusField = value;
      }
    }
  }
}