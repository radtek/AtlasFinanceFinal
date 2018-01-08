using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using DevExpress.Xpo;
using Newtonsoft.Json;
using Serilog;
using StackExchange.Redis;

using Atlas.Common.Extensions;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using Falcon.Common.Structures;
using Falcon.Common.Structures.Avs;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Structures;
using Falcon.Common.Interfaces.Structures.AVS;


namespace Falcon.Common.Repository
{
  public class AvsRepository : IAvsRepository
  {
    private readonly Lazy<IDatabase> _redis;
    private readonly Lazy<ILogger> _logger;
    private readonly Lazy<IMappingEngine> _mappingEngine;


    public AvsRepository(Lazy<ILogger> logger, Lazy<IDatabase> redis, Lazy<IConfiguration> mapperConfiguration, Lazy<IMappingEngine> mappingEngine)
    {
      _logger = logger;
      _redis = redis;
      _mappingEngine = mappingEngine;

      mapperConfiguration.Value.CreateMap<AvsStatistics, IAvsStatistics>();
      mapperConfiguration.Value.CreateMap<IAvsStatistics, AvsStatistics>();
      mapperConfiguration.Value.CreateMap<AvsTransactions, IAvsTransactions>();
      mapperConfiguration.Value.CreateMap<IAvsTransactions, AvsTransactions>();
      mapperConfiguration.Value.CreateMap<AVS_Service, AvsService>();
      mapperConfiguration.Value.CreateMap<AvsService, IAvsService>();
      mapperConfiguration.Value.CreateMap<IAvsTransactions, AvsTransactions>();
      mapperConfiguration.Value.CreateMap<Bank, IBank>();
    }


    public List<IAvsTransactions> GetTransactions(long? branchId, DateTime? startDate, DateTime? endDate, long? transactionId, string idNumber, long? bankId)
    {
      return GetAvsTransactions(branchId, startDate, endDate, transactionId, idNumber, bankId);
    }


    public List<IBank> GetSupportedBanks()
    {
      var banks = new List<Bank>
      {
        new Bank(General.BankName.ABS),
        new Bank(General.BankName.STD),
        new Bank(General.BankName.CAP),
        new Bank(General.BankName.FNB),
        new Bank(General.BankName.NED)
      };


      return _mappingEngine.Value.Map<List<Bank>, List<IBank>>(banks.OrderBy(b => b.BankName).ToList());
    }


    public List<IAvsService> GetActiveServices()
    {
      using (var uow = new UnitOfWork())
      {
        var services = new XPQuery<AVS_Service>(uow).Where(s => s.Enabled).ToList();
        return _mappingEngine.Value.Map<List<AvsService>, List<IAvsService>>(_mappingEngine.Value.Map<List<AVS_Service>, List<AvsService>>(services));
      }
    }


    public List<IAvsStatistics> GetStats(IList<IAvsTransactions> transactions)
    {
      var stats = new List<IAvsStatistics>();
      double totalTimeTaken = 0;

      var banks = transactions.Select(t => t.Bank).Distinct().OrderByDescending(b => b);
      foreach (var bank in banks)
      {
        var currentTransactions = transactions.Where(t => t.Bank == bank).ToList();
        var avsStat = new AvsStatistics()
          {
            Bank = bank,
            TotalTransactions = currentTransactions.Count,
            TotalSent = currentTransactions.Count(t => t.BatchId != null),
            TotalQueued = currentTransactions.Count(t => t.StatusId == (int)AVS.Status.Queued),
            TotalPending = currentTransactions.Count(t => t.StatusId == (int)AVS.Status.Pending),
            TotalComplete = currentTransactions.Count(t => !string.IsNullOrEmpty(t.Result))
          };

        double aveTimeTaken = 0;
        var timeTaken = currentTransactions.Where(t => t.ResponseDate.HasValue && t.ResponseDate.Value.Date == t.CreateDate.Date).Sum(t => (t.ResponseDate.Value - t.CreateDate).TotalSeconds);
        if (timeTaken > 0)
        {
          totalTimeTaken += timeTaken;
          aveTimeTaken = timeTaken / Convert.ToDouble(currentTransactions.Count(t => t.ResponseDate.HasValue));
        }
        avsStat.ResponseTime = aveTimeTaken.ConvertSecondsToMinutesString();

        stats.Add(avsStat);
      }

      stats.Add(new AvsStatistics()
      {
        Bank = "All",
        ResponseTime = ((totalTimeTaken > 0) ? totalTimeTaken / transactions.Count(t => t.ResponseDate != null) : 0).ConvertSecondsToMinutesString(),
        TotalComplete = stats.Sum(s => s.TotalComplete),
        TotalPending = stats.Sum(s => s.TotalPending),
        TotalQueued = stats.Sum(s => s.TotalQueued),
        TotalSent = stats.Sum(s => s.TotalSent),
        TotalTransactions = stats.Sum(s => s.TotalTransactions)
      });

      return stats;
    }


