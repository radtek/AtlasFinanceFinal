using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;

using Dapper;

using BLL.Accpac.GL;
using BLL.Accpac.CB;
using BLL.CustomerSpecific.Atlas.GL;
using Atlas.ACCPAC.BLL.Accpac.GL;
using Atlas.ACCPAC.BLL.Accpac.Atlas;


namespace BusinessLogicLayer.DAL
{

  public class SQL
  {
    #region Properties

    public static string IntegrationSupport
    {
      get
      {
        if (ConfigurationManager.ConnectionStrings["cnIntegration"] == null)
          throw (new NullReferenceException("ConnectionString configuration is missing from you web.config."));

        return ConfigurationManager.ConnectionStrings["cnIntegration"].ConnectionString;
      }
    }


    public static string Accpac
    {
      get
      {
        if (ConfigurationManager.ConnectionStrings["cnAccpac"] == null)
          throw (new NullReferenceException("ConnectionString configuration for Accpac is missing from you web.config."));

        return ConfigurationManager.ConnectionStrings["cnAccpac"].ConnectionString;
      }
    }

    #endregion


    #region Accpac GL

    // SELECT * FROM dbo.CSFSC WITH(NOLOCK) -> find fields where 'effectiveDate' between BGNDATEx and ENDDATEx (where x = 1..12)
    public static FiscalPeriod Get_Accpac_GL_FiscalPeriod(decimal effectiveDate)
    {
      if (_fiscalPeriod == null)
      {
        _fiscalPeriod = new List<FiscalPeriod>();
        using (var conn = new SqlConnection(Accpac))
        {
          conn.Open();

          for (var period = 1; period <= 13; period++)
          {
            _fiscalPeriod.AddRange(conn.Query<FiscalPeriod>(
              $"SELECT [FSCYEAR] AS FiscalYear, {period} as Period, " +
              $"[BGNDATE{period}] AS BeginDate, [ENDDATE{period}] AS EndDate " +
              $"FROM ATLINC.dbo.[CSFSC] c WITH(NOLOCK)"));
          }
        }
      }

      return _fiscalPeriod.FirstOrDefault(s => effectiveDate >= s.BeginDate && effectiveDate <= s.EndDate);
    }


    // Check if Valid GL Account
    public bool Check_Accpac_IsValid_GL_Account(string ACCTID, string formatted_ACCTID)
    {
      if (_accounts == null || _formattedAccounts == null)
      {
        using (var conn = new SqlConnection(Accpac))
        {
          conn.Open();
          _accounts = new HashSet<string>(conn.Query<string>("SELECT RTRIM([ACCTID]) FROM ATLINC.dbo.[GLAMF] WITH(NOLOCK)"));
          _formattedAccounts = new HashSet<string>(conn.Query<string>("SELECT RTRIM([ACCTFMTTD]) FROM ATLINC.dbo.[GLAMF] WITH(NOLOCK) "));
        }
      }

      if (!string.IsNullOrWhiteSpace(ACCTID) && _accounts.Contains(ACCTID.TrimEnd()))
      {
        return true;
      }

      if (!string.IsNullOrWhiteSpace(formatted_ACCTID) && _formattedAccounts.Contains(formatted_ACCTID.TrimEnd()))
      {
        return true;
      }

      return false;
    }


    public List<GLJournalDetail> Get_Accpac_GL_Get_JournalDetails(int InternalBatchID)
    {
      using (var conn = new SqlConnection(IntegrationSupport))
      {
        conn.Open();
        return conn.Query<GLJournalDetail>(
          "SELECT [InternalBatchDetailID], [InternalBatchID], [TransactionNr], [AccountNr], " +
          "ROUND([SourceCurrencyAmt], 2) AS SourceCurrencyAmt, [SourceCurrency], [Reference], [Description], " +
          "ISNULL([ProcessedDate], dbo.DATE(1900, 12, 31)) AS ProcessedDate, [ProcessedInd], [Error], [SourceDescription], [SourceReference] " +
          "FROM dbo.[Accpac_GLJournalDetail] WITH(NOLOCK) WHERE[InternalBatchID] = @InternalBatchID " +
          "ORDER BY [TransactionNr]", new { InternalBatchID }).ToList();
      }
    }


