
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
  public class BUR_AccountTypeCode : XPLiteObject
  {
    private Int64 _typeCodeId;
    [Key(AutoGenerate = true)]
    public Int64 TypeCodeId
    {
      get
      {
        return _typeCodeId;
      }
      set
      {

        SetPropertyValue("TypeCodeId", ref _typeCodeId, value);
      }
    }

    private string _ShortCode;
    public string ShortCode
    {
      get
      {
        return _ShortCode;
      }
      set
      {
        SetPropertyValue("ShortCode", ref _ShortCode, value);
      }
    }

    private string _Description;
    public string Description
    {
      get
      {
        return _Description;
      }
      set
      {
        SetPropertyValue("Description", ref _Description, value);
      }
    }

    #region Constructors

    public BUR_AccountTypeCode() : base() { }
    public BUR_AccountTypeCode(Session session) : base(session) { }

    #endregion
  }
}
