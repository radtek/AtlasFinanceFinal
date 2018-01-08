using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.ExceptionBase;
using Atlas.Common.Extensions;
using Atlas.Common.Utils;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Enumerators;
//using Atlas.LoanEngine.Affordability;
using AutoMapper;
using DevExpress.Xpo;
using Falcon.Common.Structures;
using Falcon.Service.Helpers;
using Account = Falcon.Common.Structures.Account;
using Contact = Atlas.Domain.Model.Contact;
//using Default = Atlas.Workflow.AtlasOnline.Default;
using Province = Falcon.Common.Structures.Province;

namespace Falcon.Service.Business
{
  public static class AccountUtility
  {
    public static List<Account> Search(string searchString)
    {
      var accountList = new List<Account>();
      var searchKeys = searchString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim().ToLower()).ToList();

      using (var uow = new UnitOfWork())
      {
        var accounts = new List<ACC_Account>();
        searchKeys.ForEach(s =>
        {
          accounts.AddRange(new XPQuery<ACC_Account>(uow).Where(a =>
            a.AccountNo.ToLower().Contains(s)
            || a.Status.Description.ToLower().Contains(s)
            || a.Person.Firstname.ToLower().Contains(s)
            || a.Person.Lastname.ToLower().Contains(s)
            || a.Person.IdNum.ToLower().Contains(s)).ToList());
        });

        foreach (var account in accounts)
        {
          var searchAccount = accountList.FirstOrDefault(a => a.AccountId == account.AccountId);
          if (searchAccount == null)
          {
            searchAccount = new Account()
            {
              AccountId = account.AccountId,
              AccountNo = account.AccountNo,
              AccountType = account.AccountType != null ? account.AccountType.Description : string.Empty,
              CreateDate = account.CreateDate,
              FirstInstalmentDate = account.FirstInstalmentDate,
              Balance = account.AccountBalance,
              OpenDate = account.OpenDate,
              Firstname = account.Person.Firstname,
              Lastname = account.Person.Lastname,
              Host = account.Host.Description,
              IdNumber = account.Person.IdNum,
              LastStatusDate = account.StatusChangeDate,
              LoanAmount = account.LoanAmount,
              PersonId = account.Person != null ? account.Person.PersonId : (long?)null,
              Status = account.Status.Description,
              SearchWordHits = 1
            };
            accountList.Add(searchAccount);
          }
          else
          {
            searchAccount.SearchWordHits++;
          }
        }
      }

      return accountList;
    }

    public static AccountDetail GetAccountDetail(long accountId, bool isCacher)
    {

      // Read from cache as priority.
      if (!isCacher)
      {
        var cache = AccountCacher.Read(accountId);

        if (cache.Item1)
          return cache.Item2;
      }

      var accountDetail = new AccountDetail();

      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);

        if (account == null)
          return null;

        accountDetail.AccountId = account.AccountId;
        accountDetail.AccountBalance = account.AccountBalance;
        accountDetail.AccountNo = account.AccountNo;
        accountDetail.AccountType = account.AccountType != null ? account.AccountType.Description : string.Empty;
        accountDetail.AccountTypeId = account.AccountType != null ? account.AccountType.AccountTypeId : (long?)null;
        accountDetail.CapitalAmount = account.CapitalAmount;
        accountDetail.DateOfBirth = account.Person.DateOfBirth;
        accountDetail.CreateDate = account.CreateDate;
        accountDetail.Email = account.Person.Email;
        accountDetail.EmployerId = account.Person.Employer.CompanyId;
        accountDetail.Firstname = account.Person.Firstname;
        accountDetail.Middlename = account.Person.Middlename;
        accountDetail.Lastname = account.Person.Lastname;
        accountDetail.Gender = account.Person.Gender;
        accountDetail.Host = account.Host.Description;
        accountDetail.HostId = account.Host.HostId;
        accountDetail.IdNumber = account.Person.IdNum;
        accountDetail.InterestRate = account.InterestRate;
        accountDetail.InstalmentAmount = account.InstalmentAmount;
        accountDetail.LastStatusDate = account.StatusChangeDate;
        accountDetail.LoanAmount = account.LoanAmount;
        accountDetail.FirstInstalmentDate = account.FirstInstalmentDate;
        accountDetail.PayoutAmount = account.PayoutAmount;
        accountDetail.Period = account.Period;
        accountDetail.PeriodFrequnency = account.PeriodFrequency.Description;
        accountDetail.PeriodFrequnencyId = account.PeriodFrequency.PeriodFrequencyId;
        accountDetail.PersonId = account.Person.PersonId;
        accountDetail.Status = account.Status.Description;
        accountDetail.StatusId = account.Status.StatusId;

        if (account.Person.Branch != null)
        {
          accountDetail.Branch = account.Person.Branch.Company.Name;
          accountDetail.BranchId = account.Person.Branch.Company.CompanyId;
        }

        var accountPayRule = new XPQuery<ACC_AccountPayRule>(uow).FirstOrDefault(a => a.Account.AccountId == accountId && a.Enabled);
        if (accountPayRule != null)
        {
          if (accountPayRule.PayDate != null)
          {
            if (accountPayRule.PayDate.PayDateType.Type == Atlas.Enumerators.Account.PayDateType.DayOfMonth)
              accountDetail.PayDate = string.Format("{0} {1} ", StringUtils.AddOrdinal(accountPayRule.PayDate.DayNo), accountPayRule.PayDate.PayDateType.Description);
            else
              accountDetail.PayDate = ((DayOfWeek)accountPayRule.PayDate.DayNo).ToStringEnum();
          }
          if (accountPayRule.PayRule != null)
            accountDetail.PayRule = accountPayRule.PayRule.Description;
        }

        var arrearage = new XPQuery<ACC_Arrearage>(uow).Where(a => a.Account == account).OrderByDescending(a => a.ArrearageCycle).FirstOrDefault();

