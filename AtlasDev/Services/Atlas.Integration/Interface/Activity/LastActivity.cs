using System;
using System.Runtime.Serialization;


namespace Atlas.Integration.Interface
{
  [DataContract(Namespace = "Atlas.Services.2015.Integration.Activity")]
  public class LastActivity
  {
    [DataMember]
    public Atlas.Integration.Interface.ActivityTypeEnum Activity { get; set; }
    [DataMember]
    public DateTime Date { get; set; }
    [DataMember]
    public string Notes { get; set; }
    [DataMember]
    public string Branch { get; set; }
  }
}

