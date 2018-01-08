using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Atlas.Online.Web.Core.Attributes
{
  public class RestrictAndRedirectAttribute : AuthorizeAttribute
  {
    #region Public Properties

    public String RedirectURL { get; set; }

    #endregion

    #region Constructor

    public RestrictAndRedirectAttribute()
      : base()
    {
    }

    #endregion

    #region Overrides

    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
      if (String.IsNullOrEmpty(RedirectURL))
        base.HandleUnauthorizedRequest(filterContext);

      else
        filterContext.Result = new RedirectResult(RedirectURL);
    }

    #endregion
  }
}