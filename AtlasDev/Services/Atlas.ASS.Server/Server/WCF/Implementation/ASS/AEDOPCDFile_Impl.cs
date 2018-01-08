using System;
using System.Linq;
using System.Text;

using Atlas.Common.Interface;
using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.Cache.Interfaces;
using Atlas.Server.Classes.CustomException;


namespace Atlas.Server.WCF.Implementation.ASS
{
  internal static class AEDOPCDFile_Impl
  {
    internal static int Execute(ILogging log, IConfigSettings config, ICacheServer cache, 
      string branchNum, DateTime startDate, DateTime endDate,
      out string file, out string errorMessage)
    {
      file = string.Empty;
      errorMessage = string.Empty;
      var methodName = "AEDOPCDFile";
      #region Check parameters
      if (string.IsNullOrEmpty(branchNum))
      {
        throw new BadParamException($"Invalid branch number: '{branchNum}'");
      }

      var dateDiff = endDate.Subtract(startDate);
      if (dateDiff.TotalDays <= 0 || dateDiff.TotalDays > 60)
      {
        throw new BadParamException($"Invalid date range: '{dateDiff.TotalDays}' days. (1-60 expected)");
      }

      #endregion

      try
      {
        using (var unitOfWork = new UnitOfWork())
        {
          // Get Branch
          var branch = unitOfWork.Query<BRN_Branch>().FirstOrDefault(s => s.LegacyBranchNum.PadLeft(3, '0') == branchNum.PadLeft(3, '0'));
          if (branch == null)
          {
            throw new BadParamException($"Unable to locate branch: '{branchNum}'");
          }

          var sb = new StringBuilder();

          sb.AppendLine("transaction_id,value_date,contract_no,term_id,actual_date,retry_count,reason,contact_amt,pan,instalment,amount,mass#,type,emp_no,orig_no_install,future3,future4,future5,original_loan_amount,spf_value");
          /*
            transaction_id,value_date,contract_no,term_id,actual_date,retry_count,reason,contact_amt,pan,instalment,amount,mass#,type,emp_no,orig_no_install,future3,future4,future5,original_loan_amount,spf_value
            72561185,20121001,4107500000012648,11526243,20121001, , ,3717.54,4938050104439041,3,619.59,000000000494740,1, ,6, , , ,619.59,.00            
          */

          #region Data
          var searchFor = string.Format("{0}x", branchNum.ToLower().PadLeft(5, '0'));
          var successful = unitOfWork.Query<AEDOReportSuccess>().Where(s =>
            s.ContractNum.ToLower().StartsWith(searchFor) &&
            s.ReportSuccess.SuccessDT >= startDate && s.ReportSuccess.SuccessDT < endDate.AddDays(1))
            .OrderBy(s => s.ReportSuccess.SuccessDT)
            .ThenBy(s => s.ContractNum);

          foreach (var success in successful)
          {
            //transaction_id - 0,value_date - 1,contract_no - 2,term_id - 3,actual_date - 4,
            // retry_count - 5,reason - 6,contact_amt - 7,pan - 8,instalment - 9, amount - 10,
            // mass# - 11,type - 12,emp_no - 13,orig_no_install - 14,future3 - 15,future4 - 16, future5 - 17
            // original_loan_amount - 18,spf_value - 19
            sb.AppendFormat("{0},{1:yyyyMMdd},{2},{3},{4:yyyyMMdd},{5},{6},{7:F2},{8},{9},{10:F2},{11},{12},{13},{14},{15},{16},{17},{18:F2},{19:F2}\r\n",
              /*0*/ success.ReportSuccess.TransactionId,
              /*1*/ success.ReportSuccess.ValueDT,
              /*2*/ success.ContractNum,
              /*3*/ success.TerminalNum,
              /*4*/ success.ReportSuccess.SuccessDT,
              /*5*/ " ",
              /*6*/ " ",
              /*7*/ success.ContractAmount,
              /*8*/ success.Pan,
              /*9*/ success.InstalmentNum,
              /*10*/success.InstalmentAmount,
              /*11*/success.CardAcceptor,
              /*12*/success.ReportBatch.ReportType,
              /*13*/success.EmployerCode,
              /*14*/"",
              /*15*/"",
              /*16*/"",
              /*17*/"",
              /*18*/success.ContractAmount,
              /*19*/0.00);
          }
          #endregion

          file = sb.ToString();
        }

        return (int)General.WCFCallResult.OK;

      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = (err is BadParamException) ? err.Message : "Unexpected server error";
        return (err is BadParamException) ? (int)General.WCFCallResult.BadParams : (int)General.WCFCallResult.ServerError;
      }
    }

  }
}
