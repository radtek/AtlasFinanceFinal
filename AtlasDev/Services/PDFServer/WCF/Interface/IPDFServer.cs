namespace Atlas.PDF.Server.WCF.Interface
{
  #region Using

  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.Serialization;
  using System.ServiceModel;

  #endregion

  [ServiceContract]
  public interface IPDFServer
  {
    [OperationContract]
    PdfResult GetPdf(string content);

    [OperationContract]
    PdfResult ConvertMhtToPdf(string content);
  }

  [DataContract]
  public class PdfResult
  {
    [DataMember]
    public string Error { get; set; }
    [DataMember]
    public byte[] Bytes { get; set; }
  }
}
