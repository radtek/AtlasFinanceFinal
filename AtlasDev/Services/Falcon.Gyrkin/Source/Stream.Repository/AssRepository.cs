using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Ass.Structures.Stream;
using Atlas.Common.Extensions;
using Atlas.Common.Utils;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.Reporting.Properties;
using DevExpress.Xpo;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Stream.Domain.Models;
using Stream.Framework.Repository;
using Stream.Framework.Structures;

namespace Stream.Repository
{
  public class AssRepository : IAssRepository
  {
    private readonly IUserRepository _userRepository;
    private readonly IConfigService _configService;

    public AssRepository(IConfigService configService, IUserRepository userRepository)
    {
      _userRepository = userRepository;
      _configService = configService;
    }

    internal List<T> GetCustomQuery<T>(string sql) where T : class, new()
    {
      var queryUtil = new RawSql();
      var data = queryUtil.ExecuteObject<T>(sql, _configService.AssConnection, commandTimeout: 3600);
      return data;
    }

    //public List<long> ImportAccountTransactions(Dictionary<long, string> loanReferences)
    //{
    //  var accountIdsAffected = new List<long>();
    //  var loanReferencesLastImportReference = loanReferences.Select(a =>
    //    new { Reference = a.Key, LastImportReference = string.IsNullOrEmpty(a.Value) ? 0 : long.Parse(a.Value) }).OrderBy(a => a.LastImportReference).ToList();
    //  //var lastMonthEnd = DateTime.Today.AddDays(-DateTime.Today.Day);
    //  var lastImportReferences = loanReferencesLastImportReference.Select(a => a.LastImportReference).Distinct().ToList();
    //  foreach (var lastImportReference in lastImportReferences)
    //  {
    //    var tmp = loanReferencesLastImportReference.Where(a => a.LastImportReference == lastImportReference).ToList();
    //    var accountsPerRun = 500;
    //    var noOfRuns = (tmp.Count / accountsPerRun);
    //    for (var i = 0; i < (noOfRuns + 1); i++)
    //    {
    //      using (var uow = new UnitOfWork())
    //      {
    //        try
    //        {
    //          var systemUserId = (long)General.Person.System;

    //          var tempLoanReferences = tmp.Skip(i * accountsPerRun).Take(accountsPerRun).Select(a => a.Reference).Distinct();
    //          var accountsTransactions = GetCustomQuery<AccountTransaction>(string.Format(Resources.STR_ImportAccountTransactions,
    //            !tempLoanReferences.Any() ? "0" : string.Join(",", tempLoanReferences), lastImportReference));
    //          tempLoanReferences = accountsTransactions.Select(a => a.LoanReference).Distinct();

    //          var accounts = new XPQuery<STR_Account>(uow).Where(a => tempLoanReferences.Contains(a.Reference2)).ToList();

    //          foreach (var account in accounts)
    //          {
    //            var newAccountTransactions = accountsTransactions.Where(a => a.LoanReference == account.Reference2
    //              && a.Seqno > (string.IsNullOrEmpty(account.LastImportReference) ? 0 : int.Parse(account.LastImportReference)))
    //              .OrderBy(a => a.Seqno).ToList();
    //            if (newAccountTransactions.Count > 0)
    //            {
    //              accountIdsAffected.Add(account.AccountId);
    //            }

    //            var existingAccountTransactions = accountsTransactions.Where(a => a.LoanReference == account.Reference2
    //              && a.Seqno <= (string.IsNullOrEmpty(account.LastImportReference) ? 0 : int.Parse(account.LastImportReference))
    //              && a.StatusDate.HasValue).OrderBy(a => a.Seqno).ToList();

    //            foreach (var accountTransaction in existingAccountTransactions)
    //            {
    //              var transaction = new XPQuery<STR_Transaction>(uow).FirstOrDefault(t => t.Account == account &&
    //                t.Reference == accountTransaction.TransactionReference && t.TransactionStatus == null);
    //              if (transaction != null)
    //              {
    //                switch (accountTransaction.TranasctionStatus)
    //                {
    //                  case "U":
    //                    transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.Reversal);
    //                    break;

