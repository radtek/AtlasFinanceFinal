using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Ass.Framework.Structures;
using Atlas.Domain.Model;
using DevExpress.Xpo;
using Falcon.Service.Core;
using Falcon.Common.Structures.Branch;
using Falcon.Common.Structures.Report.Ass;
using Falcon.Common.Structures.Report.General;
using Serilog;
using Region = Falcon.Common.Structures.Region;

namespace Falcon.Service.Business.Reporting
{
  public class AssReporting
  {
    public const string REDIS_KEY_BASIC_LOAN = "falcon.ass.reporting.basicloan.{0}.{1}.{2}";
    public const string REDIS_KEY_CLIENT_LOAN = "falcon.ass.reporting.clientloan.{0}.{1}.{2}";
    public const string REDIS_KEY_VAP = "falcon.ass.reporting.vap.{0}.{1}.{2}";
    public const string REDIS_KEY_COLLECTION_REFUND = "falcon.ass.reporting.collectionrefund.{0}.{1}.{2}";
    public const string REDIS_KEY_HANDOVER_INFO = "falcon.ass.reporting.handoverinfo.{0}.{1}.{2}";
    public const string REDIS_KEY_RESWIPE_INFO = "falcon.ass.reporting.reswipeinfo.{0}.{1}.{2}";
    public const string REDIS_KEY_ROLLED_ACCOUNTS = "falcon.ass.reporting.rolledaccounts.{0}.{1}.{2}";
    public const string REDIS_KEY_POSSIBLE_HANDOVERS = "falcon.ass.reporting.possiblehandovers.{0}";
    public const string REDIS_KEY_ARREARS = "falcon.ass.reporting.arrears.{0}";
    public const string REDIS_KEY_COLLECTIONS = "falcon.ass.reporting.collections.{0}";
    public const string REDIS_KEY_DEBTORS_BOOK = "falcon.ass.reporting.debtorsbook.{0}";
    public const string REDIS_KEY_LOANS_FLAGGED = "falcon.ass.reporting.loansflagged.{0}";
    public const string REDIS_KEY_PERSON_REGION = "falcon.ass.reporting.person.{0}.regions";
    public const string REDIS_KEY_PERSON_REGION_BRANCH = "falcon.ass.reporting.person.{0}.region.branch";
    public const string REDIS_KEY_REGION_BRANCH = "falcon.ass.region.{0}.branches";
    public const string REDIS_KEY_BUDGETS = "falcon.ass.reporting.budgets.{0}.{1}.{2}";
    public const string REDIS_KEY_DAILY_SALES_TARGET = "falcon.ass.reporting.dailysalestarget.{0}.{1}";
    public const string REDIS_KEY_BRANCH_LAST_SYNC_DATE = "falcon.ass.reporting.branch.last.sync.date.{0}";
    public const string REDIS_KEY_LAST_POSSIBLE_HANDOVERS_RUN = "falcon.ass.reporting.lastphr";
    public const string REDIS_KEY_LAST_ONE_TIME_RUN = "falcon.ass.reporting.lastotr";
    public const string REDIS_KEY_EMAIL_SENT = "falcon.ass.reporting.emailsent";
    public const string REDIS_KEY_LAST_COMPUSCAN_ENQUIRY = "falcon.ass.reporting.lastcse2.{0}.{1}";
    public const string REDIS_KEY_COMPUSCAN_PRODUCTS = "falcon.ass.reporting.compuscanproducts.{0}.{1}.{2}";
    public const string REDIS_KEY_BRANCH_TARGET = "falcon.ass.reporting.monthlybudgets.{0}";
    public const string REDIS_KEY_BRANCH_80_PERC_LESS_SYNC_DATA = "falcon.ass.reporting.branch.80perc.sync.data.{0}";
    public const string REDIS_KEY_LAST_POSSIBLE_EVERY_HOUR_RUN = "falcon.ass.reporting.everyhr";


