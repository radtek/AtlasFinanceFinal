using Atlas.Enumerators;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Structures.UserTracking
{
  public class UserTrackingPinnedUserRuleset : IUserTrackingPinnedUserRuleSet
  {
    public long RuleSetId { get; set; }
    public Tracking.AlertType AlertType { get; set; }
    public Tracking.SeverityClassification SeverityClassification { get; set; }
    public Tracking.Elapse Elapse { get; set; }
    public int Value { get; set; }
  }
}