    public static string GLJournalDetail_Update(ref GLJournalDetail i, ref int myIndex, ref string progress)
    {
      myIndex += 1;

      using (var conn = new SqlConnection(IntegrationSupport))
      {
        conn.Open();

        conn.Execute("UPDATE [Accpac_GLJournalDetail] " +
          "SET [ProcessedInd] = @ProcessedInd, [Error] = @ErrorMsg, [SourceDescription] = @SourceDescription, [SourceReference] = @SourceReference, " +
          "[ProcessedDate] = getdate() " +
          "WHERE [InternalBatchDetailID] = @InternalBatchDetailID ", new
          {
            ProcessedInd = i.ProcessedInd,
            InternalBatchDetailID = i.InternalBatchDetailID,
            SourceDescription = i.SourceDescription.Trim(),
            SourceReference = i.SourceReference.Trim(),
            ErrorMsg = i.Error
          });

        return "";
      }
    }


    public static string GLJournalDetail_Insert(ref GLJournalDetail i, ref int myIndex, ref string progress)
    {
      using (var conn = new SqlConnection(IntegrationSupport))
      {
        conn.Open();

        var batchId = conn.Query<int>("SELECT MAX([InternalBatchDetailID]) FROM [Accpac_GLJournalDetail] WITH(NOLOCK)").FirstOrDefault();
        batchId++;

        conn.Execute("INSERT INTO [Accpac_GLJournalDetail] ([InternalBatchDetailID], [InternalBatchID], [TransactionNr], [AccountNr], " +
          "[SourceCurrencyAmt], [SourceCurrency], [Reference], [Description], [ProcessedDate], [ProcessedInd], [Error], [SourceDescription], " +
          "[SourceReference]) " +
          "VALUES (@NextID, @InternalBatchID, @TransactionNr, @AccountNr, @SourceCurrencyAmt, @SourceCurrency, @Reference, " +
          "@Description, getdate(), @ProcessedInd, @ErrorMsg, @SourceDescription, @SourceReference)",
          new
          {
            NextID = batchId,
            InternalBatchID = i.InternalBatchID,
            ProcessedInd = i.ProcessedInd,
            ErrorMsg = i.Error,
            SourceDescription = string.IsNullOrEmpty(i.SourceDescription) ? string.Empty : i.SourceDescription.Trim(),
            SourceReference = string.IsNullOrEmpty(i.SourceReference) ? string.Empty : i.SourceReference.Trim(),
            TransactionNr = i.TransactionNr.Trim(),
            AccountNr = i.AccountNr.Trim(),
            SourceCurrencyAmt = i.SourceCurrencyAmt,
            SourceCurrency = i.SourceCurrency.Trim(),
            Reference = i.Reference.Trim(),
            Description = i.Description.Trim()
          });
      }

      return "";
    }


    public List<JournalHeader> Get_Accpac_GL_Get_JournalHeaders(Int16 ProcessedInd)
    {
      using (var conn = new SqlConnection(IntegrationSupport))
      {
        conn.Open();

        var result = conn.Query<JournalHeader>(
          "SELECT [InternalBatchID], [EntryNr], [Description], [FiscalYear], [FiscalPeriod], [EntryDate], [SourceLedger], " +
          "[SourceType], [AutoRefersal], [DateCreated], [ProcessedDate], [ProcessedInd], [Error], [SourceDescription], [SourceReference] " +
          "FROM dbo.[Accpac_GLJournalHeader] WITH (NOLOCK) " +
          "WHERE [ProcessedInd] = @ProcessedInd " +
          "ORDER BY EntryNr", new { ProcessedInd }).ToList();

        foreach (var line in result)
        {
          line._journalDetails = Get_Accpac_GL_Get_JournalDetails(line.InternalBatchID);
        }
        return result.Any() ? result : null;
      }
    }


    public JournalHeader Get_Accpac_GL_Get_JournalHeader(Int32 InternalBatchID)
    {
      using (var conn = new SqlConnection(IntegrationSupport))
      {
        conn.Open();
        return conn.Query<JournalHeader>(
          "SELECT [InternalBatchID], [EntryNr], [Description], [FiscalYear], [FiscalPeriod], [EntryDate], [SourceLedger], " +
          "[SourceType], [AutoRefersal], [DateCreated], [ProcessedDate], [ProcessedInd], [Error], [SourceDescription], " +
          "[SourceReference] " +
          "FROM dbo.[Accpac_GLJournalHeader] WITH (NOLOCK) " +
          "WHERE [InternalBatchID] = @InternalBatchID", new { InternalBatchID }).FirstOrDefault();
      }
    }


