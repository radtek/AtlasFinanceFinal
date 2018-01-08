using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using Atlas.Domain.Interface;

namespace Atlas.Domain.Model
{
  public sealed class UserTracking_PinnedUsers : XPLiteObject
  {
    private Int64 _pinnedUserId;
    [Key(AutoGenerate = true)]
    public Int64 PinnedUserId
    {
      get
      {
        return _pinnedUserId;
      }
      set
      {
        SetPropertyValue("PinnedUserId", ref _pinnedUserId, value);
      }
    }


    private UserTracking_RuleSet _rule;
    [Persistent("RuleSetId")]
    public UserTracking_RuleSet Rule
    {
      get
      {
        return _rule;
      }
      set
      {
        SetPropertyValue("Rule", ref _rule, value);
      }
    }

    private PER_Person _user;
    [Persistent("PersonId")]
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

    private int _violationCount;
    public int ViolationCount
    {
      get
      {
        return _violationCount;
      }
      set
      {
        SetPropertyValue("ViolationCount", ref _violationCount, value);
      }
    }

    private bool _active;
    public bool Active
    {
      get
      {
        return _active;
      }
      set
      {
        SetPropertyValue("Active", ref _active, value);
      }
    }

    private DateTime _createDate;
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

    public UserTracking_PinnedUsers() : base() { }
    public UserTracking_PinnedUsers(Session session) : base(session) { }

    #endregion
  }
}