using System;


namespace Atlas.Document.Mongo
{
  public class FileDocStorage
  {
    /// <summary>
    /// The PK/OID
    /// </summary>
    public MongoDB.Bson.ObjectId Id { get; set; }

    public byte[] Document { get; set; }    

  }
}
