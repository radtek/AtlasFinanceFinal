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
  public class STR_AccountDTO
  {
    [DataMember]
    public Int64 AccountId { get; set; }
    [DataMember]
    public STR_DebtorDTO Debtor { get; set; }
    [DataMember]
    public HostDTO Host { get; set; }
    [DataMember]
    public BRN_BranchDTO Branch { get; set; }
    [DataMember]
    public string Reference { get; set; }
    [DataMember]
    public long Reference2 { get; set; }
    [DataMember]
    public string LastImportReference { get; set; }
    [DataMember]
    public DateTime LoanDate { get; set; }
    [DataMember]
    public decimal LoanAmount { get; set; }
    [DataMember]
    public int LoanTerm { get; set; }
    [DataMember]
    public STR_FrequencyDTO Frequency { get; set; }
  }
}