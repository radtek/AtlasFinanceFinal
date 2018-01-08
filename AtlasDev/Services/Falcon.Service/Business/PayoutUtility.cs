using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using DevExpress.Xpo;
using Falcon.Service.Core;
using Falcon.Common.Structures;
using Newtonsoft.Json;

namespace Falcon.Service.Business
{
  public static class PayoutUtility
  {
    public static Tuple<PayoutStatistics, List<PayoutTransaction>> GetPayoutTransaction(long? branchId, DateTime? startRangeActionDate, DateTime? endRangeActionDate, long? payoutId, string idNumber, int? bankId)
    {
      var payoutTransactions = new List<PayoutTransaction>();

      using (var uow = new UnitOfWork())
      {
        var services = new XPQuery<PYT_HostService>(uow).Where(p => p.Enabled).Select(p => p.Service).ToList();
        var payoutQuery = new XPQuery<PYT_Payout>(uow).Where(p => services.Contains(p.Service));
        if (branchId > 0)
          payoutQuery = payoutQuery.Where(p => p.Company.CompanyId == branchId);

        if (startRangeActionDate.HasValue && endRangeActionDate.HasValue)
          payoutQuery = payoutQuery.Where(t => t.ActionDate.Date >= startRangeActionDate.Value.Date && t.ActionDate.Date <= endRangeActionDate.Value.Date);
        else if (startRangeActionDate.HasValue)
          payoutQuery = payoutQuery.Where(p => p.ActionDate.Date >= startRangeActionDate.Value.Date);
        else // Default to todays date.
          payoutQuery = payoutQuery.Where(p => p.ActionDate.Date >= DateTime.Today.Date);

        if (payoutId.HasValue && payoutId > 0)
          payoutQuery = payoutQuery.Where(p => p.PayoutId == payoutId);

        if (!string.IsNullOrEmpty(idNumber))
          payoutQuery = payoutQuery.Where(p => p.Person.IdNum == idNumber);

        if (bankId.HasValue && bankId > 0)
          payoutQuery = payoutQuery.Where(p => p.BankDetail.Bank.BankId == bankId);

        var payouts = payoutQuery.ToList();

        foreach (var payout in payouts)
        {
          var payoutTransaction = new PayoutTransaction()
          {
            ActionDate = payout.ActionDate,
            Amount = payout.Amount,
            Bank = payout.BankDetail.Bank.Description,
            BankAccountName = payout.BankDetail.AccountName,
            BankAccountNo = payout.BankDetail.AccountNum,
            BankAccountType = payout.BankDetail.AccountType.Description,
            BankAccountTypeId = payout.BankDetail.AccountType.AccountTypeId,
            BankBranchCode = payout.BankDetail.Code,
            BankId = payout.BankDetail.Bank.BankId,
            CreateDate = payout.CreateDate,
            IsValid = payout.IsValid,
            PayoutId = payout.PayoutId,
            PayoutStatus = string.Format("{0}{1}", payout.PayoutStatus.Description, payout.IsValid ? string.Empty : string.Format(" ({0})", "Invalid")),
            PayoutStatusId = payout.PayoutStatus.PayoutStatusId,
            ServiceType = payout.Service.ServiceType.Description,
            BatchStatus = string.Empty,
            Result = string.Empty,
            BranchName = string.Empty,
            FirstName = string.Empty,
            IdNumber = string.Empty,
            Surname = string.Empty
          };

          if (payout.Batch != null)
          {
            payoutTransaction.BatchId = payout.Batch.BatchId;
            payoutTransaction.BatchStatus = payout.Batch.BatchStatus.Description;
            payoutTransaction.BatchStatusId = payout.Batch.BatchStatus.BatchStatusId;
          }
          if (payout.ResultCode != null)
            payoutTransaction.Result = payout.ResultCode.Description;
          if (payout.Company != null)
          {
            payoutTransaction.BranchId = payout.Company.CompanyId;
            payoutTransaction.BranchName = payout.Company.Name;
          }

          if (payout.Person != null)
          {
            payoutTransaction.FirstName = payout.Person.Firstname;
            payoutTransaction.IdNumber = payout.Person.IdNum;
            payoutTransaction.Surname = payout.Person.Lastname;
          }

          switch (payout.PayoutStatus.Status)
          {
            case Payout.PayoutStatus.New:
            case Payout.PayoutStatus.OnHold:
              payoutTransaction.PayoutStatusColor = "label label-info";
              break;
            case Payout.PayoutStatus.Cancelled:
            case Payout.PayoutStatus.Removed:
              payoutTransaction.PayoutStatusColor = "label label-warning";
              break;
            case Payout.PayoutStatus.Failed:
              payoutTransaction.PayoutStatusColor = "label label-danger";
              break;
            case Payout.PayoutStatus.Successful:
              payoutTransaction.PayoutStatusColor = "label label-success";
              break;
            case Payout.PayoutStatus.Batched:
            case Payout.PayoutStatus.Submitted:
              payoutTransaction.PayoutStatusColor = "label label-default";
              break;
            default:
              break;
          }

          payoutTransactions.Add(payoutTransaction);
        }
      }

      return GetStatistics(payoutTransactions);
    }

