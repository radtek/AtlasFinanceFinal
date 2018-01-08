using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Model
{
  public class DBT_AVSCheckType : XPLiteObject
  {
    private int _avsCheckTypeId;
    [Key]
    public int AVSCheckTypeId
    {
      get
      {
        return _avsCheckTypeId;
      }
      set
      {
        SetPropertyValue("AVSCheckTypeId", ref _avsCheckTypeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Debit.AVSCheckType Type
    {
      get { return Description.FromStringToEnum<Enumerators.Debit.AVSCheckType>(); }
      set { value = Description.FromStringToEnum<Enumerators.Debit.AVSCheckType>(); }
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

    public DBT_AVSCheckType() : base() { }
    public DBT_AVSCheckType(Session session) : base(session) { }

    #endregion
  }
}