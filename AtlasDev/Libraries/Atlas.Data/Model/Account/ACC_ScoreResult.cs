using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model.Account
{
  public sealed class ACC_ScoreResult : XPLiteObject
  {
    private Int64 _scoreResultId;
    [Key]
    public Int64 ScoreResultId
    {
      get
      {
        return _scoreResultId;
      }
      set
      {
        SetPropertyValue("ScoreResultId", ref _scoreResultId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Account.ScoreResult Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.ScoreResult>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.ScoreResult>();
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

    #region Constructors

    public ACC_ScoreResult() : base() { }
    public ACC_ScoreResult(Session session) : base(session) { }

    #endregion
  }
}
