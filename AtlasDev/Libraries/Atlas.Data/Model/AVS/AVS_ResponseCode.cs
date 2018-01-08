using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class AVS_ResponseCode:XPLiteObject
  {
    private Int32 _responseCodeId;
    [Key(AutoGenerate=true)]
    public Int32 ResponseCodeId
    {
      get
      {
        return _responseCodeId;
      }
      set
      {
        SetPropertyValue("ResponseCodeId", ref _responseCodeId, value);
      }
    }

    [NonPersistent]
    public Enumerators.AVS.ResponseCode Type
    {
      get 
      {
        return ResponseCode.FromStringToEnum<Enumerators.AVS.ResponseCode>();
      }
      set
      {
        value = ResponseCode.FromStringToEnum<Enumerators.AVS.ResponseCode>();
      }
    }

    private string _responseCode;
    [Persistent, Size(3)]
    [Indexed]
    public string ResponseCode
    {
      get
      {
        return _responseCode;
      }
      set
      {
        SetPropertyValue("ResponseCode", ref _responseCode, value);
      }
    }

    private AVS_ResponseGroup _responseGroup;
    [Persistent("ResponseGroupId")]
    public AVS_ResponseGroup ResponseGroup
    {
      get
      {
        return _responseGroup;
      }
      set
      {
        SetPropertyValue("ResponseGroup", ref _responseGroup, value);
      }
    }

    private string _description;
    [Persistent, Size(40)]
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

    private AVS_ResponseResult _responseResult;
    [Persistent("ResponseResultId")]
    public AVS_ResponseResult ResponseResult
    {
      get
      {
        return _responseResult;
      }
      set
      {
        SetPropertyValue("ResponseResult", ref _responseResult, value);
      }
    }

    #region Constructors

    public AVS_ResponseCode() : base() { }
    public AVS_ResponseCode(Session session) : base(session) { }

    #endregion
  }
}
