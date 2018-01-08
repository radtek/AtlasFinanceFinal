#region Using

using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;

using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.Domain.Security;
using Atlas.Common.Extensions;
using System.Text;
using Atlas.Domain.DTO.Nucard;

#endregion


namespace Atlas.Data.Repository
{
  public static class NuCardData
  {
    /// <summary>
    /// Attempts to find a card in the database matching any one of the given parameters
    /// </summary>
    /// <param name="nuCardId">The NuCard.NuCardId field value (PK) to search with</param>
    /// <param name="trackingNumber">The NuCard.TrackingNum field value to search with</param>
    /// <param name="cardNo">The NuCard.CardNum field value to search with</param>
    /// <returns>The found NuCard DTO, else null</returns>
    public static NUC_NuCardDTO FindCard(Int64 nuCardId, string trackingNumber, string cardNo)
    {
      NUC_NuCardDTO result = null;

      #region Try find card with any of the given parameters
      using (var unitOfWork = new UnitOfWork())
      {
        NUC_NuCard cardInDB = null;

        if (nuCardId > 0)
        {
          cardInDB = unitOfWork.Query<NUC_NuCard>().FirstOrDefault(s => s.NuCardId == nuCardId);
        }

        if (cardInDB == null && !string.IsNullOrEmpty(cardNo))
        {
          cardInDB = unitOfWork.Query<NUC_NuCard>().FirstOrDefault(s => s.CardNum == cardNo);
        }

        if (cardInDB == null && !string.IsNullOrEmpty(trackingNumber) && trackingNumber.Length > 5)
        {
          cardInDB = unitOfWork.Query<NUC_NuCard>().FirstOrDefault(s => s.TrackingNum == trackingNumber);
        }

        if (cardInDB != null)
        {
          result = AutoMapper.Mapper.Map<NUC_NuCard, NUC_NuCardDTO>(cardInDB);
        }
      }
      #endregion

      return result;
    }


    /// <summary>
    /// Returns card DTO from the database, for the given card number
    /// </summary>
    /// <param name="cardNo">Full card number</param>
    /// <returns>The NuCardDTO found, else null</returns>
    public static NUC_NuCardDTO FindCard(string cardNo)
    {
      NUC_NuCardDTO result = null;
      using (var unitOfWork = new UnitOfWork())
      {
        var card = unitOfWork.Query<NUC_NuCard>().FirstOrDefault(s => s.CardNum == cardNo);
        if (card != null)
        {
          result = AutoMapper.Mapper.Map<NUC_NuCard, NUC_NuCardDTO>(card);
        }
      }

      return result;
    }


    /// <summary>
    /// Get listing of NuCard stopped reasons
    /// </summary>
    /// <returns></returns>
    public static List<NUC_NuCardStatus> GetStoppedReasons()
    {
      using (var UoW = new UnitOfWork())
      {
        return new XPQuery<NUC_NuCardStatus>(UoW).Where(o => o.Type == Enumerators.NuCard.NuCardStatus.Stopped_Lost ||
                                                         o.Type == Enumerators.NuCard.NuCardStatus.Stopped_Stolen ||
                                                         o.Type == Enumerators.NuCard.NuCardStatus.Stopped_OutcomeQuery ||
                                                         o.Type == Enumerators.NuCard.NuCardStatus.Stopped_ConsolidateToSingle ||
                                                         o.Type == Enumerators.NuCard.NuCardStatus.Stopped_NoLongerActive ||
                                                         o.Type == Enumerators.NuCard.NuCardStatus.Stopped_PIN_Exceeded ||
                                                         o.Type == Enumerators.NuCard.NuCardStatus.Suspect_Fraud ||
                                                         o.Type == Enumerators.NuCard.NuCardStatus.Emergency_Replacement).ToList();

      }
    }


    /// <summary>
    /// Updates a NuCard's card status (and optionally links to a branch)
    /// </summary>
    /// <param name="cardNo">Full card number</param>
    /// <param name="branchCode">Legacy branch code to link card to (optional)</param>
    /// <param name="status">New card status </param>
    public static void UpdateCardStatus(string cardNo, string profileNumber,
      string branchCode, NuCard.NuCardStatus status, PER_SecurityDTO user)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        var card = unitOfWork.Query<NUC_NuCard>().First(s => s.CardNum == cardNo);
        var dbStatus = unitOfWork.Query<NUC_NuCardStatus>().First(s => s.NuCardStatusId == (Int64)status);
        var userPerson = unitOfWork.Query<PER_Person>().First(s => s.PersonId == user.Person.PersonId);