    public Task<Tuple<List<IAvsStatistics>, List<IAvsTransactions>>> GetCachedTransactions(long? branchId, DateTime? startDate, DateTime? endDate, long? transactionId, string idNumber, long? bankId)
    {
      return GetAvsTransactionsCache(branchId, startDate, endDate, transactionId, idNumber, bankId);
    }


    public void UpdateServiceSchedule(Dictionary<IAvsService, List<IAvsServiceBank>> newServiceSchedules)
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
            if (service != null)
            {
              service.NextGenerationNo = newServiceSchedule.Key.NextGenerationNo;
              service.NextTransmissionNo = newServiceSchedule.Key.NextTransmissionNo;
            }
          }

          var linkedBanks = newServiceSchedule.Value.Where(t => t.IsLinked).ToList();
          var unlinkedBankIds = newServiceSchedule.Value.Where(t => !t.IsLinked).Select(t => t.BankId).ToList();

          var serviceScheduleBanksToUnlink = scheduleBanks.Where(s => s.ServiceSchedule.Service.ServiceId == newServiceSchedule.Key.ServiceId && unlinkedBankIds.Contains(s.Bank.BankId)).ToList();
          if (serviceScheduleBanksToUnlink.Count > 0)
          {
            uow.Delete(serviceScheduleBanksToUnlink);
            uow.PurgeDeletedObjects();
          }