    public static Tuple<PayoutStatistics, List<PayoutTransaction>> GetStatistics(List<PayoutTransaction> transactions)
    {
      var avsStat = new PayoutStatistics();

      avsStat.TotalTransactions = transactions.Count;
      avsStat.NewOnHold = transactions.Where(t => t.PayoutStatusId == (int)Payout.PayoutStatus.New || t.PayoutStatusId == (int)Payout.PayoutStatus.OnHold).Count();
      avsStat.Failed = transactions.Where(t => t.PayoutStatusId == (int)Payout.PayoutStatus.Failed).Count();
      avsStat.Invalid = transactions.Where(t => !t.IsValid).Count();
      avsStat.CancelledRemoved = transactions.Where(t => t.PayoutStatusId == (int)Payout.PayoutStatus.Cancelled || t.PayoutStatusId == (int)Payout.PayoutStatus.Removed).Count();
      avsStat.Submitted = transactions.Where(t => t.PayoutStatusId == (int)Payout.PayoutStatus.Submitted).Count();
      avsStat.Successful = transactions.Where(t => t.PayoutStatusId == (int)Payout.PayoutStatus.Successful).Count();

      return new Tuple<PayoutStatistics,List<PayoutTransaction>>(avsStat, transactions);
    }

    public static List<DashboardAlert> GetAlerts()
    {
      var alerts = new List<DashboardAlert>();

      using (var uow = new UnitOfWork())
      {
        var serviceScheduleBanks = new XPQuery<PYT_ServiceScheduleBank>(uow).Where(s => s.ServiceSchedule.Service.Enabled).ToList();

        var services = serviceScheduleBanks.Select(s => s.ServiceSchedule.Service).Distinct().ToList();
        foreach (var service in services)
        {
          var serviceOpenBanks = serviceScheduleBanks.Where(s =>
            s.ServiceSchedule.OpenTime.HasValue && s.ServiceSchedule.CloseTime.HasValue
            && s.ServiceSchedule.OpenTime.Value.TimeOfDay <= DateTime.Now.TimeOfDay
            && DateTime.Now.TimeOfDay <= s.ServiceSchedule.CloseTime.Value.TimeOfDay
            && s.ServiceSchedule.Service.ServiceId == service.ServiceId).ToList();

          var serviceClosedBanks = serviceScheduleBanks.Where(s => !serviceOpenBanks.Contains(s)).ToList();

          if (serviceClosedBanks.Count > 0)
          {
            var alert = new DashboardAlert();
            alert.Priority = DashboardAlert.Class.Warning;
            alert.PriorityString = DashboardAlert.Class.Warning.ToStringEnum();

            if (serviceOpenBanks.Count > 0)
              alert.Message = string.Format("{0} banks for Service {1} are closed - Bank(s) {2} are still open",
                (serviceClosedBanks.Count > serviceOpenBanks.Count) ? "Some" : "Most",
                service.Description,
                string.Join(", ", serviceOpenBanks.Select(s => s.Bank.Description).ToArray()));
            else
              alert.Message = string.Format("All banks for Service {0} is closed", service.Description);

            alerts.Add(alert);
          }
        }
      }

      return alerts;
    }

