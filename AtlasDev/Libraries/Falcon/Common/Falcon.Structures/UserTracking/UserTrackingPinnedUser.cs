using System;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures.UserTracking
{
  public class UserTrackingPinnedUser : IUserTrackingPinnedUser
  {
    public long PinnedUserId { get; set; }
    public long UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string LastBranchActivity { get; set; }
    public int ViolationCount { get; set; }
    public bool Active { get; set; }
    public IUserTrackingPinnedUserRuleSet RuleSet { get; set; }    
    public DateTime CreateDate { get; set; }
  }
}
