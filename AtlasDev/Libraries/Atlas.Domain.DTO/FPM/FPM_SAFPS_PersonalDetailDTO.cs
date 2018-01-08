using System.Runtime.Serialization;

namespace Atlas.Domain.DTO
{
  using System;

    [Serializable]
  [DataContract(IsReference = true)]
  public sealed class FPM_SAFPS_PersonDetailDTO
  {
    [DataMember]
    public Int64 PersonDetailId { get;set;}
    [DataMember]
    public FPM_SAFPSDTO SAFPS { get;set;}
    [DataMember]
    public string Title { get;set;}
    [DataMember]
    public string Surname { get;set;}
    [DataMember]
    public string Firstname { get;set;}
    [DataMember]
    public string ID { get;set;}
    [DataMember]
    public string Passport { get;set;}
    [DataMember]
    public string DateOfBirth { get;set;}
    [DataMember]
    public string Gender { get; set; }
    [DataMember]
    public string Email { get;set;}

  }
}
