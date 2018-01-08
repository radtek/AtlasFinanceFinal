namespace Falcon.Common.Structures
{
  public sealed class PayoutStatistics
  {
    public int TotalTransactions { get; set; }
    public int CancelledRemoved { get; set; }
    public int NewOnHold { get; set; }
    public int Invalid { get; set; }
    public int Submitted { get; set; }
    public int Successful { get; set; }
    public int Failed { get; set; }
  }
}
