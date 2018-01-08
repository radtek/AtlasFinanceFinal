using System;
using Atlas.Enumerators;


namespace Atlas.Domain.DTO
{
  public class LogHWStatusDTO
  {
    public Int64 LogHWStatusId { get; set; }
    public General.DeviceType DeviceType { get; set; }
    public Int64 DeviceId { get; set; }
    public DateTime EventDT { get; set; }
    public General.HWStatus HWStatus { get; set; }
    private string MessageStore { get; set; }
    public string ResultMessage
    {
      get { return MessageStore; }
      set { MessageStore = !string.IsNullOrEmpty(value) && value.Length >= 500 ? value.Substring(0, 500) : value; }
    }
    public PER_PersonDTO CreatedBy { get; set; }
    public PER_PersonDTO DeletedBy { get; set; }
    public PER_PersonDTO LastEditedBy { get; set; }
    public DateTime? CreatedDT { get; set; }
    public DateTime? DeletedDT { get; set; }
    public DateTime? LastEditedDT { get; set; }
  }
}
