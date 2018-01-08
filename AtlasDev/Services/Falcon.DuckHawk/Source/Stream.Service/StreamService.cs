using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Ass.Framework.Repository;
using Atlas.Ass.Framework.Structures.Stream;
using Atlas.Common.Extensions;
using Atlas.Common.Utils;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Serilog;
using Stream.Framework.DataContracts.Requests;
using Stream.Framework.Repository;
using Stream.Framework.Services;
using Stream.Framework.Structures;
using Stream.Structures.Models;
using Action = Stream.Framework.Enumerators.Action;
using CaseStatus = Stream.Framework.Enumerators.CaseStatus;
using Category = Stream.Framework.Enumerators.Category;

namespace Stream.Service
{
  public class StreamService : IStreamService
  {
    private readonly ILogger _logger;
    private readonly IStreamRepository _streamRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAssGeneralRepository _assGeneralRepository;
    private readonly IAssStreamRepository _assStreamRepository;
    private readonly IGeneralRepository _generalRepository;
    private readonly IConfigService _configService;

    public StreamService(ILogger logger, IStreamRepository streamRepository, IUserRepository userRepository,
      IAssGeneralRepository assGeneralRepository, IAssStreamRepository assStreamRepository,
      IGeneralRepository generalRepository, IConfigService configService)
    {
      _logger = logger;
      _streamRepository = streamRepository;
      _userRepository = userRepository;
      _assGeneralRepository = assGeneralRepository;
      _assStreamRepository = assStreamRepository;
      _generalRepository = generalRepository;
      _configService = configService;
    }

    public void ImportNewCollectionAccounts()
    {
      ImportNewAccounts(Framework.Enumerators.Stream.GroupType.Collections,
        Framework.Enumerators.Stream.SubCategory.Arrears_Default);
    }

    public void ImportNewSaleAccounts()
    {
      ImportNewAccounts(Framework.Enumerators.Stream.GroupType.Sales,
        Framework.Enumerators.Stream.SubCategory.Sales_Existing);
    }

    public void ImportSalesTransactions()
    {
      ImportTransactions(Framework.Enumerators.Stream.GroupType.Sales);
    }

    public void ImportCollectionsTransactions()
    {
      ImportTransactions(Framework.Enumerators.Stream.GroupType.Collections);
    }

    public void CloseUpToDateCollectionCases(General.Host host)
    {
      _streamRepository.CloseCasesWithoutArrears(host, Framework.Enumerators.Stream.GroupType.Collections);
    }

    public void ClosePaidupAccounts()
    {
      var cases = _streamRepository.GetCasesByStatus(Framework.Enumerators.Stream.GroupType.Collections,
        CaseStatus.Type.New, CaseStatus.Type.OnHold,
        CaseStatus.Type.InProgress);

      // list of cases where an account of the linked case has been closed - we have to update the case
      var dirtyCases = new Dictionary<ICase, string>();

      foreach (var @case in cases)
      {
        var accounts = _streamRepository.GetAccountsByCaseId(@case.CaseId);

        foreach (var account in accounts)
        {
          var assClosedAccounts = _assStreamRepository.GetClosedAccounts(account.Reference2).FirstOrDefault();
          if (assClosedAccounts != null)
          {
            if (!dirtyCases.ContainsKey(@case))
            {
              dirtyCases.Add(@case, assClosedAccounts.Status);
            }

            _streamRepository.AddOrUpdateAccount(new AddOrUpdateAccountRequest
            {
              DebtorId = account.Debtor.DebtorId,
              HostId = account.HostId,
              LoanAmount = account.LoanAmount,
              LoanDate = account.LoanDate,
              CloseDate = assClosedAccounts.PaidDate,
              OpenDate = account.LoanDate,
              Reference = account.Reference,
              Reference2 = account.Reference2,
              LoanTerm = account.LoanTerm,
              Frequency = account.Frequency.Type,
              AccountId = account.AccountId,
              LastReceiptDate = account.LastReceiptDate,
              BranchId = account.Branch.BranchId,
              LastReceiptAmount = account.LastReceiptAmount,
              LastImportReference = account.LastImportReference,
              ArrearsAmount = account.ArrearsAmount,
              RequiredPayment = account.RequiredPayment,
              InstalmentsOutstanding = account.InstalmentsOutstanding,
              Balance = account.Balance,
              UpToDate = true
            });
          }
        }
      }

      foreach (var @case in dirtyCases)
      {
        if (dirtyCases.Any(c => c.Value.ToLower() == "h"))
        {
          CalculateCaseBalance(@case.Key.CaseId, Framework.Enumerators.Stream.GroupType.Collections,
            CaseStatus.Type.HandedOver);
        }
        else
        {
          CalculateCaseBalance(@case.Key.CaseId, Framework.Enumerators.Stream.GroupType.Collections);
        }
      }
    }

    public void CheckAllCollectionPtps()
    {
      var ptpDates = new List<DateTime>();
      for (var i = 0; i < 4; i++)
        ptpDates.Add(DateTime.Today.AddDays(-i));

      var ptpsDue = _streamRepository.GetCaseStreamActions(Framework.Enumerators.Stream.GroupType.Collections,
        null, ptpDates,
        new[] { Action.Type.Action },
        new[]
        {
          CaseStatus.Type.New,
          CaseStatus.Type.InProgress,
          CaseStatus.Type.OnHold
        },
        Framework.Enumerators.Stream.StreamType.PTP);

      foreach (var ptpDue in ptpsDue.Where(p => p.Amount.HasValue))
      {
        var accountPayments = _assStreamRepository.GetPaymentsOnSpecificDate(ptpDates.ToArray(),
          ptpDue.AccountReference2.ToArray());

        if (ptpDue.Amount != null && IsPtpSuccessful(ptpDue.Amount.Value, accountPayments.Sum(p => p.Amount)))
        {
          // ptp successful
          _streamRepository.CompletePtp(caseStreamActionId: ptpDue.CaseStreamActionId, success: true,
            personId: (long)General.Person.System);
        }
        else
        {
          // ptp unsuccessful
          // PTP not yet completed - Add Reminder + increase priority
          _streamRepository.AddCaseStreamActionWithPriority(caseStreamId: ptpDue.CaseStreamId,
            personId: (long)General.Person.System,
            actionDate: DateTime.Today.AddDays(1).AddHours(8), actionType: Action.Type.Reminder,
            allowMultipleActionTypes: true, completePendingActions: false,
            prirotyType: Framework.Enumerators.Stream.PriorityType.High);
        }
      }

      // get older ptps and break them
      ptpDates.Clear();
      for (var i = 4; i < 14; i++)
      {
        var ptpDueDate = DateTime.Today.AddDays(-i);

        ptpsDue = _streamRepository.GetCaseStreamActions(Framework.Enumerators.Stream.GroupType.Collections,
          null, new List<DateTime> { ptpDueDate },
          new[] { Action.Type.Action },
          new[]
        {
          CaseStatus.Type.New,
          CaseStatus.Type.InProgress,
          CaseStatus.Type.OnHold
        },
          Framework.Enumerators.Stream.StreamType.PTP);
        foreach (var ptpDue in ptpsDue.Where(p => p.Amount.HasValue))
        {
          _streamRepository.CompletePtp(caseStreamActionId: ptpDue.CaseStreamActionId, success: false,
            personId: (long)General.Person.System);
        }
      }
    }

