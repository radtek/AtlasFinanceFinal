/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2014 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Chunked file support utilities
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
using System.Configuration;
using System.IO;


namespace Atlas.DocServer.WCF.Implementation.Utils
{
  /// <summary>
  /// Support utilities  or chunked file uploading
  /// </summary>
  internal class ChunkedFileUtils
  {
    /// <summary>
    /// Get the physical chunked filename for specified correlation id
    /// </summary>
    /// <param name="correlationId"></param>
    /// <returns></returns>
    public static string GetChunkedFileName(string correlationId)
    {
      return Path.Combine(ChunkTempDir(), string.Format("{0}.tmp", correlationId));
    }


    /// <summary>
    /// Utility to get temp path to use for chunked file uploads
    /// </summary>
    /// <returns></returns>
    public static string ChunkTempDir()
    {
      return ConfigurationManager.AppSettings["ChunkUploadPath"] ?? "C:\\Atlas\\";
    }


    /// <summary>
    /// Utility to delete chunked files which are older than 'daysOld' days
    /// </summary>
    internal static void DeleteOldChunkedFiles(int daysOld = 3)
    {
      var sourceDir = ChunkTempDir();
      var files = Directory.GetFiles(sourceDir, "*.*");
      foreach(var file in files)
      {
        var fi = new FileInfo(file);
        if (DateTime.Now.Subtract(fi.CreationTime) > TimeSpan.FromDays(daysOld))
        {
          try { File.Delete(file); } catch { }
        }
      }
    }
  }
}
