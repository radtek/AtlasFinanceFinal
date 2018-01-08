using System;
using System.IO;
using System.IO.Compression;

using Serilog;


namespace Atlas.ThirdParty.CS.Enquiry
{
  /// <summary>
  /// CompuScan scorecard XML utilities
  /// </summary>
  internal static class ScoreXmlUtils
  {    
    /// <summary>
    /// Extracts the XML and MHT files from ZIPped byte content (zippedScorecard)
    /// </summary>
    /// <param name="log"></param>
    /// <param name="zippedScorecard"></param>
    /// <param name="file"></param>
    /// <param name="scorecard"></param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
    internal static void UnzipScorecard(ILogger log, string zipPath, out byte[] file, out byte[] scorecard)
    {
      var methodName = "UnzipScorecard";
      file = null;
      scorecard = null;
      var tempDir = Path.Combine(Server.Training.QuartzTasks.ConfigHelper.GetTempPath(), Guid.NewGuid().ToString("N"));

      try
      {
        #region Extract the ZIP file to temp location
        using (var fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read))
        {
          using (var zipFile = new ZipArchive(fs, ZipArchiveMode.Read))
          {
            zipFile.ExtractToDirectory(tempDir);
          }
        }
        #endregion

        #region Load result
        var mhtFileName = Directory.GetFiles(tempDir, "Enq_SUMM_*.MHT"); // old V1 scorecard contains multiple MHT files- Summary provides most details
        if (mhtFileName.Length == 0)
        {
          mhtFileName = Directory.GetFiles(tempDir, "*.MHT"); // New V2 scorecard only contains a single MHT file...
          if (mhtFileName.Length == 0)
          {
            throw new Exception("MHT file missing from response");
          }
        }
        file = File.ReadAllBytes(mhtFileName[0]);

        var xmlFileName = Directory.GetFiles(tempDir, "*.XML");
        if (xmlFileName.Length == 0)
        {
          throw new Exception("XML file missing from response");
        }
        scorecard = File.ReadAllBytes(xmlFileName[0]);
        #endregion
      }
      finally
      {
        try
        {
          Directory.Delete(tempDir, true);
        }
        catch (Exception err)
        {
          log.Error(err, "{MethodName}", methodName);
        }
      }
    }
  }
}
