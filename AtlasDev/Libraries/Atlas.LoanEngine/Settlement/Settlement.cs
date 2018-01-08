using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.LoanEngine.Account;
using Atlas.LoanEngine.General;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.LoanEngine.Settlement
{
  public class Settlement
  {
    private long _accountId;
    private enum SettlementCalculation
    {
      CalculateAmountOnly,
      PostNew,
      SettlementMet
    }

    public Settlement(long accountId)
    {
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

      _accountId = accountId;
    }

    public decimal CalculateSettlementAmount(DateTime settlementDate, Enumerators.Account.SettlementType settlementType)
    {
      var settlement = DoSettlement(0, settlementDate, SettlementCalculation.CalculateAmountOnly, null);
      return settlement.TotalAmount;
    }

    public ACC_SettlementDTO PostSettlement(DateTime settlementDate, PER_PersonDTO createUser)
    {
      return DoSettlement(0, settlementDate, SettlementCalculation.PostNew, createUser);
    }

    public void UpdateSettlement(long settlementId, Enumerators.Account.SettlementStatus settlementStatus, decimal? settlementAmount =null)
    {
      if (settlementStatus == Enumerators.Account.SettlementStatus.Settled)
      {
        // Post to ledger
        DoSettlement(settlementId, null, SettlementCalculation.SettlementMet, null, settlementAmount);

        // Close Account
        var accountEng = new Account.Default(_accountId);
        accountEng.CloseAccount();
      }

      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == _accountId);
        if (account == null)
          throw new Exception(string.Format("AccountId {0} does not exist in DB", _accountId));

        var settlement = new XPQuery<ACC_Settlement>(uow).FirstOrDefault(a => a.SettlementId == settlementId);
        if (settlement == null)
          throw new Exception(string.Format("SettlementId {0} does not exist in DB", _accountId));

        settlement.SettlementStatus = new XPQuery<ACC_SettlementStatus>(uow).FirstOrDefault(s => s.Type == settlementStatus);
        settlement.LastStatusChange = DateTime.Now;

        uow.CommitChanges();
      }
    }

    private ACC_SettlementDTO DoSettlement(long settlementId, DateTime? settlementDate, SettlementCalculation whatToDo, PER_PersonDTO createUser, decimal? settlementAmount = null)
    {
      if (settlementDate.HasValue)
        if (settlementDate.Value.Date < DateTime.Today.Date)
          throw new Exception("Settlement Date cannot be in the past");

      using (var uow = new UnitOfWork())
      {
        var dummUow = new UnitOfWork();
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == _accountId);
        if (account == null)
          throw new Exception(string.Format("AccountId {0} does not exist in DB", _accountId));

        var currentSettlements = new XPQuery<ACC_Settlement>(uow).Where(s => s.Account.AccountId == _accountId && s.SettlementStatus.Type == Enumerators.Account.SettlementStatus.New).ToList();
        currentSettlements.ForEach(sm =>
          {
            sm.SettlementStatus = new XPQuery<ACC_SettlementStatus>(uow).FirstOrDefault(s => s.Type == Enumerators.Account.SettlementStatus.Cancelled);
            sm.LastStatusChange = DateTime.Now;
          });

        var settlement = new XPQuery<ACC_Settlement>(uow).FirstOrDefault(a => a.SettlementId == settlementId);
        if (whatToDo == SettlementCalculation.SettlementMet && settlement == null)
        {
          throw new Exception(string.Format("AccountId {0} does not have a linked settlement", _accountId));
        }
        else if (settlement != null)
        {
          if (settlement.ExpirationDate < DateTime.Today)
            throw new Exception("Settlement has expired");
          if (settlementAmount == null || settlementAmount < settlement.TotalAmount)
            throw new Exception("Settlement amount is too less");
        }
        else
        {
          if (whatToDo == SettlementCalculation.CalculateAmountOnly)
            settlement = new ACC_Settlement(dummUow);
          else
          {
            settlement = new ACC_Settlement(uow);
            settlement.Account = account;
            settlement.SettlementStatus = new XPQuery<ACC_SettlementStatus>(uow).FirstOrDefault(s => s.Type == Enumerators.Account.SettlementStatus.New);
            settlement.LastStatusChange = DateTime.Now;
            settlement.SettlementDate = settlementDate == null ? DateTime.Today.Date : (DateTime)settlementDate;
            settlement.ExpirationDate = (settlementDate == null ? DateTime.Today.Date : (DateTime)settlementDate).AddDays(account.AccountType.SettlementExpirationBuffer);
            settlement.CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == createUser.PersonId);
            settlement.SettlementType = new XPQuery<ACC_SettlementType>(uow).FirstOrDefault(s => s.Type == Enumerators.Account.SettlementType.Normal);
          }
          settlement.CreateDate = DateTime.Now;
        }

        var accountEngine = new Account.Default();

        // Get related Fees
        var accountTypeFees = Cache.Account.AccountTypeFeeSet.Where(a => a.AccountType.AccountTypeId == account.AccountType.AccountTypeId
          && a.Enabled && a.Fee.OnSettlement && a.CreateDate < settlement.CreateDate).OrderBy(a => a.Ordinal).ToList();

        if (whatToDo != SettlementCalculation.SettlementMet)
        {
          settlement.TotalAmount = account.AccountBalance;
          settlement.Fees = 0;
        }

        foreach (var accountTypeFee in accountTypeFees)
        {
          var feeAmount = Calculations.CalculateFee(accountTypeFee.Fee, account.LoanAmount, settlement.TotalAmount, account.Period);

          if (feeAmount > 0)
          {
            var accountFee = new ACC_AccountFee(dummUow);
            if (whatToDo == SettlementCalculation.SettlementMet)
              accountFee = new ACC_AccountFee(uow);

            accountFee.Account = account;
            accountFee.AccountTypeFee = new XPQuery<ACC_AccountTypeFee>(uow).FirstOrDefault(t => t.AccountTypeFeeId == accountTypeFee.AccountTypeFeeId);
            accountFee.Amount = feeAmount;

            // Write Fees to GL Transactions
            var feeTransaction = new LGR_Transaction(dummUow);
            if (whatToDo == SettlementCalculation.SettlementMet)
              feeTransaction = new LGR_Transaction(uow);

            feeTransaction.Account = account;
            feeTransaction.Amount = feeAmount;
            feeTransaction.CreateDate = DateTime.Now;
            feeTransaction.CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)Enumerators.General.Person.System);
            feeTransaction.Person = account.Person;
            feeTransaction.TransactionDate = settlement.CreateDate.Date;
            feeTransaction.TransactionType = accountFee.AccountTypeFee.Fee.TransactionType;
            feeTransaction.Type = accountFee.AccountTypeFee.Fee.TransactionType.TransactionTypeGroup.Type;

            if (whatToDo != SettlementCalculation.SettlementMet)
            {
              settlement.TotalAmount += feeAmount;
              settlement.Fees += feeAmount;
            }
          }
        }

        // Calculate Interest
        DateTime lastInterestCalculationDate = account.OpenDate.Value;
        var lastInterestCalculation = new XPQuery<LGR_Transaction>(uow).Where(t => t.Account.AccountId == _accountId).OrderBy(t => t.TransactionDate).LastOrDefault();
        if (lastInterestCalculation != null)
          lastInterestCalculationDate = lastInterestCalculation.TransactionDate;
        if (whatToDo != SettlementCalculation.SettlementMet)
          settlement.Interest = Calculations.CalculateDailyInterest(account.InterestRate, settlement.TotalAmount, ((settlementDate == null ? DateTime.Today : (DateTime)settlementDate).Date - lastInterestCalculationDate).TotalDays);

        var transactionType = new XPQuery<LGR_TransactionType>(uow).FirstOrDefault(t => t.Type == Enumerators.GeneralLedger.TransactionType.Interest);
        if (settlement.Interest > 0)
        {
          if (whatToDo == SettlementCalculation.SettlementMet)
          {
            var interestTransaction = new LGR_Transaction(uow);

            interestTransaction.Account = account;
            interestTransaction.Amount = settlement.Interest;
            interestTransaction.CreateDate = DateTime.Now;
            interestTransaction.CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)Enumerators.General.Person.System);
            interestTransaction.Person = account.Person;
            interestTransaction.TransactionDate = settlement.CreateDate.Date;
            interestTransaction.TransactionType = transactionType;
            interestTransaction.Type = transactionType.TransactionTypeGroup.Type;
          }
        }

        if (whatToDo != SettlementCalculation.SettlementMet)
        {
          settlement.TotalAmount += settlement.Interest;
          settlement.Amount = settlement.TotalAmount - settlement.Fees - settlement.Interest;
        }
        // Add settlement Amount
        if (whatToDo == SettlementCalculation.SettlementMet)
          transactionType = new XPQuery<LGR_TransactionType>(uow).FirstOrDefault(t => t.Type ==
                                              (settlement.SettlementType.Type == Enumerators.Account.SettlementType.InterAccount
                                              ? Enumerators.GeneralLedger.TransactionType.InterAccountSettlement
                                              : Enumerators.GeneralLedger.TransactionType.Settlement));

        if (whatToDo == SettlementCalculation.SettlementMet)
        {
          var settlementTransaction = new LGR_Transaction(uow);
          settlementTransaction.Account = account;
          settlementTransaction.Amount = settlement.Amount;
          settlementTransaction.CreateDate = DateTime.Now;
          settlementTransaction.CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)Enumerators.General.Person.System);
          settlementTransaction.Person = account.Person;
          settlementTransaction.TransactionDate = settlement.CreateDate.Date;
          settlementTransaction.TransactionType = transactionType;
          settlementTransaction.Type = transactionType.TransactionTypeGroup.Type;
          GeneralLedger gl = new GeneralLedger(account.AccountId);
          account.AccountBalance = gl.CalculateBalance(null);
          gl = null;
        }
        // posting the actual settlement
        if (whatToDo != SettlementCalculation.CalculateAmountOnly)
        {

          uow.CommitChanges();
        }

        return AutoMapper.Mapper.Map<ACC_Settlement, ACC_SettlementDTO>(settlement);
      }
    }
  }
}