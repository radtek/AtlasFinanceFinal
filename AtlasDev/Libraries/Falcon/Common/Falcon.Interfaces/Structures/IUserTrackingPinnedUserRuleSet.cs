using Atlas.Enumerators;

namespace Falcon.Common.Interfaces.Structures
{
  public interface IUserTrackingPinnedUserRuleSet
  {
    long RuleSetId { get; set; }
    Tracking.AlertType AlertType { get; set; }
    Tracking.SeverityClassification SeverityClassification { get; set; }
    Tracking.Elapse Elapse { get; set; }
    int Value { get; set; }
  }
}
