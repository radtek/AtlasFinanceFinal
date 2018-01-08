using Atlas.Domain.Model;
using Atlas.Domain.Structures;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Business.Workflow
{
  public class AccountProcess
  {
    public static List<ProcessAccount> GetByAccount(long accountId)
    {
      var processAccounts = new List<ProcessAccount>();

      using (var uow = new UnitOfWork())
      {
        var processStepJobAccounts = new XPQuery<WFL_ProcessStepJobAccount>(uow).Where(p => p.Account.AccountId == accountId).ToList();
        var affordabilityOptions = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => a.Account.AccountId == accountId).ToList();

        processStepJobAccounts.ForEach((p) =>
        {
          var requestedAffordabilityOption = affordabilityOptions.FirstOrDefault(a => a.Account.Equals(p.Account)
            && a.AffordabilityOptionType.AffordabilityOptionTypeId == (int)Enumerators.Account.AffordabilityOptionType.RequestedOption);

          ACC_AffordabilityOption acceptedAffordabilityOption = null;
          if (requestedAffordabilityOption != null)
          {
            if (requestedAffordabilityOption.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.Accepted)
              acceptedAffordabilityOption = requestedAffordabilityOption;
            else
              acceptedAffordabilityOption = affordabilityOptions.FirstOrDefault(a => a.Account.Equals(p.Account)
                && a.AffordabilityOptionStatus.AffordabilityOptionStatusId == (int)Enumerators.Account.AffordabilityOptionStatus.Accepted);
          }

          var processAccount = processAccounts.FirstOrDefault(pa => pa.ProcessJobId == p.ProcessStepJob.ProcessJob.ProcessJobId);
          if (processAccount == null)
          {
            processAccount = new ProcessAccount()
            {
              FirstName = p.Account.Person.Firstname,
              IdNumber = p.Account.Person.IdNum,
              InterestRate = p.Account.AccountType == null ? 0 : p.Account.AccountType.InterestRate,
              LastName = p.Account.Person.Lastname,
              Period = acceptedAffordabilityOption == null ? requestedAffordabilityOption == null ? 0 : requestedAffordabilityOption.Period : acceptedAffordabilityOption.Period,
              PeriodFrequency = acceptedAffordabilityOption == null ? requestedAffordabilityOption == null ? string.Empty : requestedAffordabilityOption.PeriodFrequency.Description : acceptedAffordabilityOption.PeriodFrequency.Description,
              PersonId = p.Account.Person.PersonId,
              AccountCreateDate = p.Account.CreateDate,
              AccountId = p.Account.AccountId,
              AccountNo = p.Account.AccountNo,
              AccountType = p.Account.AccountType == null ? string.Empty : p.Account.AccountType.Description,
              Amount = acceptedAffordabilityOption == null ? requestedAffordabilityOption == null ? 0 : requestedAffordabilityOption.Amount : acceptedAffordabilityOption.Amount,
              AppliedAmount = requestedAffordabilityOption == null ? 0 : requestedAffordabilityOption.Amount,
              Process = p.ProcessStepJob.ProcessJob.Process.Name,
              ProcessJobId = p.ProcessStepJob.ProcessJob.ProcessJobId,
              ProcessStartDate = p.ProcessStepJob.ProcessJob.CreateDate,
              ProcessEndDate = p.ProcessStepJob.ProcessJob.CompleteDate,
              Status = p.Account.Status.Description,
              StatusChange = p.Account.StatusChangeDate,
              TotalPayBack = Convert.ToDecimal(acceptedAffordabilityOption == null ? requestedAffordabilityOption == null ? 0 : requestedAffordabilityOption.TotalPayBack : acceptedAffordabilityOption.TotalPayBack)
            };
            processAccounts.Add(processAccount);
          }
          if (processAccount.ProcessSteps == null)
            processAccount.ProcessSteps = new List<ProcessStepAccount>();

          processAccount.ProcessSteps.Add(new ProcessStepAccount()
          {
            ProcessStep = p.ProcessStepJob.ProcessStep.Name,
            ProcessStepJobAccountId = p.ProcessStepJobAccountId,
            ProcessStepJobId = p.ProcessStepJob.ProcessStepJobId,
            ProcessStepStartDate = p.ProcessStepJob.CreateDate,
            ProcessStepEndDate = p.ProcessStepJob.CompleteDate
          });
        });
      }

      return processAccounts;
    }

    public static List<ProcessAccount> GetAccountsInProcessStep(int processStepId)
    {
      using (var uow = new UnitOfWork())
      {
        var processStepJobAccounts = new XPQuery<WFL_ProcessStepJobAccount>(uow).Where(p => p.ProcessStepJob.CompleteDate == null
          && p.ProcessStepJob.ProcessStep.ProcessStepId == processStepId
          && (
            p.ProcessStepJob.JobState.JobStateId != (int)Enumerators.Workflow.JobState.Completed
            || p.ProcessStepJob.JobState.JobStateId != (int)Enumerators.Workflow.JobState.Failed
            || p.ProcessStepJob.JobState.JobStateId != (int)Enumerators.Workflow.JobState.Stopped
            || p.ProcessStepJob.JobState.JobStateId != (int)Enumerators.Workflow.JobState.Faulty)).ToList();

        return ProcessResults(uow, processStepJobAccounts);
      }
    }

    public static List<ProcessAccount> GetAll(Atlas.Enumerators.General.Host host, long? branchId, string accountNo, DateTime? startRange, DateTime? endRange)
    {
      using (var uow = new UnitOfWork())
      {
        var processStepJobAccountQuery = new XPQuery<WFL_ProcessStepJobAccount>(uow).Where(p =>
          p.Account.Host.Type == host);

        if (branchId.HasValue && branchId > 0)
          processStepJobAccountQuery = processStepJobAccountQuery.Where(p =>
            p.Account.Person.Branch.BranchId == branchId);

        if (startRange.HasValue)
          processStepJobAccountQuery = processStepJobAccountQuery.Where(p =>
            p.ProcessStepJob.CreateDate >= startRange.Value.Date
            || (p.ProcessStepJob.LastStateDate.HasValue
              && p.ProcessStepJob.LastStateDate.Value.Date >= startRange.Value.Date)
            || (p.ProcessStepJob.CompleteDate.HasValue
              && p.ProcessStepJob.CompleteDate.Value.Date >= startRange.Value.Date));

        if (endRange.HasValue)
          processStepJobAccountQuery = processStepJobAccountQuery.Where(p =>
            p.ProcessStepJob.CreateDate <= endRange.Value.Date
            || (p.ProcessStepJob.LastStateDate.HasValue
              && p.ProcessStepJob.LastStateDate.Value.Date <= endRange.Value.Date)
            || (p.ProcessStepJob.CompleteDate.HasValue
              && p.ProcessStepJob.CompleteDate.Value.Date <= endRange.Value.Date));

        if (!string.IsNullOrEmpty(accountNo))
          processStepJobAccountQuery = processStepJobAccountQuery.Where(p =>
            p.Account.AccountNo == accountNo);

        var processStepJobAccounts = processStepJobAccountQuery.ToList();

        return ProcessResults(uow, processStepJobAccounts);
      }
    }

    public static List<ProcessStepAccount> GetProcessSteps(long processJobId)
    {
      var processStepAccounts = new List<ProcessStepAccount>();

      using (var uow = new UnitOfWork())
      {
        var processStepJobs = new XPQuery<WFL_ProcessStepJob>(uow).Where(p => p.ProcessJob.ProcessJobId == processJobId).ToList();
        foreach (var processStepJob in processStepJobs)
        {
          processStepAccounts.Add(new ProcessStepAccount()
          {
            Process = processStepJob.ProcessStep.Process.Name,
            ProcessStep = processStepJob.ProcessStep.Name,
            ProcessStepEndDate = processStepJob.CompleteDate,
            ProcessStepJobId = processStepJob.ProcessStepJobId,
            ProcessStepStartDate = processStepJob.CreateDate
          });
        }
      }

      return processStepAccounts;
    }

    public static List<ProcessAccount> ProcessResults(UnitOfWork uow, List<WFL_ProcessStepJobAccount> processStepJobAccounts)
    {
      var processAccounts = new List<ProcessAccount>();

      var accountIds = processStepJobAccounts.Select(p => p.Account.AccountId).Distinct().ToArray();

      var affordabilityOptions = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => accountIds.Contains(a.Account.AccountId)).ToList();

      processStepJobAccounts.ForEach((p) =>
      {
        var requestedAffordabilityOption = affordabilityOptions.FirstOrDefault(a => a.Account.Equals(p.Account)
          && a.AffordabilityOptionType.AffordabilityOptionTypeId == (int)Enumerators.Account.AffordabilityOptionType.RequestedOption);

        ACC_AffordabilityOption acceptedAffordabilityOption = null;
        if (requestedAffordabilityOption != null)
        {
          if (requestedAffordabilityOption.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.Accepted)
            acceptedAffordabilityOption = requestedAffordabilityOption;
          else
            acceptedAffordabilityOption = affordabilityOptions.FirstOrDefault(a => a.Account.Equals(p.Account)
              && a.AffordabilityOptionStatus.AffordabilityOptionStatusId == (int)Enumerators.Account.AffordabilityOptionStatus.Accepted);
        }
        var processAccount = processAccounts.FirstOrDefault(pa => pa.ProcessJobId == p.ProcessStepJob.ProcessJob.ProcessJobId);

        if (processAccount == null)
        {
          processAccount = new ProcessAccount()
          {
            FirstName = p.Account.Person.Firstname,
            IdNumber = p.Account.Person.IdNum,
            InterestRate = p.Account.AccountType == null ? 0 : p.Account.AccountType.InterestRate,
            LastName = p.Account.Person.Lastname,
            Period = acceptedAffordabilityOption == null ? requestedAffordabilityOption == null ? 0 : requestedAffordabilityOption.Period : acceptedAffordabilityOption.Period,
            PeriodFrequency = acceptedAffordabilityOption == null ? requestedAffordabilityOption == null ? string.Empty : requestedAffordabilityOption.PeriodFrequency.Description : acceptedAffordabilityOption.PeriodFrequency.Description,
            PersonId = p.Account.Person.PersonId,
            AccountCreateDate = p.Account.CreateDate,
            AccountId = p.Account.AccountId,
            AccountNo = p.Account.AccountNo,
            AccountType = p.Account.AccountType == null ? string.Empty : p.Account.AccountType.Description,
            Amount = acceptedAffordabilityOption == null ? requestedAffordabilityOption == null ? 0 : requestedAffordabilityOption.Amount : acceptedAffordabilityOption.Amount,
            AppliedAmount = requestedAffordabilityOption == null ? 0 : requestedAffordabilityOption.Amount,
            Process = p.ProcessStepJob.ProcessJob.Process.Name,
            ProcessJobId = p.ProcessStepJob.ProcessJob.ProcessJobId,
            ProcessStartDate = p.ProcessStepJob.ProcessJob.CreateDate,
            ProcessEndDate = p.ProcessStepJob.ProcessJob.CompleteDate,
            Status = p.Account.Status.Description,
            StatusChange = p.Account.StatusChangeDate,
            TotalPayBack = Convert.ToDecimal(acceptedAffordabilityOption == null ? requestedAffordabilityOption == null ? 0 : requestedAffordabilityOption.TotalPayBack : acceptedAffordabilityOption.TotalPayBack)
          };
          processAccounts.Add(processAccount);
        }

        if (processAccount.ProcessSteps == null)
          processAccount.ProcessSteps = new List<ProcessStepAccount>();

        processAccount.ProcessSteps.Add(new ProcessStepAccount()
        {
          ProcessStep = p.ProcessStepJob.ProcessStep.Name,
          ProcessStepJobAccountId = p.ProcessStepJobAccountId,
          ProcessStepJobId = p.ProcessStepJob.ProcessStepJobId,
          ProcessStepStartDate = p.ProcessStepJob.CreateDate,
          ProcessStepEndDate = p.ProcessStepJob.CompleteDate
        });
      });

      return processAccounts;
    }

    private static void ConvertToProcessAccount(
      ref List<ProcessAccount> processAccounts, 
      WFL_ProcessStepJobAccount processStepJobAccount, 
      ACC_AffordabilityOption acceptedAffordabilityOption, 
      ACC_AffordabilityOption requestedAffordabilityOption)
    {
      var processAccount = processAccounts.FirstOrDefault(pa => pa.ProcessJobId == processStepJobAccount.ProcessStepJob.ProcessJob.ProcessJobId);

      if (processAccount == null)
      {
        processAccount = new ProcessAccount()
        {
          FirstName = processStepJobAccount.Account.Person.Firstname,
          IdNumber = processStepJobAccount.Account.Person.IdNum,
          InterestRate = processStepJobAccount.Account.AccountType == null ? 0 : processStepJobAccount.Account.AccountType.InterestRate,
          LastName = processStepJobAccount.Account.Person.Lastname,
          Period = acceptedAffordabilityOption == null ? requestedAffordabilityOption == null ? 0 : requestedAffordabilityOption.Period : acceptedAffordabilityOption.Period,
          PeriodFrequency = acceptedAffordabilityOption == null ? requestedAffordabilityOption == null ? string.Empty : requestedAffordabilityOption.PeriodFrequency.Description : acceptedAffordabilityOption.PeriodFrequency.Description,
          PersonId = processStepJobAccount.Account.Person.PersonId,
          AccountCreateDate = processStepJobAccount.Account.CreateDate,
          AccountId = processStepJobAccount.Account.AccountId,
          AccountNo = processStepJobAccount.Account.AccountNo,
          AccountType = processStepJobAccount.Account.AccountType == null ? string.Empty : processStepJobAccount.Account.AccountType.Description,
          Amount = acceptedAffordabilityOption == null ? requestedAffordabilityOption == null ? 0 : requestedAffordabilityOption.Amount : acceptedAffordabilityOption.Amount,
          AppliedAmount = requestedAffordabilityOption == null ? 0 : requestedAffordabilityOption.Amount,
          Process = processStepJobAccount.ProcessStepJob.ProcessJob.Process.Name,
          ProcessJobId = processStepJobAccount.ProcessStepJob.ProcessJob.ProcessJobId,
          ProcessStartDate = processStepJobAccount.ProcessStepJob.ProcessJob.CreateDate,
          ProcessEndDate = processStepJobAccount.ProcessStepJob.ProcessJob.CompleteDate,
          Status = processStepJobAccount.Account.Status.Description,
          StatusChange = processStepJobAccount.Account.StatusChangeDate,
          TotalPayBack = Convert.ToDecimal(acceptedAffordabilityOption == null ? requestedAffordabilityOption == null ? 0 : requestedAffordabilityOption.TotalPayBack : acceptedAffordabilityOption.TotalPayBack)
        };
        processAccounts.Add(processAccount);
      }

      if (processAccount.ProcessSteps == null)
        processAccount.ProcessSteps = new List<ProcessStepAccount>();


      processAccount.ProcessSteps.Add(new ProcessStepAccount()
      {
        ProcessStep = processStepJobAccount.ProcessStepJob.ProcessStep.Name,
        ProcessStepJobAccountId = processStepJobAccount.ProcessStepJobAccountId,
        ProcessStepJobId = processStepJobAccount.ProcessStepJob.ProcessStepJobId,
        ProcessStepStartDate = processStepJobAccount.ProcessStepJob.CreateDate,
        ProcessStepEndDate = processStepJobAccount.ProcessStepJob.CompleteDate
      });
    }
  }
}