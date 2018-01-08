using System;


namespace Atlas.Integration
{
  
  public class Integration : IIntegration
  {
    public LoginResult Login(string userName, string password, string userIdNum, string userBranch)
    {
      throw new NotImplementedException();
    }

    public SendSMSResult SendSMS(string loginToken, string cellNumber, string text)
    {
      throw new NotImplementedException();
    }

    public SendOTPResult SendOTP(string loginToken, string cellNumber, string smsTemplate, string otpIdentifier)
    {
      throw new NotImplementedException();
    }

    public VerifyOTPResult VerifyOTP(string loginToken, string correlationId, string otp)
    {
      throw new NotImplementedException();
    }

    public ClientLastActivityResult GetClientLastActivity(string loginToken, string idNumberOrPassport)
    {
      throw new NotImplementedException();
    }

    public ScoreCardResult GetScoreCard(string loginToken, string idNumberOrPassport, bool isPassport, DateTime dateOfBirth)
    {
      throw new NotImplementedException();
    }

    public OpportunityResult AddOpportunity(string loginToken, OpportunityRequest request)
    {
      throw new NotImplementedException();
    }


    public Interface.BranchDetails[] GetBranchList(string loginToken)
    {
      throw new NotImplementedException();
    }
  }
}
