using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class WFL_ProcessStepUserGroup : XPLiteObject
  {
    private int _processStepUserGroupId;
    [Key(AutoGenerate = true)]
    public int ProcessStepUserGroupId
    {
      get
      {
        return _processStepUserGroupId;
      }
      set
      {
        SetPropertyValue("ProcessStepUserGroupId", ref _processStepUserGroupId, value);
      }
    }

    private WFL_ProcessStep _processStep;
    [Persistent("ProcessStepId")]
    public WFL_ProcessStep ProcessStep
    {
      get
      {
        return _processStep;
      }
      set
      {
        SetPropertyValue("ProcessStep", ref _processStep, value);
      }
    }

    private WFL_UserGroup _userGroup;
    [Persistent("UserGroupId")]
    [Association]
    public WFL_UserGroup UserGroup
    {
      get
      {
        return _userGroup;
      }
      set
      {
        SetPropertyValue("UserGroup", ref _userGroup, value);
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

    private DateTime? _disableDate;
    [Persistent]
    public DateTime? DisableDate
    {
      get
      {
        return _disableDate;
      }
      set
      {
        SetPropertyValue("DisableDate", ref _disableDate, value);
      }
    }

    private DateTime _createDate;
    [Persistent]
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

    public WFL_ProcessStepUserGroup() : base() { }
    public WFL_ProcessStepUserGroup(Session session) : base(session) { }

    #endregion
  }
}
