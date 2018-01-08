using System;
using System.IO;

using GdPicture12;


using Atlas.DocServer.WCF.Interface;


namespace Atlas.DocServer.WCF.Implementation.Utils
{
  internal static class GdPictureUtils
  {
    /// <summary>
    /// Perform gdPicture image clean-up (mainly for scans)
    /// </summary>
    /// <param name="gdPicture">The GdPicture object</param>
    /// <param name="imageId">The GdPicture ImageId from opening document</param>
    /// <param name="cleanUpOptions">The clean-up options to apply to the image</param>
    internal static void GdCleanUp(GdPictureImaging gdPicture, int imageId, CleanUpOptions cleanUpOptions)
    {
      if (cleanUpOptions.AutoDeskew)
      {
        gdPicture.AutoDeskew(imageId);
      }
      if (cleanUpOptions.CropBlackBorders)
      {
        gdPicture.CropBlackBorders(imageId);
      }
      if (cleanUpOptions.CropWhiteBorders)
      {
        gdPicture.CropWhiteBorders(imageId);
      }
      if (cleanUpOptions.Despeckle)
      {
        gdPicture.FxDespeckle(imageId);
      }
      if (cleanUpOptions.RemovePunchHoles)
      {
        gdPicture.RemoveHolePunch(imageId);
      }

      if (cleanUpOptions.RemoveHorizontalLines && gdPicture.IsBitonal(imageId))
      {
        gdPicture.RemoveLines(imageId, LineRemoveOrientation.Horizontal);
      }

      if (cleanUpOptions.RemoveVerticalLines && gdPicture.IsBitonal(imageId))
      {
        gdPicture.RemoveLines(imageId, LineRemoveOrientation.Vertical);
      }

      if ((cleanUpOptions.NewHeight > 0 || cleanUpOptions.NewWidth > 0) &&
        (cleanUpOptions.NewHeight < 4000 && cleanUpOptions.NewWidth < 4000))
      {
        var currWidth = gdPicture.GetWidth(imageId);
        var currHeight = gdPicture.GetHeight(imageId);
        var scale = Math.Min((float)cleanUpOptions.NewWidth / (float)currWidth, (float)cleanUpOptions.NewHeight / (float)currHeight);
        gdPicture.Scale(imageId, scale, System.Drawing.Drawing2D.InterpolationMode.Bicubic);
        gdPicture.Crop(imageId, 0, 0, cleanUpOptions.NewWidth, cleanUpOptions.NewHeight);
      }

      if (cleanUpOptions.SetColourBitDepth > 0)
      {
        if (gdPicture.GetBitDepth(imageId) != cleanUpOptions.SetColourBitDepth)
        {
          switch (cleanUpOptions.SetColourBitDepth)
          {
            case 1:
              gdPicture.ConvertTo1BppAT(imageId); // This method is particularly efficient on documents with contrasted foreground and background- i.e. scans?
              break;

            case 8:
              gdPicture.ConvertTo8BppGrayScaleAdv(imageId);
              break;

            case 9:
              gdPicture.ConvertTo8BppQ(imageId);
              break;

            case 24:
              gdPicture.ConvertTo24BppRGB(imageId);
              break;

            case 32:
              gdPicture.ConvertTo32BppRGB(imageId);
              break;
          }
        }
      }
      //else if (cleanUpOptions.AutoReduceColourDepth && gdPicture.GetBitDepth(imageId) > 1)
      //{
      //var colors = gdPicture.ColorDetection(ImageID: imageId, AutoConvert: true, ScanningContext: true, AutoRepairCharacters: true);
      //}
    }


    /// <summary>
    /// Converts an image to a PDF
    /// </summary>
    /// <param name="source">Source file byte array</param>
    /// <param name="sourceFileFormat">The source file format</param>
    /// <param name="docOptions">Conversion options</param>
    /// <param name="makePdfA">Make PDF/A- no encryption- used for PDF archiving</param>
    /// <returns>byte array containing the PDF file, null if any problem occurred</returns>
    internal static Stream ImageToPdf(Stream source, Enumerators.Document.FileFormat sourceFileFormat, DocOptions docOptions, bool makePdfA)
    {
      Stream result = new MemoryStream();

      using (var gdImage = new GdPictureImaging())
      {
        var imageNum = 0;
        var docFormat = DocUtils.ToGdDocumentFormat(sourceFileFormat);
        if (docFormat == GdPicture12.DocumentFormat.DocumentFormatUNKNOWN)
        {
          return null;
        }

        imageNum = gdImage.CreateGdPictureImageFromStream(source, docFormat);
        if (imageNum == 0)
        {
          return null;
        }

        using (var gdPDF = new GdPicturePDF())
        {
          gdPDF.NewPDF(makePdfA);

          if (sourceFileFormat == Enumerators.Document.FileFormat.TIFF && gdImage.TiffIsMultiPage(imageNum))
          {
            int pageCount = gdImage.TiffGetPageCount(imageNum);
            for (int i = 1; i <= pageCount; i++)
            {
              gdImage.TiffSelectPage(imageNum, i);
              gdPDF.AddImageFromGdPictureImage(imageNum, false, true);
            }
          }
          else
          {
            gdPDF.SelectPage(1);
            gdPDF.AddImageFromGdPictureImage(imageNum, false, true);
          }

          gdPDF.SetProducer(docOptions.Producer ?? DocConsts.STR_AtlasDocumentGenerator);
          gdPDF.SetAuthor(docOptions.Author ?? DocConsts.STR_AtlasDocumentGenerator);
          gdPDF.SetCreator(docOptions.Creator ?? DocConsts.STR_AtlasDocumentGenerator);
          gdPDF.SetKeywords(docOptions.Keywords ?? DocConsts.STR_AtlasDocumentGenerator);
          gdPDF.SetSubject(docOptions.Subject ?? DocConsts.STR_AtlasDocumentGenerator);
          gdPDF.SetTitle(docOptions.Title ?? DocConsts.STR_AtlasDocumentGenerator);

          //
          // 256-bit AES not fully supported in all viewers? 256-bit AES is 1.7 >Acrobat 8.x required
          // PDF/A must not have any encryption!
          //
          gdPDF.SaveToStream(result, makePdfA ? PdfEncryption.PdfEncryptionNone : PdfEncryption.PdfEncryption128BitAES,
            makePdfA ? "" : docOptions.UserPassword, makePdfA ? "" : docOptions.OwnerPassword,
            docOptions.CanPrint, docOptions.CanCopy, docOptions.CanModify, docOptions.CanAddNotes, docOptions.CanFillFields,
            docOptions.CanCopyAccess, docOptions.CanAssemble, docOptions.CanPrintFull);

          gdPDF.CloseDocument();
        }

        gdImage.ReleaseGdPictureImage(imageNum);
      }

      return result;
    }

  }
}
