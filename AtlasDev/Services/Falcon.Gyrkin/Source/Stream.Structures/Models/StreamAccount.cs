using System;
using Falcon.Common.Interfaces.Structures;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class StreamAccount : IStreamAccount
  {
    public long AccountId { get; set; }
    public IDebtor Debtor { get; set; }
    public long HostId { get; set; }
    public IBranch Branch { get; set; }
    public string Reference { get; set; }
    public long Reference2 { get; set; }
    public string LastImportReference { get; set; }
    public DateTime LoanDate { get; set; }
    public DateTime OpenDate { get; set; }
    public DateTime CloseDate { get; set; }
    public decimal LoanAmount { get; set; }
    public int LoanTerm { get; set; }
    public IFrequency Frequency { get; set; }
    public decimal Balance { get; set; }
    public DateTime? LastReceiptDate { get; set; }
    public decimal? LastReceiptAmount { get; set; }
    public decimal RequiredPayment { get; set; }
    public int InstalmentsOutstanding { get; set; }
    public decimal ArrearsAmount { get; set; }
  }
}