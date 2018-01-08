using Atlas.Domain.Model;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Workflow.AtlasOnline
{
  public static class Default
  {
    /// <summary>
    /// Starts a workflow for a specific account
    /// The lowest ranked process will start first
    /// </summary>
    /// <param name="workflowProcess"></param>
    /// <param name="accountId"></param>
    /// <param name="userId"></param>
    public static void StartWorkflow(Enumerators.Workflow.WorkflowProcess workflowProcess, long accountId, long userId = (long)Enumerators.General.Person.System)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist in the DB", accountId));

        var processJob = new WFL_ProcessJob(uow);
        processJob.CreateDate = DateTime.Now;
        processJob.JobState = new XPQuery<WFL_JobState>(uow).First(j => j.JobStateId == (int)Enumerators.Workflow.JobState.Ready);
        processJob.LastStateDate = DateTime.Now;

        var workflowId = (int)workflowProcess;
        var workflow = new XPQuery<WFL_WorkflowHost>(uow).FirstOrDefault(w => w.Workflow.WorkflowId == workflowId && w.Host.HostId == account.Host.HostId).Workflow;
        if (workflow == null)
          throw new Exception("Workflow does not exist for this host");
        processJob.Process = new XPQuery<WFL_Process>(uow).Where(p => p.Workflow == workflow && p.Enabled).OrderBy(p => p.Rank).FirstOrDefault();

        if (processJob.Process == null)
          throw new Exception("There are no processes for this Workflow");

        var processSetJob = new WFL_ProcessStepJob(uow);
        processSetJob.CreateDate = DateTime.Now;
        processSetJob.ProcessJob = processJob;
        processSetJob.ProcessStep = processJob.Process.ProcessSteps.OrderBy(p => p.Rank).FirstOrDefault();
        processSetJob.JobState = new XPQuery<WFL_JobState>(uow).First(j => j.JobStateId == (int)Enumerators.Workflow.JobState.Executing);
        processSetJob.LastStateDate = DateTime.Now;
        processSetJob.User = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == userId);

        var processStepJobAccount = new WFL_ProcessStepJobAccount(uow);
        processStepJobAccount.Account = account;
        processStepJobAccount.ProcessStepJob = processSetJob;

        uow.CommitChanges();
      }
    }

    /// <summary>
    /// Starts a parallel process step for a specific workflow and account
    /// </summary>
    /// <param name="workflowProcess">The workflow the process Step belongs to </param>
    /// <param name="accountId"></param>
    /// <param name="processStepId">The parallel processstep to start</param>
    /// <param name="userId"></param>
    public static void StartParallelProcessStep(Enumerators.Workflow.WorkflowProcess workflowProcess, long accountId, int processStepId, long userId = (long)Enumerators.General.Person.System)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist in the DB", accountId));

        var workflowId = (int)workflowProcess;
        var processStepJobAccount = new XPQuery<WFL_ProcessStepJobAccount>(uow).Where(p => p.Account == account
          && p.ProcessStepJob.ProcessStep.Process.Workflow.WorkflowId == workflowId
          && (p.ProcessStepJob.JobState.Type != Enumerators.Workflow.JobState.Completed
            && p.ProcessStepJob.JobState.Type != Enumerators.Workflow.JobState.Failed
            && p.ProcessStepJob.JobState.Type != Enumerators.Workflow.JobState.Stopped)
          && p.ProcessStepJob.CompleteDate == null
          && p.ProcessStepJob.ProcessStep.Enabled).FirstOrDefault();

        var processSetJob = new WFL_ProcessStepJob(uow);
        processSetJob.CreateDate = DateTime.Now;
        processSetJob.ProcessJob = processStepJobAccount.ProcessStepJob.ProcessJob;
        processSetJob.ProcessStep = processStepJobAccount.ProcessStepJob.ProcessStep.Process.ProcessSteps.Where(p => p.ProcessStepId == processStepId).FirstOrDefault();
        processSetJob.JobState = new XPQuery<WFL_JobState>(uow).First(j => j.JobStateId == (int)Enumerators.Workflow.JobState.Executing);
        processSetJob.LastStateDate = DateTime.Now;
        processSetJob.User = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == userId);

        var newProcessStepJobAccount = new WFL_ProcessStepJobAccount(uow);
        newProcessStepJobAccount.Account = account;
        newProcessStepJobAccount.ProcessStepJob = processSetJob;

        uow.CommitChanges();
      }
    }

    /// <summary>
    /// Completes a specified process step and then starts the next ranked step
    /// If the are no process steps after the current step, the process will complete 
    /// If completing a parallel task, the next step will not automatically kick in
    /// Can also complete a step and thereafter start a specified step, irrespective of whether the current step is a parallel step
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="currentProcessStepId"> Atlas.Enumerators.Workflow.ProcessStep The step to complete</param>
    /// <param name="nextProcessStepId"> Atlas.Enumerators.Workflow.ProcessStep The next step to start</param>
    /// <param name="userId"></param>
    public static void StepProcess(long accountId, int currentProcessStepId = 0, int nextProcessStepId = 0, long userId = (long)Enumerators.General.Person.System)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist in the DB", accountId));

        var processStepJobAccountQuery = new XPQuery<WFL_ProcessStepJobAccount>(uow).Where(p => p.Account == account
          && (p.ProcessStepJob.JobState.JobStateId != (int)Enumerators.Workflow.JobState.Completed
            && p.ProcessStepJob.JobState.JobStateId != (int)Enumerators.Workflow.JobState.Failed
            && p.ProcessStepJob.JobState.JobStateId != (int)Enumerators.Workflow.JobState.Stopped)
          && p.ProcessStepJob.CompleteDate == null
          && p.ProcessStepJob.ProcessStep.Enabled);

        WFL_ProcessStepJobAccount processStepJobAccount;
        if (currentProcessStepId == 0)
        {
          processStepJobAccount = processStepJobAccountQuery.Where(p => !p.ProcessStepJob.ProcessStep.IsParallelStep).OrderBy(a => a.ProcessStepJob.CreateDate).FirstOrDefault();
        }
        else
        {
          processStepJobAccount = processStepJobAccountQuery.FirstOrDefault(p => p.ProcessStepJob.ProcessStep.ProcessStepId == currentProcessStepId);
        }

        if (processStepJobAccount == null)
          throw new Exception(string.Format("Process Step Job for Account {0}, does not exist", accountId));

        processStepJobAccount.ProcessStepJob.CompleteDate = DateTime.Now;
        processStepJobAccount.ProcessStepJob.JobState = new XPQuery<WFL_JobState>(uow).First(j => j.JobStateId == (int)Enumerators.Workflow.JobState.Completed);
        processStepJobAccount.ProcessStepJob.LastStateDate = DateTime.Now;

        var tryProcessComplete = true;

        if (!processStepJobAccount.ProcessStepJob.ProcessStep.IsParallelStep || nextProcessStepId > 0)
        {
          WFL_ProcessStep nextProcessStep = null;
          if (nextProcessStepId == 0)
          {
            nextProcessStep = processStepJobAccount.ProcessStepJob.ProcessStep.Process.ProcessSteps.Where(p => p.Rank > processStepJobAccount.ProcessStepJob.ProcessStep.Rank && !p.IsParallelStep).OrderBy(p => p.Rank).FirstOrDefault();
          }
          else
          {
            nextProcessStep = processStepJobAccount.ProcessStepJob.ProcessStep.Process.ProcessSteps.Where(p => p.ProcessStepId == nextProcessStepId).FirstOrDefault();
            if (nextProcessStep == null)
              throw new Exception(string.Format("There is no Process Step {0} in Process {1}", nextProcessStepId, processStepJobAccount.ProcessStepJob.ProcessStep.Process.ProcessId));
          }

          if (nextProcessStep != null)
          {
            if (nextProcessStep.ProcessStepId == processStepJobAccount.ProcessStepJob.ProcessStep.ProcessStepId)
              return; // If next step is the same as current step, do not add a new processStepJob

            // Start the next step of the process
            var newProcessSetJob = new WFL_ProcessStepJob(uow);
            newProcessSetJob.CreateDate = DateTime.Now;
            newProcessSetJob.ProcessJob = processStepJobAccount.ProcessStepJob.ProcessJob;
            newProcessSetJob.ProcessStep = nextProcessStep;
            newProcessSetJob.JobState = new XPQuery<WFL_JobState>(uow).First(j => j.JobStateId == (int)Enumerators.Workflow.JobState.Executing);
            newProcessSetJob.LastStateDate = DateTime.Now;
            newProcessSetJob.User = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == userId);

            var newProcessStepJobAccount = new WFL_ProcessStepJobAccount(uow);
            newProcessStepJobAccount.Account = account;
            newProcessStepJobAccount.ProcessStepJob = newProcessSetJob;
            tryProcessComplete = false;
          }
        }

        uow.CommitChanges();

        if (tryProcessComplete)
          CompleteProcess(accountId);
      }
    }

    /// <summary>
    /// Allows a user to change the current process the account should be in
    /// </summary>
    /// <param name="processStepJobAccountId">the current ProcessStepJobAccountId, which holds the current process step and account</param>
    /// <param name="newProcessStepId">the process step the account should be moved to</param>
    /// <param name="redirectingUserId">the user who is performing the change</param>
    public static void RedirectAccount(long processStepJobAccountId, int newProcessStepId, long redirectingUserId)
    {
      using (var uow = new UnitOfWork())
      {
        var processStepJobAccount = new XPQuery<WFL_ProcessStepJobAccount>(uow).First(p => p.ProcessStepJobAccountId == processStepJobAccountId);

        var nextProcessStep = new XPQuery<WFL_ProcessStep>(uow).First(p => p.ProcessStepId == newProcessStepId);

        // Complete current Process Step Job
        processStepJobAccount.ProcessStepJob.CompleteDate = DateTime.Now;
        processStepJobAccount.ProcessStepJob.JobState = new XPQuery<WFL_JobState>(uow).First(j => j.JobStateId == (int)Enumerators.Workflow.JobState.Completed);
        processStepJobAccount.ProcessStepJob.LastStateDate = DateTime.Now;
        processStepJobAccount.Override = true;
        processStepJobAccount.OverrideUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == redirectingUserId);

        // Start the next step of the process
        var newProcessSetJob = new WFL_ProcessStepJob(uow);
        newProcessSetJob.CreateDate = DateTime.Now;
        newProcessSetJob.ProcessJob = processStepJobAccount.ProcessStepJob.ProcessJob;
        newProcessSetJob.ProcessStep = nextProcessStep;
        newProcessSetJob.JobState = new XPQuery<WFL_JobState>(uow).First(j => j.JobStateId == (int)Enumerators.Workflow.JobState.Executing);
        newProcessSetJob.LastStateDate = DateTime.Now;
        newProcessSetJob.User = processStepJobAccount.ProcessStepJob.User;

        var newProcessStepJobAccount = new WFL_ProcessStepJobAccount(uow);
        newProcessStepJobAccount.Account = processStepJobAccount.Account;
        newProcessStepJobAccount.ProcessStepJob = newProcessSetJob;

        uow.CommitChanges();
      }
    }

    /// <summary>
    /// Complete a process if there are no active process steps for a specific account
    /// </summary>
    /// <param name="accountId"></param>
    public static void CompleteProcess(long accountId)
    {
      using (var uow = new UnitOfWork())
      {
        var processJobs = new XPQuery<WFL_ProcessStepJobAccount>(uow).Where(p =>
          p.Account.AccountId == accountId
          && p.ProcessStepJob.ProcessJob.JobState.JobStateId != (int)Enumerators.Workflow.JobState.Completed
          && p.ProcessStepJob.ProcessJob.JobState.JobStateId != (int)Enumerators.Workflow.JobState.Failed
          && p.ProcessStepJob.ProcessJob.JobState.JobStateId != (int)Enumerators.Workflow.JobState.Stopped
          && p.ProcessStepJob.JobState.JobStateId == (int)Enumerators.Workflow.JobState.Completed
          && p.ProcessStepJob.JobState.JobStateId == (int)Enumerators.Workflow.JobState.Failed
          && p.ProcessStepJob.JobState.JobStateId == (int)Enumerators.Workflow.JobState.Stopped
          && p.ProcessStepJob.JobState.JobStateId == (int)Enumerators.Workflow.JobState.Executing
          && p.ProcessStepJob.JobState.JobStateId == (int)Enumerators.Workflow.JobState.Ready)
          .Select(p => p.ProcessStepJob.ProcessJob);

        foreach (var processJob in processJobs)
        {
          processJob.CompleteDate = DateTime.Now;
          processJob.JobState = new XPQuery<WFL_JobState>(uow).First(j => j.JobStateId == (int)Enumerators.Workflow.JobState.Completed);
          processJob.LastStateDate = DateTime.Now;
        }

        uow.CommitChanges();
      }
    }
  }
}