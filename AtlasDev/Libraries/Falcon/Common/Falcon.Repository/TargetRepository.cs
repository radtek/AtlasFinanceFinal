using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Common.Utils;
using Atlas.Domain.Model;
using Atlas.Domain.Model.Target;
using DevExpress.Xpo;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Structures.Target;
using Falcon.Common.Structures.Target;

namespace Falcon.Common.Repository
{
  public class TargetRepository : ITargetRepository
  {
    public void AddBranchCiMonthly(long branchId, long hostId, DateTime targetDate, decimal amount, float percent,
      long userId)
    {
      using (var uow = new UnitOfWork())
      {
        var monthEndDate = DateUtils.GetMonthEndDate(targetDate);
        var existingTarget = new XPQuery<TAR_BranchCIMonthly>(uow).FirstOrDefault(t => t.Branch.BranchId == branchId &&
                                                                                       t.Host.HostId == hostId &&
                                                                                       t.TargetDate.Date == monthEndDate) ??
                             new TAR_BranchCIMonthly(uow)
                             {
                               Branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(b => b.BranchId == branchId),
                               Host = new XPQuery<Host>(uow).FirstOrDefault(h => h.HostId == hostId),
                               TargetDate = monthEndDate
                             };

        existingTarget.Amount = amount;
        existingTarget.Percent = percent;
        existingTarget.CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == userId);

        uow.CommitChanges();
      }
    }

