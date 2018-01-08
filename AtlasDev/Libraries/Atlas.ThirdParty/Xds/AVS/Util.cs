using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Atlas.Domain.DTO;
using Atlas.ThirdParty.Interfaces;
using Atlas.ThirdParty.Xds.AVS.Structures;


namespace Atlas.ThirdParty.Xds.AVS
{
  public class Util : IAVSUtil
  {
    public List<Tuple<AVS_TransactionDTO, bool>> PerformAVSEnquiry(string username, string key, AVS_BatchDTO batch, List<AVS_TransactionDTO> transactions,
      Delegate updateBatchAction, Delegate importAction)
    {
      var transactionsToUpdate = new List<Tuple<AVS_TransactionDTO, bool>>();

      var transactionsPerRound = 5;
      var rounds = Math.Ceiling((decimal)transactions.Count / transactionsPerRound);
      for (var j = 0; j < rounds; j++)
      {
        var tempTransactions = transactions.Skip(j * transactionsPerRound).Take(transactionsPerRound).ToList();

        using (var connect = new Connect(username, key))
        {
          var tasks = new Task[tempTransactions.Count];
          for (var i = 0; i < tempTransactions.Count; i++)
          {
            var transaction = tempTransactions[i];

            tasks[i] = Task.Factory.StartNew(() =>
            {
              Enquire(connect, username, key, ref transaction);
              var tmpList = new List<Tuple<AVS_TransactionDTO, bool>>();
              tmpList.Add(new Tuple<AVS_TransactionDTO, bool>(transaction, false));
              transactionsToUpdate.AddRange((List<Tuple<AVS_TransactionDTO, bool>>)importAction.DynamicInvoke(tmpList, null));
            });
          }

          Task.WaitAll(tasks);
        }
      }

      updateBatchAction.DynamicInvoke(batch, true);

      return transactionsToUpdate;
    }


    public Tuple<AVS_TransactionDTO, bool> GetResponse(string username, string key, AVS_TransactionDTO pendingTransaction)
    {
      Tuple<AVS_TransactionDTO, bool> transaction = null;

      using (var connect = new Connect(username, key))
      {
        var enquiryId = 0;
        if (int.TryParse(pendingTransaction.ThirdPartyRef, out enquiryId))
        {
          var result = connect.Client.ConnectGetAccountVerificationResult(connect.Ticket, enquiryId);
          var noResult = NoResultResponse(result);
          if (noResult == null)
          {
            AVSResult.AccountVerificationResult avsResult;
            MemoryStream memStreamResult = new MemoryStream(Encoding.UTF8.GetBytes(result));
            XmlSerializer serializerResult = new XmlSerializer(typeof(AVSResult.AccountVerificationResult));
            avsResult = (AVSResult.AccountVerificationResult)serializerResult.Deserialize(memStreamResult);

            UpdateResponses(ref pendingTransaction, avsResult.ResultFile);
            transaction = new Tuple<AVS_TransactionDTO, bool>(pendingTransaction, true);
          }
          else if (string.IsNullOrEmpty(noResult.Error))
          {
            // no error, just waiting for response from banks
            transaction = new Tuple<AVS_TransactionDTO, bool>(pendingTransaction, false);
          }
          else
          {
            // error with submission
            pendingTransaction.ErrorMessage = noResult.Error;
            transaction = new Tuple<AVS_TransactionDTO, bool>(pendingTransaction, true);
          }
        }
        else
        {
          throw new Exception("Third Party Reference is not valid");
        }
      }

      return transaction;
    }


