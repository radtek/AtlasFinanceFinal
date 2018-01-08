using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Globalization;

using Serilog;
using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.ThirdParty.CompuScan.Batch.XML;
using Atlas.Common.Utils;
using Atlas.Common.ExceptionBase;
using Atlas.Common.Extensions;
using Atlas.ThirdParty.CompuScan.Batch.XML.Response;
using Atlas.Domain.DTO;

using Atlas.Enumerators;



namespace Atlas.ThirdParty.CompuScan.Batch
{
  public sealed class BatchServletImpl : IDisposable
  {
    #region Private Properties

    private static readonly ILogger _log = Log.Logger.ForContext<BatchServletImpl>();
    private Risk.BatchDestination _url { get; set; }

    #region Core Servlet Parameters

    private string p_ReqType { get; set; }
    private string p_Username { get; set; }
    private string p_Password { get; set; }
    private string p_Branch_Code { get; set; }
    private string p_FileType { get; set; }
    private string p_ENQ_Resp_Format { get; set; }
    private string p_Job_ID { get; set; }

    #endregion

    #endregion


    #region Error Codes

    internal Dictionary<int, string> _errorDict = new Dictionary<int, string>(){
                                                     {-300, "All Variables not filled in"},
                                                     {-301, "Problems with properties file"},
                                                     {-302, "Problems with decoding Data (The Uploaded content is empty)"},
                                                     {-303, "Could not allocate User Table Object"},
                                                     {-304, "File type not supported"},
                                                     {-305, "File format not correct"},
                                                     {-306, "Could not get a JOB ID"},
                                                     {-307, "Problems writing batch file"},
                                                     {-308, "Particular Job missing"},
                                                     {-309, "Response file not ready yet"},
                                                     {-310, "Problems reading response file."},
                                                     {-311, "Incorrect Branch User"},
                                                     {-312, "Problems setting Collection date"},
                                                     {-313, "No Batch Records found"},
                                                     {-314, "Problems cancelling job"},
                                                     {-315, "Batches busy processing, cannot cancel job now"},
                                                     {-403, "Invalid Username or Password" }};

    #endregion


    #region Internal Methods

    internal void UpdateBatchSubmission(long batchId, long? jobId, Risk.BatchJobStatus? status,
                                                 DateTime? batchDeliveryDate, DateTime? submissionDate,
                                                       DateTime? LastStatusUpdateDate, string errorMsg)
    {
      using (var UoW = new UnitOfWork())
      {
        var batchSubmission = new XPQuery<BUR_BatchSubmission>(UoW)
                                                 .FirstOrDefault(o => o.Batch.BatchId == batchId);

        // If error code happens to exist append to submission record
        if (!string.IsNullOrEmpty(errorMsg))
          batchSubmission.ErrorMessage = errorMsg;

        // Set the response job id from the provider
        if (jobId != null)
          batchSubmission.JobId = (long)jobId;

        if (submissionDate != null)
          batchSubmission.SubmissionDate = submissionDate;

        if (LastStatusUpdateDate != null)
          batchSubmission.LastStatusUpdateDate = LastStatusUpdateDate;

        if (status != null)
          batchSubmission.Status = (Risk.BatchJobStatus)status;

        if (batchDeliveryDate != null)
        {
          // We set the delivery date to ensure the same batch is not processed multiple times.
          var batch = new XPQuery<BUR_Batch>(UoW).FirstOrDefault(o => o.BatchId == batchId);

          if (batch != null)
          {
            batch.DeliveryDate = DateTime.Now;
          }
        }

        UoW.CommitChanges();
      }
    }


