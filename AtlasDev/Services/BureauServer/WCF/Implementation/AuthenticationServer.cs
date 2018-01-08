using System.ServiceModel;
using log4net;
using System;
using DevExpress.Xpo;
using Atlas.Domain.Model;
using System.Linq;
using Atlas.ThirdParty.Xds;
using System.Collections.Generic;
using Atlas.Bureau.Service.WCF.Interface;

namespace Atlas.Bureau.Service.WCF.Implemenation
{
  /// <summary>
  /// Authentication Server Implementation
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class AuthenticationServer : IAuthenticationServer
  {
    #region Private Members

    // Log4net
    private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    const int Delay = 16;
    const string SubscriberCode = "1442";
    const string SecurityCode = "ATL42";

    #endregion

    /// <summary>
    /// Get a collection of questions to do a challenge to the client
    /// </summary>
    /// <param name="accountId">Account identifier</param>
    /// <param name="IdNo">ID No of the client to retrive the questions</param>
    /// <param name="refNo">Unique reference no</param>
    /// <returns></returns>
    public IList<QuestionAnswers> GetQuestions(long accountId, string IdNo, string refNo)
    {
      try
      {
        _log.Info("Creating authentication object / logging into authenticatio provider...");
        using (var authenticationQuestions = new Atlas.ThirdParty.Xds.AuthenticationQuestionImpl())
        {
          _log.Info("Requesting questions from authentication provider...");
          return authenticationQuestions.GetQuestions(accountId, IdNo, refNo);
        }
      }
      catch (Exception ex)
      {
        _log.Fatal(ex.Message, ex);
        throw ex;
      }
    }

    /// <summary>
    /// Get the challenge response for the questions
    /// </summary>
    /// <param name="questionAnswers">Collection of questions returned from GetQuestions</param>
    /// <returns></returns>
    public VerificationStatus SubmitAnswers(List<QuestionAnswers> questionAnswers)
    {
      try
      {
        using (var authenticationQuestions = new Atlas.ThirdParty.Xds.AuthenticationQuestionImpl())
        {
          return authenticationQuestions.SubmitAnswers(questionAnswers);
        }
      }
      catch (Exception ex)
      {
        _log.Fatal(ex.Message, ex);
        throw ex;
      }
    }


    public Tuple<bool, int> ExceededAuthenticationTries(long accountId, string IdNo)
    {
      try
      {
        using (var uow = new UnitOfWork())
        {
          var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(p => p.AccountId == accountId);

          if (account == null)
            throw new Exception(string.Format("Account {0} does not exist in the database", accountId));

          var person = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.PersonId == account.Person.PersonId);

          if (person == null)
            throw new Exception(string.Format("Person does not exist in the database for account {0}", accountId));


          var fpmCollection = new XPQuery<FPM_Authentication>(uow).Where(p => p.Person.PersonId == person.PersonId);

          List<long> incomplete = new List<long>();

          foreach (var item in fpmCollection)
          {
            if (item.Completed && item.Authenticated && item.Enabled)
            {
              _log.Info("Person passed authentication");
              return new Tuple<bool, int>(true, incomplete.Count());
            }
            else if (!item.Authenticated)
            {
              incomplete.Add(item.AuthenticationId);
            }
          }

          if (incomplete.Count() >= 2)
            return new Tuple<bool, int>(false, incomplete.Count());
        }

        return new Tuple<bool,int>(false, 0);
      }
      catch (Exception ex)
      {
        _log.Fatal(ex.Message, ex);
        throw ex;
      }
    }
  }
}