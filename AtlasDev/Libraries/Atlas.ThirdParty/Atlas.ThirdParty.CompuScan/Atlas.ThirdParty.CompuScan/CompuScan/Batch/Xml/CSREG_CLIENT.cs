using System;
using System.Xml.Serialization;
using System.Xml.Schema;


namespace Atlas.ThirdParty.CompuScan.Batch.XML
{
  /// <remarks/>
  [Serializable]
  [XmlTypeAttribute(AnonymousType = true)]
  public partial class CSREG_CLIENT
  {
    private string reference_noField;

    private string trans_created_dateField;

    private string trans_created_timeField;

    private string country_of_originField;

    private string identity_numberField;

    private string surnameField;

    private string nameField;

    private string client_noField;

    private string commentsField;

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
    public string Surname
    {
      get
      {
        return this.surnameField;
      }
      set
      {
        this.surnameField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Name
    {
      get
      {
        return this.nameField;
      }
      set
      {
        this.nameField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Client_no
    {
      get
      {
        return this.client_noField;
      }
      set
      {
        this.client_noField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Comments
    {
      get
      {
        return this.commentsField;
      }
      set
      {
        this.commentsField = value;
      }
    }
  }
}