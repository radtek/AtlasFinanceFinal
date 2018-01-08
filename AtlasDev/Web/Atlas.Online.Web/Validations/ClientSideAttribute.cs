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
using System.Collections;
using System.Resources;

namespace Atlas.Online.Web.Validations
{
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class ClientSideAttribute : ValidationAttribute, IClientValidatable
  {
    private Type _providerType;

    public ClientSideValidationType ValidationType { get; set; }

    public ClientSideAttribute() : base() { }
    public ClientSideAttribute(Func<string> errorMessageAccessor) : base(errorMessageAccessor) { }
    public ClientSideAttribute(string errorMessage) : base(errorMessage) { }
    public ClientSideAttribute(Type providerType)
    {
      _providerType = providerType;
    }

    public override bool IsValid(object value)
    {
      // Validation for client side only
      return true;
    }

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      return  (_providerType != null) ? FromProvider(metadata) : FromThis(metadata);
    }

    private IEnumerable<ModelClientValidationRule> FromThis(ModelMetadata metadata)
    {
      yield return new ModelClientValidationRule()
      {
        ErrorMessage = String.Format(this.ErrorMessageString, metadata.DisplayName),
        ValidationType = this.ValidationType.ToString().ToLower()
      };
    }

    private IEnumerable<ModelClientValidationRule> FromProvider(ModelMetadata metadata)
    {
      IClientSideValidationsProvider provider = (IClientSideValidationsProvider)Activator.CreateInstance(_providerType);

      var validations = provider.GetValidations().GetEnumerator();

      while (validations.MoveNext())
      {
        var validation = (ClientSideValidation)validations.Current;

        string errorMessage = GetErrorMessage(validation);

        yield return new ModelClientValidationRule()
        {
          ErrorMessage = String.Format(errorMessage, metadata.DisplayName),
          ValidationType = validation.ValidationType.ToString().ToLower()
        };
      }

      yield break;
    }

    private string GetErrorMessage(ClientSideValidation validation)
    {
      if (validation.ResourceType != null)
      {
        return new ResourceManager(validation.ResourceType).GetString(validation.ResourceName);
      }
      else
      {
        return validation.ErrorMessage ?? this.ErrorMessageString;
      }
    }
  }
}