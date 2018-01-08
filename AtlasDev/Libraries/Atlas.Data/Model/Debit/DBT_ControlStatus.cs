using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_ControlStatus : XPLiteObject
  {
    private int _controlStatusId;
    [Key]
    public int ControlStatusId
    {
      get
      {
        return _controlStatusId;
      }
      set
      {
        SetPropertyValue("ControlStatusId", ref _controlStatusId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Debit.ControlStatus Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.ControlStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.ControlStatus>();
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
    public DBT_ControlStatus() : base() { }
    public DBT_ControlStatus(Session session) : base(session) { }
  }
}