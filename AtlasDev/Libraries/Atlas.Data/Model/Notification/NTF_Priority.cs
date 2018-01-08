using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class NTF_Priority : XPLiteObject
  {
    private int _priorityId;
    [Key(AutoGenerate=false)]
    public int PriorityId
    {
      get
      {
        return _priorityId;
      }
      set
      {
        SetPropertyValue("PriorityId", ref _priorityId, value);
      }
    }
    
    [NonPersistent]
    public Enumerators.Notification.NotificationPriority Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Notification.NotificationPriority>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Notification.NotificationPriority>();
      }
    } 

    private string _description;
    [Persistent, Size(10)]
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

    public NTF_Priority() : base() { }
    public NTF_Priority(Session session) : base(session) { }

    #endregion
  }
}
