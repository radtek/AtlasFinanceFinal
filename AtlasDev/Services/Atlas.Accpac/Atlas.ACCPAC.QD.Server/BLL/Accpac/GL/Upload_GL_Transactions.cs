using System;
using System.Collections.Generic;
using System.Linq;
using BLL.Accpac.CB;

using Serilog;

using BLL.Accpac.GL;
using BLL.CustomerSpecific.Atlas.GL;
using BusinessLogicLayer.Accpac;
using BusinessLogicLayer.DAL;
using WinServices.BLL;
using Atlas.ACCPAC.BLL.Accpac.Atlas;


namespace Atlas.ACCPAC.BLL.Accpac.GL
{
  public class Upload_GL_Transactions
  {
    private const string SOURCECURRENCY = "ZAR";
    private SQL sql;

    public List<ASS_DailyUpload_Header> _assDailyUpload_Headers;
    public List<BranchToLegalEntity> _branchToLegalEntities;

    public string Status { get; set; }


    public void StartProcess()
    {
      var currSection = "Starting";
      try
      {
        currSection = "ProcessAssFiles";

        #region Process EOD files (import DBF into temp tables)
        SharedMembers.ProcessAssFiles();
        #endregion

        currSection = "Transform data to GL transaction";
        #region (2) Transform Data to GL Transaction
        sql = new SQL();

        var iErr = "";
        _branchToLegalEntities = SQL.Get_Atlas_BranchToLegalEntity();
        _assDailyUpload_Headers = SQL.Get_Atlas_GL_Get_ASS_DailyUpload_Headers(0);

        var err = "";

        if (_assDailyUpload_Headers != null)
        {
          if (_assDailyUpload_Headers.Count > 0)
          {
            // Transform to GL Header
            foreach (var h in _assDailyUpload_Headers)
            {
              _log.Information("STARTING: Branch: {0}, Added: {1}", h.BranchCode, h.TransactionDate);
              iErr = "";
              iErr = TransformLoanTransToGLTrans(h);
              if (iErr.Trim() != "")
              {
                _log.Information("FAILED: Branch: {0}, Added: {1}, Error: {2}", h.BranchCode, h.TransactionDate, iErr);
                err += iErr;
              }
              else
              {
                _log.Information("COMPLETED: Branch: {0}, Added: {1}", h.BranchCode, h.TransactionDate);
              }
            }
          }
        }
        #endregion

        currSection = "Process GL journals to Accpac";
        #region (3) Process GL Journals to Accpac
        currSection = "sql.Get_Accpac_GL_Get_JournalHeaders(0)";
        var MyJournalHeader_ToGL = new List<JournalHeader>();
        if ((MyJournalHeader_ToGL = sql.Get_Accpac_GL_Get_JournalHeaders(0)) != null)
        {
          if (MyJournalHeader_ToGL.Count > 0)
          {
            currSection = "new AccountingSession()";
            var acc = new AccountingSession();
            var AccErr = "";
            var AccItemErr = "";

            // Open Accpac Session
            AccErr = "";
            currSection = "acc.OpenSession()";
            AccErr = acc.OpenSession();
            if (AccErr == null)
            {
              _log.Information("Error on StartProcess for Process GL Journals to Accpac: Accpac Session: {0}", AccErr);
            }

            if (!string.IsNullOrEmpty(AccErr))
            {
              AccErr = "Null val returned from Accpac.";
            }
            else
            {
              AccItemErr = "";
              currSection = "acc.GL_Journal_Batch(MyJournalHeader_ToGL)";
              _log.Information("ACCPAC posting started");
              AccItemErr = acc.GL_Journal_Batch(MyJournalHeader_ToGL);
              _log.Information("ACCPAC posting completed");
            }

            currSection = "acc.Session.Close()";
            if (acc != null)
            {
              acc.Session.Close();
              acc = null;
            }
          }
        }
        #endregion

        Status = "Ready";
      }
      catch (Exception ex)
      {
        _log.Error(ex, "StartProcess: Section: {0}", currSection);
        Status = "Error";
      }
    }


    public string TransformLoanTransToGLTrans(ASS_DailyUpload_Header header)
    {
      JournalHeader jh;
      GLJournalDetail jd;
      FiscalPeriod fp;
      try
      {
        var rVal = "";

        if (header == null)
          return "Null Header Object passed";

        if (header.myASS_DailyUpload_Detail == null)
          return "Null Detail Object passed";

        if (header.myASS_DailyUpload_Detail.Count == 0)
          return "No Detail Object passed";

        var branchCode = ASSBranchCodeToGL(header.BranchCode).Trim().Substring(0, 3);

        #region fiscal period

        fp = SQL.Get_Accpac_GL_FiscalPeriod(header.TransactionDate);
        if (fp == null)
        {
          header.ProcessedInd = 2;
          header.ProcessedDate = DateTime.Now;
          rVal = string.Format("Invalid Fiscal period for Branch: {0} and Trans Date: {1}", branchCode, header.TransactionDate);
          header.Error = rVal;
          Atlas_GL_ASS_DailyUpload_Header_Update(ref header);
          return rVal;
        }
        var period = "";
        period = fp.Period.ToString("D2");
        #endregion

        #region Journal header
        jh = new JournalHeader()
        {
          AutoRefersal = 0,
          DateCreated = DateTime.Now,
          Description = string.Format("BR: {0} Date: {1}", branchCode, header.TransactionDate),
          EntryDate = header.TransactionDate,
          EntryNr = "0",
          Error = "",

          FiscalPeriod = period,
          FiscalYear = fp.FiscalYear,
          InternalBatchID = 0,
          ProcessedInd = 0,
          SourceDescription = "Loan System Auto Load",
          SourceLedger = "GL",
          SourceReference = string.Format("Branch: {0} and Trans Date: {1}", branchCode, header.TransactionDate),
          SourceType = "LS"
        };
        #endregion

        var ELY_Net = 0m;
        var IL_Net = 0m;
        var CL_Net = 0m;
        var HO_Net = 0m;
        var DJNL_Net = 0m;
        var CJNL_Net = 0m;
        var WO_Net = 0m;
        var SCD_Net = 0m;

        var procInd = 1;
        var procErr = "";
        var rErr = "";

        var jdSeq = 0;

        #region Transform each GL line
        foreach (var d in header.myASS_DailyUpload_Detail)
        {
          jd = TransformToGLJournalDetail(d, ref jh, ref jdSeq, ref ELY_Net, ref IL_Net, ref CL_Net, ref HO_Net, ref DJNL_Net, ref CJNL_Net, ref WO_Net, ref SCD_Net);

          if (jd.ProcessedInd == 0)
          {
            d.ProcessedInd = 1;
            d.Error = "";
          }
          else
          {
            d.ProcessedInd = jd.ProcessedInd;
            d.Error = jd.Error;

            _log.Error("TransformToGLJournalDetail: {Error}", jd.Error);
            // Header
            procInd = 2;
            procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, jd.Error.Trim()) : jd.Error.Trim();
          }

          d.ProcessedDate = DateTime.Now;
        }
        #endregion

        // Add Net Amt's - Trans Type 19
        var bPostFix = GetBranchGL_PostFix(header.BranchCode.Trim());
        if (bPostFix.Trim() == "")
        {
          procInd = 2;
          rErr = string.Format("(Net Amt) Invalid Branch Code. No GL accounts exist for branch {0}- loaded {1}", header.BranchCode.Trim(), _branchToLegalEntities.Count);
          procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rErr.Trim()) : rErr.Trim();
        }

        var rVal55 = new GLJournalDetail();
        AddJournalDetail(ref jh, ref jdSeq,
            string.Format("1220{0}", bPostFix.Trim()),
            string.Format("{0}: {1}", header.BranchCode.Trim(), header.TransactionDate),
           "19: NET - ELY", ELY_Net * -1, ref rVal55);
        if (rVal55.ProcessedInd == 2)
        {
          procInd = 2;
          rErr = string.Format("ELY NET: {0}", rVal55.Error.Trim());
          procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rErr.Trim()) : rErr.Trim();
        }


