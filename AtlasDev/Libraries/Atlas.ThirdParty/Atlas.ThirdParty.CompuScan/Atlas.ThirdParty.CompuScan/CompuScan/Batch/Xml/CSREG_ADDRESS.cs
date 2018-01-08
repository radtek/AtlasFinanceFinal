using System;
using System.Xml.Serialization;
using System.Xml.Schema;


namespace Atlas.ThirdParty.CompuScan.Batch.XML
{
  [Serializable]
  [XmlType(AnonymousType = true)]
  public partial class CSREG_ADDRESS
  {
    private string reference_noField;

    private string trans_created_dateField;

    private string trans_created_timeField;

    private string country_of_originField;

    private string identity_numberField;

    private string address_line_1Field;

    private string address_line_2Field;

    private string address_line_3Field;

    private string address_line_4Field;

    private string address_postal_codeField;

    private string address_typeField;

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
    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Address_line_1
    {
      get
      {
        return this.address_line_1Field;
      }
      set
      {
        this.address_line_1Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Address_line_2
    {
      get
      {
        return this.address_line_2Field;
      }
      set
      {
        this.address_line_2Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Address_line_3
    {
      get
      {
        return this.address_line_3Field;
      }
      set
      {
        this.address_line_3Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Address_line_4
    {
      get
      {
        return this.address_line_4Field;
      }
      set
      {
        this.address_line_4Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Address_postal_code
    {
      get
      {
        return this.address_postal_codeField;
      }
      set
      {
        this.address_postal_codeField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
    public string Address_type
    {
      get
      {
        return this.address_typeField;
      }
      set
      {
        this.address_typeField = value;
      }
    }
  }
}
