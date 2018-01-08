/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2015 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *     Implementation of core Fingerprint functionality- provides functionality with all necessary 
 *     business rules enforced (no duplicates, not already enrolled, etc.):
 *         - Fingerprint enrollment
 *         - Verify person using template
 *         - Identify person
 *         - Get templates for person
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-11-31- Basic functionality started
 *     
 *     2012-12-03- Fleshing out
 *     
 *     2012-12-05- Mostly done
 *     
 *     2012-12-12- Testing complete
 *     
 *     2013-04-23- Changed to be more flexible in more easily support different FP templates
 *     
 *     
 * -----------------------------------------------------------------------------------------------------------------  */

using System;
using System.Collections.Generic;
using System.ServiceModel;

using Atlas.WCF.FPServer.Interface;
using Atlas.WCF.FPServer.Security.Interface;
using Atlas.WCF.FPServer.WCF.Implementation.Server;
using Atlas.WCF.FPServer.WCF.Implementation.Server.Enroll;
using Atlas.WCF.FPServer.WCF.Implementation.Server.Templates;
using Atlas.Common.Interface;

namespace Atlas.WCF.FPServer.WCF.Implementation
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class FPServer : IFPServer
  {
    public FPServer(ILogging log)
    {
      _log = log;
    }


    public int GetTemplatesForPerson(SourceRequest sourceRequest, Int64 personId, out List<FPTemplateDTO> templates, out string errorMessage)
    {
      return GetTemplatesForPerson_Impl.GetTemplatesForPerson(_log, sourceRequest, personId, out templates, out errorMessage);
    }
    

    #region Fingerprint enrolment

    /// <summary>
    /// Enrolls the fingerprints for a person- all-in-one routine for high-speed network enrollment. Requires 3 bitmaps per finger. 
    /// Not recommended for slow networks- may time-out.
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="scanner">Scanner info</param>
    /// <param name="scannerOptions">Scanner options</param>
    /// <param name="fpBitmaps">Landscape, white on black Bitmap</param>
    /// <param name="errorMessage">Any server error</param>
    /// <returns>Enumerators.WCFCallResult</returns>
    /// 
    public int EnrollPerson(SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions, FPRawBufferDTO[] fpBitmaps,
      bool isStaff,
      out string errorMessage)
    {
      return EnrollPerson_Impl.EnrollPerson(_log, sourceRequest, scanner, scannerOptions, fpBitmaps, isStaff, out errorMessage);
    }


    /// <summary>
    /// Starts a slow enroll- enrollment is built up
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="scanner"></param>
    /// <param name="scannerOptions"></param>
    /// <returns></returns>     
    public int StartEnrollPerson(SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions,
      Int64 personId,
      out Int64 startEnrollRef, out string errorMessage)
    {
      return StartEnrollPerson_Impl.StartEnrollPerson(_log, sourceRequest, scanner, scannerOptions, personId, out startEnrollRef, out errorMessage);
    }


    /// <summary>
    /// Adds one or more fingerprints to an existing upload session- must upload 3 bitmaps for a single finger. 
    /// This will create a upside-down template and add all data to SQL.
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="startEnrollRef">Reference received from the 'StartEnrollPerson' call</param>
    /// <param name="fpBitmaps">Raw, landscape, white on black IB formatted fingerprint(s)- one or more required per template finger (require exactly 3)</param>    
    /// <param name="allDone">Is this the last </param>
    /// <param name="errorMessage">Any server error message</param>    
    /// <returns>Enumerators.WCFCallResult</returns>
    public int EnrollFingerprint(SourceRequest sourceRequest, Int64 startEnrollRef, List<FPRawBufferDTO> fpBitmaps,
      out string errorMessage)
    {
      return EnrollFingerprint_Impl.EnrollFingerprint(_log, sourceRequest, startEnrollRef, fpBitmaps, out errorMessage);
    }


    /// <summary>
    /// End enrolment- copies the data from SQL to Mongo and in-memory
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="startEnrollRef"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public int EndEnrollPerson(SourceRequest sourceRequest, Int64 startEnrollRef, out string errorMessage)
    {
      return EndEnrollPerson_Impl.EndEnrollPerson(_log, sourceRequest, startEnrollRef, out errorMessage);      
    }


    /// <summary>
    /// Cancel the enrolment process- removes temporary SQL uploaded data
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="startEnrollRef"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public int CancelEnrollPerson(SourceRequest sourceRequest, Int64 startEnrollRef, out string errorMessage)
    {
      return CancelEnrollPerson_Impl.CancelEnrollPerson(_log, sourceRequest, startEnrollRef, out errorMessage);
    }
    

    public int UnEnrollPerson(SourceRequest sourceRequest, long personId, out string errorMessage)
    {
      return UnEnrollPerson_Impl.UnEnrollPerson(_log, sourceRequest, personId, out errorMessage);
    }


    #endregion


    #region Fingerprint identification (CPU intensive)

    /// <summary>
    /// Identifies a person and gets their security infomation
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="scanner">Scanner information</param>
    /// <param name="scannerOptions">Scanner options</param>
    /// <param name="fpTemplate">Template to match against (only TemplateBuffer needs to be populated)</param>
    /// <param name="personId">PersonId found, 0 if not found</param>
    /// <param name="legacyOperatorId">The person's legacy operator ID</param>
    /// <param name="securityId">The security id, if an employee</param>
    /// <param name="personRoles">Roles person has</param>
    /// <param name="errorMessage">Server error messages</param>
    /// <param name="person">Returns found person's details</param>
    /// <returns>Enumerators.WCFCallResult</returns>
    public int IdentifyPerson(SourceRequest sourceRequest, FPScannerInfoDTO scanner, FPScannerOptionDTO scannerOptions, 
      FPRawBufferDTO[] compressedImages,
      out BasicPersonDetailsDTO person, out string errorMessage)
    {
      return Atlas.WCF.FPServer.WCF.Implementation.Server.Identify.IdentifyPerson_Impl.IdentifyPerson(_log, sourceRequest, scanner, scannerOptions, compressedImages, out person, out errorMessage);      
    }

    #endregion


    #region Get options

    /// <summary>
    /// Get scanner settings to be used
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="scanner">Scanner information</param>
    /// <param name="scannerOptions">Scanner options to be used</param>
    /// <returns>Enumerators.WCFCallResult</returns>
    public int GetPersonScanOptions(SourceRequest sourceRequest, FPScannerInfoDTO scanner, Int64 personId,
      out FPScannerOptionDTO scannerOptions, out string errorMessage)
    {
      return Atlas.WCF.FPServer.WCF.Implementation.Server.Options.GetPersonScanOptions_Impl.GetPersonScanOptions(_log, sourceRequest, scanner, personId, out scannerOptions, out errorMessage);      
    }


    /// <summary>
    /// Get verification settings for person
    /// </summary>
    /// <param name="sourceRequest">Source client/user details</param>
    /// <param name="scanner">Scanner information</param>
    /// <param name="personId"></param>
    /// <param name="verifyOptions"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public int GetPersonVerifyOptions(SourceRequest sourceRequest, FPScannerInfoDTO scanner, Int64 personId,
      out FPVerifyOptionsDTO verifyOptions, out string errorMessage)
    {
      return Atlas.WCF.FPServer.WCF.Implementation.Server.Options.GetPersonVerifyOptions_Impl.GetPersonVerifyOptions(_log, sourceRequest, scanner, personId, out verifyOptions, out errorMessage);      
    }

    #endregion
           

    #region Private vars

    // Log4net
    private readonly ILogging _log;    
    
    #endregion
    
  }
}
