/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    XDS Fraud Prevention
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-12-07- Skeleton
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

namespace Atlas.Bureau.Service.WCF.Interface
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.Serialization;
  using System.ServiceModel;
  using System.Text;
  using Atlas.ThirdParty.Xds;

  [ServiceContract]
  public interface IAuthenticationServer
  {
    [OperationContract]
    IList<QuestionAnswers> GetQuestions(long accountId, string idNo, string refNo);
    [OperationContract]
    VerificationStatus SubmitAnswers(List<QuestionAnswers> questionAnswers);
    [OperationContract]
    Tuple<bool, int> ExceededAuthenticationTries(long accountId, string IdNo);
  }
}