        if (!string.IsNullOrEmpty(profileNumber))
        {
          card.NuCardProfile = unitOfWork.Query<NUC_NuCardProfile>().FirstOrDefault(s => s.ProfileNum == profileNumber);
        }

        if (status == NuCard.NuCardStatus.Active)
        {
          card.IssueDT = DateTime.Now;
          if (!string.IsNullOrEmpty(branchCode))
          {
            card.IssuedByBranch = unitOfWork.Query<BRN_Branch>().First(s => s.LegacyBranchNum.PadLeft(3, '0') == branchCode.PadLeft(3, '0'));
          }
        }

        card.Status = dbStatus;
        card.LastEditedDT = DateTime.Now;
        card.LastEditedBy = userPerson;

        unitOfWork.CommitChanges();
      }
    }


    /// <summary>
    /// Returns the first currently active card for the specified branch
    /// </summary>
    /// <param name="legacyBranchCode"></param>
    /// <returns>NuCard DTO, null if could not find an actice card</returns>
    public static NUC_NuCardDTO GetActiveCardForBranch(string legacyBranchCode)
    {
      NUC_NuCardDTO result = null;
      using (var unitOfWork = new UnitOfWork())
      {
        var branch = unitOfWork.Query<BRN_Branch>().First(s => s.LegacyBranchNum.PadLeft(3, '0') == legacyBranchCode.PadLeft(3, '0'));
        var cardActive = unitOfWork.Query<NUC_NuCardStatus>().Where(s =>
          s.Type == NuCard.NuCardStatus.Active ||
          s.Type == NuCard.NuCardStatus.USE).ToList();

        // Find card where card is active and belongs to this branch
        var foundCard = unitOfWork.Query<NUC_NuCard>().FirstOrDefault(s => cardActive.Contains(s.Status) &&
          s.ExpiryDT > DateTime.Now.AddMonths(3) && s.NuCardProfile == branch.DefaultNuCardProfile && s.AllocatedPerson != null);

        if (foundCard != null)
        {
          result = AutoMapper.Mapper.Map<NUC_NuCard, NUC_NuCardDTO>(foundCard);
        }
      }

      return result;
    }


    /// <summary>
    /// Logs a NuCard admin (API-based) request
    /// </summary>
    /// <param name="sourceRequest">Source request parameters- Json</param>
    /// <param name="startDT">Request start date/time</param>
    /// <param name="endDT">Request end date/time</param>
    /// <param name="requestType">The request type</param>
    /// <param name="requestResult">Result of request</param>
    /// <param name="amount">Amount</param>
    /// <param name="sourceCardNum">Source NuCard full card number</param>
    /// <param name="destCardNum">Destination NuCard full card number</param>
    /// <param name="xmlRequest">XML-RPC request</param>
    /// <param name="xmlResponse">XML-RPC response</param>
    /// <param name="clientTransactionID">Client transaction ID</param>
    /// <param name="serverTransactionID">Server transaction ID</param>
    /// <param name="resultText">Result message</param>
    public static void LogAdminRequest(string requestParams, COR_AppUsageDTO appUsage,
      DateTime startDT, DateTime endDT, NuCard.AdminRequestType requestType, int? requestResult, Decimal? amount,
      string sourceCardNum, string destCardNum,
      string xmlRequest, string xmlResponse,
      string clientTransactionID, string serverTransactionID, string resultText)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        var sourceCard = (!string.IsNullOrEmpty(sourceCardNum)) ? unitOfWork.Query<NUC_NuCard>().FirstOrDefault(s => s.CardNum == sourceCardNum) : null;
        var destCard = (!string.IsNullOrEmpty(destCardNum)) ? unitOfWork.Query<NUC_NuCard>().FirstOrDefault(s => s.CardNum == destCardNum) : null;

        var log = new NUC_LogAdminEvent(unitOfWork)
        {
          EventDT = DateTime.Now,
          Application = appUsage != null ? unitOfWork.Query<COR_AppUsage>().FirstOrDefault(s => s.AppUsageId == appUsage.AppUsageId) : null,

          NuCardRequestType = requestType,

          RequestResult = requestResult.HasValue ? (NuCard.AdminRequestResult)requestResult : NuCard.AdminRequestResult.NoOperation,
          Amount = amount,

          MainCard = sourceCard,
          SecondaryCard = destCard,

          AdditionalInfo = requestParams,

          ClientTransactionID = clientTransactionID,
          ServerTransactionID = serverTransactionID,

          ResultText = resultText,

          CreatedDT = DateTime.Now,

          XMLSent = xmlRequest,
          XMLReceived = xmlResponse,
        };

        unitOfWork.CommitChanges();
      }
    }


    /// <summary>
    /// Logs a NuCard request
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="startDT">Request start date/time</param>
    /// <param name="endDT">Request end date/time</param>
    /// <param name="requestType">The request type</param>
    /// <param name="requestResult">Result of request</param>
    /// <param name="amount">Amount</param>
    /// <param name="sourceRequestParams">Request parameters</param>
    /// <param name="sourceCardNum">Source NuCard full card number</param>
    /// <param name="destCardNum">Destination NuCard full card number</param>
    /// <param name="xmlRequest">XML-RPC request</param>
    /// <param name="xmlResponse">XML-RPC response</param>
    /// <param name="clientTransactionID">Client transaction ID</param>
    /// <param name="serverTransactionID">Server transaction ID</param>
    /// <param name="resultText">Result message</param>
    public static void LogStockRequest(string requestParams, COR_AppUsageDTO appUsage,
      DateTime startDT, string cardNum,
      NuCard.StockRequestType requestType, NuCard.StockRequestResult requestResult, string resultText)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        NUC_NuCard card = (string.IsNullOrEmpty(cardNum)) ? null : unitOfWork.Query<NUC_NuCard>().FirstOrDefault(s => s.CardNum == cardNum);
        var unknownCardNum = (card == null) ? cardNum : null;

        var log = new NUC_LogStockEvent(unitOfWork)
        {
          EventDT = startDT,
          Application = appUsage != null ? unitOfWork.Query<COR_AppUsage>().FirstOrDefault(s => s.AppUsageId == appUsage.AppUsageId) : null,
          SourceRequestParameters = requestParams,

          NuCardStockRequestType = requestType,
          RequestResult = requestResult,

          Card = card,
          UnknownCard = unknownCardNum,

          ResultText = resultText,

          CreatedDT = DateTime.Now,
        };
        unitOfWork.CommitChanges();
      }
    }


    /// <summary>
    /// Allocates a card to a person
    /// </summary>
    /// <param name="nuCardId">The NuCard.NuCardId</param>
    /// <param name="personId">The Person.PersonId</param>
    public static void AllocateCardToPerson(Int64 nuCardId, Int64 personId, PER_SecurityDTO user, BRN_BranchDTO branch)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        var card = unitOfWork.Query<NUC_NuCard>().First(s => s.NuCardId == nuCardId);
        var person = unitOfWork.Query<PER_Person>().First(s => s.PersonId == personId);
        var userDb = unitOfWork.Query<PER_Person>().First(s => s.PersonId == user.Person.PersonId);
        var branchDb = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == branch.BranchId);
        var cardActiveStatus = unitOfWork.Query<NUC_NuCardStatus>().First(s => s.NuCardStatusId == (Int64)NuCard.NuCardStatus.Active);
        
        card.AllocatedPerson = person;
        card.Status = cardActiveStatus;
        card.IssueDT = DateTime.Now;
        card.IssuedByBranch = branchDb;
        card.LastEditedDT = DateTime.Now;
        card.LastEditedBy = userDb;

        unitOfWork.CommitChanges();
      }
    }


    /// <summary>
    /// Gets Branch default NuCard profile DTO
    /// </summary>
    /// <param name="legacyBranchNum">Legacy branch code</param>
    /// <returns>The NuCard profile DTO</returns>
    public static NUC_NuCardProfileDTO GetBranchProfile(string legacyBranchNum)
    {
      NUC_NuCardProfileDTO result = null;

      using (var unitOfWork = new UnitOfWork())
      {
        var branch = unitOfWork.Query<BRN_Branch>().FirstOrDefault(s => s.LegacyBranchNum.PadLeft(3, '0') == legacyBranchNum.PadLeft(3, '0'));
        if (branch != null && branch.DefaultNuCardProfile != null)
        {
          result = AutoMapper.Mapper.Map<NUC_NuCardProfile, NUC_NuCardProfileDTO>(branch.DefaultNuCardProfile);
        }
      }

      return result;
    }


    /// <summary>
    /// Imports a card in the NuCard stock table, for a branch
    /// </summary>
    /// <param name="branchCode">Atlas branch</param>    
    /// <param name="user">The software user</param>
    /// <returns>The NuCardDTO imported, else null</returns>
    public static NUC_NuCardDTO ImportCardIntoStockForBranch(BRN_BranchDTO branch, PER_SecurityDTO user,
      Int64 nuCardId, string cardNo, string sequenceNo, string trackingNo, out string errorMessage)
    {
      NUC_NuCardDTO result = null;
      errorMessage = string.Empty;

      using (var unitOfWork = new UnitOfWork())
      {
        var inStockDb = unitOfWork.Query<NUC_NuCardStatus>().First(s => s.NuCardStatusId == (int)NuCard.NuCardStatus.InStock);
        var userInDb = unitOfWork.Query<PER_Person>().First(s => s.PersonId == user.Person.PersonId);
        var branchInDb = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == branch.BranchId);

        NUC_NuCard newCard;
        var newCardDTO = NuCardData.FindCard(nuCardId, trackingNo, cardNo);
        if (newCardDTO == null)
        {
          newCard = new NUC_NuCard(unitOfWork)
          {
            CardNum = cardNo,
            TrackingNum = !string.IsNullOrEmpty(trackingNo) ? trackingNo : cardNo,
            SequenceNum = sequenceNo,

            Status = unitOfWork.Query<NUC_NuCardStatus>().First(s => s.Type == NuCard.NuCardStatus.InStock),

            // TODO: How best to determine this? Best guess- perform NuCard balance request...?
            ExpiryDT = DateTime.Now.AddYears(3).AddMonths(-3),

            CreatedDT = DateTime.Now,
            CreatedBy = userInDb,
          };
        }
        else
        {
          newCard = unitOfWork.Query<NUC_NuCard>().First(s => s.NuCardId == newCardDTO.NuCardId);

          // Ensure card is in state we can use
          var okStatus = unitOfWork.Query<NUC_NuCardStatus>().Where(s =>
            s.NuCardStatusId == (int)NuCard.NuCardStatus.InTransit ||
            s.NuCardStatusId == (int)NuCard.NuCardStatus.InStock);

          if (!okStatus.Contains(newCard.Status))
          {
            errorMessage = "Card is currently " +
              ((NuCard.NuCardStatus)newCard.Status.NuCardStatusId).ToStringEnum();
            newCard = null;
          }
        }

        if (newCard != null)
        {
          newCard.LastEditedBy = userInDb;
          newCard.LastEditedDT = DateTime.Now;
          // Assume card is not in profile, NuCard admin will attempt Link and ignore 'already linked' errors...
          /*if (newCard.NuCardProfile == null)
          {
            newCard.NuCardProfile = branchInDb.DefaultNuCardProfile;
          }*/

          unitOfWork.CommitChanges();

          result = AutoMapper.Mapper.Map<NUC_NuCard, NUC_NuCardDTO>(newCard);

          return result;
        }
        else
        {
          return null;
        }
      }

    }


    /// <summary>
    /// Gets all cards in transit to a destination branch
    /// </summary>
    /// <param name="branchCode">Legacy branch code</param>
    /// <returns>List of NuCards destined for the branch</returns>
    public static List<NUC_NuCardDTO> GetCardsInTransitForBranch(BRN_BranchDTO branch, out string errorMessage)
    {
      var result = new List<NUC_NuCardDTO>();
      errorMessage = string.Empty;

      using (var unitOfWork = new UnitOfWork())
      {
        var branchInDb = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == branch.BranchId);
        var batchListInDb = unitOfWork.Query<NUC_NuCardBatch>().Where(s => s.DeliverToBranch == branchInDb &&
          (s.Status == General.NucardBatchStatus.In_Transit ||
            s.Status == General.NucardBatchStatus.Lost_In_Transit));

        foreach (var batchInDb in batchListInDb)
        {
          foreach (NUC_NuCardBatchCard cardBatchCard in batchInDb.NucardBatchCards)
          {
            result.Add(AutoMapper.Mapper.Map<NUC_NuCard, NUC_NuCardDTO>(cardBatchCard.NuCard));
          }
        }
      }

      return result;
    }


    /// <summary>
    /// Moves a card in the branch's stock and sets to 'in-stock'
    /// </summary>
    /// <param name="card"></param>
    /// <returns>Card transferred, null if nothing done</returns>
    public static NUC_NuCardDTO MoveCardIntoBranchStock(BRN_BranchDTO branch,
      PER_SecurityDTO user, Int64 nuCardId, string cardNo, string sequenceNo, string trackingNo, out string errorMessage)
    {
      errorMessage = string.Empty;
      NUC_NuCardDTO result = null;
      var foundCard = NuCardData.FindCard(nuCardId, trackingNo, cardNo);
      if (foundCard != null)
      {
        using (var unitOfWork = new UnitOfWork())
        {
          var cardInDb = unitOfWork.Query<NUC_NuCard>().First(s => s.NuCardId == foundCard.NuCardId);
          var userInDb = unitOfWork.Query<PER_Person>().First(s => s.PersonId == user.Person.PersonId);
          var branchInDb = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == branch.BranchId);

          cardInDb.Status = (cardInDb.NuCardProfile == null) ?
            unitOfWork.Query<NUC_NuCardStatus>().First(s => s.NuCardStatusId == (int)NuCard.NuCardStatus.InStock) :
            unitOfWork.Query<NUC_NuCardStatus>().First(s => s.NuCardStatusId == (int)NuCard.NuCardStatus.Linked);

          cardInDb.LastEditedBy = userInDb;
          cardInDb.LastEditedDT = DateTime.Now;

          unitOfWork.CommitChanges();

          result = AutoMapper.Mapper.Map<NUC_NuCardDTO>(cardInDb);
        }
      }
      else
      {
        errorMessage = "Card could not be located";
      }

      return result;
    }


    /// <summary>
    /// Tries to close off any orphaned, yet completed NuCard transfer batches
    /// </summary>
    /// <param param name="branch">Source branch</param>
    /// <param name="user">User performing the action</param>
    public static bool TryCloseInTransitBatchForBranch(BRN_BranchDTO branch, PER_SecurityDTO user,
      out string errorMessage)
    {
      errorMessage = string.Empty;
      using (var unitOfWork = new UnitOfWork())
      {
        var branchDb = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == branch.BranchId);
        var userInDb = unitOfWork.Query<PER_Person>().First(s => s.PersonId == user.Person.PersonId);
        var inTransitDb = unitOfWork.Query<NUC_NuCardStatus>().First(s => s.NuCardStatusId == (int)NuCard.NuCardStatus.InTransit);

        var batchesDb = unitOfWork.Query<NUC_NuCardBatch>().Where(s => s.DeliverToBranch == branchDb &&
          s.Status == General.NucardBatchStatus.In_Transit);

        foreach (var batchDb in batchesDb)
        {
          var inTransitCount = batchDb.NucardBatchCards.Count(s => s.NuCard.Status == inTransitDb);
          #region If no cards in transit for this batch- batch has been successfully completed
          if (inTransitCount == 0)
          {
            batchDb.Status = General.NucardBatchStatus.Delivered;
            batchDb.ReceivedBy = userInDb;
            batchDb.ReceivedByBranch = branchDb;
            batchDb.QuantityReceived = batchDb.QuantitySent;
            batchDb.DeliveryDT = DateTime.Now;
            batchDb.ReceivedDT = DateTime.Now;
            batchDb.LastEditedBy = userInDb;
            batchDb.LastEditedDT = DateTime.Now;
          }
          #endregion
        }
        unitOfWork.CommitChanges();

        return true;
      }
    }


    /// <summary>
    /// Create a NuCard batch destined for another branch
    /// </summary>
    /// <param name="batchDetails">Batch information supplied by the client software</param>
    /// <param name="cardsToTransfer">List of cards supplied by the client software</param>
    /// <param name="user">User performing the transaction</param>
    /// <returns>List of cards transferred</returns>
    public static List<NUC_NuCardDTO> SendCardsToBranch(PER_SecurityDTO user, BRN_BranchDTO destBranch,
      string courierOrPersonReference, string comment,
      List<NUC_NuCardDTO> cards, out string errorMessage)
    {
      errorMessage = string.Empty;
      var warnings = new StringBuilder();
      var result = new List<NUC_NuCardDTO>();

      using (var unitOfWork = new UnitOfWork())
      {
        var validStockStatus = unitOfWork.Query<NUC_NuCardStatus>().Where(s =>
          s.NuCardStatusId == (int)NuCard.NuCardStatus.InStock ||
          s.NuCardStatusId == (int)NuCard.NuCardStatus.Linked)  // TODO: Remove once new NuCard system in place
          .Select(s => s.NuCardStatusId)
          .ToList();

        #region Find cards and ensure they are in stock and not part of another batch...
        foreach (var nuCard in cards)
        {
          var findCard = NuCardData.FindCard(nuCard.NuCardId, nuCard.TrackingNum, nuCard.CardNum);

          // TODO: Check that card is in source branch stock!!
          if (findCard != null && validStockStatus.Contains((int)findCard.Status.NuCardStatusId))
          {
            result.Add(AutoMapper.Mapper.Map<NUC_NuCardDTO>(findCard));
          }
          else
          {
            if (findCard == null)
            {
              warnings.AppendFormat("Card '{0}'- not found, ", nuCard.CardNum);
            }
            else
            {
              warnings.AppendFormat("Card '{0}'- status is in invalid status (cannot dispatch): {1}, ",
                nuCard.CardNum, findCard.Status.Description);
            }
          }
        }
        #endregion

        if (result.Count > 0)
        {
          var userInDb = unitOfWork.Query<PER_Person>().First(s => s.PersonId == user.Person.PersonId);
          var destBranchDb = unitOfWork.Query<BRN_Branch>().First(s => s.LegacyBranchNum.PadLeft(3, '0') == destBranch.LegacyBranchNum.PadLeft(3, '0'));
          var nuCardBatchDb = new NUC_NuCardBatch(unitOfWork)
          {
            CreatedBy = userInDb,
            CreatedDT = DateTime.Now,
            DeliverToBranch = unitOfWork.Query<BRN_Branch>().FirstOrDefault(s => s.BranchId == destBranch.BranchId),
            QuantitySent = result.Count,
            SentBy = userInDb,
            SentDT = DateTime.Now,
            Status = General.NucardBatchStatus.In_Transit,
            TrackingNum = courierOrPersonReference,
            Comment = comment
          };

          var inTransitStatus = unitOfWork.Query<NUC_NuCardStatus>().First(s => s.NuCardStatusId == (int)NuCard.NuCardStatus.InTransit);
          foreach (var nuCard in result)
          {
            var cardInDb = unitOfWork.Query<NUC_NuCard>().First(s => s.NuCardId == nuCard.NuCardId);
            var nuCardBatchCardDb = new NUC_NuCardBatchCard(unitOfWork)
            {
              NuCard = cardInDb,
              NuCardBatch = nuCardBatchDb
            };

            cardInDb.Status = inTransitStatus;
            cardInDb.LastEditedBy = userInDb;
            cardInDb.LastEditedDT = DateTime.Now;
          }

          unitOfWork.CommitChanges();

          errorMessage = warnings.ToString();
        }
        else
        {
          errorMessage = string.Format("No cards were transferred- '{0}'", warnings.ToString());
        }

        return result;
      }
    }


    /// <summary>
    /// Adds an entry to the NUC_Transaction table for logging loads/deducts
    /// </summary>
    /// <param name="nuCardId">The NUC_Nucard.NuCardId the transaction relates to</param>
    /// <param name="serverTransactionId">The Tatuka server transaction id</param>
    /// <param name="description">A description of the transaction</param>
    /// <param name="referenceNum">The reference number of the transaction</param>
    /// <param name="amount">The amount- a load is positive</param>
    /// <param name="loadDate">Date the load was actioned</param>
    /// <param name="isPending">Is this transaction pending</param>
    /// <param name="transactionSource"></param>
    /// <param name="createdByPersonId">The person who initiated the transaction- PER_Person.PersonId</param>
    /// <param name="createdDT">Date of transaction</param>
    public static void LogTransaction(Int64 nuCardId, string serverTransactionId, string description, 
      string referenceNum, Decimal amount, DateTime loadDate, bool isPending, 
      Atlas.Enumerators.General.ApplicationIdentifiers source, 
      Enumerators.NuCard.TransactionSourceType transactionSource, 
      Int64 createdByPersonId, DateTime createdDT)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        var transaction = new NUC_Transaction(unitOfWork)
        {
          NuCard = unitOfWork.Query<NUC_NuCard>().First(s => s.NuCardId == nuCardId),
          Amount = amount,
          CreatedBy = unitOfWork.Query<PER_Person>().First(s => s.PersonId == createdByPersonId),
          CreatedDT = createdDT,
          Description = description,
          IsPending = isPending,
          LoadDT = loadDate,          
          ReferenceNum = referenceNum,
          ServerTransactionId = serverTransactionId,
          Source = unitOfWork.Query<TransactionSource>().First(s => s.Type == transactionSource),
          SourceApplication = source
        };

        unitOfWork.CommitChanges();
      }
    }   

  }
}
