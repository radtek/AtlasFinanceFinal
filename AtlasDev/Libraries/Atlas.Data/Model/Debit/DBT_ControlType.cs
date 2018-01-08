using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Model
{
  public class DBT_ControlType : XPLiteObject
  {
    
    private int _controlTypeId;
    [Key]
    public int ControlTypeId
    {
      get
      {
        return _controlTypeId;
      }
      set
      {
        SetPropertyValue("ControlTypeId", ref _controlTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Debit.ControlType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Debit.ControlType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Debit.ControlType>();
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
    public DBT_ControlType() : base() { }
    public DBT_ControlType(Session session) : base(session) { }
  }
}
