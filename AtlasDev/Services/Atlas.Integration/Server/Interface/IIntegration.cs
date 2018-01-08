using System;
using System.ServiceModel;
using Atlas.Integration.Interface;


namespace Atlas.Integration
{
  [ServiceContract]
  public interface IIntegration
  {
    [OperationContract]
    LoginResult Login(string userName, string password, string userIdNum, string userBranch);
    //                       ^-----------------------^  ^---------------------------------^
    //                                   SP                           User

    [OperationContract]
    BranchDetails[] GetBranchList(string loginToken);

    [OperationContract]
    SendSMSResult SendSMS(string loginToken, string cellNumber, string text);

    [OperationContract]
    SendOTPResult SendOTP(string loginToken, string cellNumber, string smsTemplate, string otpIdentifier);

    [OperationContract]
    VerifyOTPResult VerifyOTP(string loginToken, string correlationId, string otp);

    [OperationContract]
    ClientLastActivityResult GetClientLastActivity(string loginToken, string idNumberOrPassport);

    [OperationContract]
    ScoreCardResult GetScoreCard(string loginToken, string idNumberOrPassport, bool isPassport, DateTime dateOfBirth);

    [OperationContract]
    OpportunityResult AddOpportunity(string loginToken, OpportunityRequest request);
  }
  
}