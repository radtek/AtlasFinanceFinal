using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Business.AVS
{
  public class Default
  {
    
    /// <summary>
    /// Switches the Bank to a new service. Eg. move FNB AVS's from HYPHEN to ABSA B2C, or vice versa
    /// </summary>
    /// <param name="serviceScheduleBankId">the schedule to switch</param>
    /// <param name="newServiceId">the service to switch to</param>
    /// <param name="hostId">for a specific host</param>
    public void SwitchBankServiceSchedule(int serviceScheduleBankId, int newServiceId, int hostId)
    {
      using (var uow = new UnitOfWork())
      {
        var serviceScheduleBank = new XPQuery<AVS_ServiceScheduleBank>(uow).FirstOrDefault(s => s.ServiceScheduleBankId == serviceScheduleBankId);

        var newServiceSchedule = new XPQuery<AVS_ServiceSchedule>(uow).FirstOrDefault(s => s.Service.ServiceId == newServiceId && s.Host.HostId == hostId);

        serviceScheduleBank.ServiceSchedule = newServiceSchedule;

        uow.CommitChanges();
      }
    }

    /// <summary>
    /// Queues a new transaction in the DB to be sent
    /// </summary>
    /// <param name="hostId">which system it is from</param>
    /// <param name="bankId">bank of the bank detail</param>
    /// <param name="initials">initals of the person</param>
    /// <param name="lastName">last name of the person</param>
    /// <param name="idNumber">id number of the person</param>
    /// <param name="branchCode">branch code of the bank detail</param>
    /// <param name="accountNo">account no of the bank detail</param>
    /// <param name="serviceId">which service does the check need to be done with</param>
    /// <param name="createUserId">user thats requesting the check</param>
    /// <param name="accountId">acc_account that the transaction is linked to</param>
    /// <param name="personId">per_person that the transaction is linked to</param>
    /// <param name="bankAccountPeriodId">period at which the account waas open according to the per_person</param>
    /// <param name="companyId">branch that the transaction belongs to</param>
    /// <returns>Id of the new transaction</returns>
    public long AddTransaction(Atlas.Enumerators.General.Host host, Atlas.Enumerators.General.BankName bank, string initials, string lastName, string idNumber, string branchCode, string accountNo,
      ref int? serviceId, int? createUserId, long? accountId, long? personId, Atlas.Enumerators.General.BankPeriod? bankAccountPeriod, long? companyId)
    {
      long transactionId = 0;
      using (var uow = new UnitOfWork())
      {
        var transaction = new AVS_Transaction(uow);
        if (serviceId != null)
        {
          var tempServiceId = serviceId;
          transaction.Service = new XPQuery<AVS_Service>(uow).FirstOrDefault(s => s.ServiceId == tempServiceId);
        }
        else
        {
          transaction.Service = GetRelevantService(uow, host, bank);

          serviceId = transaction.Service.ServiceId;
        }

        transaction.Host = new XPQuery<Host>(uow).FirstOrDefault(h => h.Type == host);
        transaction.Bank = new XPQuery<BNK_Bank>(uow).FirstOrDefault(b => b.Type== bank);
        transaction.Status = new XPQuery<AVS_Status>(uow).FirstOrDefault(s => s.StatusId == (int)Enumerators.AVS.Status.Queued);
        transaction.LastStatusDate = DateTime.Now;
        transaction.Initials = initials;
        transaction.LastName = lastName;
        transaction.IdNumber = idNumber;
        transaction.BranchCode = branchCode;
        transaction.Enabled = true;
        transaction.AccountNo = accountNo;
        transaction.CreateDate = DateTime.Now;
        if (companyId != null)
          transaction.Company = new XPQuery<CPY_Company>(uow).FirstOrDefault(c => c.CompanyId == companyId);
        if (createUserId != null)
          transaction.CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == createUserId);
        if (accountId != null)
          transaction.Account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (personId != null)
          transaction.Person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);

        if (bankAccountPeriod != null)
        {
          var bankAccountPeriodId = (int)bankAccountPeriod;
          transaction.BankAccountPeriod = new XPQuery<AVS_BankAccountPeriod>(uow).FirstOrDefault(b => b.BankAccountPeriodId == bankAccountPeriodId);
        }

        uow.CommitChanges();

        //if (accountId != null)
        //  Atlas.Workflow.AtlasOnline.Default.StartParallelProcessStep(Enumerators.Workflow.WorkflowProcess.AtlasOnline, (long)accountId, (int)Enumerators.Workflow.ProcessStep.AVS, (long)Enumerators.General.Person.System);

        transactionId = transaction.TransactionId;
      }

      return transactionId;
    }

    /// <summary>
    /// Resends transactions to bankserve for verification. This will add the current TransactionId in the TransactionRef field of the new transaction
    /// If the TransactionsRef is populated on the current transaction, then it will be used instead of the TransactionId
    /// eg.: newTransaction.TransactionRef = currentTransaction.TransactionRef ?? currentTransaction.TransactionId
    /// </summary>
    /// <param name="transactionIds"></param>
    /// <returns>list of the new resent transactions</returns>
    public List<AVS_TransactionDTO> ResendTransaction(List<long> transactionIds, bool keepService = false)
    {
      using (var uow = new UnitOfWork())
      {
        var oldTransactions = new XPQuery<AVS_Transaction>(uow).Where(t => transactionIds.Contains(t.TransactionId)).ToList();

        var transactions = new List<AVS_Transaction>();

        oldTransactions.ForEach((t) =>
        {
          var transaction = new AVS_Transaction(uow);
          transaction.TransactionRef = t.TransactionRef ?? t.TransactionId;
          transaction.Status = new XPQuery<AVS_Status>(uow).FirstOrDefault(s => s.StatusId == (int)Enumerators.AVS.Status.Queued);
          transaction.LastStatusDate = DateTime.Now;
          transaction.Host = t.Host;
          transaction.Bank = t.Bank;
          transaction.Initials = t.Initials;
          transaction.LastName = t.LastName;
          transaction.IdNumber = t.IdNumber;
          transaction.BranchCode = t.BranchCode;
          transaction.Enabled = true;
          transaction.AccountNo = t.AccountNo;
          transaction.CreateDate = DateTime.Now;
          transaction.Company = t.Company;
          transaction.CreateUser = t.CreateUser;
          transaction.Account = t.Account;
          transaction.Person = t.Person;
          transaction.BankAccountPeriod = t.BankAccountPeriod;
          transaction.Service = keepService ? t.Service : GetRelevantService(uow, t.Host.Type, t.Bank.Type);

          transactions.Add(transaction);
        });

        uow.CommitChanges();

        return AutoMapper.Mapper.Map<List<AVS_Transaction>, List<AVS_TransactionDTO>>(transactions);
      }
    }

    /// <summary>
    /// Cancels Queued Transactions (Queued: not yet batched or sent to the bank)
    /// </summary>
    /// <param name="transactionIds"></param>
    /// <returns>Tuple.Item1 = List of all the transactions to cancel | Tuple.Item2 = List of all the transactions not able to cancel</returns>
    public Tuple<List<AVS_TransactionDTO>, List<AVS_TransactionDTO>> CancelTransaction(long[] transactionIds)
    {
      using (var uow = new UnitOfWork())
      {
        var transactionsNotCancelled = new List<AVS_TransactionDTO>();
        var transactionsToCancel = new XPQuery<AVS_Transaction>(uow).Where(t => transactionIds.Contains(t.TransactionId)).ToList();
        transactionsToCancel.ForEach((t) =>
        {
          if (t.Enabled && t.Batch == null)
          {
            if (t.Status.StatusId != (int)Enumerators.AVS.Status.Complete)
            {
              t.Status = new XPQuery<AVS_Status>(uow).FirstOrDefault(s => s.StatusId == (int)Enumerators.AVS.Status.Cancelled);
              t.LastStatusDate = DateTime.Now;
            }
            t.Enabled = false;
          }
          else
          {
            transactionsNotCancelled.Add(AutoMapper.Mapper.Map<AVS_Transaction, AVS_TransactionDTO>(t));
          }
        });

        uow.CommitChanges();

        return new Tuple<List<AVS_TransactionDTO>, List<AVS_TransactionDTO>>(AutoMapper.Mapper.Map<List<AVS_Transaction>, List<AVS_TransactionDTO>>(transactionsToCancel), transactionsNotCancelled);
      }
    }

    /// <summary>
    /// Get Transactions that are pending (sent to bank - waiting for response)
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    public List<AVS_TransactionDTO> GetPending(Enumerators.AVS.Service service)
    {
      var results = new List<AVS_TransactionDTO>();

      using (var uow = new UnitOfWork())
      {
        var serviceId = (int)service;
        var transactions = new XPQuery<AVS_Transaction>(uow).Where(t => t.Enabled
          && t.Service.ServiceId == serviceId
          && t.Status.StatusId == (int)Enumerators.AVS.Status.Pending).ToList();

        results = AutoMapper.Mapper.Map<List<AVS_Transaction>, List<AVS_TransactionDTO>>(transactions);
      }

      return results;
    }

    /// <summary>
    ///  Get transaction By Id
    /// </summary>
    /// <param name="transactionId">Transaction to return from datastore.</param>
    public AVS_TransactionDTO GetTransaction(long transactionId)
    {
      using (var uow = new UnitOfWork())
      {
        var transaction = new XPQuery<AVS_Transaction>(uow).FirstOrDefault(t => t.TransactionId == transactionId);

        if (transaction == null)
          return null;

        return AutoMapper.Mapper.Map<AVS_Transaction, AVS_TransactionDTO>(transaction);
      }
    }

    /// <summary>
    ///  Get transaction By thirdpartyreference
    /// </summary>
    /// <param name="transactionId">Transaction to return from datastore.</param>
    public AVS_TransactionDTO GetTransactionByThirdPartyRef(string thirdPartyRef)
    {
      using (var uow = new UnitOfWork())
      {
        var transaction = new XPQuery<AVS_Transaction>(uow).FirstOrDefault(t => t.ThirdPartyRef == thirdPartyRef);

        if (transaction == null)
          return null;

        return AutoMapper.Mapper.Map<AVS_Transaction, AVS_TransactionDTO>(transaction);
      }
    }

    /// <summary>
    /// Get Transaction By Ref or Id
    /// </summary>
    /// <param name="transactionId"></param>
    /// <returns></returns>
    public AVS_TransactionDTO GetTransactionRef(long transactionId)
    {
      using (var uow = new UnitOfWork())
      {
        var transaction = new XPQuery<AVS_Transaction>(uow).Where(t => t.TransactionRef == transactionId && t.Enabled).OrderBy(t => t.CreateDate).FirstOrDefault();

        if (transaction == null)
          transaction = new XPQuery<AVS_Transaction>(uow).FirstOrDefault(t => t.TransactionId == transactionId);

        if (transaction == null)
          return null;

        return AutoMapper.Mapper.Map<AVS_Transaction, AVS_TransactionDTO>(transaction);
      }
    }

    /// <summary>
    /// Retrun all responsecodes from DB
    /// </summary>
    /// <returns></returns>
    public List<AVS_ResponseCodeDTO> GetResponseCodes()
    {
      using (var uow = new UnitOfWork())
      {
        var responseCodes = new XPQuery<AVS_ResponseCode>(uow).ToList();
        return AutoMapper.Mapper.Map<List<AVS_ResponseCode>, List<AVS_ResponseCodeDTO>>(responseCodes);
      }
    }

    /// <summary>
    /// Get all batchIds that contain the specified transactionIds
    /// </summary>
    /// <param name="transactionIds"></param>
    /// <returns></returns>
    public long[] GetBatchIdsForTransactions(long[] transactionIds)
    {
      using (var uow = new UnitOfWork())
      {
        return new XPQuery<AVS_Transaction>(uow).Where(t => transactionIds.Contains(t.TransactionId)).Select(t => t.Batch.BatchId).Distinct().ToArray();
      }
    }

    /// <summary>
    /// Updates a specific service's transmission no, generation no, and whether the service needs to await an accepted reply or not when a new avs transaction is queued
    /// </summary>
    /// <param name="serviceId">specific service</param>
    /// <param name="newNextTransmissionNo">next transmission no bankserv is expecting</param>
    /// <param name="newNextGenerationNo">next generation no bankserv is expecting</param>
    /// <param name="awaitReply">whether the service needs to await an accepted reply or not when a new avs transaction is queued</param>
    public void UpdateServiceSequences(int serviceId, int newNextTransmissionNo, int newNextGenerationNo, bool awaitReply)
    {
      using (var uow = new UnitOfWork())
      {
        var service = new XPQuery<AVS_Service>(uow).FirstOrDefault(s => s.ServiceId == serviceId);
        service.NextTransmissionNo = newNextTransmissionNo;
        service.NextGenerationNo = newNextGenerationNo;
        service.AwaitReply = awaitReply;

        uow.CommitChanges();
      }
    }

    /// <summary>
    /// Updates the batch to a submitted state
    /// </summary>
    /// <param name="batchId">the specified batch</param>
    public void UpdateBatch_Submitted(long batchId)
    {
      using (var uow = new UnitOfWork())
      {
        var batch = new XPQuery<AVS_Batch>(uow).FirstOrDefault(b => b.BatchId == batchId);
        batch.SubmitDate = DateTime.Now;

        uow.CommitChanges();
      }
    }

   
    /// <summary>
    /// returns transactions for a specified array of id's
    /// </summary>
    /// <param name="transactionIds">ids of transactions to return</param>
    /// <returns>a list of requested transactions</returns>
    public List<AVS_TransactionDTO> GetTransactionsByIds(long[] transactionIds)
    {
      using (var uow = new UnitOfWork())
      {
        var transactions = new XPQuery<AVS_Transaction>(uow).Where(t => transactionIds.Contains(t.TransactionId)).ToList();
        return AutoMapper.Mapper.Map<List<AVS_Transaction>, List<AVS_TransactionDTO>>(transactions);
      }
    }

    /// <summary>
    /// Gets a service linked to a bank and host
    /// </summary>
    /// <param name="uow">XPO DB session</param>
    /// <param name="hostId">specified host that an avs transtion belongs to</param>
    /// <param name="bankId">specified bank that avs transaction needs to be verified against</param>
    /// <returns>the matched service</returns>
    private AVS_Service GetRelevantService(UnitOfWork uow, Atlas.Enumerators.General.Host host, Atlas.Enumerators.General.BankName bank)
    {

      // Get Possible service that this AVS can go through 
      var possibleServices = new XPQuery<AVS_ServiceScheduleBank>(uow).Where(s => s.ServiceSchedule.Host.Type == host && s.Bank.Type == bank
        && s.ServiceSchedule.Service.Enabled).OrderBy(s => s.ServiceSchedule.Service.AwaitReply == false);

      // Choose service thats currently open
      foreach (var service in possibleServices)
      {
        if (service.ServiceSchedule.OpenTime.Value.TimeOfDay <= DateTime.Now.TimeOfDay
          && service.ServiceSchedule.CloseTime.Value.TimeOfDay > DateTime.Now.AddMinutes(1).TimeOfDay)
        {
          return service.ServiceSchedule.Service;
        }
      }

      // if there isnt any open service, choose the best match base on await reply
      return possibleServices.FirstOrDefault().ServiceSchedule.Service;
    }
  }
}