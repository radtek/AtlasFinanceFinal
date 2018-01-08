using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;

namespace Atlas.Online.Web.Helpers
{
  public static class Validation
  {
    public static bool IsValidObject(object instance)
    {
      return Validator.TryValidateObject(instance, 
        new ValidationContext(instance, null, null), null);
    }

    public static bool TryValidateObject(object instance, out ModelStateDictionary modelState)
    {
      var validationContext = new ValidationContext(instance, null, null);
      var validationResults = new List<ValidationResult>();
      modelState = new ModelStateDictionary();

      if (Validator.TryValidateObject(instance, validationContext, validationResults))
      {
        return true;
      }

      foreach (var validationResult in validationResults)
      {
        modelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
      }

      return false;
    }
  }
}