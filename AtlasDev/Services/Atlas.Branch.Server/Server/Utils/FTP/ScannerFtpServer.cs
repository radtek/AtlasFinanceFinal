/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Ftp server and uploader- receives scans and uploads the files to document server via WCF
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *    Nov 2014 - Started
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Net;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;

using LumiSoft.Net.TCP;
using LumiSoft.Net.FTP;
using LumiSoft.Net.FTP.Server;
using Serilog;
using SDTBarcode;

using Atlas.DocServer.WCF.Interface;


namespace ASSSyncClient.Utils.FTP
{
  /// <summary>
  /// Class to start a local FTP server and thread to process FTP uploads
  /// </summary>
  public class ScannerFtpServer
  {
    #region Public constructor

    public ScannerFtpServer()
    {
      _log.Information("Starting scanner FTP server");

      #region Set environment
      // Where files are uploaded by the scanner, are awaiting processing
      if (!Directory.Exists(FTP_ROOT_PATH))
      {
        Directory.CreateDirectory(FTP_ROOT_PATH);
      }
      // Once file has passed barcode detection and has been successfully uploaded
      if (!Directory.Exists(FTP_PROCESSED_PATH))
      {
        Directory.CreateDirectory(FTP_PROCESSED_PATH);
      }
      // Files which do not contain barcodes, or are incompletely scanned
      if (!Directory.Exists(FTP_INCOMPLETE_PATH))
      {
        Directory.CreateDirectory(FTP_INCOMPLETE_PATH);
      }
      #endregion

      #region Start the local FTP Server
      _ftpServer = new FTP_Server()
      {
        Bindings = new LumiSoft.Net.IPBindInfo[] { new LumiSoft.Net.IPBindInfo("localhost", LumiSoft.Net.BindInfoProtocol.TCP, IPAddress.Any, 21) },
        MaxBadCommands = 5,
        SessionIdleTimeout = 80,
        MaxConnections = 2,
        GreetingText = "Atlas FTP Server- for official documents only",
        MaxConnectionsPerIP = 1,
        PassiveStartPort = 44000
      };
      _ftpServer.SessionCreated += ftpServer_SessionCreated;
      _ftpServer.Start();
      _log.Information("FTP server started");
      #endregion

      // Timer to check on FTP uploads
      _checkFTP = new Timer(OnCheckFTP, null, 10000, 15000);

      _tmrScanFTP.Start();
    }


    #endregion


    #region Public methods

    public void Stop()
    {
      _log.Information("Stopping FTP server");
      if (_ftpServer != null)
      {
        _ftpServer.Stop();
      }
      _log.Information("FTP server stopped");
    }

    #endregion


    #region Private events


