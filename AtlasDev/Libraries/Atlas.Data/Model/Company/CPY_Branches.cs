using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class CPY_Branches : XPLiteObject
  {
    private Int64 _OID;
    [Key(AutoGenerate = true)]
    public Int64 OID
    {
      get { return _OID; }
      set { SetPropertyValue("OID", ref _OID, value); }
    }

    private XPDelayedProperty _Branch = new XPDelayedProperty();
    [Persistent("Branches")]
    [Delayed("_Branch")]
    public BRN_Branch Branch
    {
      get { return (BRN_Branch) _Branch.Value; }
      set { _Branch.Value = value; }
    }

    private XPDelayedProperty _Company = new XPDelayedProperty();

    [Delayed("_Company")]
    [Persistent("Companies")]
    public CPY_Company Company
    {
      get { return (CPY_Company) _Company.Value; }
      set { _Company.Value = value; }
    }

    #region Constructors

    public CPY_Branches()
      : base()
    {
    }

    public CPY_Branches(Session session)
      : base(session)
    {
    }

    #endregion
  }
}
