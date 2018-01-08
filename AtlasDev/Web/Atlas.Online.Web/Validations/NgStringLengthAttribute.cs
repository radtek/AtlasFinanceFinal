using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Online.Web.Validations
{
  public class NgStringLengthAttribute : ValidationAttribute, IClientValidatable
  {
    public int MaximumLength { get; set; }
    public int MinimumLength { get; set; }

    public bool LimitMax { get; set; }

    public NgStringLengthAttribute(int maxLength)
      : base()
    {
      MaximumLength = maxLength; LimitMax = true;
    }

    public NgStringLengthAttribute(Func<string> errorMessageAccessor) : base(errorMessageAccessor) { }
    public NgStringLengthAttribute(string errorMessage) : base(errorMessage) { }

    public override bool IsValid(object value)
    {
      if (!(value is string))
      {
        return false;
      }
      if (value == null)
      {
        return (MinimumLength == 0);
      }

      int length = value.ToString().Length;
      return (length >= MinimumLength && length <= MaximumLength);
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      var rule =new ModelClientValidationStringLengthRule(this.ErrorMessage, MinimumLength, MaximumLength);
      rule.ValidationParameters["limit"] = LimitMax;
      yield return rule;
    }
  }
}