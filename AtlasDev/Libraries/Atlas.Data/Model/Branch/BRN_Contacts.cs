using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class BRN_Contacts:XPLiteObject
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
      get { return (BRN_Branch)_Branch.Value; }
      set { _Branch.Value = value; }
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

    public BRN_Contacts()
      : base()
    {
    }

    public BRN_Contacts(Session session)
      : base(session)
    {
    }

    #endregion

  }
}