    /// <summary>
    /// Timer to handle pending scans. Error control: will check expected vs actual page counts via 
    /// DataMatrix barcode and only upload a file, where is detects at least one complete document 
    /// has been scanned. 
    /// </summary>
    /// <param name="state"></param>
    private void OnCheckFTP(object state)
    {
      if (_inTimer) // Avoid any chance of re-entrancy
      {
        return;
      }

      _inTimer = true;
      try
      {
        if (_tmrScanFTP.IsRunning && _tmrScanFTP.Elapsed > TimeSpan.FromSeconds(15)) // At least 15 seconds since some FTP upload activity 
        {
          _tmrScanFTP.Stop();

          // Ricoh scans use pdf or tiff file format- filename uses date/time 'yyyyMMddHHmm' format, so process the most recent first
          var files = Directory.GetFileSystemEntries(FTP_ROOT_PATH, "*.*", SearchOption.TopDirectoryOnly)
            .Where(s => s.ToLower().EndsWith("pdf") || s.ToLower().EndsWith("tiff") || s.ToLower().EndsWith("tif"))
            .OrderByDescending(s => s).ToList();

          foreach (var fullFileName in files)
          {
            var fileNameOnly = Path.GetFileName(fullFileName);
            var dest = Path.Combine(FTP_PROCESSED_PATH, fileNameOnly);
            var processed = false;

            try
            {
              // Try open file exclusively (100%  sure the FTP upload is complete)
              using (File.Open(fullFileName, FileMode.Open, FileAccess.Read, FileShare.None))
              {
                // Do nothing...
              }
              Thread.Sleep(1000); // Ensure OS closed file

              var actualPageCount = Path.GetExtension(fullFileName).ToLower().EndsWith("pdf") ? GetNoOfPagesPDF(fullFileName) : GetNoOfPagesTIFF(fullFileName);
              if (actualPageCount > 0)
              {
                #region Detect any DataMatrix barcodes- only upload if we detect a barcode, else a bad scan and nove to bad folder
                var foundBarcodes = new List<Tuple<Int64, int, int>>();
                using (var barcodeEngine = new SDTBarcodeEngine("SDTHC-ARPRF-VDKHV-GRJLE-NBFKL-MKLPN-LNAEA"))
                {
                  barcodeEngine.SetReadInputTypes(SDTBarcodeEngine.SDTBARCODETYPE_DATAMATRIX);
                  barcodeEngine.SetReadInputDirections(SDTBarcodeEngine.SDTREADDIRECTION_ALL /*SDTREADDIRECTION_TTB + SDTBarcodeEngine.SDTREADDIRECTION_BTT*/);

                  for (var currPage = 0; currPage < actualPageCount; currPage++)
                  {
                    if (barcodeEngine.ReadImageFile(fullFileName, currPage) == 0) // successfully opened and read the file
                    {
                      var barcodeCount = barcodeEngine.GetResultsCount();
                      if (barcodeCount > 0)
                      {
                        _log.Information("Found {Barcodes} barcodes in file {File}, page {Page}", barcodeCount, fullFileName, currPage);

                        for (var barcodeNum = 0; barcodeNum < barcodeCount; barcodeNum++)
                        {
                          var data = barcodeEngine.GetResultValue(barcodeNum);
                          if (!string.IsNullOrEmpty(data))
                          {
                            var parameters = data.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);  // Barcode parameters are delimited with '*' -> DocId*Page*PageCount
                            Int64 storageId;
                            int pageNum;
                            int pageCount;

                            if (parameters.Length >= 3 && Int64.TryParse(parameters[0], out storageId) &&
                              int.TryParse(parameters[1], out pageNum) && int.TryParse(parameters[2], out pageCount) && pageNum <= pageCount && storageId > 0)
                            {
                              var type = barcodeEngine.GetResultType(barcodeNum);
                              var dir = barcodeEngine.GetResultDirection(barcodeNum);

                              _log.Information("Read: '{Data}', type: {Type}, direction: {Direction}", data, type, dir);
                              foundBarcodes.Add(new Tuple<Int64, int, int>(storageId, pageNum, pageCount));
                            }
                            else
                            {
                              _log.Error("Barcode did not contain expected parameters: {Data}- skipped", data);
                            }
                          }
                        }
                      }
                      else
                      {
                        _log.Warning("Failed to locate any barcodes {File}", fileNameOnly);
                      }
                    }
                    else
                    {
                      _log.Warning("Failed to open image {File}, page {Page}", fileNameOnly, currPage);
                    }
                  }
                }

                if (foundBarcodes.Count == 0)
                {
                  dest = Path.Combine(FTP_INCOMPLETE_PATH, fileNameOnly);
                  File.Move(fullFileName, dest);
                  throw new Exception(string.Format("Could not process file '{0}'- no barcodes were detected", fullFileName));
                }

                var documents = foundBarcodes.GroupBy(s => s.Item1); // Group pages by StorageId

                var completeDocCount = 0;
                foreach (var docPages in documents)
                {
                  var thisDocPageCount = docPages.Select(s => s.Item2).Distinct().Count(); // Count unique page numbers
                  var thisDocExpectedPageCount = docPages.First().Item3;                   // Get expected number of page numbers
                  if (thisDocPageCount == thisDocExpectedPageCount)
                  {
                    completeDocCount++;
                  }
                }

                if (completeDocCount == 0) // we do not have a single complete document
                {
                  dest = Path.Combine(FTP_INCOMPLETE_PATH, fileNameOnly);
                  File.Move(fullFileName, dest);
                  throw new Exception(string.Format("Could not process file '{0}'- some pages detected, but no complete document could be detected", fileNameOnly));
                }
                #endregion
              }
              else
              {
                throw new Exception(string.Format("Could not process file '{0}'- no pages detected", fileNameOnly));
              }

              #region Upload & have server process
              List<StorageInfo> docs = null;
              using (var client = new Atlas.Document.WCF.Client.ClientProxies.DocumentAdminClient(TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(60)))
              {
                #region Chunk upload the file
                var fileChunkId = client.ChunkStartUpload();

                using (var data = File.OpenRead(fullFileName))
                {
                  var uploadTime = Stopwatch.StartNew();
                  data.Seek(0, SeekOrigin.Begin);
                  var destFileSize = 0L;

                  var buffer = new byte[CHUNK_BYTE_UPLOAD_SIZE];
                  var bytesRead = 0;
                  while ((bytesRead = data.Read(buffer, 0, buffer.Length)) > 0)
                  {
                    if (bytesRead != buffer.Length) // Our last buffer read
                    {
                      Array.Resize(ref buffer, bytesRead);
                    }
                    destFileSize = client.ChunkAppendBytes(fileChunkId, buffer); // Returns new file size
                  }

                  // Be 100% sure there was no miscommunication and server has the complete file
                  if (destFileSize != data.Length)
                  {
                    throw new Exception(string.Format("Invalid server file size. Server reports: {0}, local: {1}", destFileSize, fullFileName.Length));
                  }

                  _log.Information("Successfully uploaded scanned file {FileName} with size {FileSize} in {Elapsed}ms", fileNameOnly, data.Length, uploadTime.ElapsedMilliseconds);
                }
                #endregion

                #region Have the server process the uploaded file
                _log.Information("Server started processing scan {FileName}", fileNameOnly);
                docs = client.StoreScannedFileChunked(fileChunkId, Atlas.DataSync.WCF.Client.ConversionUtils.ToDocumentFormat(fullFileName), false);
                _log.Information("Server processed scan {FileName} and returned {@DocStorage}", fileNameOnly, docs);
                #endregion
              }
              #endregion

              processed = true; //  docs != null && docs.Count > 0;   //   We can't keep uploading the document, if not WCF error, we have done our part...
            }
            catch (Exception err)
            {
              _log.Error(err, "Error processing {File}", fileNameOnly);
            }

            if (processed)
            {
              try
              {
                File.Move(fullFileName, dest);
              }
              catch (Exception err)
              {
                _log.Error(err, "Error while moving file {Source} to {Dest}", fileNameOnly, dest);
              }
            }
          }
        }
      }
      finally
      {
        _inTimer = false;
      }
    }