    //                  case "P":
    //                    transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.Paid);
    //                    break;

    //                  case "H":
    //                    transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.Handover);
    //                    break;

    //                  case "T":
    //                    transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.PartPayment);
    //                    break;

    //                  case "J":
    //                    transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.Journal);
    //                    break;

    //                  case "C":
    //                    transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.Cancel);
    //                    break;

    //                  case "W":
    //                    transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.WriteOff);
    //                    break;

    //                  case "E":
    //                    transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.EarlySettlement);
    //                    break;

    //                  case "K":
    //                  case "G":
    //                    transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.Refund);
    //                    break;
    //                }
    //              }
    //            }

    //            foreach (var accountTransaction in newAccountTransactions)
    //            {

    //              var transaction = new STR_Transaction(uow)
    //                {
    //                  Account = account,
    //                  Amount = accountTransaction.Amount,
    //                  CreateDate = DateTime.Now,
    //                  Reference = accountTransaction.TransactionReference,
    //                  TransactionDate = accountTransaction.TransactionDate
    //                };
    //              switch (accountTransaction.TransactionType)
    //              {
    //                case "A":
    //                  transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t => t.Type == Framework.Enumerators.Stream.TransactionType.Adjustment);
    //                  break;

    //                case "C":
    //                  transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t => t.Type == Framework.Enumerators.Stream.TransactionType.Cancel);
    //                  break;

    //                case "D":
    //                  transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t => t.Type == Framework.Enumerators.Stream.TransactionType.Discount);
    //                  break;

    //                case "E":
    //                  transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t => t.Type == Framework.Enumerators.Stream.TransactionType.EarlySettlement);
    //                  break;

    //                case "H":
    //                  transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t => t.Type == Framework.Enumerators.Stream.TransactionType.Handover);
    //                  break;

    //                case "J":
    //                  transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t => t.Type == Framework.Enumerators.Stream.TransactionType.Journal);
    //                  break;

    //                case "T":
    //                  transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t => t.Type == Framework.Enumerators.Stream.TransactionType.PartPayment);
    //                  break;

    //                case "P":
    //                  transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t => t.Type == Framework.Enumerators.Stream.TransactionType.Payment);
    //                  break;

    //                case "K":
    //                case "F":
    //                case "G":
    //                case "B":
    //                  transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t => t.Type == Framework.Enumerators.Stream.TransactionType.Refund);
    //                  break;

    //                case "S":
    //                  transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t => t.Type == Framework.Enumerators.Stream.TransactionType.Reschedules);
    //                  break;

    //                //case "S":transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t=>t.Type == Atlas.Enumerators.Stream.TransactionType.Scheduled);
    //                //  break;
    //                case "L":
    //                  transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t => t.Type == Framework.Enumerators.Stream.TransactionType.SplitRepay);
    //                  break;

    //                case "W":
    //                  transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t => t.Type == Framework.Enumerators.Stream.TransactionType.WriteOff);
    //                  break;

    //                case "R":
    //                  transaction.TransactionType = new XPQuery<STR_TransactionType>(uow).FirstOrDefault(t => t.Type == Framework.Enumerators.Stream.TransactionType.Instalment);
    //                  break;
    //              }

    //              switch (accountTransaction.TranasctionStatus)
    //              {
    //                case "H":
    //                  transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.Handover);
    //                  break;

    //                case "T":
    //                  transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.PartPayment);
    //                  break;

    //                case "U":
    //                  transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.Reversal);
    //                  break;

    //                case "P":
    //                  transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.Paid);
    //                  break;

    //                case "J":
    //                  transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.Journal);
    //                  break;

    //                case "C":
    //                  transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.Cancel);
    //                  break;

