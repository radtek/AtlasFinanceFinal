/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Mongo entity- Fingerprint Bitmap- We use store Bitmaps as these take less space and store width/height/pixels/etc.
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
 *
 * -----------------------------------------------------------------------------------------------------------------  */


namespace Atlas.WCF.FPServer.MongoDB.Entities
{
  #region Using

  using System;
  using global::MongoDB.Bson.Serialization.Attributes;
  using global::MongoDB.Bson;

  #endregion


  /// <summary>
  /// Raw bitmap image class
  /// </summary>
  public class FPBitmap
  {
    /// <summary>
    /// THe PK/OID
    /// </summary>
    public global::MongoDB.Bson.ObjectId Id { get; set; }

    /// <summary>
    /// The PersonId
    /// </summary>
    public Int64 PersonId { get; set; }

    /// <summary>
    /// The FingerId- 1- right thumb, 2- right index, 3- right middle, .. 5- Right small, 6- Left thumb... 10-Left small
    /// </summary>
    public Int32 FingerId { get; set; }

    /// <summary>
    /// Date of creation
    /// </summary>
    public DateTime CreatedDT { get; set; }
    
    /// <summary>
    /// Captured by PersonId
    /// </summary>
    public Int64 CreatedPersonId { get; set; }

    /// <summary>
    /// Manager PersonId
    /// </summary>
    public Int64 ManagerPersonId { get; set; }

    /// <summary>
    /// The raw bitmap image
    /// </summary>
    public Byte[] Bitmap { get; set; }

    /// <summary>
    /// The quality of the image
    /// </summary>
    public Int32 Quality { get; set; }
    
    /// <summary>
    /// The NFIQ of the image
    /// </summary>
    public Int32 NFIQ { get; set; }

    /// <summary>
    /// The width of the bitmap
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The height of the bitmap
    /// </summary>
    public int Height { get; set; }
    
    [BsonRepresentation(BsonType.Int32)]
    public BitmapKind BitmapColour { get; set; }        
  }

  public enum BitmapKind
  {
    BlackOnWhite = 1,
    WhiteOnBlack = 2
  };
}
