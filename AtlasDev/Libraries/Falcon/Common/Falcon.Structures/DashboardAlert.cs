using System.ComponentModel;

namespace Falcon.Common.Structures
{
  public sealed class DashboardAlert
  {
    public enum Class
    {
      [Description("success")]
      Success,
      [Description("info")]
      Info,
      [Description("warning")]
      Warning,
      [Description("danger")]
      Danger
    }

    public Class Priority { get; set; }
    public string PriorityString { get; set; }
    public string Message { get; set; }
  }
}