    public bool CheckForPtpPayment(long caseStreamId)
    {
      var caseStreamAction = _streamRepository.GetNextAction(caseStreamId, Action.Type.Action);
      if (caseStreamAction == null || !caseStreamAction.Amount.HasValue)
      {
        return false;
      }

      var accountReference2List = _streamRepository.GetAccountsReference2ByCaseStreamId(caseStreamId);
      var payments = _assStreamRepository.GetPaymentsOnSpecificDate(new[] { caseStreamAction.ActionDate },
        accountReference2List.ToArray());

      if (IsPtpSuccessful(caseStreamAction.Amount.Value, payments.Sum(p => p.Amount)))
      {
        // ptp successful
        _streamRepository.CompletePtp(caseStreamActionId: caseStreamAction.CaseStreamActionId, success: true,
          personId: (long)General.Person.System);
        return true;
      }
      return false;
    }

    private bool IsPtpSuccessful(decimal amountDue, decimal ptpAmount)
    {
      // 85% buffer for making it a successful ptp
      return (amountDue * 85 / 100 <= ptpAmount);
    }

    public void RemoveAssArrearClientsFromSalesStream()
    {
      var branches = _generalRepository.GetBranchIds();
      foreach (var branch in branches)
      {
        var clientReferences = _streamRepository.GetClientReferencesByAccountStatus(
          new[] { Framework.Enumerators.Stream.GroupType.Sales },
          branch, CaseStatus.Type.InProgress,
          CaseStatus.Type.New, CaseStatus.Type.OnHold);

        foreach (var clientReference in clientReferences)
        {
          var handedOver = _assGeneralRepository.CheckHandoverClients(clientReference);
          if (handedOver.Any())
          {
            var debtor = _streamRepository.GetDebtorByReference(clientReference);
            var cases = _streamRepository.GetCasesByDebtorId(debtor.DebtorId,
              Framework.Enumerators.Stream.GroupType.Sales, CaseStatus.Type.InProgress, CaseStatus.Type.New,
              CaseStatus.Type.OnHold);
            foreach (var @case in cases)
            {
              var caseStream = _streamRepository.GetOpenCaseStreamForCase(@case.CaseId);
              if (caseStream == null)
              {
                _logger.Fatal(
                  $"Open Case stream for Case {@case.CaseId} does not exist. Cannot remove handed over client from sales bucket");
              }
              else
              {
                _streamRepository.MoveCaseToStream(caseStreamId: caseStream.CaseStreamId,
                  personId: General.Person.System.ToInt(), newStream: Framework.Enumerators.Stream.StreamType.Completed,
                  completedCommentId: Framework.Enumerators.Stream.Comment.PaidUpOnAss.ToInt(),
                  completeNote: string.Empty, makeDefaultAction: true,
                  newCaseStatusType: CaseStatus.Type.HandedOver);
              }
            }
          }
        }
      }
    }

    public void RemoveAssClientsWithNewLoansFromSalesStream()
    {
      var branches = _generalRepository.GetBranchIds();
      foreach (var branch in branches)
      {
        var clientLoanDates =
          _streamRepository.GetClientReferencesAndLoanDateByAccountStatus(
            new[] { Framework.Enumerators.Stream.GroupType.Sales }, branch, CaseStatus.Type.InProgress,
            CaseStatus.Type.New, CaseStatus.Type.OnHold);

        foreach (var clientReference in clientLoanDates)
        {
          var clientHasNewerLoan = _assGeneralRepository.CheckClientsWithNewLoans(clientReference.Key,
            clientReference.Value);
          if (clientHasNewerLoan)
          {
            var debtor = _streamRepository.GetDebtorByReference(clientReference.Key);
            var cases = _streamRepository.GetCasesByDebtorId(debtor.DebtorId,
              Framework.Enumerators.Stream.GroupType.Sales, CaseStatus.Type.InProgress, CaseStatus.Type.New,
              CaseStatus.Type.OnHold);
            foreach (var @case in cases)
            {
              var caseStream = _streamRepository.GetOpenCaseStreamForCase(@case.CaseId);
              if (caseStream == null)
              {
                _logger.Fatal(
                  $"Open Case stream for Case {@case.CaseId} does not exist. Cannot remove client with newer loan from sales bucket");
              }
              else
              {
                _streamRepository.MoveCaseToStream(caseStreamId: caseStream.CaseStreamId,
                  personId: General.Person.System.ToInt(), newStream: Framework.Enumerators.Stream.StreamType.Completed,
                  completedCommentId:
                    caseStream.StreamType == Framework.Enumerators.Stream.StreamType.PTC
                      ? Framework.Enumerators.Stream.Comment.PTCSuccesful.ToInt()
                      : Framework.Enumerators.Stream.Comment.ClientTookNewerLoan.ToInt(), completeNote: string.Empty,
                  makeDefaultAction: true,
                  newCaseStatusType: CaseStatus.Type.HandedOver);
              }
            }
          }
        }
      }
    }

    public void RemoveAssDeceaseClients()
    {
      var debtorReferences =
        _streamRepository.GetClientReferencesByAccountStatus(
          new[] { Framework.Enumerators.Stream.GroupType.Sales },
          null, CaseStatus.Type.InProgress,
          CaseStatus.Type.New);
      var deceasedClients = _assGeneralRepository.GetDeceasedClients(debtorReferences);
      _streamRepository.RemoveDeceasedClients(deceasedClients);
    }

    #region private methods

    private void ImportTransactions(Framework.Enumerators.Stream.GroupType groupType)
    {
      var accountsAlreadyInSystem =
        _streamRepository.GetCurrentAccountReferencesAndLastImportReference(groupType).ToList();

      var caseIds = accountsAlreadyInSystem.Select(a => a.CaseId).Distinct().ToList();

      foreach (var caseId in caseIds.AsParallel())
      {
        var caseAccountsAlreadyInSystem = accountsAlreadyInSystem.Where(a => a.CaseId == caseId).ToList();
        var result = caseAccountsAlreadyInSystem.AsParallel()
          .Select(ImportAccountTransaction)
          .FirstOrDefault(a => a > 0);
        if (result <= 0)
        {
          continue;
        }
        SetCasePriority(caseId, groupType);
        SetCaseCategory(caseId, groupType);
        CalculateCaseBalance(caseId, groupType);
        if (groupType == Framework.Enumerators.Stream.GroupType.Collections)
        {
          _streamRepository.CheckAndCloseCaseWithoutArrears(caseId);
        }
      }
    }

