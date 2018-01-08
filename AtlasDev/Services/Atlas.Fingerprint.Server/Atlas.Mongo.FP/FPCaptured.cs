/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Mongo entity- Fingerprint Bitmap- We use store as WSQ file.
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2015-10-05- Basic functionality started
 *     
 *
 * -----------------------------------------------------------------------------------------------------------------  */

using System;

using MongoDB.Bson;


namespace Atlas.MongoDB.Entities
{  
  /// <summary>
  /// Raw bitmap image class
  /// </summary>
  public class FPCaptured
  {
    /// <summary>
    /// THe PK/OID
    /// </summary>
    public ObjectId Id { get; set; }

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
    /// The raw bitmap image
    /// </summary>
    public byte[] Bitmap { get; set; }

    /// <summary>
    /// The width of the bitmap
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// The height of the bitmap
    /// </summary>
    public int Height { get; set; }

    public string SessionId { get; set; }


    public string MachineId { get; set; }
  }

}
