using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public class AccountLastTransactionImportReference : IAccountLastTransactionImportReference
  {
    public long AccountId { get; set; }
    public long CaseId { get; set; }
    public string Reference { get; set; }
    public long Reference2 { get; set; }
    public string LastImportReference { get; set; }
  }
}
