using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  public class WFL_PeriodFrequencyDTO
  {
    public int PeriodFrequencyId { get; set; }
    public Enumerators.Workflow.PeriodFrequency Type
    {
      get
      {
        return Name.FromStringToEnum<Enumerators.Workflow.PeriodFrequency>();
      }
      set
      {
        value = Name.FromStringToEnum<Enumerators.Workflow.PeriodFrequency>();
      }
    }
    public string Name { get; set; }
  }
}
