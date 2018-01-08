using System;
using System.Threading;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Net.Mail;

using ZXing;
using Serilog;
using GdPicture12;


namespace Atlas.Branch.ScanServer
{
  internal class MainService
  {
    private Thread _processThread;

    public MainService(ILogger log)
    {
      _log = log;
      log.Information("Starting...");
    }


    internal bool Start()
    {
      _processThread = new Thread(() =>
        {
          var srcPdfPath = ConfigurationManager.AppSettings["MonitorFolder"] ?? @"C:\Scans";

          while (!_terminated.Wait(5000))
          {
            try
            {
              #region Detect barcodes in PDFs
              var pdfsWithBarcode = new List<Tuple<string, string, string>>(); // filename, contract, pack type
                                                                                             
              if (Directory.Exists(srcPdfPath))
              {
                var pdfFiles = GetPdfFilesNotInUse(srcPdfPath);
                if (pdfFiles.Any())
                {
                  #region Process files
                  var processTimer = System.Diagnostics.Stopwatch.StartNew();

                  foreach (var pdfFile in pdfFiles)
                  {
                    #region Detect barcode
                    var fileTimer = System.Diagnostics.Stopwatch.StartNew();
                    var detectedBarcodes = DetectBarcodes(pdfFile);
                    fileTimer.Stop();

                    _log.Information("{Doc}- Processed in {time}ms. Found: {Count}", pdfFile, fileTimer.ElapsedMilliseconds, detectedBarcodes.Count);

                    string contractNum = null;
                    string docType = null;
                    foreach (var barcode in detectedBarcodes)
                    {
                      if (_contractRegEx.IsMatch(barcode))
                      {
                        contractNum = barcode;
                      }
                      else if (barcode.ToLower() == "application")
                      {
                        docType = "application";
                      }
                      else if (barcode.ToLower() == "loan")
                      {
                        docType = "loan";
                      }

                      if (!string.IsNullOrEmpty(contractNum) && !string.IsNullOrEmpty(docType))
                      {
                        _log.Information("{Doc}- found document pack: {contract}, type: {Type}", pdfFile, contractNum, docType);
                        pdfsWithBarcode.Add(new Tuple<string, string, string>(pdfFile, contractNum, docType));
                        break;
                      }
                    }

                    if (string.IsNullOrEmpty(contractNum) || string.IsNullOrEmpty(docType))
                    {
                      _log.Warning("No barcodes found in document: {Doc}", pdfFile);
                    }
                    #endregion
                  }

                  processTimer.Stop();
                  _log.Information("Processed {Count} files in {Elapsed}ms", pdfFiles.Count, processTimer.ElapsedMilliseconds);
                  #endregion

                  #region Upload barcoded PDFs
                  var destDir = Path.Combine(srcPdfPath, "done");
                  if (!Directory.Exists(destDir))
                  {
                    Directory.CreateDirectory(destDir);
                  }
                  foreach (var uploadPdfFile in pdfsWithBarcode)
                  {
                    var sourceFile = uploadPdfFile.Item1;
                    if (UploadFile(filename: sourceFile, contractNo: uploadPdfFile.Item2, documentType: uploadPdfFile.Item3))
                    {
                      var destFile = Path.Combine(destDir, Path.GetFileName(sourceFile));
                      if (File.Exists(destFile))
                      {
                        destFile = Path.Combine(destDir,
                          $"{Path.GetFileNameWithoutExtension(sourceFile)}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(sourceFile)}");
                      }
                      File.Move(sourceFile, destFile);
                    }
                  }
                  #endregion

                  #region Move files which cold not be processed (no barcodes)
                  destDir = Path.Combine(srcPdfPath, "unprocessed");
                  if (!Directory.Exists(destDir))
                  {
                    Directory.CreateDirectory(destDir);
                  }
                  foreach (var pdfFile in pdfFiles)
                  {
                    if (File.Exists(pdfFile))
                    {
                      var destFile = Path.Combine(destDir, Path.GetFileName(pdfFile));
                      if (File.Exists(destFile))
                      {
                        destFile = Path.Combine(destDir,
                          $"{Path.GetFileNameWithoutExtension(pdfFile)}-{Guid.NewGuid().ToString("N")}{Path.GetExtension(pdfFile)}");
                      }
                      File.Move(pdfFile, destFile);
                    }
                  }
                  #endregion
                }
              }
            }
            catch (Exception err)
            {
              _log.Error(err, "Thread()");
            }
            #endregion
          }
        });

      _processThread.IsBackground = true;
      _processThread.Start();

      return true;
    }


