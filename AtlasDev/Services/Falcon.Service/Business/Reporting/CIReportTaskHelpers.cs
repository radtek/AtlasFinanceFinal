using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Atlas.Ass.Framework.Repository;
using Atlas.Common.Extensions;
using Atlas.Common.Utils;
using Atlas.Domain.Model;
using Atlas.ThirdParty.CompuScan.Enquiry;
using DevExpress.Xpo;
using Falcon.Service.Core;
using Serilog;

namespace Falcon.Service.Business.Reporting
{
  public class CIReportTaskHelpers
  {
    private readonly IAssCiRepository _assCiRepository;
    private readonly IAssBureauRepository _assBureauRepository;

    public CIReportTaskHelpers(IAssCiRepository assCiRepository, IAssBureauRepository assBureaRepository)
    {
      _assCiRepository = assCiRepository;
      _assBureauRepository = assBureaRepository;
    }
    
    public enum CompuscanProductTypes
    {
      [Description("1 Month")]
      OneMonth,
      [Description("1M Thin")]
      OneMThin,
      [Description("1M Capped")]
      OneMCapped,
      [Description("2 To 4 Month")]
      TwoToFourMonths,
      [Description("5 To 6 Month")]
      FiveToSixMonths,
      [Description("12 Month")]
      TwelveMonths
    }

    public void RunLastBranchSyncDate(string[] branchNos, string jobName)
    {
      Log.Information(string.Format("[FalconService][Task][{0}] - Started Query Run Last Branch Sync Date", jobName));

      using (var uow = new UnitOfWork())
      {
        var branchServers = new XPQuery<ASS_BranchServer>(uow).Where(b => branchNos.Contains(b.Branch.LegacyBranchNum));
        foreach (var branchServer in branchServers)
        {
          RedisConnection.SetStringFromObject(
            string.Format(AssReporting.REDIS_KEY_BRANCH_LAST_SYNC_DATE, branchServer.Branch.LegacyBranchNum),
            new Tuple<DateTime, string, long>(branchServer.LastSyncDT, branchServer.Branch.Company.Name,
              branchServer.ClientClientCurrentRecId - branchServer.LastProcessedClientRecId), new TimeSpan(1, 30, 0));
        }
      }

      Log.Information(string.Format("[FalconService][Task][{0}] - Finished Query Run Last Branch Sync Date", jobName));
    }

    public void RunClientInfoQuery(string[] branchNos, DateTime startDate, DateTime endDate, string jobName,
      TimeSpan? expiryTime = null)
    {
      Log.Information(
        string.Format("[FalconService][Task][{2}] - Started Query Run Client Loan Info: {0} - {1}",
          startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));

      var thisMonthClientLoanInfo = _assCiRepository.RunClientInfoQuery(branchNos.ToArray(), startDate, endDate);
      foreach (var branchNo in branchNos)
      {
        var thisMonthBranchResult = thisMonthClientLoanInfo.Where(b => b.LegacyBranchNumber == branchNo).ToList();
        RedisConnection.SetStringFromObject(
          string.Format(AssReporting.REDIS_KEY_CLIENT_LOAN, branchNo, startDate.ToString("ddMMyyyy"),
            endDate.ToString("ddMMyyyy")), thisMonthBranchResult, expiryTime ?? new TimeSpan(1, 30, 0));
      }

      Log.Information(
        string.Format("[FalconService][Task][{2}] - Finished Query Run Client Loan Info: {0} - {1}",
          startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));
    }

    public void RunVapQuery(string[] branchNos, DateTime startDate, DateTime endDate, string jobName, TimeSpan? expiryTime = null)
    {
      Log.Information(string.Format("[FalconService][Task][{2}] - Started Query Run VAP: {0} - {1}",
        startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));

      var thisMonthVap = _assCiRepository.RunVapQuery(branchNos.ToArray(), startDate, endDate);
      foreach (var branchNo in branchNos)
      {
        var thisMonthBranchResult = thisMonthVap.Where(b => b.LegacyBranchNumber == branchNo).ToList();
        RedisConnection.SetStringFromObject(
          string.Format(AssReporting.REDIS_KEY_VAP, branchNo, startDate.ToString("ddMMyyyy"),
            endDate.ToString("ddMMyyyy")), thisMonthBranchResult, expiryTime ?? new TimeSpan(1, 30, 0));
      }

      Log.Information(string.Format("[FalconService][Task][{2}] - Finished Query Run VAP: {0} - {1}",
        startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));
    }

