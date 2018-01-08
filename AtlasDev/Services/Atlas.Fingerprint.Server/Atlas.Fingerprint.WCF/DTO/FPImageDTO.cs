using System;
using System.Runtime.Serialization;


namespace Atlas.WCF.FPServer.Interface
{
  public class FPImageDTO : FPImageInfoDTO
  {
    [DataMember]
    public FPBitmapTypes ImageType { get; set; }

    /// <summary>
    /// The raw bitmap image
    /// </summary>
    [DataMember]
    public Byte[] Bitmap { get; set; }

    [DataMember]
    public BitmapColoursType ImageColours { get; set; }

    /// <summary>
    /// The width of the bitmap
    /// </summary>
    [DataMember]
    public int Width { get; set; }

    /// <summary>
    /// The height of the bitmap
    /// </summary>
    [DataMember]
    public int Height { get; set; }
  }
}
