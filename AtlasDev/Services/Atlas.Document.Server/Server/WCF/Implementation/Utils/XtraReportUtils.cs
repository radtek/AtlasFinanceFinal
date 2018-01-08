/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2014 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    XtraReport utilities
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
using System.IO;
using System.Drawing;

using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

using Atlas.DocServer.WCF.Interface;
using System.Drawing.Imaging;


namespace Atlas.DocServer.WCF.Implementation.Utils
{
  internal static class XtraReportUtils
  {
    /// <summary>
    /// Renders an XtraReport REPX report to the file format requested
    /// </summary>
    /// <param name="report">The XtraReport (must be loaded)</param>
    /// <param name="outFileFormat">The output file format</param>
    /// <param name="imageQuality">The image quality to be used for output</param>
    /// <param name="docSubject">The document subject</param>
    /// <returns></returns>
    internal static Stream RenderReport(XtraReport report, Enumerators.Document.FileFormat outFileFormat,
      DocOptions docOptions, RenderOptions renderOptions)
    {
      report.CreateDocument(false);
      Stream result = new MemoryStream();

      switch (outFileFormat)
      {
        // Single page image format
        case Enumerators.Document.FileFormat.BMP:
        case Enumerators.Document.FileFormat.EMF:
        case Enumerators.Document.FileFormat.JPEG:
        case Enumerators.Document.FileFormat.PNG:
          report.ExportToImage(result, new ImageExportOptions()
          {
            Format = Atlas.DocServer.WCF.Implementation.Utils.DocUtils.ToImagingFormat(outFileFormat),
            ExportMode = ImageExportMode.SingleFile,
            PageRange = "1",
            PageBorderColor = Color.White,
            Resolution = renderOptions.DPI > 0 && renderOptions.DPI < 600 ? renderOptions.DPI : 128
          });
          break;

        // Multi-page image format
        case Enumerators.Document.FileFormat.TIFF:
          report.ExportToImage(result, new ImageExportOptions()
          {
            Format = ImageFormat.Tiff,
            ExportMode = ImageExportMode.SingleFile,
            PageBorderColor = Color.White,
            Resolution = renderOptions.DPI > 0 && renderOptions.DPI < 600 ? renderOptions.DPI : 128
          });
          break;

        case Enumerators.Document.FileFormat.CSV:
          report.ExportToCsv(result, new CsvExportOptions()
          {
            Separator = ",",
            QuoteStringsWithSeparators = true,
            TextExportMode = TextExportMode.Value
          });
          break;

        case Enumerators.Document.FileFormat.XLSX:
          report.ExportToXlsx(result, new XlsxExportOptions()
          {
            ExportHyperlinks = true,
            ExportMode = XlsxExportMode.SingleFile,
            ShowGridLines = false,
            SheetName = docOptions.Subject ?? Atlas.DocServer.WCF.Implementation.Utils.DocConsts.STR_AtlasDocumentGenerator
          });
          break;

        case Enumerators.Document.FileFormat.XLS:
          report.ExportToXls(result, new XlsExportOptions()
          {
            ExportHyperlinks = true,
            ExportMode = XlsExportMode.SingleFile,
            ShowGridLines = false,
            SheetName = docOptions.Subject ?? Atlas.DocServer.WCF.Implementation.Utils.DocConsts.STR_AtlasDocumentGenerator,
          });
          break;

        case Enumerators.Document.FileFormat.RTF:
        case Enumerators.Document.FileFormat.DOC:
        case Enumerators.Document.FileFormat.DOCX:
          report.ExportToRtf(result, new RtfExportOptions()
          {
            ExportMode = RtfExportMode.SingleFile,
            ExportWatermarks = true
          });

          break;

        case Enumerators.Document.FileFormat.PDF:
          report.ExportToPdf(result, new PdfExportOptions()
          {
            Compressed = true,
            ConvertImagesToJpeg = true,
            ImageQuality = Atlas.DocServer.WCF.Implementation.Utils.DocUtils.ToDxJpegQuality(renderOptions.ImageQuality),
          });
          break;

        case Enumerators.Document.FileFormat.TXT:
          report.ExportToText(result, new TextExportOptions()
          {
            QuoteStringsWithSeparators = true,
            Separator = ",",
            TextExportMode = TextExportMode.Value
          });
          break;
      }

      return result;
    }

  }
}
