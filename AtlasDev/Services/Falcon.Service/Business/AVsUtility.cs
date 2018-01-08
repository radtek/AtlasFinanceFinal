using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Business.AVS;
using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using DevExpress.Xpo;
using Falcon.Service.Core;
using Falcon.Common.Structures;
using Falcon.Common.Structures.Avs;

namespace Falcon.Service.Business
{
  public class AvsUtility
  {
    public List<Bank> GetAVSBanks()
    {
      List<Bank> banks = new List<Bank>();

      banks.Add(new Bank(General.BankName.ABS));
      banks.Add(new Bank(General.BankName.STD));
      banks.Add(new Bank(General.BankName.CAP));
      banks.Add(new Bank(General.BankName.FNB));
      banks.Add(new Bank(General.BankName.NED));

      return banks.OrderBy(b => b.BankName).ToList();
    }

    public Tuple<List<AvsStatistics>, List<AvsTransactions>> GetTransactions(long? branchId, DateTime? startDate, DateTime? endDate, long? transactionId, string idNumber, long? bankId)
    {
      return GetAvsTransactionsCache(branchId, startDate, endDate, transactionId, idNumber, bankId);
    }

    public List<AvsTransactions> GetAvsTransactions(long? branchId, DateTime? startDate, DateTime? endDate, long? transactionId, string idNumber, long? bankId, long? accountId = 0)
    {
      List<AvsTransactions> transactions = new List<AvsTransactions>();

      using (var uow = new UnitOfWork())
      {
        IQueryable<AVS_Transaction> avsQuery = from t in uow.Query<AVS_Transaction>()
                                               select t;

        if (startDate.HasValue && endDate.HasValue)
          avsQuery = avsQuery.Where(t => t.LastStatusDate >= startDate.Value && t.LastStatusDate <= endDate.Value.AddDays(1));
        else if (startDate.HasValue)
          avsQuery = avsQuery.Where(p => p.LastStatusDate >= startDate.Value);
        else if (!accountId.HasValue || accountId <= 0)
          avsQuery = avsQuery.Where(p => p.LastStatusDate >= DateTime.Today);

        if (branchId.HasValue && branchId.Value > 0)
          avsQuery = avsQuery.Where(p => p.Company.CompanyId == branchId); // BranchId = CompanyId in terms of atlas structure
        // ---------------------------------------------------------------------------------------------------
        // BranchId = CompanyId  -- this assumption is not true!!?!
        // 
        // TODO: (KB-2015-06-11) Isn't this the correct way?
        // avsQuery = avsQuery.Where(p => p.Company.CompanyId == uow.Query<BRN_Branch>().Where(s => s.BranchId == branchId).Select(s => s.Company.CompanyId).First());

        if (transactionId.HasValue && transactionId.Value > 0)
          avsQuery = avsQuery.Where(t => t.TransactionId == transactionId);

        if (!string.IsNullOrEmpty(idNumber))
          avsQuery = avsQuery.Where(t => t.IdNumber == idNumber);

        if (bankId.HasValue && bankId.Value > 0)
          avsQuery = avsQuery.Where(t => t.Bank.BankId == bankId);

        if (accountId.HasValue && accountId.Value > 0)
          avsQuery = avsQuery.Where(t => t.Account.AccountId == accountId);

        var responseCodes = (from r in uow.Query<AVS_ResponseCode>()
                             select r).ToList();

        var avsResults = avsQuery.ToList();

        foreach (var item in avsResults)
        {
          string statusIcon = string.Empty;

          switch (item.Status.Type)
          {
            case AVS.Status.Queued:
              statusIcon = "icon-truck";
              break;
            case AVS.Status.Pending:
              statusIcon = "icon-time";
              break;
            case AVS.Status.Complete:
              statusIcon = "icon-thumbs-up";
              break;
            case AVS.Status.Cancelled:
              statusIcon = "icon-trash";
              break;
            default:
              break;
          }

          string resultColor = string.Empty;
          if (item.Result != null)
          {
            switch (item.Result.Type)
            {
              case AVS.Result.Passed:
                resultColor = "label label-success";
                break;
              case AVS.Result.PassedWithWarnings:
                resultColor = "label label-warning";
                break;
              case AVS.Result.Failed:
                resultColor = "label label-danger";
                break;
              case AVS.Result.NoResult:
                resultColor = "label label-info";
                break;
              case AVS.Result.Locked:
                resultColor = "label label-default";
                break;
              default:
                break;
            }
          }

          transactions.Add(new AvsTransactions
          {
            TransactionId = item.TransactionId,
            BatchId = item.Batch == null ? (long?) null : item.Batch.BatchId,
            AccountId = item.Account != null ? item.Account.AccountId : 0,
            AccountNo = item.AccountNo,
            BankId = item.Bank.BankId,
            Bank = item.Bank.Type.ToStringEnum(),
            BranchCode = item.BranchCode,
            IdNumber = item.IdNumber,
            Initials = item.Initials,
            LastName = item.LastName,
            ResponseDate = item.ResponseDate,
            ResponseAcceptsCredit =
              GetResponseColor(responseCodes.FirstOrDefault(r => r.ResponseCode == item.ResponseAcceptsCredit
                                                                 &&
                                                                 r.ResponseGroup.Type == AVS.ResponseGroup.AcceptsCredit)),
            ResponseAcceptsDebit =
              GetResponseColor(responseCodes.FirstOrDefault(r => r.ResponseCode == item.ResponseAcceptsDebit
                                                                 &&
                                                                 r.ResponseGroup.Type == AVS.ResponseGroup.AcceptsDebit)),
            ResponseAccountNumber =
              GetResponseColor(responseCodes.FirstOrDefault(r => r.ResponseCode == item.ResponseAccountNumber
                                                                 &&
                                                                 r.ResponseGroup.Type == AVS.ResponseGroup.AccountNumber)),
            ResponseAccountOpen =
              GetResponseColor(responseCodes.FirstOrDefault(r => r.ResponseCode == item.ResponseAccountOpen
                                                                 &&
                                                                 r.ResponseGroup.Type == AVS.ResponseGroup.AccountOpen)),
            ResponseIdNumber =
              GetResponseColor(responseCodes.FirstOrDefault(r => r.ResponseCode == item.ResponseIdNumber
                                                                 && r.ResponseGroup.Type == AVS.ResponseGroup.IdNumber)),
            ResponseInitials =
              GetResponseColor(responseCodes.FirstOrDefault(r => r.ResponseCode == item.ResponseInitials
                                                                 && r.ResponseGroup.Type == AVS.ResponseGroup.Initials)),
            ResponseLastName =
              GetResponseColor(responseCodes.FirstOrDefault(r => r.ResponseCode == item.ResponseLastName
                                                                 && r.ResponseGroup.Type == AVS.ResponseGroup.Surname)),
            ResponseOpenThreeMonths =
              GetResponseColor(responseCodes.FirstOrDefault(r => r.ResponseCode == item.ResponseOpenThreeMonths
                                                                 &&
                                                                 r.ResponseGroup.Type ==
                                                                 AVS.ResponseGroup.OpenThreeMonths)),
            ThirdPartyRef = item.ThirdPartyRef,
            StatusColor = "lll",
            Result = item.Result != null ? item.Result.Type.ToStringEnum() : string.Empty,
            ResultColour = resultColor,
            StatusId = item.Status.StatusId,
            Status = item.Status.Type.ToStringEnum(),
            CreateDate = item.CreateDate,
            LastStatusDate = item.LastStatusDate,
            CompanyId = item.Company != null ? item.Company.CompanyId : 0,
            Company = item.Company != null ? item.Company.Name : string.Empty,
            StatusIcon = statusIcon,
            Service = item.Service != null ? item.Service.Description : string.Empty
          });
        }
      }

      return transactions;
    }

