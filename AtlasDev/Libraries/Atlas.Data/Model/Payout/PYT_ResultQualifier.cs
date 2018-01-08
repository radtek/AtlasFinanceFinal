using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_ResultQualifier : XPLiteObject
  {
    private int _resultQualifierId;
    [Key]
    public int ResultQualifierId
    {
      get
      {
        return _resultQualifierId;
      }
      set
      {
        SetPropertyValue("ResultQualifierId", ref _resultQualifierId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Payout.ResultQualifierCode Type
    {
      get
      {
        return ResultQualifierCode.FromStringToEnum<Enumerators.Payout.ResultQualifierCode>();
      }
      set
      {
        value = ResultQualifierCode.FromStringToEnum<Enumerators.Payout.ResultQualifierCode>();
      }
    }

    private string _resultQualifierCode;
    [Persistent, Size(10)]
    public string ResultQualifierCode
    {
      get
      {
        return _resultQualifierCode;
      }
      set
      {
        SetPropertyValue("ResultQualifierCode", ref _resultQualifierCode, value);
      }
    }

    private string _description;
    [Persistent, Size(60)]
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

    public PYT_ResultQualifier() : base() { }
    public PYT_ResultQualifier(Session session) : base(session) { }

    #endregion
  }
}
