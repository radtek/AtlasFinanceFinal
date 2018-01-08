using System;
using MongoDB.Bson;


namespace Atlas.MongoDB.Entities
{
  public class FPTemplate2
  {/// <summary>
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
    public DateTime CreatedDT { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Int64 CreatedPersonId { get; set; }

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

    public Int32 Orientation { get; set; }
  }
}
