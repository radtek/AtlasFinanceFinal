using Atlas.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.Interface
{
  public interface IJob
  {
    Guid Id { get; set; }
    Int32 ThreadId { get; set; }
    DateTime StartDate { get; set; }
    DateTime? EndDate { get; set; }
    DateTime? LastExecutionDate { get; set; }
    DateTime? NextExecutionDate { get; set; }
    IStep ActiveStep { get; set; }
    bool Error { get; set; }
    Enumerators.Workflow.JobState State { get; set; }
    Enumerators.Workflow.ScheduleFrequency Frequency { get; set; }
    ICollection<IStep> Steps { get; set; }
    WFL_ProcessDTO Process { get; set; }
    WFL_ProcessJobDTO ProcessJob { get; set; }
    bool IsScheduled { get; set; }

    void StartJob();
    void StartSchedule();
    void GotoNextStep(IStep finishedStep, dynamic data = null);
    IStep GetNextStep();
    IStep GetPreviousStep();
    DateTime GetNextExecutionDate();
    void StopScheduleJob(bool restart = true);
  }
}