    public Tuple<List<AvsStatistics>, List<AvsTransactions>> GetAvsTransactionsCache(long? branchId, DateTime? startDate, DateTime? endDate, long? transactionId, string idNumber, long? bankId, long? accountId = 0)
    {
      if (startDate.HasValue && endDate.HasValue)
      {
        var redisAvsKey = string.Format("falcon.avs.query.{0}.{1}", startDate.Value.ToString("ddMMyyyy"), endDate.Value.ToString("ddMMyyyy"));

        // get transactions from Redis
        var cachedTransactions = RedisConnection.GetObjectFromString<Tuple<List<AvsStatistics>, List<AvsTransactions>>>(redisAvsKey);
        if (cachedTransactions != null)
        {
          if ((branchId.HasValue && branchId.Value > 0)
            || (transactionId.HasValue && transactionId.Value > 0)
            || (!string.IsNullOrEmpty(idNumber))
            || (bankId.HasValue && bankId.Value > 0)
            || (accountId.HasValue && accountId.Value > 0))
          {
            // apply filter to transactions
            var filterTransactions = new List<AvsTransactions>();
            if (branchId.HasValue && branchId.Value > 0)
              filterTransactions = cachedTransactions.Item2.Where(t => t.CompanyId == branchId).ToList();

            if (transactionId.HasValue && transactionId.Value > 0)
              filterTransactions = cachedTransactions.Item2.Where(t => t.TransactionId == transactionId).ToList();

            if (!string.IsNullOrEmpty(idNumber))
              filterTransactions = cachedTransactions.Item2.Where(t => t.IdNumber == idNumber).ToList();

            if (bankId.HasValue && bankId.Value > 0)
              filterTransactions = cachedTransactions.Item2.Where(t => t.BankId == bankId).ToList();

            if (accountId.HasValue && accountId.Value > 0)
              filterTransactions = cachedTransactions.Item2.Where(t => t.AccountId == accountId).ToList();

            return new Tuple<List<AvsStatistics>, List<AvsTransactions>>(GetStats(filterTransactions), filterTransactions);
          }
          else
          {
            return cachedTransactions;
          }
        }
      }

      // get new transactions from DB
      var transactions = GetAvsTransactions(branchId, startDate, endDate, transactionId, idNumber, bankId, accountId);
      return new Tuple<List<AvsStatistics>, List<AvsTransactions>>(GetStats(transactions), transactions);
    }

