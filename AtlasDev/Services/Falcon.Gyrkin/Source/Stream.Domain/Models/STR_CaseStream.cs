using System;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_CaseStream : XPLiteObject
  {
    private Int64 _caseStreamId;
    [Key(AutoGenerate = true)]
    public Int64 CaseStreamId
    {
      get
      {
        return _caseStreamId;
      }
      set
      {
        SetPropertyValue("CaseStreamId", ref _caseStreamId, value);
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

    private STR_Escalation _escalation;
    [Persistent("EscalationId")]
    public STR_Escalation Escalation
    {
      get
      {
        return _escalation;
      }
      set
      {
        SetPropertyValue("Escalation", ref _escalation, value);
      }
    }

    private STR_Stream _stream;
    [Persistent("StreamId")]
    [Indexed]
    public STR_Stream Stream
    {
      get
      {
        return _stream;
      }
      set
      {
        SetPropertyValue("Stream", ref _stream, value);
      }
    }

    private STR_Priority _priority;
    [Persistent("PriorityId")]
    [Indexed]
    public STR_Priority Priority
    {
      get
      {
        return _priority;
      }
      set
      {
        SetPropertyValue("Priority", ref _priority, value);
      }
    }

    private DateTime _lastPriorityDate;
    [Persistent]
    public DateTime LastPriorityDate
    {
      get
      {
        return _lastPriorityDate;
      }
      set
      {
        SetPropertyValue("LastPriorityDate", ref _lastPriorityDate, value);
      }
    }

    private PER_Person _completedUser;
    [Persistent("CompletedUserId")]
    public PER_Person CompletedUser
    {
      get
      {
        return _completedUser;
      }
      set
      {
        SetPropertyValue("CompletedUser", ref _completedUser, value);
      }
    }

    private STR_Comment _completeComment;
    [Persistent("CompleteCommentId")]
    public STR_Comment CompleteComment
    {
      get
      {
        return _completeComment;
      }
      set
      {
        SetPropertyValue("CompleteComment", ref _completeComment, value);
      }
    }

    private STR_Note _completeNote;
    [Persistent("CompleteNoteId")]
    public STR_Note CompleteNote
    {
      get
      {
        return _completeNote;
      }
      set
      {
        SetPropertyValue("CompleteAccountNote", ref _completeNote, value);
      }
    }

    private DateTime? _completeDate;
    [Persistent]
    public DateTime? CompleteDate
    {
      get
      {
        return _completeDate;
      }
      set
      {
        SetPropertyValue("CompleteDate", ref _completeDate, value);
      }
    }

    private int? _notInterestedCount;
    [Persistent]
    public int? NotInterestedCount
    {
      get
      {
        return _notInterestedCount;
      }
      set
      {
        SetPropertyValue("NotInterestedCount", ref _notInterestedCount, value);
      }
    }

    private PER_Person _createUser;
    [Persistent("CreateUserId")]
    public PER_Person CreateUser
    {
      get
      {
        return _createUser;
      }
      set
      {
        SetPropertyValue("CreateUser", ref _createUser, value);
      }
    }

    private DateTime _createDate;
    [Persistent]
    [Indexed]
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
    [Association("STR_CaseStreamAllocation", typeof(STR_CaseStreamAllocation))]
    public XPCollection<STR_CaseStreamAllocation> CaseStreamAllocations { get { return GetCollection<STR_CaseStreamAllocation>("CaseStreamAllocations"); } }
    [Association("CaseStream_CaseStreamEscalation", typeof(STR_CaseStreamEscalation))]
    public XPCollection<STR_CaseStreamEscalation> CaseStreamEscalations { get { return GetCollection<STR_CaseStreamEscalation>("CaseStreamEscalations"); } }


    #region Constructors

    public STR_CaseStream()
    { }
    public STR_CaseStream(Session session) : base(session) { }

    #endregion

  }
}