        if (arrearage != null && arrearage.DelinquencyRank >= 1)
        {
          accountDetail.ArrearsAmount = arrearage.TotalArrearsAmount;
          accountDetail.Delinquency = arrearage.DelinquencyRank;
        }
        else
        {
          accountDetail.ArrearsAmount = 0;
          accountDetail.Delinquency = 0;
        }
        accountDetail.DelinquencyPercentage = accountDetail.Delinquency / 12.00 * 100;
      }

      accountDetail.AffordabilityOptions = GetAffordabilityOptions(accountId);

      accountDetail.AffordabilityItems = GetAffordabilityItems(accountId);

      accountDetail.FraudEnquiries = GetFraudEnquiries(accountId);

      accountDetail.Authentication = GetAuthentications(accountId).OrderBy(p => p.CreateDate).ToList();

      accountDetail.BureauEnquiries = GetBureauEnquiries(accountId);

      accountDetail.CreditEnquiries = GetCreditEnquiries(accountId);

      accountDetail.Addresses = GetAddresses(accountId);

      accountDetail.Contacts = GetContacts(accountId);

      accountDetail.Notes = GetNotes(accountId);

      accountDetail.Statement = GetStatement(accountId);

      accountDetail.BankDetails = GetBankDetails(accountId);

      accountDetail.Workflow = GetWorkflow(accountId);

      accountDetail.StatusHistory = GetStatusHistory(accountId);

      accountDetail.AvsTransactions = new AvsUtility().GetAvsTransactions(null, null, null, null, null, null, accountId);

      accountDetail.DebitControls = GetNaedoTransactions(accountId);

      accountDetail.Payouts = GetPayouts(accountId);

      accountDetail.AccountHistory = GetAccountHistory(accountDetail.PersonId, accountId);

      accountDetail.Relations = GetPersonRelations(accountDetail.PersonId);

      accountDetail.Employers = GetEmployers(accountDetail.PersonId);

      accountDetail.Quotations = GetQuotations(accountDetail.AccountId);

      if (!isCacher)
        AccountCacher.Cache(accountDetail);

      return accountDetail;
    }

    public static List<BureauEnquiries> GetBureauEnquiries(long accountId)
    {
      var enquiries = new List<BureauEnquiries>();

      using (var uow = new UnitOfWork())
      {
        var bureauEnquiryCollection = new XPQuery<BUR_Enquiry>(uow).Where(p => p.Account.AccountId == accountId);
        foreach (var enquiry in bureauEnquiryCollection)
        {
          var enq = new BureauEnquiries();
          enq.CreateDate = enquiry.CreateDate;
          enq.EnquiryDate = enquiry.EnquiryDate;
          enq.EnquiryId = enquiry.EnquiryId;
          enq.EnquiryType = enquiry.EnquiryType;
          enq.FirstName = enquiry.FirstName;
          enq.IdentityNum = enquiry.IdentityNum;
          enq.IsSucess = enquiry.IsSucess;
          enq.LastName = enquiry.LastName;
          enq.TransactionType = enquiry.TransactionType;

          enquiries.Add(enq);
        }
      }
      return enquiries;
    }

    public static List<Quotation> GetQuotations(long accountId)
    {
      var quotations = new List<Quotation>();
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist", accountId));

        var quotationList = new XPQuery<ACC_Quotation>(uow).Where(q => q.Account == account).ToList();

        foreach (var currentQuotation in quotationList)
        {
          var quotation = new Quotation()
          {
            AccountId = account.AccountId,
            Amount = currentQuotation.AffordabilityOption.Amount,
            CapitalAmount = currentQuotation.AffordabilityOption.CapitalAmount,
            DateOfDebit = account.FirstInstalmentDate ?? DateTime.Today.AddDays(account.Period),
            TotalFees = currentQuotation.AffordabilityOption.TotalFees,
            InstalmentAmount = currentQuotation.AffordabilityOption.Instalment,
            InterestRate = currentQuotation.AffordabilityOption.InterestRate ?? account.AccountType.InterestRate,
            LastStatusDate = currentQuotation.LastStatusDate,
            NoOfInstalments = currentQuotation.AffordabilityOption.NumOfInstalment,
            Period = currentQuotation.AffordabilityOption.Period,
            PeriodFrequency = currentQuotation.AffordabilityOption.PeriodFrequency.Description,
            QuotationId = currentQuotation.QuotationId,
            QuotationNo = currentQuotation.QuotationNo,
            QuotationStatus = currentQuotation.QuotationStatus.Description,
            QuotationStatusId = currentQuotation.QuotationStatus.QuotationStatusId,
            QuoteDate = currentQuotation.CreateDate,
            TotalRepayment = currentQuotation.AffordabilityOption.TotalPayBack
          };
          switch (currentQuotation.QuotationStatus.Type)
          {
            case Atlas.Enumerators.Account.QuotationStatus.Accepted:
              quotation.QuotationStatusColor = "label label-success";
              break;
            case Atlas.Enumerators.Account.QuotationStatus.Expired:
              quotation.QuotationStatusColor = "label label-warning";
              break;
            case Atlas.Enumerators.Account.QuotationStatus.New:
              quotation.QuotationStatusColor = "label label-info";
              break;
            case Atlas.Enumerators.Account.QuotationStatus.Rejected:
              quotation.QuotationStatusColor = "label label-danger";
              break;
            case Atlas.Enumerators.Account.QuotationStatus.Issued:
              quotation.QuotationStatusColor = "label label-primary";
              break;
          }
          quotations.Add(quotation);
        }
      }
      return quotations;
    }

    public static List<AccountAffordabilityOption> GetAffordabilityOptions(long accountId)
    {
      var affordabilityOptions = new List<AccountAffordabilityOption>();

      using (var uow = new UnitOfWork())
      {
        var options = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => a.Account.AccountId == accountId).ToList();
        foreach (var option in options)
        {
          var affordability = new AccountAffordabilityOption();
          affordability.AffordabilityOptionId = option.AffordabilityOptionId;
          affordability.AffordabilityOptionStatus = option.AffordabilityOptionStatus.Description;
          affordability.AffordabilityOptionStatusId = option.AffordabilityOptionStatus.AffordabilityOptionStatusId;
          affordability.AffordabilityOptionType = option.AffordabilityOptionType.Description;
          affordability.Amount = option.Amount;
          affordability.CanClientAfford = option.CanClientAfford ?? false;
          affordability.CapitalAmount = option.CapitalAmount;
          affordability.CreateDate = option.CreatedDate;
          affordability.Instalment = option.Instalment;
          affordability.InterestRate = option.InterestRate;
          affordability.LastStatusDate = option.LastStatusDate;
          affordability.NumOfInstalments = option.NumOfInstalment;
          affordability.Period = option.Period;
          affordability.PeriodFrequency = option.PeriodFrequency.Description;
          affordability.TotalFees = option.TotalFees;
          affordability.TotalPayBack = option.TotalPayBack;

          switch (option.AffordabilityOptionStatus.Type)
          {
            case Atlas.Enumerators.Account.AffordabilityOptionStatus.Accepted:
              affordability.AffordabilityOptionStatusColor = "label label-success";
              break;
            case Atlas.Enumerators.Account.AffordabilityOptionStatus.Cancelled:
              affordability.AffordabilityOptionStatusColor = "label label-warning";
              break;
            case Atlas.Enumerators.Account.AffordabilityOptionStatus.New:
              affordability.AffordabilityOptionStatusColor = "label label-info";
              break;
            case Atlas.Enumerators.Account.AffordabilityOptionStatus.Rejected:
              affordability.AffordabilityOptionStatusColor = "label label-danger";
              break;
            case Atlas.Enumerators.Account.AffordabilityOptionStatus.Sent:
              affordability.AffordabilityOptionStatusColor = "label label-primary";
              break;
          }

          affordabilityOptions.Add(affordability);
        }
      }

      return affordabilityOptions;
    }

    public static List<AccountAffordabilityItem> GetAffordabilityItems(long accountId)
    {
      var affordabilityItems = new List<AccountAffordabilityItem>();

      using (var uow = new UnitOfWork())
      {
        var affordability = new XPQuery<ACC_Affordability>(uow).Where(a => a.Account.AccountId == accountId).OrderBy(a => a.AffordabilityCategory.Ordinal).ToList();
        var totalItem = new AccountAffordabilityItem()
        {
          AffordabilityId = 0,
          Category = "Disposable Income",
          Amount = 0,
          Type = Atlas.Enumerators.Account.AffordabilityCategoryType.Display.ToStringEnum(),
          TypeColor = ""
        };
        foreach (var affordabilityItem in affordability)
        {
          var item = MapToAffordability(Mapper.Map<ACC_Affordability, ACC_AffordabilityDTO>(affordabilityItem));
          affordabilityItems.Add(item);

          if (affordabilityItem.DeleteDate == null)
          {
            if (affordabilityItem.AffordabilityCategory.AffordabilityCategoryType == Atlas.Enumerators.Account.AffordabilityCategoryType.Income)
              totalItem.Amount += affordabilityItem.Amount;
            else if (affordabilityItem.AffordabilityCategory.AffordabilityCategoryType == Atlas.Enumerators.Account.AffordabilityCategoryType.Expense)
              totalItem.Amount -= affordabilityItem.Amount;
          }
        }

        totalItem.TypeColor = totalItem.Amount > 0 ? "label label-success" : "label label-danger";
        affordabilityItems.Add(totalItem);
      }

      return affordabilityItems;
    }

    public static List<FraudScore> GetFraudEnquiries(long accountId)
    {
      var accountBureauEnquiries = new List<FraudScore>();

      using (var uow = new UnitOfWork())
      {
        var bureauEnquiries = new XPQuery<BUR_Enquiry>(uow).Where(a => a.Account.AccountId == accountId && a.EnquiryType == Risk.RiskEnquiryType.Fraud).ToList();

        foreach (var bureauEnquiry in bureauEnquiries)
        {
          var fraudScore = new XPQuery<FPM_FraudScore>(uow).Where(p => p.Enquiry.EnquiryId == bureauEnquiry.EnquiryId).ToList();
          if (fraudScore.Count > 0)
          {
            foreach (var fraud in fraudScore)
            {
              FraudScore fpm = new FraudScore();
              fpm.BankAccountNo = fraud.BankAccountNo;
              fpm.CellNo = fraud.CellNo;
              fpm.CreatedDate = fraud.CreatedDate;
              fpm.FraudScoreId = fraud.FraudScoreId;
              fpm.IDNumber = fraud.IDNumber;
              fpm.OverrideDate = fraud.OverrideDate;
              fpm.OverrideUser = fraud.OverrideUser != null ? string.Format("{0} {1}", fraud.OverrideUser.Firstname, fraud.OverrideUser.Lastname) : string.Empty;
              fpm.Passed = fraud.Passed;
              fpm.Rating = fraud.Rating;
              fpm.RatingDescription = fraud.RatingDescription;
              fpm.Reasons = new List<FraudScoreReason>();

              var reasons = new XPQuery<FPM_FraudScore_Reason>(uow).Where(p => p.FraudScore.FraudScoreId == fraud.FraudScoreId).ToList();
              foreach (var reason in reasons)
              {
                FraudScoreReason r = new FraudScoreReason();
                r.ReasonCode = reason.ReasonCode;
                r.Description = reason.Description;
                fpm.Reasons.Add(r);
              }
              accountBureauEnquiries.Add(fpm);
            }
          }
        }
      }
      return accountBureauEnquiries;
    }

    public static List<Authentication> GetAuthentications(long accountId)
    {
      var accountAuthentications = new List<Authentication>();

      using (var uow = new UnitOfWork())
      {
        var xdsAuthentication = new XPQuery<FPM_Authentication>(uow).Where(p => p.Account.AccountId == accountId).ToList();
        if (xdsAuthentication.Count > 0)
        {
          foreach (var xds in xdsAuthentication)
          {
            accountAuthentications.Add(new Authentication()
            {
              Authenticated = xds.Authenticated,
              AuthenticatedPercentage = xds.AuthenticatedPercentage,
              AuthenticationId = xds.AuthenticationId,
              Completed = xds.Completed,
              CreateDate = xds.CreateDate,
              Enabled = xds.Enabled,
              QuestionCount = xds.QuestionCount,
              Reference = xds.Reference,
              RequiredPercentage = xds.RequiredPercentage,
              FirstName = xds.Person != null ? xds.Person.Firstname : string.Empty,
              LastName = xds.Person != null ? xds.Person.Lastname : string.Empty,
              IdNo = xds.Person != null ? xds.Person.IdNum : string.Empty,
              BankAccountNo = xds.BankDetail != null ? xds.BankDetail.AccountNum : string.Empty,
              OverrideUser = xds.OverridePerson != null ? string.Format("{0} {1}", xds.OverridePerson.Firstname, xds.OverridePerson.Lastname) : string.Empty,
              OverrideDate = xds.OverrideDate != null ? xds.OverrideDate : (DateTime?)null
            });
          }
        }
      }

      return accountAuthentications;
    }

    public static List<AccountBureauEnquiry> GetCreditEnquiries(long accountId)
    {
      var accountBureauEnquiries = new List<AccountBureauEnquiry>();
      using (var uow = new UnitOfWork())
      {
        var bureauEnquiries = new XPQuery<BUR_Enquiry>(uow).Where(a => a.Account.AccountId == accountId && a.EnquiryType == Risk.RiskEnquiryType.Credit).ToList();

        foreach (var bureauEnquiry in bureauEnquiries)
        {
          var score = new XPQuery<BUR_Colour>(uow).FirstOrDefault(p => p.EnquiryId.EnquiryId == bureauEnquiry.EnquiryId);

          var accountBureau = new AccountBureauEnquiry()
          {
            BureauAccounts = new List<AccountBureauAccount>(),
            CreateDate = bureauEnquiry.CreateDate,
            EnquiryDate = bureauEnquiry.EnquiryDate,
            EnquiryId = bureauEnquiry.EnquiryId,
            EnquiryType = bureauEnquiry.EnquiryType.ToStringEnum(),
            EnquiryTypeId = (int)bureauEnquiry.EnquiryType,
            FirstName = bureauEnquiry.FirstName,
            IdentityNum = bureauEnquiry.IdentityNum,
            IsSucess = bureauEnquiry.IsSucess,
            LastName = bureauEnquiry.LastName,
            Service = bureauEnquiry.Service.Name,
            ServiceId = bureauEnquiry.Service.ServiceId,
            TransactionType = bureauEnquiry.TransactionType.ToStringEnum(),
            TransactionTypeId = (int)bureauEnquiry.TransactionType,
            Score = score == null ? string.Empty : score.Score,
            Colour = score == null ? string.Empty : string.Format("{0},{1},{2}", score.R, score.G, score.B)
          };

          
          if (bureauEnquiry.PreviousEnquiry == null)
            accountBureau.PreviousEnquiryId = (long?)null;
          else
            accountBureau.PreviousEnquiryId = bureauEnquiry.PreviousEnquiry.EnquiryId;

          var tempBureauAccounts = new XPQuery<BUR_Accounts>(uow).Where(a => a.Enquiry.EnquiryId == bureauEnquiry.EnquiryId).ToList();
          foreach (var tempBureauAccount in tempBureauAccounts)
          {
            var accountBureauAccount = new AccountBureauAccount()
            {
              AccountNo = tempBureauAccount.AccountNo,
              BalanceDate = tempBureauAccount.BalanceDate,
              BureauAccountId = tempBureauAccount.BureauAccountId,
              CreatedDate = tempBureauAccount.CreatedDate,
              CurrentBalance = tempBureauAccount.CurrentBalance,
              Enabled = tempBureauAccount.Enabled,
              Instalment = tempBureauAccount.Instalment,
              JointParticipants = tempBureauAccount.JointParticipants,
              LastPayDate = tempBureauAccount.LastPayDate,
              OpenBalance = tempBureauAccount.OpenBalance,
              OpenDate = tempBureauAccount.OpenDate,
              OverdueAmount = tempBureauAccount.OverdueAmount,
              OverrideDate = tempBureauAccount.OverrideDate,
              Period = tempBureauAccount.Period,
              PeriodType = tempBureauAccount.PeriodType,
              Status = tempBureauAccount.Status,
              StatusDate = tempBureauAccount.StatusDate,
              SubAccountNo = tempBureauAccount.SubAccountNo,
              Subscriber = tempBureauAccount.Subscriber
            };

            if (tempBureauAccount.AccountStatusCode != null)
            {
              accountBureauAccount.AccountStatusCode = tempBureauAccount.AccountStatusCode.Description;
              accountBureauAccount.AccountStatusCodeId = tempBureauAccount.AccountStatusCode.StatusCodeId;
              accountBureauAccount.AccountStatusCodeSortCode = tempBureauAccount.AccountStatusCode.ShortCode;
            }
            if (tempBureauAccount.AccountType != null)
            {
              accountBureauAccount.AccountType = tempBureauAccount.AccountType.Description;
              accountBureauAccount.AccountTypeId = tempBureauAccount.AccountType.TypeCodeId;
            }
            if (tempBureauAccount.CreateUser != null && tempBureauAccount.CreateUser.Security != null)
              accountBureauAccount.CreateUser = tempBureauAccount.CreateUser.Security.Username;
            if (tempBureauAccount.OverrideUser != null && tempBureauAccount.OverrideUser.Security != null)
              accountBureauAccount.OverrideUser = tempBureauAccount.OverrideUser.Security.Username;

            accountBureau.BureauAccounts.Add(accountBureauAccount);
          }

          accountBureauEnquiries.Add(accountBureau);
        }
      }

      return accountBureauEnquiries;
    }

    public static List<AccountAddress> GetAddresses(long accountId)
    {
      var accountAddresses = new List<AccountAddress>();

      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0}, does not exist in the DB", accountId));

        var addresses = account.Person.GetAddressDetails.Select(a=>a.Address).OrderBy(a => a.AddressType.Description).ToList();
        foreach (var address in addresses)
        {
          var accountAddress = new AccountAddress()
          {
            AddressId = address.AddressId,
            AddressType = address.AddressType.Description,
            AddressTypeId = address.AddressType.AddressTypeId,
            AddressLine1 = address.Line1,
            AddressLine2 = address.Line2,
            AddressLine3 = address.Line3,
            AddressLine4 = address.Line4,
            PostalCode = address.PostalCode,
            IsActive = address.IsActive,
            CreateDate = address.CreatedDT
          };
          if (address.Province != null)
          {
            accountAddress.Province = address.Province.Description;
            accountAddress.ProvinceId = address.Province.ProvinceId;
          }

          accountAddresses.Add(accountAddress);
        }
      }
      return accountAddresses;
    }

    public static List<AccountContact> GetContacts(long accountId)
    {
      var accountContacts = new List<AccountContact>();
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0}, does not exist in the DB", accountId));

        var contacts = account.Person.GetContacts.Select(c=>c.Contact).ToList();
        foreach (var contact in contacts)
        {
          var accountContact = new AccountContact()
          {
            ContactId = contact.ContactId,
            ContactType = contact.ContactType.Description,
            ContactTypeId = contact.ContactType.ContactTypeId,
            CreateDate = contact.CreatedDT,
            IsActive = contact.IsActive,
            Value = contact.Value
          };
          accountContacts.Add(accountContact);
        }
      }

      return accountContacts;
    }

    public static List<AccountNote> GetNotes(long accountId, long noteId = 0)
    {
      var accountNotes = new List<AccountNote>();
      using (var uow = new UnitOfWork())
      {
        var noteQuery = new XPQuery<ACC_AccountNote>(uow).Where(a => a.Account.AccountId == accountId);
        if (noteId > 0)
          noteQuery = noteQuery.Where(n => n.Note.NoteId == noteId);
        var notes = noteQuery.OrderBy(n => n.Note.CreateDate).ToList();
        foreach (var note in notes)
        {
          var accountNote = new AccountNote()
          {
            CreateDate = note.Note.CreateDate,
            CreateUser = note.Note.CreateUser.Security == null ? string.Empty : note.Note.CreateUser.Security.Username,
            CreateUserId = note.Note.CreateUser.PersonId,
            DeleteDate = note.Note.DeleteDate == null ? (DateTime?)null : note.Note.DeleteDate,
            DeleteUser = note.Note.DeleteUser == null ? string.Empty : note.Note.DeleteUser.Security.Username,
            LastEditDate = note.Note.LastEditDate == null ? (DateTime?)null : note.Note.LastEditDate,
            Note = note.Note.Note,
            NoteId = note.Note.NoteId,
            ParentNoteId = note.Note.ParentNote == null ? (long?)null : note.Note.ParentNote.NoteId
          };

          accountNotes.Add(accountNote);
        }
      }

      return accountNotes;
    }

    public static List<AccountStatement> GetStatement(long accountId)
    {
      var accountStatement = new List<AccountStatement>();
      using (var uow = new UnitOfWork())
      {
        var transactions = new XPQuery<LGR_Transaction>(uow).Where(t => t.Account.AccountId == accountId).OrderBy(t => t.TransactionDate).ThenBy(t => t.TransactionId).ToList();
        decimal runningBalance = 0;
        foreach (var transaction in transactions)
        {
          var statementTransaction = new AccountStatement()
          {
            Amount = transaction.Amount,
            CreateDate = transaction.CreateDate,
            TransactionDate = transaction.TransactionDate,
            TransactionId = transaction.TransactionId,
            TransactionType = transaction.TransactionType.Description,
            TransactionTypeId = transaction.TransactionType.TransactionTypeId
          };

          if (transaction.Type.Type == GeneralLedger.Type.Debit)
            runningBalance += transaction.Amount;
          else
            runningBalance -= transaction.Amount;

          statementTransaction.RunningBalance = runningBalance;

          accountStatement.Add(statementTransaction);
        }
      }

      return accountStatement;
    }

    public static List<BankDetail> GetBankDetails(long accountId)
    {
      var accountBankDetails = new List<BankDetail>();
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0}, does not exist in the DB", accountId));

        var bankDetails = account.Person.GetBankDetails.Select(b=>b.BankDetail).ToList();
        foreach (var bankDetail in bankDetails)
        {
          var acountBankDetail = new BankDetail()
          {
            AccountName = bankDetail.AccountName,
            AccountNum = bankDetail.AccountNum,
            Bank = new Bank(bankDetail.Bank.Type),
            BankAccountType = bankDetail.AccountType.Description,
            BankAccountTypeId = bankDetail.AccountType.AccountTypeId,
            Code = bankDetail.Code,
            CreatedDT = bankDetail.CreatedDT,
            DetailId = bankDetail.DetailId,
            IsActive = bankDetail.IsActive
          };

          accountBankDetails.Add(acountBankDetail);
        }
      }
      return accountBankDetails;
    }

    public static List<AccountWorklow> GetWorkflow(long accountId)
    {
      var accountWorkflows = new List<AccountWorklow>();
      using (var uow = new UnitOfWork())
      {
        var processStepJobAccounts = new XPQuery<WFL_ProcessStepJobAccount>(uow).Where(p => p.Account.AccountId == accountId).OrderBy(p => p.ProcessStepJob.CreateDate).ToList();
        foreach (var processStepJobAccount in processStepJobAccounts)
        {
          var accountWorkflow = new AccountWorklow()
          {
            ProcessStepJobAccountId = processStepJobAccount.ProcessStepJobAccountId,
            Process = processStepJobAccount.ProcessStepJob.ProcessStep.Process.Name,
            ProcessCompleteDate = processStepJobAccount.ProcessStepJob.ProcessJob.CompleteDate,
            ProcessId = processStepJobAccount.ProcessStepJob.ProcessStep.Process.ProcessId,
            ProcessJobId = processStepJobAccount.ProcessStepJob.ProcessJob.ProcessJobId,
            ProcessJobState = processStepJobAccount.ProcessStepJob.ProcessJob.JobState.Name,
            ProcessJobStateId = processStepJobAccount.ProcessStepJob.ProcessJob.JobState.JobStateId,
            ProcessLastStateDate = processStepJobAccount.ProcessStepJob.ProcessJob.LastStateDate,
            ProcessStep = processStepJobAccount.ProcessStepJob.ProcessStep.Name,
            ProcessStepCompleteDate = processStepJobAccount.ProcessStepJob.CompleteDate,
            ProcessStepId = processStepJobAccount.ProcessStepJob.ProcessStep.ProcessStepId,
            ProcessStepJobId = processStepJobAccount.ProcessStepJob.ProcessStepJobId,
            ProcessStepJobState = processStepJobAccount.ProcessStepJob.JobState.Name,
            ProcessStepJobStateId = processStepJobAccount.ProcessStepJob.JobState.JobStateId,
            ProcessStepLastStateDate = processStepJobAccount.ProcessStepJob.LastStateDate
          };

          accountWorkflows.Add(accountWorkflow);
        }
      }

      return accountWorkflows;
    }

    public static List<AccountStatushistory> GetStatusHistory(long accountId)
    {
      var accountStatusHistory = new List<AccountStatushistory>();
      using (var uow = new UnitOfWork())
      {

        var accountStatuses = new XPQuery<ACC_AccountStatus>(uow).Where(a => a.Account.AccountId == accountId).OrderByDescending(s => s.CreateDate).ToList();
        foreach (var accountStatus in accountStatuses)
        {
          accountStatusHistory.Add(new AccountStatushistory()
          {
            CreateDate = accountStatus.CreateDate,
            Status = accountStatus.Status.Description,
            StatusId = accountStatus.Status.StatusId
          });
        }
      }

      return accountStatusHistory;
    }

    public static List<AccountDebitControl> GetNaedoTransactions(long accountId)
    {
      var accountDebitControls = new List<AccountDebitControl>();
      using (var uow = new UnitOfWork())
      {
        var controls = new XPQuery<ACC_DebitControl>(uow).Where(a => a.Account.AccountId == accountId).Select(c => c.Control).ToList();
        var controlIds = controls.Select(c => c.ControlId).ToList();
        var debitOrderTransactions = new XPQuery<DBT_Transaction>(uow).Where(t => controlIds.Contains(t.Control.ControlId)).ToList();
        foreach (var control in controls)
        {
          var tempDebitOrderTransactions = debitOrderTransactions.Where(t => t.Control.ControlId == control.ControlId).ToList();
          var accountDebitControl = new AccountDebitControl()
          {
            AVSCheckType = control.AVSCheckType.Description,
            AVSCheckTypeId = control.AVSCheckType.AVSCheckTypeId,
            AVSTransactionId = control.AVSTransactionId,
            Bank = new Bank(control.Bank.Type),
            BankAccountName = control.BankAccountName,
            BankAccountNo = control.BankAccountNo,
            BankAccountType = control.BankAccountType.Description,
            BankAccountTypeId = control.BankAccountType.AccountTypeId,
            BankBranchCode = control.BankBranchCode,
            BankStatementReference = control.BankStatementReference,
            ControlId = control.ControlId,
            ControlStatus = control.ControlStatus.Description,
            ControlStatusId = control.ControlStatus.ControlStatusId,
            ControlType = control.ControlType.Description,
            ControlTypeId = control.ControlType.ControlTypeId,
            CreateDate = control.CreateDate,
            CurrentRepetition = control.CurrentRepetition,
            FailureType = control.FailureType.Description,
            FailureTypeId = control.FailureType.FailureTypeId,
            FirstInstalmentDate = control.FirstInstalmentDate,
            IdNumber = control.IdNumber,
            Instalment = control.Instalment,
            IsValid = control.IsValid,
            LastInstalmentUpdate = control.LastInstalmentUpdate,
            LastStatusDate = control.LastStatusDate,
            PayDate = control.PayDate.DayNo,
            PayDateId = control.PayDate.PayDateId,
            PayRule = control.PayRule.Description,
            PayRuleId = control.PayRule.PayRuleId,
            PeriodFrequency = control.PeriodFrequency.Description,
            PeriodFrequencyId = control.PeriodFrequency.PeriodFrequencyId,
            Repetitions = control.Repetitions,
            Service = control.Service.ReferenceName,
            ServiceId = control.Service.ServiceId,
            ThirdPartyReference = control.ThirdPartyReference,
            TrackingDays = control.TrackingDays,
            Transactions = new List<AccountDebitControlTransaction>()
          };
          switch (control.ControlStatus.Type)
          {
            case Debit.ControlStatus.New:
            case Debit.ControlStatus.InProcess:
              accountDebitControl.ControlStatusColor = "label label-info";
              break;
            case Debit.ControlStatus.Cancelled:
              accountDebitControl.ControlStatusColor = "label label-warning";
              break;
            case Debit.ControlStatus.Cancelled_ValidationErrors:
              accountDebitControl.ControlStatusColor = "label label-danger";
              break;
            case Debit.ControlStatus.Completed:
              accountDebitControl.ControlStatusColor = "label label-success";
              break;
            default:
              break;
          }

          foreach (var debitOrderTransaction in tempDebitOrderTransactions)
          {
            var accountDebitControlTransaction = new AccountDebitControlTransaction()
            {
              ActionDate = debitOrderTransaction.ActionDate,
              Amount = debitOrderTransaction.Amount,
              CancelDate = debitOrderTransaction.CancelDate,
              CancelUser = debitOrderTransaction.CancelUser == null ? string.Empty : debitOrderTransaction.CancelUser.Security.Username,
              CreateDate = debitOrderTransaction.CreateDate,
              DebitType = debitOrderTransaction.DebitType.Description,
              DebitTypeId = debitOrderTransaction.DebitType.DebitTypeId,
              InstalmentDate = debitOrderTransaction.InstalmentDate,
              LastStatusDate = debitOrderTransaction.LastStatusDate,
              OverrideActionDate = debitOrderTransaction.OverrideActionDate,
              OverrideAmount = debitOrderTransaction.OverrideAmount,
              OverrideDate = debitOrderTransaction.OverrideDate,
              OverrideTrackingDays = debitOrderTransaction.OverrideTrackingDays,
              OverrideUser = debitOrderTransaction.OverrideUser == null ? string.Empty : debitOrderTransaction.OverrideUser.Security.Username,
              Repetition = debitOrderTransaction.Repetition,
              ResponseDate = debitOrderTransaction.ResponseDate,
              Status = debitOrderTransaction.Status.Description,
              StatusId = debitOrderTransaction.Status.StatusId,
              TransactionId = debitOrderTransaction.TransactionId
            };

            switch (debitOrderTransaction.Status.Type)
            {
              case Debit.Status.New:
              case Debit.Status.OnHold:
                accountDebitControlTransaction.StatusColor = "label label-info";
                break;
              case Debit.Status.Batched:
              case Debit.Status.Submitted:
                accountDebitControlTransaction.StatusColor = "label label-default";
                break;
              case Debit.Status.Cancelled:
                accountDebitControlTransaction.StatusColor = "label label-warning";
                break;
              case Debit.Status.Failed:
              case Debit.Status.Disputed:
                accountDebitControlTransaction.StatusColor = "label label-danger";
                break;
              case Debit.Status.Successful:
                accountDebitControlTransaction.StatusColor = "label label-success";
                break;
              default:
                break;
            }
            if (debitOrderTransaction.Batch != null)
            {
              accountDebitControlTransaction.BatchCreateDate = debitOrderTransaction.Batch.CreateDate;
              accountDebitControlTransaction.BatchId = debitOrderTransaction.Batch.BatchId;
              accountDebitControlTransaction.BatchLastStatusDate = debitOrderTransaction.Batch.LastStatusDate;
              accountDebitControlTransaction.BatchStatus = debitOrderTransaction.Batch.BatchStatus.Description;
              accountDebitControlTransaction.BatchStatusId = debitOrderTransaction.Batch.BatchStatus.BatchStatusId;
              accountDebitControlTransaction.BatchSubmitDate = debitOrderTransaction.Batch.SubmitDate;
              accountDebitControlTransaction.BatchSubmitUser = debitOrderTransaction.Batch.SubmitUser == null ? string.Empty : debitOrderTransaction.Batch.SubmitUser.Security.Username;
            }
            if (debitOrderTransaction.ResponseCode != null)
            {
              accountDebitControlTransaction.ResponseCode = debitOrderTransaction.ResponseCode.Code;
              accountDebitControlTransaction.ResponseCodeDescription = debitOrderTransaction.ResponseCode.Description;
              accountDebitControlTransaction.ResponseCodeId = debitOrderTransaction.ResponseCode.ResponseCodeId;
            }

            accountDebitControl.Transactions.Add(accountDebitControlTransaction);
          }

          accountDebitControls.Add(accountDebitControl);
        }
      }

      return accountDebitControls;
    }

    public static List<AccountPayout> GetPayouts(long accountId)
    {
      var accountDetailPayouts = new List<AccountPayout>();
      using (var uow = new UnitOfWork())
      {
        var payouts = new XPQuery<PYT_Payout>(uow).Where(a => a.Account.AccountId == accountId).ToList();
        foreach (var payout in payouts)
        {
          var accountPayout = new AccountPayout()
          {
            ActionDate = payout.ActionDate,
            Amount = payout.Amount,
            Bank = payout.BankDetail.Bank.Description,
            BankAccountName = payout.BankDetail.AccountName,
            BankAccountNo = payout.BankDetail.AccountNum,
            CreateDate = payout.CreateDate,
            IsValid = payout.IsValid,
            Paid = payout.Paid,
            PaidDate = payout.PaidDate,
            PayoutId = payout.PayoutId,
            PayoutStatus = payout.PayoutStatus.Description,
            PayoutStatusId = payout.PayoutStatus.PayoutStatusId,
            ResultDate = payout.ResultDate,
            ResultMessage = payout.ResultMessage
          };

          switch (payout.PayoutStatus.Status)
          {
            case Payout.PayoutStatus.New:
            case Payout.PayoutStatus.OnHold:
              accountPayout.PayoutStatusColor = "label label-info";
              break;
            case Payout.PayoutStatus.Cancelled:
            case Payout.PayoutStatus.Removed:
              accountPayout.PayoutStatusColor = "label label-warning";
              break;
            case Payout.PayoutStatus.Failed:
              accountPayout.PayoutStatusColor = "label label-danger";
              break;
            case Payout.PayoutStatus.Successful:
              accountPayout.PayoutStatusColor = "label label-success";
              break;
            case Payout.PayoutStatus.Batched:
            case Payout.PayoutStatus.Submitted:
              accountPayout.PayoutStatusColor = "label label-default";
              break;
            default:
              break;
          }
          if (payout.Batch != null)
          {
            accountPayout.BatchAuthoriseDate = payout.Batch.AuthoriseDate;
            accountPayout.BatchCreateDate = payout.Batch.CreateDate;
            accountPayout.BatchId = payout.Batch.BatchId;
            accountPayout.BatchStatus = payout.Batch.BatchStatus.Description;
            accountPayout.BatchStatusId = payout.Batch.BatchStatus.BatchStatusId;
            accountPayout.BatchSubmitDate = payout.Batch.SubmitDate;
            accountPayout.LastBatchStatusDate = payout.Batch.LastStatusDate;
          }
          if (payout.ResultCode != null)
          {
            accountPayout.ResultCode = payout.ResultCode.ResultCode;
            accountPayout.ResultCodeDescription = payout.ResultCode.Description;
            accountPayout.ResultCodeId = payout.ResultCode.ResultCodeId;
          }
          if (payout.ResultQualifier != null)
          {
            accountPayout.ResultQualifier = payout.ResultQualifier.Description;
            accountPayout.ResultQualifierId = payout.ResultQualifier.ResultQualifierId;
          }
          accountDetailPayouts.Add(accountPayout);
        }
      }

      return accountDetailPayouts;
    }

    public static void UpdateAccountStatus(long accountId, long userId, Atlas.Enumerators.Account.AccountStatus newStatus, Atlas.Enumerators.Account.AccountStatusReason? reason, Atlas.Enumerators.Account.AccountStatusSubReason? subReason)
    {
      throw new MissingMethodException();

      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account Id {0}does not exist in DB", accountId));

        if (newStatus == Atlas.Enumerators.Account.AccountStatus.Closed)
        {
          if (account.Status.StatusId < (int)Atlas.Enumerators.Account.AccountStatus.Open)
            throw new Exception(string.Format("Cannot Close account {0}. The account needs to be Open first", account.AccountNo));

          if (account.AccountBalance > account.AccountType.CloseBalance)
            throw new Exception(string.Format("Cannot Close Account {0}. Account balance is still too high", account.AccountNo));
        }
        else if (newStatus == Atlas.Enumerators.Account.AccountStatus.Open)
        {
          throw new Exception(string.Format("Cannot Open Account {0} manually", account.AccountNo));
        }
        else if ((int)newStatus > (int)Atlas.Enumerators.Account.AccountStatus.Open
          && account.Status.StatusId < (int)Atlas.Enumerators.Account.AccountStatus.Open)
        {
          throw new Exception(string.Format("Cannot change to Status {0} from {1}", newStatus.ToStringEnum(), account.Status.Description));
        }
        else if (newStatus == Atlas.Enumerators.Account.AccountStatus.Approved && account.Status.Type == Atlas.Enumerators.Account.AccountStatus.Review)
        {
          // move workflow to payout
          var reviewProcessStepId = (int)Workflow.ProcessStep.Review;
          var jobStateIds = new int[] { (int)Workflow.JobState.Pending, (int)Workflow.JobState.Ready, (int)Workflow.JobState.Executing };
          var processStepJobAccount = new XPQuery<WFL_ProcessStepJobAccount>(uow).FirstOrDefault(a => a.Account.AccountId == account.AccountId && a.ProcessStepJob.ProcessStep.ProcessStepId == reviewProcessStepId
            && a.ProcessStepJob.CompleteDate == null && jobStateIds.Contains(a.ProcessStepJob.JobState.JobStateId));
          if (processStepJobAccount != null)
          {
            //Default.RedirectAccount(processStepJobAccount.ProcessStepJobAccountId, (int)Workflow.ProcessStep.Payout, userId);
          }
        }

        //Atlas.LoanEngine.Account.Default accountUtil = new Atlas.LoanEngine.Account.Default(accountId);
        //accountUtil.SetAccountStatus(account, newStatus, reason, subReason, uow);

        uow.CommitChanges();
      }
    }

    public static AccountNote AddNote(long accountId, long userPersonId, string noteText, long? parentNoteId = null)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist in the DB", accountId));

        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == userPersonId);
        if (person == null)
          throw new Exception(string.Format("Person {0} does not exist in the DB", userPersonId));

        var note = new NTE_Note(uow)
        {
          CreateDate = DateTime.Now,
          CreateUser = person,
          Note = noteText
        };

        if (parentNoteId.HasValue && parentNoteId.Value > 0)
        {
          var parentNote = new XPQuery<NTE_Note>(uow).FirstOrDefault(n => n.NoteId == parentNoteId);
          if (parentNote != null)
            note.ParentNote = parentNote;
        }

        var accountNote = new ACC_AccountNote(uow)
        {
          Account = account,
          Note = note
        };

        uow.CommitChanges();

        return GetNotes(accountId, note.NoteId).FirstOrDefault();
      }
    }

    public static void EditNote(long noteId, long userPersonId, string noteText)
    {
      using (var uow = new UnitOfWork())
      {
        var note = new XPQuery<NTE_Note>(uow).FirstOrDefault(a => a.NoteId == noteId);
        if (note == null)
          throw new Exception(string.Format("Note {0} not found in the DB", noteId));

        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == userPersonId);
        if (person == null)
          throw new Exception(string.Format("Person {0} does not exist in the DB", userPersonId));

        note.Note = noteText;
        note.LastEditDate = DateTime.Now;

        uow.CommitChanges();
      }
    }

    public static void DeleteNote(long noteId, long userPersonId)
    {
      using (var uow = new UnitOfWork())
      {
        var note = new XPQuery<NTE_Note>(uow).FirstOrDefault(a => a.NoteId == noteId);
        if (note == null)
          throw new Exception(string.Format("Note {0} not found in the DB", noteId));

        var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == userPersonId);
        if (person == null)
          throw new Exception(string.Format("Person {0} does not exist in the DB", userPersonId));

        note.DeleteDate = DateTime.Now;
        note.DeleteUser = person;

        uow.CommitChanges();
      }
    }

    public static void AcceptAffordabilityOption(long accountId, int affordabilityOptionId)
    {
      throw new MissingMethodException();
      //var accountDefault = new Atlas.LoanEngine.Affordability.Default(accountId);
      //accountDefault.AcceptOption(affordabilityOptionId);
    }

    public static AccountAffordabilityItem AddAffordabilityItem(long accountId, int affordabilityCategoryId, decimal amount, long personId)
    {
      // can a new affordability item be added?
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Add Affordability Item: Cannot find account {0} in DB", accountId));

        if ((int)account.Status.Type >= (int)Atlas.Enumerators.Account.AccountStatus.Open)
          throw new Exception(string.Format("Add Affordability Item: Cannot add item to an open account {0}", accountId));

        var affordabilityOptionsCount = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => a.Account == account &&
          (a.AffordabilityOptionStatus.AffordabilityOptionStatusId == (int)Atlas.Enumerators.Account.AffordabilityOptionStatus.Accepted
          || a.AffordabilityOptionStatus.AffordabilityOptionStatusId == (int)Atlas.Enumerators.Account.AffordabilityOptionStatus.Cancelled
          || a.AffordabilityOptionStatus.AffordabilityOptionStatusId == (int)Atlas.Enumerators.Account.AffordabilityOptionStatus.Rejected)).Count();

        if (affordabilityOptionsCount > 0)
          throw new Exception(string.Format("Add Affordability Item: Cannot add item to Account {0}", accountId));
      }

      throw new MissingMethodException();
      //var accountDefault = new Atlas.LoanEngine.Affordability.Default(accountId);
      //var affordabilityItem = accountDefault.AddAffordabilityItem(affordabilityCategoryId, amount, personId);
      //accountDefault = null;

      //var affordabilityCalculator = new AffordabilityCalculator();
      //affordabilityCalculator.CalculateAffordabilityOptions(accountId, personId);

      //return MapToAffordability(affordabilityItem);
    }

    public static AccountAffordabilityItem DeleteAffordabilityItem(long accountId, long affordabilityId, long personId)
    {
      throw new MissingMethodException();
      //var accountDefault = new Atlas.LoanEngine.Affordability.Default(accountId);
      //var affordabilityItem = accountDefault.DeleteAffordabilityItem(affordabilityId, personId);
      //accountDefault = null;

      //return MapToAffordability(affordabilityItem);
    }

    public static List<AffordabilityCategory> GetAffordabilityCategories(General.Host host)
    {
      var categories = new List<AffordabilityCategory>();

      using (var uow = new UnitOfWork())
      {
        var affordabilityCategories = new XPQuery<ACC_AffordabilityCategory>(uow).Where(a => a.Enabled && a.Host.Type == host).OrderBy(a => a.Ordinal).ToList();
        foreach (var affordabilityCategory in affordabilityCategories)
        {
          categories.Add(new AffordabilityCategory()
            {
              AffordabilityCategoryId = affordabilityCategory.AffordabilityCategoryId,
              Description = affordabilityCategory.Description,
              Type = affordabilityCategory.AffordabilityCategoryType.ToStringEnum(),
              TypeId = (int)affordabilityCategory.AffordabilityCategoryType
            });
        }
      }

      return categories;
    }

    public static ACC_AccountDTO AcceptQuotation(long accountId, long quotationId)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist", accountId));

        var quotation = new XPQuery<ACC_Quotation>(uow).FirstOrDefault(q => q.QuotationId == quotationId && q.Account == account && (q.QuotationStatus.Type == Atlas.Enumerators.Account.QuotationStatus.Issued || q.QuotationStatus.Type == Atlas.Enumerators.Account.QuotationStatus.New));

        if (quotation == null)
          throw new Exception(string.Format("Pending Quotation {0} for Account {1} does not exist", quotationId, accountId));

        if (quotation.ExpiryDate.Date < DateTime.Today.Date)
        {
          quotation.QuotationStatus = new XPQuery<ACC_QuotationStatus>(uow).FirstOrDefault(q => q.Type == Atlas.Enumerators.Account.QuotationStatus.Expired);
          quotation.LastStatusDate = DateTime.Now;
          uow.CommitChanges();
          throw new Exception(string.Format("Quotation {0} for Account {1} has expired", quotation.QuotationNo, account.AccountNo));
        }

        quotation.QuotationStatus = new XPQuery<ACC_QuotationStatus>(uow).FirstOrDefault(q => q.Type == Atlas.Enumerators.Account.QuotationStatus.Accepted);
        quotation.LastStatusDate = DateTime.Now;

        throw new MissingMethodException();
        //var accountUtil = new Atlas.LoanEngine.Account.Default(accountId);
        //accountUtil.ApproveAccount(account, uow);

        //uow.CommitChanges();

        //Default.StepProcess(accountId, (int)Workflow.ProcessStep.Quotation, (int)Workflow.ProcessStep.Payout);

        return Mapper.Map<ACC_Account, ACC_AccountDTO>(account);
      }
    }

    public static ACC_AccountDTO RejectQuotation(long accountId, long quotationId)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new Exception(string.Format("Account {0} does not exist", accountId));

        var quotation = new XPQuery<ACC_Quotation>(uow).FirstOrDefault(q => q.QuotationId == quotationId &&
          q.Account == account &&
          (q.QuotationStatus.Type == Atlas.Enumerators.Account.QuotationStatus.Issued
          || q.QuotationStatus.Type == Atlas.Enumerators.Account.QuotationStatus.New));

        if (quotation == null)
          throw new Exception(string.Format("Pending Quotation {0} for Account {1} does not exist", quotationId, accountId));

        quotation.QuotationStatus = new XPQuery<ACC_QuotationStatus>(uow).FirstOrDefault(q => q.Type == Atlas.Enumerators.Account.QuotationStatus.Rejected);
        quotation.LastStatusDate = DateTime.Now;

        throw new MissingMethodException();
        //new Atlas.LoanEngine.Account.Default().SetAccountStatus(account, Atlas.Enumerators.Account.AccountStatus.Cancelled, Atlas.Enumerators.Account.AccountStatusReason.RejectedOptions, null, uow);

        uow.CommitChanges();

        return Mapper.Map<ACC_Account, ACC_AccountDTO>(account);
      }
    }

    public static ACC_AccountDTO AdjustAccount(long accountId, decimal loanAmount, int period)
    {
      using (var uow = new UnitOfWork())
      {
        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == accountId);
        if (account == null)
          throw new RecordNotFoundException(string.Format("Account {0} not found", accountId));

        if (account.Status.StatusId >= (int)Atlas.Enumerators.Account.AccountStatus.Open)
          throw new Exception("Account is already opened");

        // Reject accepted/new/issued quotations
        var quotations = new XPQuery<ACC_Quotation>(uow).Where(a => a.Account.AccountId == accountId).ToList();
        foreach (var quotation in quotations)
        {
          if (quotation.QuotationStatus.Type == Atlas.Enumerators.Account.QuotationStatus.Accepted
            || quotation.QuotationStatus.Type == Atlas.Enumerators.Account.QuotationStatus.New
            || quotation.QuotationStatus.Type == Atlas.Enumerators.Account.QuotationStatus.Issued)
          {
            quotation.QuotationStatus = new XPQuery<ACC_QuotationStatus>(uow).FirstOrDefault(q => q.Type == Atlas.Enumerators.Account.QuotationStatus.Rejected);
            quotation.LastStatusDate = DateTime.Now;
          }
        }

        // cancel new/sent/accepted affordability options
        var affordabilityOptions = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => a.Account.AccountId == accountId).ToList();
        foreach (var affordabilityOption in affordabilityOptions)
        {
          if (affordabilityOption.AffordabilityOptionStatus.Type == Atlas.Enumerators.Account.AffordabilityOptionStatus.New
            || affordabilityOption.AffordabilityOptionStatus.Type == Atlas.Enumerators.Account.AffordabilityOptionStatus.Sent
            || affordabilityOption.AffordabilityOptionStatus.Type == Atlas.Enumerators.Account.AffordabilityOptionStatus.Accepted)
          {
            affordabilityOption.AffordabilityOptionStatus = new XPQuery<ACC_AffordabilityOptionStatus>(uow).FirstOrDefault(a => a.Type == Atlas.Enumerators.Account.AffordabilityOptionStatus.Cancelled);
            affordabilityOption.LastStatusDate = DateTime.Now;
          }
        }

        account.LoanAmount = loanAmount;
        account.Period = period;

        uow.CommitChanges();

        throw new MissingMethodException();
        //var requestedOption = new AffordabilityCalculator().CalculateAffordabilityOptions(accountId, (long)General.Person.System, true).FirstOrDefault(a => a.AffordabilityOptionType.Type == Atlas.Enumerators.Account.AffordabilityOptionType.RequestedOption);

        //if (requestedOption.CanClientAfford ?? false)
        //  new Atlas.LoanEngine.Affordability.Default(accountId).AcceptOption(requestedOption.AffordabilityOptionId);

        return Mapper.Map<ACC_Account, ACC_AccountDTO>(account);
      }
    }

    private static List<AccountHistory> GetAccountHistory(long personId, long accountId)
    {
      List<AccountHistory> historyCollection = new List<AccountHistory>();

      using (var uow = new UnitOfWork())
      {
        var accountCollection = new XPQuery<ACC_Account>(uow).Where(p => p.Person.PersonId == personId && p.AccountId != accountId).ToList();

        foreach (var history in accountCollection)
        {
          historyCollection.Add(new AccountHistory()
          {
            AccountId = history.AccountId,
            AccountNo = history.AccountNo,
            AccountType = history.AccountType.Description,
            Balance = history.AccountBalance,
            CreateDate = history.CreateDate,
            Host = history.Host.Type.ToStringEnum(),
            LoanAmount = history.LoanAmount,
            OpenDate = history.OpenDate,
            PersonId = personId,
            Status = history.Status.Type.ToStringEnum()
          });
        }
      }
      return historyCollection;
    }

    private static AccountAffordabilityItem MapToAffordability(ACC_AffordabilityDTO affordabilityItem)
    {
      var item = new AccountAffordabilityItem()
      {
        AffordabilityId = affordabilityItem.AffordabilityId,
        Amount = affordabilityItem.Amount,
        Category = affordabilityItem.AffordabilityCategory.Description,
        Type = affordabilityItem.AffordabilityCategory.AffordabilityCategoryType.ToStringEnum(),
        CreateDate = affordabilityItem.CreateDate,
        CreateUser = affordabilityItem.CreateUser == null ? string.Empty : affordabilityItem.CreateUser.Security.Username,
        DeleteDate = affordabilityItem.DeleteDate,
        DeleteUser = affordabilityItem.DeleteUser == null ? string.Empty : affordabilityItem.DeleteUser.Security.Username
      };
      switch (affordabilityItem.AffordabilityCategory.AffordabilityCategoryType)
      {
        case Atlas.Enumerators.Account.AffordabilityCategoryType.Income:
          item.TypeColor = "label label-success";
          break;
        case Atlas.Enumerators.Account.AffordabilityCategoryType.Expense:
          item.TypeColor = "label label-danger";
          break;
        case Atlas.Enumerators.Account.AffordabilityCategoryType.Display:
          item.TypeColor = "label label-info";
          break;
        default: item.TypeColor = "";
          break;
      }

      return item;
    }

    private static List<Relation> GetPersonRelations(long personId)
    {
      List<Relation> relationsCollection = new List<Relation>();

      using (var uow = new UnitOfWork())
      {
        var relationCollection = new XPQuery<PER_Relation>(uow).Where(p => p.Person.PersonId == personId).ToList();
        foreach (var relation in relationCollection)
        {
          if (relation.RelationPerson != null && relation.Relation != null)
          {
            var rl = new Relation();
            rl.FirstName = relation.RelationPerson.Firstname;
            rl.LastName = relation.RelationPerson.Lastname;

            rl.PersonId = relation.RelationPerson.PersonId;
            rl.RelationTypeId = (int)relation.Relation.Type;
            rl.RelationType = relation.Relation.Type.ToStringEnum();
            var contact = relation.RelationPerson.GetContacts.Select(c=>c.Contact).FirstOrDefault(p => p.ContactType.ContactTypeId == General.ContactType.CellNo.ToInt());
            if (contact != null)
              rl.CellNo = contact.Value;

            relationsCollection.Add(rl);
          }
        }
      }
      return relationsCollection;
    }

    private static List<Employer> GetEmployers(long personId)
    {
      List<Employer> employerCollection = new List<Employer>();

      using (var uow = new UnitOfWork())
      {
        Func<CPY_Company, Employer> AddEmployer = (emp) =>
        {
          Employer employer = new Employer();
          employer.CompanyId = emp.CompanyId;
          employer.Name = emp.Name;
          employer.NoOfBranches = new XPQuery<BRN_Branch>(uow).Count(p => p.Company.CompanyId == emp.CompanyId);
          employer.Contacts = new List<Common.Structures.Contact>();
          employer.Addresses = new List<Address>();


          emp.GetContacts.Select(c=>c.Contact).ToList().ForEach(cpy =>
          {
            employer.Contacts.Add(new Common.Structures.Contact()
            {
              DebtorContactId = cpy.ContactId,
              Value = cpy.Value
              //ContactType = cpy.ContactType.ContactTypeId,
              //des = cpy.ContactType.Description
            });
          });

          emp.GetAddresses.Select(a=>a.Address).ToList().ForEach(adr =>
          {
            var address = new Address()
            {
              AddressId = adr.AddressId,
              AddressTypeId = adr.AddressType.AddressTypeId,
              AddressType = new AddressType(),
              Line1 = adr.Line1,
              Line2 = adr.Line2,
              Line3 = adr.Line3,
              Line4 = adr.Line4,
              PostalCode = adr.PostalCode,
              Province = new Province(),
              IsActive = adr.IsActive
            };
            address.AddressType.Description = adr.AddressType.Description;
            address.Province.Description = adr.Province.Description;
            address.Province.ShortCode= adr.Province.ShortCode;
            employer.Addresses.Add(address);
          });

          return employer;
        };

        var _person = new XPQuery<PER_Person>(uow).First(p => p.PersonId == personId);

        if (_person.Employer != null)
        {
          if (employerCollection.All(p => p.CompanyId != _person.Employer.CompanyId))
            employerCollection.Add(AddEmployer(_person.Employer));

          _person.GetEmploymentHistory.ToList().ForEach(cpy =>
          {
            if (employerCollection.All(p => p.CompanyId != cpy.Company.CompanyId))
            {
              employerCollection.Add(AddEmployer(cpy.Company));
            }
          });
        }
      }
      return employerCollection;
    }
  }
}