    /// <summary>
    /// Determines number of pages in a PDF
    /// </summary>
    /// <param name="fileName">The full PDF filename</param>
    /// <returns>The number of pages indicated by the PDF</returns>
    private static int GetNoOfPagesPDF(string fileName)
    {
      var pdfText = File.ReadAllText(fileName);
      var regx = new Regex(@"/Type\s*/Page[^s]");
      var matches = regx.Matches(pdfText);
      return matches.Count;
    }



    /// <summary>
    /// Determines number of frames (pages) in a TIFF
    /// </summary>
    /// <param name="fileName">The full TIFF filename</param>
    /// <returns>The number of frames (pages) contained in the TIFF</returns>
    private static int GetNoOfPagesTIFF(string fileName)
    {
      using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      {
        var tiff = new TiffBitmapDecoder(fs, BitmapCreateOptions.None, BitmapCacheOption.None);
        return tiff.Frames.Count;
      }
    }

    #endregion


    #region FTP events

    private void ftpServer_SessionCreated(object sender, TCP_ServerSessionEventArgs<FTP_Session> e)
    {
      e.Session.Authenticate += OnFtpServer_Authenticate;
      e.Session.CurrentDir = "/";
      e.Session.Cwd += OnFtpServer_Cwd;
      e.Session.GetDirListing += OnFtpServer_GetDirListing;
      e.Session.Stor += OnFtpServer_Stor;
      e.Session.Appe += OnFtpServer_Appe;
      e.Session.Disonnected += OnFtpServer_Disconnected;
      e.Session.GetFileSize += OnFTPServer_GetFileSize;
    }


