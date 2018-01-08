
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Region
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
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using Atlas.Domain.Interface;

namespace Atlas.Domain.Model
{
  public class Region : XPLiteObject
  {

    private Int64 _RegionId;
    [Key(AutoGenerate = true)]
    public Int64 RegionId
    {
      get
      {
        return _RegionId;
      }
      set
      {
        SetPropertyValue("RegionId", ref _RegionId, value);
      }
    }
    private NUC_NuCardProfile _Profile;
    [Persistent("ProfileId")]
    public NUC_NuCardProfile Profile
    {
      get
      {
        return _Profile;
      }
      set
      {
        SetPropertyValue("Profile", ref _Profile, value);
      }
    }

    private string _LegacyRegionCode;

    [Persistent, Size(20)]
    [Indexed]
    public string LegacyRegionCode
    {
      get
      {
        return _LegacyRegionCode;
      }
      set
      {
        SetPropertyValue("LegacyRegionCode", ref _LegacyRegionCode, value);
      }
    }

    private string _Description;
    [Persistent]
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

    private PER_Person _CreatedBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get
      {
        return _CreatedBy;
      }
      set
      {
        SetPropertyValue("CreatedBy", ref _CreatedBy, value);
      }
    }

    private DateTime? _CreatedDT;
    [Persistent]
    public DateTime? CreatedDT
    {
      get
      {
        return _CreatedDT;
      }
      set
      {
        SetPropertyValue("CreatedDT", ref _CreatedDT, value);
      }
    }

    #region Constructors

    public Region() : base() { }
    public Region(Session session) : base(session) { }

    #endregion
  }
}
