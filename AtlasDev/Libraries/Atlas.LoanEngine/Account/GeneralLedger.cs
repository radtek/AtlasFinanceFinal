using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.LoanEngine.General;


namespace Atlas.LoanEngine.Account
{
  public class GeneralLedger
  {
    #region private properties

    private long _accountId;
    
    #endregion

    #region constructor(s)

    public GeneralLedger(long accountId)
    {
      if (accountId == 0)
        throw new Exception("Account Id cannot be 0");

      _accountId = accountId;
    }

    #endregion

    #region public methods

    public void PostPastPayment(Enumerators.GeneralLedger.TransactionType transactionType, DateTime transactionDate, PER_PersonDTO user, decimal amount)
    {
      // Calculate interest up until this point if no interest was calculated
      AddInterestUntil(transactionDate, user);

      var transaction = AddTransaction(transactionType, transactionDate, user, amount);

      // Get balance at this point
      var balance = CalculateBalance(transactionDate);

      // Calculate Adjustment Amount
      var adjustmentAmount = CalculateAdjustment(balance, transaction);
      if (adjustmentAmount > 0)
      {
        AddTransaction((adjustmentAmount > 0 ? Enumerators.GeneralLedger.TransactionType.CreditAdjustment : Enumerators.GeneralLedger.TransactionType.DebitAdjustment), transactionDate, user, adjustmentAmount);
      }

      // Calculate new balance and Instalment Amount
      balance = CalculateBalance(null);

      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == _accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist", _accountId));

        account.AccountBalance = balance;

        if (account.AccountBalance <= account.AccountType.CloseBalance)
        {
          account.Status = new XPQuery<ACC_Status>(uow).FirstOrDefault(s => s.Type == Enumerators.Account.AccountStatus.Closed);
          account.StatusChangeDate = DateTime.Now;
          account.CloseDate = DateTime.Now;
        }
        else
        {
          // TODO: This will change to sending a message to the collection Engine

          var debitControl = new XPQuery<ACC_DebitControl>(uow).FirstOrDefault(d => d.Account == account);
          var remainingPeriod = debitControl.Control.Repetitions - debitControl.Control.CurrentRepetition;

          // Calculate instalment
          var initialInstalment = new XPQuery<ACC_AffordabilityOption>(uow).FirstOrDefault(a => a.Account == account && a.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.Accepted).Instalment;
          var newInstalment = Calculations.CalculateInstalment(account.AccountBalance, account.InterestRate, remainingPeriod, account.PeriodFrequency.Type);

          // if instalment is less than 50% of his inital promised instalment, then reduce the remaining period
          while (((newInstalment / initialInstalment) * 100) < 50 && remainingPeriod > 1)
          {
            remainingPeriod--;
            newInstalment = Calculations.CalculateInstalment(account.AccountBalance, account.InterestRate, remainingPeriod, account.PeriodFrequency.Type);
          }

          account.InstalmentAmount = newInstalment;
        }

        uow.CommitChanges();
      }
    }

    public void AddInterestUntil(DateTime transactionDate, PER_PersonDTO user)
    {
      var interest = CalculateInterest(transactionDate);
      if (interest > 0)
      {
        // Check if last transaction was an interest calculation
        var lastTransaction = GetLastTransaction(transactionDate);
        var addNewTransaction = true;

        if (lastTransaction.TransactionType.TransactionTypeGroup.TType == Enumerators.GeneralLedger.TransactionTypeGroup.Interest)
        {
          using (var uow = new UnitOfWork())
          {
            var account = new XPQuery<ACC_Account>(uow).First(a => a.AccountId == _accountId);

            var accountEng = new Default(_accountId);
            var accountCycles = accountEng.GetCycles(AutoMapper.Mapper.Map<ACC_Account, ACC_AccountDTO>(account));
            if (transactionDate >= accountCycles.LastOrDefault().Item1.Date && lastTransaction.TransactionDate.Date <= accountCycles.LastOrDefault().Item2.Date)
            {
              // Update the transaction instead of adding a new one
              var transactionToUpdate = new XPQuery<LGR_Transaction>(uow).FirstOrDefault(t => t.TransactionId == lastTransaction.TransactionId);
              transactionToUpdate.TransactionDate = DateTime.Today.Date;
              transactionToUpdate.Amount += interest;
              if (transactionToUpdate.CalculatedArrearsDate != null)
                transactionToUpdate.CalculatedArrearsDate = null;

              uow.CommitChanges();
              addNewTransaction = false;
            }
          }
        }
        if (addNewTransaction)
          AddTransaction(Enumerators.GeneralLedger.TransactionType.Interest, transactionDate, user, interest);
      }
    }

