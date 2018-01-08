using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Atlas.Ass.Framework.Repository;
using Atlas.Ass.Framework.Structures;
using Atlas.Ass.Repository.Properties;
using Atlas.Ass.Structures;
using Falcon.Common.Interfaces.Services;
using Serilog;

namespace Atlas.Ass.Repository
{
  public class AssCiRepository : IAssCiRepository
  {
    private readonly IConfigService _configService;
    private readonly ILogger _logger;

    public AssCiRepository(IConfigService configService, ILogger logger)
    {
      _configService = configService;
      _logger = logger;
    }

    #region Ci

    public ICollection<IBasicLoan> RunBasicInfoQuery(ICollection<string> branchNos, DateTime startRange,
      DateTime endRange,
      TimeSpan? expiryTime = null)
    {
      var basicLoans = new List<IBasicLoan>();
      Parallel.ForEach(branchNos,
        new ParallelOptions
        {
          MaxDegreeOfParallelism = 20
        },
        branchNo =>
        {
          try
          {
            var temp =
              new List<IBasicLoan>(GetData<BasicLoan>(Resources.QRY_BasicLoanInfo, startRange.ToString("yyyy-MM-dd"),
                endRange.ToString("yyyy-MM-dd"), string.Join("','", branchNo)));
            lock (basicLoans)
            {
              basicLoans.AddRange(temp);
            }
          }
          catch (Exception exception)
          {
            _logger.Error($"ERROR: {exception.Message} - {exception.StackTrace} - {branchNo}");
          }
        }
        );

      return basicLoans;
    }

    public ICollection<IClientLoanInfo> RunClientInfoQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate, TimeSpan? expiryTime = null)
    {
      var clientLoanInfo = new List<IClientLoanInfo>();

      var queriesPerRun = 5;
      for (var i = 0; i < branchNos.Count; i++)
      {
        var tempBranchNos = branchNos.Skip(i * queriesPerRun).Take(queriesPerRun).ToList();
        var clientLoanInfoQueries = new Task[tempBranchNos.Count];

        for (var j = 0; j < tempBranchNos.Count; j++)
        {
          var index = j;

          clientLoanInfoQueries[index] = Task.Run(() =>
          {
            lock (clientLoanInfo)
            {
              clientLoanInfo.AddRange(GetData<ClientLoanInfo>(Resources.QRY_ClientNewVsExistingInfo,
                startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), tempBranchNos[index]));
            }
          });
        }

        Task.WaitAll(clientLoanInfoQueries);
      }

