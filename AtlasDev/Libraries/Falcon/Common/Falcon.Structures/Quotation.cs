using System;

namespace Falcon.Common.Structures
{
  public class Quotation
  {
    public long QuotationId { get; set; }
    public long AccountId { get; set; }
    public string QuotationNo { get; set; }
    public long QuotationStatusId { get; set; }
    public string QuotationStatus { get; set; }
    public string QuotationStatusColor { get; set; }
    public DateTime LastStatusDate { get; set; }
    public DateTime QuoteDate { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalFees { get; set; }
    public float InterestRate { get; set; }
    public decimal CapitalAmount { get; set; }
    public decimal? InstalmentAmount { get; set; }
    public int Period { get; set; }
    public string PeriodFrequency { get; set; }
    public int NoOfInstalments { get; set; }
    public decimal? TotalRepayment { get; set; }
    public DateTime DateOfDebit { get; set; }
  }
}