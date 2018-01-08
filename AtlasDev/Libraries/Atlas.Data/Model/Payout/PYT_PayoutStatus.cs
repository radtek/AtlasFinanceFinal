using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_PayoutStatus:XPLiteObject
  {
    private int _payoutStatusId;
    [Key]
    public int PayoutStatusId
    {
      get
      {
        return _payoutStatusId;
      }
      set
      {
        SetPropertyValue("PayoutStatusId", ref _payoutStatusId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Payout.PayoutStatus Status
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Payout.PayoutStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Payout.PayoutStatus>();
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

    public PYT_PayoutStatus() : base() { }
    public PYT_PayoutStatus(Session session) : base(session) { }

    #endregion
  }
}
