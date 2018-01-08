
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Province
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using Atlas.Common.Extensions;
using DevExpress.Xpo;
using Atlas.Domain.Interface;

namespace Atlas.Domain.Model
{
  public class Province : XPLiteObject
  {
    private Int64 _ProvinceId;
    [Key(AutoGenerate = false)]
    public Int64 ProvinceId
    {
      get
      {
        return _ProvinceId;
      }
      set
      {
        SetPropertyValue("ProvinceId", ref _ProvinceId, value);
      }
    }

    private string _ShortCode;
    [Persistent, Size(7)]
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
    
    [Persistent, Size(15)]
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

    public Province() : base() { }
    public Province(Session session)
      : base(session)
    {

    }

    #endregion
  }
}
