using System.Diagnostics;
using Atlas.RabbitMQ.Messages.Notification;

namespace Atlas.Notification.Server.Handlers
{
  public sealed class EventLogHandle
  {
    public static void Send(EventLogNotifyMessage messg)
    {
      if (!EventLog.SourceExists(messg.Source))
        EventLog.CreateEventSource(messg.Source, messg.Log);

      EventLog.WriteEntry(messg.Source, messg.Event, messg.EntryType);
    }
  }
}