    public void RemoveDuplicateAccounts(Framework.Enumerators.Stream.GroupType groupType)
    {
      // get all cases
      var cases =
        _streamRepository.GetCasesByStatus(groupType,
          CaseStatus.Type.Closed, CaseStatus.Type.HandedOver, CaseStatus.Type.InProgress, CaseStatus.Type.New,
          CaseStatus.Type.OnHold);

      _logger.Debug($"Job [StreamRemoveDuplicateAccount][{groupType.ToStringEnum()}] received: {cases.Count}");

      Parallel.ForEach(cases, new ParallelOptions
      {
        MaxDegreeOfParallelism = 15
      }, streamCase =>
      {
        _logger.Debug($"Job [StreamRemoveDuplicateAccount][{groupType.ToStringEnum()}] start - " + streamCase.CaseId);
        try
        {
          // get all account linked to case
          var accounts = _streamRepository.GetAccountsByCaseId(streamCase.CaseId);

          var uniqueAccountReferences = accounts.Select(a => a.Reference).Distinct().ToList();
          _logger.Debug($"Job [StreamRemoveDuplicateAccount][{groupType.ToStringEnum()}] received {accounts.Count} accounts, with {uniqueAccountReferences.Count} unique");

          foreach (var uniqueAccount in uniqueAccountReferences)
          {
            var accountsWithReference = accounts.Where(a => a.Reference.Contains(uniqueAccount)).ToList();
            if (accountsWithReference.Count > 1)
            {
              var accountsToRemove = accountsWithReference.Skip(1).Select(a => a.AccountId).ToList();

              new RawSql().ExecuteScalar(
                string.Format(
                  "DELETE FROM \"STR_Transaction\" WHERE \"AccountId\" IN({0}); DELETE FROM \"STR_Account\" WHERE \"AccountId\" IN ({0});",
                  string.Join(",", accountsToRemove)), _configService.AtlasCoreConnection);
            }
          }

          SetCasePriority(streamCase.CaseId, groupType);
          SetCaseCategory(streamCase.CaseId, groupType);
          CalculateCaseBalance(streamCase.CaseId, groupType);
          if (groupType == Framework.Enumerators.Stream.GroupType.Collections)
          {
            _streamRepository.CheckAndCloseCaseWithoutArrears(streamCase.CaseId);
          }
        }
        catch (Exception ex)
        {
          _logger.Error($"Job [StreamRemoveDuplicateAccount][{groupType.ToStringEnum()}] end - " + streamCase.CaseId + ", " + ex.Message + "," + ex.StackTrace);
        }
      });
    }

    private long ImportAccountTransaction(IAccountLastTransactionImportReference accountLastTransactionImportReference)
    {
      long affectedCaseId = 0;

      try
      {
        DateTime lastImportReferenceId;
        DateTime.TryParse(accountLastTransactionImportReference.LastImportReference, out lastImportReferenceId);
        var accountTransactions =
          _assGeneralRepository.GetAccountTransactions(accountLastTransactionImportReference.Reference2,
            lastImportReferenceId);

        if (accountTransactions.Count > 0)
        {
          affectedCaseId = accountLastTransactionImportReference.CaseId;
        }
        foreach (var accountTransaction in accountTransactions)
        {
          _streamRepository.AddOrUpdateAccountTransaction(new AddOrUpdateAccountTransactionRequest
          {
            Reference = accountTransaction.TransactionReference,
            AccountId = accountLastTransactionImportReference.AccountId,
            TransactionStatus = GetTransactionStatus(accountTransaction.TranasctionStatus),
            Amount = accountTransaction.Amount,
            InstalmentNumber = accountTransaction.Order,
            TransactionDate = accountTransaction.TransactionDate,
            TransactionType = GetTransactionType(accountTransaction.TransactionType)
          });

          if (lastImportReferenceId < accountTransaction.BackupDate)
          {
            _streamRepository.UpdateAccountWithLastImportReference(accountLastTransactionImportReference.AccountId,
              DateUtils.MaxDateTime(accountTransaction.TransactionDate, accountTransaction.StatusDate).ToString());
            lastImportReferenceId = accountTransaction.BackupDate;
          }
        }

        CalculateAccountBalance(accountLastTransactionImportReference.AccountId);
      }
      catch (Exception exception)
      {
        _logger.Error(
          $"Error importing transactions for account ({accountLastTransactionImportReference.AccountId}) - {exception.Message} - {exception.StackTrace}");
      }
      return affectedCaseId;
    }

    private void CalculateAccountBalance(long accountId)
    {
      var transactions = _streamRepository.GetAccountTransactions(accountId);
      var account = _streamRepository.GetAccount(accountId);
      if (account != null)
      {
        account.Balance = account.ArrearsAmount = 0;
        account.InstalmentsOutstanding = 0;
        foreach (var transaction in transactions)
        {
          if (transaction.TransactionType == Framework.Enumerators.Stream.TransactionType.Instalment &&
              !transaction.TransactionStatus.HasValue)
          {
            if (transaction.TransactionDate.Date < DateTime.Today.Date)
            {
              // outstanding payment
              account.ArrearsAmount += transaction.Amount;
              account.InstalmentsOutstanding++;
            }
            account.Balance += transaction.Amount;
          }
          account.RequiredPayment = account.ArrearsAmount;
        }

        var lastPayment = transactions.OrderByDescending(t => t.TransactionDate).FirstOrDefault(t =>
          t.TransactionType == Framework.Enumerators.Stream.TransactionType.Payment &&
          t.TransactionStatus != Framework.Enumerators.Stream.TransactionStatus.Reversal);

        if (lastPayment == null)
        {
          account.LastReceiptDate = null;
          account.LastReceiptAmount = null;
        }
        else
        {
          account.LastReceiptDate = lastPayment.TransactionDate;
          account.LastReceiptAmount = lastPayment.Amount;
        }


        _streamRepository.AddOrUpdateAccount(new AddOrUpdateAccountRequest
        {
          BranchId = account.Branch.BranchId,
          DebtorId = account.Debtor.DebtorId,
          HostId = General.Host.ASS.ToInt(),
          LoanAmount = account.LoanAmount,
          LoanDate = account.LoanDate,
          CloseDate = account.CloseDate,
          OpenDate = account.LoanDate,
          Reference = account.Reference,
          Reference2 = account.Reference2,
          LoanTerm = account.LoanTerm,
          Frequency = account.Frequency.Type,
          ArrearsAmount = account.ArrearsAmount,
          Balance = account.Balance,
          InstalmentsOutstanding = account.InstalmentsOutstanding,
          LastReceiptDate = account.LastReceiptDate,
          RequiredPayment = account.RequiredPayment,
          AccountId = account.AccountId,
          LastImportReference = account.LastImportReference,
          LastReceiptAmount = account.LastReceiptAmount,
          UpToDate = account.ArrearsAmount <= 0
        });
      }
    }

