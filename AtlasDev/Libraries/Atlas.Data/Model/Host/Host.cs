using Atlas.Common.Extensions;
using Atlas.Enumerators;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class Host : XPLiteObject
  {
    private int _hostId;
    [Key(AutoGenerate = false)]
    public int HostId
    {
      get
      {
        return _hostId;
      }
      set
      {
        SetPropertyValue("HostId", ref _hostId, value);
      }
    }

    [NonPersistent]
    public General.Host Type
    {
      get
      {
        return Description.FromStringToEnum<General.Host>();
      }
      set
      {
        value = Description.FromStringToEnum<General.Host>();
      }
    }

    private string _description;
    [Persistent, Size(50)]
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        SetPropertyValue("Description", ref _description, value);
      }
    }
    
    #region Constructors

    public Host() : base() { }
    public Host(Session session) : base(session) { }

    #endregion
  }
}