    public static List<Region> GetPersonRegions(long personId)
    {
      var regions = RedisConnection.GetObjectFromString<List<Region>>(string.Format(REDIS_KEY_PERSON_REGION, personId));
      if (regions == null)
      {
        using (var uow = new UnitOfWork())
        {
          //var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == personId);
          //if (person.Branch == null)
          //{
          //  return new List<Structures.Region>();
          //}
          //else
          //{
          //  regions = new List<Structures.Region>();
          //  regions.Add(new Structures.Region() { RegionId = person.Branch.Region.RegionId, Description = person.Branch.Region.Description });
          //}

          regions = new List<Region>();
          new XPQuery<Atlas.Domain.Model.Region>(uow).Select(r => new { r.RegionId, r.Description }).ToList().ForEach(r =>
          {
            regions.Add(new Region { RegionId = r.RegionId, Description = r.Description });
          });
        }

        RedisConnection.SetStringFromObject(string.Format(REDIS_KEY_PERSON_REGION, personId), regions, new TimeSpan(12, 0, 0));
      }
      return regions;
    }

    public static List<Branch> GetRegionBranches(long regionId)
    {
      var branches = RedisConnection.GetObjectFromString<List<Branch>>(string.Format(REDIS_KEY_REGION_BRANCH, regionId));
      if (branches == null)
      {
        branches = new List<Branch>();
        using (var uow = new UnitOfWork())
        {
          var branch = new XPQuery<BRN_Branch>(uow).Where(b => b.Region.RegionId == regionId).ToList();
          branch.ForEach(b =>
          {
            branches.Add(new Branch { BranchId = b.BranchId, Name = b.Company.Name, RegionId = b.Region.RegionId, Region = b.Region.Description });
          });
        }

        RedisConnection.SetStringFromObject(string.Format(REDIS_KEY_REGION_BRANCH, regionId), branches, new TimeSpan(1, 0, 0));
      }
      return branches;
    }

    public static List<RegionBranch> GetPersonRegionBranches(long personId)
    {
      //var allowedRegions = new long[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };

      var regionBranches = RedisConnection.GetObjectFromString<List<RegionBranch>>(string.Format(REDIS_KEY_PERSON_REGION_BRANCH, personId));
      regionBranches = null;
      if (regionBranches == null)
      {
        regionBranches = new List<RegionBranch>
        {
          new RegionBranch
          {
            Name = "All Branches",
            MultiSelectGroup = true
          }
        };

        using (var uow = new UnitOfWork())
        {
          new XPQuery<Atlas.Domain.Model.Region>(uow).Select(r => new { r.RegionId, r.Description }).OrderBy(r => r.RegionId).ToList().ForEach(r =>
          {
            regionBranches.Add(new RegionBranch
            {
              Name = r.Description,
              MultiSelectGroup = true
            });

            new XPQuery<BRN_Branch>(uow).Where(b => b.Region.RegionId == r.RegionId && b.BranchId > 0)
              .ToList()
              .ForEach(b =>
              {
                regionBranches.Add(new RegionBranch
                {
                  BranchId = b.BranchId,
                  Name = b.Company.Name,
                  Ticked = true
                });
              });

            regionBranches.Add(new RegionBranch
            {
              MultiSelectGroup = false
            });
          });
        }

        regionBranches.Add(new RegionBranch
        {
          MultiSelectGroup = false
        });

        RedisConnection.SetStringFromObject(string.Format(REDIS_KEY_PERSON_REGION_BRANCH, personId), regionBranches, new TimeSpan(1, 0, 0));
      }
      return regionBranches;
    }

