namespace Atlas.Domain.DTO
{
  public class WFL_ProcessStepJobAccountDTO
  {
    public long ProcessStepJobAccountId { get; set; }
    public WFL_ProcessStepJobDTO ProcessStepJob { get; set; }
    public ACC_AccountDTO Account { get; set; }
    public bool Override { get; set; }
    public PER_PersonDTO OverrideUser { get; set; }
  }
}
