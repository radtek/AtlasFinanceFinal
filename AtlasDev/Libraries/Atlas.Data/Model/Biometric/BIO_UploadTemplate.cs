
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for fingeprint upload session- stores a single raw fingerprint image (256 grayscale byte array)
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *      2012-12-03- Initial creation
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;

using DevExpress.Xpo;


namespace Atlas.Domain.Model.Biometric
{
  public class BIO_UploadTemplate : XPLiteObject
  {
    #region Entities
       
    [Key(AutoGenerate = true)]
    public Int64 FPUploadImageId { get; set; }

    private BIO_UploadSession _FPUploadSession;
    [Persistent("FPUploadSessionId")]
    [Indexed(Name = "IDX_FPUploadSessionId1")]
    public BIO_UploadSession FPUploadSession
    {
      get { return _FPUploadSession; }
      set { SetPropertyValue("FPUploadSessionId", ref _FPUploadSession, value); }
    }

    private byte[] _FPTemplate;
    [Persistent]
    public byte[] FPTemplate
    {
      get { return _FPTemplate; }
      set { SetPropertyValue("FPTemplate", ref _FPTemplate, value); }
    }

    private int _FingerId;
    [Persistent]
    public int FingerId
    {
      get { return _FingerId; }
      set { SetPropertyValue("FingerId", ref _FingerId, value); }
    }

    private DateTime _UploadedDate;
    [Persistent]
    public DateTime UploadedDate
    {
      get { return _UploadedDate; }
      set { SetPropertyValue("UploadedDate", ref _UploadedDate, value); }
    }

    private int _Quality;
    [Persistent]
    public int Quality
    {
      get { return _Quality; }
      set { SetPropertyValue("Quality", ref _Quality, value); }
    }

    private int _NFIQ;
    [Persistent]
    public int NFIQ
    {
      get { return _NFIQ; }
      set { SetPropertyValue("NFIQ", ref _NFIQ, value); }
    }


    private Enumerators.Biometric.OrientationType _Orientation;
    [Persistent]
    public Enumerators.Biometric.OrientationType Orientation
    {
      get { return _Orientation; }
      set { SetPropertyValue("Orientation", ref _Orientation, value); }
    }

    #endregion


    #region Constructors

    public BIO_UploadTemplate(Session session) : base(session) { }
    public BIO_UploadTemplate() : base() { }

    #endregion


    #region Validation

    protected override void OnSaving()
    {
      if (_FPTemplate == null)
        throw new RequiredPropertyValueMissing(this, "FPTemplate");

      if (_FPUploadSession == null)
        throw new RequiredPropertyValueMissing(this, "FPUploadSession");

      if (_FingerId == 0)
        throw new RequiredPropertyValueMissing(this, "FingerId");
    }
    #endregion
  }
}
