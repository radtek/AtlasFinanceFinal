/***********************************************************************************************************
 * 
 * 1. Cards which do not have an expiry date- get expiry date from XML-RPC
 * 
 * 2. Cards edited in past 24 hours and now stopped- check if any funds and move the funda back to profile
 * 
 * 3. Check for cards which have become inactive (too many?)
 * 
 * 4. Cards which are about/have just expired, check if any funds and move the funds back to profile
 * 
 * 5. Cards which have been allocated but do not have a profile- try determine their profile by doing a 
 *    balance enquiry against each profile.
 * 
 * 
 ***********************************************************************************************************/

using System;
using System.Linq;

using Serilog;
using Newtonsoft.Json;
using Quartz;

using DevExpress.Xpo;

using Atlas.ThirdParty.XMLRPC.Classes;
using Atlas.ThirdParty.XMLRPC.Utils;
using Atlas.Domain.Model;
using Atlas.Server.NuCard.Utils;
using Atlas.NuCard.Repository;
using Atlas.NuCard.WCF.Interface;


namespace Atlas.WCF.QuartzTasks
{
  [DisallowConcurrentExecution]
  public class NuCardCheckCards : IJob
  {
    /// <summary>
    /// Main Quartz Task
    /// </summary>
    /// <param name="context"></param>
    public void Execute(IJobExecutionContext context)
    {
      _log.Information("NuCard checks starting");

      var endPoint = CachedValues.TutukaEndpoint;
      if (string.IsNullOrEmpty(endPoint))
      {
        throw new ArgumentNullException("Tutuka endpoint has not been configured in database!");
      }

      try
      {
        decimal balance;
        Enumerators.NuCard.NuCardStatus cardStatus = Enumerators.NuCard.NuCardStatus.NotSet;
        string errorMessage;
        DateTime expiryDate;

        #region Get all cards which appear to have invalid expiry dates and perform a balance enquiry to get the correct expiry date
        _log.Information("Invalid expiry dates- Process starting");
        using (var unitOfWork = new UnitOfWork())
        {
          var inUseStatus = unitOfWork.Query<NUC_NuCardStatus>().First(c => c.NuCardStatusId == (int)Atlas.Enumerators.NuCard.NuCardStatus.USE).NuCardStatusId;
          var cardsWithBadExpiry = unitOfWork.Query<NUC_NuCard>().Where(s =>
            s.Status.NuCardStatusId == inUseStatus &&
            s.NuCardProfile != null &&
            s.NuCardProfile.ProfileNum != NuCardConsts.NUCARD_PROFILENUM_UNDETERMINED &&
            ((DateTime?)s.ExpiryDT == null || s.ExpiryDT >= DateTime.Now.AddYears(3).AddMonths(6) || s.ExpiryDT <= new DateTime(2010, 1, 1)));

          foreach (var card in cardsWithBadExpiry)
          {
            if (GetCardBalance(endPoint, card.NuCardProfile.Password, card.NuCardProfile.ProfileNum, card.NuCardProfile.TerminalId, card.CardNum,
             out balance, out cardStatus, out expiryDate, out errorMessage))
            {
              card.ExpiryDT = expiryDate;
              card.LastEditedDT = DateTime.Now;
            }

            // Give Tutuka/server some breathing room
            System.Threading.Thread.Sleep(50);
          }

          unitOfWork.CommitChanges();
        }
        _log.Information("Invalid expiry dates- Process completed");
        #endregion

        #region Get cards stopped in the last 36 hours and ensure that the funds have been removed from the card
        _log.Information("Cards recently stopped/lost: Process starting");
        Decimal amountTransferred = 0;
        int cardsTransferred = 0;
        using (var unitOfWork = new UnitOfWork())
        {
          var stoppedOrExpired = unitOfWork.Query<NUC_NuCardStatus>().Where(s =>
            s.NuCardStatusId != (int)Enumerators.NuCard.NuCardStatus.USE &&
            s.NuCardStatusId != (int)Enumerators.NuCard.NuCardStatus.ISSUE &&
            s.NuCardStatusId != (int)Enumerators.NuCard.NuCardStatus.Active &&
            s.NuCardStatusId != (int)Enumerators.NuCard.NuCardStatus.InStock &&
            s.NuCardStatusId != (int)Enumerators.NuCard.NuCardStatus.InTransit &&
            s.NuCardStatusId != (int)Enumerators.NuCard.NuCardStatus.Linked)
            .Select(s => s.NuCardStatusId)
            .ToList();

          var cardsJustStoppedOrExpired = unitOfWork.Query<NUC_NuCard>().Where(s =>
            s.NuCardProfile != null && s.NuCardProfile.ProfileNum != NuCardConsts.NUCARD_PROFILENUM_UNDETERMINED &&
            stoppedOrExpired.Contains(s.Status.NuCardStatusId) &&
            s.LastEditedDT > DateTime.Now.AddHours(-36));

          foreach (var card in cardsJustStoppedOrExpired)
          {
            _log.Information("Card recently stopped/lost: {0}- checking...", card.CardNum);
            if (GetCardBalance(endPoint, card.NuCardProfile.Password, card.NuCardProfile.ProfileNum, card.NuCardProfile.TerminalId, card.CardNum,
             out balance, out cardStatus, out expiryDate, out errorMessage) && balance > 10.0M && cardStatus != Enumerators.NuCard.NuCardStatus.Active)
            {
              _log.Information("Card recently stopped/lost: {0}- Transferring R{1:F2} back to the Atlas profile (NuCard reports state as: {2})", card.CardNum, balance, cardStatus);
              if (TransferCardFundsToProfile(endPoint, card.NuCardProfile.Password, card.NuCardProfile.ProfileNum, card.NuCardProfile.TerminalId, card.CardNum, balance, out errorMessage))
              {
                amountTransferred += balance;
                cardsTransferred++;
                _log.Information("Card recently stopped/lost: {0}- R{1:F2} was successfully transferred back to profile", card.CardNum, balance);
              }
              else
              {
                _log.Error("Card recently stopped/lost: {0}- Failed to transfer funds back to profile: '{1}'", card.CardNum, errorMessage);
              }
            }
            else
            {
              _log.Information("Card recently stopped/lost: {0}- No action was performed- {1}", card.CardNum, errorMessage);
            }

            // Give Tutuka/server some breathing room
            System.Threading.Thread.Sleep(50);
          }
        }
        _log.Information("Cards recently stopped/lost: R{0:F2} transferred from {1} cards back to Atlas profiles", amountTransferred, cardsTransferred);
        #endregion

        #region Get cards with a null status
        _log.Information("Cards with null status- Process starting...");
        using (var unitOfWork = new UnitOfWork())
        {
          var cardsWithNoStatus = unitOfWork.Query<NUC_NuCard>().Where(s => s.Status == null);
          var profiles = unitOfWork.Query<NUC_NuCardProfile>().Where(s => s.ProfileNum != null && s.ProfileNum != NuCardConsts.NUCARD_PROFILENUM_UNDETERMINED);

          foreach (var card in cardsWithNoStatus)
          {
            _log.Information("Card with no status: getting balance: {0}", card.CardNum);

            var foundCard = false;

            if (card.NuCardProfile != null)
            {
              foundCard = (GetCardBalance(endPoint, card.NuCardProfile.Password, card.NuCardProfile.ProfileNum, card.NuCardProfile.TerminalId, card.CardNum,
                out balance, out cardStatus, out expiryDate, out errorMessage));
            }
            else
            {
              foreach (var profile in profiles)
              {
                foundCard = (GetCardBalance(endPoint, profile.Password, profile.ProfileNum, profile.TerminalId, card.CardNum,
                  out balance, out cardStatus, out expiryDate, out errorMessage));
                if (foundCard)
                {
                  card.NuCardProfile = profile;
                  break;
                }
              }
            }

            if (foundCard && cardStatus != Enumerators.NuCard.NuCardStatus.NotSet)
            {
              card.Status = unitOfWork.Query<NUC_NuCardStatus>().FirstOrDefault(s => s.NuCardStatusId == (int)cardStatus);
              card.LastEditedDT = DateTime.Now;

              _log.Information("Card with no status- Updating card status: {0}", cardStatus);
            }

            // Give Tutuka/server some breathing room
            System.Threading.Thread.Sleep(50);
          }

          unitOfWork.CommitChanges();
        }
        _log.Information("Cards with null status- Process completed");
        #endregion

        #region Fix *allocated* cards which have *not* been allocated to a profile, by iterating all profiles and seeing which profile returns a balance...
        _log.Information("Allocated cards with no profile- Process starting...");
        using (var unitOfWork = new UnitOfWork())
        {
          var unallocateProfile = unitOfWork.Query<NUC_NuCardProfile>().First(s => s.ProfileNum == NuCardConsts.NUCARD_PROFILENUM_UNDETERMINED);
          var allocated = unitOfWork.Query<NUC_NuCardStatus>().Where(s =>
            s.NuCardStatusId != (int)Enumerators.NuCard.NuCardStatus.InStock &&
            s.NuCardStatusId != (int)Enumerators.NuCard.NuCardStatus.InTransit &&
            s.NuCardStatusId != (int)Enumerators.NuCard.NuCardStatus.FAULT &&
            s.NuCardStatusId != (int)Enumerators.NuCard.NuCardStatus.NotSet);
          var allocatedCardsWithNoProfile = unitOfWork.Query<NUC_NuCard>().Where(s => s.NuCardProfile == null && allocated.Contains(s.Status));
          var profiles = unitOfWork.Query<NUC_NuCardProfile>().Where(s => s.ProfileNum != null && s.ProfileNum != NuCardConsts.NUCARD_PROFILENUM_UNDETERMINED);
          foreach (var card in allocatedCardsWithNoProfile)
          {
            var found = false;
            foreach (var profile in profiles)
            {
              _log.Information("Card with no profile- Attempting balance enquiry with profile: {0}, Profile: {1}", card.CardNum, profile.ProfileNum);

              if (GetCardBalance(endPoint, profile.Password, profile.ProfileNum, profile.TerminalId, card.CardNum, out balance, out cardStatus, out expiryDate, out errorMessage))
              {
                card.ExpiryDT = expiryDate;
                card.NuCardProfile = profile;
                card.LastEditedDT = DateTime.Now;

                _log.Information("Card with no profile- Linking card to profile: {0}, Profile: {1}", card.CardNum, profile.ProfileNum);
                found = true;
                break;
              }

              // Give Tutuka/server some breathing room
              System.Threading.Thread.Sleep(50);
            }

            if (!found)
            {
              card.NuCardProfile = unallocateProfile;
              card.LastEditedDT = DateTime.Now;
              _log.Information("Card with no profile- Linking card to default profile: {0}, Profile: {1}", card.CardNum, unallocateProfile.ProfileNum);
            }
          }
          unitOfWork.CommitChanges();
        }
        _log.Information("Allocated cards with no profile- Process completed");
        #endregion
      }
      catch (Exception err)
      {
        _log.Error("Execute: {0}", err);
      }

      _log.Information("NuCard checks completed");
    }


