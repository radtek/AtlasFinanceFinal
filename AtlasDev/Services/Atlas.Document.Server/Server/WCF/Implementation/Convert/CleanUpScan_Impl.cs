using System;
using System.IO;
using System.Diagnostics;

using GdPicture12;
using AutoMapper;

using Atlas.DocServer.WCF.Interface;
using Atlas.DocServer.Utils;
using Atlas.Common.Utils;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Convert
{
  class CleanUpScan_Impl
  {
    internal static byte[] Execute(ILogging log, byte[] source, bool isCompressed, FileFormatEnums.FormatType sourceFileFormat,
      string openPassword, CleanUpOptions cleanUpOptions, DocOptions docOptions, RenderOptions renderOptions,
      FileFormatEnums.FormatType destFileFormat, bool destCompress)
    {
      var methodName = "CleanUpScan";
      log.Information("{MethodName} starting- source data is {DocumentLength} bytes", methodName, source != null ? source.Length : 0);
      var sw = Stopwatch.StartNew();

      #region Check parameters
      if (source == null || source.Length == 0 || sourceFileFormat == FileFormatEnums.FormatType.NotSet || cleanUpOptions == null)
      {
        return null;
      }

      if (isCompressed)
      {
        source = Compression.InMemoryDecompress(source);
      }

      // We can only clean bitmaps/PDF
      if (!(sourceFileFormat == FileFormatEnums.FormatType.BMP || sourceFileFormat == FileFormatEnums.FormatType.JPEG ||
        sourceFileFormat == FileFormatEnums.FormatType.JPEG2K || sourceFileFormat == FileFormatEnums.FormatType.PNG ||
        sourceFileFormat == FileFormatEnums.FormatType.TIFF || sourceFileFormat == FileFormatEnums.FormatType.PDF))
      {
        return source;
      }
      #endregion

      // If source is PDF, we first need to extract page images to a TIFF multipage file
      if (sourceFileFormat == FileFormatEnums.FormatType.PDF)
      {
        source = Utils.PdfUtils.ExportPdfTo(source, openPassword, new RenderOptions(), Enumerators.Document.FileFormat.TIFF).StreamToByte();
        if (source == null)
        {
          return null;
        }
        sourceFileFormat = FileFormatEnums.FormatType.TIFF;
      }

      Stream result = new MemoryStream();
      try
      {
        using (var gdPicture = new GdPictureImaging())
        {
          var fileFormatEnum = Mapper.Map<Enumerators.Document.FileFormat>(sourceFileFormat);
          var imageId = gdPicture.CreateGdPictureImageFromByteArray(source, Utils.DocUtils.ToGdDocumentFormat(fileFormatEnum));
          if (imageId > 0)
          {
            #region Perform the clean-up
            if (sourceFileFormat == FileFormatEnums.FormatType.TIFF && gdPicture.TiffIsMultiPage(imageId))
            {
              var pageCount = gdPicture.TiffGetPageCount(imageId);
              for (int page = 1; page <= pageCount; page++)
              {
                if (gdPicture.TiffSelectPage(imageId, page) == GdPictureStatus.OK)
                {
                  Utils.GdPictureUtils.GdCleanUp(gdPicture, imageId, cleanUpOptions);
                }
              }
              gdPicture.TiffSelectPage(imageId, 1);
            }
            else
            {
              Utils.GdPictureUtils.GdCleanUp(gdPicture, imageId, cleanUpOptions);
            }
            #endregion

            #region Export
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
                gdPicture.SaveAsStream(imageId, result, GdPicture12.DocumentFormat.DocumentFormatTIFF,
                  renderOptions.ImageQuality > 0 && renderOptions.ImageQuality <= 100 ? renderOptions.ImageQuality : 95);// Quality- 1 (lower) and 100 (higher);                  
                break;

              case FileFormatEnums.FormatType.PDF:
                docOptions = Utils.DocUtils.DocOptionsGetNotNull(docOptions);
                using (var temp = new MemoryStream())
                {
                  gdPicture.SaveAsStream(imageId, temp, GdPicture12.DocumentFormat.DocumentFormatTIFF,
                    renderOptions.ImageQuality > 0 && renderOptions.ImageQuality <= 100 ? renderOptions.ImageQuality : 95);// Quality- 1 (lower) and 100 (higher);                  
                  result = Utils.GdPictureUtils.ImageToPdf(temp, Enumerators.Document.FileFormat.TIFF, docOptions, 
                    string.IsNullOrEmpty(docOptions.UserPassword) && string.IsNullOrEmpty(docOptions.OwnerPassword));
                }
                break;

              default:
                return null;
            }
            #endregion

            gdPicture.ReleaseGdPictureImage(imageId);
          }
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
      }

      if (destCompress && result != null && result.Length > 0)
      {
        log.Information("{MethodName} completed: took {ElapsedMS}ms, generated {ResultLength} bytes", methodName,
          sw.Elapsed.TotalMilliseconds, result != null ? result.Length : 0);

        return Compression.InMemoryCompress(result).StreamToByte();
      }

      log.Information("{MethodName} completed: took {ElapsedMS}ms, generated {ResultLength} bytes", methodName,
          sw.Elapsed.TotalMilliseconds, result != null ? result.Length : 0);

      return result.StreamToByte();
    }
  }
}
