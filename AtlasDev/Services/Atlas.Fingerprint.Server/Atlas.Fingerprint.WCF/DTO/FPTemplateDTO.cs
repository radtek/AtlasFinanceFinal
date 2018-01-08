using System;
using System.Runtime.Serialization;


namespace Atlas.WCF.FPServer.Interface
{
  /// <summary>
  /// Class to send/receive template information
  /// </summary>
  [DataContract]
  public class FPTemplateDTO
  {
    [DataMember]
    public Int64 PersonId { get; set; }
    // ANSI/NIST-ITL 1 standard:
    //   1- Right-hand thumb, 2- Right-hand index, 3- Right-hand middle, 4-Right-hand ring, 5- Right-hand pinky
    //   6- Left-hand thumb, 7- Left-hand index, 8- Left-hand middle, 9-Left-hand ring, 10- Left-hand pinky
    [DataMember]
    public int FingerId { get; set; }

    [DataMember]
    public byte[] TemplateBuffer { get; set; }

    [DataMember]
    public string TemplateHash { get; set; }

  }
}
