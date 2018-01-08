using Atlas.Enumerators;
using Atlas.Online.Data;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Resources;
using Atlas.Online.Web.Validations;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Atlas.Online.Web.Models.Dto
{
  public class BankDetailDto
  {  
    [Required]
    [Display(Name="Bank")]
    public WebEnumerators.BankName BankName { get; set; }
    [Required]
    [Display(Name = "Account Type")]
    public General.BankAccountType AccountType { get; set; }
    
    [Display(Name = "Bank Period")]
    [Required]
    public General.BankPeriod? BankPeriod { get; set; }
    [Required]
    [NgStringLength(20)]
    [Display(Name = "Account Holder")]
    public string AccountHolder { get; set; }
    [Required]
    [RegularExpression(@"^[0-9]{5,}$", ErrorMessage = "{0} must only contain numbers and be at least 5 characters.")]
    [NgStringLength(13)]
    [ClientSide(typeof(AccountNoValidations))]
    [Display(Name = "Account Number")]
    public string AccountNo { get; set; }

    public static BankDetailDto Create(Application application, Func<BankDetail, bool> predicate = null)
    {
      BankDetailDto result = new BankDetailDto();

      var bankDetail = application.BankDetail;
      if (bankDetail == null)
      {
        bankDetail = application.Client.BankDetails.FirstOrDefault(predicate);
        if (bankDetail == null)
        {
          return result;
        }
      }

      if (bankDetail.Bank != null)
      {
        result.BankName =  (WebEnumerators.BankName)(int)bankDetail.Bank.Type;
      }
      if (bankDetail.Period != null)
      {
        result.BankPeriod = bankDetail.Period.Type;
      }
      result.AccountNo = bankDetail.AccountNo;
      if (bankDetail.AccountType != null)
      {
        result.AccountType = bankDetail.AccountType.Type;
      }
      result.AccountHolder = bankDetail.AccountName;

      return result;
    }

    public static BankDetailDto Create(BankDetail bank)
    {
      return new BankDetailDto()
      {
        BankName = (WebEnumerators.BankName)(int) (bank.Bank != null ? bank.Bank.Type : default(General.BankName)),
        BankPeriod = bank.Period != null ? bank.Period.Type : default(General.BankPeriod),
        AccountType = bank.AccountType != null ? bank.AccountType.Type : default(General.BankAccountType),
        AccountNo = bank.AccountNo,
        AccountHolder = bank.AccountName
      };
    }

    /// <summary>
    /// Saves the Bank details to an Application instance
    /// </summary>
    /// <param name="application">Application instance</param>
    /// <returns>true if the Application needed to be saved to, otherwise false if no save was necessary.</returns>
    public bool SaveApplication(ref Application application)
    {      
      if (application == null || application.Client == null)
      {
        return false;
      }

      var saved = false;

      var session = application.Session;
      
      // Load from application
      var bankDetail = application.BankDetail;
      // otherwise load from the client
      if (bankDetail == null)
      {
        bankDetail = application.Client.BankDetails.FirstOrDefault(x => x.IsEnabled);
        if (bankDetail != null)
        {
          application.BankDetail = bankDetail;
          application.BankDetail.Save();
        }
      }

      bool changed = !this.Equals(bankDetail);

      if (bankDetail != null && changed)
      {
        // Disable old banking details
        bankDetail.IsEnabled = false;
        bankDetail.Save();
      }

      if (bankDetail == null || changed)
      {
        saved = true;

        bankDetail = ToBankDetail(session);

        application.BankDetail = bankDetail;
        application.Client.BankDetails.Add(bankDetail);
        application.Client.Save();
      }

      return saved;
    }

    public BankDetail ToBankDetail(Session session)
    {
      return new BankDetail(session)
      {
        AccountName = this.AccountHolder,
        AccountNo = this.AccountNo,
        //Edited By Prashant
        AccountType = new XPQuery<BankAccountType>(session).FirstOrDefault(b => b.AccountTypeId == (int)this.AccountType), //new XPQuery<BankAccountType>(session).FirstOrDefault(b => b.Type == this.AccountType),
        Bank = new XPQuery<Bank>(session).FirstOrDefault(b => b.BankId == (int)this.BankName && b.Enabled), //Bank = new XPQuery<Bank>(session).FirstOrDefault(b => b.BankId == Convert.ToInt32(this.BankName) && b.Enabled),
        Period = new XPQuery<BankPeriod>(session).FirstOrDefault(b => b.PeriodId == (int)this.BankPeriod), // Period = new XPQuery<BankPeriod>(session).FirstOrDefault(b => b.Type == this.BankPeriod)
          CreateDate = DateTime.Now,
        IsEnabled = true
      };
    }

    public bool Equals(BankDetail obj)
    {
      if (obj == null)
      {
        return false;
      }

      if (obj.AccountType == null || obj.Bank == null) 
      {
        throw new InvalidOperationException("BankDetails object is incomplete or invalid.");
      }

      return (
        obj.AccountName == this.AccountHolder &&
        obj.AccountNo == this.AccountNo &&
        obj.AccountType.Type == this.AccountType &&
        Convert.ToInt32(obj.Bank.Type) == Convert.ToInt32(this.BankName) &&
        (
          (!this.BankPeriod.HasValue && obj.Period == null) ||
          (this.BankPeriod.HasValue && (obj.Period != null && obj.Period.Type == this.BankPeriod.Value))
        )
      );        
    }

    internal class AccountNoValidations : IClientSideValidationsProvider
    {
      public IEnumerable<ClientSideValidation> GetValidations()
      {
        yield return new ClientSideValidation()
        {
          ResourceName = "Validation_AccountNumberInvalid",
          ResourceType = typeof(ErrorMessages),
          ValidationType = ClientSideValidationType.RemoteValidity
        };
        yield return new ClientSideValidation() 
        {
          ResourceName = "Validation_RemoteVerifyFailed",
          ResourceType = typeof(ErrorMessages),
          ValidationType = ClientSideValidationType.RemoteValidityFailed
        };
      }
    }

  }
}