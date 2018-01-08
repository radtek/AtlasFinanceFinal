using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class WFL_ProcessStepJobEscalationNotification:XPLiteObject
  {
    private int _processStepJobEscalationNotificationId;
    [Key(AutoGenerate = true)]
    public int ProcessStepJobEscalationNotificationId
    {
      get
      {
        return _processStepJobEscalationNotificationId;
      }
      set
      {
        SetPropertyValue("ProcessStepJobEscalationNotificationId", ref _processStepJobEscalationNotificationId, value);
      }
    }

    private WFL_ProcessStepJobEscalation _processStepJobEscalation;
    [Persistent("ProcessStepJobEscalationId")]
    public WFL_ProcessStepJobEscalation ProcessStepJobEscalation
    {
      get
      {
        return _processStepJobEscalation;
      }
      set
      {
        SetPropertyValue("ProcessStepJobEscalation", ref _processStepJobEscalation, value);
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
  }
}
