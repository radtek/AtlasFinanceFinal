
using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference=true)]
  public sealed class FPM_AuthenticationProcessStoreDTO
  {
    [DataMember]
    public Int64 AuthenticationProcessStoreId { get; set; }
    [DataMember]
    public FPM_AuthenticationDTO Authentication { get; set; }
    [DataMember]
    public Byte[] ProcessDocument { get; set; }
  }
}
