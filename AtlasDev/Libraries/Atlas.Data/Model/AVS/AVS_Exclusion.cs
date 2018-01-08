using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class AVS_Exclusion : XPLiteObject
  {
    private Int32 _exclusionId;
    [Key(AutoGenerate=true)]
    public Int32 ExclusionId
    {
      get
      {
        return _exclusionId;
      }
      set
      {
        SetPropertyValue("ExclusionId", ref _exclusionId, value);
      }
    }

    private string _idNumber;
    [Persistent, Size(20)]
    [Indexed]
    public string IdNumber
    {
      get
      {
        return _idNumber;
      }
      set
      {
        SetPropertyValue("IdNumber", ref _idNumber, value);
      }
    }

    private BNK_Bank _bank;
    [Persistent("BankId")]
    [Indexed]
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

    private string _accountNo;
    [Persistent, Size(20)]
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

    private bool _enabled;
    [Persistent]
    [Indexed]
    public bool Enabled
    {
      get
      {
        return _enabled;
      }
      set
      {
        SetPropertyValue("Enabled", ref _enabled, value);
      }
    }

    private DateTime _createDate;
    [Persistent]
    [Indexed]
    public DateTime CreateDate
    {
      get
      {
        return _createDate;
      }
      set
      {
        SetPropertyValue("CreateDate", ref _createDate, value);
      }
    }

    #region Constructors

    public AVS_Exclusion() : base() { }
    public AVS_Exclusion(Session session) : base(session) { }

    #endregion
  }
}
