using Fabrik.Common;
using Fabrik.Common.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Online.Web.Controllers.Api
{
  [AllowAnonymous]
  public class SiteMapController : Controller
  {
    //
    // GET: /SiteMap/


    public ActionResult Index()
    {
      var sitemapItems = new List<SitemapItem>
        {
            new SitemapItem(Url.QualifiedAction("index", "home"), changeFrequency: SitemapChangeFrequency.Always, priority: 1.0),
            new SitemapItem(Url.QualifiedAction("about", "home"), lastModified: DateTime.Now),
            new SitemapItem(Url.QualifiedAction("questions", "home"), lastModified: DateTime.Now),
            new SitemapItem(Url.QualifiedAction("contact", "home"), lastModified: DateTime.Now),
        };

      return new SitemapResult(sitemapItems);
    }
  }
}