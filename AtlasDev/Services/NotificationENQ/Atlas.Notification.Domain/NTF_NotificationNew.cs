using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  /// <summary>
  /// Notification tracking for new notification server
  /// </summary>
  public class NTF_NotificationNew : XPLiteObject
  {
    private int _notificationId;
    [Key(AutoGenerate = true)]
    public int NotificationId
    {
      get { return _notificationId; }
      set { SetPropertyValue("NotificationId", ref _notificationId, value); }
    }

    private long? _eventId;
    [Persistent("EventId")]
    public long? EventId
    {
      get { return _eventId; }
      set { SetPropertyValue("EventId", ref _eventId, value); }
    }

    private long _replyId;
    [Persistent("ReplyId")]
    public long ReplyId
    {
      get { return _replyId; }
      set { SetPropertyValue("ReplyId", ref _replyId, value); }
    }

    private string _reply;
    [Persistent("Reply"), Size(1024)]
    public string Reply
    {
      get { return _reply; }
      set { SetPropertyValue("Reply", ref _reply, value); }
    }

    /// <summary>
    /// Link to branch for charges
    /// </summary>
    private BRN_Branch _branch;
    [Persistent("BranchId")]
    public BRN_Branch Branch
    {
      get { return _branch; }
      set { SetPropertyValue("Branch", ref _branch, value); }
    }

    private NTF_Type _type;
    [Persistent("TypeId")]
    public NTF_Type Type
    {
      get { return _type; }
      set { SetPropertyValue("TypeId", ref _type, value); }
    }

    private NTF_TemplateType _templateType;
    [Persistent("TemplateTypeId")]
    public NTF_TemplateType TemplateType
    {
      get { return _templateType; }
      set { SetPropertyValue("TemplateTypeId", ref _templateType, value); }
    }

    private string _from;
    [Persistent, Size(100)]
    public string From
    {
      get { return _from; }
      set { SetPropertyValue("From", ref _from, value); }
    }

    private string _to;
    [Persistent, Size(400)]
    public string To
    {
      get { return _to; }
      set { SetPropertyValue("To", ref _to, value); }
    }

    private string _cc;
    [Persistent, Size(100)]
    public string Cc
    {
      get { return _cc; }
      set { SetPropertyValue("Cc", ref _cc, value); }
    }

    private string _bcc;
    [Persistent, Size(100)]
    public string Bcc
    {
      get { return _bcc; }
      set { SetPropertyValue("Bcc", ref _bcc, value); }
    }

    private string _subject;
    [Persistent, Size(200)]
    public string Subject
    {
      get { return _subject; }
      set { SetPropertyValue("Subject", ref _subject, value); }
    }

    private string _body;
    [Persistent, Size(int.MaxValue)]
    public string Body
    {
      get { return _body; }
      set { SetPropertyValue("Body", ref _body, value); }
    }

    private bool _isHTML;
    [Persistent]
    public bool IsHTML
    {
      get { return _isHTML; }
      set { SetPropertyValue("IsHTML", ref _isHTML, value); }
    }

    private NTF_Priority _priority;
    [Persistent("PriorityId")]
    public NTF_Priority Priority
    {
      get { return _priority; }
      set { SetPropertyValue("Priority", ref _priority, value); }
    }

    private NTF_Status _status;
    [Persistent("StatusId")]
    public NTF_Status Status
    {
      get { return _status; }
      set { SetPropertyValue("Status", ref _status, value); }
    }

    private DateTime _statusDate;
    [Persistent]
    public DateTime StatusDate
    {
      get { return _statusDate; }
      set { SetPropertyValue("StatusDate", ref _statusDate, value); }
    }

    private int _retryCount;
    [Persistent]
    public int RetryCount
    {
      get { return _retryCount; }
      set { SetPropertyValue("RetryCount", ref _retryCount, value); }
    }

    private DateTime _actionDate;
    [Persistent]
    public DateTime ActionDate
    {
      get { return _actionDate; }
      set { SetPropertyValue("ActionDate", ref _actionDate, value); }
    }

    private Guid? _notificationReference;
    [Persistent]
    public Guid? NotificationReference
    {
      get { return _notificationReference; }
      set { SetPropertyValue("NotificationReference", ref _notificationReference, value); }
    }

    private PER_Person _createUser;
    [Persistent("CreatedBy")]
    [Indexed]
    public PER_Person CreateUser
    {
      get { return _createUser; }
      set { SetPropertyValue("CreateUser", ref _createUser, value); }
    }

    private DateTime _createDate;
    [Persistent]
    public DateTime CreateDate
    {
      get { return _createDate; }
      set { SetPropertyValue("CreateDate", ref _createDate, value); }
    }


    #region Constructors

    public NTF_NotificationNew() : base() { }
    public NTF_NotificationNew(Session session) : base(session) { }

    #endregion

  }
}
