using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class ACC_ScoreRiskLevel : XPLiteObject
  {
    private Int64 _scoreRiskLevelId;
    [Key(AutoGenerate = true)]
    public Int64 ScoreRiskLevelId
    {
      get
      {
        return _scoreRiskLevelId;
      }
      set
      {
        SetPropertyValue("ScoreRiskLevelId", ref _scoreRiskLevelId, value);
      }
    }

    private RSK_Service _riskService;
    [Indexed]
    [Persistent("RiskServiceId")]
    public RSK_Service RiskService
    {
      get
      {
        return _riskService;
      }
      set
      {
        SetPropertyValue("RiskService", ref _riskService, value);
      }
    }

    private string _description;
    [Persistent, Size(20)]
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        SetPropertyValue("Description", ref _description, value);
      }
    }

    private int? _validDays;
    [Persistent]
    public int? ValidDays
    {
      get
      {
        return _validDays;
      }
      set
      {
        SetPropertyValue("ValidDays", ref _validDays, value);
      }
    }

    #region Constructors

    public ACC_ScoreRiskLevel() : base() { }
    public ACC_ScoreRiskLevel(Session session) : base(session) { }

    #endregion
  }
}