namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_SAFPS_AddressDetail : XPLiteObject
  {

    private Int64 _addressDetailId;
    [Key(AutoGenerate = true)]
    [Persistent("AddressDetailId")]
    public Int64 AddressDetailId
    {
      get
      {
        return _addressDetailId;
      }
      set
      {
        SetPropertyValue("AddressDetailId", ref _addressDetailId, value);
      }
    }

    private FPM_SAFPS_Enquiry _sAFPS;
    [Indexed]
    [Persistent("SafpsId")]
    public FPM_SAFPS_Enquiry SAFPS
    {
      get
      {
        return _sAFPS;
      }
      set
      {
        SetPropertyValue("SAFPS", ref _sAFPS, value);
      }
    }

    private string _type;
    [Indexed]
    [Persistent, Size(100)]
    public string Type
    {
      get
      {
        return _type;
      }
      set
      {
        SetPropertyValue("Type", ref _type, value);
      }
    }

    private string _street;
    [Persistent, Size(100)]
    public string Street
    {
      get
      {
        return _street;
      }
      set
      {
        SetPropertyValue("Street", ref _street, value);
      }
    }

    private string _address;
    [Persistent, Size(300)]
    public string Address
    {
      get
      {
        return _address;
      }
      set
      {
        SetPropertyValue("Address", ref _address, value);
      }
    }

    private string _city;
    [Persistent, Size(100)]
    public string City
    {
      get
      {
        return _city;
      }
      set
      {
        SetPropertyValue("City", ref _city, value);
      }
    }

    private string _postalCode;
    [Persistent, Size(100)]
    public string PostalCode
    {
      get
      {
        return _postalCode;
      }
      set
      {
        SetPropertyValue("PostalCode", ref _postalCode, value);
      }
    }

    private string _from;
    [Persistent, Size(100)]
    public string From
    {
      get
      {
        return _from;
      }
      set
      {
        SetPropertyValue("From", ref _from, value);
      }
    }

    private string _to;
    [Persistent, Size(100)]
    public string To
    {
      get
      {
        return _to;
      }
      set
      {
        SetPropertyValue("To", ref _to, value);
      }
    }  


    #region Constructors

    public FPM_SAFPS_AddressDetail() : base() { }
    public FPM_SAFPS_AddressDetail(Session session) : base(session) { }

    #endregion
  }
}
