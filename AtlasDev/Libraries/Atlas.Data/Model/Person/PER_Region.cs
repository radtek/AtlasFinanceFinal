using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class PER_Region : XPLiteObject
  {
    private Int64 _OID;
    [Key(AutoGenerate = true)]
    public Int64 OID
    {
      get
      {
        return _OID;
      }
      set
      {
        SetPropertyValue("OID", ref _OID, value);
      }
    }

    private XPDelayedProperty _Region = new XPDelayedProperty();
    [Persistent("Regions")]
    [Delayed("_Region")]
    public Region Region
    {
      get
      {
        return (Region)_Region.Value;
      }
      set
      {
        _Region.Value = value;
      }
    }

    private XPDelayedProperty _Person = new XPDelayedProperty();
    [Delayed("_Person")]
    [Persistent("PersonRegions")]
    //[Association("PersonRegions")]
    public PER_Person Person
    {
      get
      {
        return (PER_Person)_Person.Value;
      }
      set
      {
        _Person.Value = value;
      }
    }

    #region Constructors

    public PER_Region()
      : base()
    {
    }

    public PER_Region(Session session)
      : base(session)
    {
    }

    #endregion
  }
}
