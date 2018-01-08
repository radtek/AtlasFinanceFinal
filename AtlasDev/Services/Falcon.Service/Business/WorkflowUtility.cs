using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Business.Workflow;
using Atlas.Common.ExceptionBase;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using DevExpress.Xpo;
using Falcon.Common.Structures;
using Account = Atlas.Enumerators.Account;

namespace Falcon.Service.Business
{
  public class WorkflowUtility
  {
    public static List<ProcessAccount> GetWorkflow(General.Host host, long? branchId, string accountNo, DateTime? startRange, DateTime? endRange)
    {
      var processAccounts = new List<ProcessAccount>();
      var processAccountList = AccountProcess.GetAll(host, branchId, accountNo, startRange, endRange);
      foreach (var processAccount in processAccountList)
      {
        processAccounts.Add(new ProcessAccount()
        {
          AccountId = processAccount.AccountId,
          AccountNo = processAccount.AccountNo,
          AccountType = processAccount.AccountType,
          FirstName = processAccount.FirstName,
          IdNumber = processAccount.IdNumber,
          LastName = processAccount.LastName,
          PersonId = processAccount.PersonId,
          Process = processAccount.Process,
          ProcessEndDate = processAccount.ProcessEndDate,
          ProcessJobId = processAccount.ProcessJobId,
          ProcessStartDate = processAccount.ProcessStartDate,
          Status = processAccount.Status
        });
      }

      return processAccounts;
    }

    public static List<ProcessStepAccount> GetProcessSteps(long processJobId)
    {
      var processStepAccounts = new List<ProcessStepAccount>();

      var processAccounts = AccountProcess.GetProcessSteps(processJobId);
      foreach (var processAccount in processAccounts)
      {
        processStepAccounts.Add(new ProcessStepAccount()
        {
          Process = processAccount.Process,
          ProcessStep = processAccount.ProcessStep,
          ProcessStepEndDate = processAccount.ProcessStepEndDate,
          ProcessStepJobId = processAccount.ProcessStepJobId,
          ProcessStepStartDate = processAccount.ProcessStepStartDate
        });
      }

      return processStepAccounts;
    }

    public static void RedirectAccount(long processStepJobAccountId, long userId)
    {
      // get previous processStepId
      // Check if quotation is expired
      // loan is not cancelled, declined, or open
      using (var uow = new UnitOfWork())
      {
        var processStepJobAccount = new XPQuery<WFL_ProcessStepJobAccount>(uow).FirstOrDefault(p => p.ProcessStepJobAccountId == processStepJobAccountId);
        if (processStepJobAccount == null)
          throw new RecordNotFoundException(string.Format("Process Step Job Account {0} cannot be found in the DB", processStepJobAccountId));

        var prevProcessStep = new XPQuery<WFL_ProcessStepJob>(uow).Where(p => p.ProcessJob.ProcessJobId == processStepJobAccount.ProcessStepJob.ProcessJob.ProcessJobId
          && p.CreateDate <= processStepJobAccount.ProcessStepJob.CreateDate
          && !p.ProcessStep.IsParallelStep).OrderByDescending(p => p.CreateDate).Skip(1).FirstOrDefault();

        if (prevProcessStep == null)
          throw new Exception(string.Format("Account {0} does not have any previous workflow steps", processStepJobAccount.Account.AccountId));

        if (prevProcessStep.ProcessStep.ProcessStepId != (int)Workflow.ProcessStep.Processing)
          throw new Exception(string.Format("Account {0} has an invalid previous step", processStepJobAccount.Account.AccountId));

        if (!(processStepJobAccount.Account.Status.Type == Account.AccountStatus.Review
          || processStepJobAccount.Account.Status.Type == Account.AccountStatus.Technical_Error))
          throw new Exception(string.Format("Account {0} is not in the appropriate status to redirect", processStepJobAccount.Account.AccountId));

        var prevAccountStatus = new XPQuery<ACC_AccountStatus>(uow).Where(a => a.Account.AccountId == processStepJobAccount.Account.AccountId).OrderByDescending(a => a.CreateDate).Skip(1).FirstOrDefault();
        if (prevAccountStatus == null)
          throw new Exception(string.Format("Account {0} does not have more than one status", processStepJobAccount.Account.AccountId));

        if (prevAccountStatus.Status.Type != Account.AccountStatus.Inactive)
          throw new Exception(string.Format("Account {0} prev status was not Inactive", processStepJobAccount.Account.AccountId));

        var quotation = new XPQuery<ACC_Quotation>(uow).Where(a => a.Account.AccountId == processStepJobAccount.Account.AccountId
          && (a.QuotationStatus.Type != Account.QuotationStatus.Expired
          || a.QuotationStatus.Type != Account.QuotationStatus.Rejected)
          && a.ExpiryDate <= DateTime.Today).ToList();
        if (quotation.Count == 0)
          throw new Exception(string.Format("Account {0} does not have any valid quotation linked", processStepJobAccount.Account.AccountId));

        //new Default().SetAccountStatus(processStepJobAccount.Account, Account.AccountStatus.Inactive, null, null);

        //Atlas.Workflow.AtlasOnline.Default.RedirectAccount(processStepJobAccountId, prevProcessStep.ProcessStep.ProcessStepId, userId);
      }
    }
  }
}
