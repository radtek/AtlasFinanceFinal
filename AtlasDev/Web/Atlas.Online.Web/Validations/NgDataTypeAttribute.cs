using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Online.Web.Validations
{
  public class NgDataTypeAttribute : DataTypeAttribute, IClientValidatable
  {
    public NgDataTypeAttribute(DataType dataType) : base(dataType) { }    

    public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
    {
      yield return new ModelClientValidationRule()
      {
        ErrorMessage = this.ErrorMessageString,        
        ValidationType = GetValidationType()
      };
    }

    private string GetValidationType()
    {
      switch (this.DataType)
      {
        case DataType.EmailAddress:
          return "email";
        default:
          return this.DataType.ToString().ToLower();
      }
    }
  }
}