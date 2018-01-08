using Atlas.Common.Extensions;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class AVS_ResponseResult:XPLiteObject
  {
    private Int32 _responseResultId;
    [Key]
    public Int32 ResponseResultId
    {
      get
      {
        return _responseResultId;
      }
      set
      {
        SetPropertyValue("ResponseResultId", ref _responseResultId, value);
      }
    }

    [NonPersistent]
    public Enumerators.AVS.ResponseResult Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.AVS.ResponseResult>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.AVS.ResponseResult>();
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

    public AVS_ResponseResult() : base() { }
    public AVS_ResponseResult(Session session) : base(session) { }

    #endregion
  }
}
