using Stream.Framework.Structures;

namespace Stream.Structures.Models
{
  public sealed class CaseStatus : ICaseStatus
  {
    public int CaseStatusId { get; set; }
    public Framework.Enumerators.CaseStatus.Type StatusType { get; set; }
    public string Description { get; set; }
  }
}