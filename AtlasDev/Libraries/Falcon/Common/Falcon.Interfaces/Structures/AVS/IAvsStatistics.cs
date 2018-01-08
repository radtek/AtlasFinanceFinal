namespace Falcon.Common.Interfaces.Structures.AVS
{
  public interface IAvsStatistics
  {
    string Bank { get; set; }
    int TotalTransactions { get; set; }
    int TotalSent { get; set; }
    int TotalQueued { get; set; }
    int TotalPending { get; set; }
    int TotalComplete { get; set; }
    string ResponseTime { get; set; }
  }
}