using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class STR_GroupHost : XPLiteObject
  {
    private int _groupHostId;
    [Key(AutoGenerate = true)]
    public int GroupHostId
    {
      get
      {
        return _groupHostId;
      }
      set
      {
        SetPropertyValue("GroupHostId", ref _groupHostId, value);
      }
    }

    private STR_Group _group;
    [Persistent("GroupId")]
    public STR_Group Group
    {
      get
      {
        return _group;
      }
      set
      {
        SetPropertyValue("Group", ref _group, value);
      }
    }

    private Host _host;
    [Persistent("HostId")]
    public Host Host
    {
      get
      {
        return _host;
      }
      set
      {
        SetPropertyValue("Host", ref _host, value);
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


    #region Constructors

    public STR_GroupHost() : base() { }
    public STR_GroupHost(Session session) : base(session) { }

    #endregion

  }
}