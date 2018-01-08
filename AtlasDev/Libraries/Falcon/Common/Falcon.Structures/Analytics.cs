namespace Falcon.Common.Structures
{
  public sealed class Analytics
  {
    public string Branch { get; set; }
    public int AvsCount { get; set; }
    public int NaedoOrderSuccessCount { get; set; }
    public int NaedoOrderFailureCount { get; set; }
    public int RealTimePaymentCount { get; set; }
    public int RealTimePaymentPendingCount { get; set; }
    public int SourceRequest { get; set; }
  }
}