using System;
using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class AEDOLogin : XPCustomObject
  {
    public AEDOLogin() : base() { }
    public AEDOLogin(Session session) : base(session) { }

    private Int64 _aedoLoginId;
    [Key(AutoGenerate = true)]
    public Int64 AEDOLoginId
    {
      get
      {
        return _aedoLoginId;
      }
      set
      {
        SetPropertyValue("AEDOLoginId", ref _aedoLoginId, value);
      }
    }

    private string _merchantNum;
    [Persistent, Size(20)]
    public string MerchantNum
    {
      get
      {
        return _merchantNum;
      }
      set
      {
        SetPropertyValue("MerchantNum", ref _merchantNum, value);
      }
    }

    private string _password;
    [Persistent, Size(20)]
    public string Password
    {
      get
      {
        return _password;
      }
      set
      {
        SetPropertyValue("Password", ref _password, value);
      }
    }

    private DateTime _deletedDT;
    [Persistent]
    public DateTime DeletedDT
    {
      get
      {
        return _deletedDT;
      }
      set
      {
        SetPropertyValue("DeletedDT", ref _deletedDT, value);
      }
    }

    private string _userName;
    [Persistent, Size(100)]
    public string UserName
    {
      get { return _userName; }
      set { SetPropertyValue<string>("UserName", ref _userName, value); }
    }


    private string _lendorType;
    [Persistent, Size(10)]
    public string LendorType
    {
      get { return _lendorType; }
      set { SetPropertyValue<string>("LendorType", ref _lendorType, value); }
    }


    /// <see cref="Atlas.Enumerators.Credentials.CredentialPurpose"/>
    /// <summary>
    /// I don't want to make this a whole complicated enum with associated lookup tables- 
    /// I'm keeping this simple, as it is not needed in queries- see:
    /// Atlas.Enumerators.Credentials.CredentialPurpose    (bit flags)
    ///  
    /// - Keith
    /// 
    /// </summary>
    private int _credentialPurposeFlags;
    [Persistent]
    public int CredentialPurposeFlags
    {
      get { return _credentialPurposeFlags; }
      set { SetPropertyValue("CredentialPurposeFlags", ref _credentialPurposeFlags, value); }
    }

  }
}