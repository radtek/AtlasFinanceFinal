using System;
using System.IO;
using System.Text;

using GdPicture12;

using Atlas;
using Atlas.DocServer.WCF.Interface;


namespace Atlas.DocServer.WCF.Implementation.Utils
{
  internal static class PdfUtils
  {
    /// <summary>
    /// Adds/removes PDF security and sets document properties for an existing PDF
    /// </summary>
    /// <param name="source">The source PDF bytes</param>
    /// <param name="openPassword">Password required to open the PDF</param>   
    /// <param name="docOptions">Options for destination document</param>
    internal static Stream SetPDFProperties(Stream source, string openPassword, DocOptions docOptions)
    {
      if (docOptions == null || source == null)
      {
        return null;
      }

      Stream result = new MemoryStream();

      // Remove all DX properties from the PDF using GdPicture     
      using (var gdPDF = new GdPicturePDF())
      {
        if (gdPDF.LoadFromStream(source) == GdPictureStatus.OK)
        {
          if (gdPDF.IsEncrypted() && !string.IsNullOrEmpty(openPassword))
          {
            gdPDF.SetPassword(openPassword);
          }

          gdPDF.SetAuthor(docOptions.Author ?? DocConsts.STR_AtlasDocumentGenerator);
          gdPDF.SetCreator(docOptions.Creator ?? DocConsts.STR_AtlasDocumentGenerator);
          gdPDF.SetKeywords(docOptions.Keywords ?? DocConsts.STR_AtlasDocumentGenerator);
          gdPDF.SetProducer(docOptions.Producer ?? DocConsts.STR_AtlasDocumentGenerator);
          gdPDF.SetSubject(docOptions.Subject ?? DocConsts.STR_AtlasDocumentGenerator);
          gdPDF.SetTitle(docOptions.Title ?? DocConsts.STR_AtlasDocumentGenerator);

          if (gdPDF.SaveToStream(result, PdfEncryption.PdfEncryption128BitAES, docOptions.UserPassword ?? "", docOptions.OwnerPassword ?? "",
                docOptions.CanPrint, docOptions.CanCopy, docOptions.CanModify, docOptions.CanAddNotes, docOptions.CanFillFields,
                docOptions.CanCopyAccess, docOptions.CanAssemble, docOptions.CanPrintFull) != GdPictureStatus.OK)
          {
            result = null;
          }
        }
      }

      return result;
    }


    /// <summary>
    /// Adds/removes PDF security and sets document properties for an existing PDF
    /// </summary>
    /// <param name="source">The source PDF bytes</param>
    /// <param name="openPassword">Password required to open the PDF</param>
    /// <param name="setUserPassword">Set user password</param>  
    internal static Stream SetPDFProperties(byte[] source, string openPassword, DocOptions docOptions)
    {
      if (docOptions == null || source == null)
      {
        return null;
      }

      // Set all properties from the PDF using GdPicture
      using (Stream ms = new MemoryStream(source))
      {
        return SetPDFProperties(ms, openPassword, docOptions);
      }
    }
    