      return clientLoanInfo;
    }

    public ICollection<ICollectionRefund> RunCollectionRefundQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate,
      TimeSpan? expiryTime = null)
    {
      var breaks = Convert.ToInt32(Math.Ceiling((decimal)(endDate - startDate).TotalDays / 10));
      breaks = breaks == 0 ? 1 : breaks;
      var tempCollectionRefund = new List<CollectionRefund>();
      var queryStartDate = startDate;
      var queryEndDate = endDate;

      var collectionRefundQueries = new Task[breaks];
      for (var i = 1; i <= breaks; i++)
      {
        if (i > 1)
          queryStartDate = queryEndDate.AddDays(1);
        if (i < breaks)
          queryEndDate = queryStartDate.AddDays(10);
        else if (i == breaks)
          queryEndDate = endDate;

        var index = i;
        var tempStart = queryStartDate;
        var tempEnd = queryEndDate;

        collectionRefundQueries[index - 1] = Task.Run(() =>
        {
          tempCollectionRefund.AddRange(GetData<CollectionRefund>(Resources.QRY_CollectionsVsRefunds,
            tempStart.ToString("yyyy-MM-dd"), tempEnd.ToString("yyyy-MM-dd"), string.Join("','", branchNos)));
        });
      }

      Task.WaitAll(collectionRefundQueries);

      return Summarise(tempCollectionRefund, branchNos);
    }

    private static List<ICollectionRefund> Summarise(List<CollectionRefund> tempCollectionRefund,
      ICollection<string> branches)
    {
      return new List<ICollectionRefund>((from branch in branches
                                          let paynos = tempCollectionRefund.Where(b => b.LegacyBranchNumber == branch).Select(b => b.PayNo).Distinct()
                                          from payno in paynos
                                          let collectionRefunds = tempCollectionRefund.Where(b => b.LegacyBranchNumber == branch && b.PayNo == payno)
                                          select new CollectionRefund()
                                          {
                                            LegacyBranchNumber = branch,
                                            Collections = collectionRefunds.Sum(b => b.Collections),
                                            PayNo = payno,
                                            Refunds = collectionRefunds.Sum(b => b.Refunds)
                                          }).ToList());
    }

    public ICollection<IVAP> RunVapQuery(ICollection<string> branchNos, DateTime startDate, DateTime endDate,
      TimeSpan? expiryTime = null)
    {
      return
        new List<IVAP>(GetData<VAP>(Resources.QRY_Vap, string.Join("','", branchNos), startDate.ToString("yyyy-MM-dd"),
          endDate.ToString("yyyy-MM-dd")));
    }

    public ICollection<IRolledAccounts> RunRolledAccountsQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate, TimeSpan? expiryTime = null)
    {
      return
        new List<IRolledAccounts>(GetData<RolledAccounts>(Resources.QRY_RolledAccounts, string.Join("','", branchNos),
          startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd")));
    }

    public ICollection<IReswipes> RunReswipesQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate)
    {
      return new List<IReswipes>(GetData<Reswipes>(Resources.QRY_Reswipes, startDate.ToString("yyyy-MM-dd"),
        endDate.ToString("yyyy-MM-dd"), string.Join("','", branchNos)));
    }

    public ICollection<IHandoverInfo> RunHandoverInfoQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate)
    {
      return new List<IHandoverInfo>(GetData<HandoverInfo>(Resources.QRY_HandoverInfo, startDate.ToString("yyyy-MM-dd"),
        endDate.ToString("yyyy-MM-dd"), string.Join("','", branchNos)));
    }

    public ICollection<IHandoverInfo_New> RunNewHandoverInfoQuery(ICollection<string> branchNos, DateTime startDate,
      DateTime endDate)
    {
      return
        new List<IHandoverInfo_New>(GetData<HandoverInfo_New>(Resources.QRY_HandoverInfo1,
          startDate.ToString("yyyy-MM-dd"),
          endDate.ToString("yyyy-MM-dd"), string.Join("','", branchNos)));
    }

    public ICollection<IPossibleHandover> RunPossibleHandoverQuery(ICollection<string> branchNos)
    {
      List<IPossibleHandover> possibleHandovers = new List<IPossibleHandover>();
      Parallel.ForEach(branchNos, new ParallelOptions
      {
        MaxDegreeOfParallelism = 5
      }, branch =>
      {
        try
        {
          var branchresult = GetData<PossibleHandover>(Resources.QRY_PossibleHandovers, branch);
          lock (possibleHandovers)
          {
            possibleHandovers.AddRange(branchresult);
          }
        }
        catch (Exception exception)
        {
          _logger.Error($"ERROR: {exception.Message} - {exception.StackTrace} - {branch}");
        }
      });

      return possibleHandovers;
    }

    public ICollection<IArrears> RunArrearsQuery(ICollection<string> branchNos)
    {
      List<IArrears> arrears = new List<IArrears>();
      Parallel.ForEach(branchNos, new ParallelOptions
      {
        MaxDegreeOfParallelism = 5
      }, branch =>
      {
        try
        {
          var branchresult = GetData<Arrears>(Resources.QRY_Arrears, branch);
          lock (arrears)
          {
            arrears.AddRange(branchresult);
          }
        }
        catch (Exception exception)
        {
          _logger.Error($"ERROR: {exception.Message} - {exception.StackTrace} - {branch}");
        }
      });

      return arrears;
      //return new List<IArrears>(GetData<Arrears>(Resources.QRY_Arrears, string.Join("','", branchNos)));
    }

    public ICollection<ICollections> RunCollectionsQuery(ICollection<string> branchNos)
    {
      var collections = new List<ICollections>();
      Parallel.ForEach(branchNos, new ParallelOptions
      {
        MaxDegreeOfParallelism = 5
      }, branch =>
      {
        try
        {
          var collection = new Collections {LegacyBranchNumber = branch};

          // this month
          var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
          var endDate = DateTime.Today;
          var resultMtd = GetData<QryCollections>(Resources.QRY_Collections, branch, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd")).FirstOrDefault();
          if (resultMtd != null)
          {
            collection.ReceivableThisMonth = resultMtd.Receivable;
            collection.ReceivedThisMonth = resultMtd.Received;
          }

          var oldestArrears = GetData<OldestArrears>(Resources.OldestArrearsPerBranch, branch).FirstOrDefault();
          if (oldestArrears != null)
          {
            collection.OldestArrearDate = oldestArrears.OldestArrearsDate;
          }

          // Past
          startDate = startDate.AddMonths(-3);
          endDate = startDate.AddMonths(3).AddDays(-1);

          var resultPast = GetData<QryCollections>(Resources.QRY_Collections, branch, startDate.ToString("yyyy-MM-dd"),
            endDate.ToString("yyyy-MM-dd")).FirstOrDefault();
          if (resultPast != null)
          {
            collection.ReceivablePast = resultPast.Receivable;
            collection.ReceivedPast = resultPast.Received;
          }

          lock (collections)
          {
            collections.Add(collection);
          }
        }
        catch (Exception exception)
        {
          _logger.Error($"ERROR: {exception.Message} - {exception.StackTrace} - {branch}");
        }
      });

      return collections;
    }

    public ICollection<IDebtorsBook> RunDebtorsBookQuery(ICollection<string> branchNos)
    {
      return new List<IDebtorsBook>(GetData<DebtorsBook>(Resources.QRY_Debtorsbook, string.Join("','", branchNos)));
    }

    public ICollection<ILoansFlagged> RunLoansFlaggedQuery(ICollection<string> branchNos)
    {
      return new List<ILoansFlagged>(GetData<LoansFlagged>(Resources.QRY_FlaggedAccounts, string.Join("','", branchNos)));
    }

    #endregion

    private List<T> GetData<T>(string templateQuery, params object[] parameters) where T : class, new()
    {
      var query = string.Format(templateQuery, parameters);
      var queryUtil = new Common.Utils.RawSql();
      var data = queryUtil.ExecuteObject<T>(query, _configService.AssConnection);
      return data;
    }

  }
}