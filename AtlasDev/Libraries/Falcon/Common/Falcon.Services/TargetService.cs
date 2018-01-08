using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Falcon.Common.Interfaces.Structures.Target;
using Falcon.Common.Structures.Target;

namespace Falcon.Common.Services
{
  public class TargetService : ITargetService
  {
    private readonly ITargetRepository _targetRepository;
    private readonly ICompanyRepository _companyRepository;

    private readonly int[] _payNos = {1, 2, 3, 4, 5, 6, 12, 24, 0};

    public TargetService(ITargetRepository targetRepository, ICompanyRepository companyRepository)
    {
      _targetRepository = targetRepository;
      _companyRepository = companyRepository;
    }

    public void AddBranchCiMonthly(long branchId, long hostId, DateTime targetDate, decimal amount, float percent,
      long userId)
    {
      _targetRepository.AddBranchCiMonthly(branchId, hostId, targetDate, amount, percent, userId);
    }

    public void AddDailySale(long branchId, long hostId, DateTime targetDate, decimal amount, float percent, long userId)
    {
      _targetRepository.AddDailySale(branchId, hostId, targetDate, amount, percent, userId);
    }

    public void AddHandoverTarget(long branchId, long hostId, DateTime targetDate, decimal handoverBudget,
      float arrearTarget, long userId)
    {
      _targetRepository.AddHandoverTarget(branchId, hostId, targetDate, handoverBudget, arrearTarget, userId);
    }

    public void AddLoanMix(DateTime targetDate, int payNo, float percent)
    {
      _targetRepository.AddLoanMix(targetDate, payNo, percent);
    }

    public ICollection<IDailySaleModel> GetDailySales(DateTime targetMonth)
    {
      var targets = _targetRepository.GetDailySales(0, 0, targetMonth);

      var targetDate = new DateTime(targetMonth.Year, targetMonth.Month, 1);
      while (targetDate.Month == targetMonth.Month)
      {
        var target = targets.FirstOrDefault(t => t.TargetDate.Date == targetDate.Date);
        if (target == null)
        {
          target = new DailySaleModel
          {
            BranchId = 0,
            TargetDate = targetDate,
            HostId = 0,
            Amount = 0,
            Percent = 0,
            DailySaleId = 0,
            UserId = 0
          };

          targets.Add(target);
        }

        targetDate = targetDate.AddDays(1);
      }

      return targets.OrderBy(t => t.TargetDate).ToList();
    }

    public ICollection<ILoanMixModel> GetLoanMixes(DateTime targetMonth)
    {
      var loanMixes = _targetRepository.GetLoanMixes(targetMonth);

      var targetDate = (new DateTime(targetMonth.Year, targetMonth.Month, 1)).AddMonths(1).AddDays(-1);
      // last day of month
      foreach (var payno in _payNos)
      {
        var loanMix = loanMixes.FirstOrDefault(t => t.PayNo == payno);
        if (loanMix == null)
        {
          loanMix = new LoanMixModel
          {
            TargetDate = targetDate,
            PayNo = payno,
            Percent = 0,
            LoanMixId = 0
          };

          loanMixes.Add(loanMix);
        }
      }

      return loanMixes.OrderBy(t => (t.PayNo == 0 ? 99 : t.PayNo)).ToList();
    }

    public ICollection<IBranchCiMonthly> GetBranchCiMonthlys(long branchId, long hostId, DateTime targetMonth)
    {
      var branchCiMonthlys = _targetRepository.GetBranchCiMonthlys(branchId, hostId, targetMonth);

      var targetDate = (new DateTime(targetMonth.Year, targetMonth.Month, 1)).AddMonths(1).AddDays(-1);

      var host = _companyRepository.GetAllHosts((General.Host) hostId).FirstOrDefault();

      if (branchId == 0)
      {
        var activeBranches = _companyRepository.GetActiveBranches();
        var branchesWithoutTargets =
          activeBranches.Where(c => !branchCiMonthlys.Select(t => t.BranchId).Contains(c.BranchId)).ToList();
        foreach (var branchWithoutTarget in branchesWithoutTargets)
        {
          branchCiMonthlys.Add(new BranchCiMonthly
          {
            BranchId = branchWithoutTarget.BranchId,
            BranchDescription =
              string.Format("{0} ({1})", branchWithoutTarget.Name, branchWithoutTarget.LegacyBranchNum),
            HostId = hostId,
            HostDescription = host == null ? string.Empty : host.HostName,
            Percent = 0,
            TargetDate = targetDate,
            Amount = 0,
            CreateUser = string.Empty
          });
        }
      }
      else if (branchCiMonthlys.Count == 0)
      {
        var branch = _companyRepository.GetBranchesByIds(new List<long> {branchId}).FirstOrDefault();
        branchCiMonthlys.Add(new BranchCiMonthly
        {
          BranchId = branchId,
          BranchDescription =
            branch == null ? string.Empty : string.Format("{0} ({1})", branch.Name, branch.LegacyBranchNum),
          HostId = hostId,
          HostDescription = host == null ? string.Empty : host.HostName,
          Percent = 0,
          TargetDate = targetDate,
          Amount = 0,
          CreateUser = string.Empty
        });
      }

      return branchCiMonthlys.OrderBy(t => t.BranchDescription).ToList();
    }

    public ICollection<IHandoverTarget> GetHandoverTargets(long branchId, long hostId, DateTime targetMonth)
    {
      var handoverTargets = _targetRepository.GetHandoverTargets(branchId, hostId, targetMonth);

      var targetDate = (new DateTime(targetMonth.Year, targetMonth.Month, 1)).AddMonths(1).AddDays(-1);

      var host = _companyRepository.GetAllHosts((General.Host) hostId).FirstOrDefault();

      if (branchId == 0)
      {
        var activeBranches = _companyRepository.GetActiveBranches();
        var branchesWithoutTargets =
          activeBranches.Where(c => !handoverTargets.Select(t => t.BranchId).Contains(c.BranchId)).ToList();
        foreach (var branchWithoutTarget in branchesWithoutTargets)
        {
          handoverTargets.Add(new HandoverTarget
          {
            BranchId = branchWithoutTarget.BranchId,
            BranchDescription =
              string.Format("{0} ({1})", branchWithoutTarget.Name, branchWithoutTarget.LegacyBranchNum),
            HostId = hostId,
            Host = host == null ? string.Empty : host.HostName,
            TargetDate = targetDate,
            HandoverBudget = 0,
            ArrearTarget = 0
          });
        }
      }
      else if (handoverTargets.Count == 0)
      {
        var branch = _companyRepository.GetBranchesByIds(new List<long> {branchId}).FirstOrDefault();
        handoverTargets.Add(new HandoverTarget
        {
          BranchId = branchId,
          BranchDescription =
            branch == null ? string.Empty : string.Format("{0} ({1})", branch.Name, branch.LegacyBranchNum),
          HostId = hostId,
          Host = host == null ? string.Empty : host.HostName,
          TargetDate = targetDate,
          HandoverBudget = 0,
          ArrearTarget = 0
        });
      }

      return handoverTargets.OrderBy(t => t.BranchDescription).ToList();
    }
  }
}