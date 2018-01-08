/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2014 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    DataMatrix detection utilities
 *       
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2014- Created
 * 
 * 
 *  Comments:
 *  ------------------
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using GdPicture12;
using SDTBarcode;


namespace Atlas.DocServer.WCF.Implementation.Utils
{
  internal static class DataMatrixDocUtils
  {
    /// <summary>
    /// Detects DataMatrix barcodes in a given PDF and returns the barcodes found (value and position)
    /// </summary>    
    /// <param name="file">The PDF stream (reading will start at the beginning of the stream)</param>
    /// <returns>List of detected barcodes: Tuple = Physical page/barcode value/found X pos/Found Y pos</returns>
    internal static List<Tuple<int, string, int, int>> DetectDMBarcodesInPDF(Stream file)
    {
      var result = new List<Tuple<int, string, int, int>>();
      using (var barcodeEngine = new SDTBarcodeEngine("SDTHC-ARPRF-VDKHV-GRJLE-NBFKL-MKLPN-LNAEA"))
      {
        barcodeEngine.SetReadInputTypes(SDTBarcodeEngine.SDTBARCODETYPE_DATAMATRIX);
        barcodeEngine.SetReadInputDirections(SDTBarcodeEngine.SDTREADDIRECTION_ALL /*SDTREADDIRECTION_TTB + SDTBarcodeEngine.SDTREADDIRECTION_BTT*/);

        #region Handle PDF pages
        using (var gdPDF = new GdPicturePDF())
        {
          file.Position = 0;
          if (gdPDF.LoadFromStream(file) != GdPictureStatus.OK)
          {
            return null;
          }

          var pageCount = gdPDF.GetPageCount();
          if (pageCount == 0)
          {
            return null;
          }

          using (var gdImage = new GdPictureImaging())
          {
            for (var currPage = 1; currPage <= pageCount; currPage++)
            {
              gdPDF.SelectPage(currPage);
              var imageId = 0;
              if (gdPDF.IsPageImage(ref imageId, false) && imageId > 0) // Returns if the current page of the PDF is image-based. pages containing only one fully visible bitmap covering the whole page area, with no other particular drawing operation such as clipping path.
              {
                var hBitmap = gdImage.GetHBitmapFromGdPictureImage(imageId);
                if (hBitmap == null || hBitmap == IntPtr.Zero)
                {
                  return null;
                }

                if (barcodeEngine.ReadBitmapHandle(hBitmap) == 0) // successfully opened and read the file
                {
                  var barcodeCount = barcodeEngine.GetResultsCount();
                  if (barcodeCount > 0)
                  {
                    for (var barcodeNum = 0; barcodeNum < barcodeCount; barcodeNum++)
                    {
                      result.Add(new Tuple<int, string, int, int>(currPage,
                        barcodeEngine.GetResultValue(barcodeNum),
                        barcodeEngine.GetResultPositionLeft(barcodeNum),
                        barcodeEngine.GetResultPositionTop(barcodeNum)));
                    }
                  }
                }

                gdImage.ReleaseHBitmap(hBitmap);
              }

              gdImage.ReleaseGdPictureImage(imageId);
            }
          }

          gdPDF.CloseDocument();
        }
        #endregion
      }

      return result;
    }


