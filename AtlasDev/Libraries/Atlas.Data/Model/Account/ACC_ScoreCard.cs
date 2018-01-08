using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class ACC_ScoreCard : XPLiteObject
  {
    private Int64 _scoreCardId;
    [Key(AutoGenerate = true)]
    public Int64 ScoreCardId
    {
      get
      {
        return _scoreCardId;
      }
      set
      {
        SetPropertyValue("ScoreCardId", ref _scoreCardId, value);
      }
    }

    private RSK_Enquiry _riskEnquiry;
    [Persistent("RiskEnquiryId")]
    public RSK_Enquiry RiskEnquiry
    {
      get
      {
        return _riskEnquiry;
      }
      set
      {
        SetPropertyValue("RiskEnquiry", ref _riskEnquiry, value);
      }
    }

    private PER_Person _person;
    [Persistent("PersonId")]
    public PER_Person Person
    {
      get
      {
        return _person;
      }
      set
      {
        SetPropertyValue("Person", ref _person, value);
      }
    }

    private bool _isNewClient;
    [Persistent]
    public bool IsNewClient
    {
      get
      {
        return _isNewClient;
      }
      set
      {
        SetPropertyValue("IsNewClient", ref _isNewClient, value);
      }
    }

    private int _score;
    [Persistent]
    public int Score
    {
      get
      {
        return _score;
      }
      set
      {
        SetPropertyValue("Score", ref _score, value);
      }
    }

    private bool _passed;
    [Persistent]
    public bool Passed
    {
      get
      {
        return _passed;
      }
      set
      {
        SetPropertyValue("Passed", ref _passed, value);
      }
    }

    private ACC_ScoreRiskLevel _scoreRiskLevel;
    [Persistent("ScoreRiskLevelId")]
    public ACC_ScoreRiskLevel ScoreRiskLevel
    {
      get
      {
        return _scoreRiskLevel;
      }
      set
      {
        SetPropertyValue("ScoreRiskLevel", ref _scoreRiskLevel, value);
      }
    }

    private DateTime _expiryDate;
    [Persistent("ExpiryDT")]
    public DateTime ExpiryDate
    {
      get
      {
        return _expiryDate;
      }
      set
      {
        SetPropertyValue("ExpiryDate", ref _expiryDate, value);
      }
    }

    private DateTime _createdDate;
    [Persistent("CreatedDT")]
    public DateTime CreatedDate
    {
      get
      {
        return _createdDate;
      }
      set
      {
        SetPropertyValue("CreatedDate", ref _createdDate, value);
      }
    }

    private PER_Person _createdBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get
      {
        return _createdBy;
      }
      set
      {
        SetPropertyValue("CreatedBy", ref _createdBy, value);
      }
    }

    #region Constructors

    public ACC_ScoreCard() : base() { }
    public ACC_ScoreCard(Session session) : base(session) { }

    #endregion
  }
}