/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012-2013 Atlas Finance (Pty() Ltd.
 * 
 * 
 *  Description:
 *  ------------------
 *     NuCard stock database support routines- WCF implementation
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2013        - Created
 *     
 *     2013-11-14  - Code clean-up simplification 
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

using DevExpress.Xpo;
using Serilog;
using Newtonsoft.Json;

using Atlas.Domain.Model;
using Atlas.Domain.DTO;
using Atlas.Common.Utils;
using Atlas.Common.Extensions;
using Atlas;
using Atlas.Enumerators;
using Atlas.Domain.Security;
using Atlas.NuCard.WCF.Interface;
using Atlas.WCF.Implementation;
using AtlasServer.WCF.Stock.Implementation;

#endregion


namespace AtlasServer.WCF.Implementation
{
  /// <summary>
  /// Implementation of INuCardStockServer- handles purely NuCard stock related items
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class NuCardStockServer : INuCardStockServer
  {
    /// <summary>
    /// Gets the status (in stock, missing, etc.) of a particular NuCard (see General.NuCardStatus)
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="card">Card details- either cardnumber or Tracking number mnust be provided</param>
    /// <param name="cardStatus">Returns the status of the card, 0- missing, else General.NuCardStatus</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>
    /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
    public int GetCardStatus(SourceRequest sourceRequest, NuCardStockItem card, out int cardStatus, out string errorMessage)
    {
      return GetCardStatus_Impl.GetCardStatus(sourceRequest, card, out cardStatus, out errorMessage);      
    }


    /// <summary>
    /// Moves these 'unknown' cards into the branch's stock (marks as 'In Stock' and links the card to this branch).
    /// NOTE: This function should be removed once a proper stock system is running. This is a temporary fix to get
    /// 'unknown' cards into the system from ASS.
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cards">Card details- *BOTH* full Card number & Tracking number *must* be provided</param>
    /// <param name="errorMessage">Any error message to display to the end-user (out)</param>    
    /// <param name="csardsImported"></param>
    /// <returns>General.WCFCallResult enum (1- success, 0- no operation, etc.)</returns>
    public int BranchImportUnknownCards(SourceRequest sourceRequest, List<NuCardStockItem> cards,
      out List<NuCardStockItem> cardsImported, out string errorMessage)
    {
      return BranchImportUnknownCards_Impl.BranchImportUnknownCards(sourceRequest, cards, out cardsImported, out errorMessage);      
    }


    /// <summary>
    /// Returns stock currently 'In transit' for this branch
    /// </summary>
    /// <param name="sourceRequest">Source request parameters</param>
    /// <param name="cards">Cards found (out)</param>
    /// <returns>General.WCFCallResult  enum (1- success, 0- no operation, etc.)</returns>
    public int GetCardsInTransitForBranch(SourceRequest sourceRequest, out List<NuCardStockItem> cards,
      out string errorMessage)
    {
      return GetCardsInTransitForBranch_Impl.GetCardsInTransitForBranch(sourceRequest, out cards, out errorMessage);      
    }


    /// <summary>
    /// Updates status of cards from 'In Transit' to as being in this branch's stock
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="cards"></param>
    /// <param name="cardsImported"></param>
    /// <returns>General.WCFCallResult </returns>
    public int BranchAcceptInTransitCards(SourceRequest sourceRequest, List<NuCardStockItem> cards,
      out List<NuCardStockItem> cardsImported, out string errorMessage)
    {
      return BranchAcceptInTransitCards_Impl.BranchAcceptInTransitCards(sourceRequest, cards, out cardsImported, out errorMessage);      
    }


    /// <summary>
    /// Marks cards as being transferred to another branch and makes the status as 'In Transit'
    /// </summary>
    /// <param name="sourceRequest"></param>
    /// <param name="cards"></param>
    /// <param name="destinationBranch"></param>
    /// <param name="batchDetails">Details pertaining to sending the batch</param>
    /// <returns>General.WCFCallResult </returns>
    public int SendCardsToBranch(SourceRequest sourceRequest, NuCardBatchToDispatch batchDetails,
      out List<NuCardStockItem> cardsTransferred,
      out string errorMessage)
    {
      return SendCardsToBranch_Impl.SendCardsToBranch(sourceRequest, batchDetails, out cardsTransferred, out errorMessage);
    }


    #region Private methods

    #endregion


  }
}
