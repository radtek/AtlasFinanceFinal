/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Altech NAEDO interface 
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-09-18- Created
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.ServiceModel;


namespace Atlas.ThirdParty.CS.WCF.Interface
{
  // TODO: ASS requires the default... have to sync with ASS...
  //[ServiceContract(Namespace = "Atlas.ASS.Utils")] 
  [ServiceContract(Namespace = "Atlas.Services.2015.Scorecard")]
  public interface IScorecardServer
  {    
    [OperationContract]
    ScorecardV2Result GetScorecardV2(string legacyBranchNum, string firstName, string surname, string idNumber, string gender, DateTime dateOfBirth,
                      string addressLine1, string addressLine2, string addressLine3, string addressLine4, string postalCode,
                      string homeTelCode, string homeTelNo, string workTelCode, string workTelNo, string cellNo, bool isIdPassportNo);


    [OperationContract]
    ScorecardSimpleResult GetSimpleScorecard(string legacyBranchNum, string firstName, string surname, string idNumber, bool isIdPassportNo);
  }
}
