﻿using System.Web.Mvc;

namespace Falcon.Areas.Reports
{
  public class ReportsAreaRegistration : AreaRegistration
  {
    public override string AreaName
    {
      get
      {
        return "Reports";
      }
    }

    public override void RegisterArea(AreaRegistrationContext context)
    {
      context.MapRoute(
          "ReportsManagement",
          "Reports/{controller}/{action}/{id}",
          new { action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}