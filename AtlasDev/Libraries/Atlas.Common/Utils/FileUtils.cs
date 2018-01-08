/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *    Useful file utilities
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *       
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */
using System;
using System.IO;


namespace Atlas.Common.Utils
{
  public static class FileUtils
  {
    /// <summary>
    /// Returns a unique, temporary file name
    /// </summary>
    /// <returns>string- unique, temporary file name</returns>
    public static string GetTempFileName()
    {
      return Path.Combine(Path.GetTempPath(), string.Format("{0}.tmp", Guid.NewGuid().ToString("N")));
    }

  }
}
