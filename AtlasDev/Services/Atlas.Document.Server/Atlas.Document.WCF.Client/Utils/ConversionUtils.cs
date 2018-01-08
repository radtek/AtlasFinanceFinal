using System;
using System.IO;

using Atlas.DocServer.WCF.Interface;


namespace Atlas.DataSync.WCF.Client
{
  public static class ConversionUtils
  {
    public static FileFormatEnums.FormatType ToDocumentFormat(string filename)
    {
      var ext = Path.GetExtension(filename).ToLower().TrimStart('.').ToLower();
      switch (ext)
      {
        case "pdf":
          return FileFormatEnums.FormatType.PDF;

        case "bmp":
          return FileFormatEnums.FormatType.BMP;

        case "jpg":
        case "jpeg":
          return FileFormatEnums.FormatType.JPEG;

        case "jp2k":
        case "jp2000":
          return FileFormatEnums.FormatType.JPEG2K;

        case "doc":
          return FileFormatEnums.FormatType.DOC;

        case "docx":
          return FileFormatEnums.FormatType.DOCX;

        case "xls":
          return FileFormatEnums.FormatType.XLS;

        case "xlsx":
          return FileFormatEnums.FormatType.XLSX;

        case "mht":
          return FileFormatEnums.FormatType.MHT;

        case "png":
          return FileFormatEnums.FormatType.PNG;

        case "htm":
        case "html":
          return FileFormatEnums.FormatType.HTML;

        case "txt":
          return FileFormatEnums.FormatType.TXT;

        case "csv":
          return FileFormatEnums.FormatType.CSV;

        case "emf":
          return FileFormatEnums.FormatType.EMF;

        case "ods":
          return FileFormatEnums.FormatType.ODS;

        case "odt":
          return FileFormatEnums.FormatType.ODT;

        case "rtf":
          return FileFormatEnums.FormatType.RTF;

        case "tif":
        case "tiff":
          return FileFormatEnums.FormatType.TIFF;

        case "prnx":
          return FileFormatEnums.FormatType.PRNX;

        case "snx":
          return FileFormatEnums.FormatType.SNX;

        default:
          return FileFormatEnums.FormatType.NotSet;
      }
    }


    private static string ToExtension(FileFormatEnums.FormatType fileFormat)
    {
      switch (fileFormat)
      {
        case FileFormatEnums.FormatType.BMP:
          return "bmp";

        case FileFormatEnums.FormatType.CSV:
          return "csv";

        case FileFormatEnums.FormatType.DOC:
          return "doc";

        case FileFormatEnums.FormatType.DOCX:
          return "docx";

        case FileFormatEnums.FormatType.EMF:
          return "emf";

        case FileFormatEnums.FormatType.HTML:
          return "html";

        case FileFormatEnums.FormatType.JPEG:
          return "jpg";

        case FileFormatEnums.FormatType.JPEG2K:
          return "jp2k";

        case FileFormatEnums.FormatType.MHT:
          return "mht";
        case FileFormatEnums.FormatType.ODS:
          return "ods";

        case FileFormatEnums.FormatType.ODT:
          return "odt";

        case FileFormatEnums.FormatType.PDF:
          return "pdf";

        case FileFormatEnums.FormatType.PNG:
          return "png";

        case FileFormatEnums.FormatType.PRNX:
          return "prnx";

        case FileFormatEnums.FormatType.REPX:
          return "repx";

        case FileFormatEnums.FormatType.RTF:
          return "rtf";

        case FileFormatEnums.FormatType.SNX:
          return "snx";

        case FileFormatEnums.FormatType.TIFF:
          return "tiff";

        case FileFormatEnums.FormatType.TXT:
          return "txt";

        case FileFormatEnums.FormatType.XLS:
          return "xls";

        case FileFormatEnums.FormatType.XLSX:
          return "xlsx";

        case FileFormatEnums.FormatType.ZIP:
          return "zip";

        default:
          return ".notset";
      }
    }

  }
}
