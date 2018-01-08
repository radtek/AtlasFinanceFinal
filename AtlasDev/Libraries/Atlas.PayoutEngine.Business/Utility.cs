using Atlas.Common.Extensions;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.LoanEngine.Account;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlas.PayoutEngine.Business
{
  public class Utility
  {
    private int ServiceId { get; set; }
    private readonly List<DateTime> _publicHolidays;

    public Utility(int serviceId, List<DateTime> publicHolidays = null)
    {
      ServiceId = serviceId;

      if (publicHolidays == null)
        publicHolidays = new List<DateTime>();
      _publicHolidays = publicHolidays;
    }

    public Dictionary<ACC_AccountDTO, ACC_AccountPayRuleDTO> GetAccountsWithPayRules(long[] payoutIds)
    {
      var accountPayrules = new Dictionary<ACC_AccountDTO, ACC_AccountPayRuleDTO>();
      using (var uow = new UnitOfWork())
      {
        var accounts = new XPQuery<PYT_Payout>(uow).Where(p => payoutIds.Contains(p.PayoutId) && p.Account != null).Select(p => p.Account).ToList();
        var accountIds = accounts.Select(a => a.AccountId).ToList();
        var payRules = new XPQuery<ACC_AccountPayRule>(uow).Where(p => accountIds.Contains(p.Account.AccountId) && p.Enabled).ToList();
        foreach (var account in accounts)
        {
          var payrule = payRules.FirstOrDefault(p => p.Account == account);
          accountPayrules.Add(AutoMapper.Mapper.Map<ACC_Account, ACC_AccountDTO>(account), AutoMapper.Mapper.Map<ACC_AccountPayRule, ACC_AccountPayRuleDTO>(payrule));
        }
      }

      return accountPayrules;
    }

    public void PayNewPayouts(List<long> banks)
    {
      BatchAndExportPayouts(banks);
    }

    public void ImportAndValidateNewPayouts(List<PYT_ServiceScheduleBankDTO> scheduledBanks)
    {
      ImportNewPayout(scheduledBanks);

      CancelRemoveDeadPayouts();

      ValidatePendingPayouts();
    }

    private void ImportNewPayout(List<PYT_ServiceScheduleBankDTO> scheduledBanks)
    {
      using (var UoW = new UnitOfWork())
      {
        var linkedHostId = new XPQuery<PYT_HostService>(UoW).Where(h => h.Service.ServiceId == ServiceId).Select(h => h.Host.HostId).ToArray();

        var accountsToImport = (from a in UoW.Query<ACC_Account>()
                                where
                                a.Status.Type == Enumerators.Account.AccountStatus.Approved
                                && a.ProcessStepJobAccounts.Any(ps => ps.ProcessStepJob.ProcessStep.ProcessStepId == (int)Enumerators.Workflow.ProcessStep.Payout
                                  && ps.ProcessStepJob.CompleteDate == null
                                  && (ps.ProcessStepJob.JobState.Type != Enumerators.Workflow.JobState.Completed
                                    && ps.ProcessStepJob.JobState.Type != Enumerators.Workflow.JobState.Failed
                                    && ps.ProcessStepJob.JobState.Type != Enumerators.Workflow.JobState.Stopped
                                    && ps.ProcessStepJob.JobState.Type != Enumerators.Workflow.JobState.Faulty))
                                  && (
                                !a.Payouts.Any(subPA =>
                                  subPA.PayoutStatus.Status == Enumerators.Payout.PayoutStatus.New
                                    || subPA.PayoutStatus.Status == Enumerators.Payout.PayoutStatus.Batched
                                    || subPA.PayoutStatus.Status == Enumerators.Payout.PayoutStatus.Failed
                                    || subPA.PayoutStatus.Status == Enumerators.Payout.PayoutStatus.OnHold
                                    || subPA.PayoutStatus.Status == Enumerators.Payout.PayoutStatus.Submitted
                                    || subPA.PayoutStatus.Status == Enumerators.Payout.PayoutStatus.Successful
                                    ))
                                && a.PayoutAmount > 0
                                && linkedHostId.Contains(a.Host.HostId)
                                select a).ToList();

        // Stop Import Process if there isnt anything to Import
        if (accountsToImport.Count() == 0)
          return;

        var hosts = accountsToImport.Select(a => a.Host).ToList();
        var newPayoutStatus = new XPQuery<PYT_PayoutStatus>(UoW).FirstOrDefault(p => p.Status == Enumerators.Payout.PayoutStatus.New);
        var service = new XPQuery<PYT_Service>(UoW).FirstOrDefault(s => s.ServiceId == ServiceId);
        foreach (var account in accountsToImport)
        {
          var perBankDetail = new XPQuery<PER_BankDetails>(UoW).FirstOrDefault(b=>b.Person.PersonId == account.Person.PersonId && b.BankDetail !=null && b.BankDetail.IsActive);
	        if (perBankDetail != null)
	        {
		        var bankDetail = perBankDetail.BankDetail;

						if (bankDetail != null)
		        {
			        var Payout = new PYT_Payout(UoW)
			        {
				        Service = service,
				        Account = account,
				        Person = account.Person,
				        Amount = account.PayoutAmount,
				        ActionDate = GetNextActionDate(scheduledBanks.FirstOrDefault(b => b.Bank.BankId == bankDetail.Bank.BankId)),
				        PayoutStatus = newPayoutStatus,
				        BankDetail = bankDetail,
				        LastStatusDate = DateTime.Now,
				        IsValid = true,
				        CreateDate = DateTime.Now
			        };
		        }
	        }
        }

        UoW.CommitChanges();
      }
    }

    private void ValidatePendingPayouts()
    {
      using (var uow = new UnitOfWork())
      {
        var validations = new XPQuery<PYT_Validation>(uow).ToList();

        var pendingPayout = new XPQuery<PYT_Payout>(uow).Where(p => p.PayoutStatus.Status == Enumerators.Payout.PayoutStatus.New
          && p.Service.ServiceId == ServiceId).ToList();

        var pendingPayoutIds = pendingPayout.Select(p => p.PayoutId).ToArray();

        // Clear validations for pendingpayouts
        var payoutValidations = new XPQuery<PYT_PayoutValidation>(uow).Where(v => pendingPayoutIds.Contains(v.Payout.PayoutId)).ToList();
        payoutValidations.ForEach(p =>
          {
            uow.Delete(p);
          });

        uow.PurgeDeletedObjects();

        pendingPayout.ForEach(p =>
          {
            p.IsValid = true;
          });

        var payoutAmount = new XPQuery<PYT_Payout>(uow).Where(p => pendingPayoutIds.Contains(p.PayoutId)
                            && p.Amount == p.Account.PayoutAmount
                            && (p.Account.LoanAmount + p.Account.ThirdPartyPayoutAmount) == p.Account.PayoutAmount).ToList();

        payoutAmount.ForEach(p =>
          {
            var payoutValidation = new PYT_PayoutValidation(uow)
            {
              Payout = p,
              Validation = validations.FirstOrDefault(v => v.Type == Enumerators.Payout.Validation.PayoutAmountMismatch)
            };
            p.IsValid = false;
          });

        var payoutActionDate = new XPQuery<PYT_Payout>(uow).Where(p => pendingPayoutIds.Contains(p.PayoutId) && p.ActionDate.Date < DateTime.Today.Date).ToList();

        payoutActionDate.ForEach(p =>
        {
          var payoutValidation = new PYT_PayoutValidation(uow)
          {
            Payout = p,
            Validation = validations.FirstOrDefault(v => v.Type == Enumerators.Payout.Validation.ActionDatePast)
          };
          p.IsValid = false;
        });

        var payoutBankDetails = new List<PYT_Payout>();
        foreach (var payout in pendingPayout)
        {
          AVS_Transaction passedAVS = null;

					var perBankDetail = new XPQuery<PER_BankDetails>(uow).FirstOrDefault(b => b.Person.PersonId == payout.Person.PersonId && b.BankDetail != null && b.BankDetail.IsActive);
	        if (perBankDetail != null)
	        {
		        var activeBankDetail = perBankDetail.BankDetail;

		        if (payout.BankDetail == activeBankDetail)
		        {
			        passedAVS = (from a in uow.Query<AVS_Transaction>()
				        where a.Person == payout.Person
				              && a.IdNumber == payout.Person.IdNum
				              && a.AccountNo == activeBankDetail.AccountNum
				              && a.Bank == activeBankDetail.Bank
				              && a.BranchCode == activeBankDetail.Bank.UniversalCode
				              && (a.Result.Type == Enumerators.AVS.Result.Passed
				                  || a.Result.Type == Enumerators.AVS.Result.PassedWithWarnings)
				        orderby a.CreateDate descending
				        select a).FirstOrDefault();
		        }
	        }

	        if (passedAVS == null)
          {
            payoutBankDetails.Add(payout);
          }
        }

        payoutBankDetails.ForEach(p =>
        {
          var payoutValidation = new PYT_PayoutValidation(uow)
          {
            Payout = p,
            Validation = validations.FirstOrDefault(v => v.Type == Enumerators.Payout.Validation.AVSNotSuccessful)
          };
          p.IsValid = false;
        });

        uow.CommitChanges();
      }
    }

    private void CancelRemoveDeadPayouts()
    {
      using (var uow = new UnitOfWork())
      {
        var payouts = (from p in uow.Query<PYT_Payout>()
                       where (p.Account.Status.Type != Enumerators.Account.AccountStatus.Approved
                          || p.Account.ProcessStepJobAccounts.Any(psa => psa.ProcessStepJob.ProcessStep.ProcessStepId == (int)Enumerators.Workflow.ProcessStep.Payout
                            && psa.ProcessStepJob.CompleteDate != null
                              && (psa.ProcessStepJob.JobState.Type != Enumerators.Workflow.JobState.Completed
                              && psa.ProcessStepJob.JobState.Type != Enumerators.Workflow.JobState.Failed
                              && psa.ProcessStepJob.JobState.Type != Enumerators.Workflow.JobState.Stopped))
                         || p.Account.PayoutAmount != p.Amount)
                       && (p.PayoutStatus.Status == Enumerators.Payout.PayoutStatus.New
                         || p.PayoutStatus.Status == Enumerators.Payout.PayoutStatus.OnHold)
                       select p).ToList();

        if (payouts.Count > 0)
        {
          payouts.ForEach(p =>
            {
              p.PayoutStatus = new XPQuery<PYT_PayoutStatus>(uow).FirstOrDefault(s => s.Status == Enumerators.Payout.PayoutStatus.Removed);
            });

          uow.CommitChanges();
        }
      }
    }

    private void BatchAndExportPayouts(List<long> banks)
    {
      long batchId = 0;

      using (var uow = new UnitOfWork())
      {
        var pendingPayout = (from p in uow.Query<PYT_Payout>()
                             where p.PayoutStatus.Status == Enumerators.Payout.PayoutStatus.New
                               && p.IsValid
                               && p.Service.ServiceId == ServiceId
                               && banks.Contains(p.BankDetail.Bank.BankId)
                             select p).ToList();

        if (pendingPayout.Count > 0)
        {

          var batch = new PYT_Batch(uow)
          {
            BatchStatus = new XPQuery<PYT_BatchStatus>(uow).FirstOrDefault(b => b.Status == Enumerators.Payout.PayoutBatchStatus.New),
            LastStatusDate = DateTime.Now,
            CreateDate = DateTime.Now,
            CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(s => s.PersonId == (int)Enumerators.General.Person.System),
            Service = pendingPayout.FirstOrDefault().Service
          };

          pendingPayout.ForEach(p =>
            {
              p.Batch = batch;
              p.PayoutStatus = new XPQuery<PYT_PayoutStatus>(uow).FirstOrDefault(ps => ps.Status == Enumerators.Payout.PayoutStatus.Batched);
            });

          uow.CommitChanges();

          batchId = batch.BatchId;
        }
      }

      if (batchId > 0)
        PrepareExport(batchId);
    }

    private void PrepareExport(long batchId)
    {
      var nextTransmissionNo = GetNextTransmissionNo();
      var nextGenerationNo = GetNextGenerationNo();
      var nextSequenceNo = GetNextSequenceNo();

      PER_PersonDTO submittedUser = new PER_PersonDTO();

      using (var uow = new UnitOfWork())
      {
        var transmission = new PYT_Transmission(uow)
        {
          Batch = new XPQuery<PYT_Batch>(uow).FirstOrDefault(b => b.BatchId == batchId),
          CreateDate = DateTime.Now,
          TransmissionNo = nextTransmissionNo
        };

        var transmissionSet = new PYT_TransmissionSet(uow)
        {
          Transmission = transmission,
          GenerationNo = nextGenerationNo
        };

        var batchPayouts = new XPQuery<PYT_Payout>(uow).Where(p => p.Batch.BatchId == batchId).ToList();

        for (var i = 0; i < batchPayouts.Count; i++)
        {
          var payout = batchPayouts[i];
          var transmissionTransaction = new PYT_TransmissionTransaction(uow)
          {
            TransmissionSet = transmissionSet,
            Batch = new XPQuery<PYT_Batch>(uow).FirstOrDefault(b => b.BatchId == batchId),
            Payout = payout,
            SequenceNo = nextSequenceNo + i
          };
        }

        uow.CommitChanges();

        // Add transaction and address to dictionary to parse to export function
        Dictionary<PYT_TransmissionTransactionDTO, AddressDTO> transmissionTransaction_Address = new Dictionary<PYT_TransmissionTransactionDTO, AddressDTO>();
        (new XPQuery<PYT_TransmissionTransaction>(uow).Where(t => t.TransmissionSet == transmissionSet).ToList()).ForEach(t =>
          {
						var perAddress = new XPQuery<PER_AddressDetails>(uow).FirstOrDefault(a=>a.Address.IsActive&& a.Person.PersonId== t.Payout.Person.PersonId);
	          if (perAddress != null)
	          {
		          transmissionTransaction_Address.Add(
			          AutoMapper.Mapper.Map<PYT_TransmissionTransaction, PYT_TransmissionTransactionDTO>(t),
			          AutoMapper.Mapper.Map<ADR_Address, AddressDTO>(perAddress.Address));
	          }
          });

        if (transmission.Batch.Service.ServiceType.Type == Enumerators.Payout.PayoutServiceType.RTC)
        {
          RTC.Helper.Export(AutoMapper.Mapper.Map<PYT_Batch, PYT_BatchDTO>(transmission.Batch),
            AutoMapper.Mapper.Map<PYT_Transmission, PYT_TransmissionDTO>(transmission),
            AutoMapper.Mapper.Map<PYT_TransmissionSet, PYT_TransmissionSetDTO>(transmissionSet),
            transmissionTransaction_Address);
        }
        else
        {
          // PAAF Exporting function
        }

        // Update batch and payouts to submitted
        transmission.Batch.LastStatusDate = DateTime.Now;
        transmission.Batch.SubmitDate = DateTime.Now;
        transmission.Batch.SubmitUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)Enumerators.General.Person.System);
        transmission.Batch.BatchStatus = new XPQuery<PYT_BatchStatus>(uow).FirstOrDefault(b => b.Status == Enumerators.Payout.PayoutBatchStatus.SubmittedWaitingOutbox);

        var payoutStatusSubmitted = new XPQuery<PYT_PayoutStatus>(uow).FirstOrDefault(p => p.Status == Enumerators.Payout.PayoutStatus.Submitted);

        transmission.Batch.Payouts.ToList().ForEach(p =>
          {
            p.PayoutStatus = payoutStatusSubmitted;
            p.LastStatusDate = DateTime.Now;
          });

        uow.CommitChanges();
      }
    }

    /// <summary>
    /// This should close the PYT_Batch record but also write to the GL transactions schema
    /// </summary>
    /// <param name="batchId"></param>
    public void CloseBatch(long batchId)
    {
      UpdateBatchStatus(batchId, Enumerators.Payout.PayoutBatchStatus.Closed);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="results">Key = BatchId, Value = successful payoutIds</param>
    public Dictionary<long, List<PYT_PayoutDTO>> ImportPayoutReplies(Dictionary<long, Tuple<string, string>> results)
    {
      var batchResults = new Dictionary<long, List<PYT_PayoutDTO>>();
      using (var uow = new UnitOfWork())
      {
        var payoutIds = results.Keys.ToArray();
        var payouts = new XPQuery<PYT_Payout>(uow).Where(p => payoutIds.Contains(p.PayoutId));
        var payoutStatus = new XPQuery<PYT_PayoutStatus>(uow).Where(p => p.Status == Enumerators.Payout.PayoutStatus.Successful
          || p.Status == Enumerators.Payout.PayoutStatus.Failed).ToList();

        foreach (var payout in payouts)
        {
          var result = results.First(r => r.Key == payout.PayoutId);

          payout.ResultCode = new XPQuery<PYT_ResultCode>(uow).FirstOrDefault(r => r.ResultCode == result.Value.Item1);
          payout.ResultQualifier = new XPQuery<PYT_ResultQualifier>(uow).FirstOrDefault(r => r.ResultQualifierCode == result.Value.Item2);

          if (payout.ResultCode != null && (payout.ResultCode.Type == Enumerators.Payout.ResultCode.RTCSuccessful
                                            || payout.ResultCode.Type == Enumerators.Payout.ResultCode.RTCRejected
                                            || payout.ResultQualifier == null))
          {
            payout.ResultMessage = result.Value.Item2;
          }

          if (payout.ResultCode != null && (payout.ResultCode.Type == Enumerators.Payout.ResultCode.Successful
                                            || payout.ResultCode.Type == Enumerators.Payout.ResultCode.RTCSuccessful))
          {
            // Successful Payment
            payout.PayoutStatus = payoutStatus.FirstOrDefault(p => p.Status == Enumerators.Payout.PayoutStatus.Successful);
            payout.Paid = true;
            payout.PaidDate = DateTime.Now;
          }
          else
          {
            // Unsuccessful..
            payout.Paid = false;
            payout.PayoutStatus = payoutStatus.FirstOrDefault(p => p.Status == Enumerators.Payout.PayoutStatus.Failed);
          }

          payout.LastStatusDate = DateTime.Now;
        }

        //batchResults = payouts.Select(p => p.Batch).Distinct().Select(b => b.BatchId).ToArray();

        foreach (var batch in payouts.Select(p => p.Batch).Distinct())
        {
          batchResults.Add(batch.BatchId, new List<PYT_PayoutDTO>());
          var outstandingPayouts = batch.Payouts.Count(p => p.Paid == null);
          if (outstandingPayouts > 0)
            continue;

          var allPayoutsPaid = batch.Payouts.Count(p => (p.Paid ?? false));
          if (allPayoutsPaid == batch.Payouts.Count)
          {
            batch.BatchStatus = new XPQuery<PYT_BatchStatus>(uow).FirstOrDefault(s => s.Status == Enumerators.Payout.PayoutBatchStatus.Completed);
          }
          else
          {
            batch.BatchStatus = new XPQuery<PYT_BatchStatus>(uow).FirstOrDefault(s => s.Status == Enumerators.Payout.PayoutBatchStatus.CompletedWithErrors);
          }

          batch.LastStatusDate = DateTime.Now;
        }

        uow.CommitChanges();

        // Open Accounts of successful payouts
        foreach (var payout in payouts.Where(p => p.PayoutStatus.Status == Enumerators.Payout.PayoutStatus.Successful))
        {
          batchResults[payout.Batch.BatchId].Add(AutoMapper.Mapper.Map<PYT_Payout, PYT_PayoutDTO>(payout));

          var account = new Default(payout.Account.AccountId);
          account.OpenAccount();

	        Workflow.AtlasOnline.Default.StepProcess(payout.Account.AccountId, (int)Enumerators.Workflow.ProcessStep.Payout);
        }
      }

      return batchResults;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="transmissionId"></param>
    /// <param name="accepted"></param>
    /// <param name="replyCode"></param>
    /// <param name="filePath"></param>
    /// <param name="updateAllRelationsIfRejected">If transmission Rejected, and we dont have any transactional details, which is possible, we wanna reject all sets, and transactions.
    /// Also cancel the payout so the service can re-import it and send it off with a new batch</param>
    public void UpdateTransmissionReply(long transmissionId, bool accepted, string replyCode, string filePath, bool updateAllRelationsIfRejected, bool updateTransmissionSet = false)
    {
      using (var uow = new UnitOfWork())
      {
        var transmission = new XPQuery<PYT_Transmission>(uow).FirstOrDefault(t => t.TransmissionId == transmissionId && t.Accepted == null);

        if (transmission == null)
          throw new Exception($"TransmissionId {transmissionId}, does not exist in DB");
				
        var replyCodeEnum = replyCode.FromStringToEnum<Enumerators.Payout.ReplyCode>();
        transmission.ReplyCode = new XPQuery<PYT_ReplyCode>(uow).FirstOrDefault(r => r.Type == replyCodeEnum && r.ServiceType == transmission.Batch.Service.ServiceType);
        transmission.ReplyDate = DateTime.Now;
        transmission.Accepted = accepted;
        transmission.FilePath = filePath;

        if (transmission.ReplyCode == null)
        {
          transmission.ReplyCode = new XPQuery<PYT_ReplyCode>(uow).FirstOrDefault(r => r.Type == (accepted ? Enumerators.Payout.ReplyCode.Unknown_Accepted : Enumerators.Payout.ReplyCode.Unknown_Rejected));
        }

        if (!accepted && updateAllRelationsIfRejected)
        {
          // transmission was rejected, the file needs to be resent
          // Cancel linked payouts
          // Close Batch
          foreach (var payout in transmission.Batch.Payouts)
          {
            payout.Paid = false;
            payout.PayoutStatus = new XPQuery<PYT_PayoutStatus>(uow).FirstOrDefault(s => s.Status == Enumerators.Payout.PayoutStatus.Cancelled);
            payout.LastStatusDate = DateTime.Now;
          }

          transmission.Batch.BatchStatus = new XPQuery<PYT_BatchStatus>(uow).FirstOrDefault(s => s.Status == Enumerators.Payout.PayoutBatchStatus.Closed);
          transmission.Batch.LastStatusDate = DateTime.Now;

          foreach (var transmissionSet in transmission.TransmissionSets)
          {
            transmissionSet.Accepted = accepted;
            transmissionSet.ReplyCode = transmission.ReplyCode;
            transmissionSet.ReplyDate = DateTime.Now;

            foreach (var transmissionTransaction in transmissionSet.TransmissionTransactions)
            {
              transmissionTransaction.Accepted = accepted;
              transmissionTransaction.ReplyCode = transmission.ReplyCode;
              transmissionTransaction.ReplyDate = DateTime.Now;
            }
          }
        }
        else if (updateTransmissionSet)
        {
          foreach (var transmissionSet in transmission.TransmissionSets)
          {
            transmissionSet.Accepted = accepted;
            transmissionSet.ReplyCode = transmission.ReplyCode;
            transmissionSet.ReplyDate = DateTime.Now;
          }
        }

        uow.CommitChanges();
      }
    }

    public void UpdateTransmissionSetReply(long transmissionSetId, bool accepted, string replyCode)
    {
      using (var uow = new UnitOfWork())
      {
        var transmissionSet = new XPQuery<PYT_TransmissionSet>(uow).FirstOrDefault(t => t.TransmissionSetId == transmissionSetId);

        if (transmissionSet == null)
          throw new Exception($"transmissionSetId {transmissionSetId}, does not exist in DB");

        transmissionSet.ReplyCode = new XPQuery<PYT_ReplyCode>(uow).FirstOrDefault(r => r.ReplyCodeType.Type.ToStringEnum() == replyCode);
        transmissionSet.ReplyDate = DateTime.Now;
        transmissionSet.Accepted = accepted;

        uow.CommitChanges();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="transmissionTransactionReplies">long, long = transmissionId, bool = accepted, string = ReplyCode</param>
    public void UpdateTransmissionTransactionReplies(Dictionary<long, Tuple<long, bool, string>> transmissionTransactionReplies)
    {
      using (var uow = new UnitOfWork())
      {
        var payoutIds = transmissionTransactionReplies.Select(p => p.Key).ToArray();
        var transmissionTransactions = new XPQuery<PYT_TransmissionTransaction>(uow).Where(t =>
          payoutIds.Contains(t.Payout.PayoutId)
          && t.Accepted == null);

        foreach (var transmissionTransaction in transmissionTransactions)
        {
          var transmissionTransactionReply = transmissionTransactionReplies[transmissionTransaction.Payout.PayoutId];

          var replyCodeEnum = transmissionTransactionReply.Item3.FromStringToEnum<Enumerators.Payout.ReplyCode>();
          transmissionTransaction.ReplyCode = new XPQuery<PYT_ReplyCode>(uow).FirstOrDefault(r => r.Type == replyCodeEnum);
          transmissionTransaction.ReplyDate = DateTime.Now;
          transmissionTransaction.Accepted = transmissionTransactionReply.Item2;
          uow.CommitChanges();
        }
      }
    }

    public void UpdateBatch_FilePickedUp(long batchId)
    {
      UpdateBatchStatus(batchId, Enumerators.Payout.PayoutBatchStatus.SubmittedWaitingForReply);
    }

    private void UpdateBatchStatus(long batchId, Enumerators.Payout.PayoutBatchStatus status)
    {
      using (var uow = new UnitOfWork())
      {
        var batch = new XPQuery<PYT_Batch>(uow).FirstOrDefault(b => b.BatchId == batchId);

        if (batch == null)
          throw new Exception($"BatchId {batchId}, does not exist in DB");

        batch.BatchStatus = new XPQuery<PYT_BatchStatus>(uow).First(s => s.Status == status);
        batch.LastStatusDate = DateTime.Now;

        uow.CommitChanges();
      }
    }

    private int GetNextTransmissionNo()
    {
      using (var uow = new UnitOfWork())
      {
        var transmission = new XPQuery<PYT_Transmission>(uow).Where(t => t.Batch.Service.ServiceId == ServiceId
          && (t.Accepted ?? false)).OrderByDescending(t => t.TransmissionNo).FirstOrDefault();

        if (transmission == null)
          return 1;
        else
          return (transmission.TransmissionNo + 1);
      }
    }

    private int GetNextGenerationNo()
    {
      using (var uow = new UnitOfWork())
      {
        var transmissionSet = new XPQuery<PYT_TransmissionSet>(uow).Where(t => t.Transmission.Batch.Service.ServiceId == ServiceId
          && (t.Transmission.Accepted ?? false)
          && (t.Accepted ?? false)).OrderByDescending(t => t.GenerationNo).FirstOrDefault();

        if (transmissionSet == null)
          return 1;
        else
          return (transmissionSet.GenerationNo + 1);
      }
    }

    private int GetNextSequenceNo()
    {
      using (var uow = new UnitOfWork())
      {
        var transmissionTransaction = new XPQuery<PYT_TransmissionTransaction>(uow).Where(t => t.TransmissionSet.Transmission.Batch.Service.ServiceId == ServiceId
          && (t.TransmissionSet.Transmission.Accepted ?? false)
          && (t.TransmissionSet.Accepted ?? false)
          && (t.Accepted ?? false)).OrderByDescending(t => t.SequenceNo).FirstOrDefault();

        if (transmissionTransaction == null)
          return 1;
        else
          return (transmissionTransaction.SequenceNo + 1);
      }
    }

    private DateTime GetNextActionDate(PYT_ServiceScheduleBankDTO scheduleBank)
    {
      if (scheduleBank.ServiceSchedule.Service.ServiceType.Type == Enumerators.Payout.PayoutServiceType.RTC)
      {
        var nextActionDate = DateTime.Today.Date;
        if (scheduleBank.ServiceSchedule.OpenTime.Value.TimeOfDay <= DateTime.Now.TimeOfDay
          && DateTime.Now.TimeOfDay <= scheduleBank.ServiceSchedule.CloseTime.Value.TimeOfDay
            && (nextActionDate.DayOfWeek != DayOfWeek.Saturday
              && nextActionDate.DayOfWeek != DayOfWeek.Sunday))
        {
          return nextActionDate;
        }
        else
        {
          do
          {
            nextActionDate = nextActionDate.AddDays(1).Date;
          } while (_publicHolidays.Contains(nextActionDate)
            && (nextActionDate.DayOfWeek == DayOfWeek.Saturday
              || nextActionDate.DayOfWeek == DayOfWeek.Sunday));

          return nextActionDate;
        }
      }
      else
      {
        return new DateTime();
      }
    }
  }
}