    public void ResendAvsTransactions(List<long> transactionIds)
    {
      var avsUtil = new Default();
      var newTransactions = avsUtil.ResendTransaction(transactionIds);
      avsUtil = null;
    }

    public void CancelAvsTransactions(List<long> transactionIds)
    {
      var avsUtil = new Default();
      var newTransactions = avsUtil.CancelTransaction(transactionIds.ToArray());
      avsUtil = null;
    }

    public Dictionary<AvsService, List<AvsServiceBank>> GetServiceSchedules()
    {
      var avsServiceScheduleBanks = new Dictionary<AvsService, List<AvsServiceBank>>();

      using (var uow = new UnitOfWork())
      {
        var services = new XPQuery<AVS_Service>(uow).Where(s => s.Enabled).OrderBy(t => t.Description).ToList();
        var scheduleBanks = new XPQuery<AVS_ServiceScheduleBank>(uow).Where(s => services.Contains(s.ServiceSchedule.Service)).ToList();
        foreach (var service in services)
        {
          var avsService = new AvsService()
          {
            ServiceId = service.ServiceId,
            Description = service.Description,
            NextGenerationNo = service.NextGenerationNo,
            NextTransmissionNo = service.NextTransmissionNo
          };
          avsServiceScheduleBanks.Add(avsService, new List<AvsServiceBank>());

          var serviceBanks = scheduleBanks.Where(s => s.ServiceSchedule.Service.ServiceId == service.ServiceId).Select(b => b.Bank).Distinct().ToList();
          foreach (var serviceBank in serviceBanks)
          {
            avsServiceScheduleBanks[avsService].Add(new AvsServiceBank()
            {
              BankId = serviceBank.BankId,
              BankName = serviceBank.Description,
              ServiceId = service.ServiceId,
              IsLinked = true
            });
          }
          var bankIds = serviceBanks.Select(b => b.BankId).ToList();
          var unlinkedBanks = GetAVSBanks().Where(b => !bankIds.Contains(b.BankId));
          foreach (var bank in unlinkedBanks)
          {
            avsServiceScheduleBanks[avsService].Add(new AvsServiceBank()
            {
              BankId = bank.BankId,
              BankName = bank.BankName,
              ServiceId = service.ServiceId,
              IsLinked = false
            });
          }
          avsServiceScheduleBanks[avsService] = avsServiceScheduleBanks[avsService].OrderBy(b => b.BankName).ToList();
        }
      }

      return avsServiceScheduleBanks;
    }

