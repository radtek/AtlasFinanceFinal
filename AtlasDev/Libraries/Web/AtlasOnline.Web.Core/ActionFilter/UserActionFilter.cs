using System.Web.Mvc;

namespace AutoFac.Web.Core.ActionFilter
{
  //Inject a ViewBag object to Views for getting information about an authenticated user
  public class UserFilter : ActionFilterAttribute
  {
    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      UserModel userModel;
      if (filterContext.Controller.ViewBag.UserModel == null)
      {
        userModel = new UserModel();
        filterContext.Controller.ViewBag.UserModel = userModel;
      }
      else
      {
        userModel = filterContext.Controller.ViewBag.UserModel as UserModel;
      }

      base.OnActionExecuted(filterContext);
    }
  }

  public class UserModel
  {
    public bool IsUserAuthenticated { get; set; }

    public string UserName { get; set; }

    public string RoleName { get; set; }
  }
}