    //                case "W":
    //                  transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.WriteOff);
    //                  break;

    //                case "E":
    //                  transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.EarlySettlement);
    //                  break;

    //                case "K":
    //                case "G":
    //                  transaction.TransactionStatus = new XPQuery<STR_TransactionStatus>(uow).FirstOrDefault(t => t.Status == Framework.Enumerators.Stream.TransactionStatus.Refund);
    //                  break;
    //              }

    //              // below will be removed after a third tab, "Mini-Statement" is added on the manage screen
    //              if (transaction.TransactionType != null)
    //                new STR_Note(uow)
    //                {
    //                  Account = account,
    //                  AccountNoteType = new XPQuery<STR_AccountNoteType>(uow).FirstOrDefault(a => a.AccountNoteType == Framework.Enumerators.Stream.AccountNoteType.ImportedTransaction),
    //                  CreateDate = DateTime.Now,
    //                  CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == systemUserId),
    //                  Note = new NTE_Note(uow)
    //                  {
    //                    CreateDate = DateTime.Now,
    //                    CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == systemUserId),
    //                    Note = string.Format("Transaction Date: {0}, Transaction Amount: {1}, Transaction Type: {2}", accountTransaction.TransactionDate.ToString("dd/MM/yyyy"), accountTransaction.Amount, transaction.TransactionType.Description)
    //                  }
    //                };

    //              account.LastImportReference = accountTransaction.Seqno.ToString();
    //            }
    //          }
    //        }
    //        finally
    //        {
    //          uow.CommitChanges();
    //        }
    //      }
    //    }
    //  }

    //  return accountIdsAffected;
    //}

    //public void CalculateBalances(List<long> accountIds)
    //{
    //  using (var uow = new UnitOfWork())
    //  {
    //    var accountCases = new XPQuery<STR_Case>(uow).Where(a => accountIds.Contains(a.Account.AccountId) &&
    //                                            (a.CaseStatus.Status == Framework.Enumerators.CaseStatus.Type.New ||
    //                                            a.CaseStatus.Status == Framework.Enumerators.CaseStatus.Type.InProgress)).ToList();
    //    foreach (var accountCase in accountCases)
    //    {
    //      var transactions = new XPQuery<STR_Transaction>(uow).Where(t => t.Account == accountCase.Account).ToList();
    //      accountCase.ArrearsAmount = 0;
    //      accountCase.Balance = 0;
    //      accountCase.InstalmentsOutstanding = 0;
    //      foreach (var transaction in transactions)
    //      {
    //        if (transaction.TransactionType.Type == Framework.Enumerators.Stream.TransactionType.Instalment
    //          && transaction.TransactionStatus == null)
    //        {
    //          if (transaction.TransactionDate.Date < DateTime.Today.Date)
    //          {
    //            // outstanding payment
    //            accountCase.ArrearsAmount += transaction.Amount;
    //            accountCase.InstalmentsOutstanding++;
    //          }
    //          accountCase.Balance += transaction.Amount;
    //        }
    //      }
    //      accountCase.RequiredPayment = accountCase.ArrearsAmount;

    //      var lastPayment = transactions.OrderByDescending(t => t.TransactionDate).FirstOrDefault(t =>
    //        t.TransactionType.Type == Framework.Enumerators.Stream.TransactionType.Payment &&
    //        t.TransactionStatus != null &&
    //        t.TransactionStatus.Status != Framework.Enumerators.Stream.TransactionStatus.Reversal);
    //      if (lastPayment != null)
    //      {
    //        accountCase.LastReceiptDate = lastPayment.TransactionDate;
    //        accountCase.LastReceiptAmount = lastPayment.Amount;
    //      }

    //      SetCategory(accountCase, transactions.OrderBy(t => t.TransactionDate).FirstOrDefault(), transactions.Select(t => t.InstalmentNumber).ToArray(), uow);
    //    }

    //    uow.CommitChanges();
    //  }
    //}