    internal void ProcessCollection(ref trans_data transData, List<BUR_BatchItem> batchItemCollection)
    {
      // Determine all the different items for the particular batc
      foreach (var batchItem in batchItemCollection)
      {
        switch (batchItem.TransType)
        {
          #region Enquiry
          case Risk.BatchTransactionType.Enquiry:
            switch (batchItem.SubTransType)
            {
              case Risk.BatchSubTransactionType.Global:
                if (transData.CSENQ_GLOBAL2 == null)
                  transData.CSENQ_GLOBAL2 = new List<CSENQ_GLOBAL2>();

                transData.CSENQ_GLOBAL2.Add(((CSENQ_GLOBAL2)Xml.DeSerialize<CSENQ_GLOBAL2>(Compression.Decompress(batchItem.XML))));
                break;
              case Risk.BatchSubTransactionType.Conflict:
                if (transData.CSENQ_CONFLICT == null)
                  transData.CSENQ_CONFLICT = new List<CSENQ_CONFLICT>();

                transData.CSENQ_CONFLICT.Add(((CSENQ_CONFLICT)Xml.DeSerialize<CSENQ_CONFLICT>(Compression.Decompress(batchItem.XML))));
                break;
              default:
                break;
            }
            break;

          #endregion

          #region NLR
          case Risk.BatchTransactionType.NLR:
            switch (batchItem.SubTransType)
            {
              case Risk.BatchSubTransactionType.Loan:
                if (transData.NLR_LOANREG == null)
                  transData.NLR_LOANREG = new List<NLR_LOANREG>();

                transData.NLR_LOANREG.Add(((NLR_LOANREG)Xml.DeSerialize<NLR_LOANREG>(Compression.Decompress(batchItem.XML))));

                break;
              case Risk.BatchSubTransactionType.Loan2:
                if (transData.NLR_LOANREG2 == null)
                  transData.NLR_LOANREG2 = new List<NLR_LOANREG2>();

                transData.NLR_LOANREG2.Add(((NLR_LOANREG2)Xml.DeSerialize<NLR_LOANREG2>(Compression.Decompress(batchItem.XML))));

                break;
              case Risk.BatchSubTransactionType.LoanClose:
                if (transData.NLR_LOANCLOSE == null)
                  transData.NLR_LOANCLOSE = new List<NLR_LOANCLOSE>();

                transData.NLR_LOANCLOSE.Add(((NLR_LOANCLOSE)Xml.DeSerialize<NLR_LOANCLOSE>(Compression.Decompress(batchItem.XML))));

                break;
              case Risk.BatchSubTransactionType.BatB2:
                if (transData.NLR_BATB2 == null)
                  transData.NLR_BATB2 = new List<NLR_BATB2>();

                transData.NLR_BATB2.Add(((NLR_BATB2)Xml.DeSerialize<NLR_BATB2>(Compression.Decompress(batchItem.XML))));

                break;
              default:
                break;
            }
            break;

          #endregion

          #region Registration

          case Risk.BatchTransactionType.Registration:
            switch (batchItem.SubTransType)
            {
              case Risk.BatchSubTransactionType.Client:
                if (transData.CSREG_CLIENT == null)
                  transData.CSREG_CLIENT = new List<CSREG_CLIENT>();

                transData.CSREG_CLIENT.Add(((CSREG_CLIENT)Xml.DeSerialize<CSREG_CLIENT>(Compression.Decompress(batchItem.XML))));

                break;
              case Risk.BatchSubTransactionType.Loan:
                if (transData.CSREG_LOAN == null)
                  transData.CSREG_LOAN = new List<CSREG_LOAN>();

                transData.CSREG_LOAN.Add(((CSREG_LOAN)Xml.DeSerialize<CSREG_LOAN>(Compression.Decompress(batchItem.XML))));

                break;
              case Risk.BatchSubTransactionType.Payment:
                if (transData.CSREG_PAYMENT == null)
                  transData.CSREG_PAYMENT = new List<CSREG_PAYMENT>();

                transData.CSREG_PAYMENT.Add(((CSREG_PAYMENT)Xml.DeSerialize<CSREG_PAYMENT>(Compression.Decompress(batchItem.XML))));
                break;
              case Risk.BatchSubTransactionType.Address:
                if (transData.CSREG_ADDRESS == null)
                  transData.CSREG_ADDRESS = new List<CSREG_ADDRESS>();

                transData.CSREG_ADDRESS.Add(((CSREG_ADDRESS)Xml.DeSerialize<CSREG_ADDRESS>(Compression.Decompress(batchItem.XML))));

                break;
              case Risk.BatchSubTransactionType.Telephone:
                if (transData.CSREG_TELEPHONE == null)
                  transData.CSREG_TELEPHONE = new List<CSREG_TELEPHONE>();

                transData.CSREG_TELEPHONE.Add(((CSREG_TELEPHONE)Xml.DeSerialize<CSREG_TELEPHONE>(Compression.Decompress(batchItem.XML))));

                break;
              case Risk.BatchSubTransactionType.Employer:
                if (transData.CSREG_EMPLOYER == null)
                  transData.CSREG_EMPLOYER = new List<CSREG_EMPLOYER>();

                transData.CSREG_EMPLOYER.Add(((CSREG_EMPLOYER)Xml.DeSerialize<CSREG_EMPLOYER>(Compression.Decompress(batchItem.XML))));

                break;
              case Risk.BatchSubTransactionType.Global:
                if (transData.CSENQ_GLOBAL2 == null)
                  transData.CSENQ_GLOBAL2 = new List<CSENQ_GLOBAL2>();

                transData.CSENQ_GLOBAL2.Add(((CSENQ_GLOBAL2)Xml.DeSerialize<CSENQ_GLOBAL2>(Compression.Decompress(batchItem.XML))));

                break;
              case Risk.BatchSubTransactionType.Conflict:
                if (transData.CSENQ_CONFLICT == null)
                  transData.CSENQ_CONFLICT = new List<CSENQ_CONFLICT>();

                transData.CSENQ_CONFLICT.Add(((CSENQ_CONFLICT)Xml.DeSerialize<CSENQ_CONFLICT>(Compression.Decompress(batchItem.XML))));

                break;

              default:
                break;
            }
            break;

          #endregion

          case Risk.BatchTransactionType.Update:
            {
              switch (batchItem.SubTransType)
              {
                case Risk.BatchSubTransactionType.Client:
                  if (transData.CSUPD_CLIENT == null)
                    transData.CSUPD_CLIENT = new List<CSUPD_CLIENT>();

                  transData.CSUPD_CLIENT.Add(((CSUPD_CLIENT)Xml.DeSerialize<CSUPD_CLIENT>(Compression.Decompress(batchItem.XML))));

                  break;
                case Risk.BatchSubTransactionType.Loan:
                  if (transData.CSUPD_LOAN == null)
                    transData.CSUPD_LOAN = new List<CSUPD_LOAN>();

                  transData.CSUPD_LOAN.Add(((CSUPD_LOAN)Xml.DeSerialize<CSUPD_LOAN>(Compression.Decompress(batchItem.XML))));

                  break;
                default:
                  break;
              }
            }
            break;
        }
      }
    }


