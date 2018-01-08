using AccountService.WCF.Interface;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Atlas.LoanEngine.Settlement;
using Atlas.Domain.Structures;
using Atlas.Domain.DTO;

namespace AccountService.WCF.Implementation
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class AccountServer : IAccountServer
  {
    /// <summary>
    /// Get An Active account for a specified person. 
    /// Active Account = Open Status Account
    /// </summary>
    /// <param name="personId">specified person</param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <returns>AccountInfo - structure below</returns>
    public AccountInfo GetActiveAccount(long personId, out string error, out int result)
    {
      error = string.Empty;
      result = (int)General.WCFCallResult.OK;

      try
      {
        using (var uow = new UnitOfWork())
        {
          var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.Person.PersonId == personId
            && a.Status.Type == Account.AccountStatus.Open
            && a.Host.Type == General.Host.Atlas_Online);

          if (account != null)
          {
            return GetAccountInfo(new List<long>() { account.AccountId }).FirstOrDefault();
          }
          return null;
        }
      }
      catch (Exception ex)
      {
        error = ex.Message;
        result = (int)General.WCFCallResult.ServerError;
        return null;
      }
    }

    /// <summary>
    /// Get Account Info for a specific account
    /// </summary>
    /// <param name="accountId">specified account</param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <returns>AccountInfo - structure below</returns>
    public AccountInfo GetAccountInfo(long accountId, out string error, out int result)
    {
      error = string.Empty;
      result = (int)General.WCFCallResult.OK;

      try
      {
        return GetAccountInfo(new List<long>() { accountId }).FirstOrDefault();
      }
      catch (Exception ex)
      {
        error = ex.Message;
        result = (int)General.WCFCallResult.ServerError;
        return null;
      }
    }

    /// <summary>
    /// Get Account Info for all accounts a person has had - Open & Closed
    /// </summary>
    /// <param name="personId">persons Id</param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <returns>List of AccountInfo's - structure below</returns>
    public List<AccountInfo> GetAllAccounts(long personId, out string error, out int result)
    {
      error = string.Empty;
      result = (int)General.WCFCallResult.OK;

      try
      {
        using (var uow = new UnitOfWork())
        {
          var accounts = new XPQuery<ACC_Account>(uow).Where(a => a.Person.PersonId == personId
            && a.Host.Type == General.Host.Atlas_Online).ToList();

          if (accounts.Count > 0)
          {
            return GetAccountInfo(accounts.Select(a => a.AccountId).ToList());
          }
          return null;
        }
      }
      catch (Exception ex)
      {
        error = ex.Message;
        result = (int)General.WCFCallResult.ServerError;
        return null;
      }
    }

    /// <summary>
    /// Get the Active Bank Detail of a specific person
    /// </summary>
    /// <param name="personId">specified person</param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <returns>BankDetail - { BankDetailId, BankAccountName, BankAccountNumber, BankBranchCode, BankAccountType, BankAccountTypeId, BankName, BankId }</returns>
    public BankDetail GetActiveBankDetail(long personId, out string error, out int result)
    {
      error = string.Empty;
      result = (int)General.WCFCallResult.OK;

      try
      {
        using (var uow = new UnitOfWork())
        {
          var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);

          if (person != null)
          {
            var activeBankDetail = person.GetBankDetails.Select(b => b.BankDetail).FirstOrDefault(b => b.IsActive);

            var bankDetail = new BankDetail()
            {
              BankDetailId = activeBankDetail.DetailId,
              BankAccountName = activeBankDetail.AccountName,
              BankAccountNumber = activeBankDetail.AccountNum,
              BankBranchCode = activeBankDetail.Code,
              BankAccountType = activeBankDetail.AccountType.Description,
              BankAccountTypeId = activeBankDetail.AccountType.AccountTypeId,
              BankName = activeBankDetail.Bank.Description,
              BankId = activeBankDetail.Bank.BankId
            };
            return bankDetail;
          }
        }

        return null;
      }
      catch (Exception ex)
      {
        error = ex.Message;
        result = (int)General.WCFCallResult.ServerError;
        return null;
      }
    }

    /// <summary>
    /// Get pending AVS's for a specific person (AVS without a result as yet)
    /// Can also be account specific with accountId param
    /// </summary>
    /// <param name="personId">specified person</param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <param name="accountId">specified account</param>
    /// <returns>List<AVS> Type - List<{ IdNumber, Initials, LastName, BankAccountNumber, BankName, BankId }></returns>
    public List<PendingAVS> GetPendingAVS(long personId, long accountId, out string error, out int result)
    {
      error = string.Empty;
      result = (int)General.WCFCallResult.OK;

      try
      {
        using (var uow = new UnitOfWork())
        {
          var avsQuery = new XPQuery<AVS_Transaction>(uow).Where(a => a.Person.PersonId == personId
            && a.Host.Type == General.Host.Atlas_Online
            && a.Result == null);

          if (accountId > 0)
            avsQuery = avsQuery.Where(a => a.Account.AccountId == accountId);

          var pendingAVS = new List<PendingAVS>();
          avsQuery.ToList().ForEach(a => pendingAVS.Add(new PendingAVS()
          {
            IdNumber = a.IdNumber,
            Initials = a.Initials,
            LastName = a.LastName,
            BankAccountNumber = a.AccountNo,
            BankName = a.Bank.Description,
            BankId = a.Bank.BankId
          }));

          if (pendingAVS.Count > 0)
            return pendingAVS;
        }

        return null;
      }
      catch (Exception ex)
      {
        error = ex.Message;
        result = (int)General.WCFCallResult.ServerError;
        return null;
      }
    }

    ///// <summary>
    ///// Gets transaction history for a given account - Loan Statement
    ///// </summary>
    ///// <param name="accountId">specified account</param>
    ///// <param name="error">Error message if theres an error, else empty string</param>
    ///// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    ///// <returns>anonymous Type - List<{ TransactionDate, Amount, TransactionId, Description }></returns>

    /// <summary>
    /// Gets info to display on the Statement
    /// </summary>
    /// <param name="accountId">Specified Account</param>
    /// <returns>AccountStatement. Includes all neccessary information & statement history</returns>
    public AccountStatement GetStatement(long accountId)
    {
      var accountStatement = new AccountStatement();

      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist", accountId));

        // get easy info
        accountStatement.StatementDate = DateTime.Today;
        accountStatement.AccountNo = account.AccountNo;
        accountStatement.ClientName = account.Person.Firstname + " " + account.Person.Lastname;
        accountStatement.IdNumber = account.Person.IdNum;
        accountStatement.LoanAmount = account.LoanAmount;
        accountStatement.Term = account.Period;
        accountStatement.MonthlyInterestRate = account.InterestRate / 12;
        accountStatement.StartDate = account.OpenDate;
        accountStatement.EndDate = account.FirstInstalmentDate ?? DateTime.Today.AddDays(account.Period);

        if (account.Status.StatusId >= (int)Account.AccountStatus.Open)
          accountStatement.RepaymentDate = account.FirstInstalmentDate ?? DateTime.Today.AddDays(account.Period);
        else
          accountStatement.RepaymentDate = DateTime.Today.AddDays(account.Period);

        // Get contactInfo
        var cellContactInfo = account.Person.GetContacts.Select(c => c.Contact).FirstOrDefault(c => c.IsActive && c.ContactType.ContactTypeId == (int)General.ContactType.CellNo);
        if (cellContactInfo != null)
          accountStatement.ContactNumber = cellContactInfo.Value;

        // Build physcial Address
        var physicalAddress = account.Person.GetAddressDetails.Select(a => a.Address).FirstOrDefault(a => a.IsActive && a.AddressType.AddressTypeId == (int)General.AddressType.Residential);
        if (physicalAddress != null)
        {
          if (!string.IsNullOrEmpty(physicalAddress.Line1))
            accountStatement.PhysicalAddress += physicalAddress.Line1 + Environment.NewLine;
          if (!string.IsNullOrEmpty(physicalAddress.Line2))
            accountStatement.PhysicalAddress += physicalAddress.Line2 + Environment.NewLine;
          if (!string.IsNullOrEmpty(physicalAddress.Line3))
            accountStatement.PhysicalAddress += physicalAddress.Line3 + Environment.NewLine;
          if (!string.IsNullOrEmpty(physicalAddress.Line4))
            accountStatement.PhysicalAddress += physicalAddress.Line4 + Environment.NewLine;
          if (physicalAddress.Province != null)
            accountStatement.PhysicalAddress += physicalAddress.Province.Description + Environment.NewLine;
          if (!string.IsNullOrEmpty(physicalAddress.PostalCode))
            accountStatement.PhysicalAddress += physicalAddress.PostalCode;
        }

        // populate arrears info if account is in arrears
        var arrearage = new XPQuery<ACC_Arrearage>(uow).Where(a => a.Account.AccountId == accountId).OrderByDescending(a => a.PeriodStart).FirstOrDefault();
        if (arrearage != null && arrearage.DelinquencyRank > 0)
        {
          accountStatement.RepaymentDate = arrearage.PeriodEnd;
          accountStatement.RepaymentAmount = account.InstalmentAmount;
          accountStatement.AmountOverdue = arrearage.TotalArrearsAmount;
          accountStatement.DaysOverdue = (DateTime.Today - arrearage.PeriodStart).Days + (arrearage.PeriodStart - (account.FirstInstalmentDate ?? DateTime.Today.AddDays(account.Period))).Days;
          accountStatement.StartDate = arrearage.PeriodStart;
          accountStatement.EndDate = arrearage.PeriodEnd;
          accountStatement.CurrentDue = account.Status.Type == Account.AccountStatus.Open ? account.InstalmentAmount : 0;
          accountStatement.Arrears = arrearage.TotalArrearsAmount;

          if (arrearage.DelinquencyRank > 1)
            accountStatement.ArrearsAging30Days = arrearage.TotalArrearsAmount;
          if (arrearage.DelinquencyRank > 2)
            accountStatement.ArrearsAging60Days = arrearage.TotalArrearsAmount;
          if (arrearage.DelinquencyRank > 3)
            accountStatement.ArrearsAging90Days = arrearage.TotalArrearsAmount;
          if (arrearage.DelinquencyRank > 4)
            accountStatement.ArrearsAging120Days = arrearage.TotalArrearsAmount;
          if (arrearage.DelinquencyRank > 5)
            accountStatement.ArrearsAging150Days = arrearage.TotalArrearsAmount;

          accountStatement.ArrearsAgingCurrent = accountStatement.RepaymentAmount;
          accountStatement.ArrearsAgingTotalDue = accountStatement.RepaymentAmount;
        }
        else
        {
          // only applies to accounts that are not in arrears.
          // Get the Repayment amount (first instalment amount)
          var acceptedAffordabilityOptions = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => a.Account.AccountId == account.AccountId &&
            a.AffordabilityOptionStatus.AffordabilityOptionStatusId == (int)Atlas.Enumerators.Account.AffordabilityOptionStatus.Accepted).ToList();
          if (acceptedAffordabilityOptions.Count == 1)
          {
            if (acceptedAffordabilityOptions.Count == 0)
              throw new Exception("There is no affordability for the account");

            if (acceptedAffordabilityOptions.Count > 1)
              throw new Exception("There are more than one accepted option");

            var acceptedAffordabilityOption = acceptedAffordabilityOptions.FirstOrDefault();
            accountStatement.RepaymentAmount = acceptedAffordabilityOption.TotalPayBack ?? 0;
            accountStatement.CurrentDue = account.Status.Type == Account.AccountStatus.Open ? accountStatement.RepaymentAmount : 0;
            accountStatement.Arrears = 0;
          }
        }

        // Build statement Transactions 
        var lgrTransactions = new XPQuery<LGR_Transaction>(uow).Where(t => t.Account.AccountId == accountId).OrderBy(t => t.TransactionDate).OrderBy(t => t.CreateDate).ToList();

        var statement = new List<StatementTransaction>();
        if (lgrTransactions.Count > 0)
        {
          decimal balance = 0;
          lgrTransactions.OrderBy(t => t.TransactionDate).ToList().ForEach(t =>
          {
            balance += (t.Type.Type == GeneralLedger.Type.Debit ? t.Amount : t.Amount * -1);

            if (t.TransactionType.TransactionTypeGroup.TType == GeneralLedger.TransactionTypeGroup.Payment || t.TransactionType.TransactionTypeGroup.TType == GeneralLedger.TransactionTypeGroup.Settlement)
            {
              accountStatement.PaymentReceived += t.Amount;
            }
            else if (t.TransactionType.TransactionTypeGroup.TType == GeneralLedger.TransactionTypeGroup.Interest)
            {
              accountStatement.InterestAccrued += t.Amount;
            }
            else if (t.TransactionType.TransactionTypeGroup.TType == GeneralLedger.TransactionTypeGroup.Fee)
            {
              accountStatement.FeesLevied += t.Amount;
            }
            else if (t.TransactionType.Type == GeneralLedger.TransactionType.DefaultAdminFee)
            {
              accountStatement.DefaulAdminFeesLevied += t.Amount;
            }
            else
            {
              accountStatement.OtherDebits += t.Type.Type == GeneralLedger.Type.Debit ? t.Amount : 0;
              accountStatement.OtherCredits += t.Type.Type == GeneralLedger.Type.Credit ? t.Amount : 0;
            }

            statement.Add(new StatementTransaction()
            {
              TransactionDate = t.TransactionDate,
              DebitAmount = t.Type.Type == GeneralLedger.Type.Debit ? t.Amount : 0,
              CreditAmount = t.Type.Type == GeneralLedger.Type.Credit ? t.Amount : 0,
              Balance = balance,
              TransactionId = t.TransactionId,
              Description = t.TransactionType.Description
            });
          });

          accountStatement.StatementTransactions = statement;
          accountStatement.CurrentBalance = balance;
          accountStatement.LegalFeesLevied = 0; // there is no legal fees
        }

        if ((!(arrearage != null && arrearage.DelinquencyRank > 0) || arrearage == null) && accountStatement.PaymentReceived > 0 && account.Status.Type == Account.AccountStatus.Open)
          accountStatement.CurrentDue = accountStatement.RepaymentAmount = account.InstalmentAmount;

        accountStatement.TotalDue = account.Status.Type == Account.AccountStatus.Open ? accountStatement.RepaymentAmount : 0;
      }

      return accountStatement;
    }

    /// <summary>
    /// Gets the count of failure in descending order until a pass avs is found for a specified person 
    /// Count maxed to 5
    /// </summary>
    /// <param name="personId">specified person</param>
    /// <returns></returns>
    public int GetAVSFailureCount(long personId)
    {
      var failureCount = 0;

      using (var uow = new UnitOfWork())
      {
        var avsTransactions = new XPQuery<AVS_Transaction>(uow).Where(a => a.Person.PersonId == personId).OrderByDescending(a => a.CreateDate).Take(5).ToList();
        foreach (var avsTransaction in avsTransactions)
        {
          if (avsTransaction.Result != null && avsTransaction.Result.Type == AVS.Result.Failed)
            failureCount++;
          else
            break;
        }
      }

      return failureCount;
    }

    /// <summary>
    /// Not Yet Implemented
    /// Will get info for quote page
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <returns></returns>
    public Quotation GetQuote(long accountId, out string error, out int result)
    {
      error = string.Empty;
      result = (int)General.WCFCallResult.OK;

      try
      {
        using (var uow = new UnitOfWork())
        {
          var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
          if (account == null)
            throw new Exception(string.Format("Account {0} does not exist", accountId));

          var currentQuotation = new XPQuery<ACC_Quotation>(uow).FirstOrDefault(q => q.Account == account);

          if (currentQuotation == null)
            throw new Exception(string.Format("Quotation for Account {0} does not exist", accountId));

          var bankDetails = account.Person.GetBankDetails.Select(b=>b.BankDetail).ToList();
          var activeBankDetail = bankDetails.FirstOrDefault(a => a.IsActive);
          if (activeBankDetail == null)
            activeBankDetail = bankDetails.OrderByDescending(b => b.CreatedDT).FirstOrDefault();

          var residentialAddress = account.Person.GetAddressDetails.Select(a=>a.Address).FirstOrDefault(a => a.AddressType.AddressTypeId == General.AddressType.Residential.ToInt() && a.IsActive);

          if (currentQuotation.QuotationStatus.Type == Account.QuotationStatus.New)
          {
            currentQuotation.QuotationStatus = new XPQuery<ACC_QuotationStatus>(uow).FirstOrDefault(q => q.Type == Account.QuotationStatus.Issued);
            currentQuotation.LastStatusDate = DateTime.Now;
            uow.CommitChanges();
          }

          var acceptedFees = new XPQuery<ACC_AffordabilityOptionFee>(uow).Where(a => a.AffordabilityOption == currentQuotation.AffordabilityOption).ToList();

          var quotation = new Quotation()
          {
            AccountId = account.AccountId,
            AccountNo = account.AccountNo,
            Amount = currentQuotation.AffordabilityOption.Amount,
            Bank = activeBankDetail.Bank.Type,
            BankAccountNo = activeBankDetail.AccountNum,
            BankAccountName = activeBankDetail.AccountName,
            BankAccountType = activeBankDetail.AccountType.Type,
            BankBranch = string.Format("UNIVERSAL CODE ({0})", activeBankDetail.Code),
            ContactNumber = account.Person.GetContacts.Select(c=>c.Contact).Where(c => c.ContactType.ContactTypeId == General.ContactType.CellNo.ToInt() && c.IsActive).FirstOrDefault().Value,
            DateOfDebit = account.FirstInstalmentDate ?? DateTime.Today.AddDays(account.Period),
            DebitAmount = currentQuotation.AffordabilityOption.TotalPayBack ?? 0,
            FirstName = account.Person.Firstname,
            LastName = account.Person.Lastname,
            IdNumber = account.Person.IdNum,
            QuotationId = currentQuotation.QuotationId,
            QuotationNo = currentQuotation.QuotationNo,
            QuoteDate = currentQuotation.CreateDate,
            InterestRate = Math.Round(Convert.ToDecimal(account.InterestRate) / 365M, 3),
            RepaymentAmount = currentQuotation.AffordabilityOption.TotalPayBack ?? 0,
            RepaymentDate = account.FirstInstalmentDate ?? DateTime.Today.AddDays(account.Period),
            InitiationFee = acceptedFees.Where(f => f.AccountTypeFee.Fee.TransactionType.Type == Atlas.Enumerators.GeneralLedger.TransactionType.InitiationFee).Sum(f => f.Amount),
            ServiceFee = acceptedFees.Where(f => f.AccountTypeFee.Fee.TransactionType.Type == Atlas.Enumerators.GeneralLedger.TransactionType.ServiceFee).Sum(f => f.Amount)
          };

          if (residentialAddress != null)
          {
            quotation.ResidentialAddressCode = residentialAddress.PostalCode;
            quotation.ResidentialAddressLine1 = residentialAddress.Line1;
            quotation.ResidentialAddressLine2 = residentialAddress.Line2;
            quotation.ResidentialAddressLine3 = residentialAddress.Line3;
            quotation.ResidentialAddressLine4 = residentialAddress.Line4;
          }

          // TODO: type<a>
          return quotation;
        }
      }
      catch (Exception ex)
      {
        error = ex.Message;
        result = (int)General.WCFCallResult.ServerError;
        return null;
      }
    }

    /// <summary>
    /// Calculates the settlement amount for an account, given a date of settlement
    /// </summary>
    /// <param name="accountId">specified account</param>
    /// <param name="settlementDate">date of settlement</param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <returns>decimal - amount to be settlement on specified date</returns>
    public decimal? GetSettlementAmount(long accountId, DateTime settlementDate, out string error, out int result)
    {
      error = string.Empty;
      result = (int)General.WCFCallResult.OK;

      try
      {
        var settlement = new Atlas.LoanEngine.Settlement.Settlement(accountId);
        return settlement.CalculateSettlementAmount(settlementDate, Account.SettlementType.Normal);
      }
      catch (Exception ex)
      {
        error = ex.Message;
        result = (int)General.WCFCallResult.ServerError;
        return null;
      }
    }

    public Atlas.Domain.Structures.Settlement GetSettlementQuotation(long accountId, long settlementId)
    {
      using (var uow = new UnitOfWork())
      {
        var settlement = new XPQuery<ACC_Settlement>(uow).FirstOrDefault(s => s.SettlementId == settlementId
          && s.Account.AccountId == accountId);

        var accountSettlement = new Atlas.Domain.Structures.Settlement()
        {
          AccountNo = settlement.Account.AccountNo,
          Amount = settlement.Amount,
          Fees = settlement.Fees,
          FirstName = settlement.Account.Person.Firstname,
          IdNumber = settlement.Account.Person.IdNum,
          Interest = settlement.Interest,
          LastName = settlement.Account.Person.Lastname,
          SettlementDate = settlement.SettlementDate,
          SettlementId = settlement.SettlementId,
          TotalAmount = settlement.TotalAmount,
          ValidDaysLeft = (settlement.ExpirationDate.Date - DateTime.Today.Date).Days,
          ValidFrom = settlement.CreateDate,
          ExpirationDate = settlement.ExpirationDate
        };

        var residentialAddress = settlement.Account.Person.GetAddressDetails.Select(a=>a.Address).FirstOrDefault(a => a.AddressType.AddressTypeId == General.AddressType.Residential.ToInt() && a.IsActive);

        if (residentialAddress != null)
        {
          accountSettlement.ResidentialAddress = residentialAddress.Line1 + Environment.NewLine;
          accountSettlement.ResidentialAddress += residentialAddress.Line2 + Environment.NewLine;
          accountSettlement.ResidentialAddress += residentialAddress.Line3 + Environment.NewLine;
          accountSettlement.ResidentialAddress += residentialAddress.Line4 + Environment.NewLine;
          accountSettlement.ResidentialAddress += residentialAddress.PostalCode;
        }
        return accountSettlement;
      }
    }

    /// <summary>
    /// Returns an amount > 0 if the account is overdue, otherwise return 0
    /// </summary>
    /// <param name="accountId">specified account</param>
    public decimal GetOverdueAmount(long accountId)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist", accountId));

        // Calculate Interest + Arrears until today
        var gl = new Atlas.LoanEngine.Account.GeneralLedger(accountId);
        gl.AddInterestUntil(DateTime.Today.Date, new PER_PersonDTO() { PersonId = (int)General.Person.System });
        gl = null;
        var arrearageCalculation = new Atlas.LoanEngine.Account.Arrearage(accountId);
        arrearageCalculation.CalculateArrears();
        arrearageCalculation = null;

        var arrearage = new XPQuery<ACC_Arrearage>(uow).Where(a => a.Account.AccountId == accountId).OrderByDescending(a => a.PeriodStart).FirstOrDefault();
        if (arrearage.DelinquencyRank > 0)
          return arrearage.InstalmentDue;
        else
          return 0;
      }
    }

    /// <summary>
    /// Creates a settlement contract for an account on a given date
    /// </summary>
    /// <param name="accountId">specified account</param>
    /// <param name="settlementDate">date of settlement</param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <returns>ACC_SettlementDTO - The created settlement</returns>
    public Atlas.Domain.Structures.Settlement PostSettlement(long accountId, DateTime settlementDate, out string error, out int result)
    {
      error = string.Empty;
      result = (int)General.WCFCallResult.OK;

      try
      {
        var settlement = new Atlas.LoanEngine.Settlement.Settlement(accountId);
        var newSettlement = settlement.PostSettlement(settlementDate, new Atlas.Domain.DTO.PER_PersonDTO() { PersonId = (int)Atlas.Enumerators.General.Person.System });

        return new Atlas.Domain.Structures.Settlement()
        {
          SettlementId = newSettlement.SettlementId,
          Amount = newSettlement.Amount,
          ExpirationDate = newSettlement.ExpirationDate,
          Fees = newSettlement.Fees,
          Interest = newSettlement.Interest,
          TotalAmount = newSettlement.TotalAmount
        };
      }
      catch (Exception ex)
      {
        error = ex.Message;
        result = (int)General.WCFCallResult.ServerError;
        return null;
      }
    }

    /// <summary>
    /// Changes the status of an account to a specific status
    /// </summary>
    /// <param name="accountId">specified account</param>
    /// <param name="statusId">int value of the new Status = Account.AccountStatus</param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <returns>true/false on whether the account status was succesfully changed</returns>
    public bool UpdateAccountStatus(long accountId, Account.AccountStatus status, Account.AccountStatusReason? reason, Account.AccountStatusSubReason? subReason)
    {
      try
      {
        using (var uow = new UnitOfWork())
        {
          var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
          if (account == null)
            throw new Exception(string.Format("Account {0} does not exist", accountId));

          var accountEngine = new Atlas.LoanEngine.Account.Default(accountId);
          accountEngine.SetAccountStatus(account, status, reason, subReason, uow);

          uow.CommitChanges();

          return true;
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Creates a list of AccountInfo, to return for multiple accounts
    /// </summary>
    /// <param name="accountId">list of accountIds to get AccountInfo</param>
    /// <returns>List<AccountInfo> - converted account info </returns>
    private List<AccountInfo> GetAccountInfo(List<long> accountId)
    {
      var accountInfos = new List<AccountInfo>();
      using (var uow = new UnitOfWork())
      {
        var accounts = new XPQuery<ACC_Account>(uow).Where(a => accountId.Contains(a.AccountId)).ToList();

        if (accounts.Count > 0)
        {
          foreach (var account in accounts)
          {
            var accountInfo = new AccountInfo()
            {
              AccountId = account.AccountId,
              AccountNo = account.AccountNo,
              Balance = account.AccountBalance,
              InterestRate = account.InterestRate,
              LoanAmount = account.LoanAmount,
              OpenDate = account.OpenDate,
              Period = account.Period,
              Status = account.Status.Type,
              StatusReason = account.StatusReason != null ? account.StatusReason.Type : (Atlas.Enumerators.Account.AccountStatusReason?)null,
              StatusSubReason = account.StatusSubReason != null ? account.StatusSubReason.Type : (Atlas.Enumerators.Account.AccountStatusSubReason?)null
            };

            if (account.Status.StatusId >= (int)Account.AccountStatus.Open)
              accountInfo.FirstInstalmentDate = accountInfo.RepaymentDate = account.FirstInstalmentDate ?? DateTime.Today.AddDays(account.Period);
            else
              accountInfo.FirstInstalmentDate = accountInfo.RepaymentDate = DateTime.Today.AddDays(account.Period);

            if ((account.FirstInstalmentDate ?? DateTime.Today.AddDays(account.Period)) < DateTime.Today.Date)
            {
              // only applies to accounts that are not in arrears.
              // Get the Repayment amount (first instalment amount)
              var acceptedAffordabilityOptions = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => a.Account.AccountId == account.AccountId &&
                a.AffordabilityOptionStatus.AffordabilityOptionStatusId == (int)Atlas.Enumerators.Account.AffordabilityOptionStatus.Accepted).ToList();
              if (acceptedAffordabilityOptions.Count == 1)
              {
                if (acceptedAffordabilityOptions.Count == 0)
                  throw new Exception("There is no affordability for the account");

                if (acceptedAffordabilityOptions.Count > 1)
                  throw new Exception("There are more than one accepted option");

                var acceptedAffordabilityOption = acceptedAffordabilityOptions.FirstOrDefault();
                accountInfo.FirstRepaymentAmount = accountInfo.RepaymentAmount = acceptedAffordabilityOption.TotalPayBack ?? 0;
              }
            }

            // Get ArearsInfo 
            if (account.FirstInstalmentDate != null)
            {
              var arrearage = new XPQuery<ACC_Arrearage>(uow).Where(a => a.Account == account).OrderByDescending(a => a.PeriodEnd).FirstOrDefault();
              if (arrearage != null)
              {
                if (arrearage.PeriodStart.Date > account.FirstInstalmentDate)
                {
                  // account is in arrears
                  accountInfo.RepaymentDate = arrearage.PeriodEnd;
                  accountInfo.RepaymentAmount = arrearage.InstalmentDue;
                }
              }
            }

            accountInfos.Add(accountInfo);
          }
        }
      }
      return accountInfos;
    }

    /// <summary>
    /// Returns an affordability Option that the client can afford.
    /// If he does not qualify for anything, then a null object is returned
    /// </summary>
    /// <param name="accountId">specified account</param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <returns>AffordabilityOption - which option the client can afford
    /// If the client cannot afford anything, then a null is returned</returns>
    public AffordabilityOption GetAffordabilityOption(long accountId, out string error, out int result)
    {
      error = string.Empty;
      result = (int)General.WCFCallResult.OK;

      try
      {
        var affordabilityCalc = new Atlas.LoanEngine.Affordability.AffordabilityCalculator();
        var affordabilityOptions = affordabilityCalc.CalculateAffordabilityOptions(accountId, (long)Atlas.Enumerators.General.Person.System);
        var optionsClientCanAfford = affordabilityOptions.Where(a => a.CanClientAfford ?? false);

        var option = optionsClientCanAfford.FirstOrDefault(a => a.AffordabilityOptionType.Type == Account.AffordabilityOptionType.RequestedOption);
        if (option == null)
          option = affordabilityOptions.FirstOrDefault(a => a.AffordabilityOptionType.Type == Account.AffordabilityOptionType.MaxInstalment);

        if (option != null)
        {
          return new AffordabilityOption()
          {
            AffordabilityOptionId = option.AffordabilityOptionId,
            AffordabilityOptionType = option.AffordabilityOptionType.Type,
            AffordabilityOptionTypeId = option.AffordabilityOptionType.AffordabilityOptionTypeId,
            Amount = option.Amount,
            CanClientAfford = option.CanClientAfford,
            CapitalAmount = option.CapitalAmount,
            Instalment = option.Instalment,
            InterestRate = option.InterestRate,
            NumOfInstalment = option.NumOfInstalment,
            Period = option.Period,
            PeriodFrequency = option.PeriodFrequency.Description,
            PeriodFrequencyId = option.PeriodFrequency.PeriodFrequencyId,
            TotalFees = option.TotalFees,
            TotalPayBack = option.TotalPayBack
          };
        }

        return null;
      }
      catch (Exception ex)
      {
        error = ex.Message;
        result = (int)General.WCFCallResult.ServerError;
        return null;
      }
    }

    /// <summary>
    /// Accepts a certain affordability option for a specific account
    /// </summary>
    /// <param name="accountId">specified account</param>
    /// <param name="affordabilityOptionId">affordabilityOption to accept</param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <returns>true/false on whether the affordability option was accepted</returns>
    public bool AcceptAffordabilityOption(long accountId, long affordabilityOptionId, out string error, out int result)
    {
      error = string.Empty;
      result = (int)General.WCFCallResult.OK;

      try
      {
        var affordability = new Atlas.LoanEngine.Affordability.Default(accountId);
        affordability.AcceptOption(affordabilityOptionId);

        affordability = null;

        return true;
      }
      catch (Exception ex)
      {
        error = ex.Message;
        result = (int)General.WCFCallResult.ServerError;
        return false;
      }
    }

    /// <summary>
    /// Rejects a certain affordability option for a specific account
    /// </summary>
    /// <param name="accountId">specified account</param>
    /// <param name="affordabilityOptionId">affordabilityOption to reject</param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <returns>true/false on whether the affordability option was rejected</returns>
    public bool RejectAffordabilityOption(long accountId, long affordabilityOptionId, out string error, out int result)
    {
      error = string.Empty;
      result = (int)General.WCFCallResult.OK;

      try
      {
        var affordability = new Atlas.LoanEngine.Affordability.Default(accountId);
        affordability.RejectOption(affordabilityOptionId);

        return true;
      }
      catch (Exception ex)
      {
        error = ex.Message;
        result = (int)General.WCFCallResult.ServerError;
        return false;
      }
    }

    /// <summary>
    /// Reject a New/Issued Quote for an Account
    /// </summary>
    /// <param name="accountId">specifie account</param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <returns>true/false on whether the quote was rejected successfully or not</returns>
    public bool RejectQuote(long accountId, out string error, out int result)
    {
      error = string.Empty;
      result = (int)General.WCFCallResult.OK;

      try
      {
        using (var uow = new UnitOfWork())
        {
          var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
          if (account == null)
            throw new Exception(string.Format("Account {0} does not exist", accountId));

          var quotation = new XPQuery<ACC_Quotation>(uow).FirstOrDefault(q => q.Account == account &&
            (q.QuotationStatus.Type == Account.QuotationStatus.Issued
            || q.QuotationStatus.Type == Account.QuotationStatus.New));

          if (quotation == null)
            throw new Exception(string.Format("Pending Quotation for Account {0} does not exist", accountId));

          quotation.QuotationStatus = new XPQuery<ACC_QuotationStatus>(uow).FirstOrDefault(q => q.Type == Account.QuotationStatus.Rejected);
          quotation.LastStatusDate = DateTime.Now;

          new Atlas.LoanEngine.Account.Default().SetAccountStatus(account, Account.AccountStatus.Cancelled, Account.AccountStatusReason.RejectedOptions, null, uow);

          uow.CommitChanges();

          return true;
        }
      }
      catch (Exception ex)
      {
        error = ex.Message;
        result = (int)General.WCFCallResult.ServerError;
        return false;
      }
    }

    /// <summary>
    /// Accepts an Issued Quote for an Account
    /// </summary>
    /// <param name="accountId">specifie account</param>
    /// <param name="error">Error message if theres an error, else empty string</param>
    /// <param name="result">Atlas.Enumerators.General.WCFCallResult</param>
    /// <returns>true/false on whether the quote was accepted successfully or not</returns>
    public bool AcceptQuote(long accountId, out string error, out int result)
    {
      error = string.Empty;
      result = (int)General.WCFCallResult.OK;

      try
      {
        using (var uow = new UnitOfWork())
        {
          var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
          if (account == null)
            throw new Exception(string.Format("Account {0} does not exist", accountId));

          var quotation = new XPQuery<ACC_Quotation>(uow).FirstOrDefault(q => q.Account == account && (q.QuotationStatus.Type == Account.QuotationStatus.Issued || q.QuotationStatus.Type == Account.QuotationStatus.New));

          if (quotation == null)
            throw new Exception(string.Format("Pending Quotation for Account {0} does not exist", accountId));

          if (quotation.ExpiryDate.Date < DateTime.Today.Date)
          {
            quotation.QuotationStatus = new XPQuery<ACC_QuotationStatus>(uow).FirstOrDefault(q => q.Type == Account.QuotationStatus.Expired);
            quotation.LastStatusDate = DateTime.Now;
            uow.CommitChanges();
            return false;
          }

          quotation.QuotationStatus = new XPQuery<ACC_QuotationStatus>(uow).FirstOrDefault(q => q.Type == Account.QuotationStatus.Accepted);
          quotation.LastStatusDate = DateTime.Now;

          var accountUtil = new Atlas.LoanEngine.Account.Default(accountId);
          accountUtil.ApproveAccount(account, uow);

          uow.CommitChanges();

          Atlas.Workflow.AtlasOnline.Default.StepProcess(accountId, (int)Atlas.Enumerators.Workflow.ProcessStep.Quotation, (int)Atlas.Enumerators.Workflow.ProcessStep.Payout);

          return true;
        }
      }
      catch (Exception ex)
      {
        error = ex.Message;
        result = (int)General.WCFCallResult.ServerError;
        return false;
      }
    }

    /// <summary>
    /// Saves affordability items against account
    /// </summary>
    /// <param name="accountId">Account to save against</param>
    /// <param name="affordability">Affordability items to save</param>
    public void SaveAffordabilityItem(long accountId, List<Atlas.Domain.DTO.ACC_AffordabilityDTO> affordability)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist", accountId));

        foreach (var affordabilityItem in affordability)
        {
          var afford = new ACC_Affordability(uow);
          afford.Account = account;
          afford.AffordabilityCategory = new XPQuery<ACC_AffordabilityCategory>(uow).FirstOrDefault(a => a.AffordabilityCategoryType == affordabilityItem.AffordabilityCategory.AffordabilityCategoryType);
          afford.Amount = affordabilityItem.Amount;

          afford.Save();
        }

        uow.CommitChanges();
      }
    }

    /// <summary>
    /// Saves the account details
    /// </summary>
    public AccountInfo SaveAccount(decimal amount, int period, Account.PeriodFrequency frequency, long? personId, General.Host host)
    {
      var accountId = new Atlas.LoanEngine.Account.Default().CreateAccount(amount, period, (int)frequency, (long)personId, (int)host);

      return GetAccountInfo(new List<long>() { accountId }).FirstOrDefault();
    }

    /// <summary>
    /// Get the account based on person id
    /// </summary>
    public AccountInfo GetAccount(long personId)
    {
      var accountInfos = new List<AccountInfo>();

      using (var uow = new UnitOfWork())
      {
                //Edited By Prashant
                //var account = new XPQuery<ACC_Account>(uow).OrderBy(p => p.CreateDate).FirstOrDefault(a => a.Person.PersonId == personId && a.Status.Type != Account.AccountStatus.Technical_Error);
                var account = new XPQuery<ACC_Account>(uow).OrderBy(p => p.CreateDate).FirstOrDefault(a => a.Person.PersonId == personId && a.Status.StatusId != (int)Account.AccountStatus.Technical_Error);

                if (account != null)
          return GetAccountInfo(new List<long>() { account.AccountId }).FirstOrDefault();

        return null;
      }
    }

    /// <summary>
    /// Starts the workflow process
    /// </summary>
    /// <param name="accountId">account to start the workflow </param>
    public void WorkflowStart(long accountId)
    {
      Atlas.Workflow.AtlasOnline.Default.StartWorkflow(Workflow.WorkflowProcess.AtlasOnline, accountId);
    }

    /// <summary>
    /// Complete the workflow process. Meant for Declined/cancelled loans only
    /// </summary>
    /// <param name="accountId"></param>
    public void WorkflowComplete(long accountId)
    {
      Atlas.Workflow.AtlasOnline.Default.CompleteProcess(accountId);
    }

    /// <summary>
    /// Steps the process step to the next process step.. -_-
    /// if (currentProcessStepId == 0 && nextProcessStepId == 0) then the account takes the first available uncompleted step and steps it to the next ranked step
    /// if (currentProcessStepId == [value] && nextProcessStepId == 0) then the account will take the step specified and then steps the account to next ranked step
    /// if (currentProcessStepId == 0 && nextProcessStepId == [value]) then the account takes the first available uncompleted step and then steps the specified step
    /// if (currentProcessStepId == [value] && nextProcessStepId == [value]) then the account takes the current specified and then steps the next specified step
    /// </summary>
    /// <param name="accountId">account to step in the workflow</param>
    /// <param name="currentProcessStepId">the current step of the account to complete and step</param>
    /// <param name="nextProcessStepId">the next process step to redirect the account</param>
    public void WorkflowStepUp(long accountId, Atlas.Enumerators.Workflow.ProcessStep? currentProcessStepId, Atlas.Enumerators.Workflow.ProcessStep? nextProcessStepId = 0)
    {
      Atlas.Workflow.AtlasOnline.Default.StepProcess(accountId, (int)(currentProcessStepId ?? 0), (int)(nextProcessStepId ?? 0));
    }


    public bool UpdateBankDetail(long bankDetailId, bool isActive)
    {
      using (var uow = new UnitOfWork())
      {
        var bankDetail = new XPQuery<BNK_Detail>(uow).FirstOrDefault(p => p.DetailId == bankDetailId);

        if (bankDetail == null)
          throw new InvalidOperationException(string.Format("BankDetail {0} does not exist in the database.", bankDetailId));

        bankDetail.IsActive = isActive;
        bankDetail.Save();

        uow.CommitChanges();

        return true;
      }
    }
  }
}