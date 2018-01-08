using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PYT_BatchStatus:XPLiteObject
  {
    private int _batchStatusId;
    [Key]
    public int BatchStatusId
    {
      get
      {
        return _batchStatusId;
      }
      set
      {
        SetPropertyValue("BatchStatusId", ref _batchStatusId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Payout.PayoutBatchStatus Status
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Payout.PayoutBatchStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Payout.PayoutBatchStatus>();
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

    public PYT_BatchStatus() : base() { }
    public PYT_BatchStatus(Session session) : base(session) { }

    #endregion
  }
}
