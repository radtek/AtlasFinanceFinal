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
  public class UniqueIdNumberAttribute : ValidationAttribute, IClientValidatable
  {
    public override bool IsValid(object value)
    {
      using (var uow = XpoHelper.GetNewUnitOfWork()) 
      {
        var currentUserId = WebSecurity.CurrentUserId;
        var client = new XPQuery<Client>(uow).First(c => c.UserId == currentUserId);
        var str = value.ToString();

        return !new XPQuery<Client>(uow).Any(
          // Check by idNumber excluding the current client id
         c => c.IDNumber == str &&  (client == null || c.ClientId != client.ClientId)
       );
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