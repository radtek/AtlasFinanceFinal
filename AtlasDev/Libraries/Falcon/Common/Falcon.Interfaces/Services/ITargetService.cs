using System;
using System.Collections.Generic;
using Falcon.Common.Interfaces.Structures.Target;

namespace Falcon.Common.Interfaces.Services
{
  public interface ITargetService
  {
    void AddBranchCiMonthly(long branchId, long hostId, DateTime targetDate, decimal amount, float percent, long userId);

    void AddDailySale(long branchId, long hostId, DateTime targetDate, decimal amount, float percent, long userId);

    void AddHandoverTarget(long branchId, long hostId, DateTime targetDate, decimal handoverBudget, float arrearTarget, long userId);

    void AddLoanMix(DateTime targetDate, int payNo, float percent);

    // not set up for a specific branch as yet, but can be easily done if needed
    ICollection<IDailySaleModel> GetDailySales(DateTime targetMonth);

    ICollection<ILoanMixModel> GetLoanMixes(DateTime targetMonth);

    ICollection<IBranchCiMonthly> GetBranchCiMonthlys(long branchId, long hostId, DateTime targetMonth);

    ICollection<IHandoverTarget> GetHandoverTargets(long branchId, long hostId, DateTime targetMonth);
  }
}