    public decimal CalculateBalance(DateTime? transactionDate)
    {
      using (var uow = new UnitOfWork())
      {
        var transactions = new XPQuery<LGR_Transaction>(uow).Where(t => t.Account.AccountId == _accountId);

        if (transactionDate != null)
        {
          transactions = transactions.Where(t => t.TransactionDate <= transactionDate);
        }

        decimal balance = 0;
        foreach (var trans in transactions)
        {
          balance += trans.Type.Type == Enumerators.GeneralLedger.Type.Credit ? (trans.Amount * -1) : trans.Amount;
        }

        return balance;
      }
    }

    public LGR_TransactionDTO AddTransaction(Enumerators.GeneralLedger.TransactionType transactionType, DateTime transactionDate, PER_PersonDTO user, decimal amount)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == _accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist", _accountId));

        var transaction = new LGR_Transaction(uow)
        {
          Account = account,
          Amount = amount,
          CreateDate = DateTime.Now,
          CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == ((user == null) ? (int)Enumerators.General.Person.System : user.PersonId)),
          Person = account.Person,
          TransactionDate = transactionDate,
          TransactionType = new XPQuery<LGR_TransactionType>(uow).FirstOrDefault(t => t.Type == transactionType),
          Type = new XPQuery<LGR_TransactionType>(uow).FirstOrDefault(t => t.Type == transactionType).TransactionTypeGroup.Type
        };

        uow.CommitChanges();

        return AutoMapper.Mapper.Map<LGR_Transaction, LGR_TransactionDTO>(transaction);
      }
    }

    #endregion

    #region private methods

    private decimal CalculateInterest(DateTime transactionDate)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == _accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist", _accountId));

        DateTime lastInterestCalculationDate = account.OpenDate.Value;

        var lastInterestCalculation = new XPQuery<LGR_Transaction>(uow).Where(t => t.Account.AccountId == _accountId
          && t.TransactionDate <= transactionDate
          && t.TransactionType.Type == Enumerators.GeneralLedger.TransactionType.Interest).OrderBy(t => t.TransactionDate).LastOrDefault();
        if (lastInterestCalculation != null)
          lastInterestCalculationDate = lastInterestCalculation.TransactionDate;

        var balance = CalculateBalance(transactionDate);

        return Calculations.CalculateDailyInterest(account.InterestRate, balance, (transactionDate.Date - lastInterestCalculationDate.Date).TotalDays);
      }
    }

    private decimal CalculateAdjustment(decimal balance, LGR_TransactionDTO updatedTransaction)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == _accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist", _accountId));

        var tempBalance = balance;

        var transactions = new XPQuery<LGR_Transaction>(uow).Where(t => t.Account.AccountId == _accountId && t.TransactionDate > updatedTransaction.TransactionDate).OrderBy(t => t.TransactionDate).ToList();

        foreach (var transaction in transactions)
        {
          if (transaction.Fee != null && transaction.Fee.Percentage != null)
          {
            // Recalculate Fee
            transaction.Amount = Calculations.CalculateFee(AutoMapper.Mapper.Map<LGR_Fee, LGR_FeeDTO>(transaction.Fee), account.LoanAmount, tempBalance, account.Period);
          }
          else if (transaction.TransactionType.Type == Enumerators.GeneralLedger.TransactionType.Interest)
          {
            // Recalculate Interest
            DateTime lastInterestCalculationDate = account.OpenDate.Value;

            var lastInterestCalculation = new XPQuery<LGR_Transaction>(uow).Where(t => t.Account.AccountId == _accountId && t.TransactionDate < transaction.TransactionDate).OrderBy(t => t.TransactionDate).LastOrDefault();
            if (lastInterestCalculation != null)
              lastInterestCalculationDate = lastInterestCalculation.TransactionDate;

            transaction.Amount = Calculations.CalculateDailyInterest(account.InterestRate, tempBalance, (DateTime.Today.Date - lastInterestCalculationDate).TotalDays);
          }

          tempBalance += transaction.Type.Type == Enumerators.GeneralLedger.Type.Credit ? (transaction.Amount * -1) : transaction.Amount;
        }

        return (balance - tempBalance);
      }
    }

    private LGR_TransactionDTO GetLastTransaction(DateTime? transactionDate = null, Enumerators.GeneralLedger.TransactionType? transactionType = null)
    {
      using (var uow = new UnitOfWork())
      {
        var transactions = new XPQuery<LGR_Transaction>(uow).Where(t => t.Account.AccountId == _accountId);

        if (transactionType != null)
          transactions = transactions.Where(t => t.TransactionType.Type == transactionType);

        if (transactionDate != null)
          transactions = transactions.Where(t => t.TransactionDate <= transactionDate);

        var lastTransaction = transactions.OrderBy(t => t.TransactionDate).ThenBy(t => t.CreateDate).LastOrDefault();

        return AutoMapper.Mapper.Map<LGR_Transaction, LGR_TransactionDTO>(lastTransaction);
      }
    }

    #endregion
  }
}