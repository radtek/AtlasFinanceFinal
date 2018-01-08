/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2014 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Useful document utilities
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
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text;
using Atlas.DocServer.WCF.Interface;
using DevExpress.XtraPrinting;


namespace Atlas.DocServer.WCF.Implementation.Utils
{
  internal static class DocUtils
  {
    /// <summary>
    /// Convert jpeg quality setting to DevExpress pdf jpeg quality setting
    /// </summary>
    /// <param name="imageQuality">Image quality: 0-100</param>
    /// <returns>The DevExpress pdf jpg image quality</returns>
    internal static PdfJpegImageQuality ToDxJpegQuality(int imageQuality)
    {
      if (imageQuality <= 20)
      {
        return PdfJpegImageQuality.Lowest;
      }
      else if (imageQuality <= 40)
      {
        return PdfJpegImageQuality.Low;
      }
      else if (imageQuality <= 60)
      {
        return PdfJpegImageQuality.Medium;
      }
      else if (imageQuality <= 80)
      {
        return PdfJpegImageQuality.High;
      }
      else
      {
        return PdfJpegImageQuality.Highest;
      }
    }


    #region Enum utils

    /// <summary>
    /// Get GdPicture Document format enumeration from Atlas File Format enumeration
    /// </summary>
    /// <param name="fileFormat"></param>
    /// <returns></returns>
    internal static GdPicture12.DocumentFormat ToGdDocumentFormat(Enumerators.Document.FileFormat fileFormat)
    {
      var result = GdPicture12.DocumentFormat.DocumentFormatUNKNOWN;
      switch (fileFormat)
      {
        case Atlas.Enumerators.Document.FileFormat.BMP:
          result = GdPicture12.DocumentFormat.DocumentFormatBMP;
          break;

        case Atlas.Enumerators.Document.FileFormat.EMF:
          result = GdPicture12.DocumentFormat.DocumentFormatEMF;
          break;

        case Atlas.Enumerators.Document.FileFormat.JPEG:
          result = GdPicture12.DocumentFormat.DocumentFormatJPEG;
          break;

        case Atlas.Enumerators.Document.FileFormat.JPEG2K:
          result = GdPicture12.DocumentFormat.DocumentFormatJ2K;
          break;

        case Atlas.Enumerators.Document.FileFormat.PNG:
          result = GdPicture12.DocumentFormat.DocumentFormatPNG;
          break;

        case Atlas.Enumerators.Document.FileFormat.TIFF:
          result = GdPicture12.DocumentFormat.DocumentFormatTIFF;
          break;
      }

      return result;
    }


    /// <summary>
    /// Get graphic image format from Atlas file format enumeration
    /// </summary>
    /// <param name="fileFormat"></param>
    /// <returns></returns>
    internal static ImageFormat ToImagingFormat(Enumerators.Document.FileFormat fileFormat)
    {
      switch (fileFormat)
      {
        case Atlas.Enumerators.Document.FileFormat.BMP:
          return ImageFormat.Bmp;

        case Atlas.Enumerators.Document.FileFormat.EMF:
          return ImageFormat.Emf;

        case Atlas.Enumerators.Document.FileFormat.JPEG:
          return ImageFormat.Jpeg;

        case Atlas.Enumerators.Document.FileFormat.PNG:
          return ImageFormat.Png;

        case Atlas.Enumerators.Document.FileFormat.TIFF:
          return ImageFormat.Tiff;

        default:
          throw new NotSupportedException(fileFormat.ToString());
      }
    }


    /// <summary>
    /// Get DevExpress document format enumeration from Atlas File format enumeration
    /// </summary>
    /// <param name="fileFormat"></param>
    /// <returns>DevExpress document format enumeration</returns>
    internal static DevExpress.XtraRichEdit.DocumentFormat ToDocumentFormat(Enumerators.Document.FileFormat fileFormat)
    {
      switch (fileFormat)
      {
        case Atlas.Enumerators.Document.FileFormat.CSV:
        case Atlas.Enumerators.Document.FileFormat.TXT:
          return DevExpress.XtraRichEdit.DocumentFormat.PlainText;

        case Atlas.Enumerators.Document.FileFormat.DOC:
          return DevExpress.XtraRichEdit.DocumentFormat.Doc;

        case Atlas.Enumerators.Document.FileFormat.DOCX:
          return DevExpress.XtraRichEdit.DocumentFormat.OpenXml;

        case Atlas.Enumerators.Document.FileFormat.HTML:
          return DevExpress.XtraRichEdit.DocumentFormat.Html;

        case Atlas.Enumerators.Document.FileFormat.MHT:
          return DevExpress.XtraRichEdit.DocumentFormat.Mht;

        case Atlas.Enumerators.Document.FileFormat.ODT:
          return DevExpress.XtraRichEdit.DocumentFormat.OpenDocument;

        case Atlas.Enumerators.Document.FileFormat.RTF:
          return DevExpress.XtraRichEdit.DocumentFormat.Rtf;

        default:
          return DevExpress.XtraRichEdit.DocumentFormat.Undefined;
      }
    }


    /// <summary>
    /// Get DevExpress spreadsheet format enumeration from Atlas file format enumeration
    /// </summary>
    /// <param name="sourceFileFormat"></param>
    /// <returns></returns>
    internal static DevExpress.Spreadsheet.DocumentFormat ToSpreadSheetFormat(Enumerators.Document.FileFormat sourceFileFormat)
    {
      switch (sourceFileFormat)
      {
        case Atlas.Enumerators.Document.FileFormat.XLSX:
          return DevExpress.Spreadsheet.DocumentFormat.OpenXml;

        case Atlas.Enumerators.Document.FileFormat.XLS:
          return DevExpress.Spreadsheet.DocumentFormat.Xls;

        default:
          return DevExpress.Spreadsheet.DocumentFormat.Undefined;
      }
    }

    #endregion


    internal static DocOptions DocOptionsGetNotNull(DocOptions docOptions)
    {
      return docOptions ?? new
        DocOptions()
        {
          Author = DocConsts.STR_AtlasDocumentGenerator,
          CanAddNotes = true,
          CanAssemble = true,
          CanCopy = true,
          CanCopyAccess = true,
          CanFillFields = true,
          CanPrintFull = true,
          CanModify = true,
          CanPrint = true,
          Creator = DocConsts.STR_AtlasDocumentGenerator,
          OwnerPassword = "",
          Generator = DocConsts.STR_AtlasDocumentGenerator,
          Keywords = DocConsts.STR_AtlasDocumentGenerator,
          Producer = DocConsts.STR_AtlasDocumentGenerator,
          Subject = DocConsts.STR_AtlasDocumentGenerator,
          Title = DocConsts.STR_AtlasDocumentGenerator,
          UserPassword = ""
        };
    }


    /// <summary>
    /// Calculates a HMAC SHA-1 hash for given key and data
    /// </summary>
    /// <param name="key"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    internal static string CalcSHA512Hash(string key, byte[] data)
    {
      var keyBytes = Encoding.UTF8.GetBytes(key);
      using (var hmac = new HMACSHA512(keyBytes))
      {
        var hashValue = hmac.ComputeHash(data);
        var hexHash = new StringBuilder(hashValue.Length * 2);
        foreach (var ch in hashValue)
        {
          hexHash.AppendFormat("{0:x2}", ch);
        }

        return hexHash.ToString();
      }
    }
    
  }

}
