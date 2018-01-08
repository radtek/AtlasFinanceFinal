using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Model
{
  public class DBT_FailureType : XPLiteObject
  {
    private int _failureTypeId;
    [Key]
    public int FailureTypeId
    {
      get
      {
        return _failureTypeId;
      }
      set
      {
        SetPropertyValue("FailureTypeId", ref _failureTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Debit.FailureType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.FailureType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.FailureType>();
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
    public DBT_FailureType() : base() { }
    public DBT_FailureType(Session session) : base(session) { }
  }
}
