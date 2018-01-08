using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.PayoutEngine.Business
{
  public static class FileHelper
  {
    public static void CreateFileAndMoveToPath(string text, string path)
    {
      using (TextWriter textWriter = new StreamWriter(path))
      {
        textWriter.Write(text);
        textWriter.Flush();
        textWriter.Close();
      }
    }
  }
}
