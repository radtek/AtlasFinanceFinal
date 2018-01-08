using System;
using System.Collections.Generic;
using System.IO;


namespace Atlas.Services.DbfGenerate.QuartzTasks.Utils
{
  internal static class DirUtils
  {
    /// <summary>
    /// Move folder- allows moving files across different volumes (Directory.Move only supports the same volume)
    /// </summary>
    /// <param name="sourceFolder"></param>
    /// <param name="destFolder"></param>
    /// <remarks>Source: http://stackoverflow.com/questions/2947300/copy-a-directory-to-a-different-drive</remarks>
    internal static void MoveDirectory(string source, string target)
    {
      var stack = new Stack<Folders>();
      stack.Push(new Folders(source, target));

      while (stack.Count > 0)
      {
        var folders = stack.Pop();
        Directory.CreateDirectory(folders.Target);
        foreach (var file in Directory.GetFiles(folders.Source, "*.*"))
        {
          string targetFile = Path.Combine(folders.Target, Path.GetFileName(file));
          if (File.Exists(targetFile))
          {
            File.Delete(targetFile);
            System.Threading.Thread.Sleep(500); // Give system time to flush changes to disk? We experience "Cannot create a file when that file already exists" ??
          }
          File.Move(file, targetFile);
        }

        foreach (var folder in Directory.GetDirectories(folders.Source))
        {
          stack.Push(new Folders(folder, Path.Combine(folders.Target, Path.GetFileName(folder))));
        }
      }
      Directory.Delete(source, true);
    }

    class Folders
    {
      public string Source { get; private set; }
      public string Target { get; private set; }

      public Folders(string source, string target)
      {
        Source = source;
        Target = target;
      }
    }
  }
}
