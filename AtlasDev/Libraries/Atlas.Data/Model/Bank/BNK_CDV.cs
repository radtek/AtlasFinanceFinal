using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class BNK_CDV : XPCustomObject
  {
    private Int64 _CDVId;
    [Key(AutoGenerate = true)]
    public Int64 CDVId
    {
      get
      {
        return _CDVId;
      }
      set
      {
        SetPropertyValue("CDVId", ref _CDVId, value);
      }
    }

    private BNK_Bank _bank;
    [Persistent("BankId")]
    public BNK_Bank Bank
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

    private BNK_AccountType _accountType;
    [Persistent("AccountTypeId")]
    public BNK_AccountType AccountType
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

    private string _weighting;
    [Persistent, Size(11)]
    public string Weighting
    {
      get
      {
        return _weighting;
      }
      set
      {
        SetPropertyValue("Weighting", ref _weighting, value);
      }
    }

    private byte _fudgeFactor;
    [Persistent]
    public byte FudgeFactor
    {
      get
      {
        return _fudgeFactor;
      }
      set
      {
        SetPropertyValue("FudgeFactor", ref _fudgeFactor, value);
      }
    }

    private byte _modulus;
    [Persistent]
    public byte Modulus
    {
      get
      {
        return _modulus;
      }
      set
      {
        SetPropertyValue("Modulus", ref _modulus, value);
      }
    }

    private string _exceptCode;
    [Persistent, Size(1)]
    public string ExceptCode
    {
      get
      {
        return _exceptCode;
      }
      set
      {
        SetPropertyValue("ExceptCode", ref _exceptCode, value);
      }
    }

    #region Constructors

    public BNK_CDV() : base() { }
    public BNK_CDV(Session session) : base(session) { }

    #endregion
  }
}