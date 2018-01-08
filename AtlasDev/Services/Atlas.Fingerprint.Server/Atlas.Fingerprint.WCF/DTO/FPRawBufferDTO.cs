using System;
using System.Runtime.Serialization;


namespace Atlas.WCF.FPServer.Interface
{
  /// <summary>
  /// Actual raw fingeprint data (352 x 288 pixels, 256 grayscale raw buffer)
  /// </summary>
  [DataContract]
  public class FPRawBufferDTO
  {
    [DataMember]
    public Int64 PersonId { get; set; }

    /// <summary>
    ///ANSI/NIST-ITL 1 standard:
    //   1- Right-hand thumb, 2- Right-hand index, 3- Right-hand middle, 4-Right-hand ring, 5- Right-hand pinky
    //   6- Left-hand thumb, 7- Left-hand index, 8- Left-hand middle, 9-Left-hand ring, 10- Left-hand pinky 
    /// </summary>
    [DataMember]
    public int FingerId { get; set; }

    /// <summary>
    /// Raw Bitmap stream- Bitmap performs RLE compression, so image is pretty small (around 55KB), instead of raw 101,376 bytes
    /// </summary>
    [DataMember]
    public byte[] RawBuffer { get; set; }
       
    /// <summary>
    /// NFIQ of the bitmap
    /// </summary>
    [DataMember]
    public int NFIQ { get; set; }

    [DataMember]
    public int Width { get; set; }

    [DataMember]
    public int Height { get; set; }

    [DataMember]
    public BitmapColoursType BitmapColours { get; set; }
  }
}
