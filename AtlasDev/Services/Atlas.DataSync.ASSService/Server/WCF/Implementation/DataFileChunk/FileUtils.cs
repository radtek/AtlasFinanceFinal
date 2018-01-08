using System;
using System.Linq;


namespace ASSServer.WCF.Implementation.DataFileChunk
{
  internal static class FileUtils
  {
    /// <summary>
    /// Checks file contains only valid characters
    /// </summary>
    /// <param name="fileName">The filename to be checked</param>
    /// <returns>true if valid, false if not</returns>
    internal static bool FileNameValid(string fileName)
    {
      var validChars = "ABCDEFGHIJKLMNOPQRTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890_ -.".ToCharArray();
      return fileName.ToCharArray().All(s => validChars.Contains(s));
    }

  }
}
