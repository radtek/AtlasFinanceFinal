using System;

using FileHelpers;


namespace Atlas.Evolution.Server.Code.Layout
{
  [FixedLengthRecord()]
  public class Monthly_Data
  {
    [FieldFixedLength(1)]
    public string DATA = "D";

    [FieldFixedLength(13)]
    [FieldAlign(AlignMode.Right, '0')]
    public string SA_ID_NUMBER;

    [FieldFixedLength(16)]
    public string NON_SA_ID_NUMBER;

    [FieldFixedLength(1)]
    public string GENDER;

    [FieldFixedLength(8)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint DATE_OF_BIRTH;

    [FieldFixedLength(8)]
    [FieldAlign(AlignMode.Right, ' ')]
    public string BRANCH_CODE;

    [FieldFixedLength(25)]
    [FieldAlign(AlignMode.Right, ' ')]
    public string ACCOUNT_NO;

    [FieldFixedLength(4)]
    [FieldAlign(AlignMode.Right, ' ')]
    public string SUB_ACCOUNT_NO;

    [FieldFixedLength(25)]
    public string SURNAME;

    [FieldFixedLength(5)]
    public string TITLE;

    [FieldFixedLength(14)]
    public string FORENAME_OR_INITIAL_1;

    [FieldFixedLength(14)]
    public string FORENAME_OR_INITIAL_2;

    [FieldFixedLength(14)]
    public string FORENAME_OR_INITIAL_3;

    [FieldFixedLength(25)]
    public string RESIDENTIAL_ADDRESS_LINE_1;

    [FieldFixedLength(25)]
    public string RESIDENTIAL_ADDRESS_LINE_2;

    [FieldFixedLength(25)]
    public string RESIDENTIAL_ADDRESS_LINE_3;

    [FieldFixedLength(25)]
    public string RESIDENTIAL_ADDRESS_LINE_4;

    [FieldFixedLength(6)]
    [FieldAlign(AlignMode.Right, ' ')]
    public string POSTAL_CODE_OF_RESIDENTIAL_ADDRESS;

    [FieldFixedLength(1)]
    public string OWNER_TENANT;

    [FieldFixedLength(25)]
    public string POSTAL_ADDRESS_LINE_1;

    [FieldFixedLength(25)]
    public string POSTAL_ADDRESS_LINE_2;

    [FieldFixedLength(25)]
    public string POSTAL_ADDRESS_LINE_3;

    [FieldFixedLength(25)]
    public string POSTAL_ADDRESS_LINE_4;

    [FieldFixedLength(6)]
    [FieldAlign(AlignMode.Right, ' ')]
    public string POSTAL_CODE_OF_POSTAL_ADDRESS;

    [FieldFixedLength(2)]
    public string OWNERSHIP_TYPE = "00"; // Other

    [FieldFixedLength(2)]
    public string LOAN_REASON_CODE;

    [FieldFixedLength(2)]
    public string PAYMENT_TYPE;

    [FieldFixedLength(2)]
    public string TYPE_OF_ACCOUNT;

    [FieldFixedLength(8)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint DATE_ACCOUNT_OPENED;

    [FieldFixedLength(8)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint DEFERRED_PAYMENT_DATE;

    [FieldFixedLength(8)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint DATE_OF_LAST_PAYMENT;

    [FieldFixedLength(9)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint OPENING_BALANCE_CREDIT_LIMIT;

    [FieldFixedLength(9)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint CURRENT_BALANCE;

    [FieldFixedLength(1)]
    public string CURRENT_BALANCE_INDICATOR;

    [FieldFixedLength(9)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint AMOUNT_OVERDUE;

    [FieldFixedLength(9)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint INSTALMENT_AMOUNT;

    [FieldFixedLength(2)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint MONTHS_IN_ARREARS;

    /// <summary>
    /// Can only be- C D E L T V W Z
    /// </summary>
    [FieldFixedLength(2)]
    public string STATUS_CODE;

    [FieldFixedLength(2)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint REPAYMENT_FREQUENCY;

    [FieldFixedLength(4)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint TERMS;

    [FieldFixedLength(8)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint STATUS_DATE;

    [FieldFixedLength(8)]   
    public string OLD_SUPPLIER_BRANCH_CODE;

    [FieldFixedLength(25)]  
    public string OLD_ACCOUNT_NUMBER;

    [FieldFixedLength(4)]   
    public string OLD_SUB_ACCOUNT_NUMBER;

    [FieldFixedLength(10)]  
    public string OLD_SUPPLIER_REFERENCE_NUMBER;

    [FieldFixedLength(16)]
    [FieldAlign(AlignMode.Right, ' ')]
    public string HOME_TELEPHONE;

    [FieldFixedLength(16)]
    [FieldAlign(AlignMode.Right, ' ')]
    public string CELLULAR_TELEPHONE;

    [FieldFixedLength(16)]
    [FieldAlign(AlignMode.Right, ' ')]
    public string WORK_TELEPHONE;

    [FieldFixedLength(60)]
    public string EMPLOYER_DETAIL;

    [FieldFixedLength(9)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint INCOME;

    [FieldFixedLength(1)]
    public string INCOME_FREQUENCY;

    [FieldFixedLength(20)]
    public string OCCUPATION;

    [FieldFixedLength(60)]
    public string THIRD_PARTY_NAME;

    [FieldFixedLength(2)]
    public string ACCOUNT_SOLD_TO_THIRD_PARTY = "00";

    [FieldFixedLength(3)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint NO_OF_PARTICIPANTS_IN_JOINT_LOAN;

    [FieldFixedLength(2)]
    public string FILLER;
  }
}
