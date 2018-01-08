using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class WFL_ProcessStepJobAccount : XPLiteObject
  {
    private long _processStepJobAccountId;
    [Key(AutoGenerate = true)]
    public long ProcessStepJobAccountId
    {
      get
      {
        return _processStepJobAccountId;
      }
      set
      {
        SetPropertyValue("ProcessStepJobAccountId", ref _processStepJobAccountId, value);
      }
    }

    private WFL_ProcessStepJob _processStepJob;
    [Persistent("ProcessStepJobId")]
    public WFL_ProcessStepJob ProcessStepJob
    {
      get
      {
        return _processStepJob;
      }
      set
      {
        SetPropertyValue("ProcessStepJob", ref _processStepJob, value);
      }
    }

    private ACC_Account _account;
    [Persistent("AccountId")]
    [Association]
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

    private bool _override;
    [Persistent]
    public bool Override
    {
      get
      {
        return _override;
      }
      set
      {
        SetPropertyValue("Override", ref _override, value);
      }
    }

    private PER_Person _overrideUser;
    [Persistent("OverrideUserId")]
    public PER_Person OverrideUser
    {
      get
      {
        return _overrideUser;
      }
      set
      {
        SetPropertyValue("OverrideUser", ref _overrideUser, value);
      }
    }
    
    #region Constructors

    public WFL_ProcessStepJobAccount() : base() { }
    public WFL_ProcessStepJobAccount(Session session) : base(session) { }

    #endregion
  }
}
