using System;
using System.Diagnostics;
using System.IO;

using GdPicture12;

using Atlas.Common.Utils;
using Atlas.DocServer.Utils;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Convert
{
  class PDFOptimize_Impl
  {
    internal static byte[] Execute(ILogging log, byte[] source, bool isCompressed, string openPassword, bool flattenFields, bool removeAllBookmarks, bool removeAllSignatures, bool removeAllLinks, bool removeAllAnnotations, byte setImagesBitDepth, int jp2KQuality, DocOptions options, bool destCompress)
    {
      var methodName = "PDFOptimize";
      log.Information("{MethodName} starting- source data is {DocumentLength} bytes", methodName, source != null ? source.Length : 0);
      var sw = Stopwatch.StartNew();

      if (source == null || source.Length == 0)
      {
        return null;
      }


      if (jp2KQuality <= 0 || jp2KQuality > 512)
      {
        jp2KQuality = 16;
      }

      if (isCompressed)
      {
        source = Compression.InMemoryDecompress(source);
      }

      Stream result = null;
      try
      {
        using (var gdPdf = new GdPicturePDF())
        {
          using (var ms = new MemoryStream(source)) // GdPicturePDF requires stream to remain open...
          {
            gdPdf.LoadFromStream(ms);

            if (gdPdf.IsEncrypted() && !string.IsNullOrEmpty(openPassword))
            {
              if (gdPdf.SetPassword(openPassword) != GdPictureStatus.OK)
              {
                return null;
              }
            }

            gdPdf.EnableCompression(true);

            #region Flatten form fields
            if (flattenFields && gdPdf.GetFormFieldsCount() > 0)
            {
              gdPdf.FlattenFormFields();
            }
            #endregion

            #region Remove bookmarks
            if (removeAllBookmarks && gdPdf.GetBookMarkCount() > 0)
            {
              gdPdf.RemoveBookMarks();
            }
            #endregion

            #region Remove signatures
            if (removeAllSignatures)
            {
              var signatureCount = gdPdf.GetSignatureCount();
              if (signatureCount > 0)
              {
                for (var i = 1; i <= signatureCount; i++)
                {
                  gdPdf.RemoveSignature(i);
                }
              }
            }
            #endregion

            #region Remove annotations
            if (removeAllAnnotations)
            {
              var annotationCount = gdPdf.GetAnnotationCount();
              if (annotationCount > 0)
              {
                for (var i = 1; i < annotationCount; i++)
                {
                  gdPdf.RemoveAnnotation(i);
                }
              }
            }
            #endregion

            #region Optimize each page
            gdPdf.SetCompressionForBitonalImage(PdfCompression.PdfCompressionJPEG2000); // JBIG is better, but requires an additional license
            gdPdf.SetCompressionForColorImage(PdfCompression.PdfCompressionJPEG2000);

            var pageCount = gdPdf.GetPageCount();
            for (var currPageNo = 1; currPageNo <= pageCount; currPageNo++)
            {
              gdPdf.SelectPage(currPageNo);

              #region Remove any page links
              if (removeAllLinks && gdPdf.GetPageLinksCount() > 0)
              {
                gdPdf.RemovePageLinks();
              }
              #endregion

              #region Re-compress images on the current page
              var imageCount = gdPdf.GetPageImageCount();
              for (var pageImageNo = 0; pageImageNo < imageCount; pageImageNo++)
              {
                using (var gdImage = new GdPictureImaging())
                {
                  var imageId = gdPdf.ExtractPageImage(pageImageNo + 1); // *1*..imagecount
                  if (imageId != 0)
                  {
                    var imageResName = gdPdf.GetPageImageResName(pageImageNo); // 0...pageimagecount
                    if (!string.IsNullOrEmpty(imageResName))
                    {
                      if (setImagesBitDepth > 0)
                      {
                        var bitDepth = gdImage.GetBitDepth(imageId);

                        switch (setImagesBitDepth)
                        {
                          case 1:
                            if (bitDepth > 1)
                            {
                              gdImage.ConvertTo1Bpp(imageId);
                            }
                            break;

                          case 8:
                            if (bitDepth > 8)
                            {
                              gdImage.ConvertTo8BppGrayScaleAdv(imageId);
                            }
                            break;

                          case 24:
                            if (bitDepth > 24)
                            {
                              gdImage.ConvertTo24BppRGB(imageId);
                            }
                            break;
                        }
                      }

                      gdPdf.ReplaceImage(imageResName, imageId, false, true);
                      gdPdf.SetJpeg2000Quality(jp2KQuality);
                    }

                    gdImage.ReleaseGdPictureImage(imageId);
                  }
                }
              }
              #endregion
            }
            #endregion

            using (var resultStream = new MemoryStream())
            {
              options = Utils.DocUtils.DocOptionsGetNotNull(options);
              gdPdf.SaveToStream(resultStream, !string.IsNullOrEmpty(options.UserPassword) || !string.IsNullOrEmpty(options.OwnerPassword) ?
                PdfEncryption.PdfEncryption128BitAES : PdfEncryption.PdfEncryptionNone,
                options.UserPassword, options.OwnerPassword, options.CanPrint, options.CanCopy, options.CanModify,
                options.CanAddNotes, options.CanFillFields, options.CanCopyAccess, options.CanAssemble, options.CanPrintFull);

              // Only return a result if our new PDF is smaller
              if (resultStream.Length < source.Length)
              {
                result = resultStream;
              }
            }

            gdPdf.CloseDocument();
          }
        }
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
      }

      if (destCompress && result != null && result.Length > 0)
      {
        result = Compression.InMemoryCompress(result);
      }

      log.Information("{MethodName} completed: took {ElapsedMS}ms, generated {ResultLength} bytes", methodName,
          sw.Elapsed.TotalMilliseconds, result != null ? result.Length : 0);

      return result.StreamToByte();
    }
  }
}
