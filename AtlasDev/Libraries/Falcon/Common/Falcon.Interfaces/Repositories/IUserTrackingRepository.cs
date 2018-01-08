using System;
using System.Collections.Generic;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Interfaces.Repositories
{
  public interface IUserTrackingRepository
  {
    IList<IUserTrackingUserMovement> GetUserMovements(long? userId, long? branchId, DateTime startRange, DateTime endRange);
    IList<IUserTrackingBranchMovement> GetBranchMovements(long branchId, DateTime startRange, DateTime endRange);
    bool SavePin(long userId, Tracking.AlertType alertType, Tracking.SeverityClassification severityClassification, Tracking.Elapse elapse, 
      int value, string notify);
    IList<IUserTrackingPinnedUser> GetPinned(bool active);
    bool ResetViolations(bool active);
    bool UpdateViolationCount(long pinnedUserId, long ruleSetId, int violationCount);
    bool RemovePin(long pinnedUserId);
  }
}
