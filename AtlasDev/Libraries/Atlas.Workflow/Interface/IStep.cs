using Atlas.Domain.DTO;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.Interface
{
  public interface IStep
  {
    #region Properties

    DateTime StartDate { get; set; }
    DateTime? CompleteDate { get; set; }
    Int32 ThreadId { get; set; }
    WFL_ProcessStepDTO ProcessStep { get; set; }
    WFL_ProcessStepJobDTO ProcessStepJob { get; set; }
    Enumerators.Workflow.JobState State { get; set; }
    Enumerators.Workflow.Trigger Trigger { get; set; }
    Int32 Delay { get; set; }
    Enumerators.Workflow.PeriodFrequency? DelayPeriodFrequency { get; set; }
    bool UseDecisionGate { get; set; }
    Int32 Rank { get; set; }
    ILogger Logger { get; set; }

    bool Validate();
    void Start();
    void Start(dynamic data);
    void CreateProcessStepJobAccount(long accountId);
    WFL_ProcessStepJobDTO GetCurrentProcessStepJob(dynamic data);
    dynamic GetProcessData(Enumerators.Workflow.WorkflowDataExtType dataExtType);
    void SetProcessData(dynamic data, Enumerators.Workflow.WorkflowDataExtType dataExtType);
    //void Complete();
    void Complete(dynamic data);
    void Stop();
    void Log(string msg, Enumerators.General.Log4NetType logType);

    #endregion
  }
}
