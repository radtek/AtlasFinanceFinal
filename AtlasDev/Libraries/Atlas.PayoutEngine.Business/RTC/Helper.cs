using Atlas.Domain.DTO;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.PayoutEngine.Business.RTC
{
  public static class Helper
  {
    public static void Export(PYT_BatchDTO batch, PYT_TransmissionDTO transmission, PYT_TransmissionSetDTO transmissionSet, Dictionary<PYT_TransmissionTransactionDTO, AddressDTO> transmissionTransactions)
    {
      var rtcFile = string.Empty;
      var sequenceNo = 1;

      foreach (var transmissionTransaction in transmissionTransactions)
      {
        var mt101 = new FileStructure.MT101(batch.Service.SwiftCode, transmissionTransaction.Key.Payout.PayoutId, sequenceNo, transmissionTransactions.Count,
          batch.Service.AccountNo, batch.Service.Username, batch.Service.Address1, batch.Service.Address2, batch.Service.Address3, batch.Service.Address4,
          transmissionTransaction.Key.Payout.ActionDate, transmissionTransaction.Key.TransmissionSet.Transmission.TransmissionId, transmissionTransaction.Key.Payout.Amount,
          transmissionTransaction.Key.Payout.BankDetail.Bank.SwiftCode, transmissionTransaction.Key.Payout.BankDetail.Bank.UniversalCode,
          string.Format("{0} {1}", transmissionTransaction.Key.Payout.Person.Firstname, transmissionTransaction.Key.Payout.Person.Lastname), transmissionTransaction.Value.Line1,
          transmissionTransaction.Value.Line2, transmissionTransaction.Value.Line3, transmissionTransaction.Value.Line4, transmissionTransaction.Key.Payout.BankDetail.AccountNum,
          transmissionTransaction.Key.Payout.Account.AccountNo, batch.Service.ReferenceName);

        rtcFile += mt101.BuildString() + System.Environment.NewLine;

        sequenceNo++;
      }

      FileHelper.CreateFileAndMoveToPath(rtcFile, string.Format("{0}{1}_{2}.txt", batch.Service.OutgoingPath, batch.Service.ReferenceName, batch.BatchId));
    }

    public static List<Response> Import(string filePath)
    {
      var responses = new List<Response>();

      // read all data from file
      var fileContent = string.Empty;
      using (var streamReader = new StreamReader(filePath))
      {
        fileContent = streamReader.ReadToEnd();
      }

      // make sure the files not empty
      if (!string.IsNullOrEmpty(fileContent))
      {
        // split up the different transactions
        responses = new FileStructure.MT195(fileContent).ConvertToResponse();
      }

      return responses;
    }
  }
}
