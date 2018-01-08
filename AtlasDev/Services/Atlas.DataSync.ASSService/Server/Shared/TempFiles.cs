/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     24 May 2013 - Created
 * 
 * 
 *  Comments:
 *  ------------------
 *     
 * 
 * ----------------------------------------------------------------------------------------------------------------- */
 
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Atlas.Common.Interface;


namespace ASSServer.Shared
{
  /// <summary>
  /// Class to maintain temporary files which need to be deleted in a delayed fashion
  /// </summary>
  public static class TempFiles
  {
    /// <summary>
    /// Add file to temporary monitoring list, for deletion after a specified periof
    /// </summary>
    /// <param name="fileName"></param>
    public static void AddTempFile(string fileName)
    {
      var fileInfo = new FileInfo(fileName);
      lock (_locker)
      {
        _tempFiles.Add(fileInfo);
      }
    }


    /// <summary>
    /// Delete temporary files last written 'fileAgeMinutes' ago
    /// </summary>
    /// <param name="fileAgeMinutes">File age, in minutes</param>
    public static void DeleteFilesOlderThan(ILogging log, int fileAgeMinutes)
    {
      #region Get files older than fileAgeMinutes
      var filesToDelete = new List<string>();
      lock (_locker)
      {
        foreach (var item in _tempFiles)
        {
          if (DateTime.UtcNow > item.LastWriteTimeUtc.AddMinutes(fileAgeMinutes))
          {
            filesToDelete.Add(item.FullName);
          }
        }
      }
      #endregion

      #region Delete the files
      foreach (var file in filesToDelete)
      {
        if (File.Exists(file))
        {
          try
          {
            File.Delete(file);
          }
          catch (Exception err)
          {
            log.Error("DeleteFilesOlderThan", err);
          }
        }
      }
      #endregion

      #region Remove processed items from list
      lock (_locker)
      {
        _tempFiles.RemoveAll(s => filesToDelete.IndexOf(s.FullName) > -1);
      }
      #endregion
    }


    #region Private members

    private static readonly List<FileInfo> _tempFiles = new List<FileInfo>();
    private static readonly object _locker = new object();
 
    #endregion
  }
}
