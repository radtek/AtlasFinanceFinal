using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Text;

using AutoMapper;
using DevExpress.Office;
using DevExpress.Snap;
using DevExpress.Spreadsheet;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Import;
using GdPicture12;

using Atlas.Common.Utils;
using Atlas.DocServer.Utils;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Convert
{
  class Convert_Impl
  {
    internal static byte[] Execute(ILogging log, byte[] source, bool isCompressed,
      FileFormatEnums.FormatType sourceFileFormat, FileFormatEnums.FormatType destFileFormat,
      string openPassword, DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      Stream result = new MemoryStream();
      log.Information("Convert starting- {0} -> {1}, source data is {2:N0} bytes", sourceFileFormat, destFileFormat, source != null ? source.Length : 0);
      var sw = Stopwatch.StartNew();

      try
      {
        try
        {
          #region Check parameters
          if (sourceFileFormat == destFileFormat)
          {
            return source;
          }
          if (source == null || source.Length == 0 || sourceFileFormat == FileFormatEnums.FormatType.NotSet || destFileFormat == FileFormatEnums.FormatType.NotSet)
          {
            return null;
          }

          if (openPassword == null)
          {
            openPassword = "";
          }
          #endregion

          if (isCompressed)
          {
            source = Compression.InMemoryDecompress(source);
          }

          var sourceFileFormatEnum = Mapper.Map<Enumerators.Document.FileFormat>(sourceFileFormat);
          var destFileFormatEnum = Mapper.Map<Enumerators.Document.FileFormat>(destFileFormat);

          #region Source is textual based (HTML/MHT/TXT/RTF/DOC/DOCX/ODT): Use DevExpress RichEditDocumentServer to convert
          if (sourceFileFormat == FileFormatEnums.FormatType.DOC ||
            sourceFileFormat == FileFormatEnums.FormatType.DOCX ||
            sourceFileFormat == FileFormatEnums.FormatType.HTML ||
            sourceFileFormat == FileFormatEnums.FormatType.MHT ||
            sourceFileFormat == FileFormatEnums.FormatType.TXT ||
            sourceFileFormat == FileFormatEnums.FormatType.RTF ||
            sourceFileFormat == FileFormatEnums.FormatType.ODT)
          {
            using (var rtfServer = new RichEditDocumentServer())
            {
              rtfServer.Unit = DocumentUnit.Millimeter;

              #region Load source document
              rtfServer.BeginUpdate();
              try
              {
                Utils.RtfUtils.LoadDocument(rtfServer, source, sourceFileFormatEnum);

                // Only change margins if the file format does not contain margin information
                if (sourceFileFormat != FileFormatEnums.FormatType.DOC && sourceFileFormat != FileFormatEnums.FormatType.DOCX &&
                  sourceFileFormat != FileFormatEnums.FormatType.ODT && sourceFileFormat != FileFormatEnums.FormatType.RTF &&
                  sourceFileFormat != FileFormatEnums.FormatType.SNX)
                {
                  foreach (var section in rtfServer.Document.Sections)
                  {
                    section.Page.PaperKind = PaperKind.A4;
                    section.Page.Landscape = renderOptions.IsLandscape;
                    section.Margins.Left = renderOptions.LeftMarginMM;
                    section.Margins.Right = renderOptions.RightMarginMM;
                    section.Margins.Top = renderOptions.TopMarginMM;
                    section.Margins.Bottom = renderOptions.BottomMarginMM;
                  }
                }
              }
              finally
              {
                rtfServer.EndUpdate();
              }
              #endregion

              #region Produce output
              switch (destFileFormat)
              {
                case FileFormatEnums.FormatType.RTF:
                  result = new MemoryStream(Encoding.UTF8.GetBytes(rtfServer.RtfText));
                  break;

                case FileFormatEnums.FormatType.MHT:
                  result = new MemoryStream(Encoding.UTF8.GetBytes(rtfServer.MhtText));
                  break;

                case FileFormatEnums.FormatType.HTML:
                  result = new MemoryStream(Encoding.UTF8.GetBytes(rtfServer.HtmlText));
                  break;

                case FileFormatEnums.FormatType.PDF:
                  var pdfExportOptions = new PdfExportOptions()
                  {
                    Compressed = true,
                    ShowPrintDialogOnOpen = false,
                    ImageQuality = Utils.DocUtils.ToDxJpegQuality(renderOptions.ImageQuality),
                    ConvertImagesToJpeg = true
                  };

                  rtfServer.ExportToPdf(result, pdfExportOptions);
                  result = Utils.PdfUtils.SetPDFProperties(result, "", docOptions);

                  break;

                case FileFormatEnums.FormatType.ODT:
                  using (var ms = new MemoryStream())
                  {
                    rtfServer.SaveDocument(result, DevExpress.XtraRichEdit.DocumentFormat.OpenDocument);
                  }
                  break;

                case FileFormatEnums.FormatType.TXT:
                  result = new MemoryStream(Encoding.ASCII.GetBytes(rtfServer.Text));
                  break;

                case FileFormatEnums.FormatType.DOC:
                  rtfServer.SaveDocument(result, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                  break;

                case FileFormatEnums.FormatType.DOCX:
                  rtfServer.SaveDocument(result, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                  break;

                case FileFormatEnums.FormatType.PRNX:
                  using (var printingSystem = new PrintingSystem())
                  {
                    using (var printableComponentLink = new PrintableComponentLink())
                    {
                      // Add the link to the printing system's collection of links.
                      printingSystem.Links.AddRange(new object[] { printableComponentLink });
                      // Keep non-visible
                      printingSystem.ShowPrintStatusDialog = false;
                      printingSystem.ShowMarginsWarning = false;
                      printingSystem.ExportOptions.NativeFormat.Compressed = true;
                      printingSystem.ExportOptions.NativeFormat.ShowOptionsBeforeSave = false;

                      // Assign a control to be printed by this link.
                      printableComponentLink.Component = rtfServer;

                      // Create PRNX from source
                      printableComponentLink.CreateDocument(printingSystem);
                      printingSystem.SaveDocument(result);
                    }
                  }
                  break;

                case FileFormatEnums.FormatType.PNG:
                case FileFormatEnums.FormatType.TIFF:
                case FileFormatEnums.FormatType.BMP:
                case FileFormatEnums.FormatType.JPEG:
                  var printSys = new PrintingSystem();
                  var link = new PrintableComponentLink(printSys) { Component = rtfServer, PaperKind = PaperKind.A4, Landscape = renderOptions.IsLandscape };
                  link.Margins.Left = renderOptions.LeftMarginMM;
                  link.Margins.Right = renderOptions.RightMarginMM;
                  link.Margins.Top = renderOptions.TopMarginMM;
                  link.Margins.Bottom = renderOptions.BottomMarginMM;
                  link.PrintingSystem.Graph.BackColor = Color.White;
                  rtfServer.AddService(typeof(IPrintable), new Atlas.DocServer.WCF.Implementation.Utils.CustomRichEditPrinter(rtfServer));
                  link.CreateDocument();

                  if (destFileFormat == FileFormatEnums.FormatType.TIFF)
                  {
                    // Export the whole document
                    link.ExportToImage(result, new ImageExportOptions
                    {
                      Format = ImageFormat.Tiff,
                      ExportMode = ImageExportMode.SingleFilePageByPage,
                      Resolution = renderOptions.DPI > 0 && renderOptions.DPI < 600 ? renderOptions.DPI : 128
                    });
                  }
                  else
                  {
                    var exportOptions = new ImageExportOptions()
                    {
                      ExportMode = ImageExportMode.SingleFilePageByPage,
                      Resolution = renderOptions.DPI > 0 && renderOptions.DPI < 600 ? renderOptions.DPI : 128,
                      PageRange = "1",
                      Format = Utils.DocUtils.ToImagingFormat(destFileFormatEnum)
                    };

                    // Export only the first page
                    link.ExportToImage(result, exportOptions);
                  }
                  break;

                default:
                  return null;
              }
              #endregion
            }
          }
          #endregion

          #region Source is Snap document (SNX)
          else if (sourceFileFormat == FileFormatEnums.FormatType.SNX)
          {
            using (var snapServer = new SnapDocumentServer())
            {
              Utils.SnapUtils.LoadTemplate(snapServer, source, sourceFileFormatEnum);

              #region Produce output
              switch (destFileFormat)
              {
                case FileFormatEnums.FormatType.RTF:
                  result = new MemoryStream(Encoding.UTF8.GetBytes(snapServer.RtfText));
                  break;

                case FileFormatEnums.FormatType.MHT:
                  result = new MemoryStream(Encoding.UTF8.GetBytes(snapServer.MhtText));
                  break;

                case FileFormatEnums.FormatType.HTML:
                  result = new MemoryStream(Encoding.UTF8.GetBytes(snapServer.HtmlText));
                  break;

                case FileFormatEnums.FormatType.PDF:
                  var pdfExportOptions = new PdfExportOptions()
                  {
                    Compressed = true,
                    ShowPrintDialogOnOpen = false,
                    ImageQuality = Utils.DocUtils.ToDxJpegQuality(renderOptions.ImageQuality),
                    ConvertImagesToJpeg = true
                  };

                  snapServer.ExportToPdf(result, pdfExportOptions);
                  result = Utils.PdfUtils.SetPDFProperties(result, "", docOptions);

                  break;

                case FileFormatEnums.FormatType.ODT:
                  using (var ms = new MemoryStream())
                  {
                    snapServer.ExportDocument(result, DevExpress.XtraRichEdit.DocumentFormat.OpenDocument);
                  }
                  break;

                case FileFormatEnums.FormatType.TXT:
                  result = new MemoryStream(Encoding.ASCII.GetBytes(snapServer.Text));
                  break;

                case FileFormatEnums.FormatType.DOC:
                  snapServer.ExportDocument(result, DevExpress.XtraRichEdit.DocumentFormat.Doc);
                  break;

                case FileFormatEnums.FormatType.DOCX:
                  snapServer.ExportDocument(result, DevExpress.XtraRichEdit.DocumentFormat.OpenXml);
                  break;

                case FileFormatEnums.FormatType.PRNX:
                  using (var printingSystem = new PrintingSystem())
                  {
                    using (var printableComponentLink = new PrintableComponentLink())
                    {
                      // Add the link to the printing system's collection of links.
                      printingSystem.Links.AddRange(new object[] { printableComponentLink });
                      // Keep non-visible
                      printingSystem.ShowPrintStatusDialog = false;
                      printingSystem.ShowMarginsWarning = false;
                      printingSystem.ExportOptions.NativeFormat.Compressed = true;
                      printingSystem.ExportOptions.NativeFormat.ShowOptionsBeforeSave = false;

                      // Assign a control to be printed by this link.
                      printableComponentLink.Component = snapServer;

                      // Create PRNX from source
                      printableComponentLink.CreateDocument(printingSystem);
                      printingSystem.SaveDocument(result);
                    }
                  }
                  break;

                case FileFormatEnums.FormatType.PNG:
                case FileFormatEnums.FormatType.TIFF:
                case FileFormatEnums.FormatType.BMP:
                case FileFormatEnums.FormatType.JPEG:
                  var printSys = new PrintingSystem();
                  var link = new PrintableComponentLink(printSys) { Component = snapServer, PaperKind = PaperKind.A4, Landscape = renderOptions.IsLandscape };
                  link.Margins.Left = renderOptions.LeftMarginMM;
                  link.Margins.Right = renderOptions.RightMarginMM;
                  link.Margins.Top = renderOptions.TopMarginMM;
                  link.Margins.Bottom = renderOptions.BottomMarginMM;
                  link.PrintingSystem.Graph.BackColor = Color.White;
                  snapServer.AddService(typeof(IPrintable), new Atlas.DocServer.WCF.Implementation.Utils.CustomRichEditPrinter(snapServer));
                  link.CreateDocument();

                  if (destFileFormat == FileFormatEnums.FormatType.TIFF)
                  {
                    // Export the whole document
                    link.ExportToImage(result, new ImageExportOptions
                    {
                      Format = ImageFormat.Tiff,
                      ExportMode = ImageExportMode.SingleFilePageByPage,
                      Resolution = renderOptions.DPI > 0 && renderOptions.DPI < 600 ? renderOptions.DPI : 128
                    });
                  }
                  else
                  {
                    var exportOptions = new ImageExportOptions()
                    {
                      ExportMode = ImageExportMode.SingleFilePageByPage,
                      Resolution = renderOptions.DPI > 0 && renderOptions.DPI < 600 ? renderOptions.DPI : 128,
                      PageRange = "1",
                      Format = Utils.DocUtils.ToImagingFormat(destFileFormatEnum)
                    };

                    // Export only the first page
                    link.ExportToImage(result, exportOptions);
                  }
                  break;

                default:
                  return null;
              }
              #endregion
            }
          }
          #endregion

          #region Source is PRNX
          else if (sourceFileFormat == FileFormatEnums.FormatType.PRNX)
          {
            using (var printing = new PrintingSystem())
            using (var ms = new MemoryStream(source))
            {
              printing.LoadDocument(ms);

              #region Produce output
              switch (destFileFormat)
              {
                case FileFormatEnums.FormatType.DOC:
                case FileFormatEnums.FormatType.DOCX:
                case FileFormatEnums.FormatType.RTF:
                  printing.ExportToRtf(result);
                  break;

                case FileFormatEnums.FormatType.MHT:
                  printing.ExportToMht(result);
                  break;

                case FileFormatEnums.FormatType.HTML:
                  printing.ExportToHtml(result);
                  break;

                case FileFormatEnums.FormatType.PDF:
                  var pdfExportOptions = new PdfExportOptions()
                  {
                    Compressed = true,
                    ShowPrintDialogOnOpen = false,
                    ImageQuality = Utils.DocUtils.ToDxJpegQuality(renderOptions.ImageQuality),
                    ConvertImagesToJpeg = true
                  };

                  printing.ExportToPdf(result, pdfExportOptions);
                  result = Utils.PdfUtils.SetPDFProperties(result, "", docOptions);

                  break;

                case FileFormatEnums.FormatType.CSV:
                  printing.ExportToCsv(result, new CsvExportOptions { Separator = ",", QuoteStringsWithSeparators = true });
                  break;

                case FileFormatEnums.FormatType.TXT:
                  printing.ExportToText(result);
                  break;

                case FileFormatEnums.FormatType.XLS:
                  printing.ExportToXls(result, new XlsExportOptions { ExportMode = XlsExportMode.SingleFile, SheetName = !string.IsNullOrEmpty(docOptions.Subject) ? docOptions.Subject : "Sheet" });
                  break;

                case FileFormatEnums.FormatType.XLSX:
                  printing.ExportToXlsx(result, new XlsxExportOptions { ExportMode = XlsxExportMode.SingleFile, SheetName = !string.IsNullOrEmpty(docOptions.Subject) ? docOptions.Subject : "Sheet" });
                  break;

                case FileFormatEnums.FormatType.PNG:
                case FileFormatEnums.FormatType.TIFF:
                case FileFormatEnums.FormatType.BMP:
                case FileFormatEnums.FormatType.JPEG:
                  printing.ExportToImage(result, destFileFormat == FileFormatEnums.FormatType.TIFF ?
                    new ImageExportOptions
                    {
                      Format = ImageFormat.Tiff,
                      ExportMode = ImageExportMode.SingleFilePageByPage,
                      Resolution = renderOptions.DPI > 0 && renderOptions.DPI < 600 ? renderOptions.DPI : 128
                    } :

                      new ImageExportOptions()
                      {
                        ExportMode = ImageExportMode.SingleFilePageByPage,
                        Resolution = renderOptions.DPI > 0 && renderOptions.DPI < 600 ? renderOptions.DPI : 128,
                        PageRange = "1",
                        Format = Utils.DocUtils.ToImagingFormat(destFileFormatEnum)
                      });
                  break;

                default:
                  return null;
              }
              #endregion
            }
          }
          #endregion

          #region Source is Excel (XLS/XLSX)- use DevExpress Spreadsheet server
          else if (sourceFileFormat == FileFormatEnums.FormatType.XLS ||
            sourceFileFormat == FileFormatEnums.FormatType.XLSX)
          {
            using (var wb = new Workbook())
            {
              using (var ms = new MemoryStream(source))
              {
                wb.LoadDocument(ms, Utils.DocUtils.ToSpreadSheetFormat(sourceFileFormatEnum));
              }

              wb.Unit = DocumentUnit.Millimeter;

              switch (destFileFormat)
              {
                case FileFormatEnums.FormatType.XLSX:
                case FileFormatEnums.FormatType.XLS:
                  if (!string.IsNullOrEmpty(docOptions.OwnerPassword))
                  {
                    var wsCount = wb.Worksheets.Count;
                    for (var i = 0; i < wsCount; i++)
                    {
                      wb.Worksheets[i].Protect(docOptions.OwnerPassword, WorksheetProtectionPermissions.Default);
                    }
                    wb.Protect(docOptions.OwnerPassword, true, false);
                  }

                  wb.SaveDocument(result, Utils.DocUtils.ToSpreadSheetFormat(destFileFormatEnum));

                  break;

                case FileFormatEnums.FormatType.PDF:
                  var ws1Count = wb.Worksheets.Count;
                  for (var i = 0; i < ws1Count; i++)
                  {
                    wb.Worksheets[i].PrintOptions.FitToPage = true;
                    wb.Worksheets[i].PrintOptions.FitToWidth = 1;
                    wb.Worksheets[i].PrintOptions.FitToHeight = 0;

                    wb.Worksheets[i].ActiveView.PaperKind = PaperKind.A4;
                    wb.Worksheets[i].ActiveView.Orientation = renderOptions.IsLandscape ? PageOrientation.Landscape : PageOrientation.Portrait;

                    if (renderOptions.LeftMarginMM > 0)
                    {
                      wb.Worksheets[i].ActiveView.Margins.Left = renderOptions.LeftMarginMM;
                    }
                    if (renderOptions.RightMarginMM > 0)
                    {
                      wb.Worksheets[i].ActiveView.Margins.Right = renderOptions.RightMarginMM;
                    }
                    if (renderOptions.TopMarginMM > 0)
                    {
                      wb.Worksheets[i].ActiveView.Margins.Top = renderOptions.TopMarginMM;
                    }
                    if (renderOptions.BottomMarginMM > 0)
                    {
                      wb.Worksheets[i].ActiveView.Margins.Bottom = renderOptions.BottomMarginMM;
                    }

                    wb.Worksheets[i].PrintOptions.PrintGridlines = false;
                  }

                  wb.ExportToPdf(result);
                  result = Utils.PdfUtils.SetPDFProperties(result, "", docOptions);

                  break;

                case FileFormatEnums.FormatType.CSV:
                  wb.Options.Export.Csv.FormulaExportMode = FormulaExportMode.CalculatedValue;
                  wb.Options.Export.Csv.NewlineAfterLastRow = false;
                  wb.Options.Export.Csv.NewlineType = NewlineType.CrLf;
                  wb.Options.Export.Csv.TextQualifier = '"';
                  wb.Options.Export.Csv.UseCellNumberFormat = true;
                  wb.Options.Export.Csv.ValueSeparator = ',';

                  wb.SaveDocument(result, DevExpress.Spreadsheet.DocumentFormat.Csv);

                  break;

                case FileFormatEnums.FormatType.TXT:
                  wb.Options.Export.Txt.FormulaExportMode = FormulaExportMode.CalculatedValue;
                  wb.Options.Export.Txt.NewlineAfterLastRow = false;
                  wb.Options.Export.Txt.NewlineType = NewlineType.CrLf;
                  wb.Options.Export.Txt.TextQualifier = '"';
                  wb.Options.Export.Txt.UseCellNumberFormat = true;

                  wb.SaveDocument(result, DevExpress.Spreadsheet.DocumentFormat.Text);

                  break;

                case FileFormatEnums.FormatType.ODS:
                  return null; // not supported by DX
              }
            }
          }
          #endregion

          #region Source is TIFF and not PDF output- output only the first page
          else if (sourceFileFormat == FileFormatEnums.FormatType.TIFF && destFileFormat != FileFormatEnums.FormatType.PDF)
          {
            using (var gdImage = new GdPictureImaging())
            {
              var imageNum = gdImage.CreateGdPictureImageFromByteArray(source, GdPicture12.DocumentFormat.DocumentFormatTIFF);
              if (imageNum == 0)
              {
                return null;
              }

              gdImage.TiffSelectPage(imageNum, 1);
              var clonedId = gdImage.CreateClonedGdPictureImage(imageNum);
              if (clonedId == 0)
              {
                gdImage.ReleaseGdPictureImage(imageNum);
                return null;
              }
              var docFormat = Utils.DocUtils.ToGdDocumentFormat(destFileFormatEnum);
              if (docFormat == GdPicture12.DocumentFormat.DocumentFormatUNKNOWN)
              {
                gdImage.ReleaseGdPictureImage(imageNum);
                gdImage.ReleaseGdPictureImage(clonedId);
                return null;
              }

              var encoderParam = 0;
              switch (destFileFormat)
              {
                case FileFormatEnums.FormatType.JPEG:
                  encoderParam = renderOptions.ImageQuality > 0 && renderOptions.ImageQuality <= 100 ? renderOptions.ImageQuality : 95;// Quality- 1 (lower) and 100 (higher)
                  break;

                case FileFormatEnums.FormatType.JPEG2K:
                  encoderParam = renderOptions.ImageQuality > 0 && renderOptions.ImageQuality <= 512 ? renderOptions.ImageQuality : 16;// [1(MaxQuality - Lossless) ... 512(Poor quality)].
                  break;

                case FileFormatEnums.FormatType.PNG:
                  encoderParam = renderOptions.ImageQuality >= 0 && renderOptions.ImageQuality <= 9 ? renderOptions.ImageQuality : 6; //  [0(no compression - faster encoding) ... 9(max compression - slower encoding)].
                  break;

                case FileFormatEnums.FormatType.TIFF:
                  encoderParam = 65536;// 65536- Automatic compression
                  break;

                case FileFormatEnums.FormatType.BMP:
                  break;

                default:
                  return null;
              }

              gdImage.SaveAsStream(clonedId, result, docFormat, encoderParam);

              gdImage.ReleaseGdPictureImage(clonedId);
              gdImage.ReleaseGdPictureImage(imageNum);
            }
          }
          #endregion

          #region Pure graphic conversion- GdPicture
          else if (sourceFileFormat != FileFormatEnums.FormatType.PDF && destFileFormat != FileFormatEnums.FormatType.PDF) // graphic related- use GdPicture
          {
            var sourceFormat = Utils.DocUtils.ToGdDocumentFormat(sourceFileFormatEnum);
            if (sourceFormat == GdPicture12.DocumentFormat.DocumentFormatUNKNOWN)
            {
              return null;
            }
            var destFormat = Utils.DocUtils.ToGdDocumentFormat(destFileFormatEnum);
            if (destFormat == GdPicture12.DocumentFormat.DocumentFormatUNKNOWN)
            {
              return null;
            }

            using (var gdPicture = new GdPictureImaging())
            {
              var imageId = gdPicture.CreateGdPictureImageFromByteArray(source, sourceFormat);
              if (imageId == 0)
              {
                return null;
              }

              switch (destFileFormat)
              {
                case FileFormatEnums.FormatType.JPEG:
                  gdPicture.SaveAsStream(imageId, result, GdPicture12.DocumentFormat.DocumentFormatJPEG,
                    renderOptions.ImageQuality > 0 && renderOptions.ImageQuality <= 100 ? renderOptions.ImageQuality : 95);// Quality- 1 (lower) and 100 (higher)
                  break;

                case FileFormatEnums.FormatType.JPEG2K:
                  gdPicture.SaveAsStream(imageId, result, GdPicture12.DocumentFormat.DocumentFormatJP2,
                    renderOptions.ImageQuality > 0 && renderOptions.ImageQuality <= 512 ? renderOptions.ImageQuality : 16);// [1(MaxQuality - Lossless) ... 512(Poor quality)].
                  break;

                case FileFormatEnums.FormatType.PNG:
                  gdPicture.SaveAsStream(imageId, result, GdPicture12.DocumentFormat.DocumentFormatPNG,
                    renderOptions.ImageQuality >= 0 && renderOptions.ImageQuality <= 9 ? renderOptions.ImageQuality : 6); //  [0(no compression - faster encoding) ... 9(max compression - slower encoding)].
                  break;

                case FileFormatEnums.FormatType.TIFF:
                  gdPicture.SaveAsTIFF(imageId, result, false, TiffCompression.TiffCompressionAUTO,
                    renderOptions.ImageQuality > 0 && renderOptions.ImageQuality <= 100 ? renderOptions.ImageQuality : 95);// Quality- 1 (lower) and 100 (higher);                  
                  break;

                default:
                  return null;
              }

              gdPicture.ReleaseGdPictureImage(imageId);
            }
          }
          #endregion

          #region Source is PDF- use GdPicture
          else if (sourceFileFormat == FileFormatEnums.FormatType.PDF)
          {
            result = Utils.PdfUtils.ExportPdfTo(source, openPassword, renderOptions, destFileFormatEnum);
          }
          #endregion

          #region Destination is PDF and source is graphical- use GdPicture
          else if (destFileFormat == FileFormatEnums.FormatType.PDF)
          {
            using (var ms = new MemoryStream(source))
            {
              result = Utils.GdPictureUtils.ImageToPdf(ms, sourceFileFormatEnum, docOptions,
                string.IsNullOrEmpty(docOptions.OwnerPassword) && string.IsNullOrEmpty(docOptions.UserPassword));
            }
          }
          #endregion

          #region Source is DevExpress PRNX (Report Document)
          else if (sourceFileFormat == FileFormatEnums.FormatType.PRNX)
          {
            using (var ps = new PrintingSystem())
            {
              using (var ms = new MemoryStream(source))
              {
                ps.LoadDocument(ms);

                switch (destFileFormat)
                {
                  case FileFormatEnums.FormatType.BMP:
                  case FileFormatEnums.FormatType.EMF:
                  case FileFormatEnums.FormatType.JPEG:
                  case FileFormatEnums.FormatType.PNG:
                    ps.ExportToImage(result, new ImageExportOptions()
                    {
                      Format = Utils.DocUtils.ToImagingFormat(sourceFileFormatEnum),
                      ExportMode = ImageExportMode.SingleFile,
                      PageRange = "1",
                      PageBorderColor = Color.White,
                      Resolution = renderOptions.DPI > 0 && renderOptions.DPI < 600 ? renderOptions.DPI : 128
                    });

                    break;

                  case FileFormatEnums.FormatType.TIFF:
                    ps.ExportToImage(result, new ImageExportOptions()
                    {
                      Format = ImageFormat.Tiff,
                      ExportMode = ImageExportMode.SingleFile,
                      PageBorderColor = Color.White,
                      Resolution = renderOptions.DPI > 0 && renderOptions.DPI < 600 ? renderOptions.DPI : 128
                    });

                    break;

                  case FileFormatEnums.FormatType.CSV:
                    ps.ExportToCsv(result, new CsvExportOptions()
                    {
                      Separator = ",",
                      QuoteStringsWithSeparators = true,
                      TextExportMode = TextExportMode.Value
                    });

                    break;

                  case FileFormatEnums.FormatType.XLSX:
                    ps.ExportToXlsx(result, new XlsxExportOptions()
                    {
                      ExportHyperlinks = true,
                      ExportMode = XlsxExportMode.SingleFile,
                      ShowGridLines = false,
                      SheetName = docOptions.Subject ?? Utils.DocConsts.STR_AtlasDocumentGenerator
                    });

                    break;

                  case FileFormatEnums.FormatType.XLS:
                    ps.ExportToXls(result, new XlsExportOptions()
                    {
                      ExportHyperlinks = true,
                      ExportMode = XlsExportMode.SingleFile,
                      ShowGridLines = false,
                      SheetName = docOptions.Subject ?? Utils.DocConsts.STR_AtlasDocumentGenerator,
                    });

                    break;

                  case FileFormatEnums.FormatType.RTF:
                  case FileFormatEnums.FormatType.DOC:
                  case FileFormatEnums.FormatType.DOCX:
                    ps.ExportToRtf(result, new RtfExportOptions()
                    {
                      ExportMode = RtfExportMode.SingleFile,
                      ExportWatermarks = true
                    });

                    break;

                  case FileFormatEnums.FormatType.PDF:
                    ps.ExportToPdf(result, new PdfExportOptions()
                    {
                      Compressed = true,
                      ConvertImagesToJpeg = true,
                      ImageQuality = Utils.DocUtils.ToDxJpegQuality(renderOptions.ImageQuality)
                    });

                    // Remove DX doc info
                    result = Utils.PdfUtils.SetPDFProperties(result, "", docOptions);

                    break;

                  case FileFormatEnums.FormatType.TXT:
                    ps.ExportToText(result, new TextExportOptions()
                    {
                      QuoteStringsWithSeparators = true,
                      Separator = ",",
                      TextExportMode = TextExportMode.Value
                    });

                    break;
                }
              }
            }
          }
          #endregion

          #region No support
          else
          {
            log.Warning("No support to convert file type: {0} to {1}", sourceFileFormat, destFileFormat);
          }
          #endregion
        }
        catch (Exception err)
        {
          log.Error("{0}", err);
        }
      }
      finally
      {
        log.Information("Convert completed: took {0:N2}ms, generated {1:N0} bytes",
          sw.Elapsed.TotalMilliseconds, result != null ? result.Length : 0);
      }

      if (destCompress && result != null && result.Length > 0)
      {
        return Compression.InMemoryCompress(result).StreamToByte();
      }

      return result.StreamToByte();
    }
  }
}
