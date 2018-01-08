using Atlas.Domain.DTO;
using Atlas.ThirdParty.ABSA.FileStructures.Transmission;
using Atlas.ThirdParty.ABSA.NAEDO;
using Atlas.ThirdParty.ABSA.NAEDO.FileStructures;
using Atlas.ThirdParty.ABSA.NAEDO.FileStructures.Export;
using Atlas.ThirdParty.ABSA.NAEDO.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Collection.Engine.Business
{
  public sealed class FileUtility
  {
    public bool ExportFile(DBT_ServiceDTO service, List<DBT_TransmissionSetDTO> transmissionSet, DBT_TransmissionDTO transmission, DBT_BatchDTO batch, List<DBT_TransmissionTransactionDTO> transmissionTransaction, List<DBT_TransactionDTO> transactions,
                     List<DBT_ControlDTO> controls)
    {
      try
      {
        var recordStatus = service.Environment.ToString();

        var lineCount = 0;

        var transmissionHeader = new TransmissionHeader()
        {
          RecordStatus = recordStatus,
          TransmissionDate = DateTime.Today.Date.ToString("yyyyMMdd"),
          ElectronicBankingSuiteUserCode = service.UserCode,
          ElectronicBankingSuiteUserName = service.ReferenceName,
          TransmissionNo = transmission.TransmissionNo.ToString()
        };

        lineCount++;
        List<ExportFile> exportFileCollection = new List<ExportFile>();

        var trackingDays = controls.Select(c => c.TrackingDays).Distinct().OrderBy(c => c).ToList();

        foreach (var trackingDay in trackingDays)
        {
          var tempControls = controls.Where(c => c.TrackingDays == trackingDay).Distinct().ToList();
          var tempControlIds = tempControls.Select(c => c.ControlId).ToList();

          ExportFile export = new ThirdParty.ABSA.NAEDO.ExportFile();

          var transSet = transmissionSet.FirstOrDefault(
            o => o.TransmissionSetId == transmissionTransaction.FirstOrDefault(
              m => m.Transaction.Control.ControlId == tempControls.FirstOrDefault().ControlId).TransmissionSet.TransmissionSetId);
          var tempTransactions = transactions.Where(t => tempControls.Contains(t.Control)).ToList();
          lineCount++;
          var userHeader = new UserHeader();
          userHeader.DataSetStatus = recordStatus;
          userHeader.BankServUserCode = service.UserName;
          userHeader.BankServCreationDate = DateTime.Now.ToString("yyMMdd");
          userHeader.BankServPurgeDate = DateTime.Now.AddDays(+1).ToString("yyMMdd");
          userHeader.FirstActionDate = transactions.Where(o => tempControlIds.Contains(o.Control.ControlId)).Min(o => o.ActionDate).ToString("yyMMdd");
          userHeader.LastActionDate = transactions.Where(o => tempControlIds.Contains(o.Control.ControlId)).Max(o => o.ActionDate).ToString("yyMMdd");
          userHeader.FirstSequenceNo = transmissionTransaction.Where(o => o.Batch.BatchId == batch.BatchId
            && o.TransmissionSet.TransmissionSetId == transSet.TransmissionSetId).Min(o => o.SequenceNo).Value.ToString();
          userHeader.UserGenerationNo = transSet.GenerationNo.ToString();
          userHeader.TypeOfService = Utility.GetTrackingDay(trackingDay, false);

          export.UserHeader = userHeader;
          export.Transactions = new List<Transaction>();
          decimal totalSum = 0;
          //foreach (var control in tempControls)
          //{
          foreach (var transaction in transmissionTransaction.Where(o => tempControlIds.Contains(o.Transaction.Control.ControlId)).OrderBy(o => o.SequenceNo))
          {
            var control = tempControls.FirstOrDefault(c => c.ControlId == transaction.Transaction.Control.ControlId);
            lineCount++;
            var instruction = new Transaction()
            {
              DataSetStatus = recordStatus,
              OriginatingBranch = service.BranchCode,
              OriginatingAccountNo = service.AccountNo,
              UserCode = service.UserName,
              SequenceNo = transaction.SequenceNo.ToString(),
              HomingBranch = control.BankBranchCode,
              HomingAccountNo = control.BankAccountNo,
              HomingAccountType = ((int)control.BankAccountType.Type).ToString(),
              InstalmentAmount = (transaction.Transaction.Amount * 100).ToString("0"),
              ActionDate = (transaction.Transaction.OverrideActionDate ?? transaction.Transaction.ActionDate).ToString("yyMMdd"),
              EntryClass = Utility.GetTrackingDay(control.TrackingDays, true),
              UserReference = transaction.Transaction.Control.Service.UserReference,
              ContractReference = transaction.Transaction.Control.BankStatementReference,
              CycleDate = (transaction.Transaction.OverrideActionDate ?? transaction.Transaction.ActionDate).ToString("yyMMdd"),
              HomingAccountName = control.BankAccountName.ToUpper()
            };

            if ((instruction.ContractReference.Length + instruction.SequenceNo.Length) > 14)
              instruction.ContractReference = instruction.ContractReference.Substring(0, 14 - instruction.SequenceNo.Length) + instruction.SequenceNo.ToString();
            else
              instruction.ContractReference += instruction.SequenceNo;

            totalSum += transaction.Transaction.Amount;
            export.Transactions.Add(instruction);
          }
          //}

          lineCount++;
          var firstSeq = transmissionTransaction.Where(o => o.Batch.BatchId == batch.BatchId
            && o.TransmissionSet.TransmissionSetId == transSet.TransmissionSetId).Min(o => o.SequenceNo).Value.ToString();
          var lastSeq = transmissionTransaction.Where(o => o.Batch.BatchId == batch.BatchId
            && o.TransmissionSet.TransmissionSetId == transSet.TransmissionSetId).Max(o => o.SequenceNo).Value.ToString();
          var firstActionDate = transactions.Where(o => tempControlIds.Contains(o.Control.ControlId)).Min(o => o.ActionDate).ToString("yyMMdd");
          var lastActionDate = transactions.Where(o => tempControlIds.Contains(o.Control.ControlId)).Max(o => o.ActionDate).ToString("yyMMdd");
          var acbHashTotal = export.Transactions.Sum(o => Convert.ToInt64(o.HomingAccountNo)).ToString();
          if (acbHashTotal.Length > 12)
            acbHashTotal = acbHashTotal.Substring(acbHashTotal.Length - 12, 12);

          var userTrailer = new UserTrailer()
          {
            DataSetStatus = recordStatus,
            BankServUserCode = service.UserName,
            FirstSequenceNo = firstSeq,
            LastSequenceNo = lastSeq,
            FirstActionDate = firstActionDate,
            LastActionDate = lastActionDate,
            NoOfDebtRecords = export.Transactions.Count.ToString(),
            TotalDebitValue = (totalSum * 100).ToString("0"),
            HashTotalOfHomingAccountNos = acbHashTotal
          };

          export.UserTrailer = userTrailer;

          exportFileCollection.Add(export);
        }

        lineCount++;
        var transmissionTrailer = new TransmissionTrailer()
        {
          RecordStatus = recordStatus,
          NoOfRecordsInTransmission = Convert.ToString(lineCount)
        };


        CreateFileAndMoveToPath(new Instruction().GenerateFileContent(transmissionHeader, exportFileCollection, transmissionTrailer),
          string.Format("{0}{1}_{2}.txt", service.OutgoingFilePath, service.UserCode, batch.BatchId));
      }
      catch (Exception ex)
      {
        throw ex;
      }
      return true;
    }

    public static void CreateFileAndMoveToPath(string text, string path)
    {
      using (TextWriter textWriter = new StreamWriter(path))
      {
        textWriter.Write(text);
        textWriter.Flush();
        textWriter.Close();
      }
    }
  }
}