    public static string GLJournalHeader_Update(ref JournalHeader i, ref int myIndex, ref string progress)
    {
      myIndex++;

      using (var conn = new SqlConnection(IntegrationSupport))
      {
        conn.Open();

        conn.Execute(
          "UPDATE [Accpac_GLJournalHeader] SET[ProcessedInd] = @ProcessedInd, [Error] = @ErrorMsg, " +
          "[SourceDescription] = @SourceDescription, [SourceReference] = @SourceReference, [ProcessedDate] = GETDATE() " +
          "WHERE [InternalBatchID] = @InternalBatchID",
        new
        {
          InternalBatchID = i.InternalBatchID,
          ProcessedInd = i.ProcessedInd,
          ErrorMsg = i.Error,
          SourceDescription = i.SourceDescription.Trim(),
          SourceReference = i.SourceReference.Trim()
        });
      }

      return "";
    }


    public static int GLJournalHeader_Insert(ref JournalHeader i, ref int myIndex, ref string progress)
    {
      var result = 0;
      myIndex++;

      using (var conn = new SqlConnection(IntegrationSupport))
      {
        conn.Open();
        result = conn.QueryFirst<int>(
         "INSERT INTO [Accpac_GLJournalHeader] " +
         "([InternalBatchID], [EntryNr], [Description], [FiscalYear], [FiscalPeriod], " +
         "[EntryDate], [SourceLedger], [SourceType], [AutoRefersal], [DateCreated], [ProcessedDate], [ProcessedInd], " +
         "[Error], [SourceDescription], [SourceReference] ) " +
         "VALUES( COALESCE((SELECT MAX([InternalBatchID]) + 1 FROM [Accpac_GLJournalHeader]), 1), " +
         "@EntryNr, @Description, @FiscalYear, @FiscalPeriod, " +
         "@EntryDate, @SourceLedger, @SourceType, @AutoRefersal, GETDATE(), GETDATE(), @ProcessedInd, " +
         "@ErrorMsg, @SourceDescription, @SourceReference)\r\n" +

         "SELECT MAX([InternalBatchID]) FROM [Accpac_GLJournalHeader]",
         new
         {
           EntryNr = i.EntryNr,
           Description = i.Description,
           FiscalYear = i.FiscalYear,
           FiscalPeriod = i.FiscalPeriod,
           EntryDate = i.EntryDate,
           SourceLedger = i.SourceLedger,
           SourceType = i.SourceType,
           AutoRefersal = i.AutoRefersal,
           ProcessedInd = i.ProcessedInd,
           ErrorMsg = i.Error,
           SourceDescription = i.SourceDescription,
           SourceReference = i.SourceReference
         });
      }

      return result;
    }

    #endregion


    #region Accpac CB

    public static CBBank Get_Accpac_CB_GetBankDetails(string bankAccount)
    {
      // KB: Fix for bad ASS bank code- 2011-10-10- issue fixed by Celia 22nd Sept 2011
      if (bankAccount.Equals("104163511"))
      {
        bankAccount = "1040163511";
      }

      if (_bankAccounts == null)
      {
        using (var conn = new SqlConnection(Accpac))
        {
          conn.Open();
          _bankAccounts = conn.Query<CBBank>("SELECT RTRIM([BANKACCTNO]) AS BankAccountNum, RTRIM([BANKNAME]) AS BankName, RTRIM([ACCTTRFCLR]) AS GLAccountClearing " +
            "FROM ATLINC.dbo.[CBBANK] WITH(NOLOCK) ").ToLookup(k => k.BankAccountNum, v => v);
        }
      }

      return _bankAccounts.Contains(bankAccount) ? _bankAccounts[bankAccount].First() : null;     
    }

    #endregion


    #region GL - Atlas

