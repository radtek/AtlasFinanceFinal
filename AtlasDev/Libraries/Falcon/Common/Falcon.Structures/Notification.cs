using System;
using System.Collections.Generic;

namespace Falcon.Common.Structures
{
  public sealed class Notification
  {
    public string NotificationId { get; set; }
    public int PriorityLevel { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ActionUrl { get; set; }
    public DateTime NotificationDate { get; set; }
  }

  public sealed class Notifications
  {
    public long UserId { get; set; }
    public List<Notification> Notes { get; set; }
    public int NoteCount { get; set; }
  }
  //public class NotificationSearch
  //{
  //  public long BranchId { get; set; }
  //  public long UserId { get; set; }
  //  public string NotificationKey { get; set; }
  //}
}