    private void CalculateCaseBalance(long caseId, Framework.Enumerators.Stream.GroupType groupType, CaseStatus.Type? caseStatus = null)
    {
      var streamCase = _streamRepository.GetCase(caseId);
      streamCase.TotalArrearsAmount = 0;
      streamCase.TotalBalance = 0;
      streamCase.TotalInstalmentsOutstanding = 0;
      streamCase.TotalLoanAmount = 0;
      streamCase.TotalRequiredPayment = 0;
      streamCase.LastReceiptAmount = 0;
      streamCase.LastReceiptDate = null;
      var accounts = _streamRepository.GetAccountsByCaseId(caseId);

      foreach (var account in accounts)
      {
        streamCase.TotalArrearsAmount += account.ArrearsAmount;
        streamCase.TotalBalance += account.Balance;
        streamCase.TotalInstalmentsOutstanding += account.InstalmentsOutstanding;
        streamCase.TotalLoanAmount += account.LoanAmount;
        streamCase.TotalRequiredPayment += account.RequiredPayment;
        streamCase.LastReceiptAmount += account.LastReceiptAmount;

        if (!account.LastReceiptDate.HasValue)
          continue;

        if (streamCase.LastReceiptDate.HasValue)
        {
          if (streamCase.LastReceiptDate.Value.Date < account.LastReceiptDate.Value.Date)
          {
            streamCase.LastReceiptDate = account.LastReceiptDate;
          }
        }
        else
        {
          streamCase.LastReceiptDate = account.LastReceiptDate;
        }
      }

      _streamRepository.AddOrUpdateCase(new AddOrUpdateCaseRequest
      {
        Host = streamCase.Host,
        CaseStatus = streamCase.CaseStatusType,
        CompleteDate = streamCase.CompleteDate,
        DebtorId = streamCase.DebtorId,
        Reference = streamCase.Reference,
        LastReceiptDate = streamCase.LastReceiptDate,
        CaseId = streamCase.CaseId,
        GroupType = streamCase.GroupType,
        Priority = streamCase.Priority,
        BranchId = streamCase.BranchId,
        LastReceiptAmount = streamCase.LastReceiptAmount,
        SmsCount = streamCase.SmsCount,
        AllocatedUserId = streamCase.AllocatedUserId,
        LastStatusDate = streamCase.LastStatusDate,
        TotalArrearsAmount = streamCase.TotalArrearsAmount,
        TotalBalance = streamCase.TotalBalance,
        TotalInstalmentsOutstanding = streamCase.TotalInstalmentsOutstanding,
        TotalLoanAmount = streamCase.TotalLoanAmount,
        TotalRequiredPayment = streamCase.TotalRequiredPayment
      });

      #region Close case if there are no accounts in arrears (collections only)

      if (groupType == Framework.Enumerators.Stream.GroupType.Collections)
      {
        var caseStream = _streamRepository.GetOpenCaseStreamForCase(streamCase.CaseId);
        if (caseStream == null)
          return;

        var openAccounts = _streamRepository.GetAccountsByCaseId(streamCase.CaseId, false);
        if (openAccounts.Count == 0)
        {
          _streamRepository.MoveCaseToStream(
            caseStreamId: caseStream.CaseStreamId,
            personId: General.Person.System.ToInt(),
            newStream: Framework.Enumerators.Stream.StreamType.Completed,
            completedCommentId: Framework.Enumerators.Stream.Comment.PaidUpOnAss.ToInt(),
            completeNote: string.Empty,
            makeDefaultAction: true,
            newCaseStatusType: caseStatus ?? CaseStatus.Type.Closed);
        }
      }

      #endregion
    }

    private Framework.Enumerators.Stream.TransactionStatus? GetTransactionStatus(string assTransactionStatus)
    {
      switch (assTransactionStatus)
      {
        case "U":
          return Framework.Enumerators.Stream.TransactionStatus.Reversal;
        case "P":
          return Framework.Enumerators.Stream.TransactionStatus.Paid;
        case "H":
          return Framework.Enumerators.Stream.TransactionStatus.Handover;
        case "T":
          return Framework.Enumerators.Stream.TransactionStatus.PartPayment;
        case "J":
          return Framework.Enumerators.Stream.TransactionStatus.Journal;
        case "C":
          return Framework.Enumerators.Stream.TransactionStatus.Cancel;
        case "W":
          return Framework.Enumerators.Stream.TransactionStatus.WriteOff;
        case "E":
          return Framework.Enumerators.Stream.TransactionStatus.EarlySettlement;
        case "K":
        case "G":
          return Framework.Enumerators.Stream.TransactionStatus.Refund;
        default:
          return null;
      }
    }

    private Framework.Enumerators.Stream.TransactionType GetTransactionType(string transactionType)
    {
      switch (transactionType)
      {
        case "A":
          return Framework.Enumerators.Stream.TransactionType.Adjustment;
        case "C":
          return Framework.Enumerators.Stream.TransactionType.Cancel;
        case "D":
          return Framework.Enumerators.Stream.TransactionType.Discount;
        case "E":
          return Framework.Enumerators.Stream.TransactionType.EarlySettlement;
        case "H":
          return Framework.Enumerators.Stream.TransactionType.Handover;
        case "J":
          return Framework.Enumerators.Stream.TransactionType.Journal;
        case "T":
          return Framework.Enumerators.Stream.TransactionType.PartPayment;
        case "P":
          return Framework.Enumerators.Stream.TransactionType.Payment;
        case "K":
        case "F":
        case "G":
        case "B":
          return Framework.Enumerators.Stream.TransactionType.Refund;
        case "S":
          return Framework.Enumerators.Stream.TransactionType.Reschedules;
        //case "S": return Framework.Enumerators.Stream.TransactionType.Scheduled;
        case "L":
          return Framework.Enumerators.Stream.TransactionType.SplitRepay;
        case "W":
          return Framework.Enumerators.Stream.TransactionType.WriteOff;
        case "R":
          return Framework.Enumerators.Stream.TransactionType.Instalment;
        default:
          throw new Exception("Transaction Type not catered for");
      }
    }

