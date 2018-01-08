using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_ControlValidation : XPLiteObject
  {
    private int _controlValidationId;
    [Key(AutoGenerate = true)]
    public int ControlValidationId
    {
      get
      {
        return _controlValidationId;
      }
      set
      {
        SetPropertyValue("ControlValidationId", ref _controlValidationId, value);
      }
    }

    private DBT_Control _control;
    [Persistent("ControlId")]
    public DBT_Control Control
    {
      get
      {
        return _control;
      }
      set
      {
        SetPropertyValue("Control", ref _control, value);
      }
    }

    private DBT_Validation _validation;
    [Persistent("ValidationId")]
    public DBT_Validation Validation
    {
      get
      {
        return _validation;
      }
      set
      {
        SetPropertyValue("Validation", ref _validation, value);
      }
    }


    public DBT_ControlValidation() : base() { }
    public DBT_ControlValidation(Session session) : base(session) { }
  }
}