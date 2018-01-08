using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class PER_Branch : XPLiteObject
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

    private XPDelayedProperty _Branch = new XPDelayedProperty();
    [Persistent("Branches")]
    [Delayed("_Branch")]
    public BRN_Branch Branch
    {
      get
      {
        return (BRN_Branch)_Branch.Value;
      }
      set
      {
        _Branch.Value = value;
      }
    }

    private XPDelayedProperty _Person = new XPDelayedProperty();
    [Delayed("_Person")]
    [Persistent("PersonBranches")]
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

    public PER_Branch()
      : base()
    {
    }

    public PER_Branch(Session session)
      : base(session)
    {
    }

    #endregion
  }
}
