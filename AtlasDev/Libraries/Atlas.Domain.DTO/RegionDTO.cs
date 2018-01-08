using System;

using Atlas.Domain.DTO.Nucard;


namespace Atlas.Domain.DTO
{
  public class RegionDTO
  {
    public Int64 RegionId { get; set; }
    public NUC_NuCardProfileDTO Profile { get; set; }
    public string LegacyRegionCode { get; set; }
    public string Description { get; set; }
  }
}
