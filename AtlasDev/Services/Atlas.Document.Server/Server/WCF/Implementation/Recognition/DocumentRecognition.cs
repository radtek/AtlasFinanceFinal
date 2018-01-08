/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2014 Atlas Finance (Pty() Ltd.
* 
*  Description:
*  ------------------
*    WCF implementation of document recognition- detect barcodes/try determine a document type
*       
* 
*  Author:
*  ------------------
*     Keith Blows
* 
* 
*  Revision history: 
*  ------------------ 
*     May 2014 - Created
* 
*    June 2014 - Fleshing basics out
*    
*    Aug 2014  - Switched to SD-Toolkit barcode detection- royalty free, reliable and more barcode types detection
* 
 * 
*  Comments:
*  ------------------
*     Using lots of large byte[] arrays will cause GC memory issues (LOH- large object heap)- I try to use streams as 
*     much as possible to avoid LOH issues and only convert the stream to a byte[] at the end.
*     
*     The files here should only be a couple of MB, so WCF streaming is not be required. WCF streaming imposes
*     some limitations, so a Streaming WCF interface should be use for handling 100's MB/gigabyte sized files.
*     
*     
* 
* ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.ServiceModel;

using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Recognition
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class DocumentRecognition : IDocumentRecognition
  {
    public DocumentRecognition(ILogging log)
    {
      _log = log;
    }


    public List<BarcodeFound> FindBarcodes(FileFormatEnums.FormatType sourceFileFormat, byte[] source, string openPassword, bool isCompressed,
      List<BarcodeEnums.BarcodeDirections> searchOrientation, List<BarcodeEnums.BarcodeTypes> searchForTypes)
    {
      return FindBarcodes_Impl.Execute(_log, sourceFileFormat, source, openPassword, isCompressed, searchOrientation, searchForTypes);      
    }


    public DocumentFound DetermineDocumentCategory(FileFormatEnums.FormatType sourceFileFormat, byte[] source, string openPassword, bool isCompressed)
    {
      return DetermineDocumentCategory_Impl.Execute(_log, sourceFileFormat, source, openPassword, isCompressed);      
    }


    #region Private vars

    private static ILogging _log;

    #endregion

  }
}
