using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class NTF_Status : XPLiteObject
  {
    private int _statusId;
    [Key(AutoGenerate=false)]
    public int StatusId
    {
      get
      {
        return _statusId;
      }
      set
      {
        SetPropertyValue("StatusId", ref _statusId, value);
      }
    }
    
    [NonPersistent]
    public Enumerators.Notification.NotificationStatus Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Notification.NotificationStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Notification.NotificationStatus>();
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

    public NTF_Status() : base() { }
    public NTF_Status(Session session) : base(session) { }

    #endregion
  }
}
