using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Online.Web.Validations
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class CurrencyAttribute : ValidationAttribute, IClientValidatable
  {
    public string Symbol { get; set; }
    public int Decimals { get; set; }

    public CurrencyAttribute()
    {
      Symbol = "R ";
      Decimals = 0;
    }

    public override bool IsValid(object value)
    {
      float result;
      return float.TryParse(value.ToString(), out result);
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      var rule = new ModelClientValidationRule
      {
        ErrorMessage = this.ErrorMessageString,
        ValidationType = "currency"
      };

      rule.ValidationParameters.Add("symbol", this.Symbol);
      rule.ValidationParameters.Add("decimals", this.Decimals);

      yield return rule;
    }
  }
}