    /// <summary>
    /// Converts a PDF to an image or extracts all text
    /// </summary>
    /// <param name="source">Source PDF file</param>
    /// <param name="openSourcePassword">Password to open PDF</param>    
    /// <param name="renderOptions">Output rendering options</param>
    /// <param name="destFileFormat">Destination file format</param>
    /// <returns>a byte array containing the rendered file/extracted data</returns>
    internal static Stream ExportPdfTo(byte[] source, string openSourcePassword,
      RenderOptions renderOptions = null, Enumerators.Document.FileFormat destFileFormat = Atlas.Enumerators.Document.FileFormat.TIFF)
    {
      Stream result = new MemoryStream();

      using (var gdPDF = new GdPicturePDF())
      {
        using (var pdfMS = new MemoryStream(source))
        {
          if (gdPDF.LoadFromStream(pdfMS) != GdPictureStatus.OK)
          {
            return null;
          }

          if (gdPDF.IsEncrypted())
          {
            if (!string.IsNullOrEmpty(openSourcePassword))
            {
              gdPDF.SetPassword(openSourcePassword);
            }
            else
            {
              return null;
            }
          }

          var pageCount = gdPDF.GetPageCount();
          if (pageCount == 0)
          {
            return null;
          }

          #region Export to bitmap
          if (destFileFormat == Atlas.Enumerators.Document.FileFormat.BMP || destFileFormat == Atlas.Enumerators.Document.FileFormat.EMF ||
            destFileFormat == Atlas.Enumerators.Document.FileFormat.JPEG || destFileFormat == Atlas.Enumerators.Document.FileFormat.JPEG2K ||
            destFileFormat == Atlas.Enumerators.Document.FileFormat.PNG || destFileFormat == Atlas.Enumerators.Document.FileFormat.TIFF)
          {
            using (var gdImage = new GdPictureImaging())
            {
              if (destFileFormat == Atlas.Enumerators.Document.FileFormat.TIFF)
              {
                var status = GdPictureStatus.OK;
                var multiTiffId = 0;

                for (var i = 1; i <= pageCount; i++)
                {
                  if (status == GdPictureStatus.OK)
                  {
                    if (gdPDF.SelectPage(i) == GdPictureStatus.OK)
                    {
                      var imageId = 0;

                      if (gdPDF.IsPageImage(ref imageId, true) && imageId > 0) // Returns if the current page of the PDF is image-based. pages containing only one fully visible bitmap covering the whole page area, with no other particular drawing operation such as clipping path.
                      {                                                        // ImageID- Output parameter. If the PDF page is image - based, this parameter will return a GdPicture Image, corresponding to the bitmap embedded in the page. In the other case, this parameter returns 0.
                        if (i == 1)
                        {
                          multiTiffId = imageId;
                          status = gdImage.TiffSaveAsMultiPageFile(multiTiffId, result, TiffCompression.TiffCompressionLZW);
                        }
                        else
                        {
                          status = gdImage.TiffAddToMultiPageFile(multiTiffId, imageId);
                          gdImage.ReleaseGdPictureImage(imageId);
                        }
                      }
                      else // not a full-size image page
                      {
                        var rasterizedPageId = gdPDF.RenderPageToGdPictureImageEx(renderOptions != null && renderOptions.DPI > 0 && renderOptions.DPI < 600 ? renderOptions.DPI : 128, true);
                        if (rasterizedPageId == 0)
                        {
                          gdImage.ReleaseGdPictureImage(multiTiffId);
                          return null;
                        }

                        if (i == 1)
                        {
                          multiTiffId = rasterizedPageId;
                          status = gdImage.TiffSaveAsMultiPageFile(multiTiffId, result, TiffCompression.TiffCompressionJPEG);
                        }
                        else
                        {
                          status = gdImage.TiffAddToMultiPageFile(multiTiffId, rasterizedPageId);
                          gdImage.ReleaseGdPictureImage(rasterizedPageId);
                        }
                      }
                    }
                  }
                }

                if (gdImage.TiffCloseMultiPageFile(multiTiffId) != GdPictureStatus.OK)
                {
                  gdImage.ReleaseGdPictureImage(multiTiffId);
                  return null;
                }

                gdImage.ReleaseGdPictureImage(multiTiffId);
              }
              else
              {
                gdPDF.SelectPage(1);
                var rasterizedPageId = gdPDF.RenderPageToGdPictureImageEx(renderOptions != null && renderOptions.DPI > 0 && renderOptions.DPI < 600 ? renderOptions.DPI : 128, true);
                if (rasterizedPageId == 0)
                {
                  return null;
                }

                var encoderParam = 0;
                switch (destFileFormat)
                {
                  case Enumerators.Document.FileFormat.JPEG:
                    encoderParam = renderOptions != null && renderOptions.ImageQuality > 0 && renderOptions.ImageQuality <= 100 ? renderOptions.ImageQuality : 95;// Quality- 1 (lower) and 100 (higher)
                    break;

                  case Enumerators.Document.FileFormat.JPEG2K:
                    encoderParam = renderOptions != null && renderOptions.ImageQuality > 0 && renderOptions.ImageQuality <= 512 ? renderOptions.ImageQuality : 16;// [1(MaxQuality - Lossless) ... 512(Poor quality)].
                    break;

                  case Enumerators.Document.FileFormat.PNG:
                    encoderParam = renderOptions != null && renderOptions.ImageQuality >= 0 && renderOptions.ImageQuality <= 9 ? renderOptions.ImageQuality : 6; //  [0(no compression - faster encoding) ... 9(max compression - slower encoding)].
                    break;

                  case Enumerators.Document.FileFormat.TIFF:
                    encoderParam = 65536;// 65536- Automatic compression
                    break;
                }

                using (var ms = new MemoryStream())
                {
                  var docFormat = DocUtils.ToGdDocumentFormat(destFileFormat);
                  if (gdImage.SaveAsStream(rasterizedPageId, result, docFormat, encoderParam) != GdPictureStatus.OK)
                  {
                    result = null;
                  }
                }
              }
            }
          }
          #endregion

          #region Export text
          else if (destFileFormat == Atlas.Enumerators.Document.FileFormat.TXT)
          {
            // Extract text
            var sb = new StringBuilder();
            for (var i = 1; i <= pageCount; i++)
            {
              gdPDF.SelectPage(i);
              sb.Append(gdPDF.GetPageText());
            }

            result = new MemoryStream(Encoding.ASCII.GetBytes(sb.ToString()));
          }
          #endregion

          gdPDF.CloseDocument();
        }
      }

      return result;
    }

  }
}
