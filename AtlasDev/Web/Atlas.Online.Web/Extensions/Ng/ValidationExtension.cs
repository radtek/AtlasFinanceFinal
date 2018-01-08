using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Online.Web.Extensions.Ng
{
  public static class ValidationExtension
  {
    public static MvcHtmlString NgValidationMessagesFor<TModel, TProperty>(this HtmlHelper<TModel> instance, 
      Expression<Func<TModel, TProperty>> expression, 
      string ngShow = null,
      string formName = "form",
      IDictionary<string, string> errorMessages = null)
    {
      string name = ExpressionHelper.GetExpressionText(expression);
      
      ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, instance.ViewData);
      
      IEnumerable<ModelClientValidationRule> clientRules = ModelValidatorProviders.Providers.GetValidators(
        metadata ?? ModelMetadata.FromStringExpression(name, instance.ViewData), instance.ViewContext
      ).SelectMany(v => v.GetClientValidationRules());

      if (errorMessages == null)
      {
        errorMessages = new Dictionary<string, string>();
      }

      var kvPairs = clientRules.Select(rule =>
      {
        return new KeyValuePair<string, string>(MapValidationTypeToNg(rule.ValidationType), rule.ErrorMessage);
      });

      foreach (var pair in kvPairs)
      {
        errorMessages.Add(pair);
      }

      name = name.Replace('.', '_');

      return NgValidationMessageHelper(instance, name, ngShow, formName, errorMessages);
    }    
    
    public static MvcHtmlString NgValidationMessageHelper(HtmlHelper instance, string fieldName, string ngShow, string formName, IDictionary<string, string> errorMessages)
    {
      if (errorMessages.Count() == 0)
      {
        return new MvcHtmlString(String.Empty);
      }

      if (string.IsNullOrEmpty(ngShow))
      {
        ngShow = "{1}.{0}.$invalid && (form.$submitted || form.{0}.$dirty && form.{0}.$blur)";
      }

      TagBuilder root = new TagBuilder("div");
      root.AddCssClass("col sm-2-3 xsm-no-push col-push-pull push-4 mb-alpha ng-cloak");
      root.MergeAttribute("ng-show", String.Format(ngShow, fieldName, formName));

      foreach (var error in errorMessages)
      {
        TagBuilder div = new TagBuilder("div");
        div.AddCssClass("error");
        div.MergeAttribute("ng-show", String.Format("{0}.{1}.$error.{2}", formName, fieldName, error.Key));
        div.InnerHtml = error.Value;

        root.InnerHtml += div.ToString(TagRenderMode.Normal);
      }

      return root.ToMvcHtmlString();
    }

    /// <summary>
    /// Map MVC4's validation rule to Atlas' Angular rule names (in directives).
    /// Quite unsightly, but it will work.
    /// </summary>
    /// <param name="validationType"></param>
    /// <returns></returns>
    private static string MapValidationTypeToNg(string validationType)
    {
      var map = new Dictionary<string, string>()
      {
        {"regex", "pattern"},
        {"unique", "remotevalidity"},
        {"cdvvalidate", "removevalidity"},
        {"currency", "ng-currency-field"},
        {"length", "maxlength" }
      };

      if (map.ContainsKey(validationType.ToLower()))
      {
        return map[validationType.ToLower()];
      }

      return validationType;
    }

  }
}