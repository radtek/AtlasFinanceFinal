/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Interface for Fingerprint functionality
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-11-13- Basic functionality started
 *     
 *     2012-11-28- After trialling the 'Integrated Biometrics' Curve + SDK solution, the design here changed considerably.
 *                 Initially, I planned to use Bitmaps throughout and have all fingerprint intelligence on the server. 
 *                 This meant the server would create the templates and do all verifications. This would be pretty
 *                 secure, but it would also be pretty slow with the current connectivity (100KB * 3 samples * 10 fingers = 3MB).
 *                 
 *                 To improve the user experience and reduce network congestion, I have decided to move a considerable 
 *                 amount of 'intelligence' back to the client. Transferring bitmaps is unnecessary and wasteful, except
 *                 on Enrollment where we want every single scanned bitmap. Everywhere else the IB 9kb templates are used. 
 *                 For login, we merely download the templates for the user and then verify against the templates. This 
 *                 means only 95KB needs to be transferred to the client for verification purposes (2-3 secs on 384kbit).
 *                 These templates can also be cached on the client, so subsequent verifications are instant.
 *                 
 *     2013-04-12  Breaking interface changes- simplify and standardize on using PersonId at all points.
 *     
 *
 * -----------------------------------------------------------------------------------------------------------------  */
#region Using

using System;
using System.Collections.Generic;
using System.ServiceModel;

using Atlas.WCF.FPServer.Security.Interface;

#endregion


namespace Atlas.WCF.FPServer.Interface
{  
  [ServiceContract(Namespace = "Atlas.FP.Core")]
  public interface IFPServer
  {
    #region Enrollment

    // Step 1: Start a capture session
    [OperationContract]
    int StartEnrollPerson(SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions,
      Int64 personId,
      out Int64 startEnrollRef, out string errorMessage);

    // Step 2: Add one or more fingerprints to an existing capture session
    [OperationContract]
    int EnrollFingerprint(SourceRequest sourceRequest, Int64 startEnrollRef, List<FPRawBufferDTO> fpBitmaps,
      out string errorMessage);

    // Step 3: Finalize a session
    [OperationContract]
    int EndEnrollPerson(SourceRequest sourceRequest, Int64 startEnrollRef, out string errorMessage);

    [OperationContract]
    int CancelEnrollPerson(SourceRequest sourceRequest, Int64 startEnrollRef, out string errorMessage);


    // All-in-one routine- (calls the above 3 steps to enrol person in a single call)
    [OperationContract]
    int EnrollPerson(SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions, FPRawBufferDTO[] fpBitmaps,
      bool isStaff, out string errorMessage);
       
    /// Un-enroll- delete fingerprints for a person
    [OperationContract]
    int UnEnrollPerson(SourceRequest sourceRequest, Int64 personId, out string errorMessage);

    #endregion


    #region Identification / verification
      
    [OperationContract]
    int IdentifyPerson(SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions,
      FPRawBufferDTO[] compressedImages,
      out BasicPersonDetailsDTO person, out string errorMessage);

    [OperationContract]
    int GetTemplatesForPerson(SourceRequest sourceRequest, Int64 personId, 
      out List<FPTemplateDTO> templates, out string errorMessage);     
    
    #endregion


    #region Options

    [OperationContract]
    int GetPersonScanOptions(SourceRequest sourceRequest, FPScannerInfoDTO scanner, Int64 personId,
      out FPScannerOptionDTO scanOptions, out string errorMessage);

    [OperationContract]
    int GetPersonVerifyOptions(SourceRequest sourceRequest, FPScannerInfoDTO scanner, Int64 personId,
      out FPVerifyOptionsDTO verifyOptions, out string errorMessage);
      
    #endregion

  }
}