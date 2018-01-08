using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_ResultCode : XPLiteObject
  {
    private int _resultCodeId;
    [Key]
    public int ResultCodeId
    {
      get
      {
        return _resultCodeId;
      }
      set
      {
        SetPropertyValue("ResultCodeId", ref _resultCodeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Payout.ResultCode Type
    {
      get
      {
        return ResultCode.FromStringToEnum<Enumerators.Payout.ResultCode>();
      }
      set
      {
        value = ResultCode.FromStringToEnum<Enumerators.Payout.ResultCode>();
      }
    }

    private string _resultCode;
    [Persistent, Size(10)]
    public string ResultCode
    {
      get
      {
        return _resultCode;
      }
      set
      {
        SetPropertyValue("ResultCode", ref _resultCode, value);
      }
    }

    private string _description;
    [Persistent, Size(40)]
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

    #region Constructors

    public PYT_ResultCode() : base() { }
    public PYT_ResultCode(Session session) : base(session) { }

    #endregion
  }
}
