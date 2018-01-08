using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class PER_EmploymentHistory : XPLiteObject
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

    private XPDelayedProperty _Company = new XPDelayedProperty();
    [Persistent("EmploymentHistory")]
    [Delayed("_Company")]
    public CPY_Company Company
    {
      get
      {
        return (CPY_Company)_Company.Value;
      }
      set
      {
        _Company.Value = value;
      }
    }

    private XPDelayedProperty _Person = new XPDelayedProperty();
    [Delayed("_Person")]
    [Persistent("Persons")]
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

    public PER_EmploymentHistory()
      : base()
    {
    }

    public PER_EmploymentHistory(Session session)
      : base(session)
    {
    }

    #endregion

  }
}