    //private void SetCategory(STR_Case accountCase, STR_Transaction oldestOutstandingPayment, int[] outstandingInstalmentNumbers, UnitOfWork uow)
    //{
    //  var systemUserId = (long)General.Person.System;
    //  if (accountCase.Group.GroupType == Framework.Enumerators.Stream.GroupType.Collections)
    //  {
    //    if (oldestOutstandingPayment != null)
    //    {
    //      var oldestOutstandingPaymentMonthEnd = oldestOutstandingPayment.TransactionDate.AddDays(-oldestOutstandingPayment.TransactionDate.Day + 1).AddMonths(1).AddDays(-1);

    //      var currentCategory = accountCase.SubCategory == null ? (Framework.Enumerators.Category.Type?)null : accountCase.SubCategory.Category.CategoryType;

    //      Framework.Enumerators.Category.Type category;
    //      if (oldestOutstandingPaymentMonthEnd.Month == DateTime.Today.Month && oldestOutstandingPaymentMonthEnd.Year == DateTime.Today.Year)
    //        // Arrears
    //        category = Framework.Enumerators.Category.Type.Arrears;
    //      else if (oldestOutstandingPaymentMonthEnd.Month == DateTime.Today.AddMonths(-1).Month &&
    //        oldestOutstandingPaymentMonthEnd.Year == DateTime.Today.AddMonths(-1).Year)
    //        // Next Possible Handover Category
    //        category = Framework.Enumerators.Category.Type.NextPossibleHandover;
    //      else
    //        // Possible Handover Category
    //        category = Framework.Enumerators.Category.Type.PossibleHandover;

    //      if (outstandingInstalmentNumbers.Contains(1) && outstandingInstalmentNumbers.Contains(2) && outstandingInstalmentNumbers.Contains(3))
    //      {
    //        // first, second and third instalment not paid
    //        switch (category)
    //        {
    //          case Framework.Enumerators.Category.Type.PossibleHandover:
    //            {
    //              accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //                c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.PossibleHandovers_First3InstalmentsMissed);
    //              break;
    //            }
    //          case Framework.Enumerators.Category.Type.NextPossibleHandover:
    //            {
    //              accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //                c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.NextPossibleHandovers_First3InstalmentsMissed);
    //              break;
    //            }
    //          case Framework.Enumerators.Category.Type.Arrears:
    //            {
    //              accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //                c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.Arrears_First3InstalmentsMissed);
    //              break;
    //            }
    //        }
    //      }
    //      else if (outstandingInstalmentNumbers.Contains(1) && outstandingInstalmentNumbers.Contains(2))
    //      {
    //        // first and third instalment not paid  
    //        switch (category)
    //        {
    //          case Framework.Enumerators.Category.Type.PossibleHandover:
    //            {
    //              accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //                c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.PossibleHandovers_First2InstalmentsMissed);
    //              break;
    //            }
    //          case Framework.Enumerators.Category.Type.NextPossibleHandover:
    //            {
    //              accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //                c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.NextPossibleHandovers_First2InstalmentsMissed);
    //              break;
    //            }
    //          case Framework.Enumerators.Category.Type.Arrears:
    //            {
    //              accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //                c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.Arrears_First2InstalmentsMissed);
    //              break;
    //            }
    //        }
    //      }
    //      else if (outstandingInstalmentNumbers.Contains(1))
    //      {
    //        // first instalment not paid
    //        switch (category)
    //        {
    //          case Framework.Enumerators.Category.Type.PossibleHandover:
    //            {
    //              accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //                c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.Arrears_FirstInstalmentMissed);
    //              break;
    //            }
    //          case Framework.Enumerators.Category.Type.NextPossibleHandover:
    //            {
    //              accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //                c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.Arrears_FirstInstalmentMissed);
    //              break;
    //            }
    //          case Framework.Enumerators.Category.Type.Arrears:
    //            {
    //              accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //                c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.Arrears_FirstInstalmentMissed);
    //              break;
    //            }
    //        }
    //      }
    //      else
    //      {
    //        // default subcategory
    //        switch (category)
    //        {
    //          case Framework.Enumerators.Category.Type.PossibleHandover:
    //            {
    //              accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //                c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.PossibleHandovers_Default);
    //              break;
    //            }
    //          case Framework.Enumerators.Category.Type.NextPossibleHandover:
    //            {
    //              accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //                c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.NextPossibleHandovers_Default);
    //              break;
    //            }
    //          case Framework.Enumerators.Category.Type.Arrears:
    //            {
    //              accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //                c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.Arrears_Default);
    //              break;
    //            }
    //        }
    //      }