        #region Issue Loan NET
        rVal55 = new GLJournalDetail();
        AddJournalDetail(ref jh, ref jdSeq,
            string.Format("1220{0}", bPostFix.Trim()),
            string.Format("{0}: {1}", header.BranchCode.Trim(), header.TransactionDate),
            "19: NET - Issue Loan", IL_Net * -1, ref rVal55);
        if (rVal55.ProcessedInd == 2)
        {
          procInd = 2;
          rErr = string.Format("Issue Loan NET: {0}", rVal55.Error.Trim());
          procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rErr.Trim()) : rErr.Trim();
        }
        #endregion


        #region Cancel Loan NET
        rVal55 = new GLJournalDetail();
        AddJournalDetail(ref jh, ref jdSeq,
            string.Format("1220{0}", bPostFix.Trim()),
            string.Format("{0}: {1}", header.BranchCode.Trim(), header.TransactionDate),
            "19: NET - Cancel Loan", CL_Net * -1, ref rVal55);
        if (rVal55.ProcessedInd == 2)
        {
          procInd = 2;
          rErr = string.Format("Cancel Loan NET: {0}", rVal55.Error.Trim());
          procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rErr.Trim()) : rErr.Trim();
        }
        #endregion


        #region Hand-Over NET
        rVal55 = new GLJournalDetail();
        AddJournalDetail(ref jh, ref jdSeq,
            string.Format("1220{0}", bPostFix.Trim()),
            string.Format("{0}: {1}", header.BranchCode.Trim(), header.TransactionDate),
            "19: NET - Hand-Over", HO_Net * -1, ref rVal55);
        if (rVal55.ProcessedInd == 2)
        {
          procInd = 2;
          rErr = string.Format("Hand-Over NET: {0}", rVal55.Error.Trim());
          procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rErr.Trim()) : rErr.Trim();
        }
        #endregion

        //#region SDC
        //
        //         Minda said to remove --  2015-04-10
        //
        //rVal55 = new GLJournalDetail();
        //AddJournalDetail(ref jh, ref jdSeq,
        //    string.Format("1220-{0}-3", ASSBranchCodeToGL(header.BranchCode)),
        //    string.Format("{0}: {1}", header.BranchCode.Trim(), header.TransactionDate),
        //    "19: NET - Hand-Over(SDC Take-On) ", SetToPositive(HO_Net), ref rVal55);
        //if (rVal55.ProcessedInd == 2)
        //{
        //  procInd = 2;
        //  rErr = string.Format("SDC(1): {0}", rVal55.Error.Trim());
        //  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rErr.Trim()) : rErr.Trim();
        //}

        //rVal55 = new GLJournalDetail();
        //AddJournalDetail(ref jh, ref jdSeq,
        //    string.Format("1221-{0}-3", ASSBranchCodeToGL(header.BranchCode)),
        //    string.Format("{0}: {1}", header.BranchCode.Trim(), header.TransactionDate),
        //    "19: NET - Hand-Over(SDC Take-On) ", SetToNegative(HO_Net), ref rVal55);
        //if (rVal55.ProcessedInd == 2)
        //{
        //  procInd = 2;
        //  rErr = string.Format("SDC(2): {0}", rVal55.Error.Trim());
        //  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rErr.Trim()) : rErr.Trim();
        //}
        //#endregion

        #region Debit Journal NET
        rVal55 = new GLJournalDetail();
        AddJournalDetail(ref jh, ref jdSeq,
            string.Format("1220{0}", bPostFix.Trim()),
            string.Format("{0}: {1}", header.BranchCode.Trim(), header.TransactionDate),
            "19: NET - Debit Journal", DJNL_Net * -1, ref rVal55);
        if (rVal55.ProcessedInd == 2)
        {
          procInd = 2;
          rErr = string.Format("Debit Journal NET: {0}", rVal55.Error.Trim());
          procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rErr.Trim()) : rErr.Trim();
        }
        #endregion


        #region Credit Journal NET
        rVal55 = new GLJournalDetail();
        AddJournalDetail(ref jh, ref jdSeq,
            string.Format("1220{0}", bPostFix.Trim()),
            string.Format("{0}: {1}", header.BranchCode.Trim(), header.TransactionDate),
            "19: NET - Credit Journal", CJNL_Net * -1, ref rVal55);
        if (rVal55.ProcessedInd == 2)
        {
          procInd = 2;
          rErr = string.Format("Credit Journal NET: {0}", rVal55.Error.Trim());
          procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rErr.Trim()) : rErr.Trim();
        }
        #endregion


        #region Write-Off NET
        rVal55 = new GLJournalDetail();
        AddJournalDetail(ref jh, ref jdSeq,
            string.Format("1220{0}", bPostFix.Trim()),
            string.Format("{0}: {1}", header.BranchCode.Trim(), header.TransactionDate),
            "19: NET - Write-Off", WO_Net * -1, ref rVal55);
        if (rVal55.ProcessedInd == 2)
        {
          procInd = 2;
          rErr = string.Format("Write-Off NET: {0}", rVal55.Error.Trim());
          procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rErr.Trim()) : rErr.Trim();
        }
        #endregion


        #region SCD Transactions NET
        rVal55 = new GLJournalDetail();
        AddJournalDetail(ref jh, ref jdSeq,
            string.Format("1220-{0}-3", ASSBranchCodeToGL(header.BranchCode)),
            string.Format("{0}: {1}", header.BranchCode.Trim(), header.TransactionDate),
            "21: NET - SCD Transactions ", SCD_Net, ref rVal55);
        if (rVal55.ProcessedInd == 2)
        {
          procInd = 2;
          rErr = string.Format("SCD Transactions NET: {0}", rVal55.Error.Trim());
          procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rErr.Trim()) : rErr.Trim();
        }
        #endregion


        #region Add Error Results to Journal Header
        jh.Error = procErr;
        jh.ProcessedInd = (Int16)(procInd == 2 ? 2 : 0);

        jh.ProcessedDate = DateTime.Now;

        rVal = "";
        rVal = GLJournalHeader_Insert(jh);  // Insert header with details...
        if (rVal != "")
        {
          procInd = 2;
          rErr = rVal;
          procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rErr.Trim()) : rErr.Trim();
        }

        header.ProcessedInd = procInd;
        header.ProcessedDate = DateTime.Now;
        header.Error = procErr;

        rVal = "";
        Atlas_GL_ASS_DailyUpload_Header_Update(ref header);
        if (rVal != "")
        {
          procInd = 2;
          rErr = rVal;
          procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rErr.Trim()) : rErr.Trim();
        }
        #endregion

        return procErr;
      }

      catch (Exception ex)
      {
        _log.Error(ex, "TransformLoanTransToGLTrans");
        return ex.Message;
      }
    }


    private string AddJournalDetail(ref JournalHeader jh, ref int jdSeq, string accountNr, string reference, string description, decimal sourceCurrencyAmt,
        ref GLJournalDetail glJournalDetail)
    {
      try
      {
        jdSeq += 1;
        var err = "";
        var rVal = new GLJournalDetail();

        if (sourceCurrencyAmt == 0)
        {
          rVal.ProcessedInd = 0;
          rVal.Error = "";
          return "";
        }

        accountNr = ASSBranchCodeToGL(accountNr);

        rVal.TransactionNr = GetNextTransNr(jdSeq);

        rVal.AccountNr = accountNr.Trim();
        rVal.Reference = reference.Trim();
        rVal.Description = description;
        rVal.SourceCurrency = SOURCECURRENCY;
        rVal.SourceCurrencyAmt = sourceCurrencyAmt;
        rVal.ProcessedInd = 0;
        rVal.Error = "";

        // Check GL Account
        if (sql.Check_Accpac_IsValid_GL_Account(accountNr.Trim().Replace("-", ""), accountNr.Trim()) == false)
        {
          err = string.Format(" GL Account: {0} is an invalid GL Account.", accountNr);

          glJournalDetail.ProcessedInd = 2;
          glJournalDetail.Error += err;
          glJournalDetail.ProcessedDate = DateTime.Now;

          rVal.ProcessedInd = 2;
          rVal.Error = err;
        }

        jh._journalDetails.Add(rVal);

        return err;
      }
      catch (Exception ex)
      {
        _log.Error(ex, "AddJournalDetail: {@Details}", new { jdSeq, accountNr, reference, description, sourceCurrencyAmt, glJournalDetail });
        glJournalDetail.ProcessedInd = 2;
        glJournalDetail.Error += string.Format(" AddJournalDetail: {0}", ex.Message);
        glJournalDetail.ProcessedDate = DateTime.Now;

        return ex.Message;
      }
    }


    private GLJournalDetail TransformToGLJournalDetail(ASS_DailyUpload_Detail d, ref JournalHeader jh, ref int jdSeq,
        ref decimal ELY_Net, ref decimal IL_Net, ref decimal CL_Net, ref decimal HO_Net, ref decimal DJNL_Net,
        ref decimal CJNL_Net, ref decimal WO_Net, ref decimal SCD_Net)
    {
      //_log.Information("TransformToGLJournalDetail: {@Detail}", d);
      var rVal = new GLJournalDetail();
      CBBank bank;
      try
      {
        Int16 procInd = 0;
        string procErr = "";
        var bankName = "";
        var bankCAcct = "";

        var tB = "";
        var oB = "";

        // Validation & Transformation
        var bPostFix = GetBranchGL_PostFix(d.BranchCode.Trim());
        if (string.IsNullOrEmpty(bPostFix))
        {
          rVal.ProcessedInd = 2;
          rVal.Error = string.Format("(TransformToGLJournalDetail 1) Invalid Branch Code. No GL accounts exist for branch {0}- loaded {1}", d.BranchCode.Trim(), _branchToLegalEntities.Count);
          rVal.ProcessedDate = DateTime.Now;

          return rVal;
        }

        var bLegalPostFix = GetBranchGL_LegalEntityPostFix(d.BranchCode.Trim());
        if (string.IsNullOrEmpty(bLegalPostFix))
        {
          rVal.ProcessedInd = 2;
          rVal.Error = string.Format("(TransformToGLJournalDetail 2) Invalid Branch Code. No GL accounts exist for branch {0}- loaded {1}}", d.BranchCode.Trim(), _branchToLegalEntities.Count);
          rVal.ProcessedDate = DateTime.Now;

          return rVal;
        }

        switch (d.TransType.Trim())
        {
          #region 05- Loans Paid out of COH

          case "05": // Loans Paid out of COH

            procInd = 0;
            procErr = "";

            AddJournalDetail(ref jh, ref jdSeq,
                string.Format("1315{0}", bPostFix.Trim()),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "05: Loan, COH", SetToNegative(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq,
                string.Format("1220{0}", bPostFix.Trim()),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "05: Loan, COH", SetToPositive(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 06- Loans Cancelled out of COH
          case "06": // Loans Cancelled out of COH

            procInd = 0;
            procErr = "";

            AddJournalDetail(ref jh, ref jdSeq,
                string.Format("1315{0}", bPostFix.Trim()),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "05: Loan, COH", SetToPositive(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq,
                string.Format("1220{0}", bPostFix.Trim()),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "05: Loan, COH", SetToNegative(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 07- Loans Paid out of Bank Account

          case "07": // Loans Paid out of Bank Account

            procInd = 0;
            procErr = "";

            bank = SQL.Get_Accpac_CB_GetBankDetails(d.AdditionalInfo.Trim());
            if (string.IsNullOrEmpty(bank?.BankName))
            {
              procErr = string.Format("{0} is not a valid Bank Account.", d.AdditionalInfo.Trim());
              procInd = 2;

              bankName = "Invalid Bank Account";
              bankCAcct = d.AdditionalInfo.Trim();
            }
            else
            {
              bankName = bank.BankName.Trim();
              bankCAcct = bank.GLAccount_Clearing_New(ASSBranchCodeToGL(d.BranchCode));
            }

            AddJournalDetail(ref jh, ref jdSeq, bankCAcct,
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "07: Loan paid of out of Bank Account", SetToNegative(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq,
                string.Format("1220{0}", bPostFix.Trim()),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "07: Loan paid of out of Bank Account", SetToPositive(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
            }

            // Inter-Company Loans
            tB = GetBranchLegalEntity(d.BranchCode.Trim());
            oB = bankCAcct.Substring(bankCAcct.Length - 1, 1);
            if (tB.Trim() != oB.Trim())
            {
              Record_InterCompany_Loans(tB, oB, ref jh, ref jdSeq, ref d, ref procInd, ref procErr);
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 08- Bank account loans cancelled

          case "08": // Loans Paid out of Bank Account

            procInd = 0;
            procErr = "";

            bank = SQL.Get_Accpac_CB_GetBankDetails(d.AdditionalInfo.Trim());
            if (string.IsNullOrEmpty(bank?.BankName))
            {
              procErr = string.Format("{0} is not a valid Bank Account.", d.AdditionalInfo.Trim());
              procInd = 2;

              bankName = "Invalid Bank Account";
              bankCAcct = d.AdditionalInfo.Trim();
            }
            else
            {
              bankName = bank.BankName.Trim();
              bankCAcct = bank.GLAccount_Clearing_New(ASSBranchCodeToGL(d.BranchCode, 3));
            }

            AddJournalDetail(ref jh, ref jdSeq,
                string.Format("1220{0}", bPostFix.Trim()),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "08: Loan cancelled paid into Bank Account", SetToNegative(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq, bankCAcct,
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "08: Loan cancelled paid into Bank Account", SetToPositive(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
            }


            // Inter-Company Loans
            tB = GetBranchLegalEntity(d.BranchCode.Trim());
            oB = bankCAcct.Substring(bankCAcct.Length - 1, 1);
            if (tB.Trim() != oB.Trim())
            {
              Record_InterCompany_Loans(oB, tB, ref jh, ref jdSeq, ref d, ref procInd, ref procErr);
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 01- Receipts != Legal

          case "01": // Receipts != Legal

            procInd = 0;
            procErr = "";
            var reversal01 = d.TransAmt < 0 ? " Reversal" : "";

            if (d.AdditionalInfo.Trim() == "CASH")
            {
              AddJournalDetail(ref jh, ref jdSeq,
                  string.Format("1315{0}", bPostFix.Trim()),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("01: Cash Receipt{0}", reversal01), d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq,
                  string.Format("1220{0}", bPostFix.Trim()),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("01: Cash Receipt{0}", reversal01), d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;
            }
            else  // Bank Deposits
            {
              bank = SQL.Get_Accpac_CB_GetBankDetails(d.AdditionalInfo.Trim());
              if (string.IsNullOrEmpty(bank?.BankName))
              {
                procErr = string.Format("{0} is not a valid Bank Account.", d.AdditionalInfo.Trim());
                procInd = 2;

                bankName = "Invalid Bank Account";
                bankCAcct = d.AdditionalInfo.Trim();
              }
              else
              {
                bankName = bank.BankName.Trim();
                bankCAcct = bank.GLAccount_Clearing_New(ASSBranchCodeToGL(d.BranchCode, 3));
              }

              AddJournalDetail(ref jh, ref jdSeq, bankCAcct,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("01: Bank Receipt{0} ({1})", reversal01, SharedMembers.StripValues(d.Description.TrimStart(), "RECEIPTS-CURRENT", "CUR/DEPT").Trim()),
                  d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq,
                  string.Format("1220{0}", bPostFix.Trim()),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("01: Bank Receipt{0} ({1})", reversal01, SharedMembers.StripValues(d.Description.TrimStart(), "RECEIPTS-CURRENT", "CUR/DEPT").Trim()),
                  d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              // Inter-Company Loans
              tB = bankCAcct.Substring(bankCAcct.Length - 1, 1);
              oB = GetBranchLegalEntity(d.BranchCode.Trim());
              if (tB.Trim() != oB.Trim())
              {
                Record_InterCompany_Loans(tB, oB, ref jh, ref jdSeq, ref d, ref procInd, ref procErr);
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;
            }

            break;

          #endregion


          #region 03- Receipts == Legal

          case "03": // Receipts == Legal

            procInd = 0;
            procErr = "";

            var reversal03 = d.TransAmt < 0 ? " Reversal" : "";

            // Special case: Legal - actual branch is at end of description: RECEIPTS-LEGAL SDC LEG/DEP: [03]
            // Can also be: [03/111/123]   (Branch, client, loan) or {03/111/123}
            var posStart03 = d.Description.IndexOf('[');
            if (posStart03 < 0)
            {
              posStart03 = d.Description.IndexOf('{');
            }

            var posEnd03 = d.Description.IndexOf('/', posStart03);
            if (posEnd03 == -1)
            {
              posEnd03 = d.Description.IndexOf(']', posStart03);
            }
            if (posEnd03 < 0)
            {
              posEnd03 = d.Description.IndexOf('}', posStart03);
            }

            if (d.Description.IndexOf("LEGAL") > -1 && posStart03 > -1 && posEnd03 > posStart03)
            {
              d.BranchCode = d.Description.Substring(posStart03 + 1, posEnd03 - posStart03 - 1).Trim().PadLeft(3, '0');

              // Ignore sugarcell
              if (d.BranchCode == "0C5")
              {
                rVal.ProcessedInd = 0;
                rVal.Error = "Sugarcell not valid. No GL accounts exist- skipping.";
                rVal.ProcessedDate = DateTime.Now;

                return rVal;
              }

              bPostFix = GetBranchGL_PostFix(d.BranchCode);
              if (bPostFix.Trim() == "")
              {
                rVal.ProcessedInd = 2;
                rVal.Error = string.Format("Invalid Branch Code. No GL accounts exist for branch {0}- loaded {1}", d.BranchCode, _branchToLegalEntities.Count);
                rVal.ProcessedDate = DateTime.Now;

                return rVal;
              }
            }

            if (d.AdditionalInfo.Trim() == "CASH")
            {
              AddJournalDetail(ref jh, ref jdSeq, string.Format("1315{0}", bPostFix.Trim()),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("03: Cash Legal Receipt{0}", reversal03), d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq, string.Format("0185{0}", bPostFix.Trim()),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("03: Cash Legal Receipt{0}", reversal03), d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              /* Celia request removal- e-mail: 14 April 2015 at 16:42 - "AccPac Update from QD's (remove control account double entry)"
              // SDC
              AddJournalDetail(ref jh, ref jdSeq,
                  string.Format("1221-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("03: Cash Legal Receipt{0}: {1}", reversal03, d.TransAmt < 0 ? "Cr" : "Dr"), d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq,
                  string.Format("1220-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("03: Cash Legal Receipt{0}: {1}", reversal03, d.TransAmt < 0 ? "Dr" : "Cr"), d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }
              */

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;

            }
            else  // Bank Deposits
            {
              bank = SQL.Get_Accpac_CB_GetBankDetails(d.AdditionalInfo.Trim());
              if (string.IsNullOrEmpty(bank?.BankName))
              {
                procErr = string.Format("{0} is not a valid Bank Account.", d.AdditionalInfo.Trim());
                procInd = 2;

                bankName = "Invalid Bank Account";
                bankCAcct = d.AdditionalInfo.Trim();
              }
              else
              {
                bankName = bank.BankName.Trim();
                bankCAcct = bank.GLAccount_Clearing_New(ASSBranchCodeToGL(d.BranchCode, 3));
              }

              AddJournalDetail(ref jh, ref jdSeq, bankCAcct,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("03: Bank Legal Receipt{0} ({1})", reversal03,
                  SharedMembers.StripValues(d.Description.TrimStart(), "RECEIPTS-LEGAL", "LEG/D").Trim()),
                  d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              // BD recovered
              AddJournalDetail(ref jh, ref jdSeq, string.Format("0185{0}", bPostFix.Trim()),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("03: Bank Legal Receipt{0} ({1})", reversal03, SharedMembers.StripValues(d.Description.TrimStart(), "RECEIPTS-LEGAL", "LEG/D").Trim()),
                  d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }


              /* Celia request removal- e-mail: 14 April 2015 at 16:42 - "AccPac Update from QD's (remove control account double entry)"
              // SDC - Hand-over clearing
              AddJournalDetail(ref jh, ref jdSeq, string.Format("1221-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("03: Bank Legal Receipt{0} ({1}): {2}", reversal03, SharedMembers.StripValues(d.Description.TrimStart(), "RECEIPTS-LEGAL", "LEG/D").Trim(), d.TransAmt < 0 ? "Cr" : "Dr"),
                  d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              // Debtors
              AddJournalDetail(ref jh, ref jdSeq, string.Format("1220-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("03: Bank Legal Receipt{0} ({1}): {2}", reversal03, SharedMembers.StripValues(d.Description.TrimStart(), "RECEIPTS-LEGAL", "LEG/D").Trim(), d.TransAmt < 0 ? "Dr" : "Cr"),
                  d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }
              */

              // Inter-Company Loans
              tB = bankCAcct.Substring(bankCAcct.Length - 1, 1);
              oB = GetBranchLegalEntity(d.BranchCode.Trim());
              if (tB.Trim() != oB.Trim())
              {
                Record_InterCompany_Loans(tB, oB, ref jh, ref jdSeq, ref d, ref procInd, ref procErr);
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;
            }

            break;

          #endregion


          #region 04- Refunds == Legal

          case "04": // Refunds == Legal

            procInd = 0;
            procErr = "";

            // ASS Default is negative
            var reversal04 = d.TransAmt > 0 ? " Reversal" : "";

            // Special case: Legal - actual branch is at end of description: RECEIPTS-LEGAL SDC LEG/DEP: [03]
            var posStart04 = d.Description.IndexOf('[');
            if (posStart04 < 0)
            {
              posStart04 = d.Description.IndexOf('{');
            }
            var posEnd04 = d.Description.IndexOf('/', posStart04);
            if (posEnd04 == -1)
            {
              posEnd04 = d.Description.IndexOf(']', posStart04);
              if (posEnd04 < 0)
              {
                posEnd04 = d.Description.IndexOf('}', posStart04);
              }
            }

            if (d.Description.IndexOf("LEGAL") > -1 && posStart04 > -1 && posEnd04 > posStart04)
            {
              d.BranchCode = d.Description.Substring(posStart04 + 1, posEnd04 - posStart04 - 1).Trim().PadLeft(3, '0');

              // Ignore sugarcell
              if (d.BranchCode == "0C5")
              {
                rVal.ProcessedInd = 0;
                rVal.Error = "Sugarcell not valid. No GL accounts exist- skipping.";
                rVal.ProcessedDate = DateTime.Now;

                return rVal;
              }

              bPostFix = GetBranchGL_PostFix(d.BranchCode);
              if (bPostFix.Trim() == "")
              {
                rVal.ProcessedInd = 2;
                rVal.Error = string.Format("Invalid Branch Code. No GL accounts exist for branch {0}- loaded {1}", d.BranchCode, _branchToLegalEntities.Count);
                rVal.ProcessedDate = DateTime.Now;

                return rVal;
              }
            }

            if (d.AdditionalInfo.Trim() == "CASH")
            {
              // Cash on hand
              AddJournalDetail(ref jh, ref jdSeq, string.Format("1315{0}", bPostFix.Trim()),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("04: Cash Legal Refund{0}", reversal04), d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              // Bad debts recovered
              AddJournalDetail(ref jh, ref jdSeq, string.Format("0185{0}", bPostFix.Trim()),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("04: Cash Legal Refund{0}", reversal04), d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              // SDC- Trade debtors hand-over control
              AddJournalDetail(ref jh, ref jdSeq, string.Format("1221-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("04: Cash Legal Refund{0}", reversal04), d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              // Trade debtors
              AddJournalDetail(ref jh, ref jdSeq, string.Format("1220-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("04: Cash Legal Refund{0}", reversal04), d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;

            }
            else  // Bank Deposits
            {
              bank = SQL.Get_Accpac_CB_GetBankDetails(d.AdditionalInfo.Trim());
              if (string.IsNullOrEmpty(bank?.BankName))
              {
                procErr = string.Format("{0} is not a valid Bank Account.", d.AdditionalInfo.Trim());
                procInd = 2;

                bankName = "Invalid Bank Account";
                bankCAcct = d.AdditionalInfo.Trim();
              }
              else
              {
                bankName = bank.BankName.Trim();
                bankCAcct = bank.GLAccount_Clearing_New(ASSBranchCodeToGL(d.BranchCode, 3));
              }

              AddJournalDetail(ref jh, ref jdSeq, bankCAcct,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("04: Bank Legal Refund{0} ({1})", reversal04, SharedMembers.StripValues(d.Description.TrimStart(), "REFUNDS-LEGAL", "LEG/DE")),
                  d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              // Bad debts recovered
              AddJournalDetail(ref jh, ref jdSeq, string.Format("0185{0}", bPostFix.Trim()),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("04: Bank Legal Refund{0} ({1}): {2}", reversal04, SharedMembers.StripValues(d.Description.TrimStart(), "REFUNDS-LEGAL", "LEG/DE"),
                  d.TransAmt < 0 ? "Dr" : "Cr"), d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              // SDC- Trade debtors hand-over control
              /* Minda e-mail 23 Nov 2015 - these entries are superfluous
              AddJournalDetail(ref jh, ref jdSeq, string.Format("1221-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("04: Bank Legal Refund{0} ({1}): {2}", reversal04, SharedMembers.StripValues(d.Description.TrimStart(), "REFUNDS-LEGAL", "LEG/DE"),
                  d.TransAmt < 0 ? "Cr" : "Dr"), d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              // Trade debtors control
              AddJournalDetail(ref jh, ref jdSeq, string.Format("1220-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("04: Bank Legal Refund{0} ({1}): {2}", reversal04, SharedMembers.StripValues(d.Description.TrimStart(), "REFUNDS-LEGAL", "LEG/D"),
                  d.TransAmt < 0 ? "Cr" : "Dr"), d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }
              */

              // Inter-Company Loans
              tB = GetBranchLegalEntity(d.BranchCode.Trim());
              oB = bankCAcct.Substring(bankCAcct.Length - 1, 1);
              if (tB.Trim() != oB.Trim())
              {
                d.TransAmt = d.TransAmt * -1;
                Record_InterCompany_Loans(tB, oB, ref jh, ref jdSeq, ref d, ref procInd, ref procErr);
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;
            }

            break;

          #endregion


          #region 02- Refund != Legal

          case "02": // Refund != Legal

            procInd = 0;
            procErr = "";

            // Default value is negative, if positive indicates a reversal
            var reversal02 = d.TransAmt > 0 ? "Reversal" : "";

            if (d.AdditionalInfo.Trim() == "CASH")
            {
              // Cash on hand
              AddJournalDetail(ref jh, ref jdSeq, string.Format("1315{0}", bPostFix.Trim()),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("02: Cash Refund{0}", reversal02), d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              // Trade debtors controls
              AddJournalDetail(ref jh, ref jdSeq, string.Format("1220{0}", bPostFix.Trim()),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("02: Cash Refund{0}", reversal02), d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;
            }
            else  // Bank Deposits
            {
              bank = SQL.Get_Accpac_CB_GetBankDetails(d.AdditionalInfo.Trim());
              if (string.IsNullOrEmpty(bank?.BankName))
              {
                procErr = string.Format("{0} is not a valid Bank Account.", d.AdditionalInfo.Trim());
                procInd = 2;

                bankName = "Invalid Bank Account";
                bankCAcct = d.AdditionalInfo.Trim();
              }
              else
              {
                bankName = bank.BankName.Trim();
                bankCAcct = bank.GLAccount_Clearing_New(ASSBranchCodeToGL(d.BranchCode, 3));
              }

              AddJournalDetail(ref jh, ref jdSeq, bankCAcct,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("02: Bank Refund{0} ({1})", reversal02, SharedMembers.StripValues(d.Description.TrimStart(), "REFUNDS-CURRENT", "CUR/DEPT")),
                  d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              // Trade debtors control
              AddJournalDetail(ref jh, ref jdSeq,
                  string.Format("1220{0}", bPostFix.Trim()), string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("02: Bank Refund ({0})", SharedMembers.StripValues(d.Description.TrimStart(), "REFUNDS-CURRENT", "CUR/DEPT")),
                  d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              // Inter-Company Loans
              tB = GetBranchLegalEntity(d.BranchCode.Trim());
              oB = bankCAcct.Substring(bankCAcct.Length - 1, 1);
              if (tB.Trim() != oB.Trim())
              {
                d.TransAmt = d.TransAmt * -1;
                Record_InterCompany_Loans(tB, oB, ref jh, ref jdSeq, ref d, ref procInd, ref procErr);
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;
            }

            break;

          #endregion


          #region 10- Branch Expenses

          case "10": // Branch Expenses

            var beAcct1 = ASSBranchCodeToGL(d.AdditionalInfo.Trim());
            var beAcct = "";

            // Default value is positive, if negative indicates a reversal
            var reversal10 = d.TransAmt < 0 ? "Reversal" : "";

            procInd = 0;
            procErr = "";

            if (beAcct1.Trim().Length > 7)
            {
              switch (beAcct1.Substring(0, 8))
              {
                case "0540-FUE":
                case "0540-REP":
                  beAcct = string.Format("{0}-UNKN{1}", beAcct1.Substring(0, 8), bPostFix.Trim());
                  break;
              }
            }

            if (beAcct1.Trim().Length > 3)
            {
              switch (beAcct1.Substring(0, 4))
              {
                case "0305":
                case "0535":
                case "0545":
                  beAcct = string.Format("{0}-UNKN{1}", beAcct1.Substring(0, 4), bPostFix.Trim());
                  break;
              }
            }

            if (beAcct.Trim() == "")
              beAcct = beAcct1;

            // Cash on hand
            AddJournalDetail(ref jh, ref jdSeq,
                string.Format("1315{0}", bPostFix.Trim()), string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                string.Format("10: {0}{1}", d.Description.Trim(), reversal10), d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            // Expenses
            AddJournalDetail(ref jh, ref jdSeq, beAcct,
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                string.Format("10: {0}{1}", d.Description.Trim(), reversal10), d.TransAmt, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 19- Loans Transactions (large)

          case "19": // Loans Transactions

            procInd = 0;
            procErr = "";

            var addInfo = GetTextInsideCurlies(d.Description);
            var baseDesc = GetTextBeforeCurlyOpen(d.Description);

            switch (baseDesc)
            {
              case "CELL CAPTIVE PREM VAT":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: Uncol. Prem Vat: {0}", addInfo), d.TransAmt, ref rVal);
                IL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;

              case "CELL CAPTIVE POLI VAT":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: Uncol. Poli Vat: {0}", addInfo), d.TransAmt, ref rVal);
                IL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE PREM VAT WOF":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: WOF-Uncol. Prem Vat: {0}", addInfo), d.TransAmt, ref rVal);
                WO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE POLI VAT WOF":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: WOF-Uncol. Pol Vat: {0}", addInfo), d.TransAmt, ref rVal);
                WO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE PREM VAT CAN":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: CAN-Uncol. Prem Vat: {0}", addInfo), d.TransAmt, ref rVal);
                CL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE POLI VAT CAN":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: CAN-Uncol. Pol Vat: {0}", addInfo), d.TransAmt, ref rVal);
                CL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE PREM VAT JND":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JND-Uncol. Prem Vat: {0}", addInfo), d.TransAmt, ref rVal);
                DJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE POLI VAT JND":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JND-Uncol. Pol Vat: {0}", addInfo), d.TransAmt, ref rVal);
                DJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE PREM VAT JNC":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JNC-Uncol. Prem Vat: {0}", addInfo), d.TransAmt, ref rVal);
                CJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE POLI VAT JNC":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JNC-Uncol. Pol Vat: {0}", addInfo), d.TransAmt, ref rVal);
                CJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE PREM VAT ELY":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: ELY-Uncol. Prem Vat: {0}{1}", d.TransAmt >= 0 ? "Dr" : "Cr", addInfo), d.TransAmt, ref rVal);
                ELY_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE POLI VAT ELY":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: ELY-Uncol. Pol Vat: {0}{1}", d.TransAmt >= 0 ? "Dr" : "Cr", addInfo), d.TransAmt, ref rVal);
                ELY_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE PREM VAT HOV":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: HOV-Uncol. Prem Vat: {0}", addInfo), d.TransAmt, ref rVal);
                HO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE POLI VAT HOV":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: HOV-Uncol. Pol Vat: {0}", addInfo), d.TransAmt, ref rVal);
                HO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              // Detail Early Settlement Discount
              case "CELL CAPTIVE POLICY FEE ELY":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    string.Format("19: ELY-Policy Fee: {0}{1}", d.TransAmt >= 0 ? "Dr" : "Cr", addInfo), d.TransAmt, ref rVal);
                ELY_Net += d.TransAmt;

                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE PREMIUM FEE ELY":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                     string.Format("19: ELY-Uncol. Prem: {0}{1}", d.TransAmt >= 0 ? "Dr" : "Cr", addInfo), d.TransAmt, ref rVal);
                ELY_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}{1}", procErr, string.Format("  {0}", rVal.Error.Trim())) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "COLLECT FEE ELY":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: ELY-Col Fee: {0}{1}", d.TransAmt >= 0 ? "Dr" : "Cr", addInfo), d.TransAmt, ref rVal);
                ELY_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "INITIATION FEE ELY":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: ELY-Initiation Fee: {0}{1}", d.TransAmt >= 0 ? "Dr" : "Cr", addInfo), d.TransAmt, ref rVal);
                ELY_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "INTEREST ELY":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: ELY-Interest: {0}{1}", d.TransAmt >= 0 ? "Dr" : "Cr", addInfo), d.TransAmt, ref rVal);
                ELY_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "SERVICE FEE ELY":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: ELY-Service Fee: {0}{1}", d.TransAmt >= 0 ? "Dr" : "Cr", addInfo), d.TransAmt, ref rVal);
                ELY_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "EARLY DISC":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                     string.Format("19: ELY-Capital: {0}{1}", d.TransAmt >= 0 ? "Dr" : "Cr", addInfo), d.TransAmt, ref rVal);
                ELY_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "VAT CONTROL ELY":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: ELY-Vat: {0}{1}", d.TransAmt >= 0 ? "Dr" : "Cr", addInfo), d.TransAmt, ref rVal);
                ELY_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              // Detail Issue Loan
              case "CELL CAPTIVE POLICY FEE":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: Policy Fee: {0}", addInfo), d.TransAmt, ref rVal);
                IL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE PREMIUM FEE":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                     string.Format("19: Uncol. Prem: {0}", addInfo), d.TransAmt, ref rVal);
                IL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "COLLECT FEE":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: Col Fee: {0}", addInfo), d.TransAmt, ref rVal);
                IL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "INITIATION FEE":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                string.Format("19: Initiation Fee: {0}", addInfo), d.TransAmt, ref rVal);
                IL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "INTEREST":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: Interest: {0}", addInfo), d.TransAmt, ref rVal);
                IL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "SERVICE FEE":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: Service Fee: Cr{0}", addInfo), d.TransAmt, ref rVal);
                IL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "VAT CONTROL INI/SER/COL FEE":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: Vat: {0}", addInfo), d.TransAmt, ref rVal);
                IL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              // Detail Cancel Loan
              case "CELL CAPTIVE POLICY FEE CAN":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: CAN-Policy Fee: {0}", addInfo), d.TransAmt, ref rVal);
                CL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE PREMIUM FEE CAN":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: CAN-Uncol. Prem: {0}", addInfo), d.TransAmt, ref rVal);
                CL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "COLLECT FEE CAN":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: CAN-Col Fee: {0}", addInfo), d.TransAmt, ref rVal);
                CL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;
                break;


              case "INITIATION FEE CAN":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: CAN-Initiation Fee: {0}", addInfo), d.TransAmt, ref rVal);
                CL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "INTEREST CAN":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                 string.Format("19: CAN-Interest: {0}", addInfo), d.TransAmt, ref rVal);
                CL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "SERVICE FEE CAN":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: CAN-Service Fee: {0}", addInfo), d.TransAmt, ref rVal);
                CL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "VAT CONTROL CAN":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: CAN-Vat: Dr{0}", addInfo), d.TransAmt, ref rVal);
                CL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              // Detail Hand-Over
              case "CELL CAPTIVE POLICY FEE HOV":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: HOV-Policy Fee: {0}", addInfo), d.TransAmt, ref rVal);
                HO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE PREMIUM FEE HOV":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: HOV-Uncol. Prem: {0}", addInfo), d.TransAmt, ref rVal);
                HO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "COLLECT FEE HOV":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: HOV-Col Fee: {0}", addInfo), d.TransAmt, ref rVal);
                HO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "INITIATION FEE HOV":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: HOV-Initiation Fee: {0}", addInfo), d.TransAmt, ref rVal);
                HO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "INTEREST HOV":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: HOV-Interest: {0}", addInfo), d.TransAmt, ref rVal);
                HO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "SERVICE FEE HOV":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: HOV-Service Fee: {0}", addInfo), d.TransAmt, ref rVal);
                HO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "HAND OVER":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: HOV-Capital: {0}", addInfo), d.TransAmt, ref rVal);
                HO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "VAT CONTROL HOV":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: HOV-Vat: {0}", addInfo), d.TransAmt, ref rVal);
                HO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              // Detail Debit Journal
              case "CELL CAPTIVE POLICY FEE JND":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JND-Policy Fee: {0}", addInfo), d.TransAmt, ref rVal);
                DJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE PREMIUM FEE JND":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JND-Uncol. Prem: {0}", addInfo), d.TransAmt, ref rVal);
                DJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "COLLECT FEE JND":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JND-Col Fee: {0}", addInfo), d.TransAmt, ref rVal);
                DJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "INITIATION FEE JND":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JND-Initiation Fee: {0}", addInfo), d.TransAmt, ref rVal);
                DJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "INTEREST JND":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JND-Interest: {0}", addInfo), d.TransAmt, ref rVal);
                DJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "SERVICE FEE JND":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JND-Service Fee: {0}", addInfo), d.TransAmt, ref rVal);
                DJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "LOAN DEBITS JNL":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JND-Capital: {0}", addInfo), d.TransAmt, ref rVal);
                DJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "VAT CONTROL JND":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JND-Vat: {0}", addInfo), d.TransAmt, ref rVal);
                DJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              // Detail Credit Journal
              case "CELL CAPTIVE POLICY FEE JNC":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JNC-Policy Fee: {0}", addInfo), d.TransAmt, ref rVal);
                CJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE PREMIUM FEE JNC":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JNC-Uncol. Prem: {0}", addInfo), d.TransAmt, ref rVal);
                CJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "COLLECT FEE JNC":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JNC-Col Fee: {0}", addInfo), d.TransAmt, ref rVal);
                CJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "INITIATION FEE JNC":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JNC-Initiation Fee: {0}", addInfo), d.TransAmt, ref rVal);
                CJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "INTEREST JNC":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JNC-Interest: {0}", addInfo), d.TransAmt, ref rVal);
                CJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "SERVICE FEE JNC":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JNC-Service Fee: Dr{0}", addInfo), d.TransAmt, ref rVal);
                CJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "LOAN CREDITS JNL":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JNC-Capital: {0}", addInfo), d.TransAmt, ref rVal);
                CJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "VAT CONTROL JNC":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: JNC-Vat: {0}", addInfo), d.TransAmt, ref rVal);
                CJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              // Detail Write-Off
              case "CELL CAPTIVE POLICY FEE WOF":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: WOF-Policy Fee: {0}", addInfo), d.TransAmt, ref rVal);
                WO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "CELL CAPTIVE PREMIUM FEE WOF":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: WOF-Uncol. Prem: {0}", addInfo), d.TransAmt, ref rVal);
                WO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "COLLECT FEE WOF":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: WOF-Col Fee: {0}", addInfo), d.TransAmt, ref rVal);
                WO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "INITIATION FEE WOF":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: WOF-Initiation Fee: {0}", addInfo), d.TransAmt, ref rVal);
                WO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "INTEREST WOF":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: WOF-Interest: {0}", addInfo), d.TransAmt, ref rVal);
                WO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "SERVICE FEE WOF":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: WOF-Service Fee: {0}", addInfo), d.TransAmt, ref rVal);
                WO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "WRITE OFF":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: WOF-Capital: {0}", addInfo), d.TransAmt, ref rVal);
                WO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              case "VAT CONTROL WOF":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: WOF-Vat: {0}", addInfo), d.TransAmt, ref rVal);
                WO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;
                break;


              case "REBATE JNC":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: Rebate SIM sale: {0}", addInfo), d.TransAmt, ref rVal);
                CJNL_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;

              case "NUC-FEE WOF":
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("19: WOF-NuCard Fee: {0}", addInfo), d.TransAmt, ref rVal);
                WO_Net += d.TransAmt;
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;


              default:
                rVal.Error = string.Format("Can't handle sortkey 19 with type: '{0}'", baseDesc);
                rVal.ProcessedInd = 2;

                break;
            }

            break;

          #endregion


          #region 11- Branch Income

          case "11": // Branch Income

            procInd = 0;
            procErr = "";

            /*
            amt = decimal.Round(d.TransAmt / Convert.ToDecimal(1.14), 2);  // Excl Vat
            amt1 = d.TransAmt - amt; // Vat
            */

            AddJournalDetail(ref jh, ref jdSeq, "1315" + bPostFix.Trim(),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                string.Format("11: {0}", d.Description.Trim()), d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq, ASSBranchCodeToGL(d.AdditionalInfo.Trim()),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                string.Format("11: {0}", d.Description.Trim()), d.TransAmt, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            /*AddJournalDetail(ref jh, ref jdSeq, string.Format("2665{0}", bLegalPostFix.Trim()), 
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                string.Format("11: {0}: {1}", d.Description.Trim(),d.TransAmt < 0 ? "Cr" : "Dr" ), amt1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim()); 
            }*/

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 14- BANK WITHDRAWALS/DEPOSITS

          case "14": // BANK WITHDRAWALS/DEPOSITS

            procInd = 0;
            procErr = "";

            bank = SQL.Get_Accpac_CB_GetBankDetails(d.AdditionalInfo.Trim());
            if (string.IsNullOrEmpty(bank?.BankName))
            {
              procErr = d.AdditionalInfo.Trim() + " is not a valid Bank Account.";
              procInd = 2;

              bankName = "Invalid Bank Account";
              bankCAcct = d.AdditionalInfo.Trim();
            }
            else
            {
              bankName = bank.BankName.Trim();
              bankCAcct = bank.GLAccount_Clearing_New(ASSBranchCodeToGL(d.BranchCode, 3));
            }

            if (d.TransAmt > 0)
            {
              AddJournalDetail(ref jh, ref jdSeq, "1315" + bPostFix.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "14: BANK WITHDRAWALS/DEPOSITS: ", SetToNegative(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq, bankCAcct,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "14: BANK WITHDRAWALS/DEPOSITS", SetToPositive(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              // Inter-Company Loans
              tB = bankCAcct.Substring(bankCAcct.Length - 1, 1);
              oB = GetBranchLegalEntity(d.BranchCode.Trim());
              if (tB.Trim() != oB.Trim())
              {
                Record_InterCompany_Loans(tB, oB, ref jh, ref jdSeq, ref d, ref procInd, ref procErr);
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;
            }
            else  // default -- Credit Amount
            {
              AddJournalDetail(ref jh, ref jdSeq, "1315" + bPostFix.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "14: BANK WITHDRAWALS/DEPOSITS", SetToPositive(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq, bankCAcct,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "14: BANK WITHDRAWALS/DEPOSITS", SetToNegative(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              // Inter-Company Loans
              tB = GetBranchLegalEntity(d.BranchCode.Trim());
              oB = bankCAcct.Substring(bankCAcct.Length - 1, 1);
              if (tB.Trim() != oB.Trim())
              {
                Record_InterCompany_Loans(tB, oB, ref jh, ref jdSeq, ref d, ref procInd, ref procErr);
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;
            }

            break;

          #endregion


          #region 13- CASH TRANSFER BETWEEN BRANCHES

          case "13": // CASH TRANSFER BETWEEN BRANCHES

            procInd = 0;
            procErr = "";

            var oBranch = "0" + SharedMembers.StripValues(d.Description.Trim(), "BR TFR ", "").Substring(0, 2);
            var bPostFixOther = GetBranchGL_PostFix(oBranch.Trim());

            #region Inflow
            if (d.TransAmt < 0) // Inflow
            {
              AddJournalDetail(ref jh, ref jdSeq, "1320" + bPostFixOther.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("13: CASH TRANSFER BETWEEN BRANCHES - {0}", oBranch), SetToNegative(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              AddJournalDetail(ref jh, ref jdSeq, "1315" + bPostFix.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("13: CASH TRANSFER BETWEEN BRANCHES - {0}", oBranch), SetToPositive(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              // Inter-Company Loans
              tB = GetBranchLegalEntity(d.BranchCode.Trim());
              oB = GetBranchLegalEntity(oBranch.Trim());
              if (tB.Trim() != oB.Trim())
              {
                Record_InterCompany_Loans(tB, oB, ref jh, ref jdSeq, ref d, ref procInd, ref procErr);
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;

            }
            #endregion

            #region Outflow
            else // Outflow
            {
              AddJournalDetail(ref jh, ref jdSeq, "1320" + bPostFixOther.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("13: CASH TRANSFER BETWEEN BRANCHES - {0}", oBranch), SetToPositive(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              AddJournalDetail(ref jh, ref jdSeq, "1315" + bPostFix.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("13: CASH TRANSFER BETWEEN BRANCHES - {0}", oBranch), SetToNegative(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              // Inter-Company Loans
              tB = GetBranchLegalEntity(oBranch.Trim());
              oB = GetBranchLegalEntity(d.BranchCode.Trim());
              if (tB.Trim() != oB.Trim())
              {
                Record_InterCompany_Loans(tB, oB, ref jh, ref jdSeq, ref d, ref procInd, ref procErr);
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;
            }
            #endregion


            break;

          #endregion


          #region 12- TAKEN BY MEMBER

          case "12": // TAKEN BY MEMBER

            procInd = 0;
            procErr = "";

            if (d.AdditionalInfo.Trim() == "")
            {
              //procErr = "No GL Account was provided.";
              //procInd = 2;
              d.AdditionalInfo = "9999" + bPostFix.Trim();
            }


            if (d.TransAmt > 0) // Member gave Branch Cash
            {
              AddJournalDetail(ref jh, ref jdSeq, ASSBranchCodeToGL(d.AdditionalInfo.Trim()),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "12: TAKEN BY MEMBER", SetToNegative(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq, "1315" + bPostFix.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "12: TAKEN BY MEMBER", SetToPositive(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;
            }
            else // Member took Branch Cash
            {
              AddJournalDetail(ref jh, ref jdSeq, "1315" + bPostFix.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "12: TAKEN BY MEMBER", SetToNegative(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq, ASSBranchCodeToGL(d.AdditionalInfo.Trim()),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "12: TAKEN BY MEMBER", SetToPositive(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;
            }

            break;
          #endregion


          #region 20- CASH OVER/UNDER

          case "20":

            procInd = 0;
            procErr = "";

            if (d.TransAmt < 0) // Cash OVER
            {
              AddJournalDetail(ref jh, ref jdSeq, "0265" + bPostFix.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "20: CASH OVER", SetToNegative(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq, "1315" + bPostFix.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "20: CASH OVER", SetToPositive(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;
            }
            else // Cash Under
            {
              AddJournalDetail(ref jh, ref jdSeq, "0265" + bPostFix.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "20: CASH UNDER", SetToPositive(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq, "1315" + bPostFix.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "20: CASH UNDER", SetToNegative(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              rVal.Error = procErr;
              rVal.ProcessedInd = procInd;
            }

            break;

          #endregion


          #region 25- NUPAY: Issue Loan

          case "25":

            procInd = 0;
            procErr = "";

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(), // Changes: KB
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "25: NUCARD Issue Loan", SetToNegative(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq, string.Format("1220{0}", bPostFix.Trim()),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "25: NUCARD Issue Loan", SetToPositive(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 26- NUPAY: Issue Loan (Cancel)

          case "26": // NUPAY: Issue Loan (Cancel)

            procInd = 0;
            procErr = "";

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(), // Changes: KB
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "26: NUCARD Issue Loan (Cancel)", SetToPositive(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq, string.Format("1220{0}", bPostFix.Trim()),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "26: NUCARD Issue Loan (Cancel)", SetToNegative(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 28- NUCARD Refund

          case "28":
            procInd = 0;
            procErr = "";

            if (d.TransAmt < 0)
            {
              AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(), // Changes: KB
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "28: NUCARD Refund", SetToNegative(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq, "1220" + bPostFix.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "28: NUCARD Refund", SetToPositive(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }
            }
            else
            {
              AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(), // Changes: KB
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "28: NUCARD Refund Reversal", SetToPositive(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq, "1220" + bPostFix.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "28: NUCARD Refund Reversal", SetToNegative(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion

          #region 23- NUPAY- NuCard Fees

          case "23": // NUPAY

            procInd = 0;
            procErr = "";

            switch (d.Description.Trim())
            {
              case "NUCARD FEE":

                if (d.TransAmt > 0)
                {
                  AddJournalDetail(ref jh, ref jdSeq, string.Format("0377-REC{0}", bPostFix.Trim()),
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      "23: NUCARD FEES", SetToNegative(d.TransAmt), ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = rVal.Error.Trim();
                  }

                  AddJournalDetail(ref jh, ref jdSeq, string.Format("1220{0}", bPostFix.Trim()),
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      "23: NUCARD FEES", SetToPositive(d.TransAmt), ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  rVal.Error = procErr;
                  rVal.ProcessedInd = procInd;
                }
                else
                {
                  AddJournalDetail(ref jh, ref jdSeq, string.Format("1220{0}", bPostFix.Trim()),
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      "23: NUCARD FEES", SetToNegative(d.TransAmt), ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = rVal.Error.Trim();
                  }

                  AddJournalDetail(ref jh, ref jdSeq,
                      string.Format("0377-REC{0}", bPostFix.Trim()),
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      "23: NUCARD FEES", SetToPositive(d.TransAmt), ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  rVal.Error = procErr;
                  rVal.ProcessedInd = procInd;
                }

                break;

              case "NUCARD FEE VAT":

                if (d.TransAmt > 0)
                {
                  AddJournalDetail(ref jh, ref jdSeq,
                      string.Format("0377-REC{0}", bPostFix.Trim()),
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      "23: NUCARD FEES VAT", SetToPositive(d.TransAmt), ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  rVal.Error = procErr;
                  rVal.ProcessedInd = procInd;

                }
                else
                {
                  AddJournalDetail(ref jh, ref jdSeq,
                      string.Format("2665{0}", bLegalPostFix.Trim()),
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      "23: NUCARD FEES VAT", SetToNegative(d.TransAmt), ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = rVal.Error.Trim();
                  }

                  rVal.Error = procErr;
                  rVal.ProcessedInd = procInd;
                }


                break;

            }

            break;

          #endregion


          #region 24- NUPAY Cancel

          case "24": // NUPAY- normally negative
            procInd = 0;
            procErr = "";

            switch (d.Description.Trim())
            {
              case "NUCARD FEE CANCEL":
                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("0377-REC{0}", bPostFix.Trim()),
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    string.Format("24: NUCARD FEES: {0}", d.TransAmt >= 0 ? "Cr" : "Dr"), d.TransAmt * -1, ref rVal);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = rVal.Error.Trim();
                }

                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("1220{0}", bPostFix.Trim()),
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    string.Format("24: NUCARD FEES: {0}", d.TransAmt >= 0 ? "Dr" : "Cr"), d.TransAmt, ref rVal);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;

                break;

              case "NUCARD FEE VAT CANCEL": // These are passed as a pair by ass
                if (d.TransAmt < 0)
                {
                  AddJournalDetail(ref jh, ref jdSeq,
                      string.Format("0377-REC{0}", bPostFix.Trim()),
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      "24: NUCARD FEES VAT", SetToNegative(d.TransAmt), ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  rVal.Error = procErr;
                  rVal.ProcessedInd = procInd;
                }
                else
                {
                  AddJournalDetail(ref jh, ref jdSeq,
                      string.Format("2665{0}", bLegalPostFix.Trim()),
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      "24: NUCARD FEES VAT", SetToPositive(d.TransAmt), ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = rVal.Error.Trim();
                  }

                  rVal.Error = procErr;
                  rVal.ProcessedInd = procInd;
                }

                break;
            }

            break;

          #endregion


          #region 21- SDC Transactions

          case "21": // SDC Transactions

            procInd = 0;
            procErr = "";

            switch (d.Description.Trim())
            {
              case "SDC LOAN DEBITS JNL":
              case "SDC WRITE OFF":
              case "SDC DISCOUNT":
                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("0064-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    "21: SDC LOAN DEBITS JNL", SetToPositive(d.TransAmt), ref rVal);
                SCD_Net += SetToNegative(d.TransAmt);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;
                break;

              case "SDC LOAN CREDITS JNL":
                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("0064-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())), string.Format("{0}: {1}",
                    d.BranchCode.Trim(), d.TransactionDate),
                    "21: SDC LOAN CREDITS JNL", SetToNegative(d.TransAmt), ref rVal);
                SCD_Net += SetToPositive(d.TransAmt);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;
                break;

              case "SDC PENALTY INCOME":
                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("1221-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    "21: Penalty Income", SetToNegative(d.TransAmt), ref rVal);
                SCD_Net += SetToPositive(d.TransAmt);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;
                break;

              case "SDC CONSULT":
                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("0050-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    "21: Consultation charge", SetToNegative(d.TransAmt), ref rVal);
                SCD_Net += SetToPositive(d.TransAmt);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;
                break;

              case "SDC ORD LETTER":
                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("0052-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    "21: Ordinary letter charge", SetToNegative(d.TransAmt), ref rVal);
                SCD_Net += SetToPositive(d.TransAmt);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;
                break;

              case "SDC PERUSAL":
                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("0054-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())), string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    "21: Perusal charge", SetToNegative(d.TransAmt), ref rVal);
                SCD_Net += SetToPositive(d.TransAmt);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;
                break;

              case "SDC REG LETTER":
                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("0058-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    "21: Registered letter charge", SetToNegative(d.TransAmt), ref rVal);
                SCD_Net += SetToPositive(d.TransAmt);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;
                break;

              case "SDC SHERIFF":
              case "SDC CHARGE VAT":
                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("0059-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    "21: Sheriff fees", SetToNegative(d.TransAmt), ref rVal);
                SCD_Net += SetToPositive(d.TransAmt);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;
                break;

              case "SDC TRACE FEE":
                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("0062-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    "21: Tracing fee charge", SetToNegative(d.TransAmt), ref rVal);
                SCD_Net += SetToPositive(d.TransAmt);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;
                break;

              case "SDC TELEPHONE CALL":
                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("0060-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    "21: Telephone call charge", SetToNegative(d.TransAmt), ref rVal);
                SCD_Net += SetToPositive(d.TransAmt);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;
                break;

              case "SDC RECEIPT FEE":
                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("0056-{0}-3", ASSBranchCodeToGL(d.BranchCode.Trim())),
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    "21: Receipting charge", SetToNegative(d.TransAmt), ref rVal);
                SCD_Net += SetToPositive(d.TransAmt);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                }

                rVal.Error = procErr;
                rVal.ProcessedInd = procInd;
                break;
            }

            break;

          #endregion


          #region 31 = Rolled Loans (treat like sortkey 25)

          case "31":
            procInd = 0;
            procErr = "";

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "31: Rolled Loans", SetToNegative(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq, "1220" + bPostFix.Trim(),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "31: Rolled Loans", SetToPositive(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 32 = Rolled Loans Cancel (treat like 26)

          case "32":
            procInd = 0;
            procErr = "";

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "32: Rolled Loans Cancel", SetToPositive(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq, "1220" + bPostFix.Trim(),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "32: Rolled Loans Cancel", SetToNegative(d.TransAmt), ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 33 = Rolled Rec-Cur (Receipts in Current Dept) (treat like 27- reverse of 34)

          case "33":
            procInd = 0;
            procErr = "";

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "33: Rolled Rec-Cur", d.TransAmt, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq, "1220" + bPostFix.Trim(),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                string.Format("33: Rolled Rec-Cur"), d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 34 = Rolled Ref-Cur (Refunds in Current Dept) (treat like 28)

          case "34":
            procInd = 0;
            procErr = "";

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
               "34: Rolled Ref-Cur", d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq, "1310" + bPostFix.Trim(),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "34: Rolled Ref-Cur", d.TransAmt, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 35- ** N/A ** - Rolled Rec-Leg (Receipts in Legal Dept) (treat like 29)

          case "35":
            break;

          #endregion


          #region 36- ** N/A ** - Rolled Ref-Leg (Refunds in Legal Dept) (treat like 30)

          case "36":
            break;

          #endregion


          #region 37 - NuCard 9.nine -> Marketing is entity 2, if branch is entity 1,  there are VAT and IC implications

          case "37":
            procInd = 0;
            procErr = "";

            #region Box gift set
            if (d.Description.Trim().StartsWith("BOX /GIV", StringComparison.OrdinalIgnoreCase))
            {
              AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),               // Should be 0284-BOX-BBB-E
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "37: NuCard 9.nine box gift", d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq, "0284-813-2",
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "37: NuCard 9.nine box gift", d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              // If different LE:
              //        Calc VAT amt- DR: 2660-LE, CR: 26602
              var branchLE = GetBranchLegalEntity(d.BranchCode).Trim();

              // Marketing is LE 2, if region not in LE 2, IC for VAT and marketing expenses takes effect
              if (!branchLE.Equals("2"))
              {
                var nuCardVAT = d.TransAmt * 0.14M;

                // IC VAT
                AddJournalDetail(ref jh, ref jdSeq, string.Format("2660-{0}", branchLE), // Should always ne 2660-1...
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    string.Format("37: NuCard 9.nine Box IC VAT: {0}", nuCardVAT < 0 ? "Cr" : "Dr"), nuCardVAT, ref rVal);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = rVal.Error.Trim();
                }

                AddJournalDetail(ref jh, ref jdSeq, "2660-2",
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    string.Format("37: NuCard 9.nine Box IC VAT: {0}", nuCardVAT < 0 ? "Dr" : "Cr"), nuCardVAT * -1, ref rVal);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                var nuCardVATINC = d.TransAmt * 1.14M;

                // VAT INC amount- CR: 2125-LE, DR: 2130-2                                                        
                AddJournalDetail(ref jh, ref jdSeq, "2130-2",
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    string.Format("37: NuCard 9.nine Box IC INC VAT: {0}", nuCardVATINC < 0 ? "Cr" : "Dr"), nuCardVATINC, ref rVal);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }

                AddJournalDetail(ref jh, ref jdSeq, string.Format("2125-{0}", branchLE),
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    string.Format("37: NuCard 9.nine Box IC INC VAT: {0}", nuCardVATINC < 0 ? "Dr" : "Cr"), nuCardVATINC * -1, ref rVal);
                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = rVal.Error.Trim();
                }
              }
            }
            #endregion

            #region Cash into NuCard
            else if (d.Description.Trim().StartsWith("NUC /GIV", StringComparison.OrdinalIgnoreCase)) // Cash into NuCard
            {
              AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),                  // Should be 0284-BON-BBB-E or 1305-80[R]-[E]
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "37: NuCard 9.nine NuCard Bonus", d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              AddJournalDetail(ref jh, ref jdSeq, "0284-813-2",
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "37: NuCard 9.nine NuCard Bonus", d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }
            }
            #endregion

            #region SIM card Cell C
            else if (d.Description.Trim().StartsWith("SIMC/GIV", StringComparison.OrdinalIgnoreCase))
            {
              AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "37: SIM Card Cell C", d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              AddJournalDetail(ref jh, ref jdSeq, "0284-813-2",
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "37: SIM Card Cell C", d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }
            }
            #endregion

            #region SIM card Blue Label
            else if (d.Description.Trim().StartsWith("SIMB/GIV", StringComparison.OrdinalIgnoreCase))
            {
              AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "37: SIM Card Blue Label", d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

              AddJournalDetail(ref jh, ref jdSeq, "0284-813-2",
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "37: SIM Blue Label", d.TransAmt * -1, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }
            }
            #endregion

            #region Service fee overcharge- Refund into NuCard (SINGLE TRANS- we rely on ASS/QD for all legs) (2016-08-19)
            else if (d.Description.Trim().StartsWith("NUC1/GIV", StringComparison.OrdinalIgnoreCase))
            {
              AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),                  // Should be 0284-BON-BBB-E or 1305-80[R]-[E]
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "37: Service fee refund (NuCard)", d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }
            }
            #endregion

            #region Service fee overcharge- Refund into bank card - 1 (SINGLE TRANS- we rely on ASS/QD for all legs) (2016-08-19)
            else if (d.Description.Trim().StartsWith("BNK /GIV TFR", StringComparison.OrdinalIgnoreCase))
            {
              bank = SQL.Get_Accpac_CB_GetBankDetails(d.AdditionalInfo.Trim());
              if (string.IsNullOrEmpty(bank?.BankName))
              {
                procErr = string.Format("{0} is not a valid Bank Account.", d.AdditionalInfo.Trim());
                procInd = 2;

                bankName = "Invalid Bank Account";
                bankCAcct = d.AdditionalInfo.Trim();
              }
              else
              {
                bankName = bank.BankName.Trim();
                bankCAcct = bank.GLAccount_Clearing_New(ASSBranchCodeToGL(d.BranchCode, 3));
              }

              AddJournalDetail(ref jh, ref jdSeq, bankCAcct,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "37: Service fee refund (Bank)",
                  d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }
            }
            #endregion

            #region Service fee overcharge- Refund into bank card - 2 (SINGLE TRANS- we rely on ASS/QD for all legs) (2016-08-19)
            else if (d.Description.Trim().StartsWith("BNK /GIV ACC", StringComparison.OrdinalIgnoreCase))
            {
              AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "37: Service fee refund (Bank)", d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }
            }
            #endregion

            #region Service fee overcharge- Refund cash (SINGLE TRANS- we rely on ASS/QD for all legs) (2016-08-19)
            else if (d.Description.Trim().StartsWith("CSH /GIV", StringComparison.OrdinalIgnoreCase))
            {
              if (d.AdditionalInfo.Trim() == "CASH")
              {
                AddJournalDetail(ref jh, ref jdSeq,
                    string.Format("1315{0}", bPostFix.Trim()),
                    string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                    "37: Service fee refund (cash)",
                    d.TransAmt, ref rVal);
              }
              else
              {
                AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                 string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                 "37: Service fee refund (cash)", d.TransAmt, ref rVal);
              }

              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = rVal.Error.Trim();
              }
            }
            #endregion

            else
            {
              procInd = 2;
              procErr = string.Format("Unknown description: '{0}'", d.Description);
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;
          #endregion


          #region 38 SDC receipts/refunds

          case "38":
            var eventKind = d.TransAmt > 0 ? "receipt" : "refund";

            procInd = 0;
            procErr = "";

            // Bad debts recovered
            AddJournalDetail(ref jh, ref jdSeq, string.Format("0185{0}", bPostFix),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                string.Format("38: Branch SDC {0}: {1}", eventKind, (d.TransAmt * -1) > 0 ? "Dr" : "Cr"), d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            if (d.AdditionalInfo.StartsWith("AP:")) // Is ACCPAC account, not bank account
            {
              AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Substring(3).Trim(),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                string.Format("38: Branch SDC {0}: {1}", eventKind, d.TransAmt > 0 ? "Dr" : "Cr"), d.TransAmt, ref rVal);
            }
            else if (d.AdditionalInfo.Trim() == "CASH")
            {
              // Cash on hand
              AddJournalDetail(ref jh, ref jdSeq, string.Format("1315{0}", bPostFix),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("38: Branch SDC {0} cash: {1}", eventKind, d.TransAmt > 0 ? "Dr" : "Cr"), d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              // Cash clearing DR/CR - required for ACCPAC (recommended)
              // -------------------------------------------------------------------------------------------------
              AddJournalDetail(ref jh, ref jdSeq, string.Format("1320{0}", bPostFix),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("38: Branch SDC {0} cash", eventKind), SetToPositive(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }

              AddJournalDetail(ref jh, ref jdSeq, string.Format("1320{0}", bPostFix),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("38: Branch SDC {0} cash", eventKind), SetToNegative(d.TransAmt), ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }
              // -------------------------------------------------------------------------------------------------
            }
            else if ((d.AdditionalInfo.Trim()) == "ROLLED")
            {
              AddJournalDetail(ref jh, ref jdSeq, string.Format("1310{0}", bPostFix),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("38: Branch SDC {0} rolled: {1}", eventKind, d.TransAmt > 0 ? "Dr" : "Cr"), d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }
            }
            else if (!string.IsNullOrEmpty(d.AdditionalInfo))
            {
              // Get clearing account
              bank = SQL.Get_Accpac_CB_GetBankDetails(d.AdditionalInfo.Trim());
              if (string.IsNullOrEmpty(bank?.BankName))
              {
                procErr = string.Format("{0} is not a valid Bank Account.", d.AdditionalInfo.Trim());
                procInd = 2;

                bankName = "Invalid Bank Account";
                bankCAcct = d.AdditionalInfo.Trim();
              }
              else
              {
                bankName = bank.BankName.Trim();
                bankCAcct = bank.GLAccount_Clearing_New(ASSBranchCodeToGL(d.BranchCode, 3));
              }

              AddJournalDetail(ref jh, ref jdSeq, bankCAcct,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("38: Branch SDC {0} bank: {1}", eventKind, d.TransAmt > 0 ? "Dr" : "Cr"), d.TransAmt, ref rVal);
              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
              }
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 41 SIM Card sale
          case "41":
            procInd = 0;
            procErr = "";

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "41: SIM Card sale:", d.TransAmt, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            AddJournalDetail(ref jh, ref jdSeq, string.Format("0173-INC{0}", bPostFix),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "41: SIM Card sale", d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;
          #endregion


          #region 42 SIM Card cancel
          case "42":
            procInd = 0;
            procErr = "";
            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "42: SIM Card sale cancel", d.TransAmt, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            AddJournalDetail(ref jh, ref jdSeq, string.Format("0173-INC{0}", bPostFix),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "42: SIM Card sale cancel", d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;
          #endregion


          #region 43 SIM Card rebate
          case "43":
            procInd = 0;
            procErr = "";

            var rebateInfo = GetTextInsideCurlies(d.Description);

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("43: SIM card rebate: {0}", rebateInfo), d.TransAmt, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            AddJournalDetail(ref jh, ref jdSeq, string.Format("1220{0}", bPostFix),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                string.Format("43: SIM Card rebate: {0}", rebateInfo), d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region 48 SDC Suspense Deposit
          case "48":
            procInd = 0;
            procErr = "";

            var account48 = GetTextInsideCurliesClean(d.Description);
            if (!string.IsNullOrEmpty(account48))
            {
              // Debit clearing account
              bank = SQL.Get_Accpac_CB_GetBankDetails(d.AdditionalInfo.Trim());
              if (string.IsNullOrEmpty(bank?.BankName))
              {
                procErr = string.Format("{0} is not a valid Bank Account.", d.AdditionalInfo.Trim());
                procInd = 2;

                bankName = "Invalid Bank Account";
                bankCAcct = d.AdditionalInfo.Trim();
              }
              else
              {
                AddJournalDetail(ref jh, ref jdSeq, bank.GLAccountClearing,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "48: “SDC Suspense Deposit", d.TransAmt, ref rVal);

                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }
              }

              // Credit other account in curlies
              AddJournalDetail(ref jh, ref jdSeq, account48,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "48: “SDC Suspense Deposit", d.TransAmt * -1, ref rVal);

              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }
            }
            else
            {
              procInd = 2;
              procErr = "Missing the account details from description field";
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;
          #endregion


          #region 49 SDC Suspense Alloc
          case "49":
            procInd = 0;
            procErr = "";

            var account49 = GetTextInsideCurliesClean(d.Description);
            if (!string.IsNullOrEmpty(account49))
            {
              // Clearing account of the relevant SDC bank account is credited (if the sign is negative, and vice versa if positive). 
              bank = SQL.Get_Accpac_CB_GetBankDetails(d.AdditionalInfo.Trim());
              if (string.IsNullOrEmpty(bank?.BankName))
              {
                procErr = string.Format("{0} is not a valid Bank Account.", d.AdditionalInfo.Trim());
                procInd = 2;

                bankName = "Invalid Bank Account";
                bankCAcct = d.AdditionalInfo.Trim();
              }
              else
              {
                AddJournalDetail(ref jh, ref jdSeq, bank.GLAccountClearing,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "48: “SDC Suspense Alloc", d.TransAmt, ref rVal);

                if (rVal.ProcessedInd == 2)
                {
                  procInd = 2;
                  procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
                }
              }

              // The double entry will go to the account in curly brackets in the description column (i.e. 0185-849-3). 
              AddJournalDetail(ref jh, ref jdSeq, account49,
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  "48: SDC Suspense Alloc", d.TransAmt * -1, ref rVal);

              if (rVal.ProcessedInd == 2)
              {
                procInd = 2;
                procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
              }

            }
            else
            {
              procInd = 2;
              procErr = "Missing account details from description";
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;
          #endregion


          #region 50 VAP Sales
          case "50":
            // Dr other = "1220-"
            AddJournalDetail(ref jh, ref jdSeq,
                  string.Format("1220{0}", bPostFix),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("50: {0}", d.Description), d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("50: {0}", d.Description), d.TransAmt, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            break;

          #endregion


          #region 51 VAP Cost
          case "51":
            // 2514-2
            AddJournalDetail(ref jh, ref jdSeq, "2514-2",
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("51: {0}", d.Description), d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("51: {0}", d.Description), d.TransAmt, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            break;

          #endregion


          #region 52 VAP Sales Cancel
          case "52":
            //other = "1220-"
            AddJournalDetail(ref jh, ref jdSeq,
                  string.Format("1220{0}", bPostFix),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("52: {0}", d.Description), d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("52: {0}", d.Description), d.TransAmt, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            break;

          #endregion


          #region 53 VAP Cost Cancel
          case "53":
            // 2514-2
            AddJournalDetail(ref jh, ref jdSeq, "2514-2",
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("53: {0}", d.Description), d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
                  string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                  string.Format("53: {0}", d.Description), d.TransAmt, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            break;
          #endregion


          #region Salary deductions-  receipt
          case "44":
            procInd = 0;
            procErr = "";

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
              string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
              "44: Salary deduction receipt", d.TransAmt, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            AddJournalDetail(ref jh, ref jdSeq, string.Format("1220{0}", bPostFix),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "44: Salary deduction receipt", d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

          #endregion


          #region Salary deductions- refund
          case "45":
            procInd = 0;
            procErr = "";

            AddJournalDetail(ref jh, ref jdSeq, string.Format("1220{0}", bPostFix),
                string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                "45: Salary deduction refund", d.TransAmt, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = rVal.Error.Trim();
            }

            AddJournalDetail(ref jh, ref jdSeq, d.AdditionalInfo.Trim(),
              string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
              "45: Salary deduction refund", d.TransAmt * -1, ref rVal);
            if (rVal.ProcessedInd == 2)
            {
              procInd = 2;
              procErr = string.Format(procErr.Trim() != "" ? "{0} {1}" : "{0}", procErr, rVal.Error.Trim());
            }

            rVal.Error = procErr;
            rVal.ProcessedInd = procInd;

            break;

            #endregion
        }

        return rVal;
      }
      catch (Exception ex)
      {
        _log.Error(ex, "TransformToGLJournalDetail");
        Status = "Error";

        rVal.ProcessedInd = 2;
        rVal.Error = ex.Message;
        rVal.ProcessedDate = DateTime.Now;

        return rVal;
      }
    }


    private string Record_InterCompany_Loans(string tB, string oB, ref JournalHeader jh, ref int jdSeq, ref ASS_DailyUpload_Detail d, ref Int16 procInd, ref string procErr)
    {
      var rVal = new GLJournalDetail();
      try
      {
        var rVal1 = "";
        if (oB != tB) // Require Intercompany Loans
        {
          switch (tB) // The receiving Branch
          {
            case "1":

              switch (oB) // The giving Branch
              {
                case "2":
                  AddJournalDetail(ref jh, ref jdSeq, "2125-1",
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      string.Format("13: Inter-Company Loan - {0}", oB), d.TransAmt * -1, ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  AddJournalDetail(ref jh, ref jdSeq, "2130-2",
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      string.Format("13: Inter-Company Loan - {0}", oB), d.TransAmt, ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  break;


                case "3":
                  AddJournalDetail(ref jh, ref jdSeq, "2610-1",
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      string.Format("13: Inter-Company Loan - {0}", oB), d.TransAmt * -1, ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  AddJournalDetail(ref jh, ref jdSeq, "2130-3", string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      string.Format("13: Inter-Company Loan - {0}", oB), d.TransAmt, ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  break;
              }

              break;

            case "2":

              switch (oB)
              {
                case "1":
                  AddJournalDetail(ref jh, ref jdSeq, "2130-2",
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      string.Format("13: Inter-Company Loan - {0}", oB), d.TransAmt * -1, ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  AddJournalDetail(ref jh, ref jdSeq, "2125-1",
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      string.Format("13: Inter-Company Loan - {0}", oB), d.TransAmt, ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  break;


                case "3":
                  AddJournalDetail(ref jh, ref jdSeq, "2610-2",
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      string.Format("13: Inter-Company Loan - {0}", oB), d.TransAmt * -1, ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  AddJournalDetail(ref jh, ref jdSeq, "2125-3",
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      string.Format("13: Inter-Company Loan - {0}", oB), d.TransAmt, ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  break;
              }

              break;


            case "3":
              switch (oB)
              {
                case "1":
                  AddJournalDetail(ref jh, ref jdSeq, "2130-3",
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      string.Format("13: Inter-Company Loan - {0}", oB), d.TransAmt * -1, ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  AddJournalDetail(ref jh, ref jdSeq, "2610-1",
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      string.Format("13: Inter-Company Loan - {0}", oB), d.TransAmt, ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  break;


                case "2":
                  AddJournalDetail(ref jh, ref jdSeq, "2125-3",
                      string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      string.Format("13: Inter-Company Loan - {0}", oB), d.TransAmt * -1, ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  AddJournalDetail(ref jh, ref jdSeq, "2610-2", string.Format("{0}: {1}", d.BranchCode.Trim(), d.TransactionDate),
                      string.Format("13: Inter-Company Loan - {0}", oB), d.TransAmt, ref rVal);
                  if (rVal.ProcessedInd == 2)
                  {
                    procInd = 2;
                    procErr = procErr.Trim() != "" ? string.Format("{0}  {1}", procErr, rVal.Error.Trim()) : rVal.Error.Trim();
                  }

                  break;
              }

              break;
          }
        }

        return rVal1;
      }
      catch (Exception ex)
      {
        _log.Error(ex, "Record_InterCompany_Loans");
        return ex.Message;
      }
    }


    private string GetBranchGL_PostFix(string branchCode)
    {
      try
      {
        // 2013-01-11- Ryan Eber goes to entity 3??? code below will not allow this
        if (branchCode == "B8" || branchCode == "0B8")
        {
          return "-118-3";
        }

        return _branchToLegalEntities.FirstOrDefault(b => b.BranchCode == ASSBranchCodeToGL(branchCode) &&
                                                          b.LegalEntity.Trim() != "3")?.GLPostFix.Trim() ?? "";
      }
      catch (Exception ex)
      {
        _log.Error(ex, "GetBranchGL_PostFix");
        Status = "Error";
        return "";
      }
    }


    private string GetBranchLegalEntity(string branchCode)
    {
      try
      { 
        // 2013-01-11- Ryan Eber goes to entity 3??? Code below will not allow
        if (branchCode == "B8" || branchCode == "0B8")
        {
          return "3";
        }

        return _branchToLegalEntities.FirstOrDefault(b => b.BranchCode == ASSBranchCodeToGL(branchCode) &&
                                                           b.LegalEntity.Trim() != "3")?.LegalEntity.Trim() ?? "";
      }
      catch (Exception ex)
      {
        _log.Error(ex, "GetBranchLegalEntity");
        Status = "Error";
        return "";
      }
    }



    private static string ASSBranchCodeToGL(string assBranchCode, int padto = 2)
    {
      var result = assBranchCode;
      result = result.Replace("0A", "10");
      result = result.Replace("0B", "11");
      result = result.Replace("0C", "12");
      result = result.Replace("0D", "13");
      result = result.Replace("0E", "14");
      result = result.Replace("0F", "15");
      result = result.Replace("0G", "16");
      result = result.Replace("0H", "17");
      result = result.Replace("0I", "18");
      result = result.Replace("0J", "19");
      result = result.Replace("0K", "20");
      result = result.Replace("0L", "21");
      result = result.Replace("0M", "22");
      result = result.Replace("0N", "23");
      result = result.Replace("0O", "24");
      result = result.Replace("0P", "25");
      result = result.Replace("0Q", "26");
      result = result.Replace("0R", "27");
      result = result.Replace("0S", "28");
      result = result.Replace("0T", "29");
      result = result.Replace("0U", "30");

      if (result.Length < padto)
      {
        result = result.PadLeft(padto, '0');
      }

      return result;
    }


    private string GetBranchGL_LegalEntityPostFix(string branchCode)
    {
      try
      {       
        var entity = _branchToLegalEntities.FirstOrDefault(b => b.BranchCode == ASSBranchCodeToGL(branchCode) &&
                                                                 b.LegalEntity.Trim() != "3");

        return entity != null ? string.Format("-{0}", entity.LegalEntity.Trim()) : "";
      }
      catch (Exception ex)
      {
        _log.Error(ex, "GetBranchGL_LegalEntityPostFix");
        Status = "Error";
        return "";
      }
    }


    private static decimal SetToPositive(decimal amt)
    {
      return Math.Abs(amt);
    }


    private static decimal SetToNegative(decimal amt)
    {
      return Math.Abs(amt) * -1;
    }


    private string GetNextTransNr(int i)
    {
      try
      {
        return string.Format("{0:d10}", ++i);
      }
      catch (Exception ex)
      {
        _log.Error(ex, "GetNextTransNr");
        Status = "Error";
        return "";
      }
    }


    #region GL

    public static string GLJournalHeader_Update(ref JournalHeader journalHeader)
    {
      if (journalHeader == null)
        return "journalHeader object was null.";

      var myIndex = 0;
      var progress = "";
      var rVal = "";

      var err = "";
      var processInd = 99;

      if (journalHeader._journalDetails?.Count > 0)
      {
        GLJournalDetail mj;
        foreach (var jd in journalHeader._journalDetails)
        {
          mj = jd;
          SQL.GLJournalDetail_Update(ref mj, ref myIndex, ref progress);
          if (mj.ProcessedInd == 2)
          {
            processInd = 2;
            err += "\n\r " + mj.Error.Trim();
          }
        }
      }

      if (processInd != 99)
      {
        journalHeader.ProcessedInd = 2;
        journalHeader.Error += err;
      }

      rVal = SQL.GLJournalHeader_Update(ref journalHeader, ref myIndex, ref progress);

      return rVal;
    }


    private static string GLJournalHeader_Insert(JournalHeader journalHeader)
    {
      try
      {
        if (journalHeader == null)
          return "Journal Header is null.";

        var myIndex = 0;
        var progress = "";
        var rVal1 = "";
        var rVal = 0;

        var err = "";

        rVal = SQL.GLJournalHeader_Insert(ref journalHeader, ref myIndex, ref progress);
        if (journalHeader._journalDetails != null)
        {
          if (journalHeader._journalDetails.Count > 0)
          {
            GLJournalDetail mj;
            foreach (var jd in journalHeader._journalDetails)
            {
              mj = jd;
              mj.InternalBatchID = rVal;
              SQL.GLJournalDetail_Insert(ref mj, ref myIndex, ref progress);
            }
          }
        }

        rVal1 = err;
        return rVal1;
      }
      catch (Exception ex)
      {
        _log.Error(ex, "GLJournalHeader_Insert");
        return ex.Message;
      }
    }

    #endregion


    public static string Atlas_GL_ASS_DailyUpload_Header_Update(ref ASS_DailyUpload_Header ass_DailyUpload_Header)
    {
      if (ass_DailyUpload_Header == null)
        return "ass_DailyUpload_Header object was null.";

      var progress = "";
      var rVal = "";

      var err = "";
      var processInd = 99;

      if (ass_DailyUpload_Header.myASS_DailyUpload_Detail != null)
        if (ass_DailyUpload_Header.myASS_DailyUpload_Detail.Count > 0)
        {
          ASS_DailyUpload_Detail mj;
          foreach (var jd in ass_DailyUpload_Header.myASS_DailyUpload_Detail)
          {
            mj = jd;
            SQL.Atlas_GL_ASS_DailyUpload_Detail_Update(ref mj, ref progress);
            if (mj.ProcessedInd == 2)
            {
              processInd = 2;
              err += "\n\r " + mj.Error.Trim();
            }
          }
        }

      if (processInd != 99)
      {
        ass_DailyUpload_Header.ProcessedInd = 2;
        ass_DailyUpload_Header.Error += err;
      }

      rVal = SQL.Atlas_GL_ASS_DailyUpload_Header_Update(ref ass_DailyUpload_Header);

      return rVal;
    }
          

    /// <summary>
    /// Returns text inside of curly braces, i.e. XXXCFSSDF bah dfksdfk {11323432/2423423423} = ' (11323432/2423423423)'
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string GetTextInsideCurlies(string value)
    {
      var cleaned = value.Trim();
      var pos1 = cleaned.IndexOf("{");
      var pos2 = cleaned.IndexOf("}", pos1 + 1);
      if (pos1 < 0 || pos2 < 0 || pos2 < pos1)
      {
        return string.Empty;
      }

      return string.Format(" ({0})", cleaned.Substring(pos1 + 1, pos2 - pos1 - 1).Trim());
    }

    private static string GetTextInsideCurliesClean(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        return null;
      }
      var cleaned = value.Trim();

      var pos1 = cleaned.IndexOf("{");
      var pos2 = cleaned.IndexOf("}", pos1 + 1);
      if (pos1 < 0 || pos2 < 0 || pos2 < pos1)
      {
        return string.Empty;
      }

      return cleaned.Substring(pos1 + 1, pos2 - pos1 - 1).Trim();
    }


    /// <summary>
    /// Returns trimmed text before opening curly brace. If no oening curly brace, returns whole trimmed string
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string GetTextBeforeCurlyOpen(string value)
    {
      var cleaned = value.Trim();
      var pos1 = cleaned.IndexOf("{");
      return (pos1 < 0) ? cleaned : cleaned.Substring(0, pos1 - 1).Trim();
    }



    #region Private vars

    // Log4net
    private static readonly ILogger _log = Log.Logger.ForContext<Upload_GL_Transactions>();

    #endregion

  }
}