using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class PER_Contacts:XPLiteObject
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

    private XPDelayedProperty _contact = new XPDelayedProperty();
    [Delayed("_contact")]
    [Persistent("Contacts")]
    public Contact Contact
    {
      get
      {
        return (Contact)_contact.Value;
      }
      set
      {
        _contact.Value = value;
      }
    }

    #region Constructors

    public PER_Contacts()
      : base()
    {
    }

    public PER_Contacts(Session session)
      : base(session)
    {
    }

    #endregion
  }
}
