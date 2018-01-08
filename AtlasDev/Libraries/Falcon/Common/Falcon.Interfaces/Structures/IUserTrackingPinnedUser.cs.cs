using System;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IUserTrackingPinnedUser
  {
    long PinnedUserId { get; set; }
    long UserId { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string LastBranchActivity { get; set; }
    int ViolationCount { get; set; }
    bool Active { get; set; }
    IUserTrackingPinnedUserRuleSet RuleSet { get; set; }
    DateTime CreateDate { get; set; }
  }
}
