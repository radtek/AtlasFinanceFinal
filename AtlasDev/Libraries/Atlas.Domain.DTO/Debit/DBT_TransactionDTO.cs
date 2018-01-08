using System;
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class DBT_TransactionDTO
  {
    [DataMember]
    public Int64 TransactionId { get; set; }
    [DataMember]
    public DBT_ControlDTO Control { get; set; }
    [DataMember]
    public DBT_DebitTypeDTO DebitType { get; set; }
    [DataMember]
    public DBT_BatchDTO Batch { get; set; }
    [DataMember]
    public DBT_StatusDTO Status { get; set; }
    [DataMember]
    public DateTime? LastStatusDate { get; set; }
    [DataMember]
    public decimal Amount { get; set; }
    [DataMember]
    public DateTime InstalmentDate { get; set; }
    [DataMember]
    public DateTime ActionDate { get; set; }
    [DataMember]
    public int Repetition { get; set; }
    [DataMember]
    public DBT_ResponseCodeDTO ResponseCode { get; set; }
    [DataMember]
    public DateTime? ResponseDate { get; set; }
    [DataMember]
    public DateTime? CancelDate { get; set; }
    [DataMember]
    public PER_PersonDTO CancelUser { get; set; }
    [DataMember]
    public DateTime? OverrideDate { get; set; }
    [DataMember]
    public PER_PersonDTO OverrideUser { get; set; }
    [DataMember]
    public decimal? OverrideAmount { get; set; }
    [DataMember]
    public DateTime? OverrideActionDate { get; set; }
    [DataMember]
    public int? OverrideTrackingDays { get; set; }
    [DataMember]
    public DateTime? CreateDate { get; set; }
  }
}