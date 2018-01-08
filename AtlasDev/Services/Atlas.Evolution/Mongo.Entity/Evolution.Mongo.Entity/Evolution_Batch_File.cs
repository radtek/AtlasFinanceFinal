using System;

using MongoDB.Bson;


namespace Evolution.Mongo.Entity
{
  public class Evolution_Batch_File
  {
    /// <summary>
    /// The PK/OID
    /// </summary>
    public ObjectId Id { get; set; }

    /// <summary>
    /// Date of creation
    /// </summary>
    public DateTime CreatedDT { get; set; }


    /// <summary>
    /// The raw file
    /// </summary>
    public byte[] File { get; set; }

    public long UploadBatchId { get; set; }

    public string Filename { get; set; }

  }
}