    /// <summary>
    /// Gets card's status (limited to if stopped, balance and expiry date)
    /// </summary>
    /// <param name="endPoint"></param>
    /// <param name="key"></param>
    /// <param name="nuCardProfileNumber"></param>
    /// <param name="terminalId"></param>
    /// <param name="nuCardNumber"></param>
    /// <param name="balance"></param>
    /// <param name="cardStatus"></param>
    /// <param name="expiryDate"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    private static bool GetCardBalance(string endPoint, string key, string nuCardProfileNumber, string terminalId, string nuCardNumber,
      out Decimal balance, out Enumerators.NuCard.NuCardStatus cardStatus, out DateTime expiryDate, out string errorMessage)
    {
      var result = false;
      cardStatus = Enumerators.NuCard.NuCardStatus.FAULT;
      balance = 0.0M;
      errorMessage = null;
      expiryDate = DateTime.MinValue;

      try
      {
        #region Perform Balance XML request
        var xmlRequest = new Balance_Input()
        {
          cardNumber = nuCardNumber,
          profileNumber = nuCardProfileNumber,
          terminalID = terminalId,
          transactionDate = DateTime.Now,
          transactionID = Guid.NewGuid().ToString()
        };
        string xmlSent;
        string xmlRecv;
        var startDt = DateTime.Now;
        var xmlResult = NuCardXMLRPCUtils.Balance(endPoint, key, xmlRequest, out xmlSent, out xmlRecv, out errorMessage);
        var endDt = DateTime.Now;

        NuCardDb.LogAdminRequest(null, null, startDt, endDt, Enumerators.NuCard.AdminRequestType.CardBalance, xmlResult.resultCode,
          xmlResult.balanceAmount != null ? (Decimal)xmlResult.balanceAmount.Value / 100M : 0, nuCardNumber,
          null, xmlSent, xmlRecv, xmlResult.clientTransactionID, xmlResult.serverTransactionID, errorMessage);
        #endregion

        if (xmlResult.resultCode.HasValue && xmlResult.resultCode == (int)Enumerators.NuCard.AdminRequestResult.Successful &&
          xmlResult.balanceAmount.HasValue && xmlResult.expiryDate.HasValue)
        {
          result = true;
          balance = (Decimal)xmlResult.balanceAmount / 100M;
          expiryDate = (DateTime)xmlResult.expiryDate;

          #region Card status...
          cardStatus = Enumerators.NuCard.NuCardStatus.Active; // Best guess?
          if (!string.IsNullOrEmpty(xmlResult.lost) && xmlResult.lost.ToLower() == "yes")
          {
            cardStatus = Enumerators.NuCard.NuCardStatus.Stopped_Lost;
          }
          else if (!string.IsNullOrEmpty(xmlResult.expired) && xmlResult.expired.ToLower() == "yes")
          {
            cardStatus = Enumerators.NuCard.NuCardStatus.Expired;
          }
          else if (!string.IsNullOrEmpty(xmlResult.stolen) && xmlResult.stolen.ToLower() == "yes")
          {
            cardStatus = Enumerators.NuCard.NuCardStatus.Stopped_Stolen;
          }
          else if (!string.IsNullOrEmpty(xmlResult.stopped) && xmlResult.stopped.ToLower() == "yes")
          {
            cardStatus = Enumerators.NuCard.NuCardStatus.Stopped_NoLongerActive;
          }
          #endregion
        }
        else
        {
          if (string.IsNullOrEmpty(errorMessage))
          {
            errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
          }
        }
      }
      catch (Exception err)
      {
        _log.Error("GetCardBalance: {0}", err);
        errorMessage = err.Message;
      }

      return result;
    }


