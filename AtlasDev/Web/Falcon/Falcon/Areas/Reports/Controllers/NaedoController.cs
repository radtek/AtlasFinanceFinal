﻿using System.Web.Mvc;
using Falcon.Base;

namespace Falcon.Areas.Reports.Controllers
{
  [Authorize]
  public class NaedoController : AppController
  {
    public ActionResult Index()
    {
      return View();
    }

    //[Authorize]
    //public ActionResult Avs()
    //{
    //  return View();
    //}
  }
}