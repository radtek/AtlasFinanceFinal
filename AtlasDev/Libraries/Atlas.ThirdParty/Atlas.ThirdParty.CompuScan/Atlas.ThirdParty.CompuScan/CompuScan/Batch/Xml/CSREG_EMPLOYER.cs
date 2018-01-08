using System;
using System.Xml.Serialization;
using System.Xml.Schema;


namespace Atlas.ThirdParty.CompuScan.Batch.XML
{
  [Serializable]
  [XmlType(AnonymousType = true)]
  public partial class CSREG_EMPLOYER
  {
    private string reference_noField;

    private string trans_created_dateField;

    private string trans_created_timeField;

    private string country_of_originField;

    private string identity_numberField;

    private string employer_nameField;

    private string employee_occupationField;

    private string employment_typeField;

    private string employee_salary_frequencyField;

    private string employee_payslip_referenceField;

    private string employee_numberField;

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
    public string Employer_name
    {
      get
      {
        return this.employer_nameField;
      }
      set
      {
        this.employer_nameField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Employee_occupation
    {
      get
      {
        return this.employee_occupationField;
      }
      set
      {
        this.employee_occupationField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Employment_type
    {
      get
      {
        return this.employment_typeField;
      }
      set
      {
        this.employment_typeField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Employee_salary_frequency
    {
      get
      {
        return this.employee_salary_frequencyField;
      }
      set
      {
        this.employee_salary_frequencyField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Employee_payslip_reference
    {
      get
      {
        return this.employee_payslip_referenceField;
      }
      set
      {
        this.employee_payslip_referenceField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Employee_number
    {
      get
      {
        return this.employee_numberField;
      }
      set
      {
        this.employee_numberField = value;
      }
    }
  }
}