    public void AddDailySale(long branchId, long hostId, DateTime targetDate, decimal amount, float percent, long userId)
    {
      using (var uow = new UnitOfWork())
      {
        //var existingTarget = new XPQuery<TAR_DailySale>(uow).FirstOrDefault(t => t.Branch.BranchId == branchId &&
        //                                                                         t.Host.HostId == hostId &&
        //                                                                         t.TargetDate.Date == targetDate);
        var existingTarget = new XPQuery<TAR_DailySale>(uow).FirstOrDefault(t => t.TargetDate.Date == targetDate && !t.DisableDate.HasValue);

        if (existingTarget != null)
        {
          existingTarget.DisableDate = DateTime.Now;
        }

        if (amount > 0 || percent > 0)
        {
          new TAR_DailySale(uow)
          {
            Branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(b => b.BranchId == branchId),
            Host = new XPQuery<Host>(uow).FirstOrDefault(h => h.HostId == hostId),
            TargetDate = targetDate,
            Amount = amount,
            Percent = percent,
            CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == userId)
          };
        }
        uow.CommitChanges();
      }
    }

    public void AddHandoverTarget(long branchId, long hostId, DateTime targetDate, decimal handoverBudget,
      float arrearTarget, long userId)
    {
      using (var uow = new UnitOfWork())
      {
        var existingTarget = new XPQuery<TAR_HandoverTarget>(uow).FirstOrDefault(t => t.Branch.BranchId == branchId &&
                                                                                      t.Host.HostId == hostId &&
                                                                                      t.StartRange.Month ==
                                                                                      targetDate.Month &&
                                                                                      t.StartRange.Year ==
                                                                                      targetDate.Year);

        if (existingTarget != null)
        {
          existingTarget.DisableDate = DateTime.Now;
        }

        new TAR_HandoverTarget(uow)
        {
          Branch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(b => b.BranchId == branchId),
          Host = new XPQuery<Host>(uow).FirstOrDefault(h => h.HostId == hostId),
          StartRange = DateUtils.GetMonthStartDate(targetDate),
          EndRange = DateUtils.GetMonthEndDate(targetDate),
          ActiveDate = DateUtils.GetMonthStartDate(targetDate),
          HandoverBudget = handoverBudget,
          ArrearTarget = arrearTarget,
          CreateUser = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == userId)
        };

        uow.CommitChanges();
      }
    }

    public void AddLoanMix(DateTime targetDate, int payNo, float percent)
    {
      using (var uow = new UnitOfWork())
      {
        var monthEndDate = DateUtils.GetMonthEndDate(targetDate);
        var existingTarget = new XPQuery<TAR_LoanMix>(uow).FirstOrDefault(t => t.PayNo == payNo &&
                                                                               t.TargetDate.Date == monthEndDate) ??
                             new TAR_LoanMix(uow)
                             {
                               PayNo = payNo,
                               TargetDate = monthEndDate
                             };

        existingTarget.Percent = percent;

        uow.CommitChanges();
      }
    }

    public ICollection<IDailySaleModel> GetDailySales(long branchId, long hostId, DateTime targetMonth)
    {
      using (var uow = new UnitOfWork())
      {
        var dailySalesQuery =
          new XPQuery<TAR_DailySale>(uow).Where(
            t =>
              t.TargetDate.Month == targetMonth.Month && t.TargetDate.Year == targetMonth.Year && t.DisableDate == null)
            .AsQueryable();

        if (branchId > 0)
          dailySalesQuery = dailySalesQuery.Where(t => t.Branch.BranchId == branchId).AsQueryable();

        if (hostId > 0)
          dailySalesQuery = dailySalesQuery.Where(t => t.Host.HostId == hostId).AsQueryable();

        var dailySales = dailySalesQuery.OrderBy(d => d.TargetDate).ToList();

        return dailySales.Select(dailySale => new DailySaleModel
        {
          BranchId = dailySale.Branch == null ? 0 : dailySale.Branch.BranchId,
          HostId = dailySale.Host == null ? 0 : dailySale.Host.HostId,
          TargetDate = dailySale.TargetDate,
          Percent = dailySale.Percent,
          UserId = dailySale.CreateUser == null ? 0 : dailySale.CreateUser.PersonId,
          Amount = dailySale.Amount,
          DailySaleId = dailySale.DailySaleId
        }).Cast<IDailySaleModel>().ToList();
      }
    }

    public ICollection<ILoanMixModel> GetLoanMixes(DateTime targetMonth)
    {
      using (var uow = new UnitOfWork())
      {
        var loanMixes =
          new XPQuery<TAR_LoanMix>(uow).Where(
            t =>
              t.TargetDate.Month == targetMonth.Month && t.TargetDate.Year == targetMonth.Year)
            .OrderBy(t => t.TargetDate).ToList();

        return loanMixes.Select(loanMix => new LoanMixModel
        {
          LoanMixId = loanMix.LoanMixId,
          TargetDate = loanMix.TargetDate,
          PayNo = loanMix.PayNo,
          Percent = loanMix.Percent,
        }).Cast<ILoanMixModel>().ToList();
      }
    }

    public ICollection<IBranchCiMonthly> GetBranchCiMonthlys(long branchId, long hostId, DateTime targetMonth)
    {
      using (var uow = new UnitOfWork())
      {
        var branchCiMonthlysQuery =
          new XPQuery<TAR_BranchCIMonthly>(uow).Where(
            t =>
              t.TargetDate.Month == targetMonth.Month && t.TargetDate.Year == targetMonth.Year &&
              t.Host.HostId == hostId).AsQueryable();

        if (branchId > 0)
          branchCiMonthlysQuery = branchCiMonthlysQuery.Where(t => t.Branch.BranchId == branchId).AsQueryable();

        var branchCiMonthlys = branchCiMonthlysQuery.OrderBy(t => t.Branch.Company.Name).ToList();

        return branchCiMonthlys.Select(t => new BranchCiMonthly
        {
          BranchId = t.Branch.BranchId,
          BranchDescription = string.Format("{0} ({1})", t.Branch.Company.Name, t.Branch.LegacyBranchNum),
          HostId = t.Host.HostId,
          HostDescription = t.Host.Description,
          TargetDate = t.TargetDate,
          Percent = t.Percent,
          Amount = t.Amount,
          CreateUser =
            t.CreateUser != null && t.CreateUser.Security != null ? t.CreateUser.Security.Username : string.Empty
        }).Cast<IBranchCiMonthly>().ToList();
      }
    }

    public ICollection<IHandoverTarget> GetHandoverTargets(long branchId, long hostId, DateTime targetMonth)
    {
      using (var uow = new UnitOfWork())
      {
        var handoverTargetsQuery =
          new XPQuery<TAR_HandoverTarget>(uow).Where(
            t =>
              t.StartRange.Month == targetMonth.Month && t.StartRange.Year == targetMonth.Year &&
              !t.DisableDate.HasValue && t.Host.HostId == hostId).AsQueryable();

        if (branchId > 0)
          handoverTargetsQuery = handoverTargetsQuery.Where(t => t.Branch.BranchId == branchId);

        var handoverTargets = handoverTargetsQuery.ToList();

        return handoverTargets.Select(t => new HandoverTarget
        {
          BranchId = t.Branch.BranchId,
          BranchDescription = t.Branch.Company.Name,
          HostId = t.Host.HostId,
          Host = t.Host.Description,
          TargetDate = t.EndRange,
          HandoverBudget = t.HandoverBudget,
          ArrearTarget = t.ArrearTarget
        }).Cast<IHandoverTarget>().ToList();
      }
    }
  }
}