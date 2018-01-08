using System;

using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class NAEDOLogin : XPCustomObject
  {
    public NAEDOLogin() : base() { }
    public NAEDOLogin(Session session) : base(session) { }

    private Int64 _naedoLoginId;
    [Key(AutoGenerate = true)]
    public Int64 NAEDOLoginId
    {
      get
      {
        return _naedoLoginId;
      }
      set
      {
        SetPropertyValue("NAEDOLoginId", ref _naedoLoginId, value);
      }
    }

    private int _merchantId;
    [Persistent]
    public int MerchantId
    {
      get
      {
        return _merchantId;
      }
      set
      {
        SetPropertyValue("MerchantId", ref _merchantId, value);
      }
    }

    private string _username;
    [Persistent, Size(20)]
    public string Username
    {
      get
      {
        return _username;
      }
      set
      {
        SetPropertyValue("Username", ref _username, value);
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


    /// <summary>
    /// I don't want to make this a whole complicated enum with associated lookup tables- 
    /// I'm keeping this simple, as it is not needed in queries- see:
    /// Atlas.Enumerators.Credentials.CredentialPurpose    
    /// - Keith
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