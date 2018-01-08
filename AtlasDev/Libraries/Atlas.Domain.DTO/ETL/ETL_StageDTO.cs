using Atlas.Common.Extensions;

namespace Atlas.Domain.DTO
{
  public class ETL_StageDTO
  {
    public int StageId { get; set; }
    public Enumerators.ETL.Stage Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.ETL.Stage>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.ETL.Stage>();
      }
    }
    public string Description { get; set; }
  }
}
