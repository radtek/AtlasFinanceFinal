using Stream.Framework.Enumerators;

namespace Stream.Framework.Structures
{
  public interface ICaseStatus
  {
    int CaseStatusId { get; set; }
    CaseStatus.Type StatusType { get; set; }
    string Description { get; set; }
  }
}