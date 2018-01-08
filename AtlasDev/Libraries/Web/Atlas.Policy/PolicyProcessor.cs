using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using DevExpress.Xpo;
using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Policy
{
  public sealed class PolicyProcessor
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(PolicyProcessor));

    #region Decline Day Lengths

    private const int AFFORDABILITY_DELAY = 30;
    private const int COMPANY_POLICY_DELAY = 30;
    private const int CREDIT_RISK_DELAY = 30;
    private const int FRAUD_DELAY = 90;
    private const int AUTHENICATION_DELAY = 30;

    #endregion

    #region Policy Constants

    private const int OVERAGE_LIMIT = 65;
    private const int UNDERAGE_LIMIT = 21;

    #endregion

    public List<Enumerators.Account.Policy> CompanyPolicies(long accountId, UnitOfWork uow)
    {
      List<Enumerators.Account.Policy> policies = new List<Enumerators.Account.Policy>();

      var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(p => p.AccountId == accountId);

      if (account == null)
      {
        _log.Fatal(string.Format("Account [{0}] was not found in the database.", accountId));
        throw new Exception(string.Format("Account [{0}] was not found in the database.", accountId));
      }

      #region Age Policy

      if (GetPersonAge(account.Person.DateOfBirth) >= OVERAGE_LIMIT)
      {
        policies.Add(Enumerators.Account.Policy.AgeMax65Years);

        new ACC_AccountPolicy(uow)
        {
          Account = account,
          Policy = new XPQuery<ACC_Policy>(uow).FirstOrDefault(p => p.Type == Enumerators.Account.Policy.AgeMax65Years && p.Enabled),
          CreateDate = DateTime.Now
        };
      }
      else if (GetPersonAge(account.Person.DateOfBirth) <= UNDERAGE_LIMIT)
      {
        policies.Add(Enumerators.Account.Policy.AgeMinimum21OfYears);

        new ACC_AccountPolicy(uow)
        {
          Account = account,
          Policy = new XPQuery<ACC_Policy>(uow).FirstOrDefault(p => p.Type == Enumerators.Account.Policy.AgeMinimum21OfYears && p.Enabled),
          CreateDate = DateTime.Now
        };
      }

      #endregion

      return policies;
    }

    public List<ACC_PolicyDTO> AccountPolicies(long personId, UnitOfWork uow)
    {
      List<ACC_PolicyDTO> policyCollection = new List<ACC_PolicyDTO>();
      // Check accounts        
      _log.Info(string.Format("Checking policies for person [{0}]..", personId));

      var accountCollection = new XPQuery<ACC_Account>(uow).Where(a => a.Person.PersonId == personId && a.Status.Type != Enumerators.Account.AccountStatus.Technical_Error).ToList();

      _log.Info(string.Format("Found [{0}] account(s) for person [{1}]", accountCollection.Count, personId));

      if (accountCollection.Count > 1)
      {
        var lastAccount = accountCollection.OrderBy(o => o.CreateDate).FirstOrDefault();

        _log.Info(string.Format("Last account for person [{0}] has a status of [{1}]", personId, lastAccount.Status.Type.ToStringEnum()));

        switch (lastAccount.Status.Type)
        {
          case Atlas.Enumerators.Account.AccountStatus.Inactive:
            policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Account_Status_Inactive.ToStringEnum() });
            break;
          case Atlas.Enumerators.Account.AccountStatus.Pending:
            policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Account_Status_Pending.ToStringEnum() });
            break;
          case Atlas.Enumerators.Account.AccountStatus.Cancelled:
            return null;
          case Atlas.Enumerators.Account.AccountStatus.Declined:
            // Determine decline reason
            int? total = null;

            if (lastAccount.StatusReason.Type == Enumerators.Account.AccountStatusReason.Fraud)
            {
              total = FRAUD_DELAY;
              if (lastAccount.StatusSubReason.Type == Enumerators.Account.AccountStatusSubReason.PersonalSuspect ||
                 lastAccount.StatusSubReason.Type == Enumerators.Account.AccountStatusSubReason.PersonalConfirmed)
              {
                if (GetDay((int)total, lastAccount.StatusChangeDate) >= 0)
                  policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Fraud_Alert_Declined_Previous_Account.ToStringEnum() });
              }
            }
            else if (lastAccount.StatusReason.Type == Enumerators.Account.AccountStatusReason.CreditRisk)
            {
              total = CREDIT_RISK_DELAY;
              if (GetDay((int)total, lastAccount.StatusChangeDate) >= 0)
                policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Client_Credit_Risk.ToStringEnum() });
            }
            else if (lastAccount.StatusReason.Type == Enumerators.Account.AccountStatusReason.CompanyPolicy)
            {
              total = COMPANY_POLICY_DELAY;
              if (GetDay((int)total, lastAccount.StatusChangeDate) >= 0)
                policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Company_Policy.ToStringEnum() });
            }
            else if (lastAccount.StatusReason.Type == Enumerators.Account.AccountStatusReason.Affordability)
            {
              total = AFFORDABILITY_DELAY;
              if (GetDay((int)total, lastAccount.StatusChangeDate) >= 0)
                policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Company_Policy.ToStringEnum() });
            }
            else if (lastAccount.StatusReason.Type == Enumerators.Account.AccountStatusReason.Authentication)
            {
              total = AUTHENICATION_DELAY;
              if (GetDay((int)total, lastAccount.StatusChangeDate) >= 0)
                policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Fraud_Alert_Declined_Previous_Account.ToStringEnum() });
            }
            break;
          case Atlas.Enumerators.Account.AccountStatus.Review:
            policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Account_Status_Review.ToStringEnum() });
            break;
          case Atlas.Enumerators.Account.AccountStatus.PreApproved:
            policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Account_Status_PreApproved.ToStringEnum() });
            break;
          case Atlas.Enumerators.Account.AccountStatus.Approved:
            policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Account_Status_Approved.ToStringEnum() });
            break;
          case Atlas.Enumerators.Account.AccountStatus.Open:
            policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Account_Status_Current.ToStringEnum() });
            break;
          case Atlas.Enumerators.Account.AccountStatus.Legal:
            policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Handed_Over.ToStringEnum() });
            break;
          case Atlas.Enumerators.Account.AccountStatus.WrittenOff:
            policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Bad_Debt.ToStringEnum() });
            break;
          default:
            return null;

        }
      }
      return policyCollection;
    }

    public int GetReApplyDelay(long personId, UnitOfWork uow)
    {
      List<ACC_PolicyDTO> policyCollection = new List<ACC_PolicyDTO>();
      // Check accounts        
      _log.Info(string.Format("Checking policies for person [{0}]..", personId));

      var accountCollection = new XPQuery<ACC_Account>(uow).Where(a => a.Person.PersonId == personId && a.Status.Type != Enumerators.Account.AccountStatus.Technical_Error).ToList();

      _log.Info(string.Format("Found [{0}] account(s) for person [{1}]", accountCollection.Count, personId));

      if (accountCollection.Count >= 1)
      {
        var lastAccount = accountCollection.OrderBy(o => o.CreateDate).FirstOrDefault();

        _log.Info(string.Format("Last account for person [{0}] has a status of [{1}]", personId, lastAccount.Status.Type.ToStringEnum()));

        switch (lastAccount.Status.Type)
        {
          case Atlas.Enumerators.Account.AccountStatus.Inactive:
            return 1;
          case Atlas.Enumerators.Account.AccountStatus.Pending:
            return 1;
          case Atlas.Enumerators.Account.AccountStatus.Cancelled:
            return 0;
          case Atlas.Enumerators.Account.AccountStatus.Declined:
            // Determine decline reason
            int? total = null;

            if (lastAccount.StatusReason.Type == Enumerators.Account.AccountStatusReason.Fraud)
            {
              total = FRAUD_DELAY; // Hard coded for now.
              if (lastAccount.StatusSubReason.Type == Enumerators.Account.AccountStatusSubReason.PersonalSuspect ||
                 lastAccount.StatusSubReason.Type == Enumerators.Account.AccountStatusSubReason.PersonalConfirmed)
              {
                return GetDay((int)total, lastAccount.StatusChangeDate);
              }
            }
            else if (lastAccount.StatusReason.Type == Enumerators.Account.AccountStatusReason.CreditRisk)
            {
              total = CREDIT_RISK_DELAY;
              return GetDay((int)total, lastAccount.StatusChangeDate);
            }
            else if (lastAccount.StatusReason.Type == Enumerators.Account.AccountStatusReason.CompanyPolicy)
            {
              total = COMPANY_POLICY_DELAY;
              return GetDay((int)total, lastAccount.StatusChangeDate);
            }
            else if (lastAccount.StatusReason.Type == Enumerators.Account.AccountStatusReason.Affordability)
            {
              total = AFFORDABILITY_DELAY;
              return GetDay((int)total, lastAccount.StatusChangeDate);
            }
            else if (lastAccount.StatusReason.Type == Enumerators.Account.AccountStatusReason.Authentication)
            {
              total = AUTHENICATION_DELAY;
              if (GetDay((int)total, lastAccount.StatusChangeDate) >= 0)
                policyCollection.Add(new ACC_PolicyDTO() { Description = Enumerators.Account.Policy.Fraud_Alert_Declined_Previous_Account.ToStringEnum() });
            }
            break;
          case Atlas.Enumerators.Account.AccountStatus.Review:
            return 1;
          case Atlas.Enumerators.Account.AccountStatus.PreApproved:
            return 1;
          case Atlas.Enumerators.Account.AccountStatus.Approved:
            return 1;
          case Atlas.Enumerators.Account.AccountStatus.Open:
            return 1;
          case Atlas.Enumerators.Account.AccountStatus.Legal:
            return 1;
          case Atlas.Enumerators.Account.AccountStatus.WrittenOff:
            return 1;
          default:
            return 0;

        }
      }
      if (policyCollection.Count > 0)
        return 1;
      else
        return 0;
    }

    private int GetDay(int total, DateTime statusChangeDate)
    {
      TimeSpan ts = (DateTime.Now - statusChangeDate);
      return (total - Convert.ToInt32(Math.Floor(ts.TotalDays))) < 0 ? 0 : (total - Convert.ToInt32(Math.Floor(ts.TotalDays)));
    }

    private int GetPersonAge(DateTime birthDate)
    {
      DateTime today = DateTime.Now;
      int age = today.Year - birthDate.Year;
      if (birthDate > today.AddYears(-age)) age--;

      return age;
    }
  }
}
