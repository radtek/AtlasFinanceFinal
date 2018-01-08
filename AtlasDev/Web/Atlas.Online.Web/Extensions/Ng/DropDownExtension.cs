using Atlas.Online.Web.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Online.Web.Extensions.Ng
{
  public static class DropDownExtensions
  {
    // DropDownList

    public static MvcHtmlString NgDropDownList(this HtmlHelper htmlHelper, string name)
    {
      return NgDropDownList(htmlHelper, name, null /* selectList */, null /* optionLabel */, null /* htmlAttributes */);
    }

    public static MvcHtmlString NgDropDownList(this HtmlHelper htmlHelper, string name, string optionLabel)
    {
      return NgDropDownList(htmlHelper, name, null /* selectList */, optionLabel, null /* htmlAttributes */);
    }

    public static MvcHtmlString NgDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList)
    {
      return NgDropDownList(htmlHelper, name, selectList, null /* optionLabel */, null /* htmlAttributes */);
    }

    public static MvcHtmlString NgDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
    {
      return NgDropDownList(htmlHelper, name, selectList, null /* optionLabel */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static MvcHtmlString NgDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
    {
      return NgDropDownList(htmlHelper, name, selectList, null /* optionLabel */, htmlAttributes);
    }

    public static MvcHtmlString NgDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string optionLabel)
    {
      return NgDropDownList(htmlHelper, name, selectList, optionLabel, null /* htmlAttributes */);
    }

    public static MvcHtmlString NgDropDownList(this HtmlHelper htmlHelper, string name, Type enumType,  string optionLabel = null)
    {
      var selectList = DropDownHelper.FromEnum(enumType);
      return NgDropDownList(htmlHelper, name, selectList, optionLabel, null /* htmlAttributes */);
    }

    public static MvcHtmlString NgDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
    {
      return NgDropDownList(htmlHelper, name, selectList, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static MvcHtmlString NgDropDownList(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
    {
      return DropDownListHelper(htmlHelper, metadata: null, expression: name, selectList: selectList, optionLabel: optionLabel, htmlAttributes: htmlAttributes);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes = null)
    {      
      return NgDropDownListFor(htmlHelper, expression, SelectionListFromExpression(htmlHelper, expression), null /* optionLabel */, htmlAttributes /* htmlAttributes */);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, Dictionary<string, object> htmlAttributes = null)
    {
      return NgDropDownListFor(htmlHelper, expression, SelectionListFromExpression(htmlHelper, expression), null /* optionLabel */, htmlAttributes /* htmlAttributes */);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
    {
      return NgDropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, null /* htmlAttributes */);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
    {
      return NgDropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, Dictionary<string, object> htmlAttributes)
    {
      return NgDropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, htmlAttributes);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
    {
      return NgDropDownListFor(htmlHelper, expression, selectList, null /* optionLabel */, htmlAttributes);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel)
    {
      return NgDropDownListFor(htmlHelper, expression, selectList, optionLabel, null /* htmlAttributes */);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
    {
      return NgDropDownListFor(htmlHelper, expression, selectList, optionLabel, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Users cannot use anonymous methods with the LambdaExpression type")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
    {
      if (expression == null)
      {
        throw new ArgumentNullException("expression");
      }

      ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

      return DropDownListHelper(htmlHelper, metadata, ExpressionHelper.GetExpressionText(expression), selectList, optionLabel, htmlAttributes);
    }

    private static MvcHtmlString DropDownListHelper(HtmlHelper htmlHelper, ModelMetadata metadata, string expression, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
    {
      if (!htmlAttributes.ContainsKey("ng-model"))
      {
        htmlAttributes.Add("ng-model", FormsExtension.ApplyNgObjectPrefix(htmlHelper, expression));
      }

      return SelectInternal(htmlHelper, metadata, optionLabel, expression, selectList, allowMultiple: false, htmlAttributes: htmlAttributes);
    }

    // ListBox

    public static MvcHtmlString NgListBox(this HtmlHelper htmlHelper, string name)
    {
      return NgListBox(htmlHelper, name, null /* selectList */, null /* htmlAttributes */);
    }

    public static MvcHtmlString NgListBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList)
    {
      return NgListBox(htmlHelper, name, selectList, (IDictionary<string, object>)null);
    }

    public static MvcHtmlString NgListBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, object htmlAttributes)
    {
      return NgListBox(htmlHelper, name, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static MvcHtmlString NgListBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
    {
      return ListBoxHelper(htmlHelper, metadata: null, name: name, selectList: selectList, htmlAttributes: htmlAttributes);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
    {
      return NgListBoxFor(htmlHelper, expression, selectList, null /* htmlAttributes */);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
    {
      return NgListBoxFor(htmlHelper, expression, selectList, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Users cannot use anonymous methods with the LambdaExpression type")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
    {
      if (expression == null)
      {
        throw new ArgumentNullException("expression");
      }

      ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);

      return ListBoxHelper(htmlHelper,
                            metadata,
                            ExpressionHelper.GetExpressionText(expression),
                            selectList,
                            htmlAttributes);
    }

    private static MvcHtmlString ListBoxHelper(HtmlHelper htmlHelper, ModelMetadata metadata, string name, IEnumerable<SelectListItem> selectList, IDictionary<string, object> htmlAttributes)
    {
      if (!htmlAttributes.ContainsKey("ng-model"))
      {
        htmlAttributes.Add("ng-model", name);
      }

      return SelectInternal(htmlHelper, metadata, optionLabel: null, name: name, selectList: selectList, allowMultiple: true, htmlAttributes: htmlAttributes);
    }

    // Helper methods

    private static IEnumerable<SelectListItem> GetSelectData(this HtmlHelper htmlHelper, string name)
    {
      object o = null;
      if (htmlHelper.ViewData != null)
      {
        o = htmlHelper.ViewData.Eval(name);
      }
      if (o == null)
      {
        throw new InvalidOperationException(
            String.Format(
                CultureInfo.CurrentCulture,
                "Missing select data",
                name,
                "IEnumerable<SelectListItem>"));
      }
      IEnumerable<SelectListItem> selectList = o as IEnumerable<SelectListItem>;
      if (selectList == null)
      {
        throw new InvalidOperationException(
            String.Format(
                CultureInfo.CurrentCulture,
                "Wrong selection data type",
                name,
                o.GetType().FullName,
                "IEnumerable<SelectListItem>"));
      }
      return selectList;
    }

    internal static string ListItemToOption(SelectListItem item)
    {
      TagBuilder builder = new TagBuilder("option")
      {
        InnerHtml = HttpUtility.HtmlEncode(item.Text)
      };

      if (item.Value != null)
      {
        builder.Attributes["value"] = item.Value;
      }
      else
      {
        builder.Attributes["disabled"] = "disabled";
      }

      if (item.Selected)
      {
        builder.Attributes["selected"] = "selected";
      }
      return builder.ToString(TagRenderMode.Normal);
    }


    private static IEnumerable<SelectListItem> SelectionListFromExpression<TModel, TProperty>(HtmlHelper instance, Expression<Func<TModel, TProperty>> expression)
    {
      // Suport enum types out of the box
      SelectListItem[] selectList = null;

      string name = ExpressionHelper.GetExpressionText(expression);
      string fullName = instance.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
      object value = instance.GetModelStateValue(fullName, typeof(string));

      if (value == null) 
      {
        value = instance.EvalString(fullName);
      }

      Type returnType = Nullable.GetUnderlyingType(expression.ReturnType) ?? expression.ReturnType;
      if (returnType.IsEnum)
      {
        selectList = DropDownHelper.FromEnum(returnType, value);
      }
      // Support range selection from attribute
      else
      {
        var rangeAttr = ((MemberExpression)expression.Body).Member.GetCustomAttributes(typeof(RangeAttribute), false).FirstOrDefault();
        if (rangeAttr != null)
        {
          var min = Convert.ToInt32(((RangeAttribute)rangeAttr).Minimum);
          var max = Convert.ToInt32(((RangeAttribute)rangeAttr).Maximum);

          selectList = new SelectListItem[max - min + 2];
          selectList[0] = new SelectListItem()
          {
            Text = "Make Selection...",
            Value = String.Empty
          };

          for (int i = min; i <= max; i++)
          {
            selectList[i - min + 1] = new SelectListItem()
            {
              Text = i.ToString(),
              Value = i.ToString(),
              Selected = value.Equals(i)
            };
          }
        }
      }

      return selectList ?? new SelectListItem[0];
    }

    private static IEnumerable<SelectListItem> GetSelectListWithDefaultValue(IEnumerable<SelectListItem> selectList, object defaultValue, bool allowMultiple)
    {
      IEnumerable defaultValues;

      if (allowMultiple)
      {
        defaultValues = defaultValue as IEnumerable;
        if (defaultValues == null || defaultValues is string)
        {
          throw new InvalidOperationException(
              String.Format(
                  CultureInfo.CurrentCulture,
                  "Select expression not enumerable",
                  "expression"));
        }
      }
      else
      {
        defaultValues = new[] { defaultValue };
      }

      IEnumerable<string> values = from object value in defaultValues
                                    select Convert.ToString(value, CultureInfo.CurrentCulture);
      HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
      List<SelectListItem> newSelectList = new List<SelectListItem>();

      foreach (SelectListItem item in selectList)
      {
        //item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
        newSelectList.Add(item);
      }
      return newSelectList;
    }

    private static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, ModelMetadata metadata, string optionLabel, string name, IEnumerable<SelectListItem> selectList, bool allowMultiple, IDictionary<string, object> htmlAttributes)
    {
      string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
      if (String.IsNullOrEmpty(fullName))
      {
        throw new ArgumentException("Cannot be null or empty", "name");
      }

      bool usedViewData = false;

      // If we got a null selectList, try to use ViewData to get the list of items.
      if (selectList == null)
      {
        selectList = htmlHelper.GetSelectData(name);
        usedViewData = true;
      }

      object defaultValue = (allowMultiple) ? htmlHelper.GetModelStateValue(fullName, typeof(string[])) : htmlHelper.GetModelStateValue(fullName, typeof(string));

      // If we haven't already used ViewData to get the entire list of items then we need to
      // use the ViewData-supplied value before using the parameter-supplied value.
      if (!usedViewData && defaultValue == null && !String.IsNullOrEmpty(name))
      {
        defaultValue = htmlHelper.ViewData.Eval(name);
      }

      if (defaultValue != null)
      {
        selectList = GetSelectListWithDefaultValue(selectList, defaultValue, allowMultiple);
      }

      // Convert each ListItem to an <option> tag
      StringBuilder listItemBuilder = new StringBuilder();

      // Make optionLabel the first item that gets rendered.
      if (optionLabel != null)
      {
        listItemBuilder.AppendLine(ListItemToOption(new SelectListItem() { Text = optionLabel, Value = String.Empty, Selected = false }));
      }

      foreach (SelectListItem item in selectList)
      {
        listItemBuilder.AppendLine(ListItemToOption(item));
      }

      TagBuilder tagBuilder = new TagBuilder("select")
      {
        InnerHtml = listItemBuilder.ToString()
      };
      tagBuilder.MergeAttributes(htmlAttributes);
      tagBuilder.MergeAttribute("name", fullName.Replace('.', '_'), true /* replaceExisting */);
      tagBuilder.GenerateId(fullName);
      if (allowMultiple)
      {
        tagBuilder.MergeAttribute("multiple", "multiple");
      }

      // If there are any errors for a named field, we add the css attribute.
      ModelState modelState;
      if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
      {
        if (modelState.Errors.Count > 0)
        {
          tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
        }
      }

      tagBuilder.MergeAttributes(FormsExtension.GetNgValidationAttributes(htmlHelper, name, metadata));

      return tagBuilder.ToMvcHtmlString(TagRenderMode.Normal);
    }
  }
}