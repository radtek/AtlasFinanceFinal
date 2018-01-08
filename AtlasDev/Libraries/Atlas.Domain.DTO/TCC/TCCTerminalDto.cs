using System;


namespace Atlas.Domain.DTO
{
  public class TCCTerminalDTO
  {
    public Int64 TerminalId { get; set; }
    public string SupplierTerminalId { get; set; }
    public string MerchantId { get; set; }
    public string IPAddress { get; set; }
    public string Branch { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public string SerialNum { get; set; }
    public DateTime CreatedDT { get; set; }
    public int Status { get; set; }
    public DateTime? LastPolledDT { get; set; }
    public string LastPolledResult { get; set; }
    public string LastRequestType { get; set; }
    public DateTime? LastRequestDT { get; set; }
    public string LastRequestResult { get; set; }
    public string HWMake { get; set; }
    public string HWModel { get; set; }
    public Int32 HWId { get; set; }
    public Int32 SuccessPingCount { get; set; }
    public Int32 FailedPingCount { get; set; }
    public Int32 UnknownPingCount { get; set; }
  }
}
