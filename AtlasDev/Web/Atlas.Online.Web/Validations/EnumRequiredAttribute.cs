using Atlas.Online.Data.Models.Definitions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Atlas.Online.Web.Validations
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class EnumRequiredAttribute : ValidationAttribute, IClientValidatable
  {
    public IEnumerable<object> InvalidKeys { get; set; }

    public EnumRequiredAttribute(params object[] invalidKeys)
    {
      InvalidKeys = invalidKeys;
    }

    public override bool IsValid(object value)
    {
      return !InvalidKeys.Any(k => Convert.ToInt32(k) == Convert.ToInt32(value));
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      var rule = new ModelClientValidationRule
      {
        ErrorMessage = this.ErrorMessageString,
        ValidationType = "enumrequired"
      };

      yield return rule;
    }

    public override string FormatErrorMessage(string name)
    {
      return string.Format(CultureInfo.CurrentCulture, this.ErrorMessageString, name);
    }
  }
}