    public void RunRolledAccounts(string[] branchNos, DateTime startDate, DateTime endDate, string jobName, TimeSpan? expiryTime = null)
    {
      Log.Information(
        string.Format("[FalconService][Task][{2}] - Started Query Run Rolled Accounts: {0} - {1}",
          startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));

      var rolledAccounts = _assCiRepository.RunRolledAccountsQuery(branchNos.ToArray(), startDate, endDate);
      foreach (var branchNo in branchNos)
      {
        var thisMonthBranchResult = rolledAccounts.Where(b => b.LegacyBranchNumber == branchNo).ToList();
        RedisConnection.SetStringFromObject(
          string.Format(AssReporting.REDIS_KEY_ROLLED_ACCOUNTS, branchNo, startDate.ToString("ddMMyyyy"),
            endDate.ToString("ddMMyyyy")), thisMonthBranchResult, expiryTime ?? new TimeSpan(1, 30, 0));
      }

      Log.Information(
        string.Format("[FalconService][Task][{2}] - Finished Query Run Rolled Accounts: {0} - {1}",
          startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));
    }

    public void RunReswipeInfoQuery(string[] branchNos, DateTime startDate, DateTime endDate, string jobName,
      TimeSpan? expiryTime = null)
    {
      Log.Information(string.Format(
        "[FalconService][Task][{2}] - Started Query Run Reswipe Info: {0} - {1}",
        startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));

      var thisMonthData = _assCiRepository.RunReswipesQuery(branchNos.ToArray(), startDate, endDate);
      foreach (var branchNo in branchNos)
      {
        var thisMonthBranchResult = thisMonthData.Where(b => b.LegacyBranchNumber == branchNo).ToList();
        RedisConnection.SetStringFromObject(
          string.Format(AssReporting.REDIS_KEY_RESWIPE_INFO, branchNo, startDate.ToString("ddMMyyyy"),
            endDate.ToString("ddMMyyyy")), thisMonthBranchResult, expiryTime ?? new TimeSpan(1, 30, 0));
      }

      Log.Information(string.Format(
        "[FalconService][Task][{2}] - Finished Query Run Reswipe Info: {0} - {1}",
        startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));
    }

    public void RunBasicInfoQuery(string[] branchNos, DateTime startDate, DateTime endDate, string jobName, TimeSpan? expiryTime = null)
    {
      Log.Information(string.Format("[FalconService][Task][{2}] - Started Query Run Basic Loan: {0} - {1}",
        startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));

      var thisMonthBasicLoan = _assCiRepository.RunBasicInfoQuery(branchNos.ToArray(), startDate, endDate);
      foreach (var branchNo in branchNos)
      {
        var thisMonthBranchResult = thisMonthBasicLoan.Where(b => b.LegacyBranchNumber == branchNo).ToList();
        RedisConnection.SetStringFromObject(
          string.Format(AssReporting.REDIS_KEY_BASIC_LOAN, branchNo, startDate.ToString("ddMMyyyy"),
            endDate.ToString("ddMMyyyy")), thisMonthBranchResult, expiryTime ?? new TimeSpan(1, 30, 0));
      }

      Log.Information(string.Format("[FalconService][Task][{2}] - Finished Query Run Basic Loan: {0} - {1}",
        startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));
    }

