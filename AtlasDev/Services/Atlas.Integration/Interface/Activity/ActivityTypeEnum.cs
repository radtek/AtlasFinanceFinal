using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{  
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Activity")]
  public enum ActivityTypeEnum
  {
    [EnumMember]
    NewLoan = 1,

    [EnumMember]
    InstalmentPaid = 2,

    [EnumMember]
    LoanClosed = 3,

    [EnumMember]
    HandedOver = 10
  }
}
