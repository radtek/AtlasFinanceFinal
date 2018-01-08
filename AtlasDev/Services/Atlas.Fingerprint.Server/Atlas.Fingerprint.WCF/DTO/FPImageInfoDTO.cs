using System;
using System.Runtime.Serialization;


namespace Atlas.WCF.FPServer.Interface
{
  /// <summary>
  /// Fingerprint image class
  /// </summary>
  [DataContract]
  public class FPImageInfoDTO
  {
    /// <summary>
    /// THe PK/OID
    /// </summary>
    [DataMember]
    public string ObjectId { get; set; }

    /// <summary>
    /// The PersonId
    /// </summary>
    [DataMember]
    public Int64 PersonId { get; set; }

    /// <summary>
    /// The FingerId- 1- right thumb, 2- right index, 3- right middle, .. 5- Right small, 6- Left thumb... 10-Left small
    /// </summary>
    [DataMember]
    public Int32 FingerId { get; set; }

    /// <summary>
    /// Date of creation
    /// </summary>
    [DataMember]
    public DateTime CreatedDT { get; set; }

    /// <summary>
    /// Captured by PersonId
    /// </summary>
    [DataMember]
    public Int64 CreatedPersonId { get; set; }

    /// <summary>
    /// Manager PersonId
    /// </summary>
    [DataMember]
    public Int64 ManagerPersonId { get; set; }

    /// <summary>
    /// The quality of the image
    /// </summary>
    [DataMember]
    public Int32 Quality { get; set; }

    /// <summary>
    /// The NFIQ of the image
    /// </summary>
    [DataMember]
    public Int32 NFIQ { get; set; }
  }
}