    /// <summary>
    /// Detects DataMatrix barcodes on each page and then: generates a temporary PDF/A file per Atlas doc, with:
    ///   - pages in the correct order
    ///   - correctly oriented pages (automatically rotates upside-down pages)
    ///   - duplicated pages ignored
    ///   - Blank pages/pages without barcodes are ignored
    ///   - pages with invalid barcode data formatting - [StorageId]*[This page number (1-..)]*[Document page count (1-..)] and    
    /// </summary>
    /// <param name="file">The scanned input PDF stream to decode</param>
    /// <returns>Tuple of: Barcode document *indicated* unique page count, *actual* document scanned page count, temp PDF/A filename, document StorageId</returns>
    internal static List<Tuple<int, int, string, Int64>> ScannedPDFOrderDocuments(Stream file)
    {
      var result = new List<Tuple<int, int, string, Int64>>();

      var pagesExtracted = new List<Tuple<string, Int64, int, int, int>>(); // Extracted filename, storage Id, curr page, page count, barcode y pos
      try
      {
        using (var barcodeEngine = new SDTBarcodeEngine("SDTHC-ARPRF-VDKHV-GRJLE-NBFKL-MKLPN-LNAEA"))
        {
          barcodeEngine.SetReadInputTypes(SDTBarcodeEngine.SDTBARCODETYPE_DATAMATRIX);
          barcodeEngine.SetReadInputDirections(SDTBarcodeEngine.SDTREADDIRECTION_ALL /*SDTREADDIRECTION_TTB + SDTBarcodeEngine.SDTREADDIRECTION_BTT*/);

          using (var gdPDF = new GdPicturePDF())
          using (var gdImage = new GdPictureImaging())
          {
            #region Extract and save each page's image in the scanned PDF, to a TIFF file
            file.Position = 0;
            if (gdPDF.LoadFromStream(file) != GdPictureStatus.OK)
            {
              return null;
            }

            var pageCount = gdPDF.GetPageCount();
            if (pageCount == 0)
            {
              return null;
            }

            for (var currPage = 1; currPage <= pageCount; currPage++)
            {
              gdPDF.SelectPage(currPage);
              var imageId = 0;
              if (gdPDF.IsPageImage(ref imageId, false) && imageId > 0) // Returns if the current page of the PDF is image-based. pages containing only one fully visible bitmap covering the whole page area, with no other particular drawing operation such as clipping path
              {
                var hBitmap = gdImage.GetHBitmapFromGdPictureImage(imageId);
                if (hBitmap == null || hBitmap == IntPtr.Zero)
                {
                  return null;
                }

                if (barcodeEngine.ReadBitmapHandle(hBitmap) == 0 && barcodeEngine.GetResultsCount() > 0)
                {
                  var barcodeVal = barcodeEngine.GetResultValue(0);
                  if (!string.IsNullOrEmpty(barcodeVal))
                  {
                    var values = barcodeVal.Split("*".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    Int64 storageId;
                    int thisPage;
                    int thisPageCount;
                    if (values.Length >= 3 && Int64.TryParse(values[0], out storageId)
                      && int.TryParse(values[1], out thisPage)
                      && int.TryParse(values[2], out thisPageCount) && thisPage <= thisPageCount)
                    {
                      var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
                      gdImage.SaveAsTIFF(imageId, tempFile, TiffCompression.TiffCompressionAUTO);

                      pagesExtracted.Add(new Tuple<string, Int64, int, int, int>(tempFile, storageId, barcodeEngine.GetResultPositionTop(0), thisPage, thisPageCount));
                    }
                  }
                }

                gdImage.ReleaseGdPictureImage(imageId);
                gdImage.ReleaseHBitmap(hBitmap);
              }
            }

            gdPDF.CloseDocument();
            #endregion

            #region For each Storage Id, create a new PDF with pages in the correct order and orientation
            // Sort by Storage Id and then the current page
            var orderedDetails = pagesExtracted.OrderBy(s => s.Item2).ThenBy(s => s.Item4).ToList();
            var donePages = new HashSet<string>();
            Int64 lastDocStorageId = 0;
            var pagesInThisDoc = 0;
            var pageCountInThisDoc = 0;

            // Anonymous lambda to save the current PDF and add details to result
            Action<int, int, Int64> savePdf = (pagesIndicated, pagesCounted, storageId) =>
              {
                var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
                gdPDF.SaveToFile(tempFile, true);
                result.Add(new Tuple<int, int, string, long>(pagesIndicated, pagesCounted, tempFile, storageId));
                gdPDF.CloseDocument();
                pagesInThisDoc = 0;
              };
            for (var i = 0; i < orderedDetails.Count; i++)
            {
              var pageInfo = orderedDetails[i];
              var thisDocStorageId = pageInfo.Item2;

              var pageUniqueKey = string.Format("{0:D20}{1:D20}", thisDocStorageId, pageInfo.Item4);

              if (!donePages.Contains(pageUniqueKey))
              {
                if (lastDocStorageId != thisDocStorageId) // we have moved to a new document...
                {
                  if (lastDocStorageId > 0 && pagesInThisDoc > 0)
                  {
                    savePdf(pageCountInThisDoc, pagesInThisDoc, lastDocStorageId);
                  }
                                    
                  gdPDF.NewPDF(true); // Create PDF/A
                  lastDocStorageId = thisDocStorageId;
                  pageCountInThisDoc = pageInfo.Item5;
                }

                var pageImage = gdImage.CreateGdPictureImageFromFile(pageInfo.Item1); // Load image file

                if (pageInfo.Item3 > (gdImage.GetHeight(pageImage) / 2)) // barcode more than halfway down page? Page is upside down
                {
                  gdImage.Rotate(pageImage, RotateFlipType.Rotate180FlipNone);
                }
                gdPDF.AddImageFromGdPictureImage(pageImage, false, true);
                gdImage.ReleaseGdPictureImage(pageImage);

                pagesInThisDoc++;
                donePages.Add(pageUniqueKey);

                if (i == orderedDetails.Count - 1 && pagesInThisDoc > 0) // Last file
                {
                  savePdf(pageCountInThisDoc, pagesInThisDoc, lastDocStorageId);
                }                
              }
            }
            #endregion
          }
        }
      }
      finally
      {
        // Clean up temp page image files
        foreach (var tempFile in pagesExtracted)
        {
          try { File.Delete(tempFile.Item1); }
          catch { }
        }
      }

      return result;
    }

  }
}
