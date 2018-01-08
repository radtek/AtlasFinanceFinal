using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class CPY_Addresses:XPLiteObject
  {
        private Int64 _OID;
    [Key(AutoGenerate = true)]
    public Int64 OID
    {
      get { return _OID; }
      set { SetPropertyValue("OID", ref _OID, value); }
    }

    private XPDelayedProperty _Company = new XPDelayedProperty();
    [Persistent("Companies")]
    [Delayed("_Company")]
    public CPY_Company Company
    {
      get { return (CPY_Company)_Company.Value; }
      set { _Company.Value = value; }
    }

    private XPDelayedProperty _Address= new XPDelayedProperty();

    [Delayed("_Address")]
    [Persistent("Addresses")]
    public ADR_Address Address
    {
      get { return (ADR_Address)_Address.Value; }
      set { _Address.Value = value; }
    }

    #region Constructors

    public CPY_Addresses()
      : base()
    {
    }

    public CPY_Addresses(Session session)
      : base(session)
    {
    }

    #endregion
  }
}
