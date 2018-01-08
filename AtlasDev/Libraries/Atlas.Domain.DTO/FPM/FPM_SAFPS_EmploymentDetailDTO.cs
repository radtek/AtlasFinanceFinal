using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_SAFPS_EmploymentDetailDTO
  {
    [DataMember]
    public Int64 EmploymentDetailId { get; set; }
    [DataMember]
    public FPM_SAFPSDTO SAFPS { get; set; }
    [DataMember]
    public string EmploymentName { get; set; }
    [DataMember]
    public string Telephone { get; set; }
    [DataMember]
    public string RegisteredCompanyName { get; set; }
    [DataMember]
    public string CompanyNo { get; set; }
    [DataMember]
    public string Occupation { get; set; }
    [DataMember]
    public string EmployFrom { get; set; }
    [DataMember]
    public string EmployTo { get; set; }


  }
}
