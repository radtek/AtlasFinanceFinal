using Atlas.Online.Web.Common.Serializers;
using Atlas.Online.Web.Extensions;
using Atlas.Online.Web.Extensions.Ng.Lib;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Atlas.Online.Web.Extensions.Ng
{
  public static class FormsExtension
  {
    // Form
    public static MvcForm NgBeginForm(this HtmlHelper instance, string actionName = null, string controllerName = null, RouteValueDictionary routeValues = null, FormMethod method = FormMethod.Post, object htmlAttributes = null, string prefixNgModel = null, object ngInit = null)
    {
      if (!(htmlAttributes is IDictionary<string, object>))
      {
        htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
      }

      if (prefixNgModel != null)
      {
        instance.ViewData.Add("_prefixNGModel", prefixNgModel);
      }

      var dictAttributes = (IDictionary<string, object>)htmlAttributes;

      if (ngInit != null)
      {
        dictAttributes.Add("ng-init", String.Format("init({0})", JsonNet.Serialize<object>(ngInit)));
      }

      return instance.BeginForm(actionName, controllerName, routeValues, method, dictAttributes);
    }

    // CheckBox
    public static MvcHtmlString NgCheckBox(this HtmlHelper htmlHelper, string name)
    {
      return NgCheckBox(htmlHelper, name, htmlAttributes: (object)null);
    }

    public static MvcHtmlString NgCheckBox(this HtmlHelper htmlHelper, string name, bool isChecked)
    {
      return NgCheckBox(htmlHelper, name, isChecked, htmlAttributes: (object)null);
    }

    public static MvcHtmlString NgCheckBox(this HtmlHelper htmlHelper, string name, bool isChecked, object htmlAttributes)
    {
      return NgCheckBox(htmlHelper, name, isChecked, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static MvcHtmlString NgCheckBox(this HtmlHelper htmlHelper, string name, object htmlAttributes)
    {
      return NgCheckBox(htmlHelper, name, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static MvcHtmlString NgCheckBox(this HtmlHelper htmlHelper, string name, IDictionary<string, object> htmlAttributes)
    {
      return NgCheckBoxHelper(htmlHelper, metadata: null, name: name, isChecked: null, htmlAttributes: htmlAttributes);
    }

    public static MvcHtmlString NgCheckBox(this HtmlHelper htmlHelper, string name, bool isChecked, IDictionary<string, object> htmlAttributes)
    {
      return NgCheckBoxHelper(htmlHelper, metadata: null, name: name, isChecked: isChecked, htmlAttributes: htmlAttributes);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression)
    {
      return NgCheckBoxFor(htmlHelper, expression, htmlAttributes: null);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, object htmlAttributes)
    {
      return NgCheckBoxFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, IDictionary<string, object> htmlAttributes)
    {
      if (expression == null)
      {
        throw new ArgumentNullException("expression");
      }

      ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
      bool? isChecked = null;
      if (metadata.Model != null)
      {
        bool modelChecked;
        if (Boolean.TryParse(metadata.Model.ToString(), out modelChecked))
        {
          isChecked = modelChecked;
        }
      }

      return NgCheckBoxHelper(htmlHelper, metadata, ExpressionHelper.GetExpressionText(expression), isChecked, htmlAttributes);
    }

    private static MvcHtmlString NgCheckBoxHelper(HtmlHelper htmlHelper, ModelMetadata metadata, string name, bool? isChecked, IDictionary<string, object> htmlAttributes)
    {
      RouteValueDictionary attributes = ToRouteValueDictionary(htmlAttributes);

      bool explicitValue = isChecked.HasValue;
      if (explicitValue)
      {
        attributes.Remove("checked"); // Explicit value must override dictionary
      }

      return NgInputHelper(
        htmlHelper,
        InputType.CheckBox,
        metadata,
        name,
        value: "true",
        useViewData: !explicitValue,
        isChecked: isChecked ?? false,
        setId: true,
        isExplicitValue: false,
        format: null,
        htmlAttributes: attributes
      );
    }

    // Hidden

    public static MvcHtmlString NgHidden(this HtmlHelper htmlHelper, string name)
    {
      return NgHidden(htmlHelper, name, value: null, htmlAttributes: null);
    }

    public static MvcHtmlString NgHidden(this HtmlHelper htmlHelper, string name, object value)
    {
      return NgHidden(htmlHelper, name, value, htmlAttributes: null);
    }

    public static MvcHtmlString NgHidden(this HtmlHelper htmlHelper, string name, object value, object htmlAttributes)
    {
      return NgHidden(htmlHelper, name, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static MvcHtmlString NgHidden(this HtmlHelper htmlHelper, string name, object value, IDictionary<string, object> htmlAttributes)
    {
      return NgHiddenHelper(htmlHelper,
                          metadata: null,
                          value: value,
                          useViewData: value == null,
                          expression: name,
                          htmlAttributes: htmlAttributes);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgHiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
    {
      return NgHiddenFor(htmlHelper, expression, (IDictionary<string, object>)null);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgHiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
    {
      return NgHiddenFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgHiddenFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
    {
      ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
      return NgHiddenHelper(htmlHelper,
                          metadata,
                          metadata.Model,
                          false,
                          ExpressionHelper.GetExpressionText(expression),
                          htmlAttributes);
    }

    private static MvcHtmlString NgHiddenHelper(HtmlHelper htmlHelper, ModelMetadata metadata, object value, bool useViewData, string expression, IDictionary<string, object> htmlAttributes)
    {
      Binary binaryValue = value as Binary;
      if (binaryValue != null)
      {
        value = binaryValue.ToArray();
      }

      byte[] byteArrayValue = value as byte[];
      if (byteArrayValue != null)
      {
        value = Convert.ToBase64String(byteArrayValue);
      }

      return NgInputHelper(htmlHelper,
                         InputType.Hidden,
                         metadata,
                         expression,
                         value,
                         useViewData,
                         isChecked: false,
                         setId: true,
                         isExplicitValue: true,
                         format: null,
                         htmlAttributes: htmlAttributes);
    }

    // Password

    public static MvcHtmlString NgPassword(this HtmlHelper htmlHelper, string name)
    {
      return NgPassword(htmlHelper, name, value: null);
    }

    public static MvcHtmlString NgPassword(this HtmlHelper htmlHelper, string name, object value)
    {
      return NgPassword(htmlHelper, name, value, htmlAttributes: null);
    }

    public static MvcHtmlString NgPassword(this HtmlHelper htmlHelper, string name, object value, object htmlAttributes)
    {
      return NgPassword(htmlHelper, name, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static MvcHtmlString NgPassword(this HtmlHelper htmlHelper, string name, object value, IDictionary<string, object> htmlAttributes)
    {
      return NgPasswordHelper(htmlHelper, metadata: null, name: name, value: value, htmlAttributes: htmlAttributes);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgPasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
    {
      return NgPasswordFor(htmlHelper, expression, htmlAttributes: null);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgPasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
    {
      return NgPasswordFor(htmlHelper, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Users cannot use anonymous methods with the LambdaExpression type")]
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgPasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
    {
      if (expression == null)
      {
        throw new ArgumentNullException("expression");
      }

      return NgPasswordHelper(htmlHelper,
                            ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData),
                            ExpressionHelper.GetExpressionText(expression),
                            value: null,
                            htmlAttributes: htmlAttributes);
    }

    private static MvcHtmlString NgPasswordHelper(HtmlHelper htmlHelper, ModelMetadata metadata, string name, object value, IDictionary<string, object> htmlAttributes)
    {
      return NgInputHelper(htmlHelper,
                         InputType.Password,
                         metadata,
                         name,
                         value,
                         useViewData: false,
                         isChecked: false,
                         setId: true,
                         isExplicitValue: true,
                         format: null,
                         htmlAttributes: htmlAttributes);
    }

    // RadioButton

    public static MvcHtmlString NgRadioButton(this HtmlHelper htmlHelper, string name, object value)
    {
      return NgRadioButton(htmlHelper, name, value, htmlAttributes: (object)null);
    }

    public static MvcHtmlString NgRadioButton(this HtmlHelper htmlHelper, string name, object value, object htmlAttributes)
    {
      return NgRadioButton(htmlHelper, name, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static MvcHtmlString NgRadioButton(this HtmlHelper htmlHelper, string name, object value, IDictionary<string, object> htmlAttributes)
    {
      // Determine whether or not to render the checked attribute based on the contents of ViewData.
      string valueString = Convert.ToString(value, CultureInfo.CurrentCulture);
      bool isChecked = (!String.IsNullOrEmpty(name)) && (String.Equals(htmlHelper.EvalString(name), valueString, StringComparison.OrdinalIgnoreCase));
      // checked attributes is implicit, so we need to ensure that the dictionary takes precedence.
      RouteValueDictionary attributes = ToRouteValueDictionary(htmlAttributes);
      if (attributes.ContainsKey("checked"))
      {
        return NgInputHelper(htmlHelper,
                           InputType.Radio,
                           metadata: null,
                           name: name,
                           value: value,
                           useViewData: false,
                           isChecked: false,
                           setId: true,
                           isExplicitValue: true,
                           format: null,
                           htmlAttributes: attributes);
      }

      return NgRadioButton(htmlHelper, name, value, isChecked, htmlAttributes);
    }

    public static MvcHtmlString NgRadioButton(this HtmlHelper htmlHelper, string name, object value, bool isChecked)
    {
      return NgRadioButton(htmlHelper, name, value, isChecked, htmlAttributes: (object)null);
    }

    public static MvcHtmlString NgRadioButton(this HtmlHelper htmlHelper, string name, object value, bool isChecked, object htmlAttributes)
    {
      return NgRadioButton(htmlHelper, name, value, isChecked, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static MvcHtmlString NgRadioButton(this HtmlHelper htmlHelper, string name, object value, bool isChecked, IDictionary<string, object> htmlAttributes)
    {
      if (value == null)
      {
        throw new ArgumentNullException("value");
      }
      // checked attribute is an explicit parameter so it takes precedence.
      RouteValueDictionary attributes = ToRouteValueDictionary(htmlAttributes);
      attributes.Remove("checked");
      return NgInputHelper(htmlHelper,
                         InputType.Radio,
                         metadata: null,
                         name: name,
                         value: value,
                         useViewData: false,
                         isChecked: isChecked,
                         setId: true,
                         isExplicitValue: true,
                         format: null,
                         htmlAttributes: attributes);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgRadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value)
    {
      return NgRadioButtonFor(htmlHelper, expression, value, htmlAttributes: null);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgRadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, object htmlAttributes)
    {
      return NgRadioButtonFor(htmlHelper, expression, value, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgRadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, IDictionary<string, object> htmlAttributes)
    {
      ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
      return NgRadioButtonHelper(htmlHelper,
                               metadata,
                               metadata.Model,
                               ExpressionHelper.GetExpressionText(expression),
                               value,
                               null /* isChecked */,
                               htmlAttributes);
    }

    private static MvcHtmlString NgRadioButtonHelper(HtmlHelper htmlHelper, ModelMetadata metadata, object model, string name, object value, bool? isChecked, IDictionary<string, object> htmlAttributes)
    {
      if (value == null)
      {
        throw new ArgumentNullException("value");
      }

      RouteValueDictionary attributes = ToRouteValueDictionary(htmlAttributes);

      bool explicitValue = isChecked.HasValue;
      if (explicitValue)
      {
        attributes.Remove("checked"); // Explicit value must override dictionary
      }
      else
      {
        string valueString = Convert.ToString(value, CultureInfo.CurrentCulture);
        isChecked = model != null &&
                    !String.IsNullOrEmpty(name) &&
                    String.Equals(model.ToString(), valueString, StringComparison.OrdinalIgnoreCase);
      }

      return NgInputHelper(htmlHelper,
                         InputType.Radio,
                         metadata,
                         name,
                         value,
                         useViewData: false,
                         isChecked: isChecked ?? false,
                         setId: true,
                         isExplicitValue: true,
                         format: null,
                         htmlAttributes: attributes);
    }

    // TextBox

    public static MvcHtmlString NgTextBox(this HtmlHelper htmlHelper, string name)
    {
      return NgTextBox(htmlHelper, name, value: null);
    }

    public static MvcHtmlString NgTextBox(this HtmlHelper htmlHelper, string name, object value)
    {
      return NgTextBox(htmlHelper, name, value, format: null);
    }

    public static MvcHtmlString NgTextBox(this HtmlHelper htmlHelper, string name, object value, string format)
    {
      return NgTextBox(htmlHelper, name, value, format, htmlAttributes: (object)null);
    }

    public static MvcHtmlString NgTextBox(this HtmlHelper htmlHelper, string name, object value, object htmlAttributes)
    {
      return NgTextBox(htmlHelper, name, value, format: null, htmlAttributes: htmlAttributes);
    }

    public static MvcHtmlString NgTextBox(this HtmlHelper htmlHelper, string name, object value, string format, object htmlAttributes)
    {
      return NgTextBox(htmlHelper, name, value, format, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    public static MvcHtmlString NgTextBox(this HtmlHelper htmlHelper, string name, object value, IDictionary<string, object> htmlAttributes)
    {
      return NgTextBox(htmlHelper, name, value, format: null, htmlAttributes: htmlAttributes);
    }

    public static MvcHtmlString NgTextBox(this HtmlHelper htmlHelper, string name, object value, string format, IDictionary<string, object> htmlAttributes)
    {
      return NgInputHelper(htmlHelper,
                         InputType.Text,
                         metadata: null,
                         name: name,
                         value: value,
                         useViewData: (value == null),
                         isChecked: false,
                         setId: true,
                         isExplicitValue: true,
                         format: format,
                         htmlAttributes: htmlAttributes);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
    {
      return htmlHelper.NgTextBoxFor(expression, format: null);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format)
    {
      return htmlHelper.NgTextBoxFor(expression, format, (IDictionary<string, object>)null);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
    {
      return htmlHelper.NgTextBoxFor(expression, format: null, htmlAttributes: htmlAttributes);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format, object htmlAttributes)
    {
      return htmlHelper.NgTextBoxFor(expression, format: format, htmlAttributes: HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IDictionary<string, object> htmlAttributes)
    {
      return htmlHelper.NgTextBoxFor(expression, format: null, htmlAttributes: htmlAttributes);
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is an appropriate nesting of generic types")]
    public static MvcHtmlString NgTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string format, IDictionary<string, object> htmlAttributes)
    {
      ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
      return NgTextBoxHelper(htmlHelper,
                           metadata,
                           metadata.Model,
                           ExpressionHelper.GetExpressionText(expression),
                           format,
                           htmlAttributes);
    }

    private static MvcHtmlString NgTextBoxHelper(this HtmlHelper htmlHelper, ModelMetadata metadata, object model, string expression, string format, IDictionary<string, object> htmlAttributes)
    {
      return NgInputHelper(htmlHelper,
                         InputType.Text,
                         metadata,
                         expression,
                         model,
                         useViewData: false,
                         isChecked: false,
                         setId: true,
                         isExplicitValue: true,
                         format: format,
                         htmlAttributes: htmlAttributes);
    }

    // Helper methods

    internal static MvcHtmlString NgInputHelper(HtmlHelper instance, InputType inputType, ModelMetadata metadata, string name, object value, bool useViewData, bool isChecked, bool setId, bool isExplicitValue, string format, IDictionary<string, object> htmlAttributes)
    {
      string fullName = instance.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
      if (String.IsNullOrEmpty(fullName))
      {
        throw new ArgumentException("Argument cannot be null or empty.", "name");
      }

      if (htmlAttributes == null)
      {
        htmlAttributes = new Dictionary<string, object>();
      }

      // Add ngmodel if we are rendering to a model field 
      if (!htmlAttributes.ContainsKey("ng-model"))
      {
        htmlAttributes["ng-model"] = ApplyNgObjectPrefix(instance, String.IsNullOrEmpty(name) ? metadata.PropertyName : name);
      }

      TagBuilder tagBuilder = new TagBuilder("input");
      tagBuilder.MergeAttributes(htmlAttributes);
      tagBuilder.MergeAttribute("type", HtmlHelper.GetInputTypeString(inputType));
      tagBuilder.MergeAttribute("name", fullName.Replace('.', '_'), true);

      string valueParameter = instance.FormatValue(value, format);
      bool usedModelState = false;

      switch (inputType)
      {
        case InputType.CheckBox:
          bool? modelStateWasChecked = instance.GetModelStateValue(fullName, typeof(bool)) as bool?;
          if (modelStateWasChecked.HasValue)
          {
            isChecked = modelStateWasChecked.Value;
            usedModelState = true;
          }
          goto case InputType.Radio;
        case InputType.Radio:
          if (!usedModelState)
          {
            string modelStateValue = instance.GetModelStateValue(fullName, typeof(string)) as string;
            if (modelStateValue != null)
            {
              isChecked = String.Equals(modelStateValue, valueParameter, StringComparison.Ordinal);
              usedModelState = true;
            }
          }
          if (!usedModelState && useViewData)
          {
            isChecked = instance.EvalBoolean(fullName);
          }
          if (isChecked)
          {
            tagBuilder.MergeAttribute("checked", "checked");
          }
          tagBuilder.MergeAttribute("value", valueParameter, isExplicitValue);
          break;
        case InputType.Password:
          if (value != null)
          {
            tagBuilder.MergeAttribute("value", valueParameter, isExplicitValue);
          }
          break;
        default:
          string attemptedValue = (string)instance.GetModelStateValue(fullName, typeof(string));
          tagBuilder.MergeAttribute("value", attemptedValue ?? ((useViewData) ? instance.EvalString(fullName, format) : valueParameter), isExplicitValue);
          break;
      }

      if (setId)
      {
        tagBuilder.GenerateId(fullName);
      }

      // If there are any errors for a named field, we add the css attribute.
      ModelState modelState;
      if (instance.ViewData.ModelState.TryGetValue(fullName, out modelState))
      {
        if (modelState.Errors.Count > 0)
        {
          tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
        }
      }

      var validationAttrs = GetNgValidationAttributes(instance, name, metadata);
      tagBuilder.MergeAttributes(validationAttrs, true);        

      if (inputType == InputType.CheckBox)
      {
        // Render an additional <input type="hidden".../> for checkboxes. This
        // addresses scenarios where unchecked checkboxes are not sent in the request.
        // Sending a hidden input makes it possible to know that the checkbox was present
        // on the page when the request was submitted.
        StringBuilder inputItemBuilder = new StringBuilder();        

        TagBuilder hiddenInput = new TagBuilder("input");
        hiddenInput.MergeAttribute("type", HtmlHelper.GetInputTypeString(InputType.Hidden));
        hiddenInput.MergeAttribute("name", fullName);
        hiddenInput.MergeAttribute("value", "false");
        inputItemBuilder.Append(hiddenInput.ToString(TagRenderMode.SelfClosing));
        inputItemBuilder.Append(tagBuilder.ToString(TagRenderMode.SelfClosing));

        return MvcHtmlString.Create(inputItemBuilder.ToString());
      }

      return tagBuilder.ToMvcHtmlString(TagRenderMode.SelfClosing);
    }

    public static string ApplyNgObjectPrefix(HtmlHelper instance, string model)
    {
      if (!instance.ViewData.ContainsKey("_prefixNgModel"))
      {
        return model;
      }

      return String.Join(".", instance.ViewData["_prefixNgModel"], model);
    }

    //#region TwoTextBoxes
    ///// <summary>
    ///// Extension method for HtmlHelper. Creates the html required for the angular two textbox directive.
    ///// </summary>
    ///// <typeparam name="TModel"></typeparam>
    ///// <param name="instance"></param>
    ///// <param name="expression"></param>
    ///// <param name="htmlAttributes"></param>
    ///// <returns></returns>
    //public static MvcHtmlString NgTwoTextBoxesFor<TModel>(this HtmlHelper<TModel> instance, Expression<Func<TModel, string>> expression, object htmlAttributes = null)
    //{
    //  return NgTwoTextBoxesFor(instance, expression, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
    //}

    //public static MvcHtmlString NgTwoTextBoxesFor<TModel>(this HtmlHelper<TModel> instance, Expression<Func<TModel, string>> expression, IDictionary<string, object> htmlAttributes = null)
    //{
    //  ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, instance.ViewData);
    //  string fullName = instance.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(metadata.PropertyName);
    //  string attemptedValue = (string)instance.GetModelStateValue(fullName, typeof(string));

    //  return TwoTextboxesHelper(instance, metadata, metadata.Model, ExpressionHelper.GetExpressionText(expression), htmlAttributes);
    //}

    ////public static MvcHtmlString TwoTextBoxes(string model = null, IDictionary<string, object> htmlAttributes = null)
    ////{
    ////  return TwoTextboxesHelper(null, model, null, null, null, htmlAttributes);
    ////}

    //private static MvcHtmlString TwoTextboxesHelper(this HtmlHelper instance, ModelMetadata metadata, object model, string expression, IDictionary<string, object> htmlAttributes)
    //{
    //  string fullName = instance.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(metadata.PropertyName);
    //  TagBuilder div = new TagBuilder("div");

    //  div.MergeAttribute("atl-two-textboxes", null);

    //  if (instance != null && !String.IsNullOrEmpty(expression) && !htmlAttributes.ContainsKey("ng-model"))
    //  {
    //    div.MergeAttribute("ng-model", ApplyNgObjectPrefix(instance, String.IsNullOrEmpty(expression) ? metadata.PropertyName : expression));
    //  }

    //  if (!htmlAttributes.ContainsKey("name"))
    //  {
    //    div.MergeAttribute("name", expression.Replace('.', '_'));
    //  }

    //  string attemptedValue = (string)instance.GetModelStateValue(fullName, typeof(string));
    //  div.MergeAttribute("value", attemptedValue ?? instance.EvalString(fullName, null));

    //  var validationAttrs = GetNgValidationAttributes(instance, metadata.PropertyName, metadata);
    //  div.MergeAttributes(validationAttrs, true);        

    //  div.MergeAttributes(htmlAttributes);      

    //  return div.ToMvcHtmlString();
    //}
    //#endregion

    internal static IDictionary<string, object> GetNgValidationAttributes(HtmlHelper htmlHelper, string name, ModelMetadata metadata)
    {
      Dictionary<string, object> results = new Dictionary<string, object>();

      FormContext formContext = htmlHelper.ViewContext.FormContext;
      if (formContext == null)
      {
        return results;
      }

      string fullName = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
      if (formContext.RenderedField(fullName))
      {
        return results;
      }

      formContext.RenderedField(fullName, true);

      IEnumerable<ModelClientValidationRule> clientRules = ModelValidatorProviders.Providers.GetValidators(
        metadata ?? ModelMetadata.FromStringExpression(name, htmlHelper.ViewData), htmlHelper.ViewContext
      ).SelectMany(v => v.GetClientValidationRules());

      NgValidationAttributesGenerator.GetValidationAttributes(clientRules, results);

      return results;
    }

    private static RouteValueDictionary ToRouteValueDictionary(IDictionary<string, object> dictionary)
    {
      return dictionary == null ? new RouteValueDictionary() : new RouteValueDictionary(dictionary);
    }
  }
}