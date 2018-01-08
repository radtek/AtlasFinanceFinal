using CoffeeSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;

namespace AtlasOnline.Web.Core.Compiler
{
  public class CoffeeCompiler : IBundleTransform
  {

    public void Process(BundleContext context, BundleResponse response)
    {
      CoffeeScriptEngine scriptEngine = new CoffeeSharp.CoffeeScriptEngine();
      string compiledCoffeeScript = String.Empty;
      foreach (var file in response.Files)
      {
        using(var reader = new StreamReader(file.FullName))
        {
          compiledCoffeeScript += scriptEngine.Compile(reader.ReadToEnd());
        }
      }
      response.Content = compiledCoffeeScript;
      response.ContentType = "text/javacript";
      response.Cacheability = System.Web.HttpCacheability.Public;
    }
  }
}
