using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_BatchStatus : XPLiteObject
  {
    private int _batchStatusId;
    [Key(AutoGenerate = false)]
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
    public Enumerators.Debit.BatchStatus Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.BatchStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.BatchStatus>();
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
    public DBT_BatchStatus() : base() { }
    public DBT_BatchStatus(Session session) : base(session) { }
  }
}