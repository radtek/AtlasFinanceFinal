using Atlas.Online.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Validation.Providers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Atlas.Common.Utils;
using Atlas.Online.Web.Helpers;

namespace Atlas.Online.Web
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();

      //WebApiConfig.Register(GlobalConfiguration.Configuration);
      GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
      AuthConfig.RegisterAuth();

      // http://stackoverflow.com/questions/12305784/dataannotation-for-required-property
      GlobalConfiguration.Configuration.Services.RemoveAll(
        typeof (System.Web.Http.Validation.ModelValidatorProvider),
        v => v is InvalidModelValidatorProvider
        );

      // Build DB
      var xpoUoW = new SharedServices().XpoUnitOfWork;

      DomainMapper.Map();

    }
  }
}