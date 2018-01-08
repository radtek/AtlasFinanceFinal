using System;
using System.Collections.Generic;

using WinServices.BLL;
using BusinessLogicLayer.DAL;
using AccpacCOMAPI;
using Serilog;

namespace BusinessLogicLayer.Accpac
{
  public class AccountingSession
  {
    #region Enum's

    public enum IC_Adustment_Detail_Types
    {
      QtyIncrease = 1,
      QtyDecrease = 2,
      CostIncrease = 3,
      CostDecrease = 4,
      BothIncrease = 5,
      BothDecrease = 6
    }

    #endregion

    #region Struc's


    #endregion

    #region Constructor

    public AccountingSession()
    {
      PopulateClass();
    }

    private void PopulateClass()
    {
      // DAL.SQL sql = new BusinessLogicLayer.DAL.SQL();
    }

    public void Dispose()
    {
      if (Session != null)
      {
        Session.Close();
        Session = null;
      }
    }

    #endregion

    #region Member variables

    private AccpacDBLink DBLinkCmpRW;

    private string _sessionHandle = "";

    #endregion

    #region Public Properties

    public AccpacSession Session { get; set; }

    #endregion

    #region Public Operations

    internal string OpenSession()
    {
      try
      {
        if (SharedMembers.UserName.Trim() == "")
        {
          return "";
        }

        _sessionHandle = "";
        Session = new AccpacSession();
        //_mSession.RemoteConnect(this.Server, this.SUser, this.SPsw, this.Domain);
        Session.Init(_sessionHandle, SharedMembers.ApplID, SharedMembers.ProgramName, SharedMembers.AccpacVersion);
        Session.Open(SharedMembers.UserName, SharedMembers.Psw, SharedMembers.CompanyDatabase, DateTime.Today.AddMonths(-1), 0, "");

        DBLinkCmpRW = (AccpacDBLink)Session.OpenDBLink(tagDBLinkTypeEnum.DBLINK_COMPANY, tagDBLinkFlagsEnum.DBLINK_FLG_READWRITE);

        return "";
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    internal string CloseSession()
    {
      try
      {
        if (Session != null)
        {
          Session.Close();
          Session = null;
        }

        return "Close";
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    #endregion

    #region GL Entries

    public string GL_Journal_Batch(List<Atlas.ACCPAC.BLL.Accpac.GL.JournalHeader> myJournalHeaders)
    {
      Boolean temp;

      try
      {
        if (SharedMembers.UserName.Trim() == "")
        {
          return "Please provide session details.";
        }

        if (Session == null)
        {
          return "Please provide session details.";
        }

        Session.Errors.Clear();

        if (myJournalHeaders == null)
        {
          return "No Entries were provided.";
        }

        if (myJournalHeaders.Count == 0)
        {
          return "No Entries were provided.";
        }

        // Check if valid trans to process before creating a GL batch
        int nrOfItemsToProcess = 0;
        foreach (var jh in myJournalHeaders)
        {
          string err1 = "";

          if (Validate_GLJournal(jh, ref err1) == true)
          {
            if (jh._journalDetails != null)
            {
              if (jh._journalDetails.Count > 0)
              {
                nrOfItemsToProcess++;
              }
            }
          }
        }

        if (nrOfItemsToProcess == 0)
        {
          return "No valid entries were provided to process.";
        }

        //MessageBox.Show("Passed Validate Journal");

        AccpacView GLBATCH1batch;
        AccpacViewFields GLBATCH1batchFields;
        DBLinkCmpRW.OpenView("GL0008", out GLBATCH1batch);
        GLBATCH1batchFields = GLBATCH1batch.Fields;

        AccpacView GLBATCH1header;
        AccpacViewFields GLBATCH1headerFields;
        DBLinkCmpRW.OpenView("GL0006", out GLBATCH1header);
        GLBATCH1headerFields = GLBATCH1header.Fields;

        AccpacView GLBATCH1detail1;
        AccpacViewFields GLBATCH1detail1Fields;
        DBLinkCmpRW.OpenView("GL0010", out GLBATCH1detail1);
        GLBATCH1detail1Fields = GLBATCH1detail1.Fields;

        AccpacView GLBATCH1detail2;
        AccpacViewFields GLBATCH1detail2Fields;
        DBLinkCmpRW.OpenView("GL0402", out GLBATCH1detail2);
        GLBATCH1detail2Fields = GLBATCH1detail2.Fields;


        object array1 = new AccpacViewClass[]{
                    (AccpacViewClass) GLBATCH1header
                };
        GLBATCH1batch.Compose(ref array1);

        object array2 = new AccpacViewClass[]{
                    (AccpacViewClass) GLBATCH1batch,
                    (AccpacViewClass) GLBATCH1detail1
                };
        GLBATCH1header.Compose(ref array2);

        object array3 = new AccpacViewClass[]{
                    (AccpacViewClass) GLBATCH1header,
                    (AccpacViewClass) GLBATCH1detail2
                };
        GLBATCH1detail1.Compose(ref array3);

        object array4 = new AccpacViewClass[]{
                    (AccpacViewClass) GLBATCH1detail1
                };
        GLBATCH1detail2.Compose(ref array4);


        GLBATCH1batch.Browse("BATCHSTAT = 1 OR BATCHSTAT = 6 OR BATCHSTAT = 9", true);
        temp = GLBATCH1batch.Exists;
        GLBATCH1batch.RecordCreate(tagViewRecordCreateEnum.VIEW_RECORD_CREATE_INSERT);
        GLBATCH1batch.Read();

        Object Base_1_Object = (Object)("1");
        Object Base_0_Object = (Object)("0");
        Object Base_00000_Object = (Object)("00000");

        GLBATCH1batchFields.get_FieldByName("PROCESSCMD").PutWithoutVerification(ref Base_1_Object);  // Lock Batch Switch
        GLBATCH1batch.Process();

        GLBATCH1headerFields.get_FieldByName("BTCHENTRY").PutWithoutVerification(ref Base_0_Object);  // Entry Number
        GLBATCH1header.Browse("", true);
        GLBATCH1header.Fetch();

        GLBATCH1headerFields.get_FieldByName("BTCHENTRY").PutWithoutVerification(ref Base_00000_Object);  // Entry Number

        // GL Batch
        string batchDescription = "Automated: Upload_GL_Transactions: Trans Date: " + myJournalHeaders[0].EntryDate;
        Object BTCHENTRY_Object = (Object)(batchDescription.Trim());
        GLBATCH1batchFields.get_FieldByName("BTCHDESC").PutWithoutVerification(ref BTCHENTRY_Object);  // Description

        //Object Base_BATCHSTAT_Object = (Object)("9");
        //GLBATCH1batchFields.get_FieldByName("BATCHSTAT").PutWithoutVerification(ref Base_BATCHSTAT_Object);  // Batch Status: 1 = Not ready to Post; 9 = Ready to post

        GLBATCH1batch.Update();

        // MessageBox.Show("Updated Journal Header");

        string err = "";

        Atlas.ACCPAC.BLL.Accpac.GL.JournalHeader jh1;
        foreach (var jh in myJournalHeaders)
        {
          err = "";
          if (Validate_GLJournal(jh, ref err) == true)
          {
            if (jh._journalDetails?.Count > 0)
            {
              _log.Information("ACCPAC posting starting: {Branch}", jh.Description);
              // Journal Header
              temp = GLBATCH1header.Exists;
              GLBATCH1header.RecordCreate(tagViewRecordCreateEnum.VIEW_RECORD_CREATE_NOINSERT);
              temp = GLBATCH1header.Exists;

              Object DATE_ENTRY_Object = (Object)(SharedMembers.SetDateFromDB(jh.EntryDate.ToString()));
              GLBATCH1headerFields.get_FieldByName("DATEENTRY").set_Value(ref DATE_ENTRY_Object);    // Entry Date

              Object SRCETYPE_Object = (Object)(jh.SourceType.Trim());
              GLBATCH1headerFields.get_FieldByName("SRCETYPE").set_Value(ref SRCETYPE_Object);    // Source Type

              temp = GLBATCH1detail1.Exists;
              GLBATCH1detail1.RecordClear();
              temp = GLBATCH1detail1.Exists;

              foreach (var jd in jh._journalDetails)
              {
                if (jd.SourceCurrencyAmt != 0)
                {
                  // Journal Detail
                  GLBATCH1detail1.Read();
                  temp = GLBATCH1detail1.Exists;
                  GLBATCH1detail1.RecordCreate(tagViewRecordCreateEnum.VIEW_RECORD_CREATE_DELAYKEY);

                  Object ACCTID_Object = (Object)(jd.AccountNr.Trim());
                  GLBATCH1detail1Fields.get_FieldByName("ACCTID").set_Value(ref ACCTID_Object);  // Account Number

                  //Object SCURNAMT_Object = (Object)(jd.SourceCurrencyAmt.ToString());
                  //GLBATCH1detail1Fields.get_FieldByName("SCURNAMT").set_Value(ref SCURNAMT_Object);   // Source Currency Amount

                  Object SCURNAMT_Object = (Object)(jd.SourceCurrencyAmt.ToString());
                  GLBATCH1detail1Fields.get_FieldByName("TRANSAMT").set_Value(ref SCURNAMT_Object);   // Amount

                  Object TRANSDESC_Object = (Object)(jd.Description.Trim());
                  GLBATCH1detail1Fields.get_FieldByName("TRANSDESC").set_Value(ref TRANSDESC_Object);   // Description

                  Object TRANSREF_Object = (Object)(jd.Reference.Trim());
                  GLBATCH1detail1Fields.get_FieldByName("TRANSREF").set_Value(ref TRANSREF_Object);   // Reference

                  Object COMMENT_Object = (Object)("");
                  GLBATCH1detail1Fields.get_FieldByName("COMMENT").set_Value(ref COMMENT_Object);   // Comment

                  GLBATCH1detail1.Insert();
                }

                jd.ProcessedDate = DateTime.Today;
                jd.ProcessedInd = 1;
                jd.Error = "";
              }

              GLBATCH1detail1.Read();
              GLBATCH1batch.Read();
              temp = GLBATCH1header.Exists;

              Object JRNLDESC_Object = (Object)(jh.Description.Trim());
              GLBATCH1headerFields.get_FieldByName("JRNLDESC").PutWithoutVerification(ref JRNLDESC_Object);  // Description

              GLBATCH1header.Insert();
              GLBATCH1header.Read();
              temp = GLBATCH1header.Exists;

              GLBATCH1batch.Read();
              GLBATCH1headerFields.get_FieldByName("BTCHENTRY").PutWithoutVerification(ref Base_00000_Object);  // Entry Number

              temp = GLBATCH1header.Exists;
              GLBATCH1header.RecordCreate(tagViewRecordCreateEnum.VIEW_RECORD_CREATE_NOINSERT);
              temp = GLBATCH1header.Exists;

              jh.ProcessedInd = 1;
              jh.Error = "";
              jh.ProcessedDate = DateTime.Today;
              jh1 = jh;
              Atlas.ACCPAC.BLL.Accpac.GL.Upload_GL_Transactions.GLJournalHeader_Update(ref jh1);
            }
            _log.Information("ACCPAC posting completed: {Branch}", jh.Description);
          }
        }

        return "Completed";

      }
      catch (Exception ex)
      {
        CloseSession();
        return ex.Message;
      }
    }

    public Boolean Validate_GLJournal(Atlas.ACCPAC.BLL.Accpac.GL.JournalHeader jh, ref string err)
    {
      try
      {
        Boolean rVal = true;

        if (jh == null)
        {
          rVal = false;
          err = "Null JournalHeader passed. ";
        }

        if (jh._journalDetails == null)
        {
          rVal = false;
          err += "Null JournalHeader Details passed. ";
        }
        else if (jh._journalDetails.Count == 0)
        {
          rVal = false;
          err += "No JournalHeader Details passed. ";
        }

        decimal amt = 0;
        int ii = 0;

        if (jh._journalDetails != null)
        {
          var sql = new SQL();

          if (jh._journalDetails.Count > 0)
          {
            foreach (var jd in jh._journalDetails)
            {
              amt += jd.SourceCurrencyAmt;

              if (jd.SourceCurrencyAmt > 0)
              {
                ii += 1;
              }

              if (jd.ProcessedInd == 2)
              {
                err += String.Format(" Error was recorded for Journal detail: {0}. ", jd.InternalBatchDetailID);
                rVal = false;
              }

              if (jd.ProcessedInd == 1)
              {
                err += String.Format(" Already processed was recorded for Journal detail: {0}. ", jd.InternalBatchDetailID);
                rVal = false;
              }

              if (sql.Check_Accpac_IsValid_GL_Account(jd.AccountNr.Trim().Replace("-", ""), jd.AccountNr.Trim()) == false)
              {
                err += String.Format(" GL Account: {0} is an invalid GL Account. ", jd.AccountNr);
                rVal = false;
              }
            }
          }
        }

        if (amt != 0)
        {
          err += string.Format("Journal is out of Balance: {0}", amt);
          rVal = false;
        }

        if (ii == 0)
        {
          err += "Journal Has no values greater or less than 0. ";
          rVal = false;
        }

        if (err.Trim() != "")
        {
          jh.ProcessedInd = 2;
          jh.Error = "Validate_GLJournal: Error - " + err.Trim();
          jh.ProcessedDate = DateTime.Today;
          Atlas.ACCPAC.BLL.Accpac.GL.Upload_GL_Transactions.GLJournalHeader_Update(ref jh);
        }

        return rVal;
      }
      catch (Exception ex)
      {
        jh.ProcessedInd = 2;
        jh.Error = "Validate_GLJournal: Error - " + ex.Message;
        jh.ProcessedDate = DateTime.Today;
        Atlas.ACCPAC.BLL.Accpac.GL.Upload_GL_Transactions.GLJournalHeader_Update(ref jh);

        return false;
      }
    }

    public string FormatTransNrWithNeg(int i)
    {
      try
      {
        string rVal = "";
        rVal = i.ToString("000000000");
        return "-" + rVal;
      }
      catch
      {
        return "-000000002";
      }
    }
    #endregion


    private static readonly ILogger _log = Log.Logger.ForContext<AccountingSession>();

  }

}
