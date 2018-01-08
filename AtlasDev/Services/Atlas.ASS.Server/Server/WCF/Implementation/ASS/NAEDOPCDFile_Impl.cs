using System;
using System.Linq;
using System.Text;

using DevExpress.Xpo;
using Atlas.Common.Interface;

using Atlas.Enumerators;
using Atlas.Domain.Model;
using Atlas.Cache.Interfaces;


namespace Atlas.Server.WCF.Implementation.ASS
{
  internal static class NAEDOPCDFile_Impl
  {
    internal static int Execute(ILogging log, 
      string branchNum, DateTime startDate, DateTime endDate,
      out string file, out string errorMessage)
    {
      file = string.Empty;
      errorMessage = string.Empty;
      var methodName = "NAEDOPCDFile";
      #region Check parameters
      if (string.IsNullOrEmpty(branchNum))
      {
        errorMessage = string.Format("Invalid branch number: '{0}'", branchNum);
        log.Warning(new Exception(errorMessage), methodName);
        return (int)General.WCFCallResult.BadParams;
      }

      var dateDiff = endDate.Subtract(startDate);
      if (dateDiff.TotalDays <= 0 || dateDiff.TotalDays > 60)
      {
        errorMessage = string.Format("Invalid date range: '{0}' days. (1-60 expected)", dateDiff.TotalDays);
        log.Warning(new Exception(errorMessage), methodName);
        return (int)General.WCFCallResult.BadParams;
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
            errorMessage = string.Format("Invalid branch number: '{0}'", branchNum);
            log.Warning(new Exception(errorMessage), methodName);

            return (int)General.WCFCallResult.BadParams;
          }

          // Get branch's merchant ID using TCCTerminal
          var tcc = unitOfWork.Query<Atlas.Domain.Model.TCCTerminal>()
            .FirstOrDefault(s => s.Branch.PadLeft(3, '0') == branch.LegacyBranchNum.PadLeft(3, '0'));
          var merchantId = tcc.MerchantId;
          
          var sb = new StringBuilder();
          // Header
          sb.AppendLine("ReportType, TranID, TTypeID, MerchantID, ActionDate, ProccessDate, ClientRef1, ClientRef2, HomingAccNo, HomingBranch, AccType, Instalment, Amount, NoInstalments, RCode, QCode");
          
          /*
            ReportType, TranID, TTypeID, MerchantID, ActionDate, ProccessDate, ClientRef1, ClientRef2, HomingAccNo, HomingBranch, AccType, Instalment, Amount, NoInstalments, RCode, QCode
            1,566,19,11109,20121001  ,20121004  ,000000000941674          ,1812500000003980         ,62354925805,250655,1,13,157.68,,2,
            1,667,19,11109,20121001  ,20121004  ,000000000941674          ,1829500000004025         ,62358003714,250655,1,10,248.44,,2,           
          */
          #region Successful          
          var successful = unitOfWork.Query<NAEDOReportSuccess>().Where(s =>
            s.ReportSuccess.ProcessMerchant == merchantId &&              
            s.ReplyDT >= startDate && s.ReplyDT < endDate.AddDays(1))
            .OrderBy(s => s.ReplyDT);

          foreach (var success in successful)
          {
            //ReportType - 0 = '1- Success', TranID - 1, TTypeID - 2, MerchantID - 3, ActionDate - 4, ProccessDate - 5, ClientRef1 - 6, 
            // ClientRef2 - 7, HomingAccNo - 8, HomingBranch - 9, AccType - 10, Instalment - 11, Amount - 12, 
            // NoInstalments - 13, RCode - 14, QCode - 15
            Int16 instalmentCurr;
            Int16 instalmentCount;
            if (Atlas.Server.WCF.Implementation.ASS.ASSUtils.GetXOfY(success.NumInstallments, out instalmentCurr, out instalmentCount))
            {
              sb.AppendFormat("{0},{1},{2},{3},{4:yyyyMMdd}  ,{5:yyyyMMdd}  ,{6},{7},{8},{9},{10},{11},{12:F2},{13},{14},{15}\r\n",
                /*0*/"4", // 1- Successful transactions
                /*1*/success.ReportSuccess.TransactionId,
                /*2*/success.ReportSuccess.TransactionTypeId,
                /*3*/success.ReportSuccess.ProcessMerchant,
                /*4*/success.ReportSuccess.ActionDT,
                /*5*/success.ReplyDT,
                /*6*/success.ReportSuccess.ClientRef1.PadRight(25, ' '),
                /*7*/success.ReportSuccess.ClientRef2.PadRight(25, ' '),
                /*8*/"",   /* Not available */
                /*9*/"",   /* Not available */
                /*10*/"",  /* Not available */
                /*11*/instalmentCurr,
                /*12*/success.Amount,
                /*13*/instalmentCount,
                /*14*/"", // Disputes only- RCode- 30/32/36 - disputes codes- cancelled and Disputes put together
                /**/""    /* Disputes only- RCode  */);
            }
          }
          #endregion

          #region Disputed
          var disputes = unitOfWork.Query<NAEDOReportDisputed>().Where(s =>            
              s.ReportDisputed.ProcessMerchant == merchantId &&
            s.ReplyDT >= startDate && s.ReplyDT < endDate.AddDays(1));

          foreach (var dispute in disputes)
          {
            Int16 instalmentCurr;
            Int16 instalmentCount;
            if (Atlas.Server.WCF.Implementation.ASS.ASSUtils.GetXOfY(dispute.NumInstallments, out instalmentCurr, out instalmentCount))
            {
              //ReportType - 0 = '1- Success', TranID - 1, TTypeID - 2, MerchantID - 3, ActionDate - 4, ProccessDate - 5, ClientRef1 - 6, 
              // ClientRef2 - 7, HomingAccNo - 8, HomingBranch - 9, AccType 10, Instalment - 11, Amount - 12, 
              // NoInstalments - 13, RCode - 14, QCode - 15
              sb.AppendFormat("{0},{1},{2},{3},{4:yyyyMMdd}  ,{5:yyyyMMdd}  ,{6},{7},{8},{9},{10},{11},{12:F2},{13},{14},{15}\r\n",
                /*0*/"7", // 7- Disputed transaction
                /*1*/dispute.ReportDisputed.TransactionId,
                /*2*/dispute.ReportDisputed.TransactionTypeId,
                /*3*/dispute.ReportDisputed.ProcessMerchant,
                /*4*/dispute.ReportDisputed.ActionDT,
                /*5*/dispute.ReplyDT,
                /*6*/dispute.ReportDisputed.ClientRef1.PadRight(25, ' '),
                /*7*/dispute.ReportDisputed.ClientRef2.PadRight(25, ' '),
                /*8*/"",   /* Not available */
                /*9*/dispute.HomingBranch,
                /*10*/"",  /* Not available */
                /*11*/ instalmentCurr,
                /*12*/dispute.Amount,
                /*13*/instalmentCount,
                /*14*/dispute.RCode,
                /**/dispute.QCode);
            }
          }
          #endregion

          file = sb.ToString();
        }

        return (int)General.WCFCallResult.OK;

      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        errorMessage = "Unexpected server error";
        return (int)General.WCFCallResult.ServerError;
      }
    }

  }
}
