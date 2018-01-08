using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Target.Controllers
{
  public class LoanMixController : AppController
  {
    // GET: Target/LoanMix
    public ActionResult Index()
    {
      return View();
    }
  }
}