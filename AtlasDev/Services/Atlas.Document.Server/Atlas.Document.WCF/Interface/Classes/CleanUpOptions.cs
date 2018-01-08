using System;
using System.Runtime.Serialization;


namespace Atlas.DocServer.WCF.Interface
{
  [Serializable]
  [DataContract(Namespace = "urn:Atlas/ASS/DocServer/Convert/2014/05")]  
  public sealed class CleanUpOptions
  {
    [DataMember]
    public bool AutoDeskew { get; set; }   //  true to try automatically deskew

    [DataMember]
    public bool RemovePunchHoles { get; set; } // true to try remove punch holes 
   
    [DataMember]
    public bool CropWhiteBorders { get; set; } // true to crop white borders    

    [DataMember]
    public bool CropBlackBorders { get; set; } //   true to crop any black borders  
   
    [DataMember]
    public byte SetColourBitDepth { get; set; } // Colour bit depth to change to- 1, 8, 16, 24, 32, else 0, use: *9* - 8 bit, optimized palette
    
    [DataMember]
    public int NewWidth { get; set; } // New width- image will maintain aspect ratio  
  
    [DataMember]
    public int NewHeight { get; set; } // New height- image will maintain aspect ratio   
 
    [DataMember]
    public bool Despeckle { get; set; } // Performs a 3x3 despeckle filter. It works as a noise removal filter, for Salt-And-Pepper like-noise  
          
    [DataMember]
    public bool RemoveHorizontalLines { get; set; }

    [DataMember]
    public bool RemoveVerticalLines { get; set; }

  }
}
