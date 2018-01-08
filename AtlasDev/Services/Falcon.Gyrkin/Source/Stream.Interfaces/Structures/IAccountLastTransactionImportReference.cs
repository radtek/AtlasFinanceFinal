using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stream.Framework.Structures
{
  public interface IAccountLastTransactionImportReference
  {
    long AccountId { get; set; }
    long CaseId { get; set; }
    string Reference { get; set; }
    long Reference2 { get; set; }
    string LastImportReference { get; set; }
  }
}
