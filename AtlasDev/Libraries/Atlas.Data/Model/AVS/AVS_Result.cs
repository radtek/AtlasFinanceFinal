using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class AVS_Result : XPLiteObject
  {
    private Int32 _resultId;
    [Key]
    public Int32 ResultId
    {
      get
      {
        return _resultId;
      }
      set
      {
        SetPropertyValue("ResultId", ref _resultId, value);
      }
    }

    [NonPersistent]
    public Enumerators.AVS.Result Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.AVS.Result>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.AVS.Result>();
      }
    }

    private string _description;
    [Persistent, Size(20)]
    public string Description 
    {
      get
      {
        return _description;
      }
      set
      {
        SetPropertyValue("Description", ref _description, value);
      }
    }

    private bool _isSuccess;
    [Persistent]
    public bool IsSuccess
    {
      get
      {
        return _isSuccess;
      }
      set
      {
        SetPropertyValue("IsSuccess", ref _isSuccess, value);
      }
    }

    private bool _hasWarnings;
    [Persistent]
    public bool HasWarnings
    {
      get
      {
        return _hasWarnings;
      }
      set
      {
        SetPropertyValue("HasWarnings", ref _hasWarnings, value);
      }
    }

    #region Constructors

    public AVS_Result() : base() { }
    public AVS_Result(Session session) : base(session) { }

    #endregion
  }
}
