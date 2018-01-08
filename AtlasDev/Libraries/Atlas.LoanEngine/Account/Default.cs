using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Common.Extensions;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;


namespace Atlas.LoanEngine.Account
{
  public class Default
  {
    private long _accountId { get; set; }

    public Default()
    {
    }

    public Default(long accountId)
    {
      // Cache Affordability Items
      var cacheAttempts = 0;
      while (!Cache.Account.IsCached && cacheAttempts <= 3)
      {
        cacheAttempts++;
        Cache.Account.StartCache();
      }
      if (!Cache.Account.IsCached)
      {
        throw new Exception("Cannot cache Affordability Items");
      }

      if (accountId == 0)
        throw new Exception("AccountId cannot be 0");

      _accountId = accountId;
    }

    public long CreateAccount(decimal loanAmount, int period, int periodFrequencyId, long personId, int hostId)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new ACC_Account(uow)
        {
          LoanAmount = loanAmount,
          Period = period,
          PeriodFrequency = new XPQuery<ACC_PeriodFrequency>(uow).FirstOrDefault(p => p.PeriodFrequencyId == periodFrequencyId),
          Person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId),
          Host = new XPQuery<Host>(uow).FirstOrDefault(h => h.HostId == hostId),
          CreateDate = DateTime.Now,
          CreatedBy = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == 0)
        };

        uow.CommitChanges();

        account.AccountNo = GenerateAccountNo(AutoMapper.Mapper.Map<ACC_Account, ACC_AccountDTO>(account));

        Log.Save(uow, account, "Initial account created");

        // Set initial status
        SetAccountStatus(account, Enumerators.Account.AccountStatus.Inactive, null, null, uow);

        uow.CommitChanges();

        return account.AccountId;
      }
    }

    public void ApproveAccount(ACC_Account account, UnitOfWork uow = null)
    {
      var commit = uow == null;
      if (commit)
      {
        uow = new UnitOfWork();
        account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == account.AccountId);
      }
      SetAccountStatus(account, Atlas.Enumerators.Account.AccountStatus.Approved, null, null, uow);

      var acceptedAffordabilityOptions = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => a.Account == account &&
        a.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.Accepted).ToList();

      if (acceptedAffordabilityOptions.Count == 0)
        throw new Exception("There is no affordability for the account");

      if (acceptedAffordabilityOptions.Count > 1)
        throw new Exception("There are more than one accepted option");

      var acceptedAffordabilityOption = acceptedAffordabilityOptions.FirstOrDefault();

      // TODO: This will change when we add in settlements, third party payouts, extra
      account.PayoutAmount = acceptedAffordabilityOption.Amount;

      if (commit)
      {
        uow.CommitChanges();
      }
    }

    public void SetAccountStatus(ACC_Account account, Enumerators.Account.AccountStatus status, Enumerators.Account.AccountStatusReason? reason, 
      Enumerators.Account.AccountStatusSubReason? subReason, UnitOfWork uow = null)
    {
      var commit = uow == null;
      if (commit)
      {
        uow = new UnitOfWork();
        account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == account.AccountId);
      }

      // Does not set status, if new status is the same as current
      if (account.Status != null && status == account.Status.Type)
        return;

      var accountStatus = new ACC_AccountStatus(uow)
      {
        Account = account,
          //Edited By Prashant
          //Status = new XPQuery<ACC_Status>(uow).FirstOrDefault(s => s.Type == status),
          Status = new XPQuery<ACC_Status>(uow).FirstOrDefault(s => s.StatusId == (int)status),
        CreateDate = DateTime.Now
      };
            //Edited By Prashant
            //account.Status = new XPQuery<ACC_Status>(uow).FirstOrDefault(s => s.Type == status);
            account.Status = new XPQuery<ACC_Status>(uow).FirstOrDefault(s => s.StatusId == (int)status);
      account.StatusChangeDate = DateTime.Now;

      if (status == Enumerators.Account.AccountStatus.Cancelled
        || status == Enumerators.Account.AccountStatus.Declined
        || status == Enumerators.Account.AccountStatus.Technical_Error)
      {
        // End Workflow 
        var processStepJobs = new XPQuery<WFL_ProcessStepJobAccount>(uow).Where(p =>
          p.Account.AccountId == account.AccountId
          && p.ProcessStepJob.ProcessJob.JobState.JobStateId != (int)Enumerators.Workflow.JobState.Completed
          && p.ProcessStepJob.ProcessJob.JobState.JobStateId != (int)Enumerators.Workflow.JobState.Failed
          && p.ProcessStepJob.ProcessJob.JobState.JobStateId != (int)Enumerators.Workflow.JobState.Stopped
          && p.ProcessStepJob.JobState.JobStateId == (int)Enumerators.Workflow.JobState.Completed
          && p.ProcessStepJob.JobState.JobStateId == (int)Enumerators.Workflow.JobState.Failed
          && p.ProcessStepJob.JobState.JobStateId == (int)Enumerators.Workflow.JobState.Stopped)
          .Select(p => p.ProcessStepJob);

        foreach (var processStepJob in processStepJobs)
        {
          processStepJob.CompleteDate = DateTime.Now;
          processStepJob.JobState = new XPQuery<WFL_JobState>(uow).First(j => j.JobStateId == (int)Enumerators.Workflow.JobState.Completed);
          processStepJob.LastStateDate = DateTime.Now;

          if (processStepJob.ProcessJob.CompleteDate != null)
          {
            processStepJob.ProcessJob.CompleteDate = DateTime.Now;
            processStepJob.ProcessJob.JobState = new XPQuery<WFL_JobState>(uow).First(j => j.JobStateId == (int)Enumerators.Workflow.JobState.Completed);
            processStepJob.ProcessJob.LastStateDate = DateTime.Now;
          }
        }
      }

      if (reason != null)
        account.StatusReason = new XPQuery<ACC_StatusReason>(uow).FirstOrDefault(s => s.StatusReasonId == (int)reason);
      if (subReason != null)
        account.StatusSubReason = new XPQuery<ACC_StatusSubReason>(uow).FirstOrDefault(s => s.StatusSubReasonId == (int)subReason);

      if (commit)
      {
        if (account.Status == null)
          Log.Save(uow, account, string.Format("Changed account status to [{0}]", status.ToStringEnum()));
        else
          Log.Save(uow, account, string.Format("Changed status from [{0}] to [{1}]", account.Status.Type.ToStringEnum(), status.ToStringEnum()));
      }
    }

    public void OpenAccount()
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == _accountId);

        if (account == null)
          throw new Exception(string.Format("AccountId {0} does not exist in DB", _accountId));

        if (account.Status.Type == Enumerators.Account.AccountStatus.Open)
          return;

        // Write Initial Payout Amount to GL Transaction
        var loanDisbursement = new LGR_Transaction(uow)
        {
          Account = account,
          Amount = account.LoanAmount,
          CreateDate = DateTime.Now,
          CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)Enumerators.General.Person.System),
          Person = account.Person,
          TransactionDate = DateTime.Today,
          TransactionType = new XPQuery<LGR_TransactionType>(uow).FirstOrDefault(t => t.TransactionTypeId == (int)Enumerators.GeneralLedger.TransactionType.LoanDisbursement),
          Type = new XPQuery<LGR_TransactionType>(uow).FirstOrDefault(t => t.TransactionTypeId == (int)Enumerators.GeneralLedger.TransactionType.LoanDisbursement).TransactionTypeGroup.Type
        };

        var acceptedAffordabilityOptions = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => a.Account == account &&
          a.AffordabilityOptionStatus.AffordabilityOptionStatusId == (int)Enumerators.Account.AffordabilityOptionStatus.Accepted).ToList();

        if (acceptedAffordabilityOptions.Count == 0)
          throw new Exception("There is no affordability for the account");

        if (acceptedAffordabilityOptions.Count > 1)
          throw new Exception("There are more than one accepted option");

        var acceptedAffordabilityOption = acceptedAffordabilityOptions.FirstOrDefault();

        var acceptedAffordabilityOptionFees = new XPQuery<ACC_AffordabilityOptionFee>(uow).Where(a =>
          a.AffordabilityOption == acceptedAffordabilityOption).ToList();

        foreach (var acceptedAfordabilityOptionFee in acceptedAffordabilityOptionFees)
        {
          // Write to Account Fees
          var accountFee = new ACC_AccountFee(uow)
          {
            Account = account,
            AccountTypeFee = new XPQuery<ACC_AccountTypeFee>(uow).FirstOrDefault(t => t.AccountTypeFeeId == acceptedAfordabilityOptionFee.AccountTypeFee.AccountTypeFeeId),
            Amount = acceptedAfordabilityOptionFee.Amount
          };

          // Write Fees to GL Transactions
          var transaction = new LGR_Transaction(uow)
          {
            Account = account,
            Amount = acceptedAfordabilityOptionFee.Amount,
            CreateDate = DateTime.Now,
            CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)Enumerators.General.Person.System),
            Person = account.Person,
            TransactionDate = DateTime.Today,
            TransactionType = accountFee.AccountTypeFee.Fee.TransactionType,
            Type = accountFee.AccountTypeFee.Fee.TransactionType.TransactionTypeGroup.Type,
            Fee = acceptedAfordabilityOptionFee.AccountTypeFee.Fee
          };
        }

        //// Update Account 
        //account.Status = new XPQuery<ACC_Status>(uow).FirstOrDefault(s => s.Type == Enumerators.Account.AccountStatus.Open);
        //account.StatusChangeDate = DateTime.Now;
        // Set open status
        SetAccountStatus(account, Enumerators.Account.AccountStatus.Open, null, null, uow);
        account.OpenDate = acceptedAffordabilityOption.LastStatusDate ?? DateTime.Now;
        var payRule = new XPQuery<ACC_AccountPayRule>(uow).FirstOrDefault(d => d.Enabled && d.Account == account);
        if (account.LoanAmount != acceptedAffordabilityOption.Amount
          || account.InstalmentAmount != Convert.ToDecimal(acceptedAffordabilityOption.Instalment))
          throw new Exception("Cannot open account! Accepted Affordability does not match");

        if (account.PeriodFrequency.Type == Enumerators.Account.PeriodFrequency.Daily)
        {
          account.FirstInstalmentDate = (acceptedAffordabilityOption.LastStatusDate ?? account.OpenDate.Value).AddDays(account.Period);
        }
        else
        {
          // TODO: Test properly when adding weekly/monthly loans
          var firstInstalmentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, payRule.PayDate.DayNo);
          if (firstInstalmentDate.Date <= DateTime.Today.Date)
          {
            if (account.PeriodFrequency.Type == Enumerators.Account.PeriodFrequency.Monthly)
              firstInstalmentDate = firstInstalmentDate.AddMonths(1);
            else
              firstInstalmentDate = firstInstalmentDate.AddDays(7);
          }
          if ((firstInstalmentDate.Date - account.OpenDate.Value.Date).TotalDays < account.AccountType.BufferDaysFirstInstalmentDate)
          {
            if (account.PeriodFrequency.Type == Enumerators.Account.PeriodFrequency.Monthly)
              firstInstalmentDate = firstInstalmentDate.AddMonths(1);
            else
              firstInstalmentDate = firstInstalmentDate.AddDays(7);
          }
        }

        var bankDetail = new XPQuery<PYT_Payout>(uow).FirstOrDefault(p => p.Account == account && p.PayoutStatus.Status == Enumerators.Payout.PayoutStatus.Successful).BankDetail;
        // Initiate Debit Order
        if (payRule != null)
        {
          var dbtControl = new DBT_Control(uow)
          {
            Service = new XPQuery<DBT_HostService>(uow).FirstOrDefault(hs => hs.Host == account.Host && hs.Enabled).Service,
            Bank = bankDetail.Bank,
            BankAccountName = bankDetail.AccountName,
            BankAccountNo = bankDetail.AccountNum,
            BankAccountType = bankDetail.AccountType,
            BankBranchCode = bankDetail.Code,
            BankStatementReference = account.AccountNo,
            ControlStatus = new XPQuery<DBT_ControlStatus>(uow).FirstOrDefault(d => d.Type == Enumerators.Debit.ControlStatus.New),
            LastStatusDate = DateTime.Now,
            TrackingDays = account.AccountType.DefaultTrackingDays,
            CurrentRepetition = 0,
            Repetitions = account.NumOfInstalments,
            Host = account.Host,
            ControlType = new XPQuery<DBT_ControlType>(uow).FirstOrDefault(c => c.Type == Enumerators.Debit.ControlType.Predictive),
            AVSCheckType = new XPQuery<DBT_AVSCheckType>(uow).FirstOrDefault(a => a.Type == Enumerators.Debit.AVSCheckType.None),
            FailureType = new XPQuery<DBT_FailureType>(uow).FirstOrDefault(f => f.Type == Enumerators.Debit.FailureType.Retry),
            FirstInstalmentDate = account.FirstInstalmentDate.Value,
            IdNumber = account.Person.IdNum,
            Instalment = account.InstalmentAmount,
            LastInstalmentUpdate = DateTime.Now,
            PayDate = payRule.PayDate,
            PayRule = payRule.PayRule,
            PeriodFrequency = account.PeriodFrequency,
            CreateDate = DateTime.Now,
            CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)Enumerators.General.Person.System),
            IsValid = true
          };
          var accountDebitControl = new ACC_DebitControl(uow)
          {
            Account = account,
            Control = dbtControl,
            CreateDate = DateTime.Now
          };
        }
        uow.CommitChanges();
      }
    }

    public void UpdateAccountBalance(decimal? newBalance = null)
    {
      var canCloseAccount = false;

      if (newBalance == null)
      {
        // Calculate new Balance
        newBalance = new GeneralLedger(_accountId).CalculateBalance(null);
      }

      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == _accountId);
        if (account == null)
          throw new Exception(string.Format("AccountId {0} does not exist in DB", _accountId));

        account.AccountBalance = (decimal)newBalance;

        // check if account balance is low enough to have the account closed
        canCloseAccount = (account.AccountBalance <= account.AccountType.CloseBalance);

        if (!canCloseAccount)
        {
          // Recalculate Instalment
          if (account.Host.Type == Enumerators.General.Host.Atlas_Online
            && account.PeriodFrequency.Type == Enumerators.Account.PeriodFrequency.Daily)
            account.InstalmentAmount = account.AccountBalance;
        }

        uow.CommitChanges();
      }

      if (canCloseAccount)
        CloseAccount();
    }

    public void CloseAccount()
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == _accountId);

        if (account == null)
          throw new Exception(string.Format("AccountId {0} does not exist in DB", _accountId));

        if (account.AccountBalance <= account.AccountType.CloseBalance)
        {
          account.Status = new XPQuery<ACC_Status>(uow).FirstOrDefault(s => s.Type == Enumerators.Account.AccountStatus.Closed);

          // Set status to closed!
          SetAccountStatus(account, Enumerators.Account.AccountStatus.Closed, null, null, uow);

          uow.CommitChanges();
        }
        else
        {
          throw new Exception(string.Format("Cannot close account {0} - There is still an outstanding balance", _accountId));
        }
      }
    }

    /// <summary>
    /// getCycle Start/Stop of account
    /// </summary>
    /// <returns>Tuple.Item1 = StartDate, Tuple.Item2 = EndDate</returns>
    public DateTime GetFirstInstalmentDate()
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == _accountId);

        if (account == null)
          throw new Exception(string.Format("AccountId {0} does not exist in DB", _accountId));

        if (account.PeriodFrequency.Type == Enumerators.Account.PeriodFrequency.Daily)
          return account.OpenDate.Value.AddDays(account.Period).Date;

        var payRule = new XPQuery<ACC_AccountPayRule>(uow).FirstOrDefault(p => p.Account == account && p.Enabled);
        if (payRule == null)
          throw new Exception(string.Format("PayRule for Account {0} does not exist in DB", _accountId));

        var firstInstalmentDate = account.OpenDate.Value.AddDays((account.AccountType.BufferDaysFirstInstalmentDate ?? 0) + 1);

        return RefineInstalmentDate(firstInstalmentDate, account.PeriodFrequency.Type, payRule.PayDate.DayNo);
      }
    }

    public DateTime GetNextInstalmentDate(DateTime lastInstalmentDate)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == _accountId);

        if (account == null)
          throw new Exception(string.Format("AccountId {0} does not exist in DB", _accountId));

        if (account.PeriodFrequency.Type == Enumerators.Account.PeriodFrequency.Daily)
        {
          if (lastInstalmentDate < GetFirstInstalmentDate())
            return account.OpenDate.Value.AddDays(account.Period).Date;
          else
            return RefineInstalmentDate(lastInstalmentDate.AddMonths(1), Enumerators.Account.PeriodFrequency.Monthly, account.OpenDate.Value.AddDays(account.Period).Day);
        }

        var payRule = new XPQuery<ACC_AccountPayRule>(uow).FirstOrDefault(p => p.Account == account && p.Enabled);
        if (payRule == null)
          throw new Exception(string.Format("PayRule for Account {0} does not exist in DB", _accountId));

        var nextInstalmentDate = account.PeriodFrequency.Type == Enumerators.Account.PeriodFrequency.Weekly
          ? lastInstalmentDate.AddDays(7)
            : lastInstalmentDate.AddMonths(1);
        return RefineInstalmentDate(lastInstalmentDate, account.PeriodFrequency.Type, payRule.PayDate.DayNo);
      }
    }

    private DateTime RefineInstalmentDate(DateTime instalmentDate, Enumerators.Account.PeriodFrequency periodFrequency, int dayNo)
    {
      if (periodFrequency == Enumerators.Account.PeriodFrequency.Weekly)
      {
        if (dayNo >= 7)
        {
          // Assign default day - Friday
          dayNo = (int)DayOfWeek.Friday;
        }

        var found = false;
        do
        {
          if ((int)instalmentDate.DayOfWeek == dayNo)
            found = true;
          else
            instalmentDate = instalmentDate.AddDays(1);
        } while (!found);
      }
      else // Monthly
      {
        if (instalmentDate.Day < dayNo)
        {
          if (DateTime.DaysInMonth(instalmentDate.Year, instalmentDate.Month) >= dayNo)
          {
            instalmentDate = new DateTime(instalmentDate.Year, instalmentDate.Month, dayNo);
          }
          else if (DateTime.DaysInMonth(instalmentDate.Year, instalmentDate.Month) == instalmentDate.Day)
          {
            instalmentDate = new DateTime(instalmentDate.Year, instalmentDate.Month, DateTime.DaysInMonth(instalmentDate.Year, instalmentDate.Month));
          }
          else
          {
            do
            {
              instalmentDate = instalmentDate.AddDays(1);
            } while (instalmentDate.Day < dayNo && instalmentDate.AddDays(1).Month == instalmentDate.Month);
          }
        }
        else if (instalmentDate.Day > dayNo)
        {
          instalmentDate = instalmentDate.AddMonths(1);
          while (instalmentDate.Day > dayNo)
          {
            instalmentDate = instalmentDate.AddDays(-1);
          }
        }
      }

      return instalmentDate;
    }

    public List<Tuple<DateTime, DateTime>> GetCycles(ACC_AccountDTO account)
    {
      var cycles = new List<Tuple<DateTime, DateTime>>();

      var currentCycleStartDate = account.OpenDate.Value.Date;
      var currentCycleEndDate = GetFirstInstalmentDate().Date;
      cycles.Add(new Tuple<DateTime, DateTime>(currentCycleStartDate, currentCycleEndDate));

      // getCurrentCycle
      while (currentCycleEndDate <= DateTime.Today)
      {
        currentCycleStartDate = currentCycleEndDate.AddDays(1);
        currentCycleEndDate = GetNextInstalmentDate(currentCycleEndDate);
        cycles.Add(new Tuple<DateTime, DateTime>(currentCycleStartDate, currentCycleEndDate));
      }

      return cycles.OrderBy(c => c.Item1).ToList();
    }

    // generate the Account No based on the host and Account Id
    private static string GenerateAccountNo(ACC_AccountDTO account)
    {
      string code = string.Empty;

      if (account.Host.Type == Enumerators.General.Host.ASS)
        code = "ASS";
      else if (account.Host.Type == Enumerators.General.Host.Atlas_Online)
        code = "AOL";

      return string.Format("{0}{1}", code, account.AccountId.ToString("D7"));
    }
  }
}