using Atlas.Online.Web.Filters;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Online.Web
{
  public class FilterConfig
  {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
      filters.Add(new AuthorizeAttribute());
      filters.Add(new HandleErrorAttribute());
      filters.Add(new CustomHandleErrorAttribute());
    }
  }
}