namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  public sealed class FPM_SAFPS_BankDetail : XPLiteObject
  {
    private Int64 _bankDetailId;
    [Key(AutoGenerate = true)]
    [Persistent("BankDetailId")]
    public Int64 BankDetailId
    {
      get
      {
        return _bankDetailId;
      }
      set
      {
        SetPropertyValue("BankDetailId", ref _bankDetailId, value);
      }
    }

    private FPM_SAFPS_Enquiry _sAFPS;
    [Persistent("SafpsId")]
    [Indexed]
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

    private string _accountNo;
    [Persistent, Size(60)]
    [Indexed]
    public string AccountNo
    {
      get
      {
        return _accountNo;
      }
      set
      {
        SetPropertyValue("AccountNo", ref _accountNo, value);
      }
    }

    private string _accountType;
    [Persistent, Size(60)]
    public string AccountType
    {
      get
      {
        return _accountType;
      }
      set
      {
        SetPropertyValue("AccountType", ref _accountType, value);
      }
    }

    private string _bank;
    [Persistent, Size(60)]
    public string Bank
    {
      get
      {
        return _bank;
      }
      set
      {
        SetPropertyValue("Bank", ref _bank, value);
      }
    }

    private string _from;
    [Persistent, Size(30)]
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
    [Persistent, Size(30)]
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

    public FPM_SAFPS_BankDetail() : base() { }
    public FPM_SAFPS_BankDetail(Session session) : base(session) { }

    #endregion
  }
}