    /// <summary>
    /// Transfers funds from card to progfile
    /// </summary>
    /// <param name="endPoint">Tutuka endpoint</param>
    /// <param name="key">Tutuka password</param>
    /// <param name="nuCardProfileNumber">Tutuka profile</param>
    /// <param name="terminalId">Tutuka terminal</param>
    /// <param name="nuCardNumber">Full card</param>
    /// <param name="amount">Amount to transfer</param>
    /// <param name="errorMessage">Any errors</param>
    /// <returns></returns>
    private static bool TransferCardFundsToProfile(string endPoint, string key, string nuCardProfileNumber, string terminalId, string nuCardNumber,
      Decimal amount, out string errorMessage)
    {
      bool result = false;

      try
      {
        var xmlRequest = new DeductCardLoadProfile_Input()
        {
          cardNumber = nuCardNumber,
          profileNumber = nuCardProfileNumber,
          terminalID = terminalId,
          transactionDate = DateTime.Now,
          transactionID = Guid.NewGuid().ToString(),
          requestAmount = (int)amount * 100
        };
        string xmlSent;
        string xmlRecv;
        var startDt = DateTime.Now;
        var xmlResult = NuCardXMLRPCUtils.DeductCardLoadProfile(endPoint, key, xmlRequest, out xmlSent, out xmlRecv, out errorMessage);
        var endDt = DateTime.Now;

        NuCardDb.LogAdminRequest(JsonConvert.SerializeObject(new SourceRequest() { AppName = "Atlas Server" }),
          null, startDt, endDt, Enumerators.NuCard.AdminRequestType.TransferCardFunds, xmlResult.resultCode,  (Decimal)xmlResult.requestAmount / 100M,
          nuCardNumber, null, xmlSent, xmlRecv, xmlResult.clientTransactionID, xmlResult.serverTransactionID, errorMessage);

        if (xmlResult.resultCode.HasValue && xmlResult.resultCode == (int)Enumerators.NuCard.AdminRequestResult.Successful)
        {
          result = true;
        }
        else
        {
          if (string.IsNullOrEmpty(errorMessage))
          {
            errorMessage = NuCardXMLRPCUtils.GetNuCardErrorString(xmlResult.resultCode, xmlResult.resultText);
          }
        }
      }
      catch (Exception err)
      {
        _log.Error("CardToProfile: {0}", err);
        errorMessage = err.Message;
      }

      return result;
    }


    #region Logging

    private static readonly ILogger _log = Log.Logger.ForContext<NuCardCheckCards>();

    #endregion
    
  }
}
