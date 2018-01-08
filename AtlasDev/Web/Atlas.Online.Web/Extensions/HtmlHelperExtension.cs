using Atlas.Online.Web.Common.Serializers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Atlas.Online.Web.Extensions
{
  public static class HtmlHelperExtension
  {
    public static MvcHtmlString DisplayEnumFor<TModel, TValue>(this HtmlHelper<TModel> instance, Expression<Func<TModel, TValue>> expression)
    {
      var metadata = ModelMetadata.FromLambdaExpression(expression, instance.ViewData);

      if (!expression.ReturnType.IsEnum)
      {
        throw new InvalidOperationException(String.Format(
          "{0} is not an Enum property type.", expression.ReturnType
        ));
      }

      if (metadata.Model == null)
      {
        return MvcHtmlString.Empty;
      }

      string text = metadata.Model.ToString();
      FieldInfo field = expression.ReturnType.GetField(text, BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static);

      var description = field.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
      if (description != null)
      {
        text = description.Description;
      }

      return MvcHtmlString.Create(text);
    }

    public static MvcHtmlString DisplayDateTimeFor<TModel, TValue>(this HtmlHelper<TModel> instance, Expression<Func<TModel, TValue>> expression, string dateFormat = "ddd, d MMMM yyyy")
    {
      var metadata = ModelMetadata.FromLambdaExpression(expression, instance.ViewData);
      
      if (!expression.ReturnType.Equals(typeof(DateTime)))
      {
        throw new InvalidOperationException(String.Format(
          "{0} is not a DateTime property type.", expression.ReturnType
        ));
      }

      if (metadata.Model == null) 
      {
        return MvcHtmlString.Empty; 
      }

      DateTime datetime = (DateTime)metadata.Model;

      return MvcHtmlString.Create(datetime.ToString(dateFormat));
    }

    public static MvcHtmlString DisplayFormatFor<TModel, TValue>(this HtmlHelper<TModel> instance, Expression<Func<TModel, TValue>> expression,  string format)
    {
      var metadata = ModelMetadata.FromLambdaExpression(expression, instance.ViewData);

      return MvcHtmlString.Create(String.Format(CultureInfo.CurrentCulture, format, metadata.Model));
    }

    public static MvcHtmlString Json<T>(this HtmlHelper instance, T data)
    {
      return MvcHtmlString.Create(JsonNet.Serialize<T>(data, Newtonsoft.Json.TypeNameHandling.None));
    }

    public static MvcHtmlString TableCell(this HtmlHelper instance, object content, object htmlAttributes = null)
    {
      TagBuilder td = new TagBuilder("td");
      if (content != null)
      {
        td.InnerHtml = content.ToString();
      }

      if (htmlAttributes != null)
      {
        if (!(htmlAttributes is IDictionary<string, object>))
        {
          htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
        }

        td.MergeAttributes((IDictionary<string, object>)htmlAttributes);
      }

      return td.ToMvcHtmlString();
    }
  }
}