    private void ImportNewAccounts(Framework.Enumerators.Stream.GroupType groupType,
      Framework.Enumerators.Stream.SubCategory defaultSubCategory)
    {
      _logger.Debug("ImportNewAccounts: STARTED _streamRepository.GetCurrentAccountReferences");
      var alreadyInSystem = _streamRepository.GetCurrentAccountReferences(groupType);
      _logger.Debug("ImportNewAccounts: FINISHED _streamRepository.GetCurrentAccountReferences");

      _logger.Debug("ImportNewAccounts: RemoveDeceasedClients");

      var clientLoans = new List<IClientLoan>();
      if (groupType == Framework.Enumerators.Stream.GroupType.Collections)
      {
        clientLoans = RemoveDeceasedClients(
          _assStreamRepository.GetClientLoansCollections(alreadyInSystem.Select(a => a.Value).ToList())).ToList();
      }
      else if (groupType == Framework.Enumerators.Stream.GroupType.Sales)
      {
        var branches = _generalRepository.GetLegacyBranchNumbers();
        clientLoans = RemoveDeceasedClients(
          _assStreamRepository.GetClientLoansSales(alreadyInSystem.Select(a => a.Value).ToList(), branches)).ToList();
      }

      foreach (var clientLoan in clientLoans)
      {
        var debtor = _streamRepository.GetDebtorByIdNumber(clientLoan.IdentityNo.Trim()) ??
                     _streamRepository.AddOrUpdateDebtor(new AddOrUpdateDebtorRequest
                     {
                       Debtor = new Debtor
                       {
                         CreateDate = DateTime.Now,
                         Title = string.IsNullOrEmpty(clientLoan.Title) ? clientLoan.Title : clientLoan.Title.Trim(),
                         FirstName =
                           string.IsNullOrEmpty(clientLoan.FirstName)
                             ? clientLoan.FirstName
                             : clientLoan.FirstName.Trim(),
                         IdNumber =
                           string.IsNullOrEmpty(clientLoan.IdentityNo)
                             ? clientLoan.IdentityNo
                             : clientLoan.IdentityNo.Trim(),
                         LastName =
                           string.IsNullOrEmpty(clientLoan.Surname) ? clientLoan.Surname : clientLoan.Surname.Trim(),
                         OtherName =
                           string.IsNullOrEmpty(clientLoan.OtherName)
                             ? clientLoan.OtherName
                             : clientLoan.OtherName.Trim(),
                         Reference = clientLoan.ClientReference,
                         EmployerCode = clientLoan.EmployerNo,
                         DateOfBirth = clientLoan.DateOfBirth,
                         ThirdPartyReferenceNo = clientLoan.Client
                       }
                     });

        if (debtor == null)
        {
          throw new Exception("ImportNewAccounts: Error Adding/Updating Debtor");
        }

        var account = _streamRepository.AddOrUpdateAccount(new AddOrUpdateAccountRequest
        {
          BranchId = _generalRepository.GetBranchByLegacyBranchNumber(clientLoan.LegacyBranchNumber).BranchId,
          DebtorId = debtor.DebtorId,
          HostId = General.Host.ASS.ToInt(),
          LoanAmount = clientLoan.Cheque,
          LoanDate = clientLoan.LoanDate,
          CloseDate = clientLoan.EndDate,
          OpenDate = clientLoan.LoanDate,
          Reference =
            $"{clientLoan.LegacyBranchNumber}X{clientLoan.Client}X{clientLoan.Loan}",
          Reference2 = clientLoan.LoanReference,
          LoanTerm = clientLoan.LoanTerm,
          Frequency =
            clientLoan.LoanMethod.Trim() == "W"
              ? Framework.Enumerators.Stream.FrequencyType.Weekly
              : clientLoan.LoanMethod.Trim() == "B"
                ? Framework.Enumerators.Stream.FrequencyType.BiWeekly
                : Framework.Enumerators.Stream.FrequencyType.Monthly,
          UpToDate = false
        });

        // Build case
        _logger.Debug($"ImportNewAccounts: Building case for debtor {debtor.DebtorId} with account {account.AccountId}");
        var streamCases = _streamRepository.GetCasesByDebtorId(debtor.DebtorId, groupType, CaseStatus.Type.New, CaseStatus.Type.InProgress, CaseStatus.Type.OnHold);
        var allocatedUser = _userRepository.GetUserByOperatorCode(clientLoan.OperatorCode,
          clientLoan.LegacyBranchNumber) ??
                            _userRepository.GetBranchManager(clientLoan.LegacyBranchNumber) ??
                            _userRepository.GetSystemUser();

        if (allocatedUser == null)
        {
          throw new Exception("Error getting allocated user");
        }

        var streamCase = streamCases.OrderByDescending(c => c.CreateDate).FirstOrDefault();

        if (streamCase == null)
        {
          var branch = _generalRepository.GetBranchByLegacyBranchNumber(clientLoan.LegacyBranchNumber);
          streamCase = _streamRepository.AddOrUpdateCase(new AddOrUpdateCaseRequest
          {
            Host = General.Host.ASS,
            Reference =
              $"{clientLoan.LegacyBranchNumber}X{clientLoan.Client}",
            BranchId = branch.BranchId,
            DebtorId = debtor.DebtorId,
            AllocatedUserId = allocatedUser.PersonId,
            CaseStatus = CaseStatus.Type.New,
            GroupType = groupType,
            LastStatusDate = DateTime.Now,
            SmsCount = 0,
            SubCategory = defaultSubCategory,
            Priority = Framework.Enumerators.Stream.PriorityType.Normal
          });
        }

        // Set Sub Category
        streamCase = _streamRepository.AddOrUpdateCase(new AddOrUpdateCaseRequest
        {
          Host = streamCase.Host,
          CaseId = streamCase.CaseId,
          DebtorId = debtor.DebtorId,
          AllocatedUserId = allocatedUser.PersonId,
          GroupType = groupType,
          LastStatusDate = DateTime.Now,
          SmsCount = 0,
          LastReceiptAmount = streamCase.LastReceiptAmount,
          LastReceiptDate = streamCase.LastReceiptDate,
          TotalArrearsAmount = streamCase.TotalArrearsAmount,
          CompleteDate = streamCase.CompleteDate,
          TotalLoanAmount = streamCase.TotalLoanAmount,
          TotalBalance = streamCase.TotalBalance,
          TotalInstalmentsOutstanding = streamCase.TotalInstalmentsOutstanding,
          TotalRequiredPayment = streamCase.TotalRequiredPayment,
          BranchId = streamCase.BranchId,
          Reference = streamCase.Reference,
          Priority = streamCase.Priority
        });

        var caseStream = _streamRepository.GetOpenCaseStreamForCase(streamCase.CaseId);
        if (caseStream == null)
        {
          caseStream = _streamRepository.AddOrUpdateCaseStream(new AddOrUpdateCaseStreamRequest
          {
            EscalationType = Framework.Enumerators.Stream.EscalationType.None,
            CaseStreamId = 0,
            CompleteDate = null,
            CaseId = streamCase.CaseId,
            CreateUserId = General.Person.System.ToInt(),
            StreamType = Framework.Enumerators.Stream.StreamType.New,
            CompleteCommentId = 0,
            CompleteNote = string.Empty,
            CompletedUserId = 0,
            LastPriorityDate = DateTime.Now,
            PriorityType = Framework.Enumerators.Stream.PriorityType.Normal
          });

          _streamRepository.AddOrUpdateCaseStreamAllocation(new AddOrUpdateCaseStreamAllocationRequest
          {
            EscalationType = Framework.Enumerators.Stream.EscalationType.None,
            CaseStreamId = caseStream.CaseStreamId,
            CompleteDate = null,
            CaseStreamAllocationId = 0,
            TransferredOut = false,
            TransferredIn = false,
            NoActionCount = 0,
            SmsCount = 0,
            AllocatedDate = DateTime.Now,
            AllocatedUserId = allocatedUser.PersonId,
            TransferredOutDate = null,
            CompleteCommentId = 0
          });

          _streamRepository.AddOrUpdateCaseStreamEscalation(new AddOrUpdateCaseStreamEscalationRequest
          {
            EscalationType = Framework.Enumerators.Stream.EscalationType.None,
            CaseStreamId = caseStream.CaseStreamId,
            CaseStreamEscalationId = 0
          });

          _streamRepository.AddOrUpdateCaseStreamAction(new AddOrUpdateCaseStreamActionRequest
          {
            ActionDate = DateTime.Now.Hour >= 10 ? DateTime.Today.AddDays(1).AddHours(8) : DateTime.Now.AddMinutes(30),
            ActionType = Action.Type.Normal,
            Amount = null,
            IsSuccess = null,
            CompleteDate = null,
            CaseStreamId = caseStream.CaseStreamId,
            CaseStreamActionId = 0,
            DateActioned = null
          });
        }

        #region Contact

        #region Contact Cell

        _streamRepository.AddOrUpdateDebtorContact(new AddOrUpdateDebtorContactRequest
        {
          DebtorId = debtor.DebtorId,
          CreateUserId = General.Person.System.ToInt(),
          IsActive = true,
          ContactType = General.ContactType.CellNo,
          Value = clientLoan.Cell,
          DebtorContactId = 0
        });

        #endregion

        #region Contact Email

        _streamRepository.AddOrUpdateDebtorContact(new AddOrUpdateDebtorContactRequest
        {
          DebtorId = debtor.DebtorId,
          CreateUserId = General.Person.System.ToInt(),
          IsActive = true,
          ContactType = General.ContactType.Email,
          Value = clientLoan.Email,
          DebtorContactId = 0
        });

        #endregion

        #region Contact Spouse Work Tel

        _streamRepository.AddOrUpdateDebtorContact(new AddOrUpdateDebtorContactRequest
        {
          DebtorId = debtor.DebtorId,
          CreateUserId = General.Person.System.ToInt(),
          IsActive = true,
          ContactType = General.ContactType.TelNoWork,
          Value = clientLoan.SpouseWorkTel,
          DebtorContactId = 0
        });

        #endregion

        #region Contact Work Tel

        _streamRepository.AddOrUpdateDebtorContact(new AddOrUpdateDebtorContactRequest
        {
          DebtorId = debtor.DebtorId,
          CreateUserId = General.Person.System.ToInt(),
          IsActive = true,
          ContactType = General.ContactType.TelNoWork,
          Value = clientLoan.WorkTel,
          DebtorContactId = 0
        });

        #endregion

        #region Contact Work Fax

        _streamRepository.AddOrUpdateDebtorContact(new AddOrUpdateDebtorContactRequest
        {
          DebtorId = debtor.DebtorId,
          CreateUserId = General.Person.System.ToInt(),
          IsActive = true,
          ContactType = General.ContactType.TelNoWorkFax,
          Value = clientLoan.WorkFax,
          DebtorContactId = 0
        });

        #endregion

        #region Contact Home Tel

        _streamRepository.AddOrUpdateDebtorContact(new AddOrUpdateDebtorContactRequest
        {
          DebtorId = debtor.DebtorId,
          CreateUserId = General.Person.System.ToInt(),
          IsActive = true,
          ContactType = General.ContactType.TelNoHome,
          Value = clientLoan.HomeTel,
          DebtorContactId = 0
        });

        #endregion

        #endregion

        #region Address

        #region Residential

        _streamRepository.AddOrUpdateDebtorAddress(new AddOrUpdateDebtorAddressRequest
        {
          DebtorId = debtor.DebtorId,
          IsActive = true,
          CreateUserId = General.Person.System.ToInt(),
          AddressType = General.AddressType.Residential,
          DebtorAddressId = 0,
          Line1 = clientLoan.ResidentialAddress1,
          PostalCode = clientLoan.ResidentialAddressPostalCode,
          Line2 = clientLoan.ResidentialAddress2,
          Line3 = clientLoan.ResidentialAddress3
        });

        #endregion

        #region Work

        _streamRepository.AddOrUpdateDebtorAddress(new AddOrUpdateDebtorAddressRequest
        {
          DebtorId = debtor.DebtorId,
          IsActive = true,
          CreateUserId = General.Person.System.ToInt(),
          AddressType = General.AddressType.Residential,
          DebtorAddressId = 0,
          Line1 = clientLoan.WorkAddress1,
          PostalCode = clientLoan.WorkAddressPostalCode,
          Line2 = clientLoan.WorkAddress2,
          Line3 = clientLoan.WorkAddress3,
          Line4 = clientLoan.WorkAddress4
        });

        #endregion

        #endregion
      }
    }

