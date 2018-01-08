using System;
using System.Linq;
using System.Collections.Generic;

using DevExpress.Xpo;

using Atlas.Integration.Interface;
using Atlas.Server.Implementation.Token;
using Atlas.Domain.Model.Opportunity;
using Atlas.Enumerators;
using Atlas.Common.Interface;


namespace Atlas.Server.Implementation
{
  public class CheckOpportunityStatus_Impl
  {
    public static CheckOpportunityStatusResult CheckOpportunityStatus(ILogging log, 
      string loginToken, CheckOpportunityStatusRequest request)
    {
      try
      {
        log.Information("[CheckOpportunityStatus]- {log.inToken}-{@request}", loginToken, request);

        #region Basic validation
        if (string.IsNullOrEmpty(loginToken))
        {
          return new CheckOpportunityStatusResult() { Error = "Token cannot be empty- login first!" };
        }
        string userId;
        string branch;
        if (!UserToken.TryGetUserInfo(loginToken, out userId, out branch))
        {
          return new CheckOpportunityStatusResult() { Error = "log.in token invalid/has expired" };
        }

        if (request == null)
        {
          return new CheckOpportunityStatusResult() { Error = "Parameter 'request' cannot be empty" };
        }

        if (request.AddOpportunityResultIds == null || request.AddOpportunityResultIds.Length == 0)
        {
          return new CheckOpportunityStatusResult() { Error = "Parameter 'request.AddOpportunityResultIds' cannot be null" };
        }

        if (request.AddOpportunityResultIds.Length > 100)
        {
          return new CheckOpportunityStatusResult() { Error = "Parameter 'request.AddOpportunityResultIds' cannot exceed 100 items per request" };
        }
        #endregion

        var result = new List<OpportunityStatus>();
        using (var uow = new UnitOfWork())
        {
          foreach (var caseId in request.AddOpportunityResultIds)
          {
            OpportunityStatus status;
            var opportunity = uow.Query<OPP_CaseDetail>().FirstOrDefault(s => s.CaseDetailId == caseId);
            if (opportunity != null)
            {
              status = new OpportunityStatus()
              {
                AddOpportunityResultId = caseId,
                GrantedBranch = opportunity.GrantedBranch?.LegacyBranchNum ?? string.Empty,
                GrantedDate = opportunity.GrantedDate,
                GrantedLoanAmount = opportunity.GrantedLoanAmount,
                GrantedPeriodMonths = ToMonths(opportunity.GrantedPeriodType, opportunity.GrantedPeriodVal),
                Status = GetStatus(opportunity.OpportunityState)
              };
            }
            else
            {
              status = new OpportunityStatus()
              {
                AddOpportunityResultId = caseId,
                Status = OpportunityStates.NotFound
              };
            }
            result.Add(status);
          }
        }
     
        return new CheckOpportunityStatusResult() { Status = result.ToArray() };
      }
      catch (Exception err)
      {
        log.Error(err, "CheckOpportunityStatus()");
        return new CheckOpportunityStatusResult() { Error = "Unexpected server error" };
      }
    }


    private static int ToMonths(Account.PeriodFrequency period, int amount)
    {
      switch (period)
      {
        case Account.PeriodFrequency.Weekly:
          return amount / 4;

        case Account.PeriodFrequency.BiWeekly:
          return amount / 2;

        default:
          return amount;
      }
    }


    private static OpportunityStates GetStatus(Opportunity.OpportunityStatus status)
    {
      switch (status)
      {
        case Opportunity.OpportunityStatus.NotSet:
        case Opportunity.OpportunityStatus.New:
          return OpportunityStates.Pending;

        case Opportunity.OpportunityStatus.Successful:
          return OpportunityStates.Successful;

        case Opportunity.OpportunityStatus.Unsuccessful:
          return OpportunityStates.Failed;

        default:
          return OpportunityStates.Pending;
      }
    }

  }
}