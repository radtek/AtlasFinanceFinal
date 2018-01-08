using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class PER_BankDetails:XPLiteObject
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

    private XPDelayedProperty _bankDetails = new XPDelayedProperty();
    [Delayed("_bankDetails")]
    [Persistent("BankDetails")]
    public BNK_Detail BankDetail
    {
      get
      {
        return (BNK_Detail)_bankDetails.Value;
      }
      set
      {
        _bankDetails.Value = value;
      }
    }


    #region Constructors

    public PER_BankDetails()
      : base()
    {
    }

    public PER_BankDetails(Session session)
      : base(session)
    {
    }

    #endregion
  }
}
