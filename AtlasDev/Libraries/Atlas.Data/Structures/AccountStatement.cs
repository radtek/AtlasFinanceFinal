using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Structures
{
  public class AccountStatement
  {
    public string ClientName { get; set; }
    public string IdNumber { get; set; }
    public string PhysicalAddress { get; set; }
    public string ContactNumber { get; set; }
    public string AccountNo { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal RepaymentAmount { get; set; }
    public int DaysOverdue { get; set; }
    public int Term { get; set; }
    public DateTime RepaymentDate { get; set; }
    public decimal AmountOverdue { get; set; }
    public float MonthlyInterestRate { get; set; }
    public DateTime? StartDate { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime StatementDate { get; set; }
    public decimal CurrentDue { get; set; }
    public decimal Arrears { get; set; }
    public decimal TotalDue { get; set; }
    public decimal ArrearsAging150Days { get; set; }
    public decimal ArrearsAging120Days { get; set; }
    public decimal ArrearsAging90Days { get; set; }
    public decimal ArrearsAging60Days { get; set; }
    public decimal ArrearsAging30Days { get; set; }
    public decimal ArrearsAgingCurrent { get; set; }
    public decimal ArrearsAgingTotalDue { get; set; }
    public decimal PaymentReceived { get; set; }
    public decimal InterestAccrued { get; set; }
    public decimal FeesLevied { get; set; }
    public decimal LegalFeesLevied { get; set; }
    public decimal DefaulAdminFeesLevied { get; set; }
    public decimal OtherDebits { get; set; }
    public decimal OtherCredits { get; set; }
    public List<StatementTransaction> StatementTransactions { get; set; }
  }
}
