using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_PayDate : XPLiteObject
  {
    private int _payDateId;
    [Key]
    public int PayDateId
    {
      get
      {
        return _payDateId;
      }
      set
      {
        SetPropertyValue("PayDateId", ref _payDateId, value);
      }
    }

    private ACC_PayDateType _payDateType;
    [Persistent]
    public ACC_PayDateType PayDateType
    {
      get
      {
        return _payDateType;
      }
      set
      {
        SetPropertyValue("PayDateType", ref _payDateType, value);
      }
    }


    private int _dayNo;
    [Persistent]
    public int DayNo
    {
      get
      {
        return _dayNo;
      }
      set
      {
        SetPropertyValue("DayNo", ref _dayNo, value);
      }
    }

    #region Constructors

    public ACC_PayDate() : base() { }
    public ACC_PayDate(Session session) : base(session) { }

    #endregion
  }
}