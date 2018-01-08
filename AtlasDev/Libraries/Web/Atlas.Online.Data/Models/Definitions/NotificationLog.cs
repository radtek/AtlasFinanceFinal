using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class NotificationLog : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public long NotificationLogId { get; set; }
    [Indexed]
    public NotificationTask Task { get; set; }
    [Persistent("ApplicationId")]
    [Indexed]
    public Application Application { get; set; }
    [Indexed]
    public DateTime CreateDate { get; set; }

    public NotificationLog() : base() { }
    public NotificationLog(Session session) : base(session) { }

    public enum NotificationTask
    {
      AccountApprovalTask = 0,
      CancelAccountTask = 1,
      CancelPreApprovedAccountTask = 2,
      NotificationOfInactiveAccountTask = 3,
      NotificationOfRecentlyPaidTask = 4,
      OverduePaymentTask = 5,
      PaymentAcknowledgementTask = 6,
      QuotationExpiredTask = 7,
      QuoteSevenDayExpireTask = 8,
      ReminderOfPaymentDueTask = 9
    }
  }
}