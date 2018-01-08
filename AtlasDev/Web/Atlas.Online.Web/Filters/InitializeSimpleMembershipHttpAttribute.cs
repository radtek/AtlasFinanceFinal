using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using WebMatrix.WebData;
using Atlas.Online.Web.Models;
using System.Web.Http.Filters;

namespace Atlas.Online.Web.Filters
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class InitializeSimpleMembershipHttpAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
    {
      // Ensure ASP.NET Simple Membership is initialized only once per app start
      LazyInitializer.EnsureInitialized(ref InitializeSimpleMembershipAttribute._initializer, ref InitializeSimpleMembershipAttribute._isInitialized, ref InitializeSimpleMembershipAttribute._initializerLock);
    }
  }
}
