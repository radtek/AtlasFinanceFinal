using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Web.Mvc;
using WebMatrix.WebData;
using Atlas.Online.Web.Models;

namespace Atlas.Online.Web.Filters
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class InitializeSimpleMembershipAttribute : ActionFilterAttribute
  {
    internal static SimpleMembershipInitializer _initializer;
    internal static object _initializerLock = new object();
    internal static bool _isInitialized;

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      // Ensure ASP.NET Simple Membership is initialized only once per app start
      LazyInitializer.EnsureInitialized(ref _initializer, ref _isInitialized, ref _initializerLock);
    }

    internal class SimpleMembershipInitializer
    {
      public SimpleMembershipInitializer()
      {
        Database.SetInitializer<UsersContext>(null);

        try
        {
          using (var context = new UsersContext())
          {
            if (!context.Database.Exists())
            {
              // Create the SimpleMembership database without Entity Framework migration schema
              ((IObjectContextAdapter)context).ObjectContext.CreateDatabase();
            }
          }

          WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "Email", autoCreateTables: true);          
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException("The ASP.NET Simple Membership database could not be initialized. For more information, please see http://go.microsoft.com/fwlink/?LinkId=256588", ex);
        }
      }
    }
  }
}
