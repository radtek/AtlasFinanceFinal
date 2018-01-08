/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Transunion fraud enquiry 
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-04-12- Skeleton created
 *     2012-10-10- Updated with functionality.
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

namespace Atlas.Bureau.Service.WCF.Interface
{
  using Atlas.ThirdParty.Fraud.TransUnion;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.Serialization;
  using System.ServiceModel;
  using System.Text;

  [ServiceContract]
  public interface IFraudServer
  {

    [OperationContract]
    FraudResult FraudEnquiry(Int64? accountId, string idNo, string firstName, string lastName, string addressLine1, string addressLine2,
                             string suburb, string city, string postalCode, string provinceCode, string homeTelCode, string homeTelNo, string workTelCode,
                             string workTelNo, string cellNo, string bankAccountNo, string bankName, string bankBranchCode, string employer);

    [OperationContract]
    FraudResult GetEnquiryForAccount(Int64 accountId);
  }
}
