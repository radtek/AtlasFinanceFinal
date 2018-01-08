/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *     Biometric enumerators
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * ----------------------------------------------------------------------------------------------------------------- */
using System.ComponentModel;


namespace Atlas.Enumerators
{
  public class Biometric
  {
    public enum AppliesTo
    {
      [Description("Applies to Company")]
      Company = 1,

      [Description("Applies to Branch")]
      Branch = 2,

      [Description("Applies to Workstation")]
      Station = 3,

      [Description("Applies to Atlas software")]
      Software = 4,

      [Description("Applies to person type")] // i.e. Person.Type (PER_Type)
      PersonType = 10,

      [Description("Applies to specific person")]
      PersonSpecific = 11
    }

    public enum SettingType
    {
      [Description("Minimum acceptable quality")]
      MinQuality = 1,

      [Description("Immediate minimum accept quality")]
      AcceptQuality = 2,

      [Description("Highest acceptable NFIQ (1-Best, 5- Worst)")]
      NFIQHighest = 3,

      [Description("Maximum scan wait time-out in seconds")]
      MaxWaitTimeInSeconds = 4,

      [Description("Minimum fingers to be enrolled")]
      MinFingers = 5,

      [Description("Minimum security level")]
      MinSecurityLevel = 6,

      [Description("Fingerprint core detection override")]
      DetectCore = 7,

      [Description("Fingerprinting is enabled for staff")]
      FPEnabled = 1000,

      [Description("Fingerprinting is enabled for clients")]
      FPClientEnabled = 1001
    }


    public enum OrientationType
    {
      RightSide = 1,
      UpsideDown = 2
    }


    public enum RequestType
    {
      NotSet = 0,
      Enrolment = 1,
      Identification = 2,
      Verification = 3
    }


    /// <summary>
    /// Request status
    /// </summary>
    public enum RequestStatus
    {
      None = 0,
      TimedOut = 1,

      // LMS has requested the following be performed by the client software
      EnrollmentRequested = 11,
      VerifyRequested = 12,
      IdentificationRequested = 13,

      // Client software has received the request and the GUI operation is pending
      EnrollmentPending = 21,
      VerifyPending = 22,
      IdentificationPending = 23,

      // GUI cancelled
      EnrollmentCancelled = 31,
      VerifyCancelled = 32,
      IdentificationCancelled = 33,

      // GUI Successful
      EnrollmentSuccessful = 41,
      VerifySuccessful = 42,
      IdentificationSuccessful = 43,

      // GUI failed
      EnrollmentFailed = 51,
      VerifyFailed = 52,
      IdentificationFailed = 53,

      // Timed-out failed
      EnrollmentTimedOut = 51,
      VerifyTimedOut = 52,
      IdentificationTimedOut = 53,

      EnrollmentDuplicated = 60
    };


    public enum BiometricClass
    {
      None = 0,
      Fingerprint = 1,
      IdPhoto = 2
    }

  }
}