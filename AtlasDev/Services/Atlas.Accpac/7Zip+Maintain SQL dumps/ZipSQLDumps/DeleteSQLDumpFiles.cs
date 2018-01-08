using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace ZipSQLDumps
{
  class DeleteFiles
  {
    /// <summary>
    /// Deletes all '*.BAK' files, located exactly 1 folder below given 'fileDir'
    /// </summary>
    /// <param name="directory">Base folder/directory</param>
    /// <returns>Any errors, empty string if successful</returns>
    public static string Execute(string directory, string extension = "bak")
    {
      var errors = new StringBuilder();

      try
      {
        var files = Directory.GetFiles(directory, $"*.{extension}", SearchOption.AllDirectories);

        foreach (var file in files)
        {
          try
          {
            File.Delete(file);
          }
          catch (Exception delErr)
          {
            errors.AppendFormat("Error deleting file '{0}', error: [{1}]\r\n", file, delErr.Message);
          }
        }
      }
      catch (Exception err)
      {
        errors.AppendFormat("Unexpected error in [DeleteSQLDumpFiles.Execute]: '{0}'\r\n", err.Message);
      }

      return errors.ToString();
    }
  }
}
