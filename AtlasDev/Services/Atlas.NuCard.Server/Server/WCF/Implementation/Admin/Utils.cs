/* -----------------------------------------------------------------------------------------------------------------
*  Copyright (C) 2012-2015 Atlas Finance (Pty) Ltd.
* 
* 
*  Description:
*  ------------------
*    Utils for Admin
* 
* 
*  Author:
*  ------------------
*     Keith Blows
* 
* 
*  Revision history: 
*  ------------------ 
* 
* 
*         
* ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.Linq;

using Serilog;
using DevExpress.Xpo;

using Atlas.Domain.DTO;

using Atlas.Domain.Model;
using Atlas.NuCard.Repository;
using Atlas.NuCard.WCF.Interface;


namespace AtlasServer.WCF.Admin.Implementation
{
  public static class Utils
  {
    /// <summary>
    /// Logs a bad request
    /// </summary>
    /// <param name="request">Source request parameters</param>
    /// <param name="errorText">NuCard XML-RPC method called</param>
    public static void LogBadRequest(string methodName, string sourceRequestString, COR_AppUsageDTO appUsage, DateTime startDT,
      Atlas.Enumerators.NuCard.AdminRequestType requestType, Exception error, string xmlSent = null, string xmlRecv = null, 
      SourceRequest request = null)
    {
      Log.Error(error, "{MethodName}- Type: {RequestType}, {@Request}, Sent: {XMLSent}, Recv: {XMLRecv}", 
        methodName, requestType, request, xmlSent, xmlRecv);

      NuCardDb.LogAdminRequest(requestParams: sourceRequestString, appUsage: appUsage, startDT: startDT,
        endDT: DateTime.Now, requestType: requestType, requestResult: -1, amount: null,
        xmlRequest: xmlSent, xmlResponse: xmlRecv,
        sourceCardNum: null, destCardNum: null,
        clientTransactionID: null, serverTransactionID: null, resultText: error.Message);
    }



    /// <summary>
    /// Determine if this has already successfully completed- NUC_Transaction is written on 
    /// successful load/transfer.
    /// TODO: move into the domain repository
    /// </summary>
    /// <param name="transactionId">The source transaction id</param>
    /// <returns>true if this transactionId has not been completed successfully</returns>
    public static bool TransactionIdIsUnique(string transactionId)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        return !unitOfWork.Query<NUC_Transaction>().Any(s => s.ReferenceNum == transactionId);
      }
    }

    /// <summary>
    /// Determine if this has already successfully completed- NUC_Transaction is written on 
    /// successful load/transfer.
    /// TODO: move into the domain repository
    /// </summary>
    /// <param name="unitOfWork">XPO unit of work to use</param>
    /// <param name="transactionId">The source transaction id</param>
    /// <returns>true if this transactionId has not been completed successfully</returns>
    public static bool TransactionIdIsUnique(UnitOfWork unitOfWork, string transactionId)
    {
      return !unitOfWork.Query<NUC_Transaction>().Any(s => s.ReferenceNum == transactionId);      
    }


    /// <summary>
    /// Determines if loads exceeds daily limit- excludes transfers to this card
    /// </summary>
    /// <param name="cardNumber">The NuCard card number</param>
    /// <param name="legacyBranchNum">The ASS legacy branch number</param>
    /// <returns>true if this load will exceed the maximum</returns>
    public static bool ThisLoadExceedsMaximum(decimal loadRequestAmount, string cardNumber, string legacyBranchNum)
    {
      // Problem- load funds from existing card??       
      var loadRequestTotal = 0M;
      using (var unitOfWork = new UnitOfWork())
      {
        // Get nett total of loads done today
        var today = DateTime.Today;
        var totalLoads = unitOfWork.Query<NUC_Transaction>().Where(s => 
          s.NuCard.CardNum == cardNumber &&
          !s.IsPending &&
          /* && s.Amount > 0 */                                // No- get the nett amount!
          s.LoadDT >= today &&                                 // Only done today
          !s.Description.StartsWith("Transfer between cards")) // Do not include transfers between cards. TODO: We need a better way to flag the NuCard trans type/reason!
            .Sum(s => s.Amount);
        loadRequestTotal = loadRequestAmount + totalLoads ?? 0;        
      }

      // Determine maximum-only 3 branches may issue 16000 loans, but we are not allowed to load more than 10K per day without FICA...
      decimal maxDailyAmount = (legacyBranchNum == "004" || legacyBranchNum == "002" || legacyBranchNum == "001") ? 10000M : 8000M;
      return loadRequestTotal > maxDailyAmount;
    }


    /// <summary>
    /// Adds an entry to the NUC_Transaction table for logging loads/deducts
    /// </summary>
    /// <param name="nuCardId">The NUC_Nucard.NuCardId the transaction relates to</param>
    /// <param name="serverTransactionId">The Tutuka server transaction id</param>
    /// <param name="description">A description of the transaction</param>
    /// <param name="referenceNum">The reference number of the transaction</param>
    /// <param name="amount">The amount- a load is positive</param>
    /// <param name="loadDate">Date the load was actioned</param>
    /// <param name="isPending">Is this transaction pending</param>
    /// <param name="transactionSource"></param>
    /// <param name="createdByPersonId">The person who initiated the transaction- PER_Person.PersonId</param>
    /// <param name="createdDT">Date of transaction</param>
    /// TODOL: Update the domain repository
    public static void LogTransaction(Int64 nuCardId, string serverTransactionId, string description,
      string referenceNum, Decimal amount, DateTime loadDate, bool isPending,
      Atlas.Enumerators.General.ApplicationIdentifiers source,
      Atlas.Enumerators.NuCard.TransactionSourceType transactionSource,
      Int64 createdByPersonId, DateTime createdDT)
    {
      using (var unitOfWork = new UnitOfWork())
      {
        new NUC_Transaction(unitOfWork)
          {
            NuCard = unitOfWork.Query<NUC_NuCard>().First(s => s.NuCardId == nuCardId),
            Amount = amount,
            CreatedBy = unitOfWork.Query<PER_Person>().FirstOrDefault(s => s.PersonId == createdByPersonId),
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
