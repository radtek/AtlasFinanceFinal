
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2015 Atlas Finance Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for fingerprint upload session- stores a single raw fingerprint Bitmap file byte[] data
 *    Bitmap provides for size + RLE compression (lossless)
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
  public class BIO_UploadBitmap : XPLiteObject
  {
    #region Entities
      
    [Key(AutoGenerate = true)]
    public Int64 FPUploadBitmapId { get; set; }

    private BIO_UploadSession _FPUploadSession;
    [Persistent("FPUploadSessionId")]
    [Indexed(Name = "IDX_FPUploadSessionId")]
    public BIO_UploadSession FPUploadSession
    {
      get { return _FPUploadSession; }
      set { SetPropertyValue("FPUploadSessionId", ref _FPUploadSession, value); }
    }

    private int _FingerId;
    [Persistent]
    public int FingerId
    {
      get { return _FingerId; }
      set { SetPropertyValue("FingerId", ref _FingerId, value); }
    }

    private byte[] _FPBitmap;
    [Persistent]
    public byte[] FPBitmap
    {
      get { return _FPBitmap; }
      set { SetPropertyValue("FPBitmap", ref _FPBitmap, value); }
    }

    private DateTime _UploadedDate;
    [Persistent("UploadedDT")]
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

    #endregion


    #region Constructors

    public BIO_UploadBitmap(Session session) : base(session) { }
    public BIO_UploadBitmap() : base() { }

    #endregion


    #region Validation

    protected override void OnSaving()
    {
      if (_FPBitmap == null)
        throw new RequiredPropertyValueMissing(this, "FPBitmap");

      if (_FPUploadSession == null)
        throw new RequiredPropertyValueMissing(this, "FPUploadSession");

      if (_FingerId == 0)
        throw new RequiredPropertyValueMissing(this, "FingerId");
    }
    #endregion
  }

}