    private IEnumerable<IClientLoan> RemoveDeceasedClients(ICollection<IClientLoan> clientLoans)
    {
      var deceasedClients = _assGeneralRepository.GetDeceasedClients(clientLoans.Select(c => c.ClientReference).ToList());
      deceasedClients.ForEach(deceasedclientReference =>
      {
        clientLoans.Remove(clientLoans.FirstOrDefault(c => c.ClientReference == deceasedclientReference));
      });

      return clientLoans;
    }

    public void SetCaseCategory(long caseId, Framework.Enumerators.Stream.GroupType groupType)
    {
      var streamCase = _streamRepository.GetCase(caseId);

      if (streamCase == null)
        return;

      var accounts = _streamRepository.GetAccountsByCaseId(streamCase.CaseId).ToList();

      foreach (var account in accounts)
      {
        var firstAccont = accounts.IndexOf(account) == 0;

        var transactions = _streamRepository.GetAccountTransactions(account.AccountId);

        if (groupType == Framework.Enumerators.Stream.GroupType.Collections)
        {
          var outstandingTransactions =
            transactions.Where(
              t =>
                t.TransactionType == Framework.Enumerators.Stream.TransactionType.Instalment &&
                (!t.TransactionStatus.HasValue ||
                 (t.TransactionStatus != Framework.Enumerators.Stream.TransactionStatus.Paid &&
                 t.TransactionStatus != Framework.Enumerators.Stream.TransactionStatus.PartPayment))).OrderBy(t => t.TransactionDate).ToList();

          var oldestOutstandingInstalment = outstandingTransactions.FirstOrDefault();
          if (oldestOutstandingInstalment == null)
            continue;

          var category = GetCategory(oldestOutstandingInstalment.TransactionDate);

          var instalmentNumbersOutstanding =
            outstandingTransactions.Select(t => t.InstalmentNumber).Where(t => t <= 3).Distinct().ToList();

          var subCategoryType = GetCollectionSubCategory(category, instalmentNumbersOutstanding);

          // Set the sub category on the first account we start checking
          // or we set the subcategory on the priority of the subcategory
          if (firstAccont)
          {
            streamCase.SubCategoryType = subCategoryType;
          }
          else if (subCategoryType.ToInt() < streamCase.SubCategoryType.ToInt())
          {
            streamCase.SubCategoryType = subCategoryType;
          }
        }
        else if (groupType == Framework.Enumerators.Stream.GroupType.Sales)
        {
          var subCategoryType = GetSalesSubCategory(account.CloseDate);
          if (firstAccont)
          {
            streamCase.SubCategoryType = subCategoryType;
          }
          else if (subCategoryType.ToInt() > streamCase.SubCategoryType.ToInt())
          {
            streamCase.SubCategoryType = subCategoryType;
          }
        }
      }

      _streamRepository.AddOrUpdateCase(new AddOrUpdateCaseRequest
      {
        Host = streamCase.Host,
        CaseId = streamCase.CaseId,
        DebtorId = streamCase.DebtorId,
        AllocatedUserId = streamCase.AllocatedUserId,
        GroupType = groupType,
        LastStatusDate = DateTime.Now,
        SmsCount = 0,
        SubCategory = streamCase.SubCategoryType,
        LastReceiptAmount = streamCase.LastReceiptAmount,
        LastReceiptDate = streamCase.LastReceiptDate,
        TotalArrearsAmount = streamCase.TotalArrearsAmount,
        CompleteDate = streamCase.CompleteDate,
        TotalLoanAmount = streamCase.TotalLoanAmount,
        TotalBalance = streamCase.TotalBalance,
        TotalInstalmentsOutstanding = streamCase.TotalInstalmentsOutstanding,
        TotalRequiredPayment = streamCase.TotalRequiredPayment,
        BranchId = streamCase.BranchId,
        Reference = streamCase.Reference,
        Priority = streamCase.Priority
      });
    }

