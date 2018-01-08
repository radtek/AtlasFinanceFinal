using System;
using Falcon.Common.Interfaces.Structures;

namespace Stream.Framework.Structures
{
  public interface IStreamAccount
  {
    long AccountId { get; set; }
    IDebtor Debtor { get; set; }
    long HostId { get; set; }
    IBranch Branch { get; set; }
    string Reference { get; set; }
    long Reference2 { get; set; }
    string LastImportReference { get; set; }
    DateTime LoanDate { get; set; }
    DateTime OpenDate { get; set; }
    DateTime CloseDate { get; set; }
    decimal LoanAmount { get; set; }
    int LoanTerm { get; set; }
    IFrequency Frequency { get; set; }
    decimal Balance { get; set; }
    DateTime? LastReceiptDate { get; set; }
    decimal? LastReceiptAmount { get; set; }
    decimal RequiredPayment { get; set; }
    int InstalmentsOutstanding { get; set; }
    decimal ArrearsAmount { get; set; }
  }
}

