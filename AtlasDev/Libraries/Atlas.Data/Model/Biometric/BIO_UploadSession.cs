
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2015 Atlas Finance Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for a Fingerprint upload session- used by client to background upload fingerprints one or more at a time
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
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;

using Atlas.Domain.Model;
using Atlas.Domain.Security;

using DevExpress.Xpo;


namespace Atlas.Domain.Model.Biometric
{
  /// <summary>
  /// This class is used for Fingerprint upload sessions- ability for client to background upload 1
  /// fingerprint image at a time. The session allows for upload expiry/security.
  /// </summary>
  public class BIO_UploadSession : XPLiteObject
  {
    #region Entities
   
    [Key(AutoGenerate = true)]
    public Int64 FPUploadSessionId { get; set; }

    private DateTime _StartDate;
    [Persistent("StartDT")]
    public DateTime StartDate
    {
      get { return _StartDate; }
      set { SetPropertyValue("StartDate", ref _StartDate, value); }
    }

    private DateTime _LastUploadDate;
    [Persistent("LastUploadDT")]
    public DateTime LastUploadDate
    {
      get { return _LastUploadDate; }
      set { SetPropertyValue("LastUploadDate", ref _LastUploadDate, value); }
    }

    private COR_Machine _Machine;
    [Persistent("MachineId")]
    public COR_Machine Machine
    {
      get { return _Machine; }
      set { SetPropertyValue("Machine", ref _Machine, value); }
    }

    private Int64 _PersonId;
    [Persistent]
    public Int64 PersonId
    {
      get { return _PersonId; }
      set { SetPropertyValue("PersonId", ref _PersonId, value); }
    }

    private Int64 _UserPersonId;
    [Persistent]
    public Int64 UserPersonId
    {
      get { return _UserPersonId; }
      set { SetPropertyValue("UserPersonId", ref _UserPersonId, value); }
    }

    private Int64 _AdminPersonId;
    [Persistent]
    public Int64 AdminPersonId
    {
      get { return _AdminPersonId; }
      set { SetPropertyValue("AdminPersonId", ref _AdminPersonId, value); }
    }

    #endregion


    #region Constructors

    public BIO_UploadSession(Session session) : base(session) { }
    public BIO_UploadSession() : base() { }

    #endregion


    #region Validation

    protected override void OnSaving()
    {
      if (_PersonId == 0)
        throw new RequiredPropertyValueMissing(this, "PersonId");

      if (_StartDate == null || _StartDate == DateTime.MinValue)
        throw new RequiredPropertyValueMissing(this, "StartDate");

      if (_Machine == null)
        throw new RequiredPropertyValueMissing(this, "Machine");
    }
    #endregion
  }
}
