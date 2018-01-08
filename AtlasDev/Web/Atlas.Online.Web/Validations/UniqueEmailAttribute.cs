using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Models;
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
  public class UniqueEmailAttribute : ValidationAttribute, IClientValidatable
  {
    public override bool IsValid(object value)
    {
      string str = value.ToString();
      using (UsersContext db = new UsersContext())
      {
        return !db.UserProfiles.Any(u => u.Email.Equals(str, StringComparison.OrdinalIgnoreCase) && u.UserId != WebSecurity.CurrentUserId);
      }
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      yield return new ModelClientValidationRule()
      {
        ErrorMessage = String.Format(this.ErrorMessageString, metadata.DisplayName),
        ValidationType = "unique"
      };
    }
  }
}