    /// <summary>
    /// Uploads a file as per configuration
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="contractNo"></param>
    /// <returns></returns>
    private bool UploadFile(string filename, string contractNo, string documentType)
    {
      try
      {
        var sendVia = (ConfigurationManager.AppSettings["SendToType"] ?? "email").ToLower();
        var sendTo = ConfigurationManager.AppSettings["SendToAddress"];

        if (!string.IsNullOrEmpty(sendTo) && (sendVia == "email" || sendVia == "ecm"))
        {
          if (sendVia == "email")
          {
            #region Check address
            try
            {
              var address = new MailAddress(sendTo);
            }
            catch
            {
              _log.Error("Invalid e-mail address: {Address}", sendTo);
              throw;
            }
            #endregion

            #region Send
            using (var client = new SmtpClient("mail.atcorp.co.za", 25))
            {
              client.Timeout = 120000;
              using (var message = new MailMessage("scans@atcorp.co.za", sendTo, $"New contract document {contractNo}", "Please see attached contract documentation"))
              {
                message.Attachments.Add(new Attachment(filename));
                client.Send(message);
              }
            }
            #endregion
          }
          else
          {
            #region Upload to ECM
            #endregion
          }

          _log.Information("Uploading {File} via {Via} to {Address}...", filename, sendVia, sendTo);
          return true;
        }
      }
      catch (Exception err)
      {
        _log.Error(err, "UploadFile()");

      }
      return false;
    }


    /// <summary>
    /// Return QR barcodes recognized in PDF file
    /// Uses GdPicture.net PDF to extract the images from each page and zxing to detect the QR barcodes in each image.
    /// </summary>
    /// <param name="file"></param>
    /// <returns>List of QR barcodes recognized</returns>
    private List<string> DetectBarcodes(string file)
    {
      var barcodes = new List<string>();

      using (var gdImage = new GdPictureImaging())
      {
        using (var gdPDF = new GdPicturePDF())
        {
          if (gdPDF.LoadFromFile(file, false) == GdPictureStatus.OK)
          {
            var pageCount = gdPDF.GetPageCount();
            for (var pageNum = 1; pageNum <= pageCount; pageNum++)
            {
              var pageTimer = System.Diagnostics.Stopwatch.StartNew();
              if (gdPDF.SelectPage(pageNum) == GdPictureStatus.OK)
              {
                int imageId = 0;
                if (!gdPDF.IsPageImage(ref imageId, true)) // ImageID- Output parameter.If the PDF page is image - based, this parameter will return a GdPicture Image, corresponding to the bitmap embedded in the page. In the other case, this parameter returns 0.
                {
                  imageId = gdPDF.RenderPageToGdPictureImageEx(300, false);
                }

                if (imageId > 0)
                {
                  using (var ms = new MemoryStream())
                  {                   
                    gdImage.SaveAsStream(imageId, ms, DocumentFormat.DocumentFormatBMP, 0);
                    gdImage.ReleaseGdPictureImage(imageId);
                    ms.Position = 0;
                    BitmapLuminanceSource source;
                    using (var sourceBitmap = new Bitmap(ms))
                    {
                      source = new BitmapLuminanceSource(sourceBitmap);
                    }

                    if (source != null)
                    {
                      var reader = new BarcodeReader { };
                      reader.Options.PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE };
                      reader.Options.TryHarder = true; // !! Required !!
                      var results = reader.DecodeMultiple(source);
                      if (results?.Length > 0)
                      {
                        foreach (var result in results)
                        {
                          barcodes.Add(result.Text);
                        }
                      }
                    }

                    _log.Information("{Doc}-{page}- ({Timer}ms)", file, pageNum, pageTimer.ElapsedMilliseconds);
                  }
                }
              }
            }
            gdPDF.CloseDocument();
          }
        }
      }

      return barcodes;
    }


    /// <summary>
    /// Ensures file can be opened and at last 20 seconds elapsed since created
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private List<string> GetPdfFilesNotInUse(string path)
    {
      var files = Directory.GetFiles(path, "*.PDF");

      var result = new List<string>();
      foreach (var file in files)
      {
        var fi = new FileInfo(file);
        // Ensure scanner done... 20 seconds old and can open exclusively
        if (DateTime.UtcNow.Subtract(fi.CreationTimeUtc).TotalSeconds > 20)
        {
          try
          {
            using (File.Open(file, FileMode.Open, FileAccess.Read, FileShare.None)) { }
            result.Add(file);
          }
          catch (Exception openErr)
          {
            _log.Error(openErr, "GetReadableFiles()");
          }
        }
      }

      return result;
    }


    internal bool Stop()
    {
      _terminated.Set();
      Thread.Sleep(1000);
      _log.Information("Stopped");
      return true;
    }


    private static Regex _contractRegEx = new Regex("^[0-9A-Za-z]{2,3}[Xx][0-9]{5}[Xx][0-9]{4}$");
    private static Regex _idNumber = new Regex("^[0-9]{13}$");

    private static readonly ManualResetEventSlim _terminated = new ManualResetEventSlim();
    private ILogger _log;

  }
}