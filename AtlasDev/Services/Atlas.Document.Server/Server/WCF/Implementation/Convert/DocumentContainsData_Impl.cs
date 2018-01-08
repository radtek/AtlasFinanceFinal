using System;
using System.Diagnostics;
using System.IO;

using AutoMapper;
using DevExpress.Spreadsheet;
using DevExpress.XtraRichEdit;
using GdPicture12;

using Atlas.Common.Utils;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Convert
{
  class DocumentContainsData_Impl
  {
    internal static bool Execute(ILogging log, byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat, string openPassword)
    {
      var methodName = "DocumentContainsData";
      log.Information("{MethodName} starting- source data is {DocumentLength} bytes", methodName, source != null ? source.Length : 0);
      var sw = Stopwatch.StartNew();

      if (source == null || source.Length == 0)
      {
        return false;
      }

      var result = false;

      if (isCompressed)
      {
        source = Compression.InMemoryDecompress(source);
      }

      var sourceFileFormatEnum = Mapper.Map<Enumerators.Document.FileFormat>(sourceFileFormat);
      try
      {
        switch (sourceFileFormat)
        {
          case FileFormatEnums.FormatType.BMP:
          case FileFormatEnums.FormatType.EMF:
          case FileFormatEnums.FormatType.JPEG:
          case FileFormatEnums.FormatType.JPEG2K:
          case FileFormatEnums.FormatType.PNG:
          case FileFormatEnums.FormatType.TIFF:
            var fileFormatEnum = Mapper.Map<Enumerators.Document.FileFormat>(sourceFileFormat);
            using (var gdPicture = new GdPictureImaging())
            {
              var imageId = gdPicture.CreateGdPictureImageFromByteArray(source, Utils.DocUtils.ToGdDocumentFormat(fileFormatEnum));
              result = imageId != 0 && !gdPicture.IsBlank(imageId);
              if (imageId > 0)
              {
                gdPicture.ReleaseGdPictureImage(imageId);
              }
            }
            break;

          case FileFormatEnums.FormatType.PDF:
            using (var gdPDF = new GdPicturePDF())
            {
              using (var ms = new MemoryStream(source))
              {
                if (gdPDF.LoadFromStream(ms) == GdPictureStatus.OK)
                {
                  if (gdPDF.IsEncrypted() && !string.IsNullOrEmpty(openPassword))
                  {
                    gdPDF.SetPassword(openPassword);
                  }

                  // Try find image or data on each page
                  var pageCount = gdPDF.GetPageCount();
                  if (pageCount > 0)
                  {
                    var currPage = 0;
                    while (!result && ++currPage <= pageCount)
                    {
                      gdPDF.SelectPage(currPage);
                      var text = gdPDF.GetPageText();
                      var imageCount = gdPDF.GetPageImageCount();
                      result = !string.IsNullOrEmpty(text) || imageCount > 0;
                    }
                  }

                  gdPDF.CloseDocument();
                }
              }
            }

            break;

          case FileFormatEnums.FormatType.DOC:
          case FileFormatEnums.FormatType.DOCX:
          case FileFormatEnums.FormatType.RTF:
          case FileFormatEnums.FormatType.HTML:
          case FileFormatEnums.FormatType.MHT:
          case FileFormatEnums.FormatType.ODT:
            using (var rtfServer = new RichEditDocumentServer())
            {
              using (var ms = new MemoryStream(source))
              {
                rtfServer.LoadDocument(ms, Utils.DocUtils.ToDocumentFormat(sourceFileFormatEnum));
                result = !string.IsNullOrEmpty(rtfServer.Document.Text);
                if (!result)
                {
                  // Count images
                  result = rtfServer.Document.Images.Count > 0;
                }
              }
            }

            break;

          case FileFormatEnums.FormatType.XLS:
          case FileFormatEnums.FormatType.XLSX:
            using (var workbook = new Workbook())
            {
              using (var ms = new MemoryStream(source))
              {
                workbook.LoadDocument(ms, Utils.DocUtils.ToSpreadSheetFormat(sourceFileFormatEnum));
                using (var text = new MemoryStream())
                {
                  workbook.SaveDocument(text, DevExpress.Spreadsheet.DocumentFormat.Text);
                  result = text.Length > 0;
                }
              }
            }

            break;

          default:
            log.Warning("No support to check file type: {0}", sourceFileFormat);
            break;
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
      }

      log.Information("{MethodName} completed: took {ElapsedMS}ms, Result: {ContainsData}", methodName, sw.Elapsed.TotalMilliseconds, result);

      return result;
    }
  }
}