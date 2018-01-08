using System;
using System.Runtime.Serialization;

using Atlas.Domain.DTO.Nucard;


namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class BRN_BranchDTO
  {
    [DataMember]
    public Int64 BranchId { get; set; }
    [DataMember]
    public CPY_CompanyDTO Company { get; set; }
    [DataMember]
    public NUC_NuCardProfileDTO DefaultNuCardProfile { get; set; }
    [DataMember]
    public RegionDTO Region { get; set; }
    [DataMember]
    public string LegacyBranchNum { get; set; }
    [DataMember]
    public DateTime? OpenDT { get; set; }
    [DataMember]
    public DateTime? CloseDT { get; set; }
    [DataMember]
    public string Comment { get; set; }
    [DataMember]
    public PER_PersonDTO CreatedBy { get; set; }
    [DataMember]
    public PER_PersonDTO DeletedBy { get; set; }
    [DataMember]
    public DateTime? CreatedDT { get; set; }
  }
}
