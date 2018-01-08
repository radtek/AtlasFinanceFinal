using Atlas.Collection.Engine.Business.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Collection.Engine.Business.Parsers
{
  public static class Importer
  {
    public static dynamic Import(string path)
    {
      if (path.Contains("REPLY"))
      {
        ReplyImporter importer = new ReplyImporter(path);
        return importer.ProcessFile();
      }
      else if (path.Contains("OUTPUT"))
      {
        OutputImporter output = new OutputImporter(path);
        return output.ProcessFile();
      }
      return null;
    }
  }
}
