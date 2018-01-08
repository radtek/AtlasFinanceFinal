/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2015 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    CompuScan enquiry interface
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
 *     
 *     2012-10-10- Updated with functionality.
 *     
 *     2012-11-12- Added interface for atlas online 
 *     
 *     2015-03-24- Added 'accounts' (NLR/CPA) to QueueEnquiry
 *     
 * ----------------------------------------------------------------------------------------------------------------- */


using System;
using System.Collections.Generic;
using System.ServiceModel;

using Atlas.RabbitMQ.Messages.Credit;


namespace Atlas.Bureau.Service.WCF.Interface
{
  [ServiceContract]
  public interface ICreditServer
  {
    [OperationContract]
    void QueueEnquiry(string legacyBranchNum, string firstName, string surname, string IdNumber, string gender, DateTime dateOfBirth,
                      string addressLine1, string addressLine2, string addressLine3, string addressLine4, string postalCode,
                      string homeTelCode, string homeTelNo, string workTelCode, string workTelNo, string cellNo, bool isIDPassportNo,
                      bool isExistingClient, string requestUser, out decimal totalCPAAccount, out decimal totalNLRAccount, out string nlrEnquiryNo,
                      out string enquiryDate, out int score, out string riskType, out string[] reasons, out string file, 
                      out List<Atlas.RabbitMQ.Messages.Credit.Product> products,
                      out List<Atlas.RabbitMQ.Messages.Credit.NLRCPAAccount> accounts,
                      out string[] errors);

    [OperationContract]
    CreditResponse Enquiry(long accountId, string firstName, string surname, string IdNumber, string gender, DateTime dateOfBirth,
                                               string addressLine1, string addressLine2, string addressLine3, string addressLine4, string postalCode,
                                               string homeTelCode, string homeTelNo, string workTelCode, string workTelNo, string cellNo, bool isIDPassportNo,
                                               bool isExistingClient, string requestUser);

    [OperationContract]
    string GetReport(long enquiryId);
  }
}
