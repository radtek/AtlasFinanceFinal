using System;
using System.Xml.Serialization;
using System.Xml.Schema;


namespace Atlas.ThirdParty.CompuScan.Batch.XML
{
  [Serializable]
  [XmlType(AnonymousType = true)]
  public partial class NLR_LOANREG
  {
    private string reference_noField;

    private string trans_created_dateField;

    private string trans_created_timeField;

    private string country_of_originField;

    private string account_noField;

    private string sub_account_noField;

    private string loan_typeField;

    private string date_loan_disbursedField;

    private string total_amount_repayableField;

    private string monthly_instalmentField;

    private string loan_purposeField;

    private string current_balanceField;

    private string current_balance_indicatorField;

    private string loan_amountField;

    private string loan_amount_indicatorField;

    private string repayment_periodField;

    private string interest_rate_typeField;

    private string annual_rate_for_tot_charge_of_creditField;

    private string rand_value_interest_chargesField;

    private string rand_value_tot_charge_of_creditField;

    private string settlement_amountField;

    private string nlr_enq_ref_noField;

    private string nlr_loan_reg_noField;

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
    public string Account_no
    {
      get
      {
        return this.account_noField;
      }
      set
      {
        this.account_noField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Sub_account_no
    {
      get
      {
        return this.sub_account_noField;
      }
      set
      {
        this.sub_account_noField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Loan_type
    {
      get
      {
        return this.loan_typeField;
      }
      set
      {
        this.loan_typeField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Date_loan_disbursed
    {
      get
      {
        return this.date_loan_disbursedField;
      }
      set
      {
        this.date_loan_disbursedField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Total_amount_repayable
    {
      get
      {
        return this.total_amount_repayableField;
      }
      set
      {
        this.total_amount_repayableField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Monthly_instalment
    {
      get
      {
        return this.monthly_instalmentField;
      }
      set
      {
        this.monthly_instalmentField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Loan_purpose
    {
      get
      {
        return this.loan_purposeField;
      }
      set
      {
        this.loan_purposeField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Current_balance
    {
      get
      {
        return this.current_balanceField;
      }
      set
      {
        this.current_balanceField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Current_balance_indicator
    {
      get
      {
        return this.current_balance_indicatorField;
      }
      set
      {
        this.current_balance_indicatorField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Loan_amount
    {
      get
      {
        return this.loan_amountField;
      }
      set
      {
        this.loan_amountField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Loan_amount_indicator
    {
      get
      {
        return this.loan_amount_indicatorField;
      }
      set
      {
        this.loan_amount_indicatorField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Repayment_period
    {
      get
      {
        return this.repayment_periodField;
      }
      set
      {
        this.repayment_periodField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Interest_rate_type
    {
      get
      {
        return this.interest_rate_typeField;
      }
      set
      {
        this.interest_rate_typeField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Annual_rate_for_tot_charge_of_credit
    {
      get
      {
        return this.annual_rate_for_tot_charge_of_creditField;
      }
      set
      {
        this.annual_rate_for_tot_charge_of_creditField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Rand_value_interest_charges
    {
      get
      {
        return this.rand_value_interest_chargesField;
      }
      set
      {
        this.rand_value_interest_chargesField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Rand_value_tot_charge_of_credit
    {
      get
      {
        return this.rand_value_tot_charge_of_creditField;
      }
      set
      {
        this.rand_value_tot_charge_of_creditField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Settlement_amount
    {
      get
      {
        return this.settlement_amountField;
      }
      set
      {
        this.settlement_amountField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Nlr_enq_ref_no
    {
      get
      {
        return this.nlr_enq_ref_noField;
      }
      set
      {
        this.nlr_enq_ref_noField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
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
  }
}