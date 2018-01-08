using System.Web.Mvc;

namespace Falcon
{
  public class FilterConfig
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new HandleErrorAttribute());
      
      //filters.Add(new ClaimsAuthorizeAttribute());
    }
  }
}
