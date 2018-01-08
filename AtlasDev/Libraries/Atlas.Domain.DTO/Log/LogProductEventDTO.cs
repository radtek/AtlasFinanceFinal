using System;


namespace Atlas.Domain.DTO
{
  public class LogProductEventDTO
  {
    public Int64 LogProductEventId { get; set; }
    public DateTime EventDT { get; set; }
    public TransactionSourceDTO SourceSystem { get; set; }
    public DateTime RequestStartDT { get; set; }
    public DateTime RequestEndDT { get; set; }
    public Enumerators.General.LogProductRequestType ProductRequestType { get; set; }
    public COR_MachineDTO RequestedBy { get; set; }
    public string RequestParams { get; set; }
    public Enumerators.General.LogProductRequestResult RequestResult { get; set; }
    public string ResultText { get; set; }
  }
}