    internal PropertyInfo[] GetProperties()
    {
      // Get all the properties of the current class
      return this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
    }


    internal Dictionary<object, object> PropertyDictionary(PropertyInfo[] propertyInfos)
    {
      return propertyInfos.Where(propertyInfo => propertyInfo.CanRead && propertyInfo.GetValue(this, null) != null
                    && propertyInfo.Name != "_url" && propertyInfo.Name.StartsWith("p"))
                    .ToDictionary(propertyInfo => (object)propertyInfo.Name,
                              propertyInfo => ((object)Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(propertyInfo.GetValue(this, null).ToString()))));
    }


    internal void StandardParameters(long branchId, string requestType, Risk.BatchDestination dest)
    {
      using (var UoW = new UnitOfWork())
      {
        var burService = new XPQuery<BUR_Service>(UoW).FirstOrDefault(p => p.Branch.BranchId == branchId);

        if (burService == null)
          throw new Exception(string.Format("No credentials configured for branch {0} for Request {1}", branchId, requestType));

        this.p_Username = burService.Username; //"1949-1";
        this.p_Password = burService.Password; // "ATLAS1";
        this.p_ReqType = requestType;
        this.p_Branch_Code = burService.BranchCode; // "1949";
        this._url = dest;
      }
    }


    public void RetrieveJob()
    {
      using (var UoW = new UnitOfWork())
      {
        var batchProcessingCollection = new XPQuery<BUR_BatchSubmission>(UoW)
          .Where(o => o.Status == Risk.BatchJobStatus.Response_Ready).OrderBy(b => b.SubmissionDate).ToList();

        if (batchProcessingCollection.Count == 0)
          return;

        foreach (var jobItem in batchProcessingCollection)
        {
          this.StandardParameters(jobItem.Batch.Branch.BranchId, "ResponseFile", Risk.BatchDestination.LIVE);

          var propertyInfos = this.GetProperties();
          // New instance of the poster class 
          var post = new Http { Url = this._url.ToStringEnum(), TimeOut = 200000 };

          this.p_Job_ID = jobItem.JobId.ToString();

          var requestDict = this.PropertyDictionary(propertyInfos);

          // Add the required post variables into the poster
          requestDict.ToList().ForEach(i => post.PostItems.Add(i.Key.ToString(), i.Value.ToString()));

          // We know we are posting so set num
          post.Type = Http.PostTypeEnum.Post;

          try
          {
            // Do the actual post against the url
            var postResult = post.Post();

            var errorMsg = string.Empty;

            if (!StringUtils.IsBase64String(postResult))
            {
              if (_errorDict.ContainsKey(int.Parse(postResult)))
                errorMsg = _errorDict[int.Parse(postResult)];
            }

            if (string.IsNullOrEmpty(errorMsg))
              this.WriteToDisk(postResult, jobItem.BatchSubmissionId);
          }
          catch (Exception ex)
          {
            _log.Error(string.Format("[RetrieveJob] error picking up batch: {0} - {1}", ex.Message, ex.StackTrace));
            var batch =
              new XPQuery<BUR_BatchSubmission>(UoW).FirstOrDefault(o => o.BatchSubmissionId == jobItem.BatchSubmissionId);

            if (batch != null)
              batch.ErrorMessage = ex.Message;

            UoW.CommitChanges();
          }
        }
      }
    }


