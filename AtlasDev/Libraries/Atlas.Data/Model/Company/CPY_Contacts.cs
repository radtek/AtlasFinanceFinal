using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class CPY_Contacts:XPLiteObject
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

    private XPDelayedProperty _Contact = new XPDelayedProperty();

    [Delayed("_Contact")]
    [Persistent("Contacts")]
    public Contact Contact
    {
      get { return (Contact)_Contact.Value; }
      set { _Contact.Value = value; }
    }

    #region Constructors

    public CPY_Contacts()
      : base()
    {
    }

    public CPY_Contacts(Session session)
      : base(session)
    {
    }

    #endregion
  }
}
