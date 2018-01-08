using System;

namespace Atlas.Domain.DTO
{
  public class WFL_ProcessStepJobDTO
  {
    public long ProcessStepJobId { get; set; }
    public WFL_ProcessJobDTO ProcessJob { get; set; }
    public WFL_ProcessStepDTO ProcessStep { get; set; }
    public WFL_JobStateDTO JobState { get; set; }
    public PER_PersonDTO User { get; set; }
    public DateTime? LastStateDate { get; set; }
    public DateTime? CompleteDate { get; set; }
    public DateTime CreateDate { get; set; }
  }
}