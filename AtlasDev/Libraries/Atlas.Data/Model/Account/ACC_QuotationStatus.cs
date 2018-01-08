using DevExpress.Xpo;
using Atlas.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_QuotationStatus : XPLiteObject
  {
    private long _quotationStatusId;
    [Key]
    public long QuotationStatusId
    {
      get
      {
        return _quotationStatusId;
      }
      set
      {
        SetPropertyValue("QuotationStatusId", ref _quotationStatusId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Account.QuotationStatus Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.QuotationStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.QuotationStatus>();
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

    public ACC_QuotationStatus() : base() { }
    public ACC_QuotationStatus(Session session) : base(session) { }

    #endregion
  }
}
