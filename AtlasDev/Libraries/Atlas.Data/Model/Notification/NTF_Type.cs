using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class NTF_Type : XPLiteObject
  {
    private int _typeId;
    [Key]
    public int TypeId
    {
      get
      {
        return _typeId;
      }
      set
      {
        SetPropertyValue("TypeId", ref _typeId, value);
      }
    }
    
    [NonPersistent]
    public Enumerators.Notification.NotificationType Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Notification.NotificationType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Notification.NotificationType>();
      }
    } 

    private string _description;
    [Persistent, Size(20)]
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

    public NTF_Type() : base() { }
    public NTF_Type(Session session) : base(session) { }

    #endregion
  }
}