    internal MainSummary GetMainSummary(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      var mainSummary = new MainSummary
      {
        Cheque = 0,
        ChequeCount = 0,
        Collections = 0,
        Handovers = 0,
        HandoversCount = 0
      };

      foreach (var branch in GetLegacyBranchNumbers(branchIds))
      {
        var result = RedisConnection.GetObjectFromString<List<IBasicLoan>>(string.Format(REDIS_KEY_BASIC_LOAN, branch.Key, startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")));
        if (result != null)
        {
          mainSummary.Cheque += result.Sum(b => b.Cheque);
          mainSummary.ChequeCount += result.Sum(b => b.Quantity);
        }
      }

      return mainSummary;
    }

    internal List<Cheque> GetCheque(List<long> branchIds, DateTime startDate, DateTime endDate, ILogger log)
    {
      var cheque = new List<Cheque>();
      try
      {
        foreach (var branch in GetLegacyBranchNumbers(branchIds))
        {
          var chequeItem = new Cheque
          {
            Branch = branch.Key,
            BranchName = branch.Value,
            PayNo1 = new decimal(0.00),
            PayNo2 = new decimal(0),
            PayNo3 = new decimal(0),
            PayNo4 = new decimal(0),
            PayNo5 = new decimal(0),
            PayNo6 = new decimal(0),
            PayNo12 = new decimal(0),
            PayNo24 = new decimal(0),
            PayNoTot = new decimal(0)
          };
          var result = RedisConnection.GetObjectFromString<List<IBasicLoan>>(string.Format(REDIS_KEY_BASIC_LOAN, branch.Key, startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")));
          if (result != null)
          {
            var paynos = result.Select(b => b.PayNo).Distinct();
            foreach (var payno in paynos)
            {
              switch (payno)
              {
                case 1:
                  chequeItem.PayNo1 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque);
                  break;
                case 2:
                  chequeItem.PayNo2 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque);
                  break;
                case 3:
                  chequeItem.PayNo3 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque);
                  break;
                case 4:
                  chequeItem.PayNo4 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque);
                  break;
                case 5:
                  chequeItem.PayNo5 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque);
                  break;
                case 6:
                  chequeItem.PayNo6 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque);
                  break;
                case 12:
                  chequeItem.PayNo12 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque);
                  break;
                case 24:
                  chequeItem.PayNo24 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque);
                  break;
              }
            }

            chequeItem.PayNoTot = result.Sum(b => b.Cheque);
          }

