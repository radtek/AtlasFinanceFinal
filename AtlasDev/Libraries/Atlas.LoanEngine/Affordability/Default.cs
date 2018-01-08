using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.DTO;
using Atlas.Domain.Model;


namespace Atlas.LoanEngine.Affordability
{
  public class Default
  {
    private long _accountId { get; set; }

    public Default(long accountId)
    {
      if (accountId == 0)
        throw new Exception("AccountId cannot be 0");

      _accountId = accountId;
    }

    /// <summary>
    /// Accepts the specified option, with an option to cancel ALL the other pending (new/sent) options
    /// </summary>
    public void AcceptOption(long affordabilityOptionId, bool cancelOtherOptions = true)
    {
      UpdateOptionWithStatus(affordabilityOptionId, Enumerators.Account.AffordabilityOptionStatus.Accepted);

      if (cancelOtherOptions)
      {
        using (var uow = new UnitOfWork())
        {
          var options = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => a.Account.AccountId == _accountId
            && (a.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.New
              || a.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.Sent)).ToList();

          var rejectedStatus = new XPQuery<ACC_AffordabilityOptionStatus>(uow).FirstOrDefault(a => a.Type == Enumerators.Account.AffordabilityOptionStatus.Cancelled);

          options.ForEach((a) =>
            {
              a.AffordabilityOptionStatus = rejectedStatus;
              a.LastStatusDate = DateTime.Now;
            });

          uow.CommitChanges();
        }
      }
    }