    /// <summary>
    /// Write the file to disc after decoding it
    /// </summary>
    /// <param name="encodedData"></param>
    internal void WriteToDisk(string encodedData, long batchSubmissionId)
    {
      // Decode the string from base64 into byte array
      byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);

      // create new guid, and just replace the -
      string folderName = string.Empty;

      folderName = Guid.NewGuid().ToString().Replace("-", "").ToLower();

      // Create zip storage path if required
      if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ZipStorage")))
        Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ZipStorage"));

      // Write all bytes to disk with guid as file name
      string zipFile = string.Format("{0}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"ZipStorage\{0}.zip", Guid.NewGuid())));

      using (var fs = new FileStream(zipFile, FileMode.Create, FileAccess.Write))
      {
        // Writes a block of bytes to this stream using data from a byte array.
        fs.Write(encodedDataAsBytes, 0, encodedDataAsBytes.Length);
        fs.Flush();
        // close file stream
        fs.Close();
      }

      string combinedZipPath = AppDomain.CurrentDomain.BaseDirectory;

      // Check to see if it exists, else create
      if (!Directory.Exists(combinedZipPath))
        Directory.CreateDirectory(combinedZipPath);

      // Create a path for the received file with its own name
      string zipExtractionPath = Path.Combine(combinedZipPath, string.Format(@"{0}\{1}\{2}\{3}\", DateTime.Now.Year, DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"), folderName));

      // Remove the directory if it exists, this could happen if the loan failed for some reason and they are re-attempting an enquiry.
      if (Directory.Exists(zipExtractionPath))
        Directory.Delete(zipExtractionPath, true);

      Directory.CreateDirectory(zipExtractionPath);

      // Unzip the file to specified path
      using (var zip = new Ionic.Zip.ZipFile(zipFile))
      {
        zip.ExtractAll(zipExtractionPath);
      }

      this.ProcessExtraction(zipExtractionPath, batchSubmissionId);

      // Remove Directory
      Directory.Delete(zipExtractionPath, true);

