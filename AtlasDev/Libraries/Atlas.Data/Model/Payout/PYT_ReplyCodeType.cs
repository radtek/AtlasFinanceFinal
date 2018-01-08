using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_ReplyCodeType:XPLiteObject
  {
    private int _replyCodeTypeId;
    [Key]
    public int ReplyCodeTypeId
    {
      get
      {
        return _replyCodeTypeId;
      }
      set
      {
        SetPropertyValue("ReplyCodeTypeId", ref _replyCodeTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Payout.PayoutReplyCodeType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Payout.PayoutReplyCodeType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Payout.PayoutReplyCodeType>();
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

    #region Constructors

    public PYT_ReplyCodeType() : base() { }
    public PYT_ReplyCodeType(Session session) : base(session) { }

    #endregion
  }
}
