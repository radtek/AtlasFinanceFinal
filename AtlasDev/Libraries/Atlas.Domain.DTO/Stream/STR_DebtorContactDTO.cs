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
  public class STR_DebtorContactDTO
  {
    [DataMember]
    public Int64 DebtorContactId { get; set; }
    [DataMember]
    public STR_DebtorDTO Debtor { get; set; }
    [DataMember]
    public ContactDTO Contact { get; set; }
    [DataMember]
    public PER_PersonDTO CreateUser { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}