    public void SetCasePriority(long caseId, Framework.Enumerators.Stream.GroupType groupType)
    {
      if (groupType == Framework.Enumerators.Stream.GroupType.Collections)
      {
        var streamCase = _streamRepository.GetCase(caseId);

        if (streamCase == null)
          return;

        var accounts = _streamRepository.GetAccountsByCaseId(streamCase.CaseId).ToList();
        var priority = Framework.Enumerators.Stream.PriorityType.Normal;
        foreach (var account in accounts)
        {
          var transactions = _streamRepository.GetAccountTransactions(account.AccountId);

          var outstandingTransactions =
            transactions.Where(
              t =>
                t.TransactionType == Framework.Enumerators.Stream.TransactionType.Instalment &&
                (!t.TransactionStatus.HasValue ||
                 (t.TransactionStatus != Framework.Enumerators.Stream.TransactionStatus.Paid &&
                  t.TransactionStatus != Framework.Enumerators.Stream.TransactionStatus.PartPayment)))
              .OrderBy(t => t.TransactionDate)
              .ToList();

          if (
            outstandingTransactions.Any(
              t => t.TransactionDate.Date <= DateTime.Today && t.TransactionDate.AddWorkdays(5) >= DateTime.Today))
          {
            priority = Framework.Enumerators.Stream.PriorityType.High;
          }
        }

        _streamRepository.AddOrUpdateCase(new AddOrUpdateCaseRequest
        {
          Host = streamCase.Host,
          CaseId = streamCase.CaseId,
          DebtorId = streamCase.DebtorId,
          AllocatedUserId = streamCase.AllocatedUserId,
          GroupType = groupType,
          LastStatusDate = DateTime.Now,
          SmsCount = 0,
          SubCategory = null,
          LastReceiptAmount = streamCase.LastReceiptAmount,
          LastReceiptDate = streamCase.LastReceiptDate,
          TotalArrearsAmount = streamCase.TotalArrearsAmount,
          CompleteDate = streamCase.CompleteDate,
          TotalLoanAmount = streamCase.TotalLoanAmount,
          TotalBalance = streamCase.TotalBalance,
          TotalInstalmentsOutstanding = streamCase.TotalInstalmentsOutstanding,
          TotalRequiredPayment = streamCase.TotalRequiredPayment,
          BranchId = streamCase.BranchId,
          Reference = streamCase.Reference,
          Priority = priority
        });
      }
    }

    private Category.Type GetCategory(DateTime oldestInstalment)
    {
      var lastMonthEndDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddDays(-1);

      if ((lastMonthEndDate - oldestInstalment).Days >= lastMonthEndDate.Day)
      {
        return Category.Type.PossibleHandover;
      }
      if ((lastMonthEndDate - oldestInstalment).Days >= 0)
      {
        return Category.Type.NextPossibleHandover;
      }
      return Category.Type.Arrears;
    }

    private Framework.Enumerators.Stream.SubCategory GetSalesSubCategory(DateTime accountCloseDate)
    {
      return (DateTime.Today.Date - accountCloseDate.Date).TotalDays > 0
        ? Framework.Enumerators.Stream.SubCategory.Sales_Revived
        : Framework.Enumerators.Stream.SubCategory.Sales_Existing;
    }

    private Framework.Enumerators.Stream.SubCategory GetCollectionSubCategory(
      Category.Type category, List<int> outstandingInstalmentNumbers)
    {
      if (outstandingInstalmentNumbers.Intersect(new List<int> { 1, 2, 3 }).Count() == 3)
      {
        return GetCollectionSubCategoryFirst3MissedInstalments(category);
      }
      if (outstandingInstalmentNumbers.Intersect(new List<int> { 1, 2 }).Count() == 2)
      {
        return GetCollectionSubCategoryFirst2MissedInstalments(category);
      }
      if (outstandingInstalmentNumbers.Intersect(new List<int> { 1 }).Count() == 1)
      {
        return GetCollectionSubCategoryFirstMissedInstalment(category);
      }
      return GetCollectionSubCategoryDefault(category);
    }

    private Framework.Enumerators.Stream.SubCategory GetCollectionSubCategoryFirst3MissedInstalments(
      Category.Type category)
    {
      switch (category)
      {
        case Category.Type.PossibleHandover:
          {
            return Framework.Enumerators.Stream.SubCategory.PossibleHandovers_First3InstalmentsMissed;
          }
        case Category.Type.NextPossibleHandover:
          {
            return Framework.Enumerators.Stream.SubCategory.NextPossibleHandovers_First3InstalmentsMissed;
          }
        case Category.Type.Arrears:
          {
            return Framework.Enumerators.Stream.SubCategory.Arrears_First3InstalmentsMissed;
          }
        default:
          throw new Exception("Uncatered for SubCategory For First 3 Instalments Missed");
      }
    }

    private Framework.Enumerators.Stream.SubCategory GetCollectionSubCategoryFirst2MissedInstalments(
      Category.Type category)
    {
      switch (category)
      {
        case Category.Type.PossibleHandover:
          {
            return Framework.Enumerators.Stream.SubCategory.PossibleHandovers_First2InstalmentsMissed;
          }
        case Category.Type.NextPossibleHandover:
          {
            return Framework.Enumerators.Stream.SubCategory.NextPossibleHandovers_First2InstalmentsMissed;
          }
        case Category.Type.Arrears:
          {
            return Framework.Enumerators.Stream.SubCategory.Arrears_First2InstalmentsMissed;
          }
        default:
          throw new Exception("Uncatered for SubCategory For First 2 Instalments Missed");
      }
    }

