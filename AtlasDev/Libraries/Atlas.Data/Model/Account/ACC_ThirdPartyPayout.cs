using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class ACC_ThirdPartyPayout:XPLiteObject
  {
    private Int64 _thirdPartyPayoutId;
    [Key(AutoGenerate = true)]
    public Int64 ThirdPartyPayoutId
    {
      get
      {
        return _thirdPartyPayoutId;
      }
      set
      {
        SetPropertyValue("ThirdPartyPayoutId", ref _thirdPartyPayoutId, value);
      }
    }

    private ACC_Account _account;
    [Persistent("AccountId")]
    public ACC_Account Account
    {
      get
      {
        return _account;
      }
      set
      {
        SetPropertyValue("Account", ref _account, value);
      }
    }

    private CPY_Company _company;
    [Persistent("CompanyId")]
    public CPY_Company Company
    {
      get
      {
        return _company;
      }
      set
      {
        SetPropertyValue("Company", ref _company, value);
      }
    }

    private BNK_Detail _bankDetail;
    [Persistent("BankDetailId")]
    public BNK_Detail BankDetail
    {
      get
      {
        return _bankDetail;
      }
      set
      {
        SetPropertyValue("BankDetail", ref _bankDetail, value);
      }
    }

    private string _referenceNo;
    [Persistent, Size(50)]
    public string ReferenceNo
    {
      get
      {
        return _referenceNo;
      }
      set
      {
        SetPropertyValue("ReferenceNo", ref _referenceNo, value);
      }
    }

    private decimal _amount;
    [Persistent]
    public decimal Amount
    {
      get
      {
        return _amount;
      }
      set
      {
        SetPropertyValue("Amount", ref _amount, value);
      }
    }

    private bool _enabled;
    [Persistent]
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

    private DateTime? _paidDate;
    [Persistent]
    public DateTime? PaidDate
    {
      get
      {
        return _paidDate;
      }
      set
      {
        SetPropertyValue("PaidDate", ref _paidDate, value);
      }
    }

    private DateTime? _updateDate;
    [Persistent]
    public DateTime? UpdateDate
    {
      get
      {
        return _updateDate;
      }
      set
      {
        SetPropertyValue("UpdateDate", ref _updateDate, value);
      }
    }

    private PER_Person _updatedBy;
    [Persistent]
    public PER_Person UpdatedBy
    {
      get
      {
        return _updatedBy;
      }
      set
      {
        SetPropertyValue("UpdatedBy", ref _updatedBy, value);
      }
    }

    private DateTime _createdDate;
    [Persistent]
    public DateTime CreatedDate
    {
      get
      {
        return _createdDate;
      }
      set
      {
        SetPropertyValue("CreatedDate", ref _createdDate, value);
      }
    }

    private PER_Person _createdBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get
      {
        return _createdBy;
      }
      set
      {
        SetPropertyValue("CreatedBy", ref _createdBy, value);
      }
    }
  }
}