          cheque.Add(chequeItem);
        }
      }
      catch (Exception ex)
      {
        log.Error(string.Format("CI REPORT GET CHEQUE: {0} - {1}", ex.Message, ex.StackTrace));
      }
      return cheque.OrderBy(b => b.PayNoTot).ToList();
    }

    internal List<Insurance> GetInsurance(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      var cheque = new List<Insurance>();

      foreach (var branch in GetLegacyBranchNumbers(branchIds))
      {
        var chequeItem = new Insurance
        {
          Branch = branch.Key,
          BranchName = branch.Value,
          PayNo1 = new decimal(0),
          PayNo2 = new decimal(0),
          PayNo3 = new decimal(0),
          PayNo4 = new decimal(0),
          PayNo5 = new decimal(0),
          PayNo6 = new decimal(0),
          PayNo12 = new decimal(0),
          PayNo24 = new decimal(0),
          PayNoTot = new decimal(0),
        };
        var result = RedisConnection.GetObjectFromString<List<IBasicLoan>>(string.Format(REDIS_KEY_BASIC_LOAN, branch.Key, startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")));
        if (result != null)
        {
          var paynos = result.Select(b => b.PayNo).Distinct();
          foreach (var payno in paynos)
          {
            switch (payno)
            {
              case 1:
                chequeItem.PayNo1 = result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife);
                break;
              case 2:
                chequeItem.PayNo2 = result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife);
                break;
              case 3:
                chequeItem.PayNo3 = result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife);
                break;
              case 4:
                chequeItem.PayNo4 = result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife);
                break;
              case 5:
                chequeItem.PayNo5 = result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife);
                break;
              case 6:
                chequeItem.PayNo6 = result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife);
                break;
              case 12:
                chequeItem.PayNo12 = result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife);
                break;
              case 24:
                chequeItem.PayNo24 = result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife);
                break;
            }
          }

          chequeItem.PayNoChequeTot = result.Sum(b => b.Cheque);
          chequeItem.PayNoTot = result.Sum(b => b.CreditLife);
        }

        cheque.Add(chequeItem);
      }

      return cheque.OrderBy(b => b.PayNoChequeTot).ToList();
    }

    internal List<Interest> GetInterest(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      var cheque = new List<Interest>();

      foreach (var branch in GetLegacyBranchNumbers(branchIds))
      {
        var chequeItem = new Interest
        {
          Branch = branch.Key,
          BranchName = branch.Value,
          PayNo1 = new decimal(0),
          PayNo2 = new decimal(0),
          PayNo3 = new decimal(0),
          PayNo4 = new decimal(0),
          PayNo5 = new decimal(0),
          PayNo6 = new decimal(0),
          PayNo12 = new decimal(0),
          PayNo24 = new decimal(0),
          PayNoTot = new decimal(0)
        };
        var result = RedisConnection.GetObjectFromString<List<IBasicLoan>>(string.Format(REDIS_KEY_BASIC_LOAN, branch.Key, startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")));
        if (result != null)
        {
          var paynos = result.Select(b => b.PayNo).Distinct();
          foreach (var payno in paynos)
          {
            switch (payno)
            {
              case 1:
                chequeItem.PayNo1 = result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges);
                break;
              case 2:
                chequeItem.PayNo2 = result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges);
                break;
              case 3:
                chequeItem.PayNo3 = result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges);
                break;
              case 4:
                chequeItem.PayNo4 = result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges);
                break;
              case 5:
                chequeItem.PayNo5 = result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges);
                break;
              case 6:
                chequeItem.PayNo6 = result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges);
                break;
              case 12:
                chequeItem.PayNo12 = result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges);
                break;
              case 24:
                chequeItem.PayNo24 = result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges);
                break;
            }
          }

          chequeItem.PayNoChequeTot = result.Sum(b => b.Cheque);
          chequeItem.PayNoTot = result.Sum(b => b.TotalCharges);
        }

        cheque.Add(chequeItem);
      }

      return cheque.OrderBy(b => b.PayNoChequeTot).ToList();
    }

    internal List<LoanMix> GetLoanMix(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      var loanMix = new List<LoanMix>();

      foreach (var branch in GetLegacyBranchNumbers(branchIds))
      {
        var loanItem = new LoanMix
        {
          Branch = branch.Key,
          BranchName = branch.Value,
          PayNoChequeTot = new decimal(0),
          PayNoLoan1 = new decimal(0),
          PayNoLoan2 = new decimal(0),
          PayNoLoan3 = new decimal(0),
          PayNoLoan4 = new decimal(0),
          PayNoLoan5 = new decimal(0),
          PayNoLoan6 = new decimal(0),
          PayNoLoan12 = new decimal(0),
          PayNoLoan24 = new decimal(0),
          PayNoLoanTot = new decimal(0),
          PayNoLoanMix1 = 0,
          PayNoLoanMix2 = 0,
          PayNoLoanMix3 = 0,
          PayNoLoanMix4 = 0,
          PayNoLoanMix5 = 0,
          PayNoLoanMix6 = 0,
          PayNoLoanMix12 = 0,
          PayNoLoanMix24 = 0,
          PayNoLoanMixNot1 = 0
        };
        var result = RedisConnection.GetObjectFromString<List<IClientLoanInfo>>(string.Format(REDIS_KEY_CLIENT_LOAN, branch.Key, startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")));
        if (result != null)
        {
          var paynos = result.Select(b => b.PayNo).Distinct();
          foreach (var payno in paynos)
          {
            loanItem.PayNoLoanTot = result.Sum(b => b.Quantity);
            switch (payno)
            {
              case 1:
                loanItem.PayNoLoan1 = result.Where(b => b.PayNo == payno).Sum(b => b.Quantity);
                loanItem.PayNoLoanMix1 = (float)(loanItem.PayNoLoan1 / (loanItem.PayNoLoanTot * 100));
                break;
              case 2:
                loanItem.PayNoLoan2 = result.Where(b => b.PayNo == payno).Sum(b => b.Quantity);
                loanItem.PayNoLoanMix2 = (float)(loanItem.PayNoLoan2 / (loanItem.PayNoLoanTot * 100));
                loanItem.PayNoLoanMixNot1 += loanItem.PayNoLoanMix2;
                break;
              case 3:
                loanItem.PayNoLoan3 = result.Where(b => b.PayNo == payno).Sum(b => b.Quantity);
                loanItem.PayNoLoanMix3 = (float)(loanItem.PayNoLoan3 / (loanItem.PayNoLoanTot * 100));
                loanItem.PayNoLoanMixNot1 += loanItem.PayNoLoanMix3;
                break;
              case 4:
                loanItem.PayNoLoan4 = result.Where(b => b.PayNo == payno).Sum(b => b.Quantity);
                loanItem.PayNoLoanMix4 = (float)(loanItem.PayNoLoan4 / (loanItem.PayNoLoanTot * 100));
                loanItem.PayNoLoanMixNot1 += loanItem.PayNoLoanMix4;
                break;
              case 5:
                loanItem.PayNoLoan5 = result.Where(b => b.PayNo == payno).Sum(b => b.Quantity);
                loanItem.PayNoLoanMix5 = (float)(loanItem.PayNoLoan5 / (loanItem.PayNoLoanTot * 100));
                loanItem.PayNoLoanMixNot1 += loanItem.PayNoLoanMix5;
                break;
              case 6:
                loanItem.PayNoLoan6 = result.Where(b => b.PayNo == payno).Sum(b => b.Quantity);
                loanItem.PayNoLoanMix6 = (float)(loanItem.PayNoLoan6 / (loanItem.PayNoLoanTot * 100));
                loanItem.PayNoLoanMixNot1 += loanItem.PayNoLoanMix6;
                break;
              case 12:
                loanItem.PayNoLoan12 = result.Where(b => b.PayNo == payno).Sum(b => b.Quantity);
                loanItem.PayNoLoanMix12 = (float)(loanItem.PayNoLoan12 / (loanItem.PayNoLoanTot * 100));
                loanItem.PayNoLoanMixNot1 += loanItem.PayNoLoanMix12;
                break;
              case 24:
                loanItem.PayNoLoan24 = result.Where(b => b.PayNo == payno).Sum(b => b.Quantity);
                loanItem.PayNoLoanMix24 = (float)(loanItem.PayNoLoan24 / (loanItem.PayNoLoanTot * 100));
                loanItem.PayNoLoanMixNot1 += loanItem.PayNoLoanMix24;
                break;
            }
          }

          loanItem.PayNoChequeTot = result.Sum(b => b.Cheque);
        }

        loanMix.Add(loanItem);
      }

      return loanMix.OrderBy(b => b.PayNoChequeTot).ToList();
    }

    internal List<InsurancePercentiles> GetInsurancePercentiles(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      var cheque = new List<InsurancePercentiles>();

      foreach (var branch in GetLegacyBranchNumbers(branchIds))
      {
        var chequeItem = new InsurancePercentiles
        {
          Branch = branch.Key,
          BranchName = branch.Value,
          PayNoInsurance1 = 0,
          PayNoInsurance2 = 0,
          PayNoInsurance3 = 0,
          PayNoInsurance4 = 0,
          PayNoInsurance5 = 0,
          PayNoInsurance6 = 0,
          PayNoInsurance12 = 0,
          PayNoInsurance24 = 0,
          PayNoInsuranceTot = 0
        };
        var result = RedisConnection.GetObjectFromString<List<IBasicLoan>>(string.Format(REDIS_KEY_BASIC_LOAN, branch.Key, startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")));
        if (result != null)
        {
          var paynos = result.Select(b => b.PayNo).Distinct();
          foreach (var payno in paynos)
          {
            var chequeValue = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque);
            if (chequeValue > 0)
            {
              switch (payno)
              {
                case 1:
                  chequeItem.PayNoInsurance1 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife) / chequeValue);
                  break;
                case 2:
                  chequeItem.PayNoInsurance2 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife) / chequeValue);
                  break;
                case 3:
                  chequeItem.PayNoInsurance3 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife) / chequeValue);
                  break;
                case 4:
                  chequeItem.PayNoInsurance4 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife) / chequeValue);
                  break;
                case 5:
                  chequeItem.PayNoInsurance5 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife) / chequeValue);
                  break;
                case 6:
                  chequeItem.PayNoInsurance6 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife) / chequeValue);
                  break;
                case 12:
                  chequeItem.PayNoInsurance12 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife) / chequeValue);
                  break;
                case 24:
                  chequeItem.PayNoInsurance24 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.CreditLife) / chequeValue);
                  break;
              }
            }
          }

          chequeItem.PayNoChequeTot = result.Sum(b => b.Cheque);
          if (result.Sum(b => b.Cheque) > 0)
            chequeItem.PayNoInsuranceTot = (float)(result.Sum(b => b.CreditLife) / result.Sum(b => b.Cheque));
        }

        cheque.Add(chequeItem);
      }

      return cheque.OrderBy(b => b.PayNoChequeTot).ToList();
    }

    internal List<InterestPercentiles> GetInterestPercentiles(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      var cheque = new List<InterestPercentiles>();

      foreach (var branch in GetLegacyBranchNumbers(branchIds))
      {
        var chequeItem = new InterestPercentiles
        {
          Branch = branch.Key,
          BranchName = branch.Value,
          PayNoInterest1 = 0,
          PayNoInterest2 = 0,
          PayNoInterest3 = 0,
          PayNoInterest4 = 0,
          PayNoInterest5 = 0,
          PayNoInterest6 = 0,
          PayNoInterest12 = 0,
          PayNoInterest24 = 0,
          PayNoInterestTot = 0
        };
        var result = RedisConnection.GetObjectFromString<List<IBasicLoan>>(string.Format(REDIS_KEY_BASIC_LOAN, branch.Key, startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")));
        if (result != null)
        {
          var paynos = result.Select(b => b.PayNo).Distinct();
          foreach (var payno in paynos)
          {
            var chequeSum = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque);
            if (chequeSum > 0)
            {
              switch (payno)
              {
                case 1:
                  chequeItem.PayNoInterest1 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges) / chequeSum);
                  break;
                case 2:
                  chequeItem.PayNoInterest2 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges) / chequeSum);
                  break;
                case 3:
                  chequeItem.PayNoInterest3 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges) / chequeSum);
                  break;
                case 4:
                  chequeItem.PayNoInterest4 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges) / chequeSum);
                  break;
                case 5:
                  chequeItem.PayNoInterest5 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges) / chequeSum);
                  break;
                case 6:
                  chequeItem.PayNoInterest6 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges) / chequeSum);
                  break;
                case 12:
                  chequeItem.PayNoInterest12 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges) / chequeSum);
                  break;
                case 24:
                  chequeItem.PayNoInterest24 = (float)(result.Where(b => b.PayNo == payno).Sum(b => b.TotalCharges) / chequeSum);
                  break;
              }
            }
          }

          chequeItem.PayNoChequeTot = result.Sum(b => b.Cheque);
          if (result.Sum(b => b.Cheque) > 0)
            chequeItem.PayNoInterestTot = (float)(result.Sum(b => b.TotalCharges) / result.Sum(b => b.Cheque));
        }

        cheque.Add(chequeItem);
      }

      return cheque.OrderBy(b => b.PayNoChequeTot).ToList();
    }

    internal List<AverageLoan> GetAverageLoanSize(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      var cheque = new List<AverageLoan>();

      foreach (var branch in GetLegacyBranchNumbers(branchIds))
      {
        var chequeItem = new AverageLoan
        {
          Branch = branch.Key,
          BranchName = branch.Value,
          PayNo1 = 0,
          PayNo2 = 0,
          PayNo3 = 0,
          PayNo4 = 0,
          PayNo5 = 0,
          PayNo6 = 0,
          PayNo12 = 0,
          PayNo24 = 0,
          PayNoTot = 0
        };
        var result = RedisConnection.GetObjectFromString<List<IBasicLoan>>(string.Format(REDIS_KEY_BASIC_LOAN, branch.Key, startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")));
        if (result != null)
        {
          var paynos = result.Select(b => b.PayNo).Distinct();
          foreach (var payno in paynos)
          {
            var quantity = result.Where(b => b.PayNo == payno).Sum(b => b.Quantity);
            if (quantity > 0)
            {
              switch (payno)
              {
                case 1:
                  chequeItem.PayNo1 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque) / quantity;
                  break;
                case 2:
                  chequeItem.PayNo2 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque) / quantity;
                  break;
                case 3:
                  chequeItem.PayNo3 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque) / quantity;
                  break;
                case 4:
                  chequeItem.PayNo4 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque) / quantity;
                  break;
                case 5:
                  chequeItem.PayNo5 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque) / quantity;
                  break;
                case 6:
                  chequeItem.PayNo6 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque) / quantity;
                  break;
                case 12:
                  chequeItem.PayNo12 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque) / quantity;
                  break;
                case 24:
                  chequeItem.PayNo24 = result.Where(b => b.PayNo == payno).Sum(b => b.Cheque) / quantity;
                  break;
              }
            }
          }

          chequeItem.PayNoChequeTot = result.Sum(b => b.Cheque);
          if (result.Sum(b => b.Quantity) > 0)
            chequeItem.PayNoTot = result.Sum(b => b.Cheque) / result.Sum(b => b.Quantity);
        }

        cheque.Add(chequeItem);
      }

      return cheque.OrderBy(b => b.PayNoChequeTot).ToList();
    }

    internal List<AverageNewCientLoan> GetAverageNewClientLoanSize(List<long> branchIds, DateTime startDate, DateTime endDate)
    {
      var cheque = new List<AverageNewCientLoan>();

      foreach (var branch in GetLegacyBranchNumbers(branchIds))
      {
        var chequeItem = new AverageNewCientLoan
        {
          Branch = branch.Key,
          BranchName = branch.Value,
          PayNo1 = 0,
          PayNo2 = 0,
          PayNo3 = 0,
          PayNo4 = 0,
          PayNo5 = 0,
          PayNo6 = 0,
          PayNo12 = 0,
          PayNo24 = 0,
          PayNoTot = 0
        };
        var result = RedisConnection.GetObjectFromString<List<IClientLoanInfo>>(string.Format(REDIS_KEY_BASIC_LOAN, branch.Key, startDate.ToString("ddMMyyyy"), endDate.ToString("ddMMyyyy")));
        if (result != null)
        {
          var paynos = result.Select(b => b.PayNo).Distinct();
          foreach (var payno in paynos)
          {
            var quantity = result.Where(b => b.PayNo == payno).Sum(b => b.NewClientQuantity);
            if (quantity > 0)
            {
              switch (payno)
              {
                case 1:
                  chequeItem.PayNo1 = result.Where(b => b.PayNo == payno).Sum(b => b.NewClientAmount) / quantity;
                  break;
                case 2:
                  chequeItem.PayNo2 = result.Where(b => b.PayNo == payno).Sum(b => b.NewClientAmount) / quantity;
                  break;
                case 3:
                  chequeItem.PayNo3 = result.Where(b => b.PayNo == payno).Sum(b => b.NewClientAmount) / quantity;
                  break;
                case 4:
                  chequeItem.PayNo4 = result.Where(b => b.PayNo == payno).Sum(b => b.NewClientAmount) / quantity;
                  break;
                case 5:
                  chequeItem.PayNo5 = result.Where(b => b.PayNo == payno).Sum(b => b.NewClientAmount) / quantity;
                  break;
                case 6:
                  chequeItem.PayNo6 = result.Where(b => b.PayNo == payno).Sum(b => b.NewClientAmount) / quantity;
                  break;
                case 12:
                  chequeItem.PayNo12 = result.Where(b => b.PayNo == payno).Sum(b => b.NewClientAmount) / quantity;
                  break;
                case 24:
                  chequeItem.PayNo24 = result.Where(b => b.PayNo == payno).Sum(b => b.NewClientAmount) / quantity;
                  break;
              }
            }
          }

          chequeItem.PayNoChequeTot = result.Sum(b => b.Cheque);
          if (result.Sum(b => b.NewClientQuantity) > 0)
            chequeItem.PayNoTot = result.Sum(b => b.NewClientAmount) / result.Sum(b => b.NewClientQuantity);
        }

        cheque.Add(chequeItem);
      }

      return cheque.OrderBy(b => b.PayNoChequeTot).ToList();
    }

    private Dictionary<string, string> GetLegacyBranchNumbers(List<long> branchIds)
    {
      using (var uow = new UnitOfWork())
      {
        return new XPQuery<BRN_Branch>(uow).Where(b => branchIds.Contains(b.BranchId)).Select(b => new { b.LegacyBranchNum, b.Company.Name }).ToDictionary(b => b.LegacyBranchNum, b => b.Name);
      }
    }
  }
}