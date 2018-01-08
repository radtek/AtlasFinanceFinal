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
  public class ExcludeEnumAttribute : ValidationAttribute
  {
    private object[] Exclude { get; set; }

    public ExcludeEnumAttribute(params object[] excludeEnums)
    {
      Exclude = excludeEnums;
    }

    public override bool IsValid(object value)
    {
      return Exclude == null || !Exclude.Any(x => x.Equals(value));
    }

  }
}