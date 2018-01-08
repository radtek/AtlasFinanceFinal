using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_ReplyCode:XPLiteObject
  {
    private int _replyCodeId;
    [Key]
    public int ReplyCodeId
    {
      get
      {
        return _replyCodeId;
      }
      set
      {
        SetPropertyValue("ReplyCodeId", ref _replyCodeId, value);
      }
    }

    private PYT_ServiceType _serviceType;
    [Persistent("ServiceTypeId")]
    public PYT_ServiceType ServiceType
    {
      get
      {
        return _serviceType;
      }
      set
      {
        SetPropertyValue("ServiceType", ref _serviceType, value);
      }
    }

    [NonPersistent]
    public Enumerators.Payout.ReplyCode Type
    {
      get
      {
        return Code.FromStringToEnum<Enumerators.Payout.ReplyCode>();
      }
      set
      {
        value = Code.FromStringToEnum<Enumerators.Payout.ReplyCode>();
      }
    }

    private string _code;
    [Persistent, Size(10)]
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

    private PYT_ReplyCodeType _resultCodeType;
    [Persistent("ReplyCodeTypeId")]
    public PYT_ReplyCodeType ReplyCodeType
    {
      get
      {
        return _resultCodeType;
      }
      set
      {
        SetPropertyValue("ReplyCodeType", ref _resultCodeType, value);
      }
    }

    #region Constructors

    public PYT_ReplyCode() : base() { }
    public PYT_ReplyCode(Session session) : base(session) { }

    #endregion
  }
}