    public static List<BranchToLegalEntity> Get_Atlas_BranchToLegalEntity()
    {
      using (var conn = new SqlConnection(Accpac))
      {
        conn.Open();
        return conn.Query<BranchToLegalEntity>(
            "SELECT DISTINCT RTRIM(ACSEGVAL03) AS branchCode, RTRIM(ACSEGVAL04) AS legalEntity FROM dbo.GLAMF WITH(NOLOCK) " +
            "WHERE ACSEGVAL03 <> '' and ACTIVESW = 1 AND RTRIM(ACCTSEGVAL) = '0325'").ToList();
      }
    }


    public static List<ASS_DailyUpload_Detail> Get_Atlas_GL_Get_ASS_DailyUpload_Detail(string branchCode, decimal tranDate)
    {
      using (var conn = new SqlConnection(IntegrationSupport))
      {
        conn.Open();
        return conn.Query<ASS_DailyUpload_Detail>(
          "SELECT ADID, branchCode, tranDate as TransactionDate, transType, transDetailInd, description, additionalInfo, transAmt, " +
          "  tranVatAmt, ProcessedInd, ISNULL(ProcessedDate, dbo.DATE (1900, 12, 31)) AS ProcessedDate, Error " +
          "FROM dbo.ASS_DailyUpload_D WITH (NOLOCK)" +
          "WHERE branchCode = @branchCode AND tranDate = @tranDate AND transDetailInd = 1 " +
          "ORDER BY transType, transDetailInd DESC ", new { branchCode, tranDate }).ToList();
      }
    }


    public static string Atlas_GL_ASS_DailyUpload_Detail_Update(ref ASS_DailyUpload_Detail i, ref string progress)
    {
      using (var conn = new SqlConnection(IntegrationSupport))
      {
        conn.Open();
        conn.Execute(
        "UPDATE ASS_DailyUpload_D " +
        "SET ProcessedInd = @ProcessedInd, Error = @Error, ProcessedDate = getdate() " +
        "WHERE ADID = @ADID", new { ProcessedInd = i.ProcessedInd, ADID = i.ADID, Error = i.Error, });
      }

      return "";
    }


    public static List<ASS_DailyUpload_Header> Get_Atlas_GL_Get_ASS_DailyUpload_Headers(Int16 processedInd)
    {
      using (var conn = new SqlConnection(IntegrationSupport))
      {
        conn.Open();
        var result = conn.Query<ASS_DailyUpload_Header>(
          "SELECT RTRIM(LTRIM(CLASS_BR)) AS branchCode, TRDATE AS TransactionDate, AddedDate AS DateAdded, ProcessedInd, " +
          "  ISNULL(ProcessedDate, dbo.DATE(1900, 12, 31)) AS ProcessedDate, Error " +
          "FROM dbo.ASS_DailyUpload_Header WITH(NOLOCK) " +
          $"WHERE ProcessedInd = {processedInd} " +
          "  AND TRDATE = (SELECT TOP 1 TRDATE FROM ASS_DailyUpload_Header WITH(NOLOCK) WHERE ProcessedInd = 0 ORDER BY TRDATE) " +
          "ORDER BY TRDATE DESC, CLASS_BR", new { processedInd }).ToList();
        foreach (var row in result)
        {
          row.myASS_DailyUpload_Detail = SQL.Get_Atlas_GL_Get_ASS_DailyUpload_Detail(row.BranchCode.Trim(), row.TransactionDate);
        }

        return result;
      }
    }


    public static string Atlas_GL_ASS_DailyUpload_Header_Update(ref ASS_DailyUpload_Header i)
    {
      using (var conn = new SqlConnection(IntegrationSupport))
      {
        conn.Open();
        conn.Execute(
          "UPDATE ASS_DailyUpload_Header SET ProcessedInd = @ProcessedInd, Error = @Error, ProcessedDate = GETDATE()" +
          "WHERE RTRIM(LTRIM(CLASS_BR)) = @branchCode AND TRDATE = @tranDate",
        new { ProcessedInd = i.ProcessedInd, Error = i.Error, branchCode = i.BranchCode, tranDate = i.TransactionDate });
      }

      return "";
    }

    #endregion


    #region Private static fields (in-mem cache)

    private static ILookup<string, CBBank> _bankAccounts = null;

    private static HashSet<string> _accounts = null;
    private static HashSet<string> _formattedAccounts = null;

    private static List<FiscalPeriod> _fiscalPeriod = null;

    #endregion

  }
}