    public void UpdateServiceSchedule(Dictionary<AvsService, List<AvsServiceBank>> newServiceSchedules)
    {
      using (var uow = new UnitOfWork())
      {
        var serviceIds = newServiceSchedules.Keys.Select(a => a.ServiceId).ToList();
        var serviceSchedules = new XPQuery<AVS_ServiceSchedule>(uow).Where(a => serviceIds.Contains(a.Service.ServiceId)).ToList();
        var scheduleBanks = new XPQuery<AVS_ServiceScheduleBank>(uow).Where(s => serviceSchedules.Contains(s.ServiceSchedule)).ToList();
        var banks = new XPQuery<BNK_Bank>(uow).ToList();

        foreach (var newServiceSchedule in newServiceSchedules)
        {
          if (newServiceSchedule.Key.SaveSequenceNo)
          {
            var service = serviceSchedules.Where(s => s.Service.ServiceId == newServiceSchedule.Key.ServiceId).Select(s => s.Service).FirstOrDefault();
            service.NextGenerationNo = newServiceSchedule.Key.NextGenerationNo;
            service.NextTransmissionNo = newServiceSchedule.Key.NextTransmissionNo;
          }

          var linkedBanks = newServiceSchedule.Value.Where(t => t.IsLinked).ToList();
          var unlinkedBankIds = newServiceSchedule.Value.Where(t => !t.IsLinked).Select(t => t.BankId).ToList();

          var serviceScheduleBanksToUnlink = scheduleBanks.Where(s => s.ServiceSchedule.Service.ServiceId == newServiceSchedule.Key.ServiceId && unlinkedBankIds.Contains(s.Bank.BankId)).ToList();
          uow.Delete(serviceScheduleBanksToUnlink);
          uow.PurgeDeletedObjects();

          foreach (var linkedBank in linkedBanks)
          {
            var scheduleBank = scheduleBanks.Where(s => s.ServiceSchedule.Service.ServiceId == linkedBank.ServiceId && s.Bank.BankId == linkedBank.BankId).ToList();
            if (scheduleBank.Count == 0)
            {
              foreach (var serviceSchedule in serviceSchedules.Where(s => s.Service.ServiceId == linkedBank.ServiceId))
              {
                switch (serviceSchedule.Service.Type)
                {
                  case AVS.Service.Hyphen: serviceSchedule.ServiceType = new XPQuery<AVS_ServiceType>(uow).FirstOrDefault(a => a.Type == AVS.ServiceType.Hyphen);
                    break;
                  case AVS.Service.ABSA: serviceSchedule.ServiceType = new XPQuery<AVS_ServiceType>(uow).FirstOrDefault(a => a.Type == AVS.ServiceType.ABSA2ABSA);
                    break;
                  case AVS.Service.ABSA_Capitec:
                  case AVS.Service.ABSA_Nedbank:
                  case AVS.Service.ABSA_FNB:
                  case AVS.Service.ABSA_STDBank:
                    serviceSchedule.ServiceType = new XPQuery<AVS_ServiceType>(uow).FirstOrDefault(a => a.Type == AVS.ServiceType.ABSA2Other);
                    break;
                  case AVS.Service.Compuscan:
                    serviceSchedule.ServiceType = new XPQuery<AVS_ServiceType>(uow).FirstOrDefault(a => a.Type == AVS.ServiceType.Compuscan);
                    break;
                  default: break;
                }
                var newLinkedBank = new AVS_ServiceScheduleBank(uow)
                {
                  Bank = banks.First(b => b.BankId == linkedBank.BankId),
                  ServiceSchedule = serviceSchedule
                };
              }
            }
          }
        }

        uow.CommitChanges();
      }
    }

