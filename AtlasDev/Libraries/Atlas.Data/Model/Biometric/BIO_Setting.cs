
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2015 Atlas Finance Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for fingerprint settings
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-11-30- Initial creation
 *     2012-12-03- Upadated 
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;

using DevExpress.Xpo;


namespace Atlas.Domain.Model.Biometric
{
  public class BIO_Setting : XPLiteObject
  {
    #region Entities
      
    [Persistent, Key(AutoGenerate = true)]
    public Int64 FPSettingId { get; set; }

    private Atlas.Enumerators.Biometric.AppliesTo _AppliesTo;
    [Persistent]
    public Atlas.Enumerators.Biometric.AppliesTo AppliesTo
    {
      get { return _AppliesTo; }
      set { SetPropertyValue("AppliesTo", ref _AppliesTo, value); }
    }

    private Atlas.Enumerators.Biometric.SettingType _SettingType;
    [Persistent]
    public Atlas.Enumerators.Biometric.SettingType SettingType
    {
      get { return _SettingType; }
      set { SetPropertyValue("SettingType", ref _SettingType, value); }
    }

    private Int64 _Entity;
    [Persistent]
    [Indexed(Name = "IDX_Entity")]
    public Int64 Entity
    {
      get { return _Entity; }
      set { SetPropertyValue("Entity", ref _Entity, value); }
    }

    private string _Value;
    [Persistent, Size(5000)]
    public string Value
    {
      get { return _Value; }
      set { SetPropertyValue("Value", ref _Value, value); }
    }

    #endregion


    #region Constructors

    public BIO_Setting(Session session) : base(session) { }
    public BIO_Setting() : base() { }

    #endregion


    #region Validation

    /*protected override void OnSaving()
    {

    } */
    #endregion
  }
}