    private void Enquire(Connect connect, string username, string key, ref AVS_TransactionDTO transaction)
    {
      var timeOutMessage = string.Empty;
      var retryAttempts = 0;
      AccountVerification accountVerification = new AccountVerification();
      do
      {
        try
        {
          var idNumber = transaction.IdNumber;
          var avsIdType = XDSConnect.AVSIDType.SID;
          if (!new Common.Utils.IDValidator(transaction.IdNumber).isValid())
          {
            avsIdType = XDSConnect.AVSIDType.FPP;
            // make provision for standard passport numbers
            if (transaction.Bank.Type == Enumerators.General.BankName.STD)
              idNumber = Common.Utils.StringUtils.RemoveNonNumeric(transaction.IdNumber);
          }
          var result = connect.Client.ConnectAccountVerificationRealTime(connect.Ticket, XDSConnect.TypeofVerificationenum.Individual, XDSConnect.Entity.None,
            transaction.Initials, transaction.LastName, transaction.IdNumber, avsIdType, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,
            transaction.AccountNo, transaction.BranchCode, "Savings Account", GetBank(transaction.Bank.Type), "", transaction.TransactionId.ToString());
          using (MemoryStream memStreamReference = new MemoryStream(Encoding.UTF8.GetBytes(result)))
          {
            XmlSerializer serializerReference = new XmlSerializer(typeof(AccountVerification));
            accountVerification = (AccountVerification)serializerReference.Deserialize(memStreamReference);
          }
        }
        catch (TimeoutException exception)
        {
          timeOutMessage = exception.Message;
        }
        retryAttempts++;
      } while (!string.IsNullOrEmpty(timeOutMessage) && retryAttempts < 3);

      if (!string.IsNullOrEmpty(timeOutMessage))
      {
        transaction.ErrorMessage = timeOutMessage;
      }

      transaction.ThirdPartyRef = accountVerification.ReferenceNo.ToString();
    }


    private void UpdateResponses(ref AVS_TransactionDTO transaction, AVSResult.AccountVerificationResultResultFile resultFile)
    {
      transaction.ResponseAcceptsCredit = GetResponsecode(resultFile.ACCOUNTACCEPTSCREDITS);
      transaction.ResponseAcceptsDebit = GetResponsecode(resultFile.ACCOUNTACCEPTSDEBITS);
      transaction.ResponseAccountNumber = GetResponsecode(resultFile.ACCOUNTFOUND);
      transaction.ResponseAccountOpen = GetResponsecode(resultFile.ACCOUNTOPEN);
      transaction.ResponseIdNumber = GetResponsecode(resultFile.IDNUMBERMATCH);
      transaction.ResponseInitials = GetResponsecode(resultFile.INITIALSMATCH);
      transaction.ResponseLastName = GetResponsecode(resultFile.SURNAMEMATCH);
      transaction.ResponseOpenThreeMonths = GetResponsecode(resultFile.ACCOUNTOPENFORATLEASTTHREEMONTHS);

      if (resultFile.ERRORCONDITIONNUMBER.Contains("Timeout "))
        transaction.ErrorMessage = resultFile.ERRORCONDITIONNUMBER;
    }


    private string GetResponsecode(string description)
    {
      switch (description.Trim())
      {
        case "Yes":
          return "00";
        case "No":
          return "01";
        case "Not Available":
          return "99";
        default:
          return "99";
      }
    }


    private string GetBank(Atlas.Enumerators.General.BankName bank)
    {
      switch (bank)
      {
        case Enumerators.General.BankName.ABS:
          return "ABSA";
        case Enumerators.General.BankName.STD:
          return "STANDARD BANK";
        case Enumerators.General.BankName.FNB:
          return "FNB";
        case Enumerators.General.BankName.NED:
          return "NEDBANK";
        case Enumerators.General.BankName.CAP:
          return "CAPITEC";
        case Enumerators.General.BankName.AFR:
          return "AFRICAN BANK";
        case Enumerators.General.BankName.MER:
          return "MERCANTILE BANK";
        default: return string.Empty;
      }
    }


    private NoResult NoResultResponse(string result)
    {
      try
      {
        NoResult avsResult;
        MemoryStream memStreamResult = new MemoryStream(Encoding.UTF8.GetBytes(result));
        XmlSerializer serializerResult = new XmlSerializer(typeof(NoResult));
        avsResult = (NoResult)serializerResult.Deserialize(memStreamResult);
        return avsResult;
      }
      catch (InvalidOperationException)
      {
        return null;
      }
    }

  }
}