using System;
using System.Runtime.Serialization;


namespace Atlas.WCF.FPServer.Interface
{
  [DataContract]
  public sealed class RecoveryInfoDTO
  {
    [DataMember]
    public string RequestId { get; set;}

    [DataMember]
    public Int64 StartEnrollRef { get; set; }

    [DataMember]
    public Int64 PersonId { get; set; }

    [DataMember]
    public Int64 UserPersonId { get; set; }

    [DataMember]
    public string Message1 { get; set;}

    [DataMember]
    public string Message2 { get; set; }

    [DataMember]
    public string Message3 { get; set;}

    [DataMember]
    public int TimeoutSecs { get; set; }

    [DataMember]
    public string CSVFingersDone { get; set; }

    [DataMember]
    public BasicPersonDetailsDTO Person { get; set; }
  }


}