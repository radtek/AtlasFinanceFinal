using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class PER_AddressDetails:XPLiteObject
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

    private XPDelayedProperty _address = new XPDelayedProperty();
    [Delayed("_address")]
    [Persistent("AddressDetails")]
    public ADR_Address Address
    {
      get
      {
        return (ADR_Address)_address.Value;
      }
      set
      {
        _address.Value = value;
      }
    }


    #region Constructors

    public PER_AddressDetails()
      : base()
    {
    }

    public PER_AddressDetails(Session session)
      : base(session)
    {
    }

    #endregion
  }
}
