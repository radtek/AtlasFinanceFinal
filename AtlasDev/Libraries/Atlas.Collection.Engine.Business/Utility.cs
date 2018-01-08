using Atlas.Collection.Engine.Business.ExceptionBase;
using Atlas.Common.ExceptionBase;
using Atlas.Common.Extensions;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.LoanEngine;
using Atlas.RabbitMQ.Messages.DebitOrder;
using Atlas.ThirdParty.ABSA.NAEDO.FileStructures.Reply;
using DevExpress.Xpo;
using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Collection.Engine.Business
{
  public sealed class Utility : IDisposable
  {
    #region Private Properties

    private int _serviceId { get; set; }
    private List<DateTime> _publicHolidays;
    private const int READ_AHEAD_DAY = +1;
    private static ILogger _logger = null;

    #endregion

    #region Constructor

    public Utility(int serviceId, ILogger logger, List<DateTime> publicHolidays = null)
    {
      _serviceId = serviceId;
      _publicHolidays = publicHolidays;
      _logger = logger;
    }

    public Utility(ILogger logger)
    {
      _logger = logger;
      using (var uow = new UnitOfWork())
      {
        var publicHolidays = new XPQuery<PublicHoliday>(uow).ToList();
        _publicHolidays = new List<DateTime>();
        _publicHolidays.AddRange(publicHolidays.Select(d => d.Date));
      }
    }

    #endregion

    #region Public Methods


    /// <summary>
    /// Returns the tracking day
    /// </summary>
    /// <returns></returns>
    public static string GetTrackingDay(int trackingDays, bool isTransaction)
    {
      switch (trackingDays)
      {
        case (int)Debit.TrackingDay.No_Track:
          return isTransaction == false ? "NO TRACK  " : "12";
        case (int)Debit.TrackingDay.One_Day_Tracking:
          return isTransaction == false ? "01D TRACK " : "13";
        case (int)Debit.TrackingDay.Two_Day_Tracking:
          return isTransaction == false ? "02D TRACK " : "01";
        case (int)Debit.TrackingDay.Three_Day_Tracking:
          return isTransaction == false ? "03D TRACK " : "14";
        case (int)Debit.TrackingDay.Four_Day_Tracking:
          return isTransaction == false ? "04D TRACK " : "02";
        case (int)Debit.TrackingDay.Five_Day_Tracking:
          return isTransaction == false ? "05D TRACK " : "03";
        case (int)Debit.TrackingDay.Six_Day_Tracking:
          return isTransaction == false ? "06D TRACK " : "04";
        case (int)Debit.TrackingDay.Seven_Day_Tracking:
          return isTransaction == false ? "07D TRACK " : "15";
        case (int)Debit.TrackingDay.Eight_Day_Tracking:
          return isTransaction == false ? "08D TRACK " : "05";
        case (int)Debit.TrackingDay.Nine_Day_Tracking:
          return isTransaction == false ? "09D TRACK " : "06";
        case (int)Debit.TrackingDay.Ten_Day_Tracking:
          return isTransaction == false ? "10D TRACK " : "07";
        case (int)Debit.TrackingDay.Fourteen_Day_Tracking:
          return isTransaction == false ? "14D TRACK " : "16";
        case (int)Debit.TrackingDay.TwentyOne_Day_Tracking:
          return isTransaction == false ? "21D TRACK " : "17";
        case (int)Debit.TrackingDay.ThirtyTwo_Day_Tracking:
          return isTransaction == false ? "32D TRACK " : "18";
        case (int)Debit.TrackingDay.NAEDO_Recall:
          return "NADRCL    ";
        default:
          break;
      }

      return string.Empty;
    }

    public DBT_ControlDTO CreateNewControl(string thirdPartyReference, string bankStatementReference, Atlas.Enumerators.Debit.ControlType controlType, Atlas.Enumerators.Debit.AVSCheckType avsCheckType,
      Atlas.Enumerators.General.BankName bank, string bankAccountName, string bankAccountNo, Atlas.Enumerators.General.BankAccountType bankAccountType, string bankBranchCode, string idNumber,
      Atlas.Enumerators.Debit.FailureType failureType, DateTime firstInstalment, decimal instalment, Atlas.Enumerators.Account.PayRule payRule, Account.PeriodFrequency periodFrequency,
      int repititions, Atlas.Enumerators.Debit.TrackingDay trackingDays, Atlas.Enumerators.General.Host host, long? branchId, Atlas.Enumerators.Account.PayDateType payDateType,
      int payDateNo, long createUserId = (long)Atlas.Enumerators.General.Person.System, List<Tuple<int, decimal, DateTime, DateTime>> predefinedTransactions = null)
    {
      if (!IsValidActionDate(firstInstalment))
        throw new GeneralDebitOrderException("Invalid First Instalment Date - public holiday or sunday");

      using (var uow = new UnitOfWork())
      {
        var control = new XPQuery<DBT_Control>(uow).FirstOrDefault(c => c.BankStatementReference == bankStatementReference && c.ThirdPartyReference == thirdPartyReference && c.Host.Type == host
          && (c.ControlStatus.Type == Debit.ControlStatus.New || c.ControlStatus.Type == Debit.ControlStatus.InProcess));
        if (control != null)
        {
          var error = string.Format("Control with ThirdPartyReference: {0} and Contract Reference: {1} already exists for Host {2}", thirdPartyReference, bankStatementReference, host.ToStringEnum());
          _logger.Error(error);
          throw new RecordAlreadyExistsException(error);
        }

        control = new DBT_Control(uow);
        control.ControlType = new XPQuery<DBT_ControlType>(uow).First(a => a.Type == controlType);
        control.AVSCheckType = new XPQuery<DBT_AVSCheckType>(uow).First(a => a.Type == avsCheckType);
        control.BankStatementReference = bankStatementReference.ToUpper();
        control.ThirdPartyReference = thirdPartyReference;
        control.Host = new XPQuery<Host>(uow).First(h => h.Type == host);
        control.IdNumber = idNumber;
        control.Bank = new XPQuery<BNK_Bank>(uow).First(b => b.Type == bank);
        control.BankAccountName = bankAccountName.ToUpper();
        control.BankAccountNo = bankAccountNo;
        control.BankAccountType = new XPQuery<BNK_AccountType>(uow).First(a => a.Type == bankAccountType);
        control.BankBranchCode = bankBranchCode;
        control.ControlStatus = new XPQuery<DBT_ControlStatus>(uow).First(c => c.Type == Debit.ControlStatus.New);
        control.CreateDate = DateTime.Now;
        control.CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == createUserId);
        control.CurrentRepetition = 0;
        control.FailureType = new XPQuery<DBT_FailureType>(uow).First(f => f.Type == failureType);
        control.FirstInstalmentDate = firstInstalment;
        control.Instalment = instalment;
        control.IsValid = false;
        control.LastInstalmentUpdate = DateTime.Now;
        control.LastStatusDate = DateTime.Now;
        control.PayDate = new XPQuery<ACC_PayDate>(uow).First(p => p.PayDateType.Type == payDateType && p.DayNo == payDateNo);
        control.PayRule = new XPQuery<ACC_PayRule>(uow).First(p => p.Type == payRule);
        control.PeriodFrequency = new XPQuery<ACC_PeriodFrequency>(uow).First(p => p.Type == periodFrequency);
        control.Repetitions = repititions;
        var bankServiceIds = new XPQuery<DBT_ServiceScheduleBank>(uow).Where(s => s.Bank == control.Bank).Select(s => s.ServiceSchedule.Service.ServiceId).ToList();
        control.Service = new XPQuery<DBT_HostService>(uow).Where(h => h.Host.Type == host && h.Enabled && bankServiceIds.Contains(h.Service.ServiceId)).FirstOrDefault().Service;
        control.TrackingDays = (int)trackingDays;

        if (branchId.HasValue && branchId > 0)
          control.CompanyBranch = new XPQuery<CPY_Company>(uow).First(c => c.CompanyId == branchId);

        if (controlType == Debit.ControlType.Predefined)
        {
          foreach (var predefinedTransaction in predefinedTransactions)
          {
            var transaction = new DBT_Transaction(uow)
            {
              ActionDate = predefinedTransaction.Item4,
              Amount = predefinedTransaction.Item2,
              Control = control,
              CreateDate = DateTime.Now,
              DebitType = new XPQuery<DBT_DebitType>(uow).FirstOrDefault(s => s.Type == Debit.DebitType.Regular),
              InstalmentDate = predefinedTransaction.Item3,
              LastStatusDate = DateTime.Now,
              Repetition = predefinedTransaction.Item1,
              Status = new XPQuery<DBT_Status>(uow).FirstOrDefault(s => s.Type == Debit.Status.New)
            };
          }
        }

        uow.CommitChanges();

        return AutoMapper.Mapper.Map<DBT_Control, DBT_ControlDTO>(control);
      }
    }

    public void UpdateControlWithAVSTransaction(long controlId, long avsTransactionId)
    {
      _logger.Info("Utility.Update Control With AVS Transaction(): Started");

      using (var uow = new UnitOfWork())
      {
        var control = new XPQuery<DBT_Control>(uow).FirstOrDefault(c => c.ControlId == controlId);
        if (control == null)
          throw new RecordNotFoundException(string.Format("Update Control With AVS Transaction - ControlId: {0} not found in DB or cannot update control with AVS TransactionId {1}", controlId, avsTransactionId));

        control.AVSTransactionId = avsTransactionId;

        uow.CommitChanges();
      }

      _logger.Info("Utility.Update Control With AVS Transaction(): Completed");
    }

    public void ImportNewControl()
    {
      _logger.Info("Utility.Import New Control(): Started");
      using (var uow = new UnitOfWork())
      {
        // get new controls
        var newControls = new XPQuery<DBT_Control>(uow).Where(c => c.ControlStatus.Type == Debit.ControlStatus.New).ToList();

        var validControlIds = (ValidateOrder(AutoMapper.Mapper.Map<List<DBT_Control>, List<DBT_ControlDTO>>(newControls))).Select(c => c.ControlId).ToList();
        var validControls = new XPQuery<DBT_Control>(uow).Where(c => validControlIds.Contains(c.ControlId)).ToList();
        _logger.Info(string.Format("Utility.Import New Control(): {0} new Controls - {1} are valid", newControls.Count, validControls.Count));

        // loop through controls to start first debit order transaction
        foreach (var control in validControls)
        {
          var newTransaction = new DBT_Transaction(uow);
          newTransaction.CreateDate = DateTime.Now;
          newTransaction.Control = control;
          newTransaction.Repetition = ++control.CurrentRepetition;
          newTransaction.DebitType = new XPQuery<DBT_DebitType>(uow).FirstOrDefault(d => d.DebitTypeId == (int)Debit.DebitType.Regular);
          newTransaction.Status = new XPQuery<DBT_Status>(uow).FirstOrDefault(d => d.StatusId == (int)Debit.Status.New);
          newTransaction.LastStatusDate = DateTime.Now;
          newTransaction.Control.ControlStatus = new XPQuery<DBT_ControlStatus>(uow).FirstOrDefault(d => d.ControlStatusId == (int)Debit.ControlStatus.InProcess);
          newTransaction.Amount = 0; // Gets amount from control when actually submitting to bank
          newTransaction.ActionDate = newTransaction.InstalmentDate = control.FirstInstalmentDate;
          UpdateTransactionActionDate(ref newTransaction, true);

          control.ControlStatus = new XPQuery<DBT_ControlStatus>(uow).First(s => s.Type == Debit.ControlStatus.InProcess);
          control.LastStatusDate = DateTime.Now;
          //control.CurrentRepetition++;
        }

        uow.CommitChanges();
      }
      _logger.Info("Utility.Import New Control(): Completed");
    }

    public bool CancelControl(long controlId, bool cancelTransactions)
    {
      var cancelled = false;
      using (var uow = new UnitOfWork())
      {
        var control = new XPQuery<DBT_Control>(uow).FirstOrDefault(c => c.ControlId == controlId
          && (c.ControlStatus.Type == Debit.ControlStatus.InProcess || c.ControlStatus.Type == Debit.ControlStatus.New));
        if (control == null)
        {
          throw new RecordNotFoundException(string.Format("Cancel Control - ControlId: {0} not found in DB or cannot cancel control in this status", controlId));
        }
        else
        {
          var batchSubmittedTranscationsQuery = new XPQuery<DBT_Transaction>(uow).Where(t => t.Control == control &&
            (t.Status.Type == Debit.Status.Batched
            || t.Status.Type == Debit.Status.Submitted));
          if (!cancelTransactions)
            batchSubmittedTranscationsQuery = batchSubmittedTranscationsQuery.Where(t => t.Status.Type == Debit.Status.New);
          var batchSubmittedTranscations = batchSubmittedTranscationsQuery.ToList();
          if (batchSubmittedTranscations.Count() > 0)
            throw new GeneralDebitOrderException(string.Format("CancelControl - ControlId: {0} has transactions that are processing", controlId));

          control.LastStatusDate = DateTime.Now;
          control.ControlStatus = new XPQuery<DBT_ControlStatus>(uow).FirstOrDefault(c => c.Type == Debit.ControlStatus.Cancelled);

          var transactionsToCancel = new XPQuery<DBT_Transaction>(uow).Where(t => t.Control == control && (t.Status.Type == Debit.Status.New || t.Status.Type == Debit.Status.OnHold)).ToList();
          transactionsToCancel.ForEach(t =>
          {
            t.Status = new XPQuery<DBT_Status>(uow).First(s => s.Type == Debit.Status.Cancelled);
            t.LastStatusDate = DateTime.Now;
          });

          uow.CommitChanges();
          cancelled = true;
        }
      }

      return cancelled;
    }

    public DBT_BatchDTO GetPendingBatch()
    {
      using (var uow = new UnitOfWork())
      {
        var batch = new XPQuery<DBT_Batch>(uow).FirstOrDefault(b => b.BatchStatus.Type == Debit.BatchStatus.New
          || b.BatchStatus.Type == Debit.BatchStatus.SubmittedWaitingOutbox
          || b.BatchStatus.Type == Debit.BatchStatus.ValidatedWithErrors
          || b.BatchStatus.Type == Debit.BatchStatus.SubmittedWaitingForReply);

        if (batch != null)
          return AutoMapper.Mapper.Map<DBT_Batch, DBT_BatchDTO>(batch);
        else
          return null;
      }
    }

    public long PrepareExport()
    {
      _logger.Info("Utility.PrepareExport: Started");
      using (var uow = new UnitOfWork())
      {
        var service = new XPQuery<DBT_Service>(uow).FirstOrDefault(s => s.ServiceId == _serviceId);
        List<DBT_Transaction> itemsToBatch = new List<DBT_Transaction>();

        // find New batch 
        var existingBatch = new XPQuery<DBT_Batch>(uow).FirstOrDefault(b => b.BatchStatus.Type == Debit.BatchStatus.New
          || b.BatchStatus.Type == Debit.BatchStatus.SubmittedWaitingOutbox
          || b.BatchStatus.Type == Debit.BatchStatus.ValidatedWithErrors
          || b.BatchStatus.Type == Debit.BatchStatus.SubmittedWaitingForReply);
        if (existingBatch != null)
        {
          if (existingBatch.BatchStatus.Type == Debit.BatchStatus.New)
          {
            // re-prepare batch
            RePrepareNewBatchForExport(existingBatch.BatchId);
            return existingBatch.BatchId;
          }
          else
          {
            return 0;
          }
        }

        var nextActionDate = GetNextActionDate(DateTime.Today);

        var unBatchedTransactions = new XPQuery<DBT_Transaction>(uow).Where(t => t.Status.Type == Debit.Status.New && t.Batch == null
          && (t.OverrideActionDate ?? t.ActionDate) == nextActionDate).ToList();
        _logger.Info(string.Format("Utility.PrepareExport: {0} Unbatched transactions found", unBatchedTransactions.Count));
        if (unBatchedTransactions.Count == 0)
        {
          // Is tomorrow a public holiday? We need to submit today for tomorrow. And the next day?
          if (_publicHolidays.Contains(DateTime.Today.AddDays(1)))
          {
            nextActionDate = GetNextActionDate(DateTime.Today.AddDays(1));
            unBatchedTransactions = new XPQuery<DBT_Transaction>(uow).Where(t => t.Status.Type == Debit.Status.New && t.Batch == null
              && (t.OverrideActionDate ?? t.ActionDate) == nextActionDate).ToList();
            _logger.Info(string.Format("Utility.PrepareExport: {0} Unbatched transactions found (submitting for tomorrow - public holiday)", unBatchedTransactions.Count));
          }
          else
          {
            return 0;
          }
        }

        var batch = new DBT_Batch(uow);
        batch.BatchStatus = new XPQuery<DBT_BatchStatus>(uow).FirstOrDefault(b => b.Type == Debit.BatchStatus.New);
        batch.Service = service;
        batch.LastStatusDate = DateTime.Now;
        batch.CreateDate = DateTime.Now;

        _logger.Info("Utility.PrepareExport: Created Batch");

        var transmission = new DBT_Transmission(uow);
        transmission.Batch = batch;
        transmission.TransmissionNo = service.NextTransmissionNo;
        transmission.Accepted = null;
        transmission.CreateDate = DateTime.Now;

        _logger.Info(string.Format("Utility.PrepareExport: Created Transmission with Transmission No: {0}", service.NextTransmissionNo));

        unBatchedTransactions.ForEach((item) =>
        {
          if (item.Control.ControlType.Type == Debit.ControlType.Predictive
            && !item.OverrideAmount.HasValue
            && item.Amount != item.Control.Instalment)
            item.Amount = item.Control.Instalment;

          if (item.Amount > 0)
            itemsToBatch.Add(item);
        });

        itemsToBatch = itemsToBatch.OrderBy(o => o.Control.TrackingDays).ToList();

        _logger.Info(string.Format("Utility.PrepareExport: {0} Items Needed Batching", itemsToBatch.Count));

        if (itemsToBatch.Count == 0)
          return 0;

        var trackingDay = itemsToBatch.Select(o => o.Control.TrackingDays).Distinct().OrderBy(t => t).ToList();

        int generationNo = service.NextGenerationNo;
        int sequenceNo = GetNextSequenceNo(service);
        foreach (var track in trackingDay)
        {
          //var itemsForTrack = itemsToBatch.Where(t => t.Control.TrackingDays == track).ToList();
          //var itemsForTrackReferences = itemsForTrack.Select(t => t.Control.Service.UserReference).Distinct().ToList();
          //foreach (var itemsForsTrackReference in itemsForTrackReferences)
          //{
          // Create TransmissionSet in here
          //}

          // Create transmission sets for each control
          var transmissionSet = new DBT_TransmissionSet(uow);
          transmissionSet.Transmission = transmission;
          transmissionSet.GenerationNo = generationNo;
          transmissionSet.Accepted = null;
          transmissionSet.CreateDate = DateTime.Now;

          _logger.Info(string.Format("Utility.PrepareExport: Created Transmission Set with Generation No: {0}", generationNo));

          generationNo++;

          var transactionStatusBatched = new XPQuery<DBT_Status>(uow).First(s => s.StatusId == (int)Debit.Status.Batched);
          var itemsToBatchTracking = itemsToBatch.Where(o => o.Control.TrackingDays == track).ToList();

          _logger.Info(string.Format("Utility.PrepareExport: Items in this Transmission Set: {0}", itemsToBatchTracking.Count));

          foreach (var item in itemsToBatchTracking)
          {
            var transmissionTransaction = new DBT_TransmissionTransaction(uow);
            transmissionTransaction.TransmissionSet = transmissionSet;
            transmissionTransaction.Transaction = item;
            transmissionTransaction.Batch = batch;
            transmissionTransaction.SequenceNo = sequenceNo;
            transmissionTransaction.Accepted = null;
            transmissionTransaction.CreateDate = DateTime.Now;

            item.Batch = batch;
            item.Status = transactionStatusBatched;
            item.LastStatusDate = DateTime.Now;

            sequenceNo++;
          }
        }

        //service.NextSequenceNo = sequenceNo;
        //service.LastSequenceUpdate = DateTime.Today;

        uow.CommitChanges();
        _logger.Info("Utility.PrepareExport: Completed");
        return batch.BatchId;
      }
    }

    public void UpdateBatchStatus(long batchId, Debit.BatchStatus batchStatus)
    {
      using (var uow = new UnitOfWork())
      {
        var batchUpdate = new XPQuery<DBT_Batch>(uow).FirstOrDefault(b => b.BatchId == batchId);

        if (batchUpdate == null)
        {
          _logger.Fatal(string.Format("Export() - Batch {0} is missing from the database.", batchId));
          return;
        }

        if (batchStatus == Debit.BatchStatus.SubmittedWaitingForOutput)
        {
          batchUpdate.SubmitDate = DateTime.Now;
          batchUpdate.SubmitUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == 0);
        }
        else if (batchStatus == Debit.BatchStatus.SubmittedWaitingOutbox)
        {
          var transactionStatusSubmitted = new XPQuery<DBT_Status>(uow).First(s => s.StatusId == (int)Debit.Status.Submitted);
          var batchedTransactionToUpdate = new XPQuery<DBT_Transaction>(uow).Where(t => t.Batch == batchUpdate).ToList();
          batchedTransactionToUpdate.ForEach(t =>
          {
            t.Status = transactionStatusSubmitted;
            t.LastStatusDate = DateTime.Now;
          });
        }

        if (batchStatus == Debit.BatchStatus.SubmittedWaitingForReply)
          if (batchUpdate.BatchStatus.BatchStatusId >= (int)Debit.BatchStatus.SubmittedWaitingForReply)
            return;

        batchUpdate.BatchStatus = new XPQuery<DBT_BatchStatus>(uow).FirstOrDefault(b => b.Type == batchStatus);
        batchUpdate.LastStatusDate = DateTime.Now;

        uow.CommitChanges();
      }
    }

    public void UpdateControl(long controlId, Enumerators.General.BankName? bank, string bankBranchCode, string bankAccountNo,
      Enumerators.General.BankAccountType? bankAccountType, string bankAccountName, Atlas.Enumerators.Debit.FailureType? failureType, Atlas.Enumerators.Debit.AVSCheckType? avsCheckType, decimal? instalment,
      Atlas.Enumerators.Account.PeriodFrequency? frequency, Atlas.Enumerators.Account.PayRule? payRule, Atlas.Enumerators.Account.PayDateType? payDateType, int payDateNo, DateTime? overrideNextActionDate)
    {
      using (var uow = new UnitOfWork())
      {
        var control = new XPQuery<DBT_Control>(uow).FirstOrDefault(c => c.ControlId == controlId);
        if (control == null)
          throw new RecordNotFoundException(string.Format("Update Control - ControlId: {0} not found in DB or cannot cancel control in this status", controlId));
        if (bank.HasValue)
          control.Bank = new XPQuery<BNK_Bank>(uow).FirstOrDefault(b => b.Type == bank);
        if (!string.IsNullOrEmpty(bankBranchCode))
          control.BankBranchCode = bankBranchCode;
        if (!string.IsNullOrEmpty(bankAccountNo))
          control.BankAccountNo = bankAccountNo;
        if (!string.IsNullOrEmpty(bankAccountName))
          control.BankAccountName = bankAccountName;
        if (bankAccountType.HasValue)
          control.BankAccountType = new XPQuery<BNK_AccountType>(uow).FirstOrDefault(b => b.Type == bankAccountType);
        if (failureType.HasValue)
          control.FailureType = new XPQuery<DBT_FailureType>(uow).FirstOrDefault(f => f.Type == failureType);
        if (avsCheckType.HasValue)
          control.AVSCheckType = new XPQuery<DBT_AVSCheckType>(uow).FirstOrDefault(a => a.Type == avsCheckType);
        if (instalment.HasValue)
          control.Instalment = Convert.ToDecimal(instalment);
        if (frequency.HasValue)
          control.PeriodFrequency = new XPQuery<ACC_PeriodFrequency>(uow).FirstOrDefault(f => f.Type == frequency);
        if (payRule.HasValue)
          control.PayRule = new XPQuery<ACC_PayRule>(uow).FirstOrDefault(p => p.Type == payRule);
        if (payDateType.HasValue)
          control.PayDate = new XPQuery<ACC_PayDate>(uow).First(p => p.PayDateType.Type == payDateType && p.DayNo == payDateNo);
        if (overrideNextActionDate.HasValue)
        {
          var nextTransaction = new XPQuery<DBT_Transaction>(uow).FirstOrDefault(t => t.Control == control && t.Repetition == control.CurrentRepetition && (t.Status.Type == Debit.Status.New || t.Status.Type == Debit.Status.OnHold));
          nextTransaction.OverrideActionDate = overrideNextActionDate;
          nextTransaction.OverrideDate = DateTime.Now;
          nextTransaction.OverrideUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)Atlas.Enumerators.General.Person.System);
        }

        uow.CommitChanges();
      }
    }

    public Tuple<long, Debit.BatchStatus> GetUnsubmittedBatch()
    {
      using (var uow = new UnitOfWork())
      {
        var service = new XPQuery<DBT_Service>(uow).FirstOrDefault(s => s.ServiceId == _serviceId);

        // find New batch 
        var existingBatch = new XPQuery<DBT_Batch>(uow).FirstOrDefault(b => b.Service == service
          && (b.BatchStatus.Type == Debit.BatchStatus.New
            || b.BatchStatus.Type == Debit.BatchStatus.SubmittedWaitingOutbox
            || b.BatchStatus.Type == Debit.BatchStatus.ValidatedWithErrors
            || b.BatchStatus.Type == Debit.BatchStatus.SubmittedWaitingForReply));
        if (existingBatch != null)
          return new Tuple<long, Debit.BatchStatus>(existingBatch.BatchId, existingBatch.BatchStatus.Type);
        else
          return null;
      }
    }

    public int UnbatchedTransactions()
    {
      using (var uow = new UnitOfWork())
      {
        var nextActionDate = GetNextActionDate(DateTime.Today);

        return new XPQuery<DBT_Transaction>(uow).Count(t => t.Status.StatusId == (int)Debit.Status.New && t.Batch == null
          && (t.OverrideActionDate ?? t.ActionDate) == nextActionDate);
      }
    }

    public void ImportReplyFile(Business.Reply reply)
    {
      using (var uow = new UnitOfWork())
      {
        var transmission = new XPQuery<DBT_Transmission>(uow).FirstOrDefault(t => t.TransmissionNo == Convert.ToInt64(reply.TransmissionStatus.TransmissionNo)
                && !t.Accepted.HasValue); //&& t.ServiceMessage == null);

        var service = transmission.Batch.Service;

        if (reply.TransmissionStatus.TransmissionStatus == Constant.ACCEPTED)
        {
          service.NextTransmissionNo += 1;
          transmission.Accepted = true;
        }
        else
        {
          transmission.Accepted = false;
        }
        transmission.ReplyDate = DateTime.Now;

        transmission.Save();

        _logger.Info(string.Format("ImportReply() - Tranmission {0} has been {1}", reply.TransmissionStatus.TransmissionNo, reply.TransmissionStatus.TransmissionStatus));

        // No need to contiue processing, the tranmission has been rejected.
        if (reply.TransmissionStatus.TransmissionStatus != Constant.ACCEPTED)
        {
          var transmissionSets = new XPQuery<DBT_TransmissionSet>(uow).Where(t => !t.Accepted.HasValue && t.Transmission.TransmissionId == transmission.TransmissionId).ToList();
          transmissionSets.ForEach(transmissionSet =>
          {
            transmissionSet.Accepted = false;
            var transmissionTransactions = new XPQuery<DBT_TransmissionTransaction>(uow).Where(tt => tt.TransmissionSet == transmissionSet).ToList();
            transmissionTransactions.ForEach(transmissionTransaction =>
            {
              transmissionTransaction.Accepted = false;
            });
          });

          transmission.Batch.BatchStatus = new XPQuery<DBT_BatchStatus>(uow).FirstOrDefault(b => b.Type == Debit.BatchStatus.Failed);
          transmission.Batch.LastStatusDate = DateTime.Now;

          uow.CommitChanges();
          return;
        }

        // Check each userset in the reply file.
        foreach (var userSet in reply.UserSet)
        {
          var rejectedCount = userSet.RejectedTransaction == null ? 0 : userSet.RejectedTransaction.Count();
          var transactionCount = userSet.Transactions == null ? 0 : userSet.Transactions.Count();

          _logger.Info(string.Format("ImportReply() - UserSet {0} - Rejected/Warning Transactions {1} - Transactions {2} - Batch Status {3}",
            userSet.UserSetStatus.BankServUserCodeGenerationNo, rejectedCount, transactionCount, userSet.UserSetStatus.UserSetStatus));

          // Ensure we select the correct user set 
          var transmissionSet = new XPQuery<DBT_TransmissionSet>(uow).FirstOrDefault(t => !t.Accepted.HasValue && t.GenerationNo == Convert.ToInt64(userSet.GenerationNo)
                      && t.Transmission.TransmissionId == transmission.TransmissionId);

          if (transmissionSet != null)
          {
            if (userSet.UserSetStatus.UserSetStatus == Constant.ACCEPTED)
            {
              service.NextGenerationNo++;
              transmissionSet.Accepted = true;
            }
            else
            {
              transmissionSet.Accepted = false;
            }

            transmissionSet.Save();

            if (userSet.Transactions != null)
            {
              // Go through all transactions within each userset.
              foreach (var transaction in userSet.Transactions)
              {
                // Find any rejected transmissions from the SeqNo and GenerationNo
                RejectedMessageItem rejectedTransmission = null;

                if (userSet.RejectedTransaction != null)
                  rejectedTransmission = userSet.RejectedTransaction.FirstOrDefault(o => o.UserSequenceNo == transaction.SequenceNo
                                                                                                      && o.BankServUserCodeGenerationNo == userSet.GenerationNo);

                // Find the correlating transmission transaction in the db
                var transmissionTransaction = new XPQuery<DBT_TransmissionTransaction>(uow).FirstOrDefault(t => t.TransmissionSet.TransmissionSetId == transmissionSet.TransmissionSetId
                                                                                                           && t.SequenceNo == Convert.ToInt64(transaction.SequenceNo));

                if (transmissionTransaction == null)
                  _logger.Fatal(string.Format("ImportReply() - Transmission transaction record not found within userset {0} and sequence no {1}", userSet.GenerationNo, transaction.SequenceNo));

                if (transmissionTransaction != null)
                {
                  if (rejectedTransmission != null)
                  {
                    bool isRejected = rejectedTransmission.ErrorMessage.Contains(Constant.REJECTED);
                    bool isWarning = rejectedTransmission.ErrorMessage.Contains(Constant.WARNING);

                    if (isRejected)
                    {
                      transmissionTransaction.ServiceMessage = new XPQuery<DBT_ServiceMessage>(uow).FirstOrDefault(s => s.Code == rejectedTransmission.ErrorCode);
                      transmissionTransaction.Accepted = false;
                      transmissionTransaction.Transaction.Status = new XPQuery<DBT_Status>(uow).FirstOrDefault(s => s.StatusId == (int)Debit.Status.Failed);
                      transmissionTransaction.Transaction.LastStatusDate = DateTime.Now;
                    }
                    else if (!isRejected && isWarning)
                    {
                      transmissionTransaction.Accepted = true;
                      transmissionTransaction.ServiceMessage = new XPQuery<DBT_ServiceMessage>(uow).FirstOrDefault(s => s.Code == rejectedTransmission.ErrorCode);
                    }
                  }
                  else
                  {
                    transmissionTransaction.Accepted = true;
                  }
                  if (transmission.Accepted == true)
                  {
                    if (service.LastSequenceUpdate.Date == DateTime.Today)
                    {
                      if (service.NextSequenceNo < transmissionTransaction.SequenceNo)
                        service.NextSequenceNo = transmissionTransaction.SequenceNo;
                    }
                    else
                    {
                      service.NextSequenceNo = transmissionTransaction.SequenceNo;
                    }
                    service.LastSequenceUpdate = DateTime.Now;
                  }
                  transmissionTransaction.Save();
                }
              }
            }
          }
        }
        // Update the batch.
        uow.CommitChanges();

        if (transmission.Accepted.HasValue)
          UpdateBatchStatus(transmission.Batch.BatchId, transmission.Accepted ?? false ? Debit.BatchStatus.SubmittedWaitingForOutput : Debit.BatchStatus.Failed);
      }
    }

    public List<ACC_AccountDTO> GetAccounts(IEnumerable<long> controlIds)
    {
      using (var uow = new UnitOfWork())
      {
        var accounts = new XPQuery<ACC_DebitControl>(uow).Where(c => controlIds.Contains(c.Control.ControlId)).Select(c => c.Account).ToList();
        return AutoMapper.Mapper.Map<List<ACC_Account>, List<ACC_AccountDTO>>(accounts);
      }
    }

    /// <summary>
    /// Imports the output file in order to determine the status of the debit order.
    /// </summary>
    public List<long> ImportOutputFile(Business.Output.Output output)
    {
      var affectedControlIds = new List<long>();
      var controlsToTryComplete = new List<long>();
      using (var uow = new UnitOfWork())
      {
        if (output.UserSet.Count > 0)
        {
          var transactionStatus = new XPQuery<DBT_Status>(uow).Where(a => a.StatusId == (int)Debit.Status.Successful
            || a.StatusId == (int)Debit.Status.Failed
            || a.StatusId == (int)Debit.Status.Disputed);

          var controlStatuses = new XPQuery<DBT_ControlStatus>(uow).ToList();

          foreach (var set in output.UserSet)
          {
            foreach (var trans in set.Transactions)
            {
              if (trans.BankServRecordIdentifier.Contains("65") || trans.BankServRecordIdentifier.Contains("35") || trans.BankServRecordIdentifier.Contains("90"))
              {
                //long transmissionTransactionId = Convert.ToInt64(trans.ContractReference.Trim());
                //var transmission = new XPQuery<DBT_TransmissionTransaction>(uow).FirstOrDefault(t => t.TransmissionTransactionId == transmissionTransactionId
                //    && t.Accepted == true); //&& t.ServiceMessage == null);

                var transmissionTranscation = new XPQuery<DBT_TransmissionTransaction>(uow).FirstOrDefault(t => t.SequenceNo == int.Parse(trans.TransactionSequenceNo)
                    && t.Accepted == true
                    && (t.Transaction.OverrideActionDate ?? t.Transaction.ActionDate) == DateTime.ParseExact(trans.CycleDate, "yyMMdd", CultureInfo.CurrentCulture));

                if (transmissionTranscation == null)
                {
                  _logger.Fatal(string.Format("ImportOutput() - TransmissionTransaction record with Id {0} was not found in the database. - {1}, {2}", trans.ContractReference.Trim(), trans.TransactionSequenceNo, DateTime.ParseExact(trans.CycleDate, "yyMMdd", CultureInfo.CurrentCulture).ToShortDateString()));
                }
                else
                {
                  affectedControlIds.Add(transmissionTranscation.Transaction.Control.ControlId);

                  if (trans.BankServRecordIdentifier.Contains("65"))// “65” – NAEDO Request Response Record
                  {
                    var transaction = new XPQuery<DBT_Transaction>(uow).FirstOrDefault(t => t.TransactionId == transmissionTranscation.Transaction.TransactionId
                      && t.Status.StatusId == (int)Debit.Status.Submitted);

                    if (transaction == null)
                    {
                      _logger.Fatal(string.Format("ImportOuput() - Request Response - Transaction record with Id {0} was not found in the database with a submitted status.", transmissionTranscation.Transaction.TransactionId));
                    }
                    else
                    {
                      transaction.ResponseCode = new XPQuery<DBT_ResponseCode>(uow).FirstOrDefault(r => r.Code == trans.ResponseCode);
                      transaction.ResponseDate = DateTime.Now;
                      _logger.Info(string.Format("ImportOuput() - Transaction {0} was returned with a response of {1}", transaction.TransactionId, transaction.ResponseCode.Description));
                      if (transaction.ResponseCode.IsFailed.HasValue)
                      {
                        transaction.Status = ((bool)transaction.ResponseCode.IsFailed)
                          ? transactionStatus.FirstOrDefault(s => s.StatusId == (int)Debit.Status.Failed)
                          : transactionStatus.FirstOrDefault(s => s.StatusId == (int)Debit.Status.Successful);
                        transaction.LastStatusDate = DateTime.Now;

                        if (transaction.Control.ControlType.Type == Debit.ControlType.Predictive
                          && transaction.DebitType.Type == Debit.DebitType.Regular)
                        {
                          var repetition = transaction.Repetition;
                          if (transaction.Status.Type == Debit.Status.Successful
                            && transaction.Control.CurrentRepetition < transaction.Control.Repetitions)
                          {
                            repetition++;
                            CreateNewTransaction(uow, transaction, repetition);
                          }
                          else if (transaction.Status.Type == Debit.Status.Failed)
                          {
                            if (transaction.Control.FailureType.Type == Debit.FailureType.Continue)
                            {
                              if (transaction.Control.CurrentRepetition < transaction.Control.Repetitions)
                              {
                                repetition++;
                                CreateNewTransaction(uow, transaction, repetition);
                              }
                              else
                              {
                                controlsToTryComplete.Add(transaction.Control.ControlId);
                              }
                            }
                            else if (transaction.Control.FailureType.Type == Debit.FailureType.Retry)
                            {
                              CreateNewTransaction(uow, transaction, repetition);
                            }
                            else if (transaction.Control.FailureType.Type == Debit.FailureType.Break)
                            {
                              UpdateControlStatus(uow, transaction.Control, controlStatuses.FirstOrDefault(c => c.Type == Debit.ControlStatus.CompletedWithFailedDebit));
                            }
                          }
                          else
                          {
                            controlsToTryComplete.Add(transaction.Control.ControlId);
                          }
                        }
                        else
                        {
                          controlsToTryComplete.Add(transaction.Control.ControlId);
                        }
                      }
                    }
                  }
                  else if (trans.BankServRecordIdentifier.Contains("35"))
                  {
                    var transaction = new XPQuery<DBT_Transaction>(uow).FirstOrDefault(t => t.TransactionId == transmissionTranscation.Transaction.TransactionId
                      && t.Status.StatusId == (int)Debit.Status.Successful);

                    if (transaction == null)
                    {
                      _logger.Fatal(string.Format("ImportOuput() - Dispute - Transaction record with Id {0} was not found in the database with a successful status.", transmissionTranscation.Transaction.TransactionId));
                    }
                    else
                    {
                      transaction.ResponseCode = new XPQuery<DBT_ResponseCode>(uow).FirstOrDefault(r => r.Code == trans.ResponseCode);
                      transaction.ResponseDate = DateTime.Now;
                      _logger.Info(string.Format("ImportOuput() - Transaction {0} was disputed with a response of {1}", transaction.TransactionId, transaction.ResponseCode.Description));
                      if (transaction.ResponseCode.IsFailed.HasValue)
                      {
                        transaction.Status = transactionStatus.FirstOrDefault(s => s.StatusId == (int)Debit.Status.Disputed);
                        transaction.LastStatusDate = DateTime.Now;
                      }
                    }
                  }
                  // TODO: “90” – NAEDO Recall Response Record)
                  else if (trans.BankServRecordIdentifier.Contains("90"))
                  {
                    throw new NotImplementedException("This has not been implemented as we don't do recalls as yet");
                  }
                }
              }
            }
          }
          uow.CommitChanges();
        }
      }

      this.CheckBatch();

      this.TryCompletingControls(controlsToTryComplete);

      return affectedControlIds;
    }

    public DBT_TransactionDTO AddAdditionalDebitOrder(long controlId, DateTime actionDate, decimal amount, DateTime instalmentDate)
    {
      using (var uow = new UnitOfWork())
      {
        var control = new XPQuery<DBT_Control>(uow).FirstOrDefault(c => c.ControlId == controlId);
        if (control == null)
          throw new RecordNotFoundException(string.Format("Add Additional Debit Order - ControlId: {0} not found in DB", controlId));

        var newTransaction = new DBT_Transaction(uow)
        {
          ActionDate = actionDate,
          Amount = amount,
          Control = control,
          CreateDate = DateTime.Now,
          DebitType = new XPQuery<DBT_DebitType>(uow).FirstOrDefault(d => d.Type == Debit.DebitType.Additional),
          InstalmentDate = instalmentDate,
          LastStatusDate = DateTime.Now,
          Repetition = 0,
          Status = new XPQuery<DBT_Status>(uow).FirstOrDefault(d => d.Type == Debit.Status.New)
        };

        uow.CommitChanges();

        return AutoMapper.Mapper.Map<DBT_Transaction, DBT_TransactionDTO>(newTransaction);
      }
    }

    public DBT_TransactionDTO CancelAdditionalDebitOrder(long controlId, long transactionId)
    {
      using (var uow = new UnitOfWork())
      {
        var control = new XPQuery<DBT_Control>(uow).FirstOrDefault(c => c.ControlId == controlId);
        if (control == null)
          throw new RecordNotFoundException(string.Format("Cancel Additional Debit Order - ControlId: {0} not found in DB", controlId));

        var transaction = new XPQuery<DBT_Transaction>(uow).FirstOrDefault(t => t.Control == control && t.TransactionId == transactionId
          && (t.Status.Type == Debit.Status.New
            || t.Status.Type == Debit.Status.OnHold));
        if (transaction == null)
          throw new RecordNotFoundException(string.Format("Cancel Additional Debit Order - TransactionId {0} not found in DB with status New or OnHold", transactionId));

        transaction.Status = new XPQuery<DBT_Status>(uow).FirstOrDefault(s => s.Type == Debit.Status.Cancelled);
        transaction.LastStatusDate = DateTime.Now;

        uow.CommitChanges();

        return AutoMapper.Mapper.Map<DBT_Transaction, DBT_TransactionDTO>(transaction);
      }
    }

    /// <summary>
    /// Removes all transactions from a batch so that it can be resent.
    /// </summary>
    /// <param name="batchId"></param>
    public void UnbatchRejectedTransactions(long batchId)
    {
      using (var uow = new UnitOfWork())
      {
        var batch = new XPQuery<DBT_Batch>(uow).FirstOrDefault(b => b.BatchId == batchId);
        if (batch == null)
          throw new RecordNotFoundException(string.Format("Unbatch Rejected Transactions - BatchId {0} not found in DB", batchId));

        if (batch.BatchStatus.Type != Debit.BatchStatus.Failed)
          throw new GeneralDebitOrderException(string.Format("Unbatch Rejected Transactions - BatchId {0} does not have a status of 'Failed", batchId));

        if (batch.SubmitDate.HasValue && batch.SubmitDate.Value.Date != DateTime.Today.Date)
          throw new GeneralDebitOrderException(string.Format("Unbatch Rejected Transactions - BatchId {0} was not submitted today", batchId));

        var transactions = new XPQuery<DBT_Transaction>(uow).Where(t => t.Batch == batch).ToList();
        if (transactions.Count == 0)
          throw new GeneralDebitOrderException(string.Format("Unbatch Rejected Transactions - BatchId {0} does not have any batched transcations", batchId));

        var newStatus = new XPQuery<DBT_Status>(uow).FirstOrDefault(s => s.Type == Debit.Status.New);
        transactions.ForEach(t =>
        {
          t.Batch = null;
          t.Status = newStatus;
          t.LastStatusDate = DateTime.Now;
        });

        uow.CommitChanges();
      }
    }

    /// <summary>
    /// Used to update the status of a batch.
    /// </summary>
    public void CheckBatch()
    {
      using (var uow = new UnitOfWork())
      {
        var batchCollection = new XPQuery<DBT_Batch>(uow).Where(b => b.BatchStatus.Type == Debit.BatchStatus.SubmittedWaitingForOutput).ToList();

        foreach (var batch in batchCollection)
        {
          var batchItemCollection = new XPQuery<DBT_TransmissionTransaction>(uow).Where(t =>
            t.Batch.BatchId == batch.BatchId
            && t.Accepted == true);

          var pending = batchItemCollection.Count(b => b.Transaction.ResponseCode == null);
          pending += batchItemCollection.Count(b => b.Transaction.ResponseCode != null && !b.Transaction.ResponseCode.IsFailed.HasValue);
          var failed = batchItemCollection.Count(b => b.Transaction.ResponseCode != null && b.Transaction.ResponseCode.IsFailed.HasValue && b.Transaction.ResponseCode.IsFailed.Value);
          var passed = batchItemCollection.Count(b => b.Transaction.ResponseCode != null && b.Transaction.ResponseCode.IsFailed.HasValue && !b.Transaction.ResponseCode.IsFailed.Value);

          if (pending == 0)
          {
            if (passed == batchItemCollection.Count())
              batch.BatchStatus = new XPQuery<DBT_BatchStatus>(uow).FirstOrDefault(b => b.Type == Debit.BatchStatus.Completed);
            else if (failed > 0 && (failed + passed) == batchItemCollection.Count())
              batch.BatchStatus = new XPQuery<DBT_BatchStatus>(uow).FirstOrDefault(b => b.Type == Debit.BatchStatus.CompletedWithErrors);
            else if (failed == batchItemCollection.Count())
              batch.BatchStatus = new XPQuery<DBT_BatchStatus>(uow).FirstOrDefault(b => b.Type == Debit.BatchStatus.Failed);

            batch.LastStatusDate = DateTime.Now;

            _logger.Info(string.Format("CheckBatch() - Batch {0} has been updated to Status {1}", batch.BatchId, batch.BatchStatus.Description));

            uow.CommitChanges();
          }
        }
      }
    }

    public List<ResponseControl> Query(Atlas.Enumerators.General.Host? host, long? branchId, DateTime? startRange, DateTime? endRange, bool controlOnly = true)
    {

      _logger.Info("[Query] - Search criteria, Host: {0}, BranchId: {1}, StartRange: {2}, EndRage: {3}, ControlOnly{4}", host, branchId, startRange, endRange, controlOnly);

      var result = new List<ResponseControl>();
      using (var uow = new UnitOfWork())
      {
        var query = new XPQuery<DBT_Control>(uow).Where(a => a.ControlId > 0);

        if (host.HasValue)
          query = query.Where(a => a.Host.Type == host);

        if (branchId.HasValue && branchId > 0)
          query = query.Where(a => a.CompanyBranch.CompanyId == branchId);

        if (startRange.HasValue && endRange.HasValue)
          query = query.Where(a => a.Transactions.Where(t => t.ActionDate >= startRange && t.ActionDate <= endRange).Count() > 0);
        else if (startRange.HasValue)
          query = query.Where(a => a.Transactions.Where(t => t.ActionDate >= startRange).Count() > 0);
        else if (endRange.HasValue)
          query = query.Where(a => a.Transactions.Where(t => t.ActionDate <= endRange).Count() > 0);

        var debitControls = query.ToList();

        //FFR: No point in doing the rest if nothing was found.
        if (debitControls.Count == 0)
        {
          _logger.Info("[Query] - No debit orders found with search criteria, Host: {0}, BranchId: {1}, StartRange: {2}, EndRage: {3}, ControlOnly{4}", host,
            branchId, startRange, endRange, controlOnly);

          // Return empty collection
          return new List<ResponseControl>();
        }

        var inValidControlIds = debitControls.Where(c => !c.IsValid).Select(c => c.ControlId).ToList();
        var validations = new XPQuery<DBT_ControlValidation>(uow).Where(c => inValidControlIds.Contains(c.Control.ControlId)).ToList();

        var transactions = new List<DBT_Transaction>();
        if (!controlOnly)
          transactions = new XPQuery<DBT_Transaction>(uow).Where(t => debitControls.Contains(t.Control)).ToList();

        var transmissions = new XPQuery<DBT_TransmissionTransaction>(uow).Where(t => transactions.Contains(t.Transaction)).ToList();

        foreach (var control in debitControls)
        {
          var responseControl = new ResponseControl();
          if (control.AVSCheckType != null)
            responseControl.AVSCheckType = control.AVSCheckType.Type;
          responseControl.Bank = control.Bank.Description;
          responseControl.BankId = control.Bank.BankId;
          responseControl.BankBranchCode = control.BankBranchCode;
          responseControl.BankAccountNo = control.BankAccountNo;
          responseControl.BankAccountName = control.BankAccountName;
          responseControl.BankAccountTypeId = control.BankAccountType.AccountTypeId;
          responseControl.BankAccountType = control.BankAccountType.Description;
          responseControl.BankStatementReference = control.BankStatementReference;
          responseControl.ControlId = control.ControlId;
          responseControl.IdNumber = control.IdNumber;
          responseControl.Instalment = control.Instalment;
          responseControl.LastInstalmentUpdate = control.LastInstalmentUpdate;
          responseControl.Repetitions = control.Repetitions;
          responseControl.ResponseTransactions = new List<ResponseTransaction>();
          responseControl.ThirdPartyReference = control.ThirdPartyReference;
          responseControl.TrackingDay = (Atlas.Enumerators.Debit.TrackingDay)control.TrackingDays;
          responseControl.TrackingDays = control.TrackingDays;
          responseControl.ValidationErrors = new List<Debit.ValidationType>();

          if (control.ControlStatus != null)
            responseControl.ControlStatus = control.ControlStatus.Type;

          if (control.ControlType != null)
            responseControl.ControlType = control.ControlType.Type;
          responseControl.CurrentRepetition = control.CurrentRepetition;

          if (control.FailureType != null)
            responseControl.FailureType = control.FailureType.Type;
          responseControl.FirstInstalmentDate = control.FirstInstalmentDate;

          if (control.PeriodFrequency != null)
            responseControl.Frequency = control.PeriodFrequency.Type;

          if (control.PayRule != null)
            responseControl.PayRule = control.PayRule.Type;

          if (control.PayDate != null)
            responseControl.PayDateType = control.PayDate.PayDateType.Type;

          if (control.PayDate.PayDateType.Type == Enumerators.Account.PayDateType.DayOfWeek)
            responseControl.PayDateDayOfWeek = (DayOfWeek)(control.PayDate.DayNo - 1);
          else
            responseControl.PayDateDayOfMonth = control.PayDate.DayNo;

          if (!control.IsValid)
            responseControl.ValidationErrors.AddRange(validations.Where(c => control.ControlId == c.Control.ControlId).Select(v => v.Validation.Type));

          var controlTransactions = transactions.Where(t => t.Control == control);
          foreach (var transaction in controlTransactions)
          {
            var responseTransaction = new ResponseTransaction();

            var transactionTransmission = transmissions.FirstOrDefault(t => t.Transaction == transaction);

            responseTransaction.ActionDate = transaction.ActionDate;
            responseTransaction.Amount = transaction.Amount;
            responseTransaction.CancelDate = transaction.CancelDate;
            responseTransaction.InstalmentDate = transaction.InstalmentDate;
            responseTransaction.OverrideActionDate = transaction.OverrideActionDate;
            responseTransaction.OverrideAmount = transaction.OverrideAmount;
            responseTransaction.OverrideTrackingDays = transaction.OverrideTrackingDays;
            responseTransaction.Repetition = transaction.Repetition;
            responseTransaction.ResponseCode = transaction.ResponseCode == null ? null : transaction.ResponseCode.Code;
            responseTransaction.ResponseCodeDescription = transaction.ResponseCode == null ? null : transaction.ResponseCode.Description;
            responseTransaction.Status = transaction.Status.Type;
            responseTransaction.TransactionNo = transaction.TransactionId;
            responseTransaction.ResponseBatch = new ResponseTransmissionBatch();

            if (transactionTransmission != null)
            {
              responseTransaction.ResponseBatch.BatchId = transactionTransmission.TransmissionSet.Transmission.Batch.BatchId;
              responseTransaction.ResponseBatch.BatchStatus = transactionTransmission.TransmissionSet.Transmission.Batch.BatchStatus.Type;
              responseTransaction.ResponseBatch.CreateDate = transactionTransmission.TransmissionSet.Transmission.Batch.CreateDate;
              responseTransaction.ResponseBatch.LastStatusDate = transactionTransmission.TransmissionSet.Transmission.Batch.LastStatusDate;
              responseTransaction.ResponseBatch.SubmitDate = transactionTransmission.TransmissionSet.Transmission.Batch.SubmitDate;
              responseTransaction.ResponseBatch.TransmissionAccepted = transactionTransmission.TransmissionSet.Transmission.Accepted;
              responseTransaction.ResponseBatch.TransmissionNo = transactionTransmission.TransmissionSet.Transmission.TransmissionNo;
            }

            responseControl.ResponseTransactions.Add(responseTransaction);
          }

          result.Add(responseControl);
        }

        return result;
      }
    }

    public List<NaedoBatch> GetNaedoBatches(DateTime? startRange, DateTime? endRange, long? batchId, Atlas.Enumerators.Debit.BatchStatus? batchStatus, bool batchOnly = true)
    {
      var naedoBatches = new List<NaedoBatch>();
      using (var uow = new UnitOfWork())
      {
        var batchQuery = new XPQuery<DBT_Batch>(uow).Where(b => b.BatchId >= 0);

        if (startRange.HasValue)
          batchQuery = batchQuery.Where(b => (b.LastStatusDate ?? b.CreateDate).Date >= startRange.Value.Date);

        if (endRange.HasValue)
          batchQuery = batchQuery.Where(b => (b.LastStatusDate ?? b.CreateDate).Date <= endRange.Value.Date);

        if (batchId.HasValue)
          batchQuery = batchQuery.Where(b => b.BatchId == batchId);

        if (batchStatus.HasValue)
          batchQuery = batchQuery.Where(b => b.BatchStatus.Type == batchStatus);

        var batches = batchQuery.ToList();
        var transmissions = new XPQuery<DBT_Transmission>(uow).Where(b => batches.Contains(b.Batch)).ToList();
        var transactions = new List<DBT_Transaction>();
        var transmissionTransactions = new List<DBT_TransmissionTransaction>();

        if (!batchOnly)
        {
          transactions = new XPQuery<DBT_Transaction>(uow).Where(t => batches.Contains(t.Batch)).ToList();
          transmissionTransactions = new XPQuery<DBT_TransmissionTransaction>(uow).Where(t => batches.Contains(t.Batch) && transactions.Contains(t.Transaction)).ToList();
        }

        foreach (var batch in batches)
        {
          var transmission = transmissions.FirstOrDefault(t => t.Batch == batch);
          var naedoBatch = new NaedoBatch();
          naedoBatch.BatchId = batch.BatchId;
          naedoBatch.BatchStatus = batch.BatchStatus.Type;
          naedoBatch.BatchStatusDescription = batch.BatchStatus.Type.ToStringEnum();
          naedoBatch.CreateDate = batch.CreateDate;
          naedoBatch.LastStatusDate = batch.LastStatusDate;
          naedoBatch.SubmitDate = batch.SubmitDate;

          if (transmission == null)
          {
            naedoBatch.TransmissionAccepted = null;
            naedoBatch.TransmissionNo = 0;
          }
          else
          {
            naedoBatch.TransmissionAccepted = transmission.Accepted;
            naedoBatch.TransmissionNo = transmission.TransmissionNo;
          }

          naedoBatch.NaedoBatchTransactions = new List<NaedoBatchTransaction>();

          var batchTransactions = transactions.Where(b => b.Batch == batch);
          foreach (var batchTransaction in batchTransactions)
          {
            var transmissionTransaction = transmissionTransactions.Where(t => t.Transaction == batchTransaction).FirstOrDefault();

            var naedoBatchTransaction = new NaedoBatchTransaction();
            naedoBatchTransaction.ActionDate = batchTransaction.ActionDate;
            naedoBatchTransaction.Amount = batchTransaction.Amount;
            naedoBatchTransaction.CancelDate = batchTransaction.CancelDate;
            naedoBatchTransaction.InstalmentDate = batchTransaction.InstalmentDate;
            naedoBatchTransaction.OverrideActionDate = batchTransaction.OverrideActionDate;
            naedoBatchTransaction.OverrideAmount = batchTransaction.OverrideAmount;
            naedoBatchTransaction.OverrideTrackingDays = batchTransaction.OverrideTrackingDays;
            naedoBatchTransaction.Repetition = batchTransaction.Repetition;
            naedoBatchTransaction.ReplyCode = transmissionTransaction == null || transmissionTransaction.ServiceMessage == null ? string.Empty : transmissionTransaction.ServiceMessage.Code;
            naedoBatchTransaction.ReplyCodeDescription = transmissionTransaction == null || transmissionTransaction.ServiceMessage == null ? string.Empty : transmissionTransaction.ServiceMessage.Description;
            naedoBatchTransaction.ResponseCode = batchTransaction.ResponseCode == null ? string.Empty : batchTransaction.ResponseCode.Code;
            naedoBatchTransaction.ResponseCodeDescription = batchTransaction.ResponseCode == null ? string.Empty : batchTransaction.ResponseCode.Description;
            naedoBatchTransaction.Status = batchTransaction.Status.Type;
            naedoBatchTransaction.StatusDescription = batchTransaction.Status.Type.ToStringEnum();
            naedoBatchTransaction.TransactionNo = batchTransaction.TransactionId;
            naedoBatchTransaction.ControlId = batchTransaction.Control.ControlId;
            naedoBatchTransaction.Bank = batchTransaction.Control.Bank.Description;
            naedoBatchTransaction.BankAccountNo = batchTransaction.Control.BankAccountNo;
            naedoBatchTransaction.BankAccountName = batchTransaction.Control.BankAccountName;
            naedoBatchTransaction.BankStatementReference = batchTransaction.Control.BankStatementReference;
            naedoBatchTransaction.IdNumber = batchTransaction.Control.IdNumber;

            naedoBatch.NaedoBatchTransactions.Add(naedoBatchTransaction);
          }
          naedoBatches.Add(naedoBatch);
        }
      }
      return naedoBatches;
    }

    public void LoadETLBatches()
    {
      using (var uow = new UnitOfWork())
      {
        var newDebitOrders = new XPQuery<ETL_DebitOrder>(uow).Where(d => (d.DebitOrderBatch.Stage.Type == ETL.Stage.New || d.DebitOrderBatch.Stage.Type == ETL.Stage.Loading)
          && (d.Stage.Type == ETL.Stage.New || d.Stage.Type == ETL.Stage.Loading)).ToList();

        _logger.Info(string.Format("Utility.LoadETLBatches: {0} new debit Orders", newDebitOrders.Count));

        foreach (var debitOrder in newDebitOrders)
        {
          try
          {
            if (debitOrder.DebitOrderBatch.Stage.Type == ETL.Stage.New)
            {
              _logger.Info(string.Format("Utility.LoadETLBatches: Updating Stage Of Batch {0} to Loading", debitOrder.DebitOrderBatch.DebitOrderBatchId));
              debitOrder.DebitOrderBatch.Stage = new XPQuery<ETL_Stage>(uow).FirstOrDefault(s => s.Type == ETL.Stage.Loading);
              debitOrder.DebitOrderBatch.LastStageDate = DateTime.Now;
            }

            debitOrder.Stage = new XPQuery<ETL_Stage>(uow).FirstOrDefault(s => s.Type == ETL.Stage.Loading);
            debitOrder.LastStageDate = DateTime.Now;

            var control = CreateNewControl(debitOrder.ThirdPartyReference, debitOrder.BankStatementReference, Debit.ControlType.Predictive, Debit.AVSCheckType.None, debitOrder.Bank.Type, debitOrder.AccountName,
              debitOrder.AccountNumber, debitOrder.BankAccountType.Type, debitOrder.BankBranchCode, debitOrder.IdNumber, Debit.FailureType.Continue, debitOrder.FirstActionDate, debitOrder.InstalmentAmount,
              debitOrder.PayRule.Type, debitOrder.PeriodFrequency.Type, debitOrder.Repititions, (Atlas.Enumerators.Debit.TrackingDay)debitOrder.TrackingDays, debitOrder.DebitOrderBatch.Host.Type,
              debitOrder.DebitOrderBatch.Company == null ? (long?)null : debitOrder.DebitOrderBatch.Company.CompanyId, debitOrder.PayDateType.Type, debitOrder.PayDateNo);

            _logger.Info(string.Format("Utility.LoadETLBatches: ETL_DebitOrderId({0})=>DBT_ControlId({1})", debitOrder.DebitOrderId, control.ControlId));

            debitOrder.DebitOrderControl = new XPQuery<DBT_Control>(uow).First(d => d.ControlId == control.ControlId);

            debitOrder.Stage = new XPQuery<ETL_Stage>(uow).FirstOrDefault(s => s.Type == ETL.Stage.Completed);
            debitOrder.LastStageDate = DateTime.Now;
          }
          catch (RecordAlreadyExistsException exception)
          {
            _logger.Error(string.Format("Utility.LoadETLBatches: {0}", exception.Message));

            debitOrder.Stage = new XPQuery<ETL_Stage>(uow).FirstOrDefault(s => s.Type == ETL.Stage.Error);
            debitOrder.LastStageDate = DateTime.Now;
            debitOrder.ErrorMessage = exception.Message;
          }
          catch (Exception exception)
          {
            _logger.Error(string.Format("Utility.LoadETLBatches: {0}", exception.Message));

            debitOrder.Stage = new XPQuery<ETL_Stage>(uow).FirstOrDefault(s => s.Type == ETL.Stage.Error);
            debitOrder.LastStageDate = DateTime.Now;
            debitOrder.ErrorMessage = exception.Message;
            uow.CommitChanges();

            throw exception;
          }
        }

        uow.CommitChanges();

        var etlBatches = newDebitOrders.Select(d => d.DebitOrderBatch).Distinct();
        foreach (var batch in etlBatches)
        {
          var debitOrders = new XPQuery<ETL_DebitOrder>(uow).Where(d => d.DebitOrderBatch == batch).Select(d => d.Stage).ToList();
          var isCompleted = debitOrders.Where(d => d.Type == ETL.Stage.New).Count() == 0;
          var completedWithErrors = debitOrders.Where(d => d.Type == ETL.Stage.Error).Count() > 0;
          var completedWithOnlyErrors = debitOrders.Where(d => d.Type == ETL.Stage.Completed).Count() == 0;
          if (isCompleted)
          {
            _logger.Info(string.Format("Utility.LoadETLBatches: Updating Stage Of Batch {0} to Completed", batch.DebitOrderBatchId));
            if (completedWithOnlyErrors)
              batch.Stage = new XPQuery<ETL_Stage>(uow).FirstOrDefault(s => s.Type == ETL.Stage.Error);
            else if (completedWithErrors)
              batch.Stage = new XPQuery<ETL_Stage>(uow).FirstOrDefault(s => s.Type == ETL.Stage.CompletedContainsErrors);
            else
              batch.Stage = new XPQuery<ETL_Stage>(uow).FirstOrDefault(s => s.Type == ETL.Stage.Completed);
            batch.LastStageDate = DateTime.Now;

            uow.CommitChanges();
          }
        }
      }
    }

    public void ExportBatch(long batchId)
    {
      using (var uow = new UnitOfWork())
      {
        var service = new XPQuery<DBT_Service>(uow).First(s => s.ServiceId == _serviceId);

        var batch = new XPQuery<DBT_Batch>(uow).First(b => b.BatchId == batchId);
        var transmission = new XPQuery<DBT_Transmission>(uow).First(b => b.Batch == batch);
        var transmissionTransactions = new XPQuery<DBT_TransmissionTransaction>(uow).Where(o => o.Batch.BatchId == batchId).ToList();

        var exported = new FileUtility().ExportFile(AutoMapper.Mapper.Map<DBT_Service, DBT_ServiceDTO>(service),
                                     AutoMapper.Mapper.Map<List<DBT_TransmissionSet>, List<DBT_TransmissionSetDTO>>(transmissionTransactions.Select(t => t.TransmissionSet).ToList()),
                                     AutoMapper.Mapper.Map<DBT_Transmission, DBT_TransmissionDTO>(transmission),
                                     AutoMapper.Mapper.Map<DBT_Batch, DBT_BatchDTO>(batch),
                                     AutoMapper.Mapper.Map<List<DBT_TransmissionTransaction>, List<DBT_TransmissionTransactionDTO>>(transmissionTransactions),
                                     AutoMapper.Mapper.Map<List<DBT_Transaction>, List<DBT_TransactionDTO>>(transmissionTransactions.Select(t => t.Transaction).ToList()),
                                     AutoMapper.Mapper.Map<List<DBT_Control>, List<DBT_ControlDTO>>(transmissionTransactions.Select(t => t.Transaction).ToList().Select(t => t.Control).ToList()));
        if (exported)
        {
          UpdateBatchStatus(batch.BatchId, Debit.BatchStatus.SubmittedWaitingOutbox);
        }
      }
    }

    public List<DBT_ControlDTO> GetControlsToAVS()
    {
      using (var uow = new UnitOfWork())
      {
        var avsBankIds = new List<long>();
        var avsBanks = new Atlas.Enumerators.General.BankName[]{
            Atlas.Enumerators.General.BankName.STD, 
            Atlas.Enumerators.General.BankName.FNB, 
            Atlas.Enumerators.General.BankName.CAP, 
            Atlas.Enumerators.General.BankName.NED,
            Atlas.Enumerators.General.BankName.ABS};
        avsBanks.ToList().ForEach(type =>
        {
          avsBankIds.Add((long)type);
        });
        var controls = new XPQuery<DBT_Control>(uow).Where(c => c.AVSCheckType.Type == Debit.AVSCheckType.ControlCreation
          && c.AVSTransactionId == 0 && avsBankIds.Contains(c.Bank.BankId)).ToList();
        return AutoMapper.Mapper.Map<List<DBT_Control>, List<DBT_ControlDTO>>(controls);
      }
    }

    /// <summary>
    /// Updates Control with Avs TransactionId
    /// </summary>
    /// <param name="controlAvs">key = controlId, value = avsTransactionId</param>
    public void UpdateAVSIds(Dictionary<long, long> controlAvs)
    {
      using (var uow = new UnitOfWork())
      {
        var controlIds = controlAvs.Select(a => a.Key).ToList();
        var controls = new XPQuery<DBT_Control>(uow).Where(c => controlIds.Contains(c.ControlId) && c.AVSTransactionId == 0).ToList();
        controls.ForEach(c =>
        {
          c.AVSTransactionId = controlAvs[c.ControlId];
        });

        uow.CommitChanges();
      }
    }

    public void Dispose()
    {
      if (_publicHolidays != null && _publicHolidays.Count > 0)
        _publicHolidays.Clear();
    }

    #endregion

    #region Private Methods

    private void RePrepareNewBatchForExport(long batchId)
    {
      using (var uow = new UnitOfWork())
      {
        var service = new XPQuery<DBT_Service>(uow).FirstOrDefault(s => s.ServiceId == _serviceId);

        var transmissionTransactions = new XPQuery<DBT_TransmissionTransaction>(uow).Where(t => t.Batch.BatchId == batchId && !t.Accepted.HasValue && !t.TransmissionSet.Accepted.HasValue && !t.TransmissionSet.Transmission.Accepted.HasValue).OrderBy(t => t.SequenceNo).ToList();
        var transmissionSets = transmissionTransactions.Select(t => t.TransmissionSet).Distinct().OrderBy(t => t.GenerationNo).ToList();
        var transmission = transmissionSets.Select(t => t.Transmission).Distinct().OrderByDescending(t => t.CreateDate).FirstOrDefault();

        transmission.TransmissionNo = service.NextTransmissionNo;
        int generationNo = service.NextGenerationNo;
        int sequenceNo = GetNextSequenceNo(service);
        foreach (var transmissionSet in transmissionSets)
        {
          transmissionSet.GenerationNo = generationNo;
          generationNo++;
          foreach (var transmissionTransaction in transmissionTransactions)
          {
            transmissionTransaction.SequenceNo = sequenceNo;
            sequenceNo++;
          }
        }

        //service.NextSequenceNo = sequenceNo;
        //service.LastSequenceUpdate = DateTime.Today;

        uow.CommitChanges();
      }
    }

    private bool IsValidActionDate(DateTime actionDate)
    {
      return (actionDate.DayOfWeek != DayOfWeek.Sunday
        && !_publicHolidays.Contains(actionDate.Date));
    }

    private DateTime GetNextActionDate(DateTime afterThisDate)
    {
      var validDate = false;
      var nextActionDate = afterThisDate;
      if (DateTime.Today.DayOfWeek == DayOfWeek.Saturday)
        nextActionDate = nextActionDate.AddDays(3);
      else
        nextActionDate = nextActionDate.AddDays(2);

      while (!validDate)
      {
        if (nextActionDate.DayOfWeek != DayOfWeek.Sunday
          && !_publicHolidays.Contains(nextActionDate.Date))
          validDate = true;
        else
          nextActionDate = nextActionDate.AddDays(1);
      }
      return nextActionDate;
    }

    private void CreateNewTransaction(UnitOfWork uow, DBT_Transaction lastTransaction, int repetition)
    {
      var newTransaction = new DBT_Transaction(uow);
      newTransaction.Control = lastTransaction.Control;
      newTransaction.Control.CurrentRepetition = repetition;
      newTransaction.DebitType = new XPQuery<DBT_DebitType>(uow).FirstOrDefault(d => d.Type == Debit.DebitType.Regular); // Confirm
      newTransaction.Status = new XPQuery<DBT_Status>(uow).FirstOrDefault(d => d.Type == Debit.Status.New);
      newTransaction.LastStatusDate = DateTime.Now;
      newTransaction.Amount = 0;
      newTransaction.InstalmentDate = Atlas.LoanEngine.DebitOrder.ActionDateCalculation.CalculateNextInstalmentDate(lastTransaction.InstalmentDate, lastTransaction.Control.PeriodFrequency.Type);
      newTransaction.ActionDate = Atlas.LoanEngine.DebitOrder.ActionDateCalculation.CalculateActionDate(_publicHolidays, lastTransaction.Control.PayRule.Type, newTransaction.InstalmentDate);
      newTransaction.Repetition = repetition;
      newTransaction.CreateDate = DateTime.Now;
    }

    private void UpdateTransactionActionDate(ref DBT_Transaction transaction, bool checkPublicHolidayOnly = false)
    {
      var actionDate = Atlas.LoanEngine.DebitOrder.ActionDateCalculation.CalculateActionDate(_publicHolidays, transaction.Control.PayRule.Type, transaction.InstalmentDate);

      while (_publicHolidays.Contains(actionDate) || actionDate.DayOfWeek == DayOfWeek.Sunday)
      {
        actionDate = actionDate.AddDays(-1);
        transaction.OverrideTrackingDays++;
      }
      transaction.ActionDate = actionDate;
    }

    private List<DBT_ControlDTO> ValidateOrder(List<DBT_ControlDTO> controlsToValidate)
    {
      using (var uow = new UnitOfWork())
      {
        var controlIds = controlsToValidate.Select(c => c.ControlId).ToList();
        var controls = new XPQuery<DBT_Control>(uow).Where(d => controlIds.Contains(d.ControlId)).ToList();

        foreach (var control in controls)
        {
          control.IsValid = true;

          var controlValidations = new XPQuery<DBT_ControlValidation>(uow).Where(v => v.Control.ControlId == control.ControlId).ToList();
          controlValidations.ForEach((p) =>
          {
            uow.Delete(p);
          });

          var avsBanks = new Atlas.Enumerators.General.BankName[]{
            Atlas.Enumerators.General.BankName.STD, 
            Atlas.Enumerators.General.BankName.FNB, 
            Atlas.Enumerators.General.BankName.CAP, 
            Atlas.Enumerators.General.BankName.NED,
            Atlas.Enumerators.General.BankName.ABS};
          if (avsBanks.Contains(control.Bank.Type))
          {
            if (control.AVSCheckType.Type == Debit.AVSCheckType.ControlCreation)
            {
              var avsEnquiry = new XPQuery<AVS_Transaction>(uow).FirstOrDefault(t => t.TransactionId == control.AVSTransactionId && t.Enabled);

              if (avsEnquiry == null)
              {
                control.IsValid = false;
                var validation = new DBT_ControlValidation(uow);
                validation.Control = control;
                validation.Validation = new XPQuery<DBT_Validation>(uow).FirstOrDefault(d => d.Type == Debit.ValidationType.AVSDoesNotExistOrPending);
              }
              else
              {
                if (avsEnquiry.Result == null || avsEnquiry.Result.Type == AVS.Result.NoResult || avsEnquiry.Result.Type == AVS.Result.Failed)
                {
                  if (avsEnquiry.Result != null)
                  {
                    if (!(avsEnquiry.ResponseAcceptsDebit == "00"
                      && avsEnquiry.ResponseAccountNumber == "00"
                      && avsEnquiry.ResponseAccountOpen == "00"
                      && avsEnquiry.ResponseIdNumber == "00"))
                    {
                      control.IsValid = false;
                      var validation = new DBT_ControlValidation(uow);
                      validation.Control = control;
                      validation.Validation = new XPQuery<DBT_Validation>(uow).FirstOrDefault(d => d.Type == Debit.ValidationType.BankAccountInvalidOrInactive);
                    }
                  }
                  else
                  {
                    control.IsValid = false;
                    var validation = new DBT_ControlValidation(uow);
                    validation.Control = control;
                    validation.Validation = new XPQuery<DBT_Validation>(uow).FirstOrDefault(d => d.Type == Debit.ValidationType.AVSDoesNotExistOrPending);
                  }
                }
              }
            }
          }

          if (control.FirstInstalmentDate.Date <= DateTime.Today.AddDays(1).Date && !control.IsValid)
          {
            control.ControlStatus = new XPQuery<DBT_ControlStatus>(uow).FirstOrDefault(c => c.Type == Debit.ControlStatus.Cancelled_ValidationErrors);
            control.LastStatusDate = DateTime.Now;
          }
        }

        uow.CommitChanges();
        uow.PurgeDeletedObjects();

        return AutoMapper.Mapper.Map<List<DBT_Control>, List<DBT_ControlDTO>>(controls.Where(c => c.IsValid).ToList());
      }
    }

    private int GetNextSequenceNo(DBT_Service service)
    {
      if (service.LastSequenceUpdate.Date == DateTime.Today.Date)
        return (service.NextSequenceNo);
      else
        return 1; // First Sequence
    }

    private void TryCompletingControls(List<long> controlIds)
    {
      using (var uow = new UnitOfWork())
      {
        var controls = new XPQuery<DBT_Control>(uow).Where(c => controlIds.Contains(c.ControlId) && c.CurrentRepetition == c.Repetitions).ToList();
        var transactions = new XPQuery<DBT_Transaction>(uow).Where(t => controls.Contains(t.Control) && t.Status.StatusId < (int)Debit.Status.Successful).ToList();
        DBT_ControlStatus completedControlStatus = null;
        foreach (var control in controls)
        {
          var canComplete = transactions.Where(t => t.Control == control).Count() == 0;
          if (canComplete)
          {
            if (completedControlStatus == null)
              completedControlStatus = new XPQuery<DBT_ControlStatus>(uow).FirstOrDefault(c => c.Type == Debit.ControlStatus.Completed);
            UpdateControlStatus(uow, control, completedControlStatus);
          }
        }

        uow.CommitChanges();
      }
    }

    private void UpdateControlStatus(UnitOfWork uow, DBT_Control control, DBT_ControlStatus controlStatus)
    {
      control.ControlStatus = controlStatus;
      control.LastStatusDate = DateTime.Now;
    }

    #endregion
  }
}