    //      if (accountCase.SubCategory != null && currentCategory != accountCase.SubCategory.Category.CategoryType)
    //      {
    //        if (accountCase.SubCategory.Category.CategoryType == Framework.Enumerators.Category.Type.Arrears)
    //        {
    //          SendSms(caseId: accountCase.CaseId, personId: systemUserId, smsTemplate: Notification.NotificationTemplate.Stream_SMS_FirstWarning);
    //        }
    //        else if (accountCase.SubCategory.Category.CategoryType == Framework.Enumerators.Category.Type.NextPossibleHandover)
    //        {
    //          SendSms(caseId: accountCase.CaseId, personId: systemUserId, smsTemplate: Notification.NotificationTemplate.Stream_SMS_SecondWarning);
    //        }
    //        else if (accountCase.SubCategory.Category.CategoryType == Framework.Enumerators.Category.Type.PossibleHandover)
    //        {
    //          SendSms(caseId: accountCase.CaseId, personId: systemUserId, smsTemplate: Notification.NotificationTemplate.Stream_SMS_ThirdWarning);
    //        }
    //      }
    //    }
    //  }
    //  else if (accountCase.Group.GroupType == Framework.Enumerators.Stream.GroupType.Sales)
    //  {
    //    if ((DateTime.Today - accountCase.CloseDate).TotalDays > 90)
    //      //revived clients
    //      accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //        c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.Sales_Revived);
    //    else
    //      // existing clients
    //      accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //        c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.Sales_Existing);
    //  }

    //  if (accountCase.SubCategory == null)
    //  {
    //    if (accountCase.Group.GroupType == Framework.Enumerators.Stream.GroupType.Sales)
    //      accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //        c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.Sales_Existing);
    //    else
    //      accountCase.SubCategory = new XPQuery<STR_SubCategory>(uow).FirstOrDefault(c =>
    //        c.SubCategoryId == (int)Framework.Enumerators.Stream.SubCategory.Arrears_Default);
    //  }
    //}

    //public Dictionary<long, Tuple<long, bool?>> CheckPayment(List<IGetCaseStreamAction> ptpCases)
    //{
    //  var results = new Dictionary<long, Tuple<long, bool?>>();

    //  var loanReferences = ptpCases.Select(a => a.AccountReference2).ToList();

    //  var accountsPerRun = 100;
    //  var noOfRuns = (loanReferences.Count / accountsPerRun) + 1;

    //  var queryDaysInterval = 4; // days to go back to bring payments
    //  for (var i = 0; i < noOfRuns; i++)
    //  {
    //    var tempLoanReferences = loanReferences.Skip(i * accountsPerRun).Take(accountsPerRun);
    //    var payments = GetCustomQuery<AccountTransaction>(string.Format(Resources.STR_ImportAccountPayments,
    //      !tempLoanReferences.Any() ? "0" : string.Join(",", tempLoanReferences), queryDaysInterval));