    static void OnFTPServer_GetFileSize(object sender, FTP_e_GetFileSize e)
    {
      _log.Information("FTP GetFileSize- {0}", e.FileName);

      e.FileSize = 0;
      var fileName = FtpServerGetRealPath(e.FileName);
      if (File.Exists(fileName))
      {
        var fi = new FileInfo(fileName);
        e.FileSize = fi.Length;
      }
    }


    /// <summary>
    /// Disconnected
    /// TODO: This needs to be a task/thread handler?
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFtpServer_Disconnected(object sender, EventArgs e)
    {
      _log.Information("FTP client disconnected");
    }


    /// <summary>
    /// Append to a file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFtpServer_Appe(object sender, FTP_e_Appe e)
    {
      _log.Information("FTP APPE- {Filename}", e.FileName);
      e.FileStream = File.Open(FtpServerGetRealPath(e.FileName), FileMode.OpenOrCreate);

      _tmrScanFTP.Restart();
    }


    /// <summary>
    /// Store a file
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFtpServer_Stor(object sender, FTP_e_Stor e)
    {
      _log.Information("FTP STOR- {Filename}", e.FileName);
      e.FileStream = File.Open(FtpServerGetRealPath(e.FileName), FileMode.Create);

      _tmrScanFTP.Restart();
    }


    /// <summary>
    /// Get directory listing- return files
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFtpServer_GetDirListing(object sender, FTP_e_GetDirListing e)
    {
      _log.Information("FTP GetDirListing- {Path}", e.Path);
      var files = Directory.GetFiles(FtpServerGetRealPath());
      foreach (var file in files)
      {
        var fileInfo = new FileInfo(file);
        e.Items.Add(new FTP_ListItem(fileInfo.Name, fileInfo.Length, fileInfo.LastWriteTime, false));
      }
    }


    /// <summary>
    /// Change working directory- do nothing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OnFtpServer_Cwd(object sender, FTP_e_Cwd e)
    {
      _log.Information("FTP CWD- {DirName}", e.DirName);
      e.Response = new FTP_t_ReplyLine[] { new FTP_t_ReplyLine(250, string.Format("\"{0}\" CWD command successful", e.DirName), true) };
    }


    /// <summary>
    /// Authenticate- accept anything
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnFtpServer_Authenticate(object sender, FTP_e_Authenticate e)
    {
      _log.Information("FTP Authenticate- {User}", e.User);
      e.IsAuthenticated = true;
    }


    /// <summary>
    /// Return the actual physical file path
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    private static string FtpServerGetRealPath(string fileName = "")
    {
      return Path.Combine(FTP_ROOT_PATH, fileName);
    }

    #endregion


    #region Private fields

    /// <summary>
    /// FTP server for file uploads from branch scanner
    /// </summary>
    private readonly FTP_Server _ftpServer;

    /// <summary>
    /// Timer to process uploaded files, after a period has elapsed
    /// </summary>
    private readonly Stopwatch _tmrScanFTP = new Stopwatch();

    /// <summary>
    /// Timer to check on FTP
    /// </summary>
    private readonly Timer _checkFTP;

    /// <summary>
    /// Logging
    /// </summary>
    private static readonly ILogger _log = Log.Logger.ForContext<ScannerFtpServer>();

    /// <summary>
    /// Are we currently in the timer event?
    /// </summary>
    private static bool _inTimer = false;

    #endregion


    #region Private consts

    /// <summary>
    /// Incoming path for FTP server uploads 
    /// </summary>
    private const string FTP_ROOT_PATH = @"C:\Atlas\LMS\DataSync\FTP\Incoming";

    /// <summary>
    /// Path for successfully processed scans
    /// </summary>
    private const string FTP_PROCESSED_PATH = @"C:\Atlas\LMS\DataSync\FTP\Done";

    /// <summary>
    /// Path for scans where barcode could not be detected/incomplete documents
    /// </summary>
    private const string FTP_INCOMPLETE_PATH = @"C:\Atlas\LMS\DataSync\FTP\Incomplete";


    /// <summary>
    /// Byte upload chunk size to use, in bytes
    /// </summary>
    private const int CHUNK_BYTE_UPLOAD_SIZE = 86400;

    #endregion

  }
}
