using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class DBT_Validation : XPLiteObject
  {
    private int _validationId;
    [Key(AutoGenerate = false)]
    public int ValidationId
    {
      get
      {
        return _validationId;
      }
      set
      {
        SetPropertyValue("ValidationId", ref _validationId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Debit.ValidationType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.ValidationType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.ValidationType>();
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
    public DBT_Validation() : base() { }
    public DBT_Validation(Session session) : base(session) { }
  }
}