          foreach (var linkedBank in linkedBanks)
          {
            var scheduleBank = scheduleBanks.Where(s => s.ServiceSchedule.Service.ServiceId == linkedBank.ServiceId && s.Bank.BankId == linkedBank.BankId).ToList();
            if (scheduleBank.Count == 0)
            {
              var bank = linkedBank;
              foreach (var serviceSchedule in serviceSchedules.Where(s => s.Service.ServiceId == bank.ServiceId))
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
                }
                new AVS_ServiceScheduleBank(uow)
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


    public bool Resend(long transactionId, int serviceId)
    {
      using (var uow = new UnitOfWork())
      {
        var avs = new XPQuery<AVS_Transaction>(uow).FirstOrDefault(p => p.TransactionId == transactionId);

        if (avs != null)
        {
          if (serviceId != 0)
            avs.Service = new XPQuery<AVS_Service>(uow).FirstOrDefault(s => s.ServiceId == serviceId);
          avs.Status = new XPQuery<AVS_Status>(uow).FirstOrDefault(p => p.Type == AVS.Status.Queued);
          avs.Batch = null;
          avs.ThirdPartyRef = null;
          avs.Result = null;
          avs.ResponseAccountNumber = null;
          avs.ResponseIdNumber = null;
          avs.ResponseInitials = null;
          avs.ResponseLastName = null;
          avs.ResponseAccountOpen = null;
          avs.ResponseAcceptsCredit = null;
          avs.ResponseAcceptsDebit = null;
          avs.ResponseOpenThreeMonths = null;
          avs.ResponseDate = null;
          avs.LastStatusDate = DateTime.Now;

          avs.Save();
        }

        uow.CommitChanges();

        return true;
      }
    }


    #region Internal

    internal async Task<Tuple<List<IAvsStatistics>, List<IAvsTransactions>>> GetAvsTransactionsCache(long? branchId, DateTime? startDate, DateTime? endDate, long? transactionId, string idNumber, long? bankId, long? accountId = 0)
    {
      if (startDate.HasValue && endDate.HasValue)
      {
        var redisAvsKey = string.Format("falcon.avs.query.{0}.{1}", startDate.Value.ToString("ddMMyyyy"), endDate.Value.ToString("ddMMyyyy"));

        _logger.Value.Information("GetAvsTransactionsCache - Key {key}", redisAvsKey);
        var time = new System.Diagnostics.Stopwatch();
        time.Start();

        // get transactions from Redis
        var result = _redis.Value.StringGet(redisAvsKey);

        if (!result.IsNull)
        {
          var cachedTransactions = await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Tuple<List<AvsStatistics>, List<AvsTransactions>>>(result));
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

              time.Stop();

              _logger.Value.Information("GetAvsTransactionsCache - Key {key} took {time}ms to execute", redisAvsKey, time.ElapsedMilliseconds);

              return new Tuple<List<IAvsStatistics>, List<IAvsTransactions>>(GetStats(_mappingEngine.Value.Map<List<AvsTransactions>, List<IAvsTransactions>>(filterTransactions)),
                _mappingEngine.Value.Map<List<AvsTransactions>, List<IAvsTransactions>>(filterTransactions));
            }
            else
            {
              _logger.Value.Information("GetAvsTransactionsCache - Key {key} took {time}ms to execute", redisAvsKey, time.ElapsedMilliseconds);

              return new Tuple<List<IAvsStatistics>, List<IAvsTransactions>>(_mappingEngine.Value.Map<List<AvsStatistics>, List<IAvsStatistics>>(cachedTransactions.Item1),
                _mappingEngine.Value.Map<List<AvsTransactions>, List<IAvsTransactions>>(cachedTransactions.Item2));
            }
          }
        }
      }
      // get new transactions from DB
      var transactions = GetAvsTransactions(branchId, startDate, endDate, transactionId, idNumber, bankId, accountId);
      return new Tuple<List<IAvsStatistics>, List<IAvsTransactions>>(GetStats(transactions), transactions);
    }


    internal List<IAvsTransactions> GetAvsTransactions(long? branchId, DateTime? startDate, DateTime? endDate, long? transactionId, string idNumber, long? bankId, long? accountId = 0)
    {
      var transactions = new List<IAvsTransactions>();

      using (var uow = new UnitOfWork())
      {
        IQueryable<AVS_Transaction> avsQuery = uow.Query<AVS_Transaction>();

        if (startDate.HasValue && endDate.HasValue)
          avsQuery = avsQuery.Where(t => t.LastStatusDate >= startDate.Value && t.LastStatusDate <= endDate.Value.AddDays(1));
        else if (startDate.HasValue)
          avsQuery = avsQuery.Where(p => p.LastStatusDate >= startDate.Value);
        else if (!accountId.HasValue || accountId <= 0)
          avsQuery = avsQuery.Where(p => p.LastStatusDate >= DateTime.Today);

        if (branchId.HasValue && branchId.Value > 0)
          avsQuery = avsQuery.Where(p => p.Company.CompanyId == branchId); // BranchId = CompanyId in terms of atlas structure

        if (transactionId.HasValue && transactionId.Value > 0)
          avsQuery = avsQuery.Where(t => t.TransactionId == transactionId);

        if (!string.IsNullOrEmpty(idNumber))
          avsQuery = avsQuery.Where(t => t.IdNumber == idNumber);

        if (bankId.HasValue && bankId.Value > 0)
          avsQuery = avsQuery.Where(t => t.Bank.BankId == bankId);

        if (accountId.HasValue && accountId.Value > 0)
          avsQuery = avsQuery.Where(t => t.Account.AccountId == accountId);

        var responseCodes = uow.Query<AVS_ResponseCode>().ToList();

        var avsResults = avsQuery.ToList();

        foreach (var item in avsResults)
        {
          var statusIcon = string.Empty;

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
          }

          var resultColor = string.Empty;
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


    internal string GetResponseColor(AVS_ResponseCode responseCode)
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

    #endregion

  }
}