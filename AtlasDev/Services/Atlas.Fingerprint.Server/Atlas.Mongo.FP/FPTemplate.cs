
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Mongo entity- Fingerprint template- 9456 byte Integrated Biometrics
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-11-13- Basic functionality started
 *     
 *     2013-03-28- Added image orientation
 *
 * -----------------------------------------------------------------------------------------------------------------  */
using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;


namespace Atlas.MongoDB.Entities
{
  public class FPTemplate
  {
    /// <summary>
    /// 
    /// </summary>
    public ObjectId Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Int64 PersonId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Int32 FingerId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [BsonRepresentation(BsonType.Int32)]
    public TemplateType TemplateTypeId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [BsonRepresentation(BsonType.Int32)]
    public Atlas.Enumerators.Biometric.OrientationType ImageOrientation { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime CreatedDT { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Int64 CreatedPersonId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Int64 ManagerPersonId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Int64 LastReviewedPersonId { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime LastReviewedDT { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Byte[] TemplateBuffer { get; set; }
  }

  public enum TemplateType
  {
    IntegratedBiometrics9052 = 1,
    IntegratedBiometrics404 = 2
  };

}

