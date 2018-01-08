using Atlas.Online.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Online.Web.Helpers
{
  // Modified from http://kyleleneau.com/blog/2010/10/01/simple-asp-net-mvc-navigation-menu/
  public static class ViewHelper
  {

    public static string CurrentController(this HtmlHelper helper)
    {
        return helper.ViewContext.Controller.ControllerContext.RouteData.GetRequiredString("controller");
    }

    public static string CurrentAction(this HtmlHelper helper)
    {
        return helper.ViewContext.Controller.ControllerContext.RouteData.GetRequiredString("action");
    }

    public static bool IsCurrentController(this HtmlHelper helper, string testController)
    {
        return helper.CurrentController().Equals(testController, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsCurrentControllerAction(this HtmlHelper helper, string controller, string action)
    {      
        return helper.IsCurrentController(controller) &&
          helper.CurrentAction().Equals(action, StringComparison.OrdinalIgnoreCase);
    }
   
    public static MvcHtmlString NavItem(this HtmlHelper helper, string controller)
    {
        return helper.NavItem(controller, controller, "Index", () => helper.IsCurrentController(controller));
    }

    public static MvcHtmlString NavItem(this HtmlHelper helper, string text, string controller, string action)
    {
        return helper.NavItem(text, controller, action, () => helper.IsCurrentControllerAction(controller, action));
    }

    public static MvcHtmlString NavItem(this HtmlHelper helper, string text, string controller, string action, Func<bool> isActive)
    {
        var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
        var url = urlHelper.RouteUrl(new { controller = controller, action = action });

        var active = isActive();

        return NavItem(text, url, active, controller.ToLower());
    }

    public static MvcHtmlString NavItems(this HtmlHelper helper, Dictionary<string, string> items)
    {
      var result = new StringBuilder();
      foreach (var item in items)
      {
        var text = item.Key;
        var path = item.Value;
        var url = path;

        // Parse as controller#action
        if (path[0].Equals('@')) {
          var parts = path.Substring(1).Split('#');
          if (parts.Length == 2)
          {
            var controller = parts[0];
            var action = parts[1];

            result.Append(helper.NavItem(item.Key, controller, action, () => helper.IsCurrentControllerAction(controller, action)));
            continue;            
          }          
        }

        result.Append(NavItem(text, path, false));
      }

      return MvcHtmlString.Create(result.ToString());
    }

    private static MvcHtmlString NavItem(string text, string url, bool active, string extraCssClass = "")
    {
      TagBuilder li = new TagBuilder("li");

      li.AddCssClass("menu-item");

      if (active)
      {
        li.AddCssClass("current-menu-item");
      }

      if (!String.IsNullOrEmpty(extraCssClass))
      {
        li.AddCssClass(extraCssClass);
      }

      TagBuilder a = new TagBuilder("a");
      a.MergeAttribute("href", url);
      a.InnerHtml = text;

      li.InnerHtml = a.ToString();

      return li.ToMvcHtmlString();
    }

    public static MvcHtmlString SubNavItem(this HtmlHelper helper, string text, string controller, string action, Func<bool> isActive)
    {
        var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
        var url = urlHelper.RouteUrl(new { controller = controller, action = action });

        var active = isActive();
        var extraCss = string.Format("{0}_{1}", controller, action).ToLower();

        return NavItem(text, url, active, extraCss);
    }


    public static MvcHtmlString LoginStatusNavItem(this HtmlHelper helper)
    {
        if (HttpContext.Current.Request.IsAuthenticated)
            return helper.NavItem("Log Out", "Account", "Logout");

        return helper.NavItem("Login", "Account", "Login");
    }

    public static string SubNavListItems(this HtmlHelper helper)
    {
        var sb = new StringBuilder();
        var worker = ControllerNavigationItemCollection.Current;
        var name = helper.ViewContext.Controller.GetType().Name;

        if (worker.Controllers.ContainsKey(name))
        {
            var items = worker.Controllers[name].OrderBy(i => i.SortOrder);

            foreach (var navItem in items)
            {
                var item = navItem;

                if (navItem.IsSecure)
                {
                    if (HttpContext.Current.Request.IsAuthenticated)
                    {
                        sb.Append(helper.SubNavItem(navItem.Text, navItem.Controller, navItem.Action, () => helper.IsCurrentControllerAction(item.Controller, item.Action)));
                    }
                }
                else
                {
                    sb.Append(helper.SubNavItem(navItem.Text, navItem.Controller, navItem.Action, () => helper.IsCurrentControllerAction(item.Controller, item.Action)));
                }
            }
        }

        return sb.ToString();
    }
    }

    /// <summary>
    /// This Attribute should go on Controller Actions
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal sealed class NavigationItemAttribute : Attribute
    {
    public NavigationItemAttribute(string text)
        : this(text, 0) { }

    public NavigationItemAttribute(string text, int order)
    {
        Text = text;
        SortOrder = order;
    }

    public string Text { get; private set; }
    public int SortOrder { get; set; }
    }

    /// <summary>
    /// POCO class for storage of reflection findings.
    /// </summary>
    public class ControllerNavigationItem
    {
    public string Controller { get; set; }
    public string Action { get; set; }
    public int SortOrder { get; set; }
    public string Text { get; set; }
    public bool IsSecure { get; set; }
    }

    /// <summary>
    /// Singleton object to load controllers and actions into a hashtable for lookup later.
    /// </summary>
    public sealed class ControllerNavigationItemCollection
    {
    static ControllerNavigationItemCollection _instance;
    static readonly object padlock = new object();

    public IDictionary<string, IEnumerable<ControllerNavigationItem>> Controllers { get; private set; }

    ControllerNavigationItemCollection()
    {
        Controllers = new Dictionary<string, IEnumerable<ControllerNavigationItem>>();
        PopulateCollection();
    }

    private void PopulateCollection()
    {
        var asm = Assembly.GetExecutingAssembly();
        var controllers = (from t in asm.GetTypes()
                            where
                                typeof(Controller).IsAssignableFrom(t) &&
                                typeof(Controller) != t
                            select t).ToList();

        controllers.ForEach(t => Controllers.Add(t.Name, GetControllerNavItems(t)));
    }

    private static IEnumerable<ControllerNavigationItem> GetControllerNavItems(Type controllerType)
    {
        var controllerDescriptor = new ReflectedControllerDescriptor(controllerType);

        var actions = (from a in controllerDescriptor.GetCanonicalActions()
                        let subNavAttr = (NavigationItemAttribute)a.GetCustomAttributes(typeof(NavigationItemAttribute), false).SingleOrDefault()
                        let authorize = a.GetCustomAttributes(typeof(AuthorizeAttribute), false).SingleOrDefault()
                        where a.IsDefined(typeof(NavigationItemAttribute), false)
                        select new ControllerNavigationItem
                        {
                            Action = a.ActionName,
                            Controller = a.ControllerDescriptor.ControllerName,
                            IsSecure = authorize != null,
                            SortOrder = subNavAttr.SortOrder,
                            Text = subNavAttr.Text
                        }).AsEnumerable();

        return actions;
    }

    public static ControllerNavigationItemCollection Current
    {
        get
        {
            lock (padlock)
            {
                return _instance ?? (_instance = new ControllerNavigationItemCollection());
            }
        }
    }
  }
}