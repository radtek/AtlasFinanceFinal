using System;

using Test.Atlas;


namespace Test
{
  class Program
  {
    static void Main()
    {
      try
      {
        try
        {
          new IntegrationClient().Using(wcfClient =>
            {
              Console.WriteLine("Logging in...");
              var login = wcfClient.Login(
                new SystemLoginRequest { UserName = "david", Password = "kitley", SystemId = "server" },
                new UserLoginRequest { UserBranch = "01", UserIdNum = "7112155076082" });

              if (!login.Successful)
              {
                Console.WriteLine("Login() error: {0}", login.Error);
                return;
              }
              Console.WriteLine("Logged in: {0}", login.LoginToken);

              /*
              Console.WriteLine("Sending OTP...");
              // Use login            
              var sendOtp = wcfClient.SendOTPViaSMS(login.LoginToken, new SendOTPRequest
              {
                CellularNumber = "0837947058",
                MessageTemplate = "your OTP: {OTP}",
                OtpTemplateId = "{OTP}"
              });

              if (sendOtp.ResultId <= 0 || !string.IsNullOrEmpty(sendOtp.Error))
              {
                Console.WriteLine("SendOTPViaSMS() error: {0}", sendOtp.Error);
                return;
              }
              Console.WriteLine("SendOTPViaSMS(): {0}", sendOtp.OTP);

              Console.WriteLine("Getting branch list");
              var branchList = wcfClient.GetBranchList(login.LoginToken);
              if (branchList.BranchList == null || branchList.BranchList.Length == 0)
              {
                Console.WriteLine("GetBranchList() failed");
                return;
              }
              Console.WriteLine("GetBranchList(): {0}", branchList.BranchList.Length);

              var users = wcfClient.GetUsers(login.LoginToken);
              if (users == null || users.UserList == null || users.UserList.Length == 0)
              {
                Console.WriteLine("GetUsers() failed");
                return;
              }
              Console.WriteLine("GetUsers(): {0}", users.UserList.Length);
              */

              var sendSMS = wcfClient.SendSMS(login.LoginToken, new SendSMSRequest() { CellularNumber = "0837947058", Message = "Test" });
              if (sendSMS == null)
              {
              }
              /*
              //var score = wcfClient.GetScoreCard(login.LoginToken, new ScoreCardRequest() { IdNumberOrPassport = "6512265885085", IsPassport = false, FirstName = "MZOXOLO ABEDNIGO", Surname = "ZANGWA" });
              //if (score == null)
              //{
//
  //            }                           
              
              var activity = wcfClient.GetClientLastActivity(login.LoginToken, new ClientLastActivityRequest { IdNumberOrPassport = "7203095608082", StartDate = new DateTime(2015, 8, 20) });
              if (activity == null)
              {

              }

              //var addOpp = wcfClient.AddOpportunity(login.LoginToken, new AddOpportunityRequest
              //{
              //  BranchCode = "001",
              //  CallerReferenceId = "9999999999",
              //  CellularNumber = "0837947058",
              //  Completed = DateTime.Now,
              //  DateOfBirth = new DateTime(1971, 12, 15),
              //  FirstName = "Keith",
              //  FollowUpStart = DateTime.Now.AddDays(2),
              //  GPSLocation = "10'15'13N12'12'34E",
              //  IdNumber = "1234567890123",
              //  ScoreCardEnquiryId = 489995,
              //  Started = DateTime.Now.AddHours(-1),
              //  Successful = true,
              //  Surname = "BLOWS",
              //  UserID = "7112155076082",
              //  VettingParameters = new VettingParameter[] { 
              //    new VettingParameter { 
              //      Comment = "Duh", 
              //      Parameter = "Are you gainfully employed and suitable", 
              //      Value = "yebo", 
              //      PositiveOutcome = true } }
              //});
              //if (addOpp == null)
              //{

              //}
             */

              var check = wcfClient.CheckOpportunityStatus(login.LoginToken, new CheckOpportunityStatusRequest { AddOpportunityResultIds = new int[] { 1, 2, 3 } });
              if (check == null)
              {
                Console.WriteLine("CheckOpportunityStatus returned null!");
              }
              else
              {
                Console.WriteLine("CheckOpportunityStatus- {0}", check.Status.Length);
              }
            });
        }
        catch (Exception err)
        {
          Console.WriteLine("Comms/server error:' {0}'", err.Message);
        }
      }
      finally
      {
        Console.WriteLine("Press a key...");
        Console.ReadKey();
      }
    }
  }
}
