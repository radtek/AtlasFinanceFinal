using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stream.Framework.Structures
{
  public interface IWorkItemAccount
  {
    long AccountId { get; set; }
    string Reference { get; set; }
    decimal Arrears { get; set; }
    int InstalmentsOutstanding { get; set; }
    int LoanTerm { get; set; }
    string Frequency { get; set; }
    DateTime? LastReceiptDate { get; set; }
    decimal? LastReceiptAmount { get; set; }
    bool InArrears { get; set; }
  }
}