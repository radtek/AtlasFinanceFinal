using System;
using System.Text.RegularExpressions;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Integration.Interface;
using Atlas.Server.Implementation.Token;
using Atlas.Domain.Model;
using Atlas.Domain.Model.Opportunity;
using Atlas.Common.Interface;


namespace Atlas.Server.Implementation
{
  public class AddOpportunity_Impl
  {
    internal static AddOpportunityResult AddOpportunity(ILogging log, string loginToken, AddOpportunityRequest request)
    {
      try
      {
        log.Information("[AddOpportunity]- {LoginToken}-{@request}", loginToken, request);

        #region Basic validation
        if (string.IsNullOrEmpty(loginToken))
        {
          return new AddOpportunityResult() { Error = "Token cannot be empty- login first!", ResultId = -1 };
        }
        string userId;
        string branch;
        if (!UserToken.TryGetUserInfo(loginToken, out userId, out branch))
        {
          return new AddOpportunityResult() { Error = "Login token invalid/has expired", ResultId = -1 };
        }

        if (request == null)
        {
          return new AddOpportunityResult() { Error = "Parameter 'request' cannot be empty", ResultId = -1 };
        }

        if (string.IsNullOrEmpty(request.CallerReferenceId))
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.CallerReferenceId' cannot be empty", ResultId = -1 };
        }

        if (request.CallerReferenceId.Length > 50)
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.CallerReferenceId' cannot be longer than 50 characters", ResultId = -1 };
        }

        if (request.Started == DateTime.MinValue)
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.Started' cannot be empty", ResultId = -1 };
        }

        if (request.Completed == DateTime.MinValue)
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.Completed' cannot be empty", ResultId = -1 };
        }

        if (request.Started > request.Completed)
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.Started' cannot be later than parameter 'request.Completed'", ResultId = -1 };
        }

        if (string.IsNullOrEmpty(request.IdNumber) || request.IdNumber.Length < 5 || request.IdNumber.Length > 20)
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.IdNumber' cannot be empty and must be 5-20 characters", ResultId = -1 };
        }
        var cleanedId = Regex.Replace(request.IdNumber, @"[^\d]", string.Empty);
        if (cleanedId.Length < 5)
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.IdNumber' must consist of at least 5 characters", ResultId = -1 };
        }

        if (string.IsNullOrEmpty(request.FirstName) || request.FirstName.Length > 50)
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.FirstName' cannot be empty and must be 1-50 characters", ResultId = -1 };
        }

        if (string.IsNullOrEmpty(request.Surname) || request.Surname.Length > 50)
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.Surname' cannot be empty and must be 1-50 characters", ResultId = -1 };
        }

        if (DateTime.Now.Subtract(request.DateOfBirth).TotalDays < (18 * 365.25))
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.DateOfBirth' indicates person is under 18 years old", ResultId = -1 };
        }

        if (DateTime.Now.Subtract(request.DateOfBirth).TotalDays > (65 * 365.25))
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.DateOfBirth' indicates person is over 65 years old", ResultId = -1 };
        }

        if (string.IsNullOrEmpty(request.CellularNumber) || Regex.Replace(request.CellularNumber, @"[^\d]", string.Empty).Length != 10)
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.CellularNumber' cannot be empty and must be 10 digits", ResultId = -1 };
        }

        if (request.UserID != userId)
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.UserID' does not match the token logon userID", ResultId = -1 };
        }

        if (request.VettingParameters == null || request.VettingParameters.Length == 0)
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.VettingParameters' cannot be null", ResultId = -1 };
        }
        foreach (var question in request.VettingParameters)
        {
          if (question != null)
          {
            if (string.IsNullOrEmpty(question.Parameter))
            {
              return new AddOpportunityResult() { Error = "Parameter 'request.VettingParameters[].Parameter' cannot be null", ResultId = -1 };
            }
            if (string.IsNullOrEmpty(question.Value))
            {
              return new AddOpportunityResult() { Error = "Parameter 'request.VettingParameters[].Value' cannot be null", ResultId = -1 };
            }
          }
          else
          {
            return new AddOpportunityResult() { Error = "Parameter 'request.VettingParameters[] contains an empty item", ResultId = -1 };
          }
        }

        if (userId == request.IdNumber)
        {
          return new AddOpportunityResult() { Error = "Parameter 'request.IdNumber' cannot be the same as the user's ID number", ResultId = -1 };
        }
        #endregion

        using (var uow = new UnitOfWork())
        {
          #region Check user/branch parameters
          var usePersonDb = uow.Query<PER_Person>().FirstOrDefault(s => s.IdNum == request.UserID);
          if (usePersonDb == null)
          {
            return new AddOpportunityResult() { Error = "Parameter 'request.UserID' contains an unknown user ID number", ResultId = -1 };
          }

          var branchDb = uow.Query<BRN_Branch>().FirstOrDefault(s => s.LegacyBranchNum.PadLeft(3, '0') == request.BranchCode.PadLeft(3, '0'));
          if (branchDb == null)
          {
            return new AddOpportunityResult() { Error = "Parameter 'request.BranchCode' contains an unknown branch code", ResultId = -1 };
          }

          var enquiryDb = uow.Query<BUR_Enquiry>().FirstOrDefault(s => s.EnquiryId == request.ScoreCardEnquiryId);
          if (enquiryDb == null)
          {
            return new AddOpportunityResult() { Error = "Parameter 'request.ScoreCardEnquiryId' contains an unknown enquiry", ResultId = -1 };
          }
          
          // Check for dupe
          var hourAgo = DateTime.Now.Subtract(TimeSpan.FromHours(1));
          var existing = uow.Query<OPP_CaseDetail>()
            .FirstOrDefault(s => (s.Created >= hourAgo && s.ClientIdNum == request.IdNumber) ||
              (s.CallerSysRefId == request.CallerReferenceId));
          if (existing != null)
          {
            return new AddOpportunityResult() { Error = "Duplicate request- ID (request.IdNumber) has already been processed within the past 60 minutes, or duplicate caller reference (request.CallerReferenceId)", ResultId = -1 };
          }
          #endregion

          #region Insert new case
          var newCase = new OPP_CaseDetail(uow)
          {
            OpportunityState = Enumerators.Opportunity.OpportunityStatus.New,
            CallerSysRefId = request.CallerReferenceId,
            Started = request.Started,
            Completed = request.Completed,
            FollowUp = request.FollowUpStart,
            Created = DateTime.Now,
            UserPerson = usePersonDb,
            Branch = branchDb,
            GPSCoords = request.GPSLocation,
            ClientCellularNum = request.CellularNumber,
            ClientIdNum = request.IdNumber,
            ClientFirstname = request.FirstName,
            ClientLastname = request.Surname,
            ClientDateOfBirth = request.DateOfBirth,
            Enquiry = enquiryDb,
            PassedVetting = request.Successful
          };
          #endregion

          #region Insert pre-vetting questions/answers
          foreach (var question in request.VettingParameters)
          {
            new OPP_CasePreVetQuestion(uow)
            {
              CaseDetail = newCase,
              Parameter = question.Parameter,
              Value = question.Value,
              PositiveOutcome = question.PositiveOutcome,
              Comment = question.Comment
            };
          }
          #endregion

          uow.CommitChanges();

          return new AddOpportunityResult() { ResultId = newCase.CaseDetailId };
        }
      }
      catch (Exception err)
      {
        log.Error(err, "AddOpportunity");
        return new AddOpportunityResult() { Error = "Unexpected server error", ResultId = -1 };
      }
    }
  }
}