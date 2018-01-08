
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

using DevExpress.Xpo;

using Atlas.Domain.Security;


namespace Atlas.Domain.Model.Biometric
{
  /// <summary>
  /// Class to log results of Fingerprint server requests
  /// </summary>
  public class BIO_LogRequest : XPLiteObject
  {
    [Persistent, Key(AutoGenerate = true)]
    public Int64 LogRequestId { get; set; }
      
    private COR_Machine _Machine;
    [Persistent, Indexed]
    public COR_Machine Machine
    {
      get { return _Machine; }
      set { SetPropertyValue("Machine", ref _Machine, value); }
    }

    private PER_Person _Person;
    [Persistent, Indexed]
    public PER_Person Person
    {
      get { return _Person; }
      set { SetPropertyValue("Person", ref _Person, value); }
    }

    private PER_Person _UserPerson;
    [Persistent, Indexed]
    public PER_Person UserPerson
    {
      get { return _UserPerson; }
      set { SetPropertyValue("UserPerson", ref _UserPerson, value); }
    }

    private DateTime _StartDT;
    [Persistent, Indexed]
    public DateTime StartDT
    {
      get { return _StartDT; }
      set { SetPropertyValue("StartDT", ref _StartDT, value); }
    }

    private DateTime _EndDT;
    [Persistent]
    public DateTime EndDT
    {
      get { return _EndDT; }
      set { SetPropertyValue("EndDT", ref _EndDT, value); }
    }

    private string _Error;
    [Persistent, Size(5000)]
    public string Error
    {
      get { return _Error; }
      set { SetPropertyValue("Error", ref _Error, value); }
    }

    private string _RequestId;
    [Persistent, Size(40)]
    public string RequestId
    {
      get { return _RequestId; }
      set { SetPropertyValue("RequestId", ref _RequestId, value); }
    }

    private int _TimeoutSecs;
    [Persistent]
    public int TimeoutSecs
    {
      get { return _TimeoutSecs; }
      set { SetPropertyValue("TimeoutSecs", ref _TimeoutSecs, value); }
    }

    private Atlas.Enumerators.Biometric.RequestStatus _FPResult;
    [Persistent]
    public Atlas.Enumerators.Biometric.RequestStatus FPResult
    {
      get { return _FPResult; }
      set { SetPropertyValue("FPResult", ref _FPResult, value); }
    }

    private Atlas.Enumerators.Biometric.BiometricClass _BiometricClass;
    [Persistent]
    public Atlas.Enumerators.Biometric.BiometricClass BiometricClass
    {
      get { return _BiometricClass; }
      set { SetPropertyValue("BiometricClass", ref _BiometricClass, value); }
    }

    private string _DeviceId;
    [Persistent, Size(40)]
    public string DeviceId
    {
      get { return _DeviceId; }
      set { SetPropertyValue("DeviceId", ref _DeviceId, value); }
    }

    private PER_Person _SecondaryPerson;
    [Persistent]
    public PER_Person SecondaryPerson
    {
      get { return _SecondaryPerson; }
      set { SetPropertyValue("SecondaryPersonId", ref _SecondaryPerson, value); }
    } 

    #region Constructors

    public BIO_LogRequest(Session session) : base(session) { }
    public BIO_LogRequest() : base() { }

    #endregion

  }
}