using Falcon.Common.Interfaces.Structures.AVS;

namespace Falcon.Common.Structures.Avs
{
  public sealed class AvsStatistics : IAvsStatistics
  {
    public string Bank { get; set; }
    public int TotalTransactions { get; set; }
    public int TotalSent { get; set; }
    public int TotalQueued { get; set; }
    public int TotalPending { get; set; }
    public int TotalComplete { get; set; }
    public string ResponseTime { get; set; }
  }
}
