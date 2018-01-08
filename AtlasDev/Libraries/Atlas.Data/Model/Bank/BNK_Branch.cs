using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class BNK_Branch : XPLiteObject
  {
    private Int64 _BranchId;
    [Key(AutoGenerate = true)]
    public Int64 BranchId
    {
      get
      {
        return _BranchId;
      }
      set
      {
        SetPropertyValue("BranchId", ref _BranchId, value);
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

    private string _branchCode;
    [Persistent,Size(20)]
    [Indexed(Name = "IDX_BranchCode")]
    public string BranchCode
    {
      get
      {
        return _branchCode;
      }
      set
      {
        SetPropertyValue("BranchCode", ref _branchCode, value);
      }
    }

    #region Constructors

    public BNK_Branch() : base() { }
    public BNK_Branch(Session session) : base(session) { }

    #endregion
  }
}