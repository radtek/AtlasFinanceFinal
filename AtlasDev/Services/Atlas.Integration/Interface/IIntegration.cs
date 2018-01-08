using System;
using System.ServiceModel;


namespace Atlas.Integration.Interface
{
  [ServiceContract(Namespace = "Atlas.Services.2015.Integration")]
  public interface IIntegration
  {
    /// <summary>
    /// Login to system
    /// </summary>
    /// <param name="system">The calling server system information</param>
    /// <param name="user">The user/agent who this service is on behalf of</param>
    /// <returns>Login result</returns>
    [OperationContract]    
    LoginResult Login(SystemLoginRequest system, UserLoginRequest user);

    /// <summary>
    /// Get listing of branches
    /// </summary>
    /// <param name="loginToken">The result from 'Login': LoginResult.LoginToken</param>
    /// <returns>Listing of Atlas branch information</returns>
    [OperationContract]
    BranchListResult GetBranchList(string loginToken);
    

    /// <summary>
    /// Get listing of active users
    /// </summary>
    /// <param name="loginToken">The result from 'Login': LoginResult.LoginToken</param>
    /// <returns>Listing of Atlas users</returns>
    [OperationContract]
    UsersResult GetUsers(string loginToken);

    /// <summary>
    /// Send a SMS
    /// </summary>
    /// <param name="loginToken">The result from 'Login': LoginResult.LoginToken</param>
    /// <param name="request"></param>
    /// <returns>Result of adding SMS to queue</returns>
    [OperationContract]
    SendSMSResult SendSMS(string loginToken, SendSMSRequest request);

    /// <summary>
    /// Send OTP via SMS
    /// </summary>
    /// <param name="loginToken">The result from 'Login': LoginResult.LoginToken</param>
    /// <param name="request">OTP request parameters</param>
    /// <returns>Result of adding OTP SMS request</returns>
    [OperationContract]
    SendOTPResult SendOTPViaSMS(string loginToken, SendOTPRequest request);
    
    /// <summary>
    /// Get client last activities
    /// </summary>
    /// <param name="loginToken">The result from 'Login': LoginResult.LoginToken</param>
    /// <param name="request"></param>
    /// <returns>Listing of last activities</returns>
    [OperationContract]
    ClientLastActivityResult GetClientLastActivity(string loginToken, ClientLastActivityRequest request);

    /// <summary>
    /// Pull scorecard for client
    /// </summary>
    /// <param name="loginToken">The result from 'Login': LoginResult.LoginToken</param>
    /// <param name="request">The scorecard request parameters</param>
    /// <returns>The basic scorecard information</returns>
    [OperationContract]
    ScoreCardResult GetScoreCard(string loginToken, ScoreCardRequest request);

    /// <summary>
    /// Add a new opportunity
    /// </summary>
    /// <param name="loginToken">The result from 'Login': LoginResult.LoginToken</param>
    /// <param name="request">The add opportunity details</param>
    /// <returns>Result of adding an opportunity</returns>
    [OperationContract]
    AddOpportunityResult AddOpportunity(string loginToken, AddOpportunityRequest request);

    /// <summary>
    /// Check on the status of an opportunity added by 'AddOpportunity'
    /// </summary>
    /// <param name="loginToken">The result from 'Login': LoginResult.LoginToken</param>
    /// <param name="request">The Added opportunities to check on</param>
    /// <returns>Status of added opportunities</returns>
    [OperationContract]
    CheckOpportunityStatusResult CheckOpportunityStatus(string loginToken, CheckOpportunityStatusRequest request);
    
  }

}
