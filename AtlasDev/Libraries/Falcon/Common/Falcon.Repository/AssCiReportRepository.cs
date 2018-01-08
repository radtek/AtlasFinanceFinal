using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Ass.Framework.Structures;
using Atlas.Common.Extensions;
using Atlas.Common.Utils;
using Atlas.Domain.Ass;
using Atlas.Domain.Ass.Models;
using Atlas.Domain.Model;
using Atlas.Reporting.DTO;
using AutoMapper;
using DevExpress.Xpo;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Repository
{
  public class AssCiReportRepository : IAssCiReportRepository
  {
    private readonly IConfigService _configService;
    private readonly IMappingEngine _mappingEngine;

    internal enum AssPayNo
    {
      PayNo1 = 1,
      PayNo2 = 2,
      PayNo3 = 3,
      PayNo4 = 4,
      PayNo5 = 5,
      PayNo6 = 6,
      PayNo12 = 12,
      PayNo24 = 24,
      Total = 0
    }

    public AssCiReportRepository(IConfigService configService, IMappingEngine mappingEngine,
      IConfiguration mapperConfiguration)
    {
      _configService = configService;
      _mappingEngine = mappingEngine;
    }

    public void ImportCiReport(ICollection<IBranch> branches, DateTime reportDate, ICollection<IBasicLoan> basicLoans,
      ICollection<ICollectionRefund> collectionRefunds,
      ICollection<IClientLoanInfo> clientLoanInfos, ICollection<IReswipes> reswipes,
      ICollection<IRolledAccounts> rolledAccounts, ICollection<IVAP> vaps)
    {
      using (var uow = new UnitOfWork())
      {
        // update Ci report record
        var branchIds = branches.Select(b => b.BranchId).Distinct().AsEnumerable();
        var ciReportBranches =
          new XPQuery<ASS_CiReport>(uow).Where(
            c => branchIds.Contains(c.Branch.BranchId) && c.Date.Date == reportDate.Date).ToList();

        foreach (var branch in branches)
        {
          foreach (var payno in EnumUtil.GetValues<AssPayNo>().Where(p => p != AssPayNo.Total))
          {
            var ciReportBranch =
              ciReportBranches.FirstOrDefault(b => b.Branch.BranchId == branch.BranchId && b.PayNo == payno.ToInt()) ??
              new ASS_CiReport(uow)
              {
                Date = reportDate,
                Branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(b => b.BranchId == branch.BranchId),
                CiReportVersion = GetLatestReportVersionByDate(reportDate, uow),
                PayNo = payno.ToInt()
              };

            var basicLoan =
              basicLoans.FirstOrDefault(b => b.PayNo == payno.ToInt() && b.LegacyBranchNumber == branch.LegacyBranchNum);
            var collectionRefund =
              collectionRefunds.FirstOrDefault(
                b => b.PayNo == payno.ToInt() && b.LegacyBranchNumber == branch.LegacyBranchNum);
            var clientLoanInfo =
              clientLoanInfos.FirstOrDefault(
                b => b.PayNo == payno.ToInt() && b.LegacyBranchNumber == branch.LegacyBranchNum);
            var reswipe =
              reswipes.FirstOrDefault(b => b.PayNo == payno.ToInt() && b.LegacyBranchNumber == branch.LegacyBranchNum);
            var rolledAccount =
              rolledAccounts.FirstOrDefault(
                b => b.PayNo == payno.ToInt() && b.LegacyBranchNumber == branch.LegacyBranchNum);
            var vap =
              vaps.FirstOrDefault(b => b.PayNo == payno.ToInt() && b.LegacyBranchNumber == branch.LegacyBranchNum);

            ciReportBranch.NoOfLoans = basicLoan == null ? 0 : basicLoan.Quantity;
            ciReportBranch.BranchLoans = basicLoan == null ? 0 : basicLoan.BranchLoans;
            ciReportBranch.Cheque = basicLoan == null ? 0 : basicLoan.Cheque;
            ciReportBranch.ChequeToday = basicLoan == null ? 0 : basicLoan.ChequeToday;
            ciReportBranch.Collections = collectionRefund == null ? 0 : collectionRefund.Collections;
            ciReportBranch.ExistingClientCount = clientLoanInfo == null ? 0 : clientLoanInfo.ExistingClientCount;
            ciReportBranch.ChargesExclVAT = basicLoan == null ? 0 : basicLoan.ChargesExclVAT;
            ciReportBranch.ChargesVAT = basicLoan == null ? 0 : basicLoan.ChargesVAT;
            ciReportBranch.TotalCharges = basicLoan == null ? 0 : basicLoan.TotalCharges;
            ciReportBranch.CreditLife = basicLoan == null ? 0 : basicLoan.CreditLife;
            ciReportBranch.LoanFeeExclVAT = basicLoan == null ? 0 : basicLoan.LoanFeeExclVAT;
            ciReportBranch.LoanFeeVAT = basicLoan == null ? 0 : basicLoan.LoanFeeVAT;
            ciReportBranch.LoanFeeInclVAT = basicLoan == null ? 0 : basicLoan.LoanFeeInclVAT;
            ciReportBranch.FuneralAddOn = basicLoan == null ? 0 : basicLoan.FuneralAddOn;
            ciReportBranch.AgeAddOn = basicLoan == null ? 0 : basicLoan.AgeAddOn;
            ciReportBranch.VAPExcl = basicLoan == null ? 0 : basicLoan.VAPExcl;
            ciReportBranch.VAPVAT = basicLoan == null ? 0 : basicLoan.VAPVAT;
            ciReportBranch.VAPIncl = basicLoan == null ? 0 : basicLoan.VAPIncl;
            ciReportBranch.TotalAddOn = basicLoan == null ? 0 : basicLoan.TotalAddOn;
            ciReportBranch.TotFeeExcl = basicLoan == null ? 0 : basicLoan.TotFeeExcl;
            ciReportBranch.TotFeeVAT = basicLoan == null ? 0 : basicLoan.TotFeeVAT;
            ciReportBranch.TotFeeIncl = basicLoan == null ? 0 : basicLoan.TotFeeIncl;
            ciReportBranch.NewClientAmount = clientLoanInfo == null ? 0 : clientLoanInfo.NewClientAmount;
            ciReportBranch.Refunds = collectionRefund == null ? 0 : collectionRefund.Refunds;
            ciReportBranch.ReswipeBankChange = reswipe == null ? 0 : reswipe.BankChange;
            ciReportBranch.ReswipeInstalmentChange = reswipe == null ? 0 : reswipe.InstalmentChange;
            ciReportBranch.ReswipeLoanTermChange = reswipe == null ? 0 : reswipe.LoanTermChange;
            ciReportBranch.NewClientNoOfLoans = clientLoanInfo == null ? 0 : clientLoanInfo.NewClientQuantity;
            ciReportBranch.RevivedClientCount = clientLoanInfo == null ? 0 : clientLoanInfo.RevivedClientCount;
            ciReportBranch.RevivedClientAmount = clientLoanInfo == null ? 0 : clientLoanInfo.RevivedClientAmount;
            ciReportBranch.RollbackValue = rolledAccount == null ? 0 : rolledAccount.RollbackValue;
            ciReportBranch.SalesRepLoans = basicLoan == null ? 0 : basicLoan.SalesRepLoans;
            ciReportBranch.VapDeniedByConWithAuth = vap == null ? 0 : vap.VapDeniedByConWithAuth;
            ciReportBranch.VapDeniedByConWithOutAuth = vap == null ? 0 : vap.VapDeniedByConWithOutAuth;
            ciReportBranch.VapExcludedLoans = vap == null ? 0 : vap.VapExcludedLoans;
            ciReportBranch.VapLinkedLoans = vap == null ? 0 : vap.VapLinkedLoans;
            ciReportBranch.VapLinkedLoansValue = vap == null ? 0 : vap.VapLinkedLoansValue;
          }
        }

        uow.CommitChanges();
      }
    }

    public void ImportCiReportPossibleHandover(ICollection<IBranch> branches, DateTime reportDate,
      ICollection<IPossibleHandover> possibleHandovers,
      ICollection<IArrears> arrears, ICollection<IDebtorsBook> debtorsBooks, ICollection<ILoansFlagged> loansFlaggeds,
      ICollection<ICollections> collections)
    {
      using (var uow = new UnitOfWork())
      {
        var branchIds = branches.Select(b => b.BranchId).Distinct().AsEnumerable();
        var ciReportPossibleHandoverBranches =
          new XPQuery<ASS_CiReportPossibleHandover>(uow).Where(
            c => branchIds.Contains(c.Branch.BranchId) && c.Date.Date == reportDate.Date).ToList();

        foreach (var branch in branches)
        {
          var ciReportPossibleHandoverBranch =
            ciReportPossibleHandoverBranches.FirstOrDefault(b => b.Branch.BranchId == branch.BranchId) ??
            new ASS_CiReportPossibleHandover(uow)
            {
              Date = reportDate,
              Branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(b => b.BranchId == branch.BranchId),
              CiReportVersion = GetLatestReportVersionByDate(reportDate, uow),
            };

          var possibleHandover =
            possibleHandovers.FirstOrDefault(b => b.LegacyBranchNumber == branch.LegacyBranchNum);
          var arrear =
            arrears.FirstOrDefault(b => b.LegacyBranchNumber == branch.LegacyBranchNum);
          var debtorsBook =
            debtorsBooks.FirstOrDefault(b => b.LegacyBranchNumber == branch.LegacyBranchNum);
          var loansFlagged =
            loansFlaggeds.FirstOrDefault(b => b.LegacyBranchNumber == branch.LegacyBranchNum);
          var collection =
            collections.Where(b => b.LegacyBranchNumber == branch.LegacyBranchNum).ToList();

          ciReportPossibleHandoverBranch.Arrears = arrear == null ? 0 : arrear.ArrearsValue;
          ciReportPossibleHandoverBranch.DebtorsBookValue = debtorsBook == null ? 0 : debtorsBook.DebtorsBookValue;
          ciReportPossibleHandoverBranch.FlaggedNoOfLoans = loansFlagged == null ? 0 : loansFlagged.NoOfLoans;
          ciReportPossibleHandoverBranch.FlaggedOverdueValue = loansFlagged == null ? 0 : loansFlagged.OverdueValue;
          ciReportPossibleHandoverBranch.NextPossibleHandOvers = possibleHandover == null
            ? 0
            : possibleHandover.NextPossibleHandOvers;
          var dateTime = collection.Max(a => a.OldestArrearDate);
          if (dateTime != null)
            ciReportPossibleHandoverBranch.OldestArrearsDate = dateTime.Value;
          ciReportPossibleHandoverBranch.PossibleHandOvers = possibleHandover == null
            ? 0
            : possibleHandover.PossibleHandOvers;
          ciReportPossibleHandoverBranch.ReceivablePast =
            collections.Where(c => c.LegacyBranchNumber == branch.LegacyBranchNum).Sum(c => c.ReceivablePast);
          ciReportPossibleHandoverBranch.ReceivableThisMonth =
            collections.Where(c => c.LegacyBranchNumber == branch.LegacyBranchNum).Sum(c => c.ReceivableThisMonth);
          ciReportPossibleHandoverBranch.ReceivedPast =
            collections.Where(c => c.LegacyBranchNumber == branch.LegacyBranchNum).Sum(c => c.ReceivedPast);
          ciReportPossibleHandoverBranch.ReceivedThisMonth =
            collections.Where(c => c.LegacyBranchNumber == branch.LegacyBranchNum).Sum(c => c.ReceivedThisMonth);
        }

        uow.CommitChanges();
      }
    }

    public void ImportCiReportHandoverInfo(ICollection<IBranch> branches,
      ICollection<IHandoverInfo_New> handoverInfos)
    {
      using (var uow = new UnitOfWork())
      {
        var branchIds = branches.Select(b => b.BranchId).Distinct().AsEnumerable();
        var reportDates = handoverInfos.Select(h => h.HandoverDate.Date).Distinct().ToArray();
        var ciReportPossibleHandoverBranches =
          new XPQuery<ASS_CiReportHandoverInfo>(uow).Where(
            c => branchIds.Contains(c.Branch.BranchId) && reportDates.Contains(c.Date.Date)).ToList();

        foreach (var branch in branches)
        {
          foreach (var payno in EnumUtil.GetValues<AssPayNo>().Where(p => p != AssPayNo.Total))
          {
            foreach (var reportDate in reportDates)
            {
              var ciReportPossibleHandoverBranch =
                ciReportPossibleHandoverBranches.FirstOrDefault(
                  b => b.Branch.BranchId == branch.BranchId && b.Date == reportDate.Date && b.PayNo == payno.ToInt()) ??
                new ASS_CiReportHandoverInfo(uow)
                {
                  Date = reportDate.Date,
                  PayNo = payno.ToInt(),
                  Branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(b => b.BranchId == branch.BranchId),
                  CiReportVersion = GetLatestReportVersionByDate(reportDate, uow),
                };

              var handoverInfo =
                handoverInfos.Where(
                  b =>
                    b.LegacyBranchNumber == branch.LegacyBranchNum && b.HandoverDate.Date == reportDate &&
                    b.PayNo == payno.ToInt()).ToList();

              ciReportPossibleHandoverBranch.HandedOverLoansAmount = handoverInfo.Sum(h => h.Amount);
              ciReportPossibleHandoverBranch.HandedOverLoansQuantity = handoverInfo.Sum(h => h.Quantity);
              ciReportPossibleHandoverBranch.HandedOverClientQuantity = handoverInfo.Sum(h => h.ClientQuantity);
            }
          }
        }

        uow.CommitChanges();
      }
    }

    public void ImportCiReportScore(ICollection<IBranch> branches, DateTime reportDate,
      ICollection<IBasicLoan> basicLoans)
    {
      using (var uow = new UnitOfWork())
      {
        // update Ci report record
        var branchIds = branches.Select(b => b.BranchId).Distinct().AsEnumerable();
        var ciReportScoreBranches =
          new XPQuery<ASS_CiReportScore>(uow).Where(
            c => branchIds.Contains(c.Branch.BranchId) && c.Date.Date == reportDate.Date).ToList();

        foreach (var branch in branches)
        {
          foreach (var payno in EnumUtil.GetValues<AssPayNo>().Where(p => p != AssPayNo.Total))
          {
            var ciReportScoreBranch =
              ciReportScoreBranches.FirstOrDefault(b => b.Branch.BranchId == branch.BranchId && b.PayNo == payno.ToInt()) ??
              new ASS_CiReportScore(uow)
              {
                Date = reportDate,
                Branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(b => b.BranchId == branch.BranchId),
                CiReportVersion = GetLatestReportVersionByDate(reportDate, uow),
                PayNo = payno.ToInt()
              };

            var basicLoan =
              basicLoans.FirstOrDefault(b => b.PayNo == payno.ToInt() && b.LegacyBranchNumber == branch.LegacyBranchNum);

            ciReportScoreBranch.ScoreAboveXWeekly = basicLoan == null ? 0 : basicLoan.ScoreAbove615Weekly;
            ciReportScoreBranch.ScoreAboveXBiWeekly = basicLoan == null ? 0 : basicLoan.ScoreAbove615BiWeekly;
            ciReportScoreBranch.ScoreAboveXMonthly = basicLoan == null ? 0 : basicLoan.ScoreAbove615Monthly;
          }
        }

        uow.CommitChanges();
      }
    }

    public void ImportCiReportBureauProducts(ICollection<IBranch> branches, DateTime reportDate,
      ICollection<ICompuscanProducts> products)
    {
      using (var uow = new UnitOfWork())
      {
        // update Ci report record
        var branchIds = branches.Select(b => b.BranchId).Distinct().AsEnumerable();
        var ciReportCompuscanProducts =
          new XPQuery<ASS_CiReportCompuscanProduct>(uow).Where(
            c => branchIds.Contains(c.Branch.BranchId) && c.Date.Date == reportDate.Date).ToList();

        foreach (var branch in branches)
        {
          var ciReportCompuscanProduct =
            ciReportCompuscanProducts.FirstOrDefault(b => b.Branch.BranchId == branch.BranchId) ??
            new ASS_CiReportCompuscanProduct(uow)
            {
              Date = reportDate,
              Branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(b => b.BranchId == branch.BranchId),
              CiReportVersion = GetLatestReportVersionByDate(reportDate, uow)
            };

          var compuscanProduct =
            products.FirstOrDefault(b => b.BranchId == branch.BranchId);

          if (compuscanProduct != null)
          {
            ciReportCompuscanProduct.Declined = compuscanProduct.Declined;
            ciReportCompuscanProduct.FiveToSixMonth = compuscanProduct.FiveToSixMonths;
            ciReportCompuscanProduct.OneMonth = compuscanProduct.OneMonth;
            ciReportCompuscanProduct.OneMonthCapped = compuscanProduct.OneMCapped;
            ciReportCompuscanProduct.OneMonthThin = compuscanProduct.OneMThin;
            ciReportCompuscanProduct.TwoToFourMonth = compuscanProduct.TwoToFourMonths;
            ciReportCompuscanProduct.TwelveMonth = compuscanProduct.TwelveMonths;
          }
        }

        uow.CommitChanges();
      }
    }

    public ICollection<ASS_CiReportLastImportDate> GetBranchLastImportDate()
    {
      return GetCustomQuery<ASS_CiReportLastImportDate>(Atlas.Reporting.Properties.Resources.ASS_CiReportLastImportDate);
    }

    public List<ASS_CiReportBranchSummary> GetCiReportBranchSummaries(ICollection<long> branchIds, DateTime startDate,
      DateTime endDate)
    {
      return GetCustomQuery<ASS_CiReportBranchSummary>(string.Format(Atlas.Reporting.Properties.Resources.ASS_CiReportBranchSummaries,
          startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), string.Join(",", branchIds)));
    }

    public List<ASS_CiReportSales> GetCiReportSales(ICollection<long> branchIds, DateTime startDate, DateTime endDate,
      CiReportGrouping grouping)
    {
      var ciReportSales =
        GetCustomQuery<ASS_CiReportSales>(string.Format(Atlas.Reporting.Properties.Resources.ASS_CiReportSales,
          startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), string.Join(",", branchIds),
          grouping.ToStringEnum(), grouping == CiReportGrouping.Branch ? "BranchId" : (grouping == CiReportGrouping.Region?"Region":"Region\" - BR.\"Region")));

      var ciReportSalesTargets =
        GetCustomQuery<ASS_CiReportSalesTargets>(
          string.Format(Atlas.Reporting.Properties.Resources.ASS_CiReportSalesTargets,
          startDate.ToString("yyyy-MM-dd"), (endDate.Date>DateTime.Today? DateTime.Today: endDate).ToString("yyyy-MM-dd"), string.Join(",", branchIds),
            grouping.ToStringEnum()));

      foreach (var ciReportSale in ciReportSales)
      {
        var totalRow = ciReportSales.FirstOrDefault(b => b.PayNo == 0 && b.Id == ciReportSale.Id);
        if (totalRow != null && totalRow.NewClientNoOfLoans > 0)
        {
          ciReportSale.NewClientMix = ciReportSale.NewClientNoOfLoans/(float) totalRow.NewClientNoOfLoans;
        }
        var ciReportSalesTarget =
          ciReportSalesTargets.FirstOrDefault(t => t.Id == ciReportSale.Id && t.PayNo == ciReportSale.PayNo);

        if (ciReportSalesTarget != null)
        {
          ciReportSale.Target = ciReportSalesTarget.Target;
          ciReportSale.TargetPercent = ciReportSalesTarget.TargetPercent > 100 ? 100 : ciReportSalesTarget.TargetPercent;
          ciReportSale.DeviationPercent = ciReportSale.ActualPercent - ciReportSale.TargetPercent;
        }
      }

      return ciReportSales;
    }

    public List<ASS_CiReportCompuscanScores> GetCiReportScores(ICollection<long> branchIds,
      DateTime startDate, DateTime endDate, CiReportGrouping grouping)
    {
      return
        GetCustomQuery<ASS_CiReportCompuscanScores>(
          string.Format(Atlas.Reporting.Properties.Resources.ASS_CiReportScore,
            startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"), string.Join(",", branchIds),
            grouping.ToStringEnum()));
    }

    public List<ASS_CiReportPossibleHandovers> GetCiReportPossibleHandovers(ICollection<IBranch> branches,
      DateTime endDate, CiReportGrouping grouping)
    {
      var possibleHandovers = new List<ASS_CiReportPossibleHandovers>();
      var totalRows =
        GetCustomQuery<ASS_CiReportPossibleHandovers>(
          string.Format(Atlas.Reporting.Properties.Resources.ASS_CiReportPossibleHandovers,
            endDate.ToString("yyyy-MM-dd"), string.Join(",", branches.Select(b => b.BranchId)),
            grouping.ToStringEnum()
            )).OrderBy(b=>b.Id);

      foreach (var totalRow in totalRows)
      {
        totalRow.ParentId = totalRow.Id;

        var row = totalRow;
        var detail =
          GetCustomQuery<ASS_CiReportPossibleHandovers>(
            string.Format(Atlas.Reporting.Properties.Resources.ASS_CiReportPossibleHandovers,
              endDate.ToString("yyyy-MM-dd"),
              grouping == CiReportGrouping.Region
                ? string.Join(",", branches.Where(b => b.RegionId == row.Id).Select(b => b.BranchId))
                : string.Join(",", branches.Select(b => b.BranchId)),
              ((CiReportGrouping)(grouping.ToInt() - 1)).ToStringEnum()));

        detail.ForEach(d =>
        {
          d.ParentId = totalRow.Id;
        });

        possibleHandovers.AddRange(detail);

        possibleHandovers.Add(totalRow);
      }

      return possibleHandovers;
    }

    public List<ASS_CiReportLowMean> GetCiReportLowMeans(ICollection<long> branchIds, DateTime startDate,
      DateTime endDate, float meanLimit)
    {
      return
        GetCustomQuery<ASS_CiReportLowMean>(
          string.Format(Atlas.Reporting.Properties.Resources.ASS_CiReportLowMean, startDate.ToString("yyyy-MM-dd"),
            endDate.ToString("yyyy-MM-dd"), string.Join(",", branchIds), meanLimit));
    }

    public List<ASS_CiReportVersion> GetCiReportVersion(DateTime startDate, DateTime endDate)
    {
      using (var uow = new UnitOfWork())
      {
        var ciReportVersions =
          new XPQuery<ASS_CiReportVersion>(uow).Where(
            r => r.VersionDate.Date <= startDate.Date && r.VersionDate.Date <= endDate.Date)
            .OrderByDescending(v => v.VersionDate)
            .ToList();

        return _mappingEngine.Map<List<ASS_CiReportVersion>>(ciReportVersions);
      }
    }

    private ASS_CiReportVersion GetLatestReportVersionByDate(DateTime reportDate, Session uow)
    {
      return
        new XPQuery<ASS_CiReportVersion>(uow).Where(d => d.VersionDate.Date >= reportDate.Date)
          .OrderByDescending(d => d.VersionDate)
          .FirstOrDefault();
    }

    internal List<T> GetCustomQuery<T>(string sql) where T : class, new()
    {
      var queryUtil = new RawSql();
      var data = queryUtil.ExecuteObject<T>(sql, _configService.AtlasCoreConnection, commandTimeout: 3600);
      return data;
    }
  }
}