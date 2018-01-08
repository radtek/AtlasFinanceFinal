using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class STR_CaseStreamAllocation : XPLiteObject
  {
    private Int64 _caseStreamAllocationId;
    [Key(AutoGenerate = true)]
    public Int64 CaseStreamAllocationId
    {
      get
      {
        return _caseStreamAllocationId;
      }
      set
      {
        SetPropertyValue("CaseStreamAllocationId", ref _caseStreamAllocationId, value);
      }
    }

    private STR_CaseStream _caseStream;
    [Persistent("CaseStreamId")]
    [Indexed]
    [Association("STR_CaseStreamAllocation")]
    public STR_CaseStream CaseStream
    {
      get
      {
        return _caseStream;
      }
      set
      {
        SetPropertyValue("CaseStream", ref _caseStream, value);
      }
    }

    private STR_Escalation _escalation;
    [Persistent("EscalationId")]
    [Indexed]
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

    private PER_Person _allocatedUser;
    [Persistent("AllocatedUserId")]
    [Indexed]
    public PER_Person AllocatedUser
    {
      get
      {
        return _allocatedUser;
      }
      set
      {
        SetPropertyValue("AllocatedUser", ref _allocatedUser, value);
      }
    }

    private DateTime _allocatedDate;
    [Persistent]
    [Indexed]
    public DateTime AllocatedDate
    {
      get
      {
        return _allocatedDate;
      }
      set
      {
        SetPropertyValue("AllocatedDate", ref _allocatedDate, value);
      }
    }

    private int _noActionCount;
    [Persistent]
    public int NoActionCount
    {
      get
      {
        return _noActionCount;
      }
      set
      {
        SetPropertyValue("NoActionCount", ref _noActionCount, value);
      }
    }

    private bool _transferredIn;
    [Persistent]
    public bool TransferredIn
    {
      get
      {
        return _transferredIn;
      }
      set
      {
        SetPropertyValue("TransferredIn", ref _transferredIn, value);
      }
    }

    private DateTime? _transferredOutDate;
    [Persistent]
    public DateTime? TransferredOutDate
    {
      get
      {
        return _transferredOutDate;
      }
      set
      {
        SetPropertyValue("TransferredOutDate", ref _transferredOutDate, value);
      }
    }

    private bool _transferredOut;
    [Persistent]
    [Indexed]
    public bool TransferredOut
    {
      get
      {
        return _transferredOut;
      }
      set
      {
        SetPropertyValue("TransferredOut", ref _transferredOut, value);
      }
    }

    private int _smsCount;
    [Persistent]
    public int SMSCount
    {
      get
      {
        return _smsCount;
      }
      set
      {
        SetPropertyValue("SMSCount", ref _smsCount, value);
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


    #region Constructors

    public STR_CaseStreamAllocation() : base() { }
    public STR_CaseStreamAllocation(Session session) : base(session) { }

    #endregion

  }
}