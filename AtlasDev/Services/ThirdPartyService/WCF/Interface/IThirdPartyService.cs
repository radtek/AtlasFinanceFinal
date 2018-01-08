using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Atlas.Enumerators;
using Atlas.ThirdParty.Service.Structure;

namespace Atlas.ThirdParty.Service.Interface
{
  [ServiceContract]
  public interface IThirdPartyService
  {
    #region Authentication

    /// <summary>
    /// Retrieve authentication ticket
    /// </summary>
    /// <param name="username">username stored in security store</param>
    /// <param name="password">password associated with username</param>
    /// <returns>authentication ticket</returns>
    /// 

    [OperationContract]
    string Authenticate(string username, string password);


    [OperationContract]
    bool IsValid(string username, string password, string authenticationTicket);

    [OperationContract]
    bool InValidate(string authenticationTicket);

    [OperationContract]
    string GenerateCheckSum(Transaction transaction);

    #endregion

    #region Transaction

    //[OperationContract]
    //Dictionary<Guid, List<string>> NAEDO_PreValidate(Dictionary<Guid, Structure.Transaction> transactions);

    //[OperationContract]
    //long NAEDO_Submit(string checkSum, Transaction transaction);

    //[OperationContract]
    //Dictionary<Guid, long> NAEDO_SubmitBulk(Dictionary<Guid, Dictionary<string, Structure.Transaction>> transactions);

    //[OperationContract]
    //long? NAEDO_GetControlId(Structure.Transaction transaction);

    //[OperationContract]
    //long? NAEDO_LinkTransactionToControl(string thirdPartyReferenceNo);

    //[OperationContract]
    //Response NAEDO_GetResponse(long referenceId);

    //[OperationContract]
    //Dictionary<Guid, Structure.Response> NAEDO_GetResponseBulk(Dictionary<Guid, long> transactions);

    //[OperationContract]
    //string NAEDO_RequestCancellationToken(string idNo, string cellNo, long controlId);

    //[OperationContract]
    //bool NAEDO_VerifyAndCancel(string checkSum, int OTP);

    #endregion
  }
}
