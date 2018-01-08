namespace Falcon.Common.Structures
{
  public sealed class NaedoServiceBank
  {
    public int ServiceId { get; set; }
    public long BankId { get; set; }
    public string BankName { get; set; }
    public bool IsLinked { get; set; }
  }

  public sealed class NaedoService
  {
    public int ServiceId { get; set; }
    public int NextTransmissionNo { get; set; }
    public int NextGenerationNo { get; set; }
    public int NextSequenceNo { get; set; }
    public string ServiceName { get; set; }
    public bool SaveSequenceNo { get; set; }
  }
}