    public static List<Bank> GetBanks(bool rtcBankOnly = true)
    {
      List<Bank> banks = new List<Bank>();

      banks.Add(new Bank(General.BankName.ABS));
      banks.Add(new Bank(General.BankName.STD));
      banks.Add(new Bank(General.BankName.CAP));
      banks.Add(new Bank(General.BankName.FNB));
      banks.Add(new Bank(General.BankName.NED));

      if (!rtcBankOnly)
      {
        banks.Add(new Bank(General.BankName.AFR));
        banks.Add(new Bank(General.BankName.BID));
        banks.Add(new Bank(General.BankName.BOA));
        banks.Add(new Bank(General.BankName.BOL));
        banks.Add(new Bank(General.BankName.INV));
        banks.Add(new Bank(General.BankName.LIS));
        banks.Add(new Bank(General.BankName.MER));
        banks.Add(new Bank(General.BankName.NBS));
        banks.Add(new Bank(General.BankName.P_N_P));
        banks.Add(new Bank(General.BankName.PEP));
        banks.Add(new Bank(General.BankName.PER));
        banks.Add(new Bank(General.BankName.POS));
        banks.Add(new Bank(General.BankName.SAM));
        banks.Add(new Bank(General.BankName.TEL));
        banks.Add(new Bank(General.BankName.UBA));
        banks.Add(new Bank(General.BankName.UNB));
      }

      return banks.OrderBy(b => b.BankName).ToList();
    }

    public static void PlaceOnHold(long payoutId)
    {
      using (var uow = new UnitOfWork())
      {
        var payout = new XPQuery<PYT_Payout>(uow).FirstOrDefault(p => p.PayoutId == payoutId);
        if (payout == null)
          throw new KeyNotFoundException(string.Format("Cannot find payout record {0} in DB", payoutId));

        if (payout.PayoutStatus.Status == Payout.PayoutStatus.New)
        {
          payout.PayoutStatus = new XPQuery<PYT_PayoutStatus>(uow).FirstOrDefault(t => t.Status == Payout.PayoutStatus.OnHold);
          payout.LastStatusDate = DateTime.Now;

          uow.CommitChanges();
        }
        else
        {
          throw new Exception("Cannot hold payout");
        }
      }
    }

    public static void RemoveFromHold(long payoutId)
    {
      using (var uow = new UnitOfWork())
      {
        var payout = new XPQuery<PYT_Payout>(uow).FirstOrDefault(p => p.PayoutId == payoutId);
        if (payout == null)
          throw new KeyNotFoundException(string.Format("Cannot find payout record {0} in DB", payoutId));

        if (payout.PayoutStatus.Status == Payout.PayoutStatus.OnHold)
        {
          payout.PayoutStatus = new XPQuery<PYT_PayoutStatus>(uow).FirstOrDefault(t => t.Status == Payout.PayoutStatus.New);
          payout.LastStatusDate = DateTime.Now;

          uow.CommitChanges();
        }
        else
        {
          throw new Exception("Cannot remove hold from payout");
        }
      }
    }

    private static T GetObjectFromString<T>(string key)
    {
      var conn = RedisConnection.Current.GetDatabase();
      var bankString = conn.StringGet(key);
      if (string.IsNullOrEmpty(bankString))
        return default(T);
      else
        return JsonConvert.DeserializeObject<T>(bankString);
    }
  }
}