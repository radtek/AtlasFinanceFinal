/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Atlas NuCard stock handling
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-08-20- Skeleton created
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Runtime.Serialization;

#endregion


namespace Atlas.NuCard.WCF.Interface
{
  [ServiceContract(Namespace = "Atlas.ThirdParty.Altech.NuCard")]
  public interface INuCardStockServer
  {
    /// <summary>
    /// Gets the status (in stock, missing, etc.) of a particular card
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="card">Card details- either cardnumber or Tracking number mnust be provided</param>
    /// <param name="cardStatus">Returns the status of the card, 0- missing, else General.NuCardStatus</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>Calls_Lookups enum (1- success, 0- no operation, etc.)</returns>
    [OperationContract]
    int GetCardStatus(SourceRequest sourceRequest, NuCardStockItem card,
      out int cardStatus, out string errorMessage);


    /// <summary>
    /// Moves these 'unknown' cards into the branch's stock (marks as 'In Stock' and links the card to this branch).
    /// NOTE: This function should be removed once a proper stock system is running. This is a temporary fix to get
    /// 'unknown' cards into the system
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cards">Card details- *BOTH* full Card number & Tracking number *must* be provided</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>    
    /// <param name="csardsImported"></param>
    /// <returns>Calls_Lookups enum (1- success, 0- no operation, etc.)</returns>
    [OperationContract]
    int BranchImportUnknownCards(SourceRequest sourceRequest, List<NuCardStockItem> cards,
      out List<NuCardStockItem> cardsImported, out string errorMessage);


    /// <summary>
    /// Returns stock current In transit for this branch
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cards">Cards found (out)</param>
    /// <returns>Calls_Lookups enum (1- success, 0- no operation, etc.)</returns>
    [OperationContract]
    int GetCardsInTransitForBranch(SourceRequest sourceRequest,
      out List<NuCardStockItem> cards, out string errorMessage);


    /// <summary>
    /// Updates status of cards from 'In Transit' to as being in this branch's stock
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="cards"></param>
    /// <param name="cardsImported"></param>
    /// <returns></returns>
    [OperationContract]
    int BranchAcceptInTransitCards(SourceRequest sourceRequest, List<NuCardStockItem> cards,
      out List<NuCardStockItem> cardsImported, out string errorMessage);


    /// <summary>
    /// Marks cards as being transferred to another branch
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="cards"></param>
    /// <param name="destinationBranch"></param>
    /// <param name="batchDetails">Details pertaining to sending the batch</param>
    /// <returns></returns>
    [OperationContract]
    int SendCardsToBranch(SourceRequest sourceRequest, NuCardBatchToDispatch batchDetails,
      out List<NuCardStockItem> cardsTransferred,
      out string errorMessage);
  }
}
