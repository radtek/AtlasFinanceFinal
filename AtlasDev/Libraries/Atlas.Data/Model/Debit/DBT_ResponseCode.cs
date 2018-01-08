using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_ResponseCode : XPLiteObject
  {
    private int _responseCodeId;
    [Key(AutoGenerate = true)]
    public int ResponseCodeId
    {
      get
      {
        return _responseCodeId;
      }
      set
      {
        SetPropertyValue("ResponseCodeId", ref _responseCodeId, value);
      }
    }

    private string _code;
    [Persistent, Size(20)]
    public string Code
    {
      get
      {
        return _code;
      }
      set
      {
        SetPropertyValue("Code", ref _code, value);
      }
    }

      private string _description;
    [Persistent, Size(100)]
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

    private bool? _isFailed;
    [Persistent]
    public bool? IsFailed
    {
      get
      {
        return _isFailed;
      }
      set
      {
        SetPropertyValue("IsFailed", ref _isFailed, value);
      }
    }


    public DBT_ResponseCode() : base() { }
    public DBT_ResponseCode(Session session) : base(session) { }
  }
}