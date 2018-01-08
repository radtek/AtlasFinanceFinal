using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class STR_CaseStreamEscalation : XPLiteObject
  {
    private Int64 _caseStreamEscalationId;
    [Key(AutoGenerate = true)]
    public Int64 CaseStreamEscalationId
    {
      get
      {
        return _caseStreamEscalationId;
      }
      set
      {
        SetPropertyValue("CaseStreamEscalationId", ref _caseStreamEscalationId, value);
      }
    }

    private STR_CaseStream _caseStream;
    [Persistent("CaseStreamId")]
    [Indexed]
    [Association("CaseStream_CaseStreamEscalation")]
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

    public STR_CaseStreamEscalation() : base() { }
    public STR_CaseStreamEscalation(Session session) : base(session) { }

    #endregion

  }
}
