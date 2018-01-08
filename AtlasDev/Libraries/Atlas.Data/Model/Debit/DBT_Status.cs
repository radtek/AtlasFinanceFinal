using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_Status : XPLiteObject
  {
    private int _statusId;
    [Key(AutoGenerate = false)]
    public int StatusId
    {
      get
      {
        return _statusId;
      }
      set
      {
        SetPropertyValue("StatusId", ref _statusId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Debit.Status Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.Status>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.Status>();
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
    public DBT_Status() : base() { }
    public DBT_Status(Session session) : base(session) { }
  }
}