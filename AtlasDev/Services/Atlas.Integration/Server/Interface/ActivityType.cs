using System;
using System.Runtime.Serialization;

namespace Atlas.Integration
{
  [DataContract]
  public enum ActivityType
  {
    [EnumMember]
    NewLoan = 1,

    [EnumMember]
    InstalmentPaid = 2,

    [EnumMember]
    HandedOver = 10
  }
}
