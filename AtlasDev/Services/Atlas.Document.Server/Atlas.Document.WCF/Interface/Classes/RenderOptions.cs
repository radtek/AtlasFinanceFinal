using System;
using System.Runtime.Serialization;


namespace Atlas.DocServer.WCF.Interface
{
  [Serializable]
  [DataContract(Namespace = "urn:Atlas/ASS/DocServer/Generation/2014/05")]  
  /// <summary>
  /// Document render options
  /// </summary>
  public sealed class RenderOptions
  {
    [DataMember]
    public int DPI { get; set; } // Render DPI for PDF to bitmap (96- screen, 128- default, 300- high quality, 600- very high quality)

    [DataMember]
    public bool IsLandscape { get; set; } // Render in landscape page orientation?   
 
    [DataMember]
    public int LeftMarginMM { get; set; } // Left page margin in millimeters

    [DataMember]
    public int RightMarginMM { get; set; } // Right page margin in millimeters  
  
    [DataMember]
    public int TopMarginMM { get; set; } // Top page margin in millimeters

    [DataMember]
    public int BottomMarginMM { get; set; } // Bottom page margin in millimeters     
   
    [DataMember]
    public int ImageQuality { get; set; } // Image quality: PDF/JPEG: 1..100, PNG: 1..9, JP2K: 1..512
    
  }
}
