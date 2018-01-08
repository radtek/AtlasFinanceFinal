/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2014 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    WCF implementation of document conversion (images- TIFF/JPG/JP2K/BMP, text- Text,CSV,MHT,HTML,DOC,DOCX,XLS,XLSX)
 *    using the DevExpress document/spreadsheet server and GdPicture imaging SDK.
 *       
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     May 2014- Created
 * 
 * 
 *  Comments:
 *  ------------------
 *     Using lots of large byte[] arrays will cause GC LOH memory issues- I try to use streams as 
 *     much as possible to minimize LOH issues and only convert the stream to a byte[] at the end.
 *     
 *     The files here should only be a couple of MB, so WCF streaming is not be required. WCF streaming imposes
 *     considerable limitations, so a separate Streaming WCF interface should be defined for handling >100MB/gigabyte 
 *     sized files.     
 *     
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.ServiceModel;

using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Convert
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class DocumentConvertServer : IDocumentConvertServer
  {
    public DocumentConvertServer(ILogging log)
    {
      _log = log;
    }


    public byte[] Convert(byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat,
      FileFormatEnums.FormatType destFileFormat, string openPassword,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      return Convert_Impl.Execute(_log, source, isCompressed, sourceFileFormat, destFileFormat, openPassword, docOptions, renderOptions, destCompress);      
    }


    public byte[] PDFAddPassword(byte[] source, bool isCompressed, string openPassword, DocOptions docOptions, bool destCompress)
    {
      return PDFAddPassword_Impl.Execute(_log, source, isCompressed, openPassword, docOptions, destCompress);      
    }


    public byte[] PDFOptimize(byte[] source, bool isCompressed, string openPassword,
      bool flattenFields, bool removeAllBookmarks, bool removeAllSignatures, bool removeAllLinks, bool removeAllAnnotations,
      byte setImagesBitDepth, int jp2KQuality, DocOptions options, bool destCompress)
    {
      return PDFOptimize_Impl.Execute(_log, source, isCompressed, openPassword, flattenFields, removeAllBookmarks,
        removeAllSignatures, removeAllLinks, removeAllAnnotations, setImagesBitDepth, jp2KQuality, options, destCompress);      
    }


    public byte[] PDFAddSignature(byte[] source, bool isCompressed, string openPassword, DocOptions options, bool destCompress)
    {
      return PDFAddSignature_Impl.Execute(_log, source, isCompressed, openPassword, options, destCompress);      
    }


    public bool IsValidDocument(byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat, string openPassword)
    {
      return IsValidDocument_Impl.Execute(_log, source, isCompressed, sourceFileFormat, openPassword);      
    }


    public bool DocumentContainsData(byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat,
      string openPassword)
    {
      return DocumentContainsData_Impl.Execute(_log, source, isCompressed, sourceFileFormat, openPassword);      
    }


    public byte[] CleanUpScan(byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat, string openPassword,
      CleanUpOptions cleanUpOptions, DocOptions docOptions, RenderOptions renderOptions,
      FileFormatEnums.FormatType destFileFormat, bool destCompress)
    {
      return CleanUpScan_Impl.Execute(_log, source, isCompressed, sourceFileFormat, openPassword, cleanUpOptions, docOptions, renderOptions,
        destFileFormat, destCompress);
    }


    #region Private vars

    private static ILogging _log;

    #endregion

  }
}