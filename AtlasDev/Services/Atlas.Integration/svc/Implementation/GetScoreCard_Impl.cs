using System;
using System.Text.RegularExpressions;
using System.Linq;

using Atlas.Integration.Interface;
using Atlas.Server.Implementation.Token;
using Atlas.ThirdParty.CS.WCF.Interface;
using Atlas.ThirdParty.CS.Bureau.Client.ClientProxies;
using Atlas.Common.Interface;


namespace Atlas.Server.Implementation
{
  public class GetScoreCard_Impl
  {
    internal static ScoreCardResult GetScoreCard(ILogging log, string loginToken, ScoreCardRequest request)
    {
      try
      {
        log.Information("[GetScoreCard]- {LoginToken}-{@request}", loginToken, request);

        #region Basic validation
        if (string.IsNullOrEmpty(loginToken))
        {
          return new ScoreCardResult() { Error = "Token cannot be empty- login first!" };
        }
        string userId;
        string branch;
        if (!UserToken.TryGetUserInfo(loginToken, out userId, out branch))
        {
          return new ScoreCardResult() { Error = "Login token invalid/has expired" };
        }

        if (request == null)
        {
          return new ScoreCardResult() { Error = "Parameter 'request' cannot be empty" };
        }

        if (string.IsNullOrEmpty(request.IdNumberOrPassport) || request.IdNumberOrPassport.Length < 5 || request.IdNumberOrPassport.Length > 20)
        {
          return new ScoreCardResult() { Error = "Parameter 'request.IdNumberOrPassport' cannot be empty and must be 5-20 characters" };
        }
        var cleanedId = Regex.Replace(request.IdNumberOrPassport, @"[^\d]", string.Empty);
        if (cleanedId.Length < 5)
        {
          return new ScoreCardResult() { Error = "Parameter 'request.IdNumberOrPassport' must consist of at least 5 characters" };
        }

        if (string.IsNullOrEmpty(request.FirstName))
        {
          return new ScoreCardResult() { Error = "Parameter 'request.FirstName' cannot be empty" };
        }

        if (string.IsNullOrEmpty(request.Surname))
        {
          return new ScoreCardResult() { Error = "Parameter 'request.Surname' cannot be empty" };
        }
        #endregion

        ScorecardSimpleResult score = null;
        using (var client = new ScorecardClient())
        {
          score = client.GetSimpleScorecard(branch, request.FirstName, request.Surname, request.IdNumberOrPassport, request.IsPassport);

          if (score == null)
          {
            log.Error("GetSimpleScorecard() returned empty result");
            return new ScoreCardResult() { Error = "Unable to retrieve scorecard" };
          }

          if (!score.Successful)
          {
            log.Error("GetSimpleScorecard() returned empty result");
            return new ScoreCardResult() { Error = string.Format("Error obtaining scorecard: {0}", score.Error) };
          }
        }

        var result = new ScoreCardResult()
        {
          EnquiryId = score.EnquiryId,
          CodexScore = score.Score,
          AtlasProducts = score.AtlasProducts.Select(s => new AtlasProductResult { Passed = s.Outcome, Product = s.ProductDescription, Reason = string.Join(",", s.Reasons) }).ToArray(),
          Comments = string.Empty
        };

        log.Information("[GetScoreCard]- Success: {@Result}", result);
        return result;
      }
      catch (Exception err)
      {
        log.Error(err, "GetScoreCard()");
        return new ScoreCardResult() { Error = "Unexpected server error" };
      }
    }
  }
}