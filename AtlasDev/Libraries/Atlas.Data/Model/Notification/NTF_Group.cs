using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.Model
{
  public class NTF_Group : XPLiteObject
  {
    private int _groupId;
    [Key]
    public int GroupId
    {
      get
      {
        return _groupId;
      }
      set
      {
        SetPropertyValue("GroupId", ref _groupId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Notification.Group Group
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Notification.Group>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Notification.Group>();
      }
    }

    private string _description;
    [Persistent, Size(40)]
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

    public NTF_Group() : base() { }
    public NTF_Group(Session session) : base(session) { }

    #endregion
  }
}