    //    foreach (var loanReference in tempLoanReferences)
    //    {
    //      var ptpsForLoanReferences = ptpCases.Where(p => p.AccountReference2 == loanReference && p.ActionType == Framework.Enumerators.Action.Type.Action).ToList();
    //      var paymentAmount = payments.Where(a => a.LoanReference == loanReference).Sum(a => a.Amount);
    //      foreach (var ptpsForLoanReference in ptpsForLoanReferences)
    //      {
    //        if (ptpsForLoanReference.Amount.HasValue)
    //        {
    //          if (ptpsForLoanReference.Amount <= paymentAmount)
    //          {
    //            // PTP Successful
    //            results.Add(ptpsForLoanReference.CaseStreamActionId, new Tuple<long, bool?>(ptpsForLoanReference.CaseStreamId, true));
    //            paymentAmount -= ptpsForLoanReference.Amount.Value;
    //          }
    //          else
    //          {
    //            // PTP unsuccessful - Add Reminder + increase priority
    //            // OR 
    //            // PTP Broken
    //            results.Add(ptpsForLoanReference.CaseStreamActionId, new Tuple<long, bool?>(ptpsForLoanReference.CaseStreamId,
    //              (DateTime.Today - ptpsForLoanReference.ActionDate.Date).TotalDays >= 1 ? false : (bool?)null));
    //          }
    //        }
    //      }
    //    }
    //  }

    //  return results;
    //}

    // <summary>
    // Allows a specific PTP to be checked
    // </summary>
    // <returns>Return true if there was a payment, and sets the PTP to successful, otherwise return false, and nothing else happens</returns>
    //public bool CheckForPayment(ICaseStreamAction caseStreamAction, long accountReference2, DateTime transcat)
    //{
    //  var queryDaysInterval = 4; // days to go back to bring payments
    //  var payments = GetCustomQuery<AccountTransaction>(string.Format(Resources.STR_ImportAccountPayments,
    //    accountReference2, queryDaysInterval));
    //  var accountPayments = payments.Where(p => p.LoanReference == accountReference2 &&
    //    p.TransactionDate == caseStreamAction.ActionDate.Date);

    //  return (caseStreamAction.Amount <= accountPayments.Sum(t => t.Amount));
    //}

    //public List<long> CheckForHandedOverClients(List<long> loanReferences)
    //{
    //  var caseStreamIds = new List<long>();
    //  var clients = GetCustomQuery(string.Format(Resources.STR_ClientsHandedOver,
    //    !loanReferences.Any() ? "0" : string.Join(",", loanReferences)));
    //  if (clients.Count > 0)
    //  {
    //    using (var uow = new UnitOfWork())
    //    {
    //      var caseStatusIds = new[] { (int)Framework.Enumerators.CaseStatus.Type.New, (int)Framework.Enumerators.CaseStatus.Type.InProgress };
    //      var salesGroupId = (int)Framework.Enumerators.Stream.GroupType.Sales;
    //      caseStreamIds = new XPQuery<STR_CaseStream>(uow).Where(a => clients.Contains(a.Case.Account.Debtor.Reference)
    //        && caseStatusIds.Contains(a.Case.CaseStatus.CaseStatusId)
    //        && !a.CompleteDate.HasValue
    //        && a.Case.Group.GroupId == salesGroupId).Select(c => c.CaseStreamId).ToList();
    //    }
    //  }
    //  return caseStreamIds;
    //}


    //public Dictionary<long, bool> CheckForClientsWithNewerLoan(Dictionary<long, DateTime> clientsWithLoanDates)
    //{
    //  var results = new Dictionary<long, bool>();

    //  var sb = new StringBuilder();
    //  foreach (var clientsWithLoanDate in clientsWithLoanDates)
    //  {
    //    sb.AppendLine(string.Format("WHEN CL.recid = {0} THEN to_date('{1}', 'YYYY-MM-DD')", clientsWithLoanDate.Key, clientsWithLoanDate.Value.ToString("yyyy-MM-dd")));
    //  }

