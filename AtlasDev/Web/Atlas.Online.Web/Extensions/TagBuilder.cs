using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Online.Web.Extensions
{
  public static class TagBuilderExtensions
  {
    internal static MvcHtmlString ToMvcHtmlString(this TagBuilder tagBuilder, TagRenderMode renderMode = TagRenderMode.Normal)
    {
      Debug.Assert(tagBuilder != null);
      return new MvcHtmlString(tagBuilder.ToString(renderMode));
    }
  }
}