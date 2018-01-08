using System;


namespace Atlas.Domain.DTO
{
  public class LogNuCardEventDTO
  {
    public Int64 LogNuCardEventId { get; set; }
    public DateTime EventDT { get; set; }
    public TransactionSourceDTO SourceSystem { get; set; }
    public Enumerators.General.LogNuCardRequestType NuCardRequestType { get; set; }
    public MachineStoreDTO RequestedBy { get; set; }
    public Enumerators.General.NuCardLogRequestResult RequestResult { get; set; }
    public Decimal? Amount { get; set; }
    public NuCardDTO SourceCard { get; set; }
    public NuCardDTO DestCard { get; set; }
    public string AdditionalInfo { get; set; }
    public string ClientTransactionID { get; set; }
    public string ServerTransactionID { get; set; }
    public string ResultText { get; set; }
    public PER_PersonDTO CreatedBy { get; set; }
    public PER_PersonDTO DeletedBy { get; set; }
    public PER_PersonDTO LastEditedBy { get; set; }
    public DateTime? CreatedDT { get; set; }
    public DateTime? DeletedDT { get; set; }
    public DateTime? LastEditedDT { get; set; }
  }
}
