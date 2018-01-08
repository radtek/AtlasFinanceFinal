using System;

namespace Stream.Framework.DataContracts.Requests
{
  public class AddOrUpdateAccountRequest
  {
    public long AccountId { get; set; }
    public long DebtorId { get; set; }
    public long HostId { get; set; }
    public long BranchId { get; set; }
    public string Reference { get; set; }
    public long Reference2 { get; set; }
    public string LastImportReference { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime CloseDate { get; set; }
    public decimal LoanAmount { get; set; }
    public int LoanTerm { get; set; }
    public Enumerators.Stream.FrequencyType Frequency { get; set; }
    public decimal Balance { get; set; }
    public DateTime? LastReceiptDate { get; set; }
    public decimal? LastReceiptAmount { get; set; }
    public decimal RequiredPayment { get; set; }
    public int InstalmentsOutstanding { get; set; }
    public decimal ArrearsAmount { get; set; }
    public bool? UpToDate { get; set; }
  }
}