using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Atlas.Online.Web.Common.Extensions;
using System.Globalization;

namespace Atlas.Online.Web.Extensions
{
  public static class AffordabilityExtension
  {
    public static MvcHtmlString AffordabilityDiffFor<TModel, TValue>(this HtmlHelper<TModel> instance, Expression<Func<TModel, TValue>> expression, string format = null, string negFormat = null, string posFormat = null, string negClass = null, string posClass = null)
    {
      var tag = new TagBuilder("strong");
      var metadata = ModelMetadata.FromLambdaExpression(expression, instance.ViewData);

      double value = Convert.ToDouble(metadata.Model);
      string text = value.ToString();
      if (value < 0)
      {
        if (!String.IsNullOrEmpty(negClass)) 
        {
          tag.AddCssClass(negClass);
        }

        if (negFormat != null) 
        {
          text = String.Format(CultureInfo.CurrentCulture, negFormat, value);
        }
      }
      else if (value > 0)
      {
        if (!String.IsNullOrEmpty(posClass))
        {
          tag.AddCssClass(posClass);
        }

        if (posFormat != null)
        {
          text = String.Format(CultureInfo.CurrentCulture, posFormat, value);
        }
      }
      else
      {
        return MvcHtmlString.Empty;
      }

      if (!String.IsNullOrEmpty(format))
      {
        text = String.Format(CultureInfo.CurrentCulture, format, value);
      }

      tag.SetInnerText(text);

      return tag.ToMvcHtmlString();
    }
  }
}