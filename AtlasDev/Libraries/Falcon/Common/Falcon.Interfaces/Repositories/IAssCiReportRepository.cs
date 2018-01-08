using System;
using System.Collections.Generic;
using System.ComponentModel;
using Atlas.Ass.Framework.Structures;
using Atlas.Domain.Ass;
using Atlas.Domain.Ass.Models;
using Atlas.Reporting.DTO;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Interfaces.Repositories
{
  public interface IAssCiReportRepository
  {
    void ImportCiReport(ICollection<IBranch> branches, DateTime reportDate, ICollection<IBasicLoan> basicLoans,
      ICollection<ICollectionRefund> collectionRefunds,
      ICollection<IClientLoanInfo> clientLoanInfos, ICollection<IReswipes> reswipes,
      ICollection<IRolledAccounts> rolledAccounts, ICollection<IVAP> vaps);

    void ImportCiReportPossibleHandover(ICollection<IBranch> branches, DateTime reportDate,
      ICollection<IPossibleHandover> possibleHandovers,
      ICollection<IArrears> arrears, ICollection<IDebtorsBook> debtorsBooks, ICollection<ILoansFlagged> loansFlaggeds,
      ICollection<ICollections> collections);

    void ImportCiReportScore(ICollection<IBranch> branches, DateTime reportDate,
      ICollection<IBasicLoan> basicLoans);

    void ImportCiReportHandoverInfo(ICollection<IBranch> branches,
      ICollection<IHandoverInfo_New> handoverInfos);

    void ImportCiReportBureauProducts(ICollection<IBranch> branches, DateTime reportDate, ICollection<ICompuscanProducts> products);

    ICollection<ASS_CiReportLastImportDate> GetBranchLastImportDate();

    List<ASS_CiReportBranchSummary> GetCiReportBranchSummaries(ICollection<long> branchIds, DateTime startDate,
      DateTime endDate);

    List<ASS_CiReportSales> GetCiReportSales(ICollection<long> branchIds, DateTime startDate, DateTime endDate,
      CiReportGrouping grouping);

    List<ASS_CiReportCompuscanScores> GetCiReportScores(ICollection<long> branchIds, DateTime startDate,
      DateTime endDate, CiReportGrouping grouping);

    List<ASS_CiReportPossibleHandovers> GetCiReportPossibleHandovers(ICollection<IBranch> branches, DateTime endDate, CiReportGrouping grouping);

    List<ASS_CiReportLowMean> GetCiReportLowMeans(ICollection<long> branchIds, DateTime startDate, DateTime endDate,
      float meanLimit);

    // TODO: change to interface
    List<ASS_CiReportVersion> GetCiReportVersion(DateTime startDate, DateTime endDate);
  }

  public enum CiReportGrouping
  {
    [Description("BR.\"BranchId\" AS \"Id\", CP.\"Name\"")]
    Branch = 1,
    [Description("RG.\"RegionId\" AS \"Id\", RG.\"Description\" AS \"Name\"")]
    Region = 2,
    [Description("0 AS \"Id\", '' AS \"Name\"")]
    None = 3
  }
}