using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_ServiceType : XPLiteObject
  {
    private int _serviceTypeId;
    [Key]
    public int ServiceTypeId
    {
      get
      {
        return _serviceTypeId;
      }
      set
      {
        SetPropertyValue("ServiceTypeId", ref _serviceTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Payout.PayoutServiceType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Payout.PayoutServiceType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Payout.PayoutServiceType>();
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

    public PYT_ServiceType() : base() { }
    public PYT_ServiceType(Session session) : base(session) { }

    #endregion
  }
}