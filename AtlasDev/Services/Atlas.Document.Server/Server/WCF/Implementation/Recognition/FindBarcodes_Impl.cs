using System;
using System.Collections.Generic;
using System.Diagnostics;

using AutoMapper;
using GdPicture12;
using SDTBarcode;

using Atlas.Common.Utils;
using Atlas.DocServer.Utils;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Recognition
{
  internal class FindBarcodes_Impl
  {
    internal static List<BarcodeFound> Execute(ILogging log, FileFormatEnums.FormatType sourceFileFormat, byte[] source, string openPassword, bool isCompressed, List<BarcodeEnums.BarcodeDirections> searchOrientation, List<BarcodeEnums.BarcodeTypes> searchForTypes)
    {
      var methodName = "FindBarcodes";
      log.Information("{MethodName} started: {DocumentLegth} bytes- {DocumentFormat}", methodName, source != null ? source.Length : 0, sourceFileFormat);

      if (source == null || source.Length == 0)
      {
        log.Warning("{MethodName}: parameter 'source' empty", methodName);
        return null;
      }

      if (sourceFileFormat == FileFormatEnums.FormatType.NotSet)
      {
        log.Warning("{MethodName}: parameter 'sourceFileFormat' empty", methodName);
        return null;
      }

      // Must be bitmap or PDF
      if (sourceFileFormat != FileFormatEnums.FormatType.BMP && sourceFileFormat != FileFormatEnums.FormatType.JPEG &&
        sourceFileFormat != FileFormatEnums.FormatType.JPEG2K && sourceFileFormat != FileFormatEnums.FormatType.PDF &&
        sourceFileFormat != FileFormatEnums.FormatType.PNG && sourceFileFormat != FileFormatEnums.FormatType.TIFF)
      {
        log.Warning("{MethodName} parameter 'sourceFileFormat' no support for file type {0}", methodName, sourceFileFormat);
        return null;
      }

      if (searchOrientation == null || searchOrientation.Count == 0)
      {
        searchOrientation = new List<BarcodeEnums.BarcodeDirections> { BarcodeEnums.BarcodeDirections.Angle0, BarcodeEnums.BarcodeDirections.Angle180 };
      }

      if (searchForTypes == null || searchForTypes.Count == 0)
      {
        searchForTypes = new List<BarcodeEnums.BarcodeTypes> { BarcodeEnums.BarcodeTypes.Code9Of3, BarcodeEnums.BarcodeTypes.DataMatrix };
      }

      try
      {
        var timer = Stopwatch.StartNew();
        if (isCompressed)
        {
          source = Compression.InMemoryDecompress(source);
        }

        var result = new List<BarcodeFound>();
        using (var gdPicture = new GdPictureImaging())
        {
          // If PDF, convert to TIFF for GdPicture...
          if (sourceFileFormat == FileFormatEnums.FormatType.PDF)
          {
            using (var dest = Utils.PdfUtils.ExportPdfTo(source, openPassword, new RenderOptions { DPI = 300, ImageQuality = 100 }, Enumerators.Document.FileFormat.TIFF))
            {
              source = dest.StreamToByte();
            }
            sourceFileFormat = FileFormatEnums.FormatType.TIFF;
          }

          var fileFormatEnum = Mapper.Map<Enumerators.Document.FileFormat>(sourceFileFormat);
          var imageId = gdPicture.CreateGdPictureImageFromByteArray(source, Utils.DocUtils.ToGdDocumentFormat(fileFormatEnum));
          if (imageId > 0)
          {
            using (var barcodeEngine = new SDTBarcodeEngine("SDTHC-ARPRF-VDKHV-GRJLE-NBFKL-MKLPN-LNAEA"))
            {
              barcodeEngine.SetLicenseUpgradeKey("SDTHN-CJTAB-PCADG-HRNFF-MFBLK-ZBLPH-BMBPK");
              barcodeEngine.SetReadInputTypes(FromBarcodeType(searchForTypes));
              barcodeEngine.SetReadInputDirections(FromBarcodeDirection(searchOrientation));

              if (sourceFileFormat == FileFormatEnums.FormatType.TIFF && gdPicture.TiffIsMultiPage(imageId))
              {
                #region Handle multi-page TIFF by processing page by page
                var pageCount = gdPicture.TiffGetPageCount(imageId);
                for (var currPage = 1; currPage <= pageCount; currPage++)
                {
                  if (gdPicture.TiffSelectPage(imageId, currPage) == GdPictureStatus.OK)
                  {
                    var hBitmap = gdPicture.GetHBitmapFromGdPictureImage(imageId);
                    if (gdPicture.GetStat() == GdPictureStatus.OK && hBitmap != null && hBitmap != IntPtr.Zero)
                    {
                      try
                      {
                        if (barcodeEngine.ReadBitmapHandle(hBitmap) == 0)
                        {
                          var count = barcodeEngine.GetResultsCount();
                          for (var barcode = 0; barcode < count; barcode++)
                          {
                            result.Add(new BarcodeFound()
                            {
                              Type = ToBarcodeType(barcodeEngine.GetResultType(barcode)),
                              Direction = ToBarcodeDirection(barcodeEngine.GetResultDirection(barcode)),
                              PageNo = currPage,
                              Data = barcodeEngine.GetResultValue(barcode),

                              X1 = barcodeEngine.GetResultPositionLeft(barcode),
                              X2 = barcodeEngine.GetResultPositionRight(barcode),
                              Y1 = barcodeEngine.GetResultPositionTop(barcode),
                              Y2 = barcodeEngine.GetResultPositionBottom(barcode),
                            });
                          }
                        }
                        else
                        {
                          log.Warning("{MethodName} failed to extract any barcodes from page {Page}", methodName, currPage);
                        }
                      }
                      finally
                      {
                        gdPicture.ReleaseHBitmap(hBitmap);
                      }
                    }
                    else
                    {
                      log.Warning("{MethodName} failed to extract HBitmap- {Error}", methodName, gdPicture.GetStat());
                    }
                  }
                  else
                  {
                    log.Warning("{MethodName} TiffSelectPage failed for page {Page}", methodName, currPage);
                  }
                }
                #endregion
              }
              else
              {
                #region Process single image
                var hBitmap = gdPicture.GetHBitmapFromGdPictureImage(imageId);
                if (gdPicture.GetStat() == GdPictureStatus.OK && hBitmap != null && hBitmap != IntPtr.Zero)
                {
                  try
                  {
                    if (barcodeEngine.ReadBitmapHandle(hBitmap) == 0)
                    {
                      var count = barcodeEngine.GetResultsCount();
                      for (var barcode = 0; barcode < count; barcode++)
                      {
                        result.Add(new BarcodeFound()
                        {
                          Type = ToBarcodeType(barcodeEngine.GetResultType(barcode)),
                          Direction = ToBarcodeDirection(barcodeEngine.GetResultDirection(barcode)),
                          PageNo = 1,
                          Data = barcodeEngine.GetResultValue(barcode),
                          X1 = barcodeEngine.GetResultPositionLeft(barcode),
                          X2 = barcodeEngine.GetResultPositionRight(barcode),
                          Y1 = barcodeEngine.GetResultPositionTop(barcode),
                          Y2 = barcodeEngine.GetResultPositionBottom(barcode),
                        });
                      }
                    }
                    else
                    {
                      log.Warning("{MethodName} failed to extract any barcodes from page {Page}", methodName, 1);
                    }
                  }
                  finally
                  {
                    gdPicture.ReleaseHBitmap(hBitmap);
                  }
                }
                else
                {
                  log.Warning("{MethodName} failed to extract HBitmap- {Error}", methodName, gdPicture.GetStat());
                }
                #endregion
              }
            }

            gdPicture.ReleaseGdPictureImage(imageId);
          }
        }

        log.Information("{MethodName} completed with result: {@BarcodesFound}, time: {ElapsedMS}ms", methodName, result, timer.ElapsedMilliseconds);

        return result;
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        return null;
      }
    }
        

    #region Private methods

    /// <summary>
    /// Returns SD-Toolkit type to Enum
    /// </summary>
    /// <param name="sdBarcodeType">The sd barcode type, from BarcodeEngine.GetResultType() method</param>
    /// <returns>BarcodeType Enum</returns>
    private static BarcodeEnums.BarcodeTypes ToBarcodeType(int barcodeType)
    {
      // Cannot use switch as these are no constants
      if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_CODABAR)
      {
        return BarcodeEnums.BarcodeTypes.CodaBar;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_CODE11)
      {
        return BarcodeEnums.BarcodeTypes.Code11;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_CODE128)
      {
        return BarcodeEnums.BarcodeTypes.Code128;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_CODE32)
      {
        return BarcodeEnums.BarcodeTypes.Code32;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_CODE39)
      {
        return BarcodeEnums.BarcodeTypes.Code3Of9;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_CODE93)
      {
        return BarcodeEnums.BarcodeTypes.Code9Of3;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_DATAMATRIX)
      {
        return BarcodeEnums.BarcodeTypes.DataMatrix;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_EAN13)
      {
        return BarcodeEnums.BarcodeTypes.Ean13;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_EAN5)
      {
        return BarcodeEnums.BarcodeTypes.Ean5;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_EAN8)
      {
        return BarcodeEnums.BarcodeTypes.Ean8;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_GS1DATABAR)
      {
        return BarcodeEnums.BarcodeTypes.Gs1DataBar;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_I2OF5)
      {
        return BarcodeEnums.BarcodeTypes.I2Of5;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_INTELLIMAIL)
      {
        return BarcodeEnums.BarcodeTypes.IntelliMail;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_MSI)
      {
        return BarcodeEnums.BarcodeTypes.MSI;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_PATCH_CODE)
      {
        return BarcodeEnums.BarcodeTypes.PatchCode;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_PDF417)
      {
        return BarcodeEnums.BarcodeTypes.Pdf417;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_PLUS2)
      {
        return BarcodeEnums.BarcodeTypes.Plus2;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_PLUS5)
      {
        return BarcodeEnums.BarcodeTypes.Plus5;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_POSTNET)
      {
        return BarcodeEnums.BarcodeTypes.Postnet;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_QRCODE)
      {
        return BarcodeEnums.BarcodeTypes.QrCode;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_UPCA)
      {
        return BarcodeEnums.BarcodeTypes.UpcA;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_UPCB)
      {
        return BarcodeEnums.BarcodeTypes.UpcB;
      }
      else if (barcodeType == SDTBarcodeEngine.SDTBARCODETYPE_UPCE)
      {
        return BarcodeEnums.BarcodeTypes.UpcE;
      }

      return BarcodeEnums.BarcodeTypes.NotSet;
    }


    /// <summary>
    /// Converts SD-toolkit BarcodeEngine.GetResultDirection() to Enum
    /// </summary>
    /// <param name="sdDirection">The direction (BarcodeEngine.SDTREADDIRECTION_...)</param>
    /// <returns>BarcodeDirections Enum</returns>
    private static BarcodeEnums.BarcodeDirections ToBarcodeDirection(int sdDirection)
    {
      // Cannot use switch as these are not constants
      if (sdDirection == SDTBarcodeEngine.SDTREADDIRECTION_ALL)
      {
        return BarcodeEnums.BarcodeDirections.All;
      }
      else if (sdDirection == SDTBarcodeEngine.SDTREADDIRECTION_LTR)
      {
        return BarcodeEnums.BarcodeDirections.Angle0;
      }
      else if (sdDirection == SDTBarcodeEngine.SDTREADDIRECTION_TTB)
      {
        return BarcodeEnums.BarcodeDirections.Angle90;
      }
      else if (sdDirection == SDTBarcodeEngine.SDTREADDIRECTION_RTL)
      {
        return BarcodeEnums.BarcodeDirections.Angle180;
      }
      else if (sdDirection == SDTBarcodeEngine.SDTREADDIRECTION_BTT)
      {
        return BarcodeEnums.BarcodeDirections.Angle270;
      }

      return BarcodeEnums.BarcodeDirections.NotSet;
    }


    /// <summary>
    /// Returns SD-Toolkit int from Enums
    /// </summary>
    /// <param name="barcodeTypes">List of barcode types to search for</param>
    /// <returns>int for use with BarcodeEngine.SetReadInputTypes</returns>
    private static int FromBarcodeType(List<BarcodeEnums.BarcodeTypes> barcodeTypes)
    {
      var result = 0;
      foreach (var barcodeType in barcodeTypes)
      {
        switch (barcodeType)
        {
          case BarcodeEnums.BarcodeTypes.CodaBar:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_CODABAR;
            break;

          case BarcodeEnums.BarcodeTypes.Code11:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_CODE11;
            break;

          case BarcodeEnums.BarcodeTypes.Code128:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_CODE128;
            break;

          case BarcodeEnums.BarcodeTypes.Code32:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_CODE32;
            break;

          case BarcodeEnums.BarcodeTypes.Code3Of9:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_CODE39;
            break;

          case BarcodeEnums.BarcodeTypes.Code9Of3:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_CODE93;
            break;

          case BarcodeEnums.BarcodeTypes.DataMatrix:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_DATAMATRIX;
            break;

          case BarcodeEnums.BarcodeTypes.Ean13:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_EAN13;
            break;

          case BarcodeEnums.BarcodeTypes.Ean5:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_EAN5;
            break;

          case BarcodeEnums.BarcodeTypes.Ean8:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_EAN8;
            break;

          case BarcodeEnums.BarcodeTypes.Gs1DataBar:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_GS1DATABAR;
            break;

          case BarcodeEnums.BarcodeTypes.I2Of5:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_I2OF5;
            break;

          case BarcodeEnums.BarcodeTypes.IntelliMail:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_INTELLIMAIL;
            break;

          case BarcodeEnums.BarcodeTypes.MSI:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_MSI;
            break;

          case BarcodeEnums.BarcodeTypes.PatchCode:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_PATCH_CODE;
            break;

          case BarcodeEnums.BarcodeTypes.Pdf417:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_PDF417;
            break;

          case BarcodeEnums.BarcodeTypes.PharmaCode:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_PHARMACODE;
            break;

          case BarcodeEnums.BarcodeTypes.Plus2:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_PLUS2;
            break;

          case BarcodeEnums.BarcodeTypes.Plus5:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_PLUS5;
            break;

          case BarcodeEnums.BarcodeTypes.Postnet:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_POSTNET;
            break;

          case BarcodeEnums.BarcodeTypes.QrCode:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_QRCODE;
            break;

          case BarcodeEnums.BarcodeTypes.UpcA:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_UPCA;
            break;

          case BarcodeEnums.BarcodeTypes.UpcB:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_UPCB;
            break;

          case BarcodeEnums.BarcodeTypes.UpcE:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_UPCE;
            break;

          case BarcodeEnums.BarcodeTypes.All1D:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_ALL_1D;
            break;

          case BarcodeEnums.BarcodeTypes.All2D:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_ALL_2D;
            break;

          case BarcodeEnums.BarcodeTypes.All:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_ALL_2D | SDTBarcodeEngine.SDTBARCODETYPE_ALL_1D;
            break;

          case BarcodeEnums.BarcodeTypes.NotSet:
            result |= SDTBarcodeEngine.SDTBARCODETYPE_UNKNOWN;
            break;
        }
      }

      return result;
    }


    /// <summary>
    /// Converts enums to int for BarcodeEngine.GetResultDirection()
    /// </summary>
    /// <param name="barcodeDirections">Barcode directions Enum</param>
    /// <returns>int for use with BarcodeEngine.SetReadInputDirections </returns>
    private static int FromBarcodeDirection(List<BarcodeEnums.BarcodeDirections> barcodeDirections)
    {
      var result = 0;

      foreach (var direction in barcodeDirections)
      {
        switch (direction)
        {
          case BarcodeEnums.BarcodeDirections.Angle0:
            result |= SDTBarcodeEngine.SDTREADDIRECTION_LTR;
            break;

          case BarcodeEnums.BarcodeDirections.Angle90:
            result |= SDTBarcodeEngine.SDTREADDIRECTION_TTB;
            break;

          case BarcodeEnums.BarcodeDirections.Angle180:
            result |= SDTBarcodeEngine.SDTREADDIRECTION_RTL;
            break;

          case BarcodeEnums.BarcodeDirections.Angle270:
            result |= SDTBarcodeEngine.SDTREADDIRECTION_BTT;
            break;

          case BarcodeEnums.BarcodeDirections.All:
            result |= SDTBarcodeEngine.SDTREADDIRECTION_ALL;
            break;
        }
      }

      return result;
    }

    #endregion
    
  }
}