      // Remove Zip file
      File.Delete(zipFile);

    }


    internal void ProcessExtraction(string filePath, long batchSubmissionId)
    {

      foreach (var file in Directory.GetFiles(filePath, "*.xml"))
      {
        if (file.Contains("Resp_"))
        {
          var responseObject = (TRANS_DATA)Xml.DeSerialize<TRANS_DATA>(File.ReadAllText(file));
          using (var UoW = new UnitOfWork())
          {
            List<long> processSubmissionBatchId = new List<long>();

            foreach (var item in responseObject.Items)
            {
              var batchSubmissionItemCollection = new XPQuery<BUR_BatchSubmissionItem>(UoW).Where(o => o.SubmissionBatch.BatchSubmissionId == batchSubmissionId).ToList();


              if (batchSubmissionItemCollection.Count == 0)
              {
                _log.Fatal(string.Format("[ProcessExtraction] - No items found in batch submission {0}", batchSubmissionId));
                break;
              }

              var batchSubmissionItem = batchSubmissionItemCollection.FirstOrDefault(p => p.BatchReferenceNo == item.REFERENCE_NO);

              if (batchSubmissionItem == null)
              {
                _log.Fatal(string.Format("[ProcessExtraction] - Unable to locate batch submission item with reference no {0}", item.REFERENCE_NO));

                continue;
              }
              _log.Information("[ProcessExtraction] - Located batch submission item with reference no {0}", item.REFERENCE_NO);

              batchSubmissionItem.ResponseXML = Compression.Compress(Xml.Serialize<TRANS_DATARECORD>(item));
              batchSubmissionItem.TransmissionStatus = item.TRANS_STATUS;
              if (Convert.ToInt32(item.ERROR_CODE) > 0)
              {
                batchSubmissionItem.ErrorCode = Convert.ToInt32(item.ERROR_CODE);
                batchSubmissionItem.ErrorMessage = item.MESSAGE;
              }

              if (!processSubmissionBatchId.Contains(batchSubmissionItem.SubmissionBatch.BatchSubmissionId))
                processSubmissionBatchId.Add(batchSubmissionItem.SubmissionBatch.BatchSubmissionId);

              batchSubmissionItem.Save();
              UoW.CommitChanges();
            }

            if (processSubmissionBatchId.Count == 0)
            {
              var submissionBatch = new XPQuery<BUR_BatchSubmission>(UoW).FirstOrDefault(o => o.BatchSubmissionId == batchSubmissionId);
              if (submissionBatch == null)
                throw new RecordNotFoundException(string.Format("Submission parent record not found {0}", batchSubmissionId));

              submissionBatch.Status = Risk.BatchJobStatus.Job_Error;

              UoW.CommitChanges();
            }

            foreach (var item in processSubmissionBatchId)
            {
              var batchSubmissionItemCollection = new XPQuery<BUR_BatchSubmissionItem>(UoW).Where(o => o.SubmissionBatch.BatchSubmissionId == item &&
                             o.TransmissionStatus == null || o.TransmissionStatus == string.Empty).ToList();

              var submissionBatch = new XPQuery<BUR_BatchSubmission>(UoW).FirstOrDefault(o => o.BatchSubmissionId == item);

              if (submissionBatch == null)
                throw new RecordNotFoundException(string.Format("Submission parent record not found {0}", item));

              if (batchSubmissionItemCollection.Count > 0)
                submissionBatch.Status = Risk.BatchJobStatus.Response_Item_Problem;
              else
                submissionBatch.Status = Risk.BatchJobStatus.Job_Processed;

              UoW.CommitChanges();
            }
          }
        }
      }
    }

    #endregion


    #region Public Methods

    public Int64 CreateBatchItem(long batchId, string uniqueRefNo, Risk.BatchTransactionType transactionType, Risk.BatchSubTransactionType subTransactionType, byte[] xml)
    {
      using (var UoW = new UnitOfWork())
      {
        var batchItem = new BUR_BatchItem(UoW)
        {
          Batch = new XPQuery<BUR_Batch>(UoW).Where(o => o.BatchId == batchId).FirstOrDefault(),
          TransType = transactionType,
          SubTransType = subTransactionType,
          BatchReferenceNo = uniqueRefNo,
          XML = xml,
          CreateDate = DateTime.Now
        };

        UoW.CommitChanges();

        return batchItem.BatchItemId;
      }
    }


    /// <summary>
    /// Create association for branch and unique reference
    /// </summary>
    public bool CreateAssociation(string legacyBranchum, string uniqueRefNo, string seqNo)
    {
      using (var UoW = new UnitOfWork())
      {
        var branch = new XPQuery<BRN_Branch>(UoW).FirstOrDefault(o => o.LegacyBranchNum.Trim().PadLeft(3, '0') == legacyBranchum.Trim().PadLeft(3, '0'));

        if (branch == null)
          throw new RecordNotFoundException(string.Format("Branch {0} does not exist in the database", legacyBranchum.Trim().PadLeft(3, '0')));

        var checkAssociation = new XPQuery<BUR_SubmissionAssociation>(UoW).FirstOrDefault(o => o.SubmissionAssociation.SeqNo == seqNo && o.SubmissionAssociation.Branch.BranchId
          == branch.BranchId);

        if (checkAssociation != null)
        {
          _log.Warning("Record with seqno {0} already exists for branch {1}", seqNo, legacyBranchum.Trim().PadLeft(3, '0'));
          return false;
        }

        var batchAssociation = new BUR_SubmissionAssociation(UoW);
        batchAssociation.SubmissionAssociation.SeqNo = seqNo;
        batchAssociation.SubmissionAssociation.UniqueRefNo = uniqueRefNo;
        batchAssociation.SubmissionAssociation.Branch = branch;

        UoW.CommitChanges();

        return true;
      }
    }


    /// <summary>
    /// Creates a new batch if one doesnt exist for the day
    /// </summary>
    /// <param name="legacyBranchNum">Branch on which to create the batch</param>
    public long CreateBatch(string legacyBranchNum)
    {
      using (var UoW = new UnitOfWork())
      {
        var branch = new XPQuery<BRN_Branch>(UoW).FirstOrDefault(o => o.LegacyBranchNum.Trim().PadLeft(3, '0') == legacyBranchNum.Trim().PadLeft(3, '0'));

        if (branch == null)
          throw new RecordNotFoundException(string.Format("Branch {0} was not found in the database", legacyBranchNum.Trim().PadLeft(3, '0')));

        // Only add to existing batch if it has not been delivered.
        var batch = new XPQuery<BUR_Batch>(UoW).FirstOrDefault(o => o.Branch.BranchId == branch.BranchId
                                                                 && o.CreatedDate.Value.Date == DateTime.Now.Date && o.DeliveryDate == null);

        Func<BUR_Batch> createBatch = () =>
        {
          var bat = new BUR_Batch(UoW)
          {
            Branch = branch,
            CreateUser = new XPQuery<PER_Person>(UoW).FirstOrDefault(p => p.PersonId == 0),
            CreatedDate = DateTime.Now,
            Locked = false
          };
          UoW.CommitChanges();
          return bat;
        };

        if (batch == null)
        {
          return createBatch().BatchId;
        }
        else if (batch.Locked)
        {
          return createBatch().BatchId;
        }
        else
        {
          return batch.BatchId;
        }
      }
    }


    public void RetrieveJobStatus()
    {
      #region Determine  batch submission status

      using (var UoW = new UnitOfWork())
      {
        var jobStatuses = new List<Risk.BatchJobStatus>();

        jobStatuses.Add(Risk.BatchJobStatus.Job_Is_Queued);
        jobStatuses.Add(Risk.BatchJobStatus.Job_Pending);
        jobStatuses.Add(Risk.BatchJobStatus.Job_Staging);
        jobStatuses.Add(Risk.BatchJobStatus.Pending_Pickup);
        jobStatuses.Add(Risk.BatchJobStatus.Job_Is_Finished);

        var pendingBatchCollection = new XPQuery<BUR_BatchSubmission>(UoW)
          .Where(o => jobStatuses.Contains(o.Status) && o.JobId > 0).OrderBy(o => o.SubmissionDate).ToList();

        foreach (var pendingItem in pendingBatchCollection)
        {
          this.StandardParameters(pendingItem.Batch.Branch.BranchId, "JobStatus", Risk.BatchDestination.LIVE);

          #region Core Parameters

          // Get all the properties of the current class
          var propertyInfos = this.GetProperties();

          #endregion

          // New instance of the poster class 
          var post = new Http { Url = this._url.ToStringEnum(), TimeOut = 200000 };

          this.p_Job_ID = pendingItem.JobId.ToString();

          var requestDict = this.PropertyDictionary(propertyInfos);

          // Add the required post variables into the poster
          requestDict.ToList().ForEach(i => post.PostItems.Add(i.Key.ToString(), i.Value.ToString()));

          // We know we are posting so set num
          post.Type = Http.PostTypeEnum.Post;

          try
          {
            // Do the actual post against the url
            var postResult = post.Post();

            _log.Information("[RetrieveJobStatus] Request: {@requestDict}, Post result: {postResult}", requestDict,
              postResult);
            int postCode;
            if (!int.TryParse(postResult, NumberStyles.Integer, CultureInfo.InvariantCulture, out postCode))
            {
              _log.Error("[RetrieveJobStatus] Invalid response: {PostResult}", postResult);
            }

            var status = Risk.BatchJobStatus.Job_Error;
            string errorMsg = null;
            if (_errorDict.ContainsKey(postCode))
            {
              errorMsg = _errorDict[int.Parse(postResult)];
            }

            if (string.IsNullOrWhiteSpace(errorMsg) && !string.IsNullOrWhiteSpace(postResult))
            {
              Enum.TryParse<Risk.BatchJobStatus>(postResult, out status);
            }

            UpdateBatchSubmission(pendingItem.Batch.BatchId, null,
              string.IsNullOrWhiteSpace(errorMsg) ? status : (Risk.BatchJobStatus?)null, null, null, DateTime.Now,
              errorMsg);
          }
          catch (Exception ex)
          {
            _log.Error(string.Format("[RetrieveJobStatus] error posting to Provider: {0} - {1}", ex.Message, ex.StackTrace));
            var batch =
              new XPQuery<BUR_BatchSubmission>(UoW).FirstOrDefault(
                o => o.BatchSubmissionId == pendingItem.BatchSubmissionId);

            if (batch != null)
              batch.ErrorMessage = ex.Message;

            UoW.CommitChanges();
          }
        }
      }

      #endregion

      // Lee: commented the below line, since it will now be called from the quartz job directly
      //this.RetrieveJob();
    }


    /// <summary>r
    /// Function to determine what batches need to be sent
    /// </summary>
    public void DeliverBatch(bool ignoreLock)
    {
      #region Authentication Parameters

      this.p_ENQ_Resp_Format = "XML";
      this.p_FileType = "XML";

      #endregion

      #region Determine what batch data to send

      // Create instance of transport data xml
      trans_data transData = null;

      // Collections
      Dictionary<BUR_BatchDTO, trans_data> submissionData = null;

      using (var UoW = new UnitOfWork())
      {
        // Get all pending batches to be sent.
        var batchCollectionQuery =
          new XPQuery<BUR_Batch>(UoW).Where(
            o => o.DeliveryDate == null && o.CreatedDate.HasValue && o.CreatedDate.Value >= DateTime.Today.AddDays(-60))
            .AsQueryable();

        if (!ignoreLock)
          batchCollectionQuery = batchCollectionQuery.Where(b => !b.Locked);

        var batchCollection = batchCollectionQuery.OrderBy(b => b.CreatedDate).ToList();

        // All batches have been sent.
        if (batchCollection.Count == 0)
          return;

        // Loop through batches and retrieve items for each.
        foreach (var batch in batchCollection)
        {
          // Force lock the batch during processing
          // this will prevent items that do not get picked up to not be sent
          // force the items to a new batch and process later.
          batch.Locked = true;
          UoW.CommitChanges();

          // Retrieve all batch items.
          var batchItemCollection = new XPQuery<BUR_BatchItem>(UoW).Where(o => o.Batch.BatchId == batch.BatchId).ToList();

          // No batch items for this batch
          if (batchItemCollection.Count == 0)
            continue;

          // Create instance of transport data xml
          transData = new trans_data();

          this.ProcessCollection(ref transData, batchItemCollection);

          if (submissionData == null)
            submissionData = new Dictionary<BUR_BatchDTO, trans_data>();

          // Add to dictionary for later use
          submissionData.Add(AutoMapper.Mapper.Map<BUR_Batch, BUR_BatchDTO>(batch), transData);
        }
      }

      if (submissionData != null)
      {
        #region Build Submission Records

        Parallel.ForEach(submissionData, pair =>
        {
          using (var uow = new UnitOfWork())
          {
            var batchRecord = new XPQuery<BUR_BatchSubmission>(uow).FirstOrDefault(o => o.Batch.BatchId == pair.Key.BatchId);

            if (batchRecord != null)
              return;

            var batchSubmission = new BUR_BatchSubmission(uow)
            {
              ErrorMessage = string.Empty,
              Status = Risk.BatchJobStatus.Pending_Pickup,
              LastStatusUpdateDate = DateTime.Now,
              Batch = new XPQuery<BUR_Batch>(uow).Where(o => o.BatchId == pair.Key.BatchId).FirstOrDefault(),
              XML = Compression.Compress(Xml.Serialize<trans_data>(pair.Value))
            };

            uow.CommitChanges();
          }
        });

        Parallel.ForEach(submissionData, pair =>
        {
          using (var uow = new UnitOfWork())
          {
            var batchItemCollection = new XPQuery<BUR_BatchItem>(uow).Where(o => o.Batch.BatchId == pair.Key.BatchId).ToList();

            foreach (var batchPair in batchItemCollection)
            {
              var batchItemRecord = new XPQuery<BUR_BatchSubmissionItem>(uow).FirstOrDefault(o => o.BatchReferenceNo == batchPair.BatchReferenceNo
                 && o.BatchItem.BatchItemId == batchPair.BatchItemId);

              if (batchItemRecord != null)
                return;

              var batchItemSubmission = new BUR_BatchSubmissionItem(uow)
              {
                SubmissionBatch = new XPQuery<BUR_BatchSubmission>(uow).Where(o => o.Batch.BatchId == batchPair.Batch.BatchId).FirstOrDefault(),
                BatchItem = batchPair,
                BatchReferenceNo = batchPair.BatchReferenceNo,
                SubmissionXML = batchPair.XML
              };

              uow.CommitChanges();
            }
          }
        });

        #endregion

        // Prepare and package and send off batch submission
        string errorMsg = string.Empty;

        foreach (var submission in submissionData)
        {
          try
          {
            this.StandardParameters(submission.Key.Branch.BranchId, "BatchData", Risk.BatchDestination.LIVE);

            #region Core Parameters

            // Get all the properties of the current class
            var propertyInfos = this.GetProperties();

            var requestDict = this.PropertyDictionary(propertyInfos);

            requestDict.Add("p_MyOrigin", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("AtlasFinance")));

            #endregion

            // New instance of the poster class 
            var post = new Http { Url = this._url.ToStringEnum(), TimeOut = 200000 };

            if (requestDict.ContainsKey("p_Data"))
              requestDict.Remove("p_Data");

            requestDict.Add("p_Data", Xml.SerializeToZipToBase64<trans_data>(submission.Value));

            // Add the required post variables into the poster
            requestDict.ToList().ForEach(i => post.PostItems.Add(i.Key.ToString(), i.Value.ToString()));

            // We know we are posting so set num
            post.Type = Http.PostTypeEnum.Post;

            // Do the actual post against the url
            string postResult = string.Empty;

            try
            {
              postResult = post.Post();
            }
            catch (Exception ex)
            {
              using (var UoW = new UnitOfWork())
              {
                var batch = new XPQuery<BUR_BatchSubmission>(UoW).FirstOrDefault(o => o.Batch.BatchId == submission.Key.BatchId);

                if (batch != null)
                {
                  batch.ErrorMessage = ex.Message;
                  batch.Invalid = true;
                }

                UoW.CommitChanges();
              }
            }
            try
            {
              if (_errorDict.ContainsKey(int.Parse(postResult)))
                errorMsg = _errorDict[int.Parse(postResult)];
            }
            catch (Exception exception)
            {
              _log.Error(string.Format("Error occured in BatchServletImpl.DeliverBatch with submission data of batchId, {0}, error parsing postresult {1} :{2} - {3}", submission.Key.BatchId, postResult, exception.Message, exception.StackTrace));
              throw;
            }

            this.UpdateBatchSubmission(submission.Key.BatchId, errorMsg != string.Empty ? (long?)null : long.Parse(postResult),
                                         Risk.BatchJobStatus.Pending_Pickup, DateTime.Now, DateTime.Now, null, errorMsg);
          }
          catch (Exception exception)
          {
            _log.Error(string.Format("Error occured in BatchServletImpl.DeliverBatch with submission data of batchId, {0} :{1} - {2}", submission.Key.BatchId, exception.Message, exception.StackTrace));
          }
        }
      #endregion
      }
    }


    public void Dispose()
    {

    }
  }
    #endregion

}