    // TODO: move to bureau Repo
    public void RunCompuScanProducts(long[] branchIds, DateTime startDate, DateTime endDate, string jobName)
    {
      Log.Information(
        string.Format("[FalconService][Task][{2}] - Started Query Run CompuScan Products: {0} - {1}",
          startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));

      if ((DateTime.Today - startDate.Date).TotalDays <= 90)
      {
        using (var uow = new UnitOfWork())
        {
          var lastEnquiryDate =
            RedisConnection.GetObjectFromString<DateTime?>(string.Format(AssReporting.REDIS_KEY_LAST_COMPUSCAN_ENQUIRY,
              startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy"))) ?? startDate;

          var latestsEnquiryDate =
            new XPQuery<BUR_Enquiry>(uow).Where(
              p =>
                p.EnquiryDate >= lastEnquiryDate && p.EnquiryDate.Date <= endDate &&
                branchIds.Contains(p.Branch.BranchId) && p.Storage.Count > 0).Max(e => e.EnquiryDate);
          if (latestsEnquiryDate > lastEnquiryDate)
          {
            var branchProducts = new Dictionary<string, Dictionary<string, int>>();
            List<BUR_Enquiry> enquiries;
            var i = 0;
            const string declined = "Declined";
            const string totalCompuscanEnquiries = "Total Compuscan Enquiries";
            do
            {
              enquiries =
                new XPQuery<BUR_Enquiry>(uow).Where(
                  p =>
                    p.EnquiryDate >= lastEnquiryDate && p.EnquiryDate <= latestsEnquiryDate &&
                    branchIds.Contains(p.Branch.BranchId) && p.IsSucess)
                  .OrderBy(e => e.EnquiryId)
                  .Skip(i * 100)
                  .Take(100)
                  .ToList();
              i++;
              foreach (var enq in enquiries)
              {
                if (!branchProducts.ContainsKey(enq.Branch.LegacyBranchNum))
                {
                  var branchProduct =
                    RedisConnection.GetObjectFromString<Dictionary<string, int>>(
                      string.Format(AssReporting.REDIS_KEY_COMPUSCAN_PRODUCTS, enq.Branch.LegacyBranchNum,
                        startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy"))) ??
                    new Dictionary<string, int> { { declined, 0 }, { totalCompuscanEnquiries, 0 } };
                  branchProducts.Add(enq.Branch.LegacyBranchNum, branchProduct);
                }

                if (enq.Storage.FirstOrDefault() != null)
                {
                  var burStorage = enq.Storage.FirstOrDefault();
                  if (burStorage != null && burStorage.ResponseMessage != null)
                  {
                    var storage = enq.Storage.FirstOrDefault();
                    if (storage != null)
                    {
                      var dbResult =
                        ((ResponseResultV2)
                          Xml.DeSerialize<ResponseResultV2>(
                            Compression.Decompress(storage.ResponseMessage)));

                      if (dbResult != null)
                      {
                        if (_assBureauRepository.DoesNlrExistsInAss(dbResult.NLREnquiryReferenceNo))
                        {
                          var foundSuccess = false;
                          branchProducts[enq.Branch.LegacyBranchNum][totalCompuscanEnquiries]++;

                          if (dbResult.Products.Count > 0)
                          {
                            var compuscanProductTypes =
                              EnumUtil.GetValues<CompuscanProductTypes>().Reverse().ToList();
                            compuscanProductTypes.ForEach(p =>
                            {
                              if (!branchProducts[enq.Branch.LegacyBranchNum].ContainsKey(p.ToStringEnum()))
                                branchProducts[enq.Branch.LegacyBranchNum].Add(p.ToStringEnum(), 0);
                            });
                            if (dbResult.Products != null)
                            {
                              foreach (var compuscanProductType in compuscanProductTypes.OrderByDescending(c => c))
                              {
                                var prod =
                                  dbResult.Products.FirstOrDefault(
                                    p =>
                                      string.Equals(p.Description.Trim(), compuscanProductType.ToStringEnum()) &&
                                      string.Equals(p.Outcome, "Y"));
                                if (prod != null)
                                {
                                  branchProducts[enq.Branch.LegacyBranchNum][prod.Description.Trim()]++;
                                  foundSuccess = true;
                                  break;
                                }
                              }
                            }
                          }

                          if (!foundSuccess)
                          {
                            if (!branchProducts[enq.Branch.LegacyBranchNum].ContainsKey(declined))
                              branchProducts[enq.Branch.LegacyBranchNum].Add(declined, 0);
                            branchProducts[enq.Branch.LegacyBranchNum][declined]++;
                          }
                        }
                      }
                    }
                  }
                }
              }
            } while (enquiries.Count == 100);

            foreach (var branchProduct in branchProducts)
            {
              RedisConnection.SetStringFromObject(
                string.Format(AssReporting.REDIS_KEY_COMPUSCAN_PRODUCTS, branchProduct.Key,
                  startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")), branchProduct.Value,
                new TimeSpan(90, 0, 0, 0)); // expires in 90 days
            }

            RedisConnection.SetStringFromObject<DateTime?>(
              string.Format(AssReporting.REDIS_KEY_LAST_COMPUSCAN_ENQUIRY, startDate.ToString("ddMMyyyy"),
                endDate.ToString("ddMMyyyy")), latestsEnquiryDate, new TimeSpan(90, 0, 0, 0)); // expires in 90 days
          }
        }
      }

      Log.Information(
        string.Format("[FalconService][Task][{2}] - Finished Query Run CompuScan Products: {0} - {1}",
          startDate.ToString("dd/MM/yyyy"), endDate.ToString("dd/MM/yyyy"), jobName));
    }
  }
}
