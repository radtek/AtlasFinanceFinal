using Atlas.Online.Data.Models.Definitions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Atlas.Online.Web.Validations
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class UniqueCellAttribute : ValidationAttribute, IClientValidatable
  {
    public override bool IsValid(object value)
    {
      using (var uow = XpoHelper.GetNewUnitOfWork()) 
      {
        // Normalise 
        string cell = NormaliseCell(value.ToString());
        // check if the cell number is in use
        var currentlyInUse =new XPQuery<Client>(uow).Any(
          c =>
            c.Contacts.Any(
              p =>
                p.ContactType.ContactTypeId == Convert.ToInt32(Enumerators.General.ContactType.CellNo) &&
                p.Value == cell) &&
            (WebSecurity.CurrentUserId < 0 || c.UserId == WebSecurity.CurrentUserId));
        // returns true if the cell no is not in use
        return !currentlyInUse;
      }
    }

    public static string NormaliseCell(string cell)
    {
      cell = Regex.Replace(cell, @"[^.0-9]", String.Empty);
      return Regex.Replace(cell, @"^\+?27", "0");
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