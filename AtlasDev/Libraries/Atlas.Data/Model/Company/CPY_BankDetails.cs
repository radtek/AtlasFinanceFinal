using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class CPY_BankDetails : XPLiteObject
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

    private XPDelayedProperty _BankDetails = new XPDelayedProperty();

    [Delayed("_BankDetails")]
    [Persistent("BankDetails")]
    public BNK_Detail BankDetail
    {
      get { return (BNK_Detail)_BankDetails.Value; }
      set { _BankDetails.Value = value; }
    }

    #region Constructors

    public CPY_BankDetails()
      : base()
    {
    }

    public CPY_BankDetails(Session session)
      : base(session)
    {
    }

    #endregion
  }
}
