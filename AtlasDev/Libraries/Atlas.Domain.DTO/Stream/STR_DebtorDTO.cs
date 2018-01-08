using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class STR_DebtorDTO
  {
    [DataMember]
    public long DebtorId { get; set; }
    [DataMember]
    public string IdNumber { get; set; }
    [DataMember]
    public DateTime DateOfBirth { get; set; }
    [DataMember]
    public string Title { get; set; }
    [DataMember]
    public string FirstName { get; set; }
    [DataMember]
    public string LastName { get; set; }
    [DataMember]
    public string OtherName { get; set; }
    [DataMember]
    public long Reference { get; set; }
    [DataMember]
    public string ThirdPartyReferenceNo { get; set; }
    [DataMember]
    public string EmployerCode { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}