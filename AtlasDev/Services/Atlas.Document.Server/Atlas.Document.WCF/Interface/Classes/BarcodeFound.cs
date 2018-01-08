using System;
using System.Runtime.Serialization;


namespace Atlas.DocServer.WCF.Interface
{
  [Serializable]
  [DataContract(Namespace = "urn:Atlas/ASS/DocServer/Recognition/2014/05")]  
  public sealed class BarcodeFound
  {   
    [DataMember]
    public BarcodeEnums.BarcodeTypes Type { get; set; }
    
    [DataMember]
    public int BarcodeNumber { get; set; }
    
    [DataMember]
    public int Columms { get; set; }
    
    [DataMember]
    public int Rows { get; set; }
    
    [DataMember]
    public Double SkewAngle { get; set; }
    
    [DataMember]
    public int X1 { get; set; }
   
    [DataMember]
    public int X2 { get; set; }
    
    [DataMember]
    public int Y1 { get; set; }
    
    [DataMember]
    public int Y2 { get; set; }
    
    [DataMember]
    public string Data { get; set; }
    
    [DataMember]
    public int PageNo { get; set; }
    
    [DataMember]
    public BarcodeEnums.BarcodeDirections Direction { get; set; }
  }
  
}
