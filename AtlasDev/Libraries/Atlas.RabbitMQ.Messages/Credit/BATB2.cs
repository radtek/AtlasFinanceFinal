using System;


namespace Atlas.RabbitMQ.Messages.Credit
{
  [Serializable]
  public sealed class BATB2
  {
    public BATB2(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public long? MessageId { get; set; }

    #region Properties

    public string LegacyBranchNo { get; set; }
    public string SequenceNo { get; set; }
    public string AccountNo { get; set; }
    public decimal AnnualRateForTotalChargeOfCredit { get; set; }
    public decimal BalanceOverdue { get; set; }
    public string BalanceOverdueIndicator { get; set; }
    public string BranchCode { get; set; }
    public decimal CurrentBalance { get; set; }
    public string CurentBalanceIndicator { get; set; }
    public string AccountOpened { get; set; }
    public string LastPayment { get; set; }
    public string DateOfBirth { get; set; }
    public string EmployeeNo { get; set; }
    public string EmployerName { get; set; }
    public string EmployerPayslipReferenceNo { get; set; }
    public string EmploymentType { get; set; }
    public string EndUseCode { get; set; }
    public string Forename1 { get; set; }
    public string Forename2 { get; set; }
    public string Forename3 { get; set; }
    public string Gender { get; set; }
    public string HomeTelCellNo { get; set; }
    public string IDNo { get; set; }
    public string InterestRateType { get; set; }
    public string LoanIndicator { get; set; }
    public decimal LoanAmount { get; set; }
    public string LoanType { get; set; }
    public decimal MonthlyInstalment { get; set; }
    public int MonthsInArrears { get; set; }
    public string NLREnquiryReferenceNo { get; set; }
    public string NLRLoanRegistrationNo { get; set; }
    public string NonSAIDNo { get; set; }
    public string OldAccountNo { get; set; }
    public string OldSubAccountNo { get; set; }
    public string OldSupplierBranchCode { get; set; }
    public string OldSupplierReferenceNo { get; set; }
    public string OpeneingBalanceIndicator { get; set; }
    public string OwnerTenant { get; set; }
    public string PostalAddressLine1 { get; set; }
    public string PostalAddressLine2 { get; set; }
    public string PostalAddressLine3 { get; set; }
    public string PostalAddressLine4 { get; set; }
    public string PostalAddressPostCode { get; set; }
    public decimal InterestCharges { get; set; }
    public decimal TotalChargeOfCredit { get; set; }
    public string RepaymentPeriod { get; set; }
    public string ResidentialAddressLine1 { get; set; }
    public string ResidentialAddressLine2 { get; set; }
    public string ResidentialAddressLine3 { get; set; }
    public string ResidentialAddressLine4 { get; set; }
    public string ResidentialAddressPostalCode { get; set; }
    public string SalaryFrequency { get; set; }
    public decimal SettlementAmount { get; set; }
    public string StatusCode { get; set; }
    public string StatusDate { get; set; }
    public string SubAccountNo { get; set; }
    public string Lastname { get; set; }
    public string Title { get; set; }
    public decimal TotalAmountRepayable { get; set; }
    public string WorkTelCellNo { get; set; }
    public string CountryOfOrigin { get; set; }
    public string ReferencenNo { get; set; }
    public string TransactionCreateDate { get; set; }
    public string TransactionCreateTime { get; set; }

    #endregion
  }
}
