/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
* 
*  Description:
*  ------------------
*    Bank account verification service 
* 
* 
*  Author:
*  ------------------
*     Keith Blows
* 
* 
*  Revision history: 
*  ------------------ 
*     2012-04-12- Skeleton created
*     
*     2012-10-29- Use Lee's routine to calculate CDV- requires generic bank accounts...
* 
* ----------------------------------------------------------------------------------------------------------------- */
using System;
using System.ServiceModel;

namespace AtlasServer.WCF.Interface
{
  // TODO: Have to sync namespace with ASS...
  //[ServiceContract(Namespace = "Atlas.Core.Bank")]
  [ServiceContract]
  public interface IAVS
  {
    // Peform bank account check digit validation (CDV)
    [OperationContract]
    bool CDV_Perform(string branchCode, string accountNumber, int accountType,
      out string errorMessage, out int result);

  }
}
