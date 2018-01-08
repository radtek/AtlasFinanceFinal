using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  public class WFL_JobStateDTO
  {
    public int JobStateId { get; set; }
    public Enumerators.Workflow.JobState Type
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.JobState>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.JobState>();
      }
    }
    public string Name { get; set; }
  }
}