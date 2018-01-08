using System;

using FileHelpers;


namespace Atlas.Evolution.Server.Code.Layout
{
  [IgnoreFirst(1)]
  [IgnoreLast(1)]
  [FixedLengthRecord()]
  public class Monthly_Data
  {
    [FieldFixedLength(1)]
    public string DATA = "D";

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(13)]    
    public string SA_ID_NUMBER;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(16)]
    public string NON_SA_ID_NUMBER;

    [FieldFixedLength(1)]
    public string GENDER;

    [FieldFixedLength(8)]   
    public uint DATE_OF_BIRTH;

    [FieldTrim(TrimMode.Left, " ")]
    [FieldFixedLength(8)]   
    public string BRANCH_CODE;

    [FieldTrim(TrimMode.Left, " ")]
    [FieldFixedLength(25)]  
    public string ACCOUNT_NO;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(4)]   
    public string SUB_ACCOUNT_NO;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(25)]
    public string SURNAME;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(5)]
    public string TITLE;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(14)]
    public string FORENAME_OR_INITIAL_1;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(14)]
    public string FORENAME_OR_INITIAL_2;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(14)]
    public string FORENAME_OR_INITIAL_3;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(25)]
    public string RESIDENTIAL_ADDRESS_LINE_1;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(25)]
    public string RESIDENTIAL_ADDRESS_LINE_2;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(25)]
    public string RESIDENTIAL_ADDRESS_LINE_3;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(25)]
    public string RESIDENTIAL_ADDRESS_LINE_4;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(6)] 
    public string POSTAL_CODE_OF_RESIDENTIAL_ADDRESS;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(1)]
    public string OWNER_TENANT;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(25)]
    public string POSTAL_ADDRESS_LINE_1;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(25)]
    public string POSTAL_ADDRESS_LINE_2;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(25)]
    public string POSTAL_ADDRESS_LINE_3;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(25)]
    public string POSTAL_ADDRESS_LINE_4;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(6)]
    public string POSTAL_CODE_OF_POSTAL_ADDRESS;

    [FieldFixedLength(2)]
    public string OWNERSHIP_TYPE = "00"; // Other

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(2)]
    public string LOAN_REASON_CODE;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(2)]
    public string PAYMENT_TYPE;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(2)]
    public string TYPE_OF_ACCOUNT;
    
    [FieldFixedLength(8)] 
    public uint DATE_ACCOUNT_OPENED;

    [FieldFixedLength(8)]  
    public uint DEFERRED_PAYMENT_DATE;

    [FieldFixedLength(8)]  
    public uint DATE_OF_LAST_PAYMENT;

    [FieldFixedLength(9)]   
    public uint OPENING_BALANCE_CREDIT_LIMIT;

    [FieldFixedLength(9)]   
    public uint CURRENT_BALANCE;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(1)]
    public string CURRENT_BALANCE_INDICATOR;

    [FieldFixedLength(9)]   
    public uint AMOUNT_OVERDUE;

    [FieldFixedLength(9)]   
    public uint INSTALMENT_AMOUNT;

    [FieldFixedLength(2)]  
    public uint MONTHS_IN_ARREARS;

    [FieldTrim(TrimMode.Right, " ")]
    /// <summary>
    /// Can only be- C D E L T V W Z
    /// </summary>
    [FieldFixedLength(2)]
    public string STATUS_CODE;

    [FieldFixedLength(2)]   
    public uint REPAYMENT_FREQUENCY;

    [FieldFixedLength(4)]   
    public uint TERMS;

    [FieldFixedLength(8)]   
    public uint STATUS_DATE;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(8)]   
    public string OLD_SUPPLIER_BRANCH_CODE;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(25)]  
    public string OLD_ACCOUNT_NUMBER;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(4)]   
    public string OLD_SUB_ACCOUNT_NUMBER;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(10)]  
    public string OLD_SUPPLIER_REFERENCE_NUMBER;

    [FieldTrim(TrimMode.Left, " ")]
    [FieldFixedLength(16)]   
    public string HOME_TELEPHONE;

    [FieldTrim(TrimMode.Left, " ")]
    [FieldFixedLength(16)]   
    public string CELLULAR_TELEPHONE;

    [FieldTrim(TrimMode.Left, " ")]
    [FieldFixedLength(16)] 
    public string WORK_TELEPHONE;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(60)]
    public string EMPLOYER_DETAIL;

    [FieldFixedLength(9)]
    public uint INCOME;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(1)]
    public string INCOME_FREQUENCY;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(20)]
    public string OCCUPATION;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(60)]
    public string THIRD_PARTY_NAME;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(2)]
    public string ACCOUNT_SOLD_TO_THIRD_PARTY = "00";
    
    [FieldFixedLength(3)]   
    public uint NO_OF_PARTICIPANTS_IN_JOINT_LOAN;

    [FieldTrim(TrimMode.Right, " ")]
    [FieldFixedLength(2)]
    public string FILLER;

    //[FieldFixedLength(1)]
    [FieldHidden]
    public string LegacyBranchCode;
  }
}
