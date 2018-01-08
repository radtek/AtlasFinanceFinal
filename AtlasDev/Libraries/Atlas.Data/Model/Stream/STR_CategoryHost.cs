using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class STR_CategoryHost : XPLiteObject
  {
    private Int64 _categoryHostId;
    [Key(AutoGenerate = true)]
    public Int64 CategoryHostId
    {
      get
      {
        return _categoryHostId;
      }
      set
      {
        SetPropertyValue("CategoryHostId", ref _categoryHostId, value);
      }
    }

    private STR_Category _category;
    [Persistent("CategoryId")]
    public STR_Category Category
    {
      get
      {
        return _category;
      }
      set
      {
        SetPropertyValue("Category", ref _category, value);
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

    public STR_CategoryHost() : base() { }
    public STR_CategoryHost(Session session) : base(session) { }

    #endregion

  }
}