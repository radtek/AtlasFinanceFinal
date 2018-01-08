using System;
using System.Runtime.Serialization;


namespace Atlas.WCF.FPServer.Interface
{
  /// <summary>
  /// Verification options
  /// </summary>
  [DataContract]
  public class FPVerifyOptionsDTO
  {
    [DataMember]
    public int MinSecurityLevel { get; set; }
  }
}
