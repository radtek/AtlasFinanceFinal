using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class WFL_UserGroup : XPLiteObject
  {
    private int _userGroupId;
    [Key(AutoGenerate = true)]
    public int UserGroupId
    {
      get
      {
        return _userGroupId;
      }
      set
      {
        SetPropertyValue("UserGroupId", ref _userGroupId, value);
      }
    }

    private string _name;
    [Persistent, Size(50)]
    public string Name
    {
      get
      {
        return _name;
      }
      set
      {
        SetPropertyValue("Name", ref _name, value);
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

    [Association]
    public XPCollection<WFL_ProcessStepUserGroup> ProcessStepUserGroups { get { return GetCollection<WFL_ProcessStepUserGroup>("ProcessStepUserGroups"); } }

    #region Constructors

    public WFL_UserGroup() : base() { }
    public WFL_UserGroup(Session session) : base(session) { }

    #endregion
  }
}
