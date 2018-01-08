using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_DebitType : XPLiteObject
  {
    private int _debitTypeId;
    [Key(AutoGenerate = false)]
    public int DebitTypeId
    {
      get
      {
        return _debitTypeId;
      }
      set
      {
        SetPropertyValue("DebitTypeId", ref _debitTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Debit.DebitType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.DebitType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.DebitType>();
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
    public DBT_DebitType() : base() { }
    public DBT_DebitType(Session session) : base(session) { }
  }
}