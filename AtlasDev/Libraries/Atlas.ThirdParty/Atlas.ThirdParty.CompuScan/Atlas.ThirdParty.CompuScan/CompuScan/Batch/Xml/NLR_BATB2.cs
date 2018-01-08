using System;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace Atlas.ThirdParty.CompuScan.Batch.XML
{
  [Serializable]
  [XmlType(AnonymousType = true)]
  public partial class NLR_BATB2
  {
    private string reference_noField;

    private string trans_created_dateField;

    private string trans_created_timeField;

    private string country_of_originField;

    private string identity_numberField;

    private string non_sa_identity_noField;

    private string genderField;

    private string date_of_birthField;

    private string branch_codeField;

    private string account_noField;

    private string sub_account_noField;

    private string surnameField;

    private string titleField;

    private string forename1Field;

    private string forename2Field;

    private string forename3Field;

    private string residential_address_line1Field;

    private string residential_address_line2Field;

    private string residential_address_line3Field;

    private string residential_address_line4Field;

    private string residential_address_postal_codeField;

    private string owner_tenantField;

    private string postal_address_line1Field;

    private string postal_address_line2Field;

    private string postal_address_line3Field;

    private string postal_address_line4Field;

    private string postal_address_post_codeField;

    private string loan_typeField;

    private string date_account_openedField;

    private string date_last_paymentField;

    private string loan_amountField;

    private string opening_balance_indicatorField;

    private string current_balanceField;

    private string current_balance_indicatorField;

    private string balance_overdueField;

    private string balance_overdue_indicatorField;

    private string monthly_instalmentField;

    private string load_indicatorField;

    private string months_in_arrearsField;

    private string status_codeField;

    private string repayment_periodField;

    private string status_dateField;

    private string old_supplier_branch_codeField;

    private string old_account_noField;

    private string old_sub_account_noField;

    private string old_supplier_ref_noField;

    private string home_tel_cell_noField;

    private string work_tel_cell_noField;

    private string employer_nameField;

    private string end_use_codeField;

    private string total_amount_repayableField;

    private string interest_rate_typeField;

    private string annual_rate_for_tot_charge_of_creditField;

    private string rand_value_interest_chargesField;

    private string rand_value_tot_charge_of_creditField;

    private string employment_typeField;

    private string employer_payslip_refField;

    private string employee_noField;

    private string settlement_amountField;

    private string nlr_enq_ref_noField;

    private string salary_frequencyField;

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
    public string Non_sa_identity_no
    {
      get
      {
        return this.non_sa_identity_noField;
      }
      set
      {
        this.non_sa_identity_noField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Gender
    {
      get
      {
        return this.genderField;
      }
      set
      {
        this.genderField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Date_of_birth
    {
      get
      {
        return this.date_of_birthField;
      }
      set
      {
        this.date_of_birthField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Branch_code
    {
      get
      {
        return this.branch_codeField;
      }
      set
      {
        this.branch_codeField = value;
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
    public string Title
    {
      get
      {
        return this.titleField;
      }
      set
      {
        this.titleField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Forename1
    {
      get
      {
        return this.forename1Field;
      }
      set
      {
        this.forename1Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Forename2
    {
      get
      {
        return this.forename2Field;
      }
      set
      {
        this.forename2Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Forename3
    {
      get
      {
        return this.forename3Field;
      }
      set
      {
        this.forename3Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Residential_address_line1
    {
      get
      {
        return this.residential_address_line1Field;
      }
      set
      {
        this.residential_address_line1Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Residential_address_line2
    {
      get
      {
        return this.residential_address_line2Field;
      }
      set
      {
        this.residential_address_line2Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Residential_address_line3
    {
      get
      {
        return this.residential_address_line3Field;
      }
      set
      {
        this.residential_address_line3Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Residential_address_line4
    {
      get
      {
        return this.residential_address_line4Field;
      }
      set
      {
        this.residential_address_line4Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Residential_address_postal_code
    {
      get
      {
        return this.residential_address_postal_codeField;
      }
      set
      {
        this.residential_address_postal_codeField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Owner_tenant
    {
      get
      {
        return this.owner_tenantField;
      }
      set
      {
        this.owner_tenantField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Postal_address_line1
    {
      get
      {
        return this.postal_address_line1Field;
      }
      set
      {
        this.postal_address_line1Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Postal_address_line2
    {
      get
      {
        return this.postal_address_line2Field;
      }
      set
      {
        this.postal_address_line2Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Postal_address_line3
    {
      get
      {
        return this.postal_address_line3Field;
      }
      set
      {
        this.postal_address_line3Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Postal_address_line4
    {
      get
      {
        return this.postal_address_line4Field;
      }
      set
      {
        this.postal_address_line4Field = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Postal_address_post_code
    {
      get
      {
        return this.postal_address_post_codeField;
      }
      set
      {
        this.postal_address_post_codeField = value;
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
    public string Date_account_opened
    {
      get
      {
        return this.date_account_openedField;
      }
      set
      {
        this.date_account_openedField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Date_last_payment
    {
      get
      {
        return this.date_last_paymentField;
      }
      set
      {
        this.date_last_paymentField = value;
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
    public string Opening_balance_indicator
    {
      get
      {
        return this.opening_balance_indicatorField;
      }
      set
      {
        this.opening_balance_indicatorField = value;
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
    public string Balance_overdue
    {
      get
      {
        return this.balance_overdueField;
      }
      set
      {
        this.balance_overdueField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Balance_overdue_indicator
    {
      get
      {
        return this.balance_overdue_indicatorField;
      }
      set
      {
        this.balance_overdue_indicatorField = value;
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
    public string Load_indicator
    {
      get
      {
        return this.load_indicatorField;
      }
      set
      {
        this.load_indicatorField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Months_in_arrears
    {
      get
      {
        return this.months_in_arrearsField;
      }
      set
      {
        this.months_in_arrearsField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Status_code
    {
      get
      {
        return this.status_codeField;
      }
      set
      {
        this.status_codeField = value;
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
    public string Status_date
    {
      get
      {
        return this.status_dateField;
      }
      set
      {
        this.status_dateField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Old_supplier_branch_code
    {
      get
      {
        return this.old_supplier_branch_codeField;
      }
      set
      {
        this.old_supplier_branch_codeField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Old_account_no
    {
      get
      {
        return this.old_account_noField;
      }
      set
      {
        this.old_account_noField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Old_sub_account_no
    {
      get
      {
        return this.old_sub_account_noField;
      }
      set
      {
        this.old_sub_account_noField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Old_supplier_ref_no
    {
      get
      {
        return this.old_supplier_ref_noField;
      }
      set
      {
        this.old_supplier_ref_noField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Home_tel_cell_no
    {
      get
      {
        return this.home_tel_cell_noField;
      }
      set
      {
        this.home_tel_cell_noField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Work_tel_cell_no
    {
      get
      {
        return this.work_tel_cell_noField;
      }
      set
      {
        this.work_tel_cell_noField = value;
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
    public string End_use_code
    {
      get
      {
        return this.end_use_codeField;
      }
      set
      {
        this.end_use_codeField = value;
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
    public string Employer_payslip_ref
    {
      get
      {
        return this.employer_payslip_refField;
      }
      set
      {
        this.employer_payslip_refField = value;
      }
    }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Unqualified)]
    public string Employee_no
    {
      get
      {
        return this.employee_noField;
      }
      set
      {
        this.employee_noField = value;
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
    public string Salary_frequency
    {
      get
      {
        return this.salary_frequencyField;
      }
      set
      {
        this.salary_frequencyField = value;
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