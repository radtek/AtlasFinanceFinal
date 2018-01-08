using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Target.Controllers
{
  public class DailySalesController : AppController
  {
    // GET: Target/DailySales
    public ActionResult Index()
    {
      return View();
    }
  }
}