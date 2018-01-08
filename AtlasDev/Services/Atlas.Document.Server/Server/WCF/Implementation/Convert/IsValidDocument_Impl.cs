using System;
using System.Diagnostics;
using System.IO;

using AutoMapper;
using DevExpress.Spreadsheet;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit;
using GdPicture12;

using Atlas.Common.Utils;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Convert
{
  class IsValidDocument_Impl
  {
    internal static bool Execute(ILogging log, byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat, string openPassword)
    {
      var methodName = "IsValidDocument";
      log.Information("{MethodName} starting- source data is {DocumentLength} bytes", methodName, source != null ? source.Length : 0);
      var sw = Stopwatch.StartNew();

      var result = false;

      if (source == null || source.Length == 0 || sourceFileFormat == FileFormatEnums.FormatType.NotSet)
      {
        return false;
      }

      if (isCompressed)
      {
        source = Compression.InMemoryDecompress(source);
      }

      var sourceFileFormatEnum = Mapper.Map<Enumerators.Document.FileFormat>(sourceFileFormat);

      try
      {
        switch (sourceFileFormat)
        {
          case FileFormatEnums.FormatType.PRNX:
            using (var ps = new PrintingSystem())
            {
              using (var ms = new MemoryStream(source))
              {
                ps.LoadDocument(ms);
                result = ps.PageCount > 0;
              }
            }
            break;

          case FileFormatEnums.FormatType.DOC:
          case FileFormatEnums.FormatType.DOCX:
          case FileFormatEnums.FormatType.HTML:
          case FileFormatEnums.FormatType.MHT:
          case FileFormatEnums.FormatType.RTF:
          case FileFormatEnums.FormatType.ODT:
            using (var rtfServer = new RichEditDocumentServer())
            {
              using (var ms = new MemoryStream(source))
              {
                rtfServer.LoadDocument(ms, Utils.DocUtils.ToDocumentFormat(sourceFileFormatEnum));
                result = true;
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
                result = true;
              }
            }
            break;

          case FileFormatEnums.FormatType.BMP:
          case FileFormatEnums.FormatType.EMF:
          case FileFormatEnums.FormatType.JPEG:
          case FileFormatEnums.FormatType.JPEG2K:
          case FileFormatEnums.FormatType.PNG:
          case FileFormatEnums.FormatType.TIFF:
            using (var gdImage = new GdPictureImaging())
            {
              var docFormat = Utils.DocUtils.ToGdDocumentFormat(sourceFileFormatEnum);
              if (docFormat == GdPicture12.DocumentFormat.DocumentFormatUNKNOWN)
              {
                break;
              }

              var imageId = gdImage.CreateGdPictureImageFromByteArray(source, docFormat);
              if (imageId == 0)
              {
                break;
              }

              result = gdImage.GetWidth(imageId) > 0 && gdImage.GetHeight(imageId) > 0;
              gdImage.ReleaseGdPictureImage(imageId);
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

                  result = gdPDF.GetPageCount() > 0;

                  gdPDF.CloseDocument();
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

      log.Information("{MethodName} completed: took {ElapsedMS}ms, Result: {IsValid}", methodName, sw.Elapsed.TotalMilliseconds, result);

      return result;
    }
  }
}