    public string GetResponseColor(AVS_ResponseCode responseCode)
    {
      if (responseCode != null)
      {
        switch (responseCode.ResponseResult.Type)
        {
          case AVS.ResponseResult.Passed:
            return "label-success";
          case AVS.ResponseResult.NoResponse:
            return "label-warning";
          case AVS.ResponseResult.Failed:
            return "label-danger";
          default:
            return "";
        }
      }
      else
      {
        return string.Empty;
      }
    }

    public List<AvsStatistics> GetStats(List<AvsTransactions> transactions)
    {
      var stats = new List<AvsStatistics>();
      double totalTimeTaken = 0;

      var banks = transactions.Select(t => t.Bank).Distinct().OrderByDescending(b => b);
      foreach (var bank in banks)
      {
        var currentTransactions = transactions.Where(t => t.Bank == bank).ToList();
        var avsStat = new AvsStatistics();
        avsStat.Bank = bank;
        avsStat.TotalTransactions = currentTransactions.Count;

        avsStat.TotalSent = currentTransactions.Where(t => t.BatchId != null).Count();
        avsStat.TotalQueued = currentTransactions.Where(t => t.StatusId == (int)AVS.Status.Queued).Count();
        avsStat.TotalPending = currentTransactions.Where(t => t.StatusId == (int)AVS.Status.Pending).Count();
        avsStat.TotalComplete = currentTransactions.Where(t => !string.IsNullOrEmpty(t.Result)).Count();

        double aveTimeTaken = 0;
        var timeTaken = currentTransactions.Where(t => t.ResponseDate.HasValue && t.ResponseDate.Value.Date == t.CreateDate.Date).Sum(t => (t.ResponseDate.Value - t.CreateDate).TotalSeconds);
        if (timeTaken > 0)
        {
          totalTimeTaken += timeTaken;
          aveTimeTaken = timeTaken / Convert.ToDouble(currentTransactions.Count(t => t.ResponseDate.HasValue));
        }
        avsStat.ResponseTime = ConvertSecondsToMinutesString(aveTimeTaken);

        stats.Add(avsStat);
      }

      stats.Add(new AvsStatistics()
      {
        Bank = "All",
        ResponseTime = ConvertSecondsToMinutesString((totalTimeTaken > 0) ? totalTimeTaken / transactions.Count(t => t.ResponseDate != null) : 0),
        TotalComplete = stats.Sum(s => s.TotalComplete),
        TotalPending = stats.Sum(s => s.TotalPending),
        TotalQueued = stats.Sum(s => s.TotalQueued),
        TotalSent = stats.Sum(s => s.TotalSent),
        TotalTransactions = stats.Sum(s => s.TotalTransactions)
      });

      return stats;
    }

    private string ConvertSecondsToMinutesString(double seconds)
    {
      var minutes = (Math.Round(seconds / 60, 0)).ToString();
      return (string.Format("{0}:{1}", minutes.Length < 2 ? minutes.PadLeft(2, '0') : minutes, (Math.Round(seconds, 0) % 60).ToString().PadLeft(2, '0')));
    }
  }
}