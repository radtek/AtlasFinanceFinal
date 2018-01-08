using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Online.Web.Extensions.Ng.Lib
{
  public static class NgValidationAttributesGenerator
  {
    public static void GetValidationAttributes(IEnumerable<ModelClientValidationRule> clientRules, IDictionary<string, object> results)
    {
      if (clientRules == null)
      {
        throw new ArgumentNullException("clientRules");
      }

      if (results == null)
      {
        throw new ArgumentNullException("results");
      }

      foreach (ModelClientValidationRule rule in clientRules)
      {
        switch (rule.ValidationType)
        {
          case "required":
            results.Add("required", null);
            break;
          case "length":
            if (rule.ValidationParameters.ContainsKey("min")) 
            {
              results.Add("ng-minlength", rule.ValidationParameters["min"]);
            }

            if (rule.ValidationParameters.ContainsKey("max"))
            {
              results.Add("ng-maxlength", rule.ValidationParameters["max"]);

              if (rule.ValidationParameters.ContainsKey("limit") && rule.ValidationParameters["limit"] as bool? == true)
              {
                results.Add("f8-limit-length", rule.ValidationParameters["max"]);
              }
            }            
            break;
          case "range": 
            if (rule.ValidationParameters.ContainsKey("min")) 
            {
              results.Add("min", rule.ValidationParameters["min"]);
            }

            if (rule.ValidationParameters.ContainsKey("max"))
            {
              results.Add("max", rule.ValidationParameters["max"]);
            }
            break;   
          case "equalto":
            var other = rule.ValidationParameters["other"] as String;
            if (!String.IsNullOrEmpty(other) && other.Length > 2) {              
              results.Add("f8-compare", other.Substring(2));
            }
            break;
          case "regex":
            results.Add("ng-pattern", "/" + rule.ValidationParameters["pattern"] + "/");
            break;
          case "email":
            results.Add("type", "email");
            break;
          case "currency":
            var symbol = rule.ValidationParameters["symbol"];
            var decimals = rule.ValidationParameters["decimals"];
            results.Add("f8-currency-field", String.Format("{{ symbol:'{0}', decimals: {1} }}", symbol, decimals));
            break;
          default:
            results.Add("data-val-" + rule.ValidationType, String.Empty);
            break;
        }

        ValidateNgValidationRule(rule);
      }

    }

    private static void ValidateNgValidationRule(ModelClientValidationRule rule)
    {
      if (String.IsNullOrWhiteSpace(rule.ValidationType))
      {
        throw new InvalidOperationException(
            String.Format(
                CultureInfo.CurrentCulture,
                "{0} cannot be empty.",
                rule.GetType().FullName));
      }

      if (rule.ValidationType.Any(c => !Char.IsLower(c)))
      {
        throw new InvalidOperationException(
            String.Format(CultureInfo.CurrentCulture, 
                          "{0} must be legal for {1}.",
                          rule.ValidationType,
                          rule.GetType().FullName));
      }

      foreach (var key in rule.ValidationParameters.Keys)
      {
        if (String.IsNullOrWhiteSpace(key))
        {
          throw new InvalidOperationException(
              String.Format(
                  CultureInfo.CurrentCulture,
                  "{0} cannot be empty.",
                  rule.GetType().FullName));
        }

        if (!Char.IsLower(key.First()) || key.Any(c => !Char.IsLower(c) && !Char.IsDigit(c)))
        {
          throw new InvalidOperationException(
              String.Format(
                  CultureInfo.CurrentCulture,
                  "{1} must be legal for {0}",
                  key,
                  rule.GetType().FullName));
        }
      }
    }
  }
}