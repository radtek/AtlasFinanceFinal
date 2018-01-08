using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class WFL_UserGroupLink : XPLiteObject
  {
    private int _userGroupLinkId;
    [Key(AutoGenerate = true)]
    public int UserGroupLinkId
    {
      get
      {
        return _userGroupLinkId;
      }
      set
      {
        SetPropertyValue("UserGroupLinkId", ref _userGroupLinkId, value);
      }
    }

    private WFL_UserGroup _userGroup;
    [Persistent("UserGroupId")]
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

    private PER_Person _user;
    [Persistent("UserId")]
    [Association]
    public PER_Person User
    {
      get
      {
        return _user;
      }
      set
      {
        SetPropertyValue("User", ref _user, value);
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

    public WFL_UserGroupLink() : base() { }
    public WFL_UserGroupLink(Session session) : base(session) { }

    #endregion
  }
}
