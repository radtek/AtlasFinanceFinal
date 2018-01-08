using System;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_CaseNotification : XPLiteObject
  {
    private Int64 _caseNotificationId;
    [Key(AutoGenerate = true)]
    public Int64 CaseNotificationId
    {
      get
      {
        return _caseNotificationId;
      }
      set
      {
        SetPropertyValue("CaseNotificationId", ref _caseNotificationId, value);
      }
    }

    private STR_Case _case;
    [Persistent("CaseId")]
    [Indexed]
    public STR_Case Case
    {
      get
      {
        return _case;
      }
      set
      {
        SetPropertyValue("Case", ref _case, value);
      }
    }

    private NTF_Notification _notification;
    [Persistent("NotificationId")]
    public NTF_Notification Notification
    {
      get
      {
        return _notification;
      }
      set
      {
        SetPropertyValue("Notification", ref _notification, value);
      }
    }

    private Guid _notificationReference;
    [Persistent]
    public Guid NotificationReference
    {
      get
      {
        return _notificationReference;
      }
      set
      {
        SetPropertyValue("NotificationReference", ref _notificationReference, value);
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

    public STR_CaseNotification()
    { }
    public STR_CaseNotification(Session session) : base(session) { }

    #endregion

  }
}