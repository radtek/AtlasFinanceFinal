using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Business.Constants;
using Atlas.Business.NuCardAdmin;
using Atlas.Business.Security.Role;
using Atlas.Common.Extensions;
using Atlas.Domain.DTO;
using Atlas.Domain.DTO.Nucard;
using Atlas.Domain.Model;
using DevExpress.Xpo;

namespace Atlas.Business.NuCard
{
  public class Card
  {
    public string CardNum { get; set; }
  }

  public class PaymentType
  {
    public object PaymentTypeCode { get; set; }
    public string Description { get; set; }
  }

  public class StatementLine
  {
    public string TransactionDescription { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; }
  }

  public class PaymentApproval
  {
    public string AssignedTo { get; set; }
    public string NuCard { get; set; }
    public string Reference { get; set; }
    public decimal Amount { get; set; }
    public bool Paid { get; set; }
    public string ErrorMessage { get; set; }
  }

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public static class NucardHelper
  {
    internal static string TransactionType(int value)
    {
      switch (value)
      {
        case 1:
          return "Load";
        case 2:
          return "Deduction";
        case 3:
          return "Authorisation";
        default:
          return string.Empty;
      }
    }

    /// <summary>
    /// Creates a source request for WCF
    /// </summary>
    /// <param name="nucard">NuCard this transaction relates to</param>
    /// <param name="appVersion">Calling application version</param>
    /// <returns></returns>
    private static SourceRequest GetSourceRequest(NUC_NuCardDTO nucard, string appVersion)
    {
      return new SourceRequest()
      {
        BranchCode = nucard.IssuedByBranch.LegacyBranchNum,
        AppName = Enumerators.General.ApplicationIdentifiers.AtlasManagement.ToStringEnum(),
        AppVer = appVersion,
        MachineDateTime = DateTime.Now,
        MachineIPAddresses = "127.0.0.1",
        MachineName = "000-IT-10",
        MachineUniqueID = "t34t2t4324t234t23",//      RoleContext.LoggedInUser.Security.MachineStore.HardwareKey,
        UserIDOrPassport = "0000"///RoleContext.LoggedInUser.IdNum
      };
    }

    /// <summary>
    /// Create a transaction
    /// </summary>
    /// <param name="nucard">NuCard to create the transaction for</param>
    /// <param name="person">Person the transaction relates to.</param>
    /// <param name="amount">Amount for the transaction</param>
    /// <param name="errorMessage">Error message</param>
    /// <returns></returns>
    public static NUC_TransactionDTO CreateTransaction(NUC_NuCardDTO nucard, string branchId, Enumerators.General.PaymentType paymentType, decimal amount, out string errorMessage)
    {
      errorMessage = string.Empty;

      if (nucard == null)
        throw new Exception(string.Format(Messages.OBJECT_CANNOT_BE_NULL, "NuCard"));

      NUC_TransactionDTO transactionDTO = null;

      string referenceTemplate = string.Empty;

      if (paymentType == Enumerators.General.PaymentType.COM)
        referenceTemplate = "COM-{0}-{1}";
      else if (paymentType == Enumerators.General.PaymentType.STD)
        referenceTemplate = "STD-{0}";

      if (nucard.Status.Type != Enumerators.NuCard.NuCardStatus.Active)
      {
        errorMessage = Messages.NUCARD_IS_NOT_ACTIVE;
        return null;
      }
      string branchNum = string.Empty;

      using (var UoW = new UnitOfWork())
      {
        if (!string.IsNullOrEmpty(branchId))
        {
          var branch = new XPQuery<BRN_Branch>(UoW).FirstOrDefault(o => o.BranchId == Convert.ToInt64(branchId));
          if (branch == null)
            throw new Exception("Branch cannot be null");

          branchNum = BRN_Branch.ASSBranchCodeToGL(branch.LegacyBranchNum);
        }

        var transaction = new NUC_Transaction(UoW);
        transaction.Amount = amount;
        transaction.LoadDT = DateTime.Now;
        transaction.NuCard = new XPQuery<NUC_NuCard>(UoW).First(o => o.NuCardId == nucard.NuCardId);
        transaction.IsPending = true;
        transaction.CreatedBy = new XPQuery<PER_Person>(UoW).FirstOrDefault(o => o.PersonId == RoleContext.LoggedInUser.PersonId);
        transaction.CreatedDT = DateTime.Now;
        transaction.Source = new XPQuery<TransactionSource>(UoW).FirstOrDefault(o => o.Type == Enumerators.NuCard.TransactionSourceType.API);
        transaction.SourceApplication = Enumerators.General.ApplicationIdentifiers.AtlasManagement;

        UoW.CommitChanges();

        transactionDTO = transaction == null ? null : AutoMapper.Mapper.Map<NUC_Transaction, NUC_TransactionDTO>(transaction);
      }

      using (var UoW = new UnitOfWork())
      {
        var referenceTransaction = new XPQuery<NUC_Transaction>(UoW).FirstOrDefault(o => o.NucardTransactionId == transactionDTO.NucardTransactionId);
        if (!string.IsNullOrEmpty(branchId))
        {
          referenceTransaction.ReferenceNum = string.Format(referenceTemplate, transactionDTO.NucardTransactionId.ToString(), branchNum);
        }
        else
        {
          referenceTransaction.ReferenceNum = string.Format(referenceTemplate, transactionDTO.NucardTransactionId.ToString());
        }

        UoW.CommitChanges();
        transactionDTO.ReferenceNum = referenceTransaction.ReferenceNum;
      }

      return transactionDTO;
    }

    /// <summary>
    /// Creates a NuCard approval process
    /// </summary>
    /// <param name="nucard">NuCard the process is for</param>
    /// <param name="currentPerson">Person the process belongs to</param>
    /// <param name="transaction">Transaction that the process is linked to</param>
    public static void CreateNuCardApprovalProcess(NUC_NuCardDTO nucard, PER_PersonDTO currentPerson, NUC_TransactionDTO transaction)
    {
      if (nucard == null)
        throw new NullReferenceException(string.Format(Messages.OBJECT_CANNOT_BE_NULL, "NuCard"));

      if (currentPerson == null)
        throw new NullReferenceException(string.Format(Messages.OBJECT_CANNOT_BE_NULL, "CurrentPerson"));

      var personsWithAssociatedRole = RoleContext.GetPersonWithRole(Enumerators.General.RoleType.NuCard_Payout_Approver_Two);

      if (personsWithAssociatedRole.Count == 0)
        throw new Exception(Messages.PERSON_NOT_CONFIGURED_SECOND_LEVEL);

      using (var UoW = new UnitOfWork())
      {
        var convertedCard = new XPQuery<NUC_NuCard>(UoW).First(o => o.NuCardId == nucard.NuCardId);

        NUC_NuCardProcess firstAssignee = new NUC_NuCardProcess(UoW)
        {
          AssignedUser = new XPQuery<PER_Person>(UoW).First(o => o.PersonId == currentPerson.PersonId),
          CreatedDT = DateTime.Now,
          NuCard = convertedCard,
          IsApproved = false,
          Transaction = new XPQuery<NUC_Transaction>(UoW).First(o => o.NucardTransactionId == transaction.NucardTransactionId),
          CreatedBy = new XPQuery<PER_Person>(UoW).FirstOrDefault(o => o.PersonId == RoleContext.LoggedInUser.PersonId)
        };

        foreach (var item in personsWithAssociatedRole)
        {
          NUC_NuCardProcess secondAssignee = new NUC_NuCardProcess(UoW)
          {
            AssignedUser = new XPQuery<PER_Person>(UoW).First(o => o.PersonId == item.Person.PersonId),
            CreatedDT = DateTime.Now,
            NuCard = convertedCard,
            IsApproved = false,
            DependantNuCardProcess = firstAssignee,
            Transaction = new XPQuery<NUC_Transaction>(UoW).First(o => o.NucardTransactionId == transaction.NucardTransactionId),
            CreatedBy = new XPQuery<PER_Person>(UoW).FirstOrDefault(o => o.PersonId == RoleContext.LoggedInUser.PersonId)
          };
        }

        UoW.CommitChanges();
      }
    }

    /// <summary>
    /// Approves the NuCard payment
    /// </summary>
    /// <param name="nucardProcess">NuCard Process that payment is linked against</param>
    public static void ApprovePayment(NUC_NuCardProcessDTO nucardProcess)
    {
      if (nucardProcess == null)
        throw new NullReferenceException(string.Format(Messages.OBJECT_CANNOT_BE_NULL, "NuCardProcess"));

      using (var UoW = new UnitOfWork())
      {
        var process = new XPQuery<NUC_NuCardProcess>(UoW).FirstOrDefault(o => o.NuCardProcessId == nucardProcess.NuCardProcessId);

        if (process == null)
          throw new Exception(Messages.PROCESS_NOT_FOUND);

        process.IsApproved = true;
        process.ApprovedDT = DateTime.Now;

        UoW.CommitChanges();
      }
    }

    /// <summary>
    /// Declines the payment process
    /// </summary>
    /// <param name="nucardProcess">NuCard Process that the decline is linked against</param>
    public static void DeclinePayment(NUC_NuCardProcessDTO nucardProcess)
    {
      if (nucardProcess == null)
        throw new NullReferenceException(string.Format(Messages.OBJECT_CANNOT_BE_NULL, "NuCardProcess"));

      using (var UoW = new UnitOfWork())
      {
        var process = new XPQuery<NUC_NuCardProcess>(UoW).FirstOrDefault(o => o.NuCardProcessId == nucardProcess.NuCardProcessId);

        if (process == null)
          throw new Exception(Messages.PROCESS_NOT_FOUND);

        process.IsDeclined = true;
        process.DeclinedDT = DateTime.Now;

        UoW.CommitChanges();
      }
    }

    /// <summary>
    /// Returns the balance of a NuCard
    /// </summary>
    /// <param name="nucard">NuCard to return the balance for</param>
    /// <param name="appVersion">Calling application version</param>
    /// <param name="errorMessage">Any errors that may occurr</param>
    public static BalanceResult GetCardBalance(NUC_NuCardDTO nucard, string appVersion, out string errorMessage)
    {
      BalanceResult balanceResult = null;
      string serverTransactionId;

      using (var clientServer = new NuCardAdminServerClient("NuCardAdmin.NET"))
      {

        clientServer.CardBalance(GetSourceRequest(nucard, appVersion), nucard.CardNum, Guid.NewGuid().ToString(), out balanceResult, out serverTransactionId, out errorMessage);

      }
      return balanceResult;
    }


    /// <summary>
    /// Stops a NuCard
    /// </summary>
    /// <param name="nucard">NuCard that must be stopped</param>
    /// <param name="nucardStatus">The stop status for the NuCard</param>
    /// <param name="appVersion">The calling application version</param>
    /// <param name="errorMessage">Any errors that may occur</param>
    public static bool StopCard(NUC_NuCardDTO nucard, Enumerators.NuCard.NuCardStatus nucardStatus, string appVersion, out string errorMessage)
    {
      string serverTransactionId;
      int transferredAmount;

      using (var clientServer = new NuCardAdminServerClient())
      {
        clientServer.StopCard(GetSourceRequest(nucard, appVersion), nucard.CardNum, (int)nucardStatus, Guid.NewGuid().ToString(), out transferredAmount,
          out  serverTransactionId, out errorMessage);

        if (!string.IsNullOrEmpty(errorMessage))
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Transfer from card to card
    /// </summary>
    /// <param name="nucard">NuCard wishing to transfer from</param>
    /// <param name="appVersion">Calling application version</param>
    /// <param name="toCard">Card that you wish to transfer to</param>
    /// <param name="amount">Amount you wish to transfer leave it 0.00 to transfer everything</param>
    /// <param name="stopFromCard">Should the from card be stopped</param>
    /// <param name="nucardStatus">Stop status for the from card</param>
    /// <param name="transferredAmount">The total amount transferred out</param>
    /// <param name="errorMessage">Any errors that may occur</param>
    public static bool TransferCardToCard(NUC_NuCardDTO nucard, string appVersion, string toCard, decimal amount,
                                                           bool stopFromCard, Atlas.Enumerators.NuCard.NuCardStatus nucardStatus, out decimal transferredAmount,
                                                           out string errorMessage)
    {
      string serverTransactionId;
      using (var clientServer = new NuCardAdminServerClient())
      {
        int transedOutAmount;
        clientServer.TransferFundsBetweenCards(GetSourceRequest(nucard, appVersion), nucard.CardNum, toCard, Convert.ToInt32(amount * 100),
          Guid.NewGuid().ToString(), stopFromCard, (int)nucardStatus, out transedOutAmount, out serverTransactionId, out errorMessage);

        transferredAmount = (Convert.ToDecimal(transedOutAmount) * 100);

        using (var UoW = new UnitOfWork())
        {
          var transaction = new NUC_Transaction(UoW);
          transaction.Amount = transferredAmount;
          transaction.LoadDT = DateTime.Now;
          transaction.NuCard = new XPQuery<NUC_NuCard>(UoW).First(o => o.CardNum == toCard);
          transaction.ReferenceNum = Guid.NewGuid().ToString();
          transaction.IsPending = false;
          transaction.CreatedBy = new XPQuery<PER_Person>(UoW).FirstOrDefault(o => o.PersonId == RoleContext.LoggedInUser.PersonId);
          transaction.CreatedDT = DateTime.Now;
          transaction.Source = new XPQuery<TransactionSource>(UoW).FirstOrDefault(o => o.Type == Enumerators.NuCard.TransactionSourceType.API);
          transaction.SourceApplication = Enumerators.General.ApplicationIdentifiers.AtlasManagement;

          UoW.CommitChanges();
        }

        if (!string.IsNullOrEmpty(errorMessage))
        {
          return false;
        }
      }
      return true;
    }

    /// <summary>
    /// Transfer funds from card to originating profile
    /// </summary>
    /// <param name="nucard">NuCard that the funds are being transferred from</param>
    /// <param name="appVersion">Calling application version</param>
    /// <param name="amountToTransfer">Amount to transfer out leave 0 to transfer entire balance</param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static bool TransferFromCardToProfile(NUC_NuCardDTO nucard, string appVersion, decimal amountToTransfer, out string errorMessage)
    {
      string serverTransactionId;
      int transferredAmount;

      using (var clientServer = new NuCardAdminServerClient())
      {
        clientServer.DeductFromCardLoadProfile(GetSourceRequest(nucard, appVersion), nucard.CardNum,
           Convert.ToInt32(amountToTransfer * 100), Guid.NewGuid().ToString(), out transferredAmount, out serverTransactionId, out errorMessage);

        if (!string.IsNullOrEmpty(errorMessage))
        {
          return false;
        }
        return true;
      }
    }

    public static bool AllocateCard(NUC_NuCardDTO nucard, PER_PersonDTO person, string appVersion, decimal amount, out string errorMessage)
    {
      string serverTransactionId = string.Empty;

        throw new InvalidOperationException("Cannot get person contact");
      //var cellNum = person.Contacts.FirstOrDefault(o => o.ContactType.Type == Enumerators.General.ContactType.CellNo);

      //if (cellNum == null)
      //  throw new Exception(Messages.PERSON_ALLOCATED_REQUIRES_VALID_CELL_NO);

      //using (var clientServer = new NuCardAdminServerClient())
      //{

      //  clientServer.AllocateAndDeductFromProfileLoadCard(GetSourceRequest(nucard, appVersion), nucard.CardNum, person.Firstname,
      //      person.Lastname, person.IdNum, cellNum.Value, Convert.ToInt32(amount * 100), Guid.NewGuid().ToString(), out serverTransactionId,
      //      out errorMessage);

      //  if (!string.IsNullOrEmpty(errorMessage))
      //  {
      //    return false;
      //  }
      //  return true;
      //  }
    }

    public static List<StatementLine> CardStatement(NUC_NuCardDTO nucard, string appVersion)
    {
      StatementResult statementResult = null;
      string serverTransactionId = string.Empty;
      string errorMessage = string.Empty;

      using (var clientServer = new NuCardAdminServerClient())
      {
        clientServer.CardStatement(GetSourceRequest(nucard, appVersion), nucard.CardNum, out statementResult, out serverTransactionId, out errorMessage);
      }

      List<StatementLine> statementLines = new List<StatementLine>();

      if (statementResult.StatementLines != null)
      {
        foreach (var item in statementResult.StatementLines)
        {
          statementLines.Add(new StatementLine()
          {
            Amount = (item.TransactionAmountInCents / 100),
            TransactionDate = item.TransactionDate,
            TransactionDescription = item.TransactionDescription,
            Type = TransactionType(item.TransactionType)
          });
        }
      }

      return statementLines;
    }

    public static bool TransferToCard(NUC_NuCardProcessDTO nucardProcess, string appVersion, out string errorMessage)
    {
      string serverTransactionId = string.Empty;

      using (var clientServer = new NuCardAdminServerClient())
      {
        clientServer.DeductFromProfileLoadCard(GetSourceRequest(nucardProcess.NuCard, appVersion),
          nucardProcess.NuCard.CardNum, Convert.ToInt32(nucardProcess.Transaction.Amount * 100),
          Guid.NewGuid().ToString(), out serverTransactionId, out errorMessage);

        if (string.IsNullOrEmpty(errorMessage))
        {
          using (var UoW = new UnitOfWork())
          {
            var transaction = new XPQuery<NUC_Transaction>(UoW).Where(o => o.NucardTransactionId == nucardProcess.Transaction.NucardTransactionId).FirstOrDefault();
            if (transaction == null)
              throw new Exception(string.Format(Messages.RECORD_IS_MISSING, "Transaction"));

            transaction.IsPending = false;
            transaction.LastEditedBy = new XPQuery<PER_Person>(UoW).FirstOrDefault(o => o.PersonId == RoleContext.LoggedInUser.PersonId);
            transaction.LastEditedDT = DateTime.Now;
            transaction.ServerTransactionId = serverTransactionId;

            UoW.CommitChanges();
            return true;
          }
        }
        else
        {
          return false;
        }
      }
    }

    public static List<NUC_NuCardProcessDTO> GetMyProcesses(PER_PersonDTO person)
    {
      List<NUC_NuCardProcessDTO> myProcesses = new List<NUC_NuCardProcessDTO>();
      using (var UoW = new UnitOfWork())
      {
        var processesForUser = new XPQuery<NUC_NuCardProcess>(UoW).Where(o => o.AssignedUser.PersonId == person.PersonId
                                                                     && o.ApprovedDT == null && o.DeclinedDT == null).ToList();

        foreach (var process in processesForUser)
        {
          if (process.DependantNuCardProcess != null)
          {
            if (process.DependantNuCardProcess.IsApproved)
            {
              myProcesses.Add(AutoMapper.Mapper.Map<NUC_NuCardProcess, NUC_NuCardProcessDTO>(process));
            }
          }
          else
          {
            myProcesses.Add(AutoMapper.Mapper.Map<NUC_NuCardProcess, NUC_NuCardProcessDTO>(process));
          }
        }
      }
      return myProcesses;
    }

    /// <summary>
    /// Returns the maximum allowed commission transfer per session
    /// </summary>
    public static decimal GetMaximumCommission()
    {
      using (var UoW = new UnitOfWork())
      {
        var config = new XPQuery<Config>(UoW).FirstOrDefault(o => o.DataType == (int)Enumerators.General.ApplicationIdentifiers.AtlasManagement &&
                                                               o.DataSection == "COMMAX");

        if (config == null)
          throw new Exception("Missing configuration entry for commission");

        return Convert.ToDecimal(config.DataValue);
      }
    }

    /// <summary>
    /// Imports a batch file into memory
    /// </summary>
    /// <param name="fileName">File containing card numbers/sequence numbers/tracking numbers</param>
    public static List<NUC_NuCardDTO> ImportBatchFile(string fileName)
    {
      //List<NUC_NuCardDTO> nucardDTO = new List<NUC_NuCardDTO>();
      //foreach (var card in Atlas.Common.CSV.NucardParse.ParseBatchFile(fileName))
      //{
      //  if (card.Tracking != "Tracking")
      //  {
      //    nucardDTO.Add(new NUC_NuCardDTO() { TrackingNum = card.Tracking, CardNum = card.Card.Replace(" ", ""), SequenceNum = card.Sequence });
      //  }
      //}
      //return nucardDTO;
      throw new NotImplementedException();
    }

    public static NUC_NuCardBatchDTO CreateNuCardBatch(NUC_NuCardBatchDTO nucardBatch, List<NUC_NuCardDTO> nucardDTO)
    {
      NUC_NuCardBatchDTO batchDTO = null;

      using (var UoW = new UnitOfWork())
      {
        NUC_NuCardBatch batch = new NUC_NuCardBatch(UoW);
        batch.Comment = nucardBatch.Comment;
        batch.Courier = new XPQuery<CPY_Company>(UoW).FirstOrDefault(o => o.CompanyId == nucardBatch.Courier.CompanyId);
        batch.CreatedBy = new XPQuery<PER_Person>(UoW).FirstOrDefault(o => o.PersonId == RoleContext.LoggedInUser.PersonId);
        batch.CreatedDT = DateTime.Now;
        batch.DeliverToBranch = new XPQuery<BRN_Branch>(UoW).FirstOrDefault(o => o.BranchId == nucardBatch.DeliverToBranch.BranchId);
        batch.DeliveryDT = nucardBatch.DeliveryDT;
        batch.OutSequence = nucardBatch.OutSequence;
        batch.QuantitySent = nucardBatch.QuantitySent;
        batch.SentBy = new XPQuery<PER_Person>(UoW).FirstOrDefault(o => o.PersonId == RoleContext.LoggedInUser.PersonId);
        batch.SequenceEnd = nucardBatch.SequenceEnd;
        batch.SequenceStart = nucardBatch.SequenceStart;
        batch.Status = nucardBatch.Status;
        batch.TrackingNum = nucardBatch.TrackingNum;

        if (nucardDTO != null)
        {
          if (nucardDTO.Count > 0)
          {
            DateTime dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            foreach (var item in nucardDTO)
            {
              batch.NucardBatchCards.Add(new NUC_NuCardBatchCard(UoW)
              {
                NuCard = new NUC_NuCard(UoW)
                {
                  CardNum = item.CardNum,
                  SequenceNum = item.SequenceNum,
                  TrackingNum = item.TrackingNum,
                  Status = new XPQuery<NUC_NuCardStatus>(UoW).FirstOrDefault(o => o.Type == Enumerators.NuCard.NuCardStatus.InStock),
                  CreatedBy = new XPQuery<PER_Person>(UoW).FirstOrDefault(o => o.PersonId == RoleContext.LoggedInUser.PersonId),
                  CreatedDT = DateTime.Now,
                  ExpiryDT = dt.AddYears(3)
                },
                NuCardBatch = batch
              });
            }
          }
        }

        UoW.CommitChanges();

        batchDTO = AutoMapper.Mapper.Map<NUC_NuCardBatch, NUC_NuCardBatchDTO>(batch);
      }
      return batchDTO;
    }


    public static List<NUC_NuCardBatchDTO> LoadPendingBatches()
    {
      List<NUC_NuCardBatchDTO> pendingBatches = new List<NUC_NuCardBatchDTO>();
      using (var UoW = new UnitOfWork())
      {
        var batchCollection = new XPQuery<NUC_NuCardBatch>(UoW)
                                       .Where(o => o.Status == Enumerators.General.NucardBatchStatus.Waiting_Collection).ToList();

        pendingBatches = AutoMapper.Mapper.Map<List<NUC_NuCardBatch>, List<NUC_NuCardBatchDTO>>(batchCollection);
      }

      return pendingBatches;
    }

    public static List<string> BatchStatus()
    {
      List<string> items = new List<string>();

      foreach (Enumerators.General.NucardBatchStatus batchStatus in Enum.GetValues(typeof(Enumerators.General.NucardBatchStatus)))
      {
        items.Add(((Enumerators.General.NucardBatchStatus)batchStatus).ToStringEnum());
      }

      return items;
    }

    public static void UpdateBatch(NUC_NuCardBatchDTO batch)
    {
      using (var UoW = new UnitOfWork())
      {
        var batchRec = new XPQuery<NUC_NuCardBatch>(UoW).FirstOrDefault(o => o.NuCardBatchId == batch.NuCardBatchId);

        if (batchRec == null)
          throw new Exception("Batch record does not exist");

        batchRec.Status = batch.Status;
        batchRec.TrackingNum = batch.TrackingNum;

        UoW.CommitChanges();
      }
    }
  }
}
