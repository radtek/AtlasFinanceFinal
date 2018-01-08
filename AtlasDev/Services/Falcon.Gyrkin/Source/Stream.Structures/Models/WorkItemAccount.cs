using System;
using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class WorkItemAccount : IWorkItemAccount
  {
    public long AccountId { get; set; }
    public string Reference { get; set; }
    public decimal Arrears { get; set; }
    public int InstalmentsOutstanding { get; set; }
    public int LoanTerm { get; set; }
    public string Frequency { get; set; }
    public DateTime? LastReceiptDate { get; set; }
    public decimal? LastReceiptAmount { get; set; }
    public bool InArrears { get; set; }
  }
}