    /// <summary>
    /// Adds an affordability item to account
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="personId"></param>
    public ACC_AffordabilityDTO AddAffordabilityItem(int affordabilityCategoryId, decimal amount, long personId, long? topUpId = null)
    {
      using (var uow = new UnitOfWork())
      {
        if (amount <= 0)
          throw new Exception(string.Format("Affordability: Account{0}, Amount {1} too long", _accountId, amount));

        var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == _accountId);
        if (account == null)
          throw new Exception(string.Format("Affordability: Account {0} not found", _accountId));

        var createUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
        var affordabilityCategory = new XPQuery<ACC_AffordabilityCategory>(uow).FirstOrDefault(a => a.AffordabilityCategoryId == affordabilityCategoryId);
        if (affordabilityCategory == null)
          throw new Exception(string.Format("Affordability: Account {0}, Affordability Category {1} not found", _accountId, affordabilityCategoryId));

        var affordability = new ACC_Affordability(uow)
        {
          Account = account,
          AffordabilityCategory = affordabilityCategory,
          Amount = amount,
          CreateDate = DateTime.Now,
          CreateUser = createUser
        };

        uow.CommitChanges();

        return AutoMapper.Mapper.Map<ACC_Affordability, ACC_AffordabilityDTO>(affordability);
      }
    }


    /// <summary>
    /// "Deletes" an item from the account Affordability
    /// </summary>
    /// <param name="affordabilityId"></param>
    /// <param name="personId"></param>
    public ACC_AffordabilityDTO DeleteAffordabilityItem(long affordabilityId, long personId)
    {
      using (var uow = new UnitOfWork())
      {
        var affordability = new XPQuery<ACC_Affordability>(uow).FirstOrDefault(a => a.AffordabilityId == affordabilityId
          && a.Account.AccountId == _accountId);

        if (affordability == null)
          throw new Exception(string.Format("Affordability: Account {0}, Affordability {1} not found", _accountId, affordabilityId));

        var deleteUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
        if (deleteUser==null)
          throw new Exception(string.Format("Affordability: Account {0}, Person {1} not found", _accountId, personId));

        affordability.DeleteDate = DateTime.Now;
        affordability.DeleteUser = deleteUser;

        uow.CommitChanges();

        return AutoMapper.Mapper.Map<ACC_Affordability, ACC_AffordabilityDTO>(affordability);
      }
    }


    /// <summary>
    /// Rejects the specified option
    /// </summary>
    public void RejectOption(long affordabilityOptionId)
    {
      UpdateOptionWithStatus(affordabilityOptionId, Enumerators.Account.AffordabilityOptionStatus.Rejected);
    }


    /// <summary>
    /// Cancels the specified option
    /// </summary>
    public void CancelOption(long affordabilityOptionId)
    {
      UpdateOptionWithStatus(affordabilityOptionId, Enumerators.Account.AffordabilityOptionStatus.Cancelled);
    }


    /// <summary>
    /// Resets the specified option
    /// Cannot reset an option in if theres an accepted option selected for this account
    /// </summary>
    public void ResetOption(long affordabilityOptionId)
    {
      using (var uow = new UnitOfWork())
      {
        var acceptedOptions = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => a.Account.AccountId == _accountId
          && a.AffordabilityOptionId != affordabilityOptionId
          && a.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.Accepted).Count();

        if (acceptedOptions > 0)
          throw new Exception("Cannot reset option, there's already another accepted option");

        var thisOption = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => a.Account.AccountId == _accountId
          && a.AffordabilityOptionId == affordabilityOptionId).FirstOrDefault();

        if (thisOption == null)
          throw new Exception("Affordability Option does not exist for this account");

        if (thisOption.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.Sent ||
          thisOption.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.New)
          throw new Exception("Cannot reset this option, its already New/Sent");
      }

      UpdateOptionWithStatus(affordabilityOptionId, Enumerators.Account.AffordabilityOptionStatus.New, true);
    }


    /// <summary>
    /// Cancels all pending options for account
    /// </summary>
    public void CancelAll()
    {
      UpdateAllPendingOptions(Enumerators.Account.AffordabilityOptionStatus.Cancelled);
    }


    /// <summary>
    /// Cancels all pending options for account
    /// </summary>
    public void RejectAll()
    {
      UpdateAllPendingOptions(Enumerators.Account.AffordabilityOptionStatus.Rejected);
    }


    /// <summary>
    /// Updates all pending options for account to a specified status
    /// </summary>
    /// <param name="status"></param>
    private void UpdateAllPendingOptions(Enumerators.Account.AffordabilityOptionStatus status)
    {
      using (var uow = new UnitOfWork())
      {
        var options = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => a.Account.AccountId == _accountId
          && (a.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.New
            || a.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.Sent)).ToList();

        options.ForEach((item) =>
        {
          item.AffordabilityOptionStatus = new XPQuery<ACC_AffordabilityOptionStatus>(uow).FirstOrDefault(a => a.Type == status);
          item.LastStatusDate = DateTime.Now;
        });

        uow.CommitChanges();
      }
    }


    /// <summary>
    /// Updates the option with a status.
    /// </summary>
    /// <param name="affordabilityId"></param>
    /// <param name="status"></param>
    private void UpdateOptionWithStatus(long affordabilityOptionId, Enumerators.Account.AffordabilityOptionStatus status, bool reset = false)
    {
      using (var uow = new UnitOfWork())
      {
        var optionQ = new XPQuery<ACC_AffordabilityOption>(uow).Where(a => a.AffordabilityOptionId == affordabilityOptionId
          && a.Account.AccountId == _accountId);

        if (!reset && status!= Enumerators.Account.AffordabilityOptionStatus.Accepted)
          optionQ = optionQ.Where(a => a.AffordabilityOptionStatus.AffordabilityOptionStatusId == (int)Enumerators.Account.AffordabilityOptionStatus.New
            || a.AffordabilityOptionStatus.AffordabilityOptionStatusId == (int)Enumerators.Account.AffordabilityOptionStatus.Sent);

        var option = optionQ.FirstOrDefault();

        if (option == null)
          throw new Exception("Specified option does not exist");

        // Cater for Atlas Online. If affordability is accepted twice..
        if (option.AffordabilityOptionStatus.Type == Enumerators.Account.AffordabilityOptionStatus.Accepted
          && status == Enumerators.Account.AffordabilityOptionStatus.Accepted)
          return;

        option.AffordabilityOptionStatus = new XPQuery<ACC_AffordabilityOptionStatus>(uow).FirstOrDefault(a => a.Type == status);
        option.LastStatusDate = DateTime.Now;

        if (status == Enumerators.Account.AffordabilityOptionStatus.Accepted)
        {
          option.Account.LoanAmount = option.Amount;
          option.Account.LoanAmount = option.Amount;
          option.Account.TotalFees = option.TotalFees;
          option.Account.CapitalAmount = option.CapitalAmount;
          option.Account.InstalmentAmount = Convert.ToDecimal(option.Instalment);
          option.Account.NumOfInstalments = option.NumOfInstalment;
          option.Account.AccountBalance = option.CapitalAmount;
          option.Account.InterestRate = option.InterestRate ?? 0;
          option.Account.AccountType = option.AccountType;

          // Create Quotation
          var newOrPendingQuotations = new XPQuery<ACC_Quotation>(uow).Where(q => q.Account == option.Account &&
            (q.QuotationStatus.QuotationStatusId != (int)Enumerators.Account.QuotationStatus.Rejected
              || q.QuotationStatus.QuotationStatusId != (int)Enumerators.Account.QuotationStatus.Expired)).ToList();

          if (newOrPendingQuotations.Count > 0)
            throw new Exception(string.Format("Cannot Create A quotation for Account {0}. There is already another Quotation Pending", option.Account.AccountId));

          var newQuotation = new ACC_Quotation(uow);
          newQuotation.Account = option.Account;
          newQuotation.AffordabilityOption = option;
          newQuotation.QuotationStatus = new XPQuery<ACC_QuotationStatus>(uow).FirstOrDefault(q => q.QuotationStatusId == (int)Enumerators.Account.QuotationStatus.New);
          newQuotation.LastStatusDate = DateTime.Now;
          newQuotation.CreateDate = DateTime.Now;
          newQuotation.CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == (int)Enumerators.General.Person.System);
          newQuotation.ExpiryDate = DateTime.Today.Date.AddDays(option.AccountType.QuotationExpiryPeriod);
          
          uow.CommitChanges();

         newQuotation.QuotationNo = GenerateQuotationNo(AutoMapper.Mapper.Map<ACC_Quotation, ACC_QuotationDTO>(newQuotation));
        }

        uow.CommitChanges();
      }
    }


    private string GenerateQuotationNo(ACC_QuotationDTO quotation)
    {
      string code = string.Empty;

      if (quotation.Account.Host.Type == Enumerators.General.Host.ASS)
        code = "ASS";
      else if (quotation.Account.Host.Type == Enumerators.General.Host.Atlas_Online)
        code = "AOL";

      return string.Format("{0}{1}{2}", code, quotation.QuotationId.ToString("D7"), "Q");
    }

  }
}