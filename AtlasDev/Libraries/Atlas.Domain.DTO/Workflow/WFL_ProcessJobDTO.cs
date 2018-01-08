using System;

namespace Atlas.Domain.DTO
{
  public class WFL_ProcessJobDTO
  {
    public long ProcessJobId { get; set; }
    public WFL_ProcessDTO Process { get; set; }
    public WFL_JobStateDTO JobState { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime? CompleteDate { get; set; }
  }
}
