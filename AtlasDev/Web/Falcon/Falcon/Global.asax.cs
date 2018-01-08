using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Validation.Providers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Falcon.App_Start;
using Falcon.Gyrkin.Library.Handler;
using Falcon.Gyrkin.Library.Security.TokenCache.Providers;
using Thinktecture.IdentityModel.Web;
using ModelValidatorProvider = System.Web.Http.Validation.ModelValidatorProvider;

namespace Falcon
{
  public class MvcApplication : HttpApplication
  {
    protected void Application_Start()
    {
      AreaRegistration.RegisterAllAreas();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
      AutofacConfig.ConfigureContainer();

      AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
      PassiveSessionConfiguration.ConfigureSessionCache(new SessionTokenCacheRepository());
      GlobalConfiguration.Configuration.Services.RemoveAll(typeof(ModelValidatorProvider),v => v is InvalidModelValidatorProvider);
      GlobalConfiguration.Configuration.MessageHandlers.Add(new CompressHandler());
    }

    public override void Init()
    {
      PassiveModuleConfiguration.CacheSessionsOnServer();
      PassiveModuleConfiguration.SuppressLoginRedirectsForApiCalls();
      base.Init();
    }

    public void Application_OnError()
    {
      var ex = Context.Error;
      if (ex is SecurityTokenException)
      {
        Context.ClearError();
        if (FederatedAuthentication.SessionAuthenticationModule != null)
        {
          FederatedAuthentication.SessionAuthenticationModule.SignOut();
        }
        Response.Redirect("~/");
      }
    }
  }
}