    //  var clients = GetCustomQuery<ClientLoan>(
    //    string.Format(Resources.STR_ClientsTookNewerLoan,
    //    !clientsWithLoanDates.Any() ? "0" : string.Join(",", clientsWithLoanDates.Keys), sb));
    //  if (clients.Count > 0)
    //  {
    //    using (var uow = new UnitOfWork())
    //    {
    //      var clientReferences = clients.Select(d => d.ClientReference);
    //      var caseStatusIds = new[] { (int)Framework.Enumerators.CaseStatus.Type.New, (int)Framework.Enumerators.CaseStatus.Type.InProgress };
    //      var salesGroupId = (int)Framework.Enumerators.Stream.GroupType.Sales;
    //      var caseStreams = new XPQuery<STR_CaseStream>(uow).Where(a => clientReferences.Contains(a.Case.Account.Debtor.Reference)
    //        && caseStatusIds.Contains(a.Case.CaseStatus.CaseStatusId)
    //        && !a.CompleteDate.HasValue
    //        && a.Case.Group.GroupId == salesGroupId).ToList();

    //      lock (results)
    //      {
    //        caseStreams.ForEach(caseStream =>
    //        {
    //          if (!results.ContainsKey(caseStream.CaseStreamId))
    //            results.Add(caseStream.CaseStreamId, caseStream.Stream.StreamType == Framework.Enumerators.Stream.StreamType.PTC);
    //        });
    //      }
    //    }
    //  }
    //  return results;
    //}


    //public Dictionary<long, Framework.Enumerators.CaseStatus.Type> CheckForHandoverAndPaidUps(List<long> loanReferences)
    //{
    //  var results = new Dictionary<long, Framework.Enumerators.CaseStatus.Type>();
    //  var accounts = GetCustomQuery<IAccount>(string.Format(Resources.STR_CheckAccounBalanceToClose,
    //    !loanReferences.Any() ? "0" : string.Join(",", loanReferences)));
    //  if (accounts.Count > 0)
    //  {
    //    using (var uow = new UnitOfWork())
    //    {
    //      var reference2 = accounts.Select(l => l.LoanReference).ToList();
    //      var caseStreams = new XPQuery<STR_CaseStream>(uow).Where(c => reference2.Contains(c.Case.Account.Reference2) && !c.CompleteDate.HasValue).ToList();
    //      var systemUserId = (long)General.Person.System;
    //      var systemUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == systemUserId);
    //      var accountNoteType = new XPQuery<STR_AccountNoteType>(uow).FirstOrDefault(a =>
    //        a.AccountNoteType == Framework.Enumerators.Stream.AccountNoteType.ImportedTransaction);
    //      foreach (var caseStream in caseStreams)
    //      {
    //        var loan = accounts.FirstOrDefault(l => l.LoanReference == caseStream.Case.Account.Reference2);

    //        if (loan != null)
    //        {
    //          var newNote = new NTE_Note(uow)
    //          {
    //            CreateDate = DateTime.Now,
    //            CreateUser = systemUser,
    //            Note = string.Format("Status change on Ass to {0} on {1}", loan.StatusDescription, loan.PaidDate)
    //          };

    //          new STR_Note(uow)
    //          {
    //            Account = caseStream.Case.Account,
    //            AccountNoteType = accountNoteType,
    //            Case = caseStream.Case,
    //            CreateDate = DateTime.Now,
    //            CreateUser = systemUser,
    //            Note = newNote
    //          };
    //          results.Add(caseStream.CaseStreamId, loan.Status == "H" ? Framework.Enumerators.CaseStatus.Type.HandedOver : Framework.Enumerators.CaseStatus.Type.Closed);
    //        }
    //      }

    //      uow.CommitChanges();
    //    }
    //  }

    //  return results;
    //}


    //private void SendSms(long caseId, long personId, Notification.NotificationTemplate smsTemplate)
    //{
    //  var blockedSmsTemplates = new[]
    //    {
    //      Notification.NotificationTemplate.Stream_SMS_Initiate, 
    //      Notification.NotificationTemplate.Stream_SMS_PaymentThanks
    //    };

    //  if (blockedSmsTemplates.Contains(smsTemplate))
    //    return;

