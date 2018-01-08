using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;

namespace Plugins.Sample
{
  [Export("Test",typeof(IController))]
  [PartCreationPolicy(CreationPolicy.NonShared)]
  public class TestController : Controller
  {
    public ActionResult Index()
    {
      return
        View(string.Format(@"{0}\{1}", AppDomain.CurrentDomain.RelativeSearchPath, @"Modules\Test\Views\Test\index.html"));
    }
  }
}