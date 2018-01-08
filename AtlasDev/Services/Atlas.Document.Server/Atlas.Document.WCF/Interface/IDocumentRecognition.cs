using System;
using System.ServiceModel;
using System.Collections.Generic;


namespace Atlas.DocServer.WCF.Interface
{
  /// <summary>
  /// Document recognition related functionality- i.e. barcode detection, automatic document recognition
  /// </summary>
  [ServiceContract(Namespace = "urn:Atlas/ASS/DocServer/Recognition/2014/06")]
  public interface IDocumentRecognition
  {
    /// <summary>
    /// Finds all barcodes in a file
    /// </summary>
    /// <param name="sourceFileFormat">The source file format</param>
    /// <param name="source">The source file byte data</param>
    /// <param name="openPassword">Password to open source file</param>
    /// <param name="isCompressed">Is the source byte data compressed</param>
    /// <param name="searchOrientation">Orientation of barcodes to search in file</param>
    /// <param name="searchForTypes">Barcode types to search for</param>
    /// <returns>List of barcodes found</returns>
    [OperationContract]
    List<BarcodeFound> FindBarcodes(FileFormatEnums.FormatType sourceFileFormat, byte[] source, string openPassword, bool isCompressed,
      List<BarcodeEnums.BarcodeDirections> searchOrientation, List<BarcodeEnums.BarcodeTypes> searchForTypes);


    /// <summary>
    /// Try determine the category of the document using GdPicture ADR templates
    /// </summary>
    /// <param name="sourceFileFormat">The source file format</param>
    /// <param name="source">The source file byte data</param>
    /// <param name="openPassword">Password to open source file</param>
    /// <param name="isCompressed">Is the source byte data compressed</param>
    /// <returns>The document category found, null if no match</returns>
    [OperationContract]
    DocumentFound DetermineDocumentCategory(FileFormatEnums.FormatType sourceFileFormat, byte[] source, string openPassword, bool isCompressed);
           
  }

}
