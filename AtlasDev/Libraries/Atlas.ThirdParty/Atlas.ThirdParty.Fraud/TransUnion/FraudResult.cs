using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Atlas.ThirdParty.Fraud.TransUnion
{
  [DataContract]
  public sealed class FraudResult
  {
    [DataMember]
    public long FraudScoreId { get; set; }
    [DataMember]
    public int Rating { get; set; }
    [DataMember]
    public List<string> ReasonCodes { get; set; }
    [DataMember]
    public EnquiryStatus EnquiryStatus { get;set;}
    [DataMember]
    public Enumerators.Account.AccountStatus Status { get; set; }
    [DataMember]
    public Enumerators.Account.AccountStatusReason StatusReason { get; set; }
    [DataMember]
    public Enumerators.Account.AccountStatusSubReason SubStatusReason { get; set; }
  }

  [DataContract]
  public enum EnquiryStatus
  {
    [EnumMember]
    Unknown = 0,
    [EnumMember]
    Success = 1,
    [EnumMember]
    Error = 2
  }
}