    private Framework.Enumerators.Stream.SubCategory GetCollectionSubCategoryFirstMissedInstalment(
      Category.Type category)
    {
      switch (category)
      {
        case Category.Type.PossibleHandover:
          {
            return Framework.Enumerators.Stream.SubCategory.PossibleHandovers_FirstInstalmentMissed;
          }
        case Category.Type.NextPossibleHandover:
          {
            return Framework.Enumerators.Stream.SubCategory.NextPossibleHandovers_FirstInstalmentMissed;
          }
        case Category.Type.Arrears:
          {
            return Framework.Enumerators.Stream.SubCategory.Arrears_FirstInstalmentMissed;
          }
        default:
          throw new Exception("Uncatered for SubCategory For First Instalment Missed");
      }
    }

    private Framework.Enumerators.Stream.SubCategory GetCollectionSubCategoryDefault(
      Category.Type category)
    {
      switch (category)
      {
        case Category.Type.PossibleHandover:
          {
            return Framework.Enumerators.Stream.SubCategory.PossibleHandovers_Default;
          }
        case Category.Type.NextPossibleHandover:
          {
            return Framework.Enumerators.Stream.SubCategory.NextPossibleHandovers_Default;
          }
        case Category.Type.Arrears:
          {
            return Framework.Enumerators.Stream.SubCategory.Arrears_Default;
          }
        default:
          throw new Exception("Uncatered for SubCategory");
      }
    }

    #endregion

    #region SMS sending for clients in Arrears

    //private void SendHandoverSms(long caseId, Framework.Enumerators.Category.Type categoryType)
    //{
    //  switch (categoryType)
    //  {
    //    case Framework.Enumerators.Category.Type.Arrears:
    //      SendSms(caseId: caseId, personId: General.Person.System.ToInt(),
    //        smsTemplate: Notification.NotificationTemplate.Stream_SMS_FirstWarning);
    //      break;
    //    case Framework.Enumerators.Category.Type.NextPossibleHandover:
    //      SendSms(caseId: caseId, personId: General.Person.System.ToInt(),
    //        smsTemplate: Notification.NotificationTemplate.Stream_SMS_SecondWarning);
    //      break;
    //    case Framework.Enumerators.Category.Type.PossibleHandover:
    //      SendSms(caseId: caseId, personId: General.Person.System.ToInt(),
    //        smsTemplate: Notification.NotificationTemplate.Stream_SMS_ThirdWarning);
    //      break;
    //  }
    //}

    // <summary>
    // Not enabled, uncomment and refactor if needed
    // </summary>
    // <param name="caseId"></param>
    // <param name="personId"></param>
    // <param name="smsTemplate"></param>
    //private void SendSms(long caseId, long personId, Notification.NotificationTemplate smsTemplate)
    //{
    //var blockedSmsTemplates = new[]
    //{
    //  Notification.NotificationTemplate.Stream_SMS_Initiate, 
    //  Notification.NotificationTemplate.Stream_SMS_PaymentThanks
    //};

    //if (blockedSmsTemplates.Contains(smsTemplate))
    //  return;

    //using (var uow = new UnitOfWork())
    //{
    //  var accountCase = new XPQuery<STR_Case>(uow).FirstOrDefault(a => a.CaseId == caseId);
    //  if (accountCase == null)
    //    throw new Exception(string.Format("Case with Id {0} does not exist", caseId));

    //  var createUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
    //  if (createUser == null)
    //    throw new Exception(string.Format("Person {0} does not exist", personId));

    //  var cellContact = accountCase.Account.Debtor.Contacts.Where(c =>
    //    c.Contact.ContactType.Type == General.ContactType.CellNo).OrderBy(c => c.CreateDate).FirstOrDefault();
    //  if (cellContact != null)
    //  {
    //    var template = new XPQuery<NTF_Template>(uow).FirstOrDefault(t => t.TemplateType.Type == smsTemplate);
    //    if (template == null)
    //    {
    //      new STR_Note(uow)
    //      {
    //        Account = accountCase.Account,
    //        AccountNoteType = new XPQuery<STR_AccountNoteType>(uow).FirstOrDefault(a => a.AccountNoteType == Framework.Enumerators.Stream.AccountNoteType.Normal),
    //        Case = accountCase,
    //        CreateDate = DateTime.Now,
    //        CreateUser = createUser,
    //        Note = new NTE_Note(uow)
    //        {
    //          CreateDate = DateTime.Now,
    //          CreateUser = createUser,
    //          Note = string.Format("SMS Sending Failed by [{0} {1}]: SMS template {2} does not exist", createUser.Firstname, createUser.Lastname, smsTemplate.ToStringEnum())
    //        }
    //      };
    //    }
    //    else
    //    {
    //      var templateKeys = new Dictionary<string, string>
    //    {
    //      {
    //        "[CLIENT_NAME]", string.Format("{0} {1} {2}",
    //          accountCase.Account.Debtor.Title, accountCase.Account.Debtor.FirstName,
    //          accountCase.Account.Debtor.LastName)
    //      }
    //    };
    //      var firstOrDefault = accountCase.Account.Branch.Company.Contacts.FirstOrDefault();
    //      if (firstOrDefault != null)
    //        templateKeys.Add("[BRANCH_TEL]", firstOrDefault.Value);

    //      var branchManager = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.Security.IsActive &&
    //        p.Roles.Any(a => a.RoleType.Type == General.RoleType.Branch_Manager) &&
    //        p.Branch == accountCase.Account.Branch);
    //      templateKeys.Add("[BRANCH_MANAGER_NAME]", branchManager == null ? "Branch Manager" : string.Format("{0} {1}", branchManager.Firstname, branchManager.Lastname));

    //      var smsMessage = FillInTheBlanks(template.Template, templateKeys);

    //      //_smsService.Send(cellContact.Contact.Value, smsMessage, Atlas.Enumerators.Notification.NotificationPriority.High);
    //      new STR_Note(uow)
    //      {
    //        Account = accountCase.Account,
    //        AccountNoteType = new XPQuery<STR_AccountNoteType>(uow).FirstOrDefault(a => a.AccountNoteType == Framework.Enumerators.Stream.AccountNoteType.Action),
    //        Case = accountCase,
    //        CreateDate = DateTime.Now,
    //        CreateUser = createUser,
    //        Note = new NTE_Note(uow)
    //        {
    //          CreateDate = DateTime.Now,
    //          CreateUser = createUser,
    //          Note = string.Format("SMS Sent to {0} by [{1} {2}]: '{3}'", cellContact.Contact.Value, createUser.Firstname, createUser.Lastname, smsMessage)
    //        }
    //      };

    //      accountCase.SMSCount++;
    //    }
    //  }

    //  uow.CommitChanges();
    //}
    //}

    #endregion
  }
}
