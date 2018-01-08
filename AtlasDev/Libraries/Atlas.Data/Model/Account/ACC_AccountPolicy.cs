using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class ACC_AccountPolicy : XPCustomObject
  {
    [Key(AutoGenerate = true)]
    public Int64 AccountPolicyId { get;set;}

    [Persistent("AccountId")]
    [Association]
    public ACC_Account Account {get;set;}

    [Persistent("PolicyId")]
    public ACC_Policy Policy { get; set; }

    public DateTime CreateDate { get; set; }

    #region Constructors

    public ACC_AccountPolicy() : base() { }
    public ACC_AccountPolicy(Session session) : base(session) { }

    #endregion
  }
}
