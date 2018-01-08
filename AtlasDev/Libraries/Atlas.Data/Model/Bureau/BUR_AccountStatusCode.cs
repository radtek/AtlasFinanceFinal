// -----------------------------------------------------------------------
// <copyright file="Accounts.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class BUR_AccountStatusCode : XPLiteObject
  {
    private Int64 _statusCodeId;
    [Key(AutoGenerate = true)]
    public Int64 StatusCodeId
    {
      get { return _statusCodeId; }
      set
      {
        SetPropertyValue("StatusCodeId", ref _statusCodeId, value);
      }
    }

    private string _ShortCode;
    public string ShortCode
    {
      get { return _ShortCode; }
      set
      {
        SetPropertyValue("ShortCode", ref _ShortCode, value);
      }

    }

    private string _Description;
    public string Description
    {
      get { return _Description; }
      set
      {
        SetPropertyValue("Description", ref _Description, value);
      }
    }

    #region Constructors

    public BUR_AccountStatusCode() : base() { }
    public BUR_AccountStatusCode(Session session) : base(session) { }

    #endregion
  }
}