    //  using (var uow = new UnitOfWork())
    //  {
    //    var accountCase = new XPQuery<STR_Case>(uow).FirstOrDefault(a => a.CaseId == caseId);
    //    if (accountCase == null)
    //      throw new Exception(string.Format("Case with Id {0} does not exist", caseId));

    //    var createUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
    //    if (createUser == null)
    //      throw new Exception(string.Format("Person {0} does not exist", personId));

    //    var cellContact = accountCase.Account.Debtor.Contacts.Where(c => 
    //      c.Contact.ContactType.Type == General.ContactType.CellNo).OrderBy(c => c.CreateDate).FirstOrDefault();
    //    if (cellContact != null)
    //    {
    //      var template = new XPQuery<NTF_Template>(uow).FirstOrDefault(t => t.TemplateType.Type == smsTemplate);
    //      if (template == null)
    //      {
    //        new STR_Note(uow)
    //        {
    //          Account = accountCase.Account,
    //          AccountNoteType = new XPQuery<STR_AccountNoteType>(uow).FirstOrDefault(a => a.AccountNoteType == Framework.Enumerators.Stream.AccountNoteType.Normal),
    //          Case = accountCase,
    //          CreateDate = DateTime.Now,
    //          CreateUser = createUser,
    //          Note = new NTE_Note(uow)
    //          {
    //            CreateDate = DateTime.Now,
    //            CreateUser = createUser,
    //            Note = string.Format("SMS Sending Failed by [{0} {1}]: SMS template {2} does not exist", createUser.Firstname, createUser.Lastname, smsTemplate.ToStringEnum())
    //          }
    //        };
    //      }
    //      else
    //      {
    //        var templateKeys = new Dictionary<string, string>
    //        {
    //          {
    //            "[CLIENT_NAME]", string.Format("{0} {1} {2}",
    //              accountCase.Account.Debtor.Title, accountCase.Account.Debtor.FirstName,
    //              accountCase.Account.Debtor.LastName)
    //          }
    //        };
    //        var firstOrDefault = accountCase.Account.Branch.Company.Contacts.FirstOrDefault();
    //        if (firstOrDefault != null)
    //          templateKeys.Add("[BRANCH_TEL]", firstOrDefault.Value);

    //        var branchManager = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.Security.IsActive && 
    //          p.Roles.Any(a => a.RoleType.Type == General.RoleType.Branch_Manager) && 
    //          p.Branch == accountCase.Account.Branch);
    //        templateKeys.Add("[BRANCH_MANAGER_NAME]", branchManager == null ? "Branch Manager" : string.Format("{0} {1}", branchManager.Firstname, branchManager.Lastname));
            
    //        var smsMessage = FillInTheBlanks(template.Template, templateKeys);

    //        //_smsService.Send(cellContact.Contact.Value, smsMessage, Atlas.Enumerators.Notification.NotificationPriority.High);
    //        new STR_Note(uow)
    //        {
    //          Account = accountCase.Account,
    //          AccountNoteType = new XPQuery<STR_AccountNoteType>(uow).FirstOrDefault(a => a.AccountNoteType == Framework.Enumerators.Stream.AccountNoteType.Action),
    //          Case = accountCase,
    //          CreateDate = DateTime.Now,
    //          CreateUser = createUser,
    //          Note = new NTE_Note(uow)
    //          {
    //            CreateDate = DateTime.Now,
    //            CreateUser = createUser,
    //            Note = string.Format("SMS Sent to {0} by [{1} {2}]: '{3}'", cellContact.Contact.Value, createUser.Firstname, createUser.Lastname, smsMessage)
    //          }
    //        };

    //        accountCase.SMSCount++;
    //      }
    //    }

    //    uow.CommitChanges();
    //  }
    //}

    //private string FillInTheBlanks(string originalText, Dictionary<string, string> options)
    //{
    //  return options.Aggregate(originalText, (current, option) => current.Replace(option.Key, option.Value));
    //}

  }
}