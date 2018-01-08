using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.LoanEngine.Account;
using Atlas.LoanEngine.General;


namespace Atlas.LoanEngine.DebitOrder
{
  public sealed class Default
  {
    #region Constructor

    public Default(long accountId)
    {
      if (accountId <= 0)
        throw new Exception("Account identifier may only be a positive long.");

      using (var uow = new UnitOfWork())
      {
        _account = AutoMapper.Mapper.Map<ACC_Account, ACC_AccountDTO>(new XPQuery<ACC_Account>(uow).FirstOrDefault(d => d.AccountId == (long)accountId));
        _systemUser = AutoMapper.Mapper.Map<PER_Person, PER_PersonDTO>(new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)Enumerators.General.Person.System));
      }

      _accountUtil = new Account.Default(accountId);
    }

    #endregion


    #region Public Methods

    // TODO: This will move into the Atlas Online Collector Engine
    public void PostSuccessfulDebitOrder(long debitControlTransactionId)
    {
      using (var uow = new UnitOfWork())
      {
        var debitControlTransaction = new XPQuery<ACC_DebitControlTransaction>(uow).FirstOrDefault(d => d.DebitControlTransactionId == debitControlTransactionId);
        if (debitControlTransaction == null)
          throw new Exception(string.Format("Debit Control Transaction with Id ({0}) does not exist in DB", debitControlTransactionId));

        var ledgerEngine = new GeneralLedger(_account.AccountId);

        // get last interest calculation
        ledgerEngine.AddInterestUntil(DateTime.Today.Date, _systemUser);

        ledgerEngine.AddTransaction(Enumerators.GeneralLedger.TransactionType.NAEDODebitOrder,
          debitControlTransaction.DebitTransaction.ActionDate,
          _systemUser,
          (debitControlTransaction.DebitTransaction.OverrideAmount == null || debitControlTransaction.DebitTransaction.OverrideAmount == 0) ? debitControlTransaction.DebitTransaction.Amount : Convert.ToDecimal(debitControlTransaction.DebitTransaction.OverrideAmount));

        debitControlTransaction.DebitControl.Account.AccountBalance = ledgerEngine.CalculateBalance(null);
        if (debitControlTransaction.DebitControl.Account.AccountBalance <= debitControlTransaction.DebitControl.Account.AccountType.CloseBalance)
        {
          debitControlTransaction.DebitControl.Account.Status = new XPQuery<ACC_Status>(uow).FirstOrDefault(s => s.Type == Enumerators.Account.AccountStatus.Closed);
          debitControlTransaction.DebitControl.Account.StatusChangeDate = DateTime.Now;
          debitControlTransaction.DebitControl.Account.CloseDate = DateTime.Now;
        }
        else
        {
          var remainingPeriod = 0;
          if (debitControlTransaction.DebitControl.Account.PeriodFrequency.Type == Enumerators.Account.PeriodFrequency.Daily)
            remainingPeriod = (debitControlTransaction.DebitTransaction.ActionDate.AddMonths(1) - debitControlTransaction.DebitTransaction.ActionDate).Days;
          else
            remainingPeriod = debitControlTransaction.DebitTransaction.Control.Repetitions - debitControlTransaction.DebitTransaction.Control.CurrentRepetition;

          debitControlTransaction.DebitControl.Account.InstalmentAmount = GetNewInstalment(debitControlTransaction.DebitControl.Account.InstalmentAmount, debitControlTransaction.DebitControl.Account.AccountBalance, debitControlTransaction.DebitControl.Account.InterestRate, remainingPeriod, debitControlTransaction.DebitControl.Account.PeriodFrequency.Type);
        }

        ledgerEngine = null;

        uow.CommitChanges();
      }

      _accountUtil.UpdateAccountBalance();
    }


    // TODO: This will move into the Atlas Online Collector Engine
    public void PostUnsuccessfulDebitOrder(long debitControlTransactionId)
    {
      using (var uow = new UnitOfWork())
      {
        var debitControlTransaction = new XPQuery<ACC_DebitControlTransaction>(uow).FirstOrDefault(d => d.DebitControlTransactionId == debitControlTransactionId);
        if (debitControlTransaction == null)
          throw new Exception(string.Format("Debit Control Transaction with Id ({0}) does not exist in DB", debitControlTransactionId));

        var ledgerEngine = new GeneralLedger(_account.AccountId);

        // get last interest calculation
        ledgerEngine.AddInterestUntil(DateTime.Today.Date, _systemUser);

        ledgerEngine.AddTransaction(Enumerators.GeneralLedger.TransactionType.NAEDODebitOrder,
          debitControlTransaction.DebitTransaction.ActionDate,
          _systemUser,
          (debitControlTransaction.DebitTransaction.OverrideAmount == null || debitControlTransaction.DebitTransaction.OverrideAmount == 0) ? debitControlTransaction.DebitTransaction.Amount : Convert.ToDecimal(debitControlTransaction.DebitTransaction.OverrideAmount));

        ledgerEngine.AddTransaction(Enumerators.GeneralLedger.TransactionType.NAEDODebitOrderUnsuccessful,
          debitControlTransaction.DebitTransaction.ActionDate,
          _systemUser,
          (debitControlTransaction.DebitTransaction.OverrideAmount == null || debitControlTransaction.DebitTransaction.OverrideAmount == 0) ? debitControlTransaction.DebitTransaction.Amount : Convert.ToDecimal(debitControlTransaction.DebitTransaction.OverrideAmount));

        var remainingPeriod = 0;
        if (debitControlTransaction.DebitControl.Account.PeriodFrequency.Type == Enumerators.Account.PeriodFrequency.Daily)
          remainingPeriod = (debitControlTransaction.DebitTransaction.ActionDate.AddMonths(1) - debitControlTransaction.DebitTransaction.ActionDate).Days;
        else
          remainingPeriod = debitControlTransaction.DebitTransaction.Control.Repetitions - debitControlTransaction.DebitTransaction.Control.CurrentRepetition;

        debitControlTransaction.DebitControl.Account.AccountBalance = ledgerEngine.CalculateBalance(null);

        if (debitControlTransaction.DebitControl.Account.Host.Type != Enumerators.General.Host.Atlas_Online
          && debitControlTransaction.DebitControl.Account.PeriodFrequency.Type != Enumerators.Account.PeriodFrequency.Daily)
          debitControlTransaction.DebitControl.Account.InstalmentAmount = GetNewInstalment(debitControlTransaction.DebitControl.Account.InstalmentAmount, debitControlTransaction.DebitControl.Account.AccountBalance, debitControlTransaction.DebitControl.Account.InterestRate, remainingPeriod, debitControlTransaction.DebitControl.Account.AccountType.PeriodFrequency.Type);

        ledgerEngine = null;
      }
    }


    public void AddAdditionalDebitOrder(long controlId, decimal amount, DateTime actionDate, int trackingDay)
    {
      using (var uow = new UnitOfWork())
      {
        var control = new XPQuery<DBT_Control>(uow).FirstOrDefault(c => c.ControlId == controlId);
        if (control == null)
          throw new NullReferenceException(string.Format("Control {0} does not exist for account {0}", controlId, _account.AccountNo));

        var newTransaction = new DBT_Transaction(uow)
        {
          Control = control,
          Status = new XPQuery<DBT_Status>(uow).FirstOrDefault(s => s.StatusId == (int)Enumerators.Debit.Status.New),
          LastStatusDate = DateTime.Now,
          DebitType = new XPQuery<DBT_DebitType>(uow).FirstOrDefault(d => d.DebitTypeId == (int)Enumerators.Debit.DebitType.Additional),
          Amount = amount,
          InstalmentDate = actionDate,
          ActionDate = actionDate,
          Repetition = 0,
          CreateDate = DateTime.Now
        };
        if (trackingDay != control.TrackingDays)
          newTransaction.OverrideTrackingDays = trackingDay;

        uow.CommitChanges();
      }
    }


    public void CancelDebitOrder(long transactionId, long cancellingUserId)
    {
      using (var uow = new UnitOfWork())
      {
        var cancellingUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == cancellingUserId);
        if (cancellingUser == null)
          throw new NullReferenceException(string.Format("This user {0} does not exist", cancellingUserId));

        var transaction = new XPQuery<DBT_Transaction>(uow).FirstOrDefault(t => t.TransactionId == transactionId);
        if (transaction == null)
          throw new NullReferenceException(string.Format("Transaction {0} does not exist for account {0}", transactionId, _account.AccountNo));

        if (transaction.DebitType.Type != Enumerators.Debit.DebitType.Additional)
          throw new Exception(string.Format("Transaction {0} cannot be cancelled - Additional Debit Orders only", transactionId));

        if (transaction.Status.Type != Enumerators.Debit.Status.New)
          throw new Exception(string.Format("Transaction {0} cannot be cancelled - Status is not New", transactionId));

        transaction.Status = new XPQuery<DBT_Status>(uow).FirstOrDefault(s => s.StatusId == (int)Enumerators.Debit.Status.Cancelled);
        transaction.LastStatusDate = DateTime.Now;
        transaction.CancelDate = DateTime.Now;
        transaction.CancelUser = cancellingUser;

        uow.CommitChanges();
      }
    }


    public void OverrideDebitOrder(long transactionId, long overridingUserId, decimal? amount, DateTime? actionDate, int? trackingDays)
    {
      using (var uow = new UnitOfWork())
      {
        var overridingUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == overridingUserId);
        if (overridingUser == null)
          throw new NullReferenceException(string.Format("This user {0} does not exist", overridingUserId));

        var transaction = new XPQuery<DBT_Transaction>(uow).FirstOrDefault(t => t.TransactionId == transactionId);
        if (transaction == null)
          throw new NullReferenceException(string.Format("Transaction {0} does not exist for account {0}", transactionId, _account.AccountNo));

        if (transaction.Status.Type != Enumerators.Debit.Status.New)
          throw new Exception(string.Format("Transaction {0} cannot be cancelled - Status is not New", transactionId));

        if (amount != null)
          transaction.OverrideAmount = amount;
        if (actionDate != null)
          transaction.OverrideActionDate = actionDate;
        if (trackingDays != null)
          transaction.OverrideTrackingDays = trackingDays;

        transaction.OverrideDate = DateTime.Now;
        transaction.OverrideUser = overridingUser;

        uow.CommitChanges();
      }
    }

    #endregion


    #region Private Properties

    private ACC_AccountDTO _account = null;
    private PER_PersonDTO _systemUser = null;
    private Account.Default _accountUtil = null;

    #endregion


    #region Private Methods

    private decimal GetNewInstalment(decimal? currentInstalment, decimal accountBalance, float interestRate, int remainingPeriod, Enumerators.Account.PeriodFrequency periodFrequency)
    {
      var newInstalment = Calculations.CalculateInstalment(accountBalance, interestRate, remainingPeriod, periodFrequency);

      // // if instalment is less than 50% of his inital promised instalment, then reduce the remaining period
      //while (((newInstalment / currentInstalment) * 100) < 50 && remainingPeriod > 1)
      //{
      //  remainingPeriod--;
      //  newInstalment = Calculations.CalculateInstalment(accountBalance, interestRate, remainingPeriod, periodFrequency);
      //}

      return newInstalment;
    }

    #endregion

  }
}