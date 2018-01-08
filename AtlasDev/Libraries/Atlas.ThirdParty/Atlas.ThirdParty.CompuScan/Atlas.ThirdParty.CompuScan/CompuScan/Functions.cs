using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;

using Serilog;
using DevExpress.Xpo;
using Ninject;

using Atlas.Common.Extensions;
using Atlas.Common.Utils;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.LoanEngine.Account;
using Atlas.RabbitMQ.Messages.Credit;
using Atlas.ThirdParty.CompuScan.Batch;
using Atlas.ThirdParty.CompuScan.Batch.XML;
using Atlas.ThirdParty.CompuScan.Enquiry;
using EasyNetQ;


namespace Atlas.ThirdParty.CompuScan
{
  public sealed class Functions : IDisposable
  {
    #region Static Members

    private static readonly ILogger _log = Serilog.Log.Logger.ForContext<Functions>();

    #endregion


    #region Private Members

    private IKernel _kernel = null;
	  private readonly IBus _bus;

	  private int _objectVersion = 2;

    #endregion


    #region Constructors

    public Functions(IKernel kernel, IBus bus)
    {
	    _kernel = kernel;
	    _bus = bus;
    }

	  #endregion


    #region Public

    public void Perform(CreditRequest req)
    {
      string userName = string.Empty;
      string password = string.Empty;
      int threshold;
      Risk.CreditCheckDestination creditCheckDestination;

      long? serviceId = null;

      using (var uow = new UnitOfWork())
      {
        var creditService = new XPQuery<BUR_Service>(uow).FirstOrDefault(s => s.Enabled && s.ServiceType == Risk.ServiceType.Atlas_Online_Credit);

        if (creditService == null)
          throw new Exception("No service profile found for credit");

        userName = creditService.Username;
        password = creditService.Password;
        serviceId = creditService.ServiceId;
        threshold = creditService.Days;

        creditCheckDestination = GetCreditDestination(creditService.Destination);
      }

      // Insert risk enquiry tracking record.
      Int64? transNo = null;
      try
      {
        List<string> errorArr = new List<string>();
        decimal totalCPAAccount = 0.0M;
        decimal totalNLRAccount = 0.0M;

        string Run_CompuScore = string.Empty;

        Run_CompuScore = "Y";

        ACC_AccountDTO account = null;
        // Get the account/
        using (var uow = new UnitOfWork())
        {
          var accountRec = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == req.AccountId);

          if (accountRec == null)
          {
            _log.Fatal(":: Account {AccountId} was not found in the database", req.AccountId);
            return;
          }

          account = AutoMapper.Mapper.Map<ACC_Account, ACC_AccountDTO>(accountRec);
        }

        #region Create Request Object

        // Create Request object
        var doRequest = CreateRequest(firstName: req.Firstname, surname: req.Surname, IdNumber: req.IDNumber,
          gender: req.Gender, dateOfBirth: req.DateOfBirth, addressLine1: req.AddressLine1, addressLine2: req.AddressLine2,
          suburb: req.Suburb, city: req.City, postalCode: req.PostalCode, homeTelCode: req.HomeTelCode,
          homeTelNo: req.HomeTelNo, workTelCode: req.WorkTelCode, workTelNo: req.WorkTelNo, cellNo: req.CellNo,
          isIDPassportNo: req.IsIDPassportNo, isExistingClient: req.IsExistingClient, requestUser: req.RequestUser,
          CC_enquiry: "Y", CCPlusCPA_enquiry: "Y", NLR_enquiry: "Y", RunCodix: "Y", Run_CompuScore: Run_CompuScore);


        #endregion

        #region Check to see if there was a recent enquiry

        bool _reEnquire = true;

        // Check to see if there is a recent inquiry
        var riskRec = GetRiskRecord(req.IDNumber, Risk.RiskTransactionType.Score, threshold);

        if (riskRec == null)
          _reEnquire = true;
        else
          _reEnquire = false;

        #endregion

        #region Create Risk Record

        if (riskRec == null)
          CreateRiskRecord(req.AccountId, req.CorrelationId, null, (long)serviceId, req.Firstname, req.Surname, req.IDNumber, doRequest, req.RequestUser, Risk.RiskTransactionType.Score, 1, out transNo);
        else
          CreateRiskRecord(req.AccountId, req.CorrelationId, riskRec, (long)serviceId, req.Firstname, req.Surname, req.IDNumber, doRequest, req.RequestUser, Risk.RiskTransactionType.Score, 1, out transNo);

        #endregion

        #region Do Compuscan Call

        // Make call to compuscan.
        IResponseResult result = Retry(() =>
          DoProcessOrDecode(_reEnquire, (long)transNo, riskRec, doRequest, userName, password, creditCheckDestination), 3, 3000, req.CorrelationId, (long)req.AccountId);

        if (!string.IsNullOrEmpty(result.Error))
          throw new Exception(string.Format(":: DoProcessOrDecodeException for account {0} and EnquiryId {1} - Error {2} Description {3}", req.AccountId, transNo, result.Error, result.ErrorDescription));

        #endregion

        #region Save Data

        // Save the Account data
        SaveAccountData(result, account, (long)transNo, req.RequestUser, ref totalCPAAccount, ref totalNLRAccount, ref errorArr);

        SaveColour((long)transNo, result);

        #endregion

        UpdateRiskRecord(result, (long)transNo, ref errorArr);

        // Save safps data from report
        SaveSAFPS((long)transNo, result);

        List<Risk.Policy> policyCollection;

        ProcessSAFPS((long)transNo, req, out policyCollection);

        #region Process Policies

        var policies = ProcessPolicies(account, policyCollection, result);

        var resp = new CreditResponse(req.CorrelationId) { Score = result.Score };

        #endregion

        #region Process Band

        var band = ProcessBands(Convert.ToInt32(resp.Score));

        //// !!!!!!! DEV REV REMOVE DEV DEV DEV DEV !!!!!!!
        //// !!!!!!! DEV REV REMOVE DEV DEV DEV DEV !!!!!!!
        //// !!!!!!! DEV REV REMOVE DEV DEV DEV DEV !!!!!!!
        //// !!!!!!! DEV REV REMOVE DEV DEV DEV DEV !!!!!!!
	      if (ConfigurationManager.AppSettings["pass_score"] == "true")
	      {
					_log.Warning($"--- OVERRIDE CREDIT BAND --- pass_score is set to true in config");
		      policyCollection = new List<Risk.Policy>();
		      policies = new List<Risk.Policy>();
		      band.Band = new BUR_BandDTO {Type = Risk.Band.Medium, Description = "Medium", Pass = true};
	      }
	      //// !!!!!!! DEV REV REMOVE DEV DEV DEV DEV !!!!!!!
				//// !!!!!!! DEV REV REMOVE DEV DEV DEV DEV !!!!!!!
				//// !!!!!!! DEV REV REMOVE DEV DEV DEV DEV !!!!!!!
				//// !!!!!!! DEV REV REMOVE DEV DEV DEV DEV !!!!!!!



				if (band != null)
        {
          resp.RiskType = band.Band.Description;

          if (policyCollection.Count == 0 && band.Band.Type != Risk.Band.Declined)
          {
            if (policies.Count > 0 && band.Band.Type != Risk.Band.Declined)
            {
              resp.Decision = Enumerators.Account.AccountStatus.Declined;
              UpdateAccount(account, Enumerators.Account.AccountStatus.Declined, Enumerators.Account.AccountStatusReason.CreditRisk, null);
            }
            else
            {
              resp.Decision = Enumerators.Account.AccountStatus.Approved;
            }
          }
          else if (policyCollection.Count > 0 && band.Band.Type != Risk.Band.Declined)
          {
            if (policyCollection.Contains(Risk.Policy.SAFPS_Multiple_Incident))
            {
              resp.Decision = Enumerators.Account.AccountStatus.Declined;
              UpdateAccount(account, Enumerators.Account.AccountStatus.Declined, Enumerators.Account.AccountStatusReason.Fraud, Enumerators.Account.AccountStatusSubReason.SAFPSMultipleIncidentListings);
            }
            else
            {
              resp.Decision = Enumerators.Account.AccountStatus.Declined;
              UpdateAccount(account, Enumerators.Account.AccountStatus.Declined, Enumerators.Account.AccountStatusReason.Fraud, Enumerators.Account.AccountStatusSubReason.IdentityTheftSuspect);
            }
          }
          else if (band.Band.Type == Risk.Band.Declined)
          {
            resp.Decision = Enumerators.Account.AccountStatus.Declined;
            UpdateAccount(account, Enumerators.Account.AccountStatus.Declined, Enumerators.Account.AccountStatusReason.CreditRisk, null);
          }
        }

        #endregion

        _log.Information(":: Request Ended {CorrelationId} - Score: {Score}, Decision: {Decision} - Ended @ {EndAt} for Account {AccountId}", req.CorrelationId, resp.Score, resp.Decision, DateTime.Now.ToLongTimeString(), req.AccountId);

        Trace(errorArr, result, totalCPAAccount, totalNLRAccount, result.Reasons, (long)transNo);

        resp.NLREnquiryNo = result.NLREnquiryReferenceNo;
        resp.TotalCPAAccount = totalCPAAccount;
        resp.TotalNLRAccount = totalNLRAccount;

        UpdateNLREnquiryNo(account, resp.NLREnquiryNo);

        resp.Reasons = new List<string>();
        if (result.Reasons != null)
        {
          resp.Reasons.AddRange(result.Reasons.Where(t => !string.IsNullOrEmpty(t)));
        }

        _bus.Publish<CreditResponse>(resp);

        req = null;
      }
      catch (Exception ex)
      {
        _log.Fatal(ex.StackTrace);
        using (var uow = new UnitOfWork())
        {
          var riskRec = new XPQuery<BUR_Enquiry>(uow).FirstOrDefault(p => p.EnquiryId == (long)transNo);
          if (riskRec != null)
            riskRec.IsSucess = false;

          uow.CommitChanges();

					// Reply with technical error.
					_bus.Publish<CreditResponse>(new CreditResponse(req.CorrelationId) { Decision = Enumerators.Account.AccountStatus.Technical_Error });
        }
      }
    }


    public void Perform(CreditRequestLegacy req)
    {
      _log.Information(string.Format("Perform - CreditRequestLegacy entry point with message: {0}", req.ToJSON()));

      // Insert risk enquiry tracking record.
      Int64? transNo = null;
      try
      {
        if (string.IsNullOrEmpty(req.LegacyBranchNo))
          throw new NullReferenceException("LegacyBranchNo may not be null!");

        string userName = string.Empty;
        string password = string.Empty;
        long? serviceId = null;
        int threshold;
        long branchId;
        Risk.CreditCheckDestination creditCheckDestination;

        using (var uow = new UnitOfWork())
        {
          var legacyBranch = new XPQuery<BRN_Branch>(uow).FirstOrDefault(p => p.LegacyBranchNum.Trim().PadLeft(3, '0') == req.LegacyBranchNo.PadLeft(3, '0'));

          if (legacyBranch == null)
            throw new Exception(string.Format("Branch {0} does not exist in the databse!", req.LegacyBranchNo));


          var bureauServiceCredentials = new XPQuery<BUR_Service>(uow).FirstOrDefault(
           p => p.Branch.BranchId == legacyBranch.BranchId && p.Enabled);

          if (bureauServiceCredentials == null)
            throw new Exception(string.Format("No login credentials found for branch {0}", req.LegacyBranchNo.PadLeft(3, '0')));

          _log.Information("Found Service {Name} for Branch No: {BranchNo}", bureauServiceCredentials.Name, req.LegacyBranchNo.PadLeft(3, '0'));

          userName = bureauServiceCredentials.Username;
          password = bureauServiceCredentials.Password;
          serviceId = bureauServiceCredentials.ServiceId;
          threshold = bureauServiceCredentials.Days;
          branchId = legacyBranch.BranchId;

          creditCheckDestination = GetCreditDestination(bureauServiceCredentials.Destination);

        }

        List<string> errorArr = new List<string>();
        decimal totalCPAAccount = 0.0M;
        decimal totalNLRAccount = 0.0M;

        string Run_CompuScore = string.Empty;

        var risktype = Risk.RiskTransactionType.Score;

        // Removed as per Tim's request to always perform a score.
        //if (!req.IsExistingClient)
        //{
        Run_CompuScore = "Y";
        //}
        //else
        //{
        //  CC_enquiry = "Y";
        //  NLR_enquiry = "Y";
        //  CCPlusCPA_enquiry = "Y";
        //  Run_CompuScore = "N";
        //  RunCodix = "N";
        //  risktype = Risk.RiskTransactionType.NLR;
        //}

        #region Create Request Object

        // Create Request object
        var doRequest = CreateRequest(firstName: req.Firstname, surname: req.Surname, IdNumber: req.IDNumber,
          gender: req.Gender, dateOfBirth: req.DateOfBirth, addressLine1: req.AddressLine1, addressLine2: req.AddressLine2,
          suburb: req.Suburb, city: req.City, postalCode: req.PostalCode, homeTelCode: req.HomeTelCode, homeTelNo: req.HomeTelNo,
          workTelCode: req.WorkTelCode, workTelNo: req.WorkTelNo, cellNo: req.CellNo, isIDPassportNo: req.IsIDPassportNo,
          isExistingClient: req.IsExistingClient, requestUser: req.RequestUser, CC_enquiry: "Y", CCPlusCPA_enquiry: "Y",
          NLR_enquiry: "Y", RunCodix: "Y", Run_CompuScore: Run_CompuScore);

        #endregion

        #region Check to see if there was a recent enquiry

        bool _reEnquire = true;

        // Check to see if there is a recent inquiry
        var riskRec = GetRiskRecord(req.IDNumber, risktype, threshold);

        if (riskRec == null)
        {
          if (req.IsQueryOnly.HasValue && req.IsQueryOnly.Value)
          {
            // get latest enquiry irrespective of age
            riskRec = GetRiskRecord(req.IDNumber, risktype, 180); // we only keep 6 months back....

            if (riskRec == null || GetStorageCount(riskRec) == 0)
            {
              // respond with null
              _log.Information(string.Format("Perform - CreditRequestLegacy - RiskRecord is NULL AND IsQueryOnly == true: {0}", req.ToJSON()));
              CreditStreamResponse streamResponse = new CreditStreamResponse(req.CorrelationId) { Error = "RiskRecord is NULL AND IsQueryOnly == true" };
							_bus.Publish(streamResponse);
              return;
            }
            else
            {
              _reEnquire = false;
            }
          }
        }
        else
        {
          _reEnquire = false;
        }

        #endregion

        #region Create Risk Record

        if (riskRec == null)
          CreateRiskRecord(null, req.CorrelationId, riskRec, serviceId, req.Firstname, req.Surname, req.IDNumber, doRequest, req.RequestUser, risktype, branchId, out transNo);
        else
          transNo = riskRec.EnquiryId;

        #endregion

        #region Do Compuscan Call

        // Make call to compuscan.
        dynamic result = Retry(() => DoProcessOrDecode(_reEnquire, (long)transNo, riskRec, doRequest, userName, password, creditCheckDestination), 3, 3000, req.CorrelationId, null);

        if (result == null)
          throw new Exception(string.Format("DoProcessOrDecodeException for EnquiryId {0}", transNo));

        if (!string.IsNullOrEmpty(result.Error))
        {
          _log.Fatal("DoProcessOrDecodeException - Exception Error Code {0} {1}", result.Error, result.ErrorDescription);
          throw new Exception(string.Format("DoProcessOrDecodeException - Exception Error Code {0} {1}", result.Error, result.ErrorDescription));
        }

        #endregion

        #region Save Data

        // Save the Account data
        SaveAccountData(result, null, (long)transNo, req.RequestUser, ref totalCPAAccount, ref totalNLRAccount, ref errorArr);

        SaveColour((long)transNo, result);

        #endregion

        UpdateRiskRecord(result, (long)transNo, ref errorArr);

        // Save safps data from report
        SaveSAFPS((long)transNo, result);

        #region Process Policies


        if (riskRec != null)
          _objectVersion = riskRec.ObjectVersion;


        var resp = new CreditResponse(req.CorrelationId) { Score = result.Score, RiskType = result.RiskType, File = Convert.ToBase64String(result.SummaryFile) };

        if (_objectVersion > 1)
          resp.Products = AutoMapper.Mapper.Map<List<Atlas.ThirdParty.CompuScan.Enquiry.Product>, List<Atlas.RabbitMQ.Messages.Credit.Product>>(result.Products);

        #endregion

        _log.Information("Request Ended {CorrelationId} - Score: {Score}, Decision: {Decision} - Ended @ {EndedAt} for Account {AccountId}", req.CorrelationId, resp.Score, resp.Decision, DateTime.Now.ToLongTimeString(), req.AccountId);

        if (!req.IsExistingClient)
        {
          Trace(errorArr, result, totalCPAAccount, totalNLRAccount, result.Reasons, (long)transNo);
        }

        resp.NLREnquiryNo = result.NLREnquiryReferenceNo;
        resp.TotalCPAAccount = totalCPAAccount;
        resp.TotalNLRAccount = totalNLRAccount;
        resp.ScoreDate = riskRec == null ? DateTime.Now : riskRec.CreateDate;
        resp.Age = riskRec == null ? 0 : Convert.ToInt32((DateTime.Now - (DateTime)riskRec.CreateDate).TotalDays);

        resp.Reasons = new List<string>();
        var reasons = result.Reasons != null ? result.Reasons as List<string> : null;
        if (reasons != null && reasons.Any())
        {
          resp.Reasons.AddRange(reasons.Where(s => string.IsNullOrEmpty(s)));
        }

        if (result.Accounts != null)
        {
          resp.NLRCPAAccounts = AutoMapper.Mapper.Map<List<Atlas.ThirdParty.CompuScan.Enquiry.Account>, List<NLRCPAAccount>>(result.Accounts);
        }

        if (req.IsQueryOnly == null)
        {
					_bus.Publish<CreditResponse>(resp);
        }
        else
        {
          var streamResponse = AutoMapper.Mapper.Map<CreditResponse, CreditStreamResponse>(resp);
					_bus.Publish(streamResponse);
        }

        req = null;
      }
      catch (Exception ex)
      {
        _log.Fatal(ex.StackTrace);
        if (transNo != null)
        {
          using (var uow = new UnitOfWork())
          {
            var riskRec = new XPQuery<BUR_Enquiry>(uow).FirstOrDefault(p => p.EnquiryId == (long)transNo);
            if (riskRec != null)
              riskRec.IsSucess = false;

            uow.CommitChanges();

						// Reply with technical error.
						_bus.Publish<CreditResponse>(new CreditResponse(req.CorrelationId) { Decision = Enumerators.Account.AccountStatus.Technical_Error });
          }
        }
      }
    }


    public void ProcessSAFPS(long enquiryId, CreditRequest req, out List<Risk.Policy> policies)
    {
      policies = new List<Risk.Policy>();

      using (var uow = new UnitOfWork())
      {
        var safpsEnquiry = new XPQuery<FPM_SAFPS_Enquiry>(uow).FirstOrDefault(e => e.Enquiry.EnquiryId == enquiryId);

        if (safpsEnquiry == null)
          _log.Warning("Unable to locate SAFPS Enquiry for Credit Enquiry {0}", enquiryId);
        else
        {
          var account = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == req.AccountId);

          #region No Policies


          var cellPolicy = PolicyCache.GetCachItem(Risk.Policy.SAFPS_Cell_No);

          if (cellPolicy)
          {
            var cellNos = new List<string>();
            cellNos.Add(req.CellNo);

            var cellSAFPS = new XPQuery<FPM_SAFPS_TelephoneDetail>(uow).Where(t => cellNos.Contains(t.No) && t.SAFPS.SafpsId == safpsEnquiry.SafpsId).ToList();

            if (cellSAFPS.Count > 0)
            {
              policies.Add(Risk.Policy.SAFPS_Cell_No);
              new BUR_AccountPolicy(uow)
              {
                Account = account,
                Policy = new XPQuery<BUR_Policy>(uow).FirstOrDefault(p => p.Type == Risk.Policy.SAFPS_Cell_No)
              };
            }
          }

          var homNoPolicy = PolicyCache.GetCachItem(Risk.Policy.SAFPS_Home_No);

          if (homNoPolicy)
          {
            var homeNos = new List<string>();
            homeNos.Add(string.Format("({0}){1}", req.HomeTelCode, req.HomeTelNo));
            homeNos.Add(string.Format("{0}{1}", req.HomeTelCode, req.HomeTelNo));

            var homeNoSAFPS = new XPQuery<FPM_SAFPS_TelephoneDetail>(uow).Where(t => homeNos.Contains(t.No) && t.SAFPS.SafpsId == safpsEnquiry.SafpsId).ToList();

            if (homeNoSAFPS.Count > 0)
            {
              policies.Add(Risk.Policy.SAFPS_Home_No);

              new BUR_AccountPolicy(uow)
              {
                Account = account,
                Policy = new XPQuery<BUR_Policy>(uow).FirstOrDefault(p => p.Type == Risk.Policy.SAFPS_Home_No)
              };
            }
          }

          #endregion

          #region Incidents

          var incidentCollection = new XPQuery<FPM_SAFPS_IncidentDetail>(uow).Where(i => i.SAFPS.SafpsId == safpsEnquiry.SafpsId && !i.Victim).ToList();

          var singleIncident = PolicyCache.GetCachItem(Risk.Policy.SAFPS_Single_Incident);

          if (singleIncident)
          {
            if (incidentCollection.Count == 1)
            {
              policies.Add(Risk.Policy.SAFPS_Single_Incident);
              new BUR_AccountPolicy(uow)
              {
                Account = account,
                Policy = new XPQuery<BUR_Policy>(uow).FirstOrDefault(p => p.Type == Risk.Policy.SAFPS_Single_Incident)
              };
            }
          }

          var multipleIncident = PolicyCache.GetCachItem(Risk.Policy.SAFPS_Multiple_Incident);

          if (multipleIncident)
          {
            if (incidentCollection.Count >= 2)
            {
              policies.Add(Risk.Policy.SAFPS_Multiple_Incident);
              new BUR_AccountPolicy(uow)
              {
                Account = account,
                Policy = new XPQuery<BUR_Policy>(uow).FirstOrDefault(p => p.Type == Risk.Policy.SAFPS_Multiple_Incident)
              };
            }
          }

          #endregion

          #region Bank Details

          var bankDetail = PolicyCache.GetCachItem(Risk.Policy.SAFPS_BankAccount_No);

          if (bankDetail)
          {
            List<string> bankNoCollection = new List<string>();
            account.Person.GetBankDetails.Select(b => b.BankDetail)
              .ToList()
              .ForEach(o => bankNoCollection.Add(o.AccountNum));
            var bankDetailCollection = new XPQuery<FPM_SAFPS_BankDetail>(uow).Where(b => bankNoCollection.Contains(b.AccountNo) && b.SAFPS.SafpsId == safpsEnquiry.SafpsId).ToList();

            if (bankDetailCollection.Count > 0)
            {
              policies.Add(Risk.Policy.SAFPS_BankAccount_No);
              new BUR_AccountPolicy(uow)
              {
                Account = account,
                Policy = new XPQuery<BUR_Policy>(uow).FirstOrDefault(p => p.Type == Risk.Policy.SAFPS_BankAccount_No)
              };
            }
          }

          #endregion

          #region Case Detail

          var caseDetail = PolicyCache.GetCachItem(Risk.Policy.SAFPS_Case);

          if (caseDetail)
          {
            var caseDetailCollection = new XPQuery<FPM_SAFPS_CaseDetail>(uow).Where(c => c.SAFPS.SafpsId == safpsEnquiry.SafpsId).ToList();
            if (caseDetailCollection.Count > 0)
            {
              policies.Add(Risk.Policy.SAFPS_Case);
              new BUR_AccountPolicy(uow)
              {
                Account = account,
                Policy = new XPQuery<BUR_Policy>(uow).FirstOrDefault(p => p.Type == Risk.Policy.SAFPS_Case)
              };
            }
          }

          #endregion

          #region Subject

          var subject = PolicyCache.GetCachItem(Risk.Policy.SAFPS_Subject);

          if (subject)
          {
            var subjectCollection = new XPQuery<FPM_SAFPS_Subject>(uow).Where(s => s.SAFPS.SafpsId == safpsEnquiry.SafpsId && !s.Victim).ToList();

            if (subjectCollection.Count > 0)
            {
              policies.Add(Risk.Policy.SAFPS_Subject);
              new BUR_AccountPolicy(uow)
              {
                Account = account,
                Policy = new XPQuery<BUR_Policy>(uow).FirstOrDefault(p => p.Type == Risk.Policy.SAFPS_Subject)
              };
            }
          }

          #endregion

          uow.CommitChanges();
        }
      }
    }


    public void RegisterClient(RegisterClient req)
    {
      using (var batchImpl = new BatchServletImpl())
      {
        long? batchId = null;
        batchId = batchImpl.CreateBatch(req.LegacyBranchNo);
        string uniqueRefNo = StringUtils.RandomString(10);

        Int64 id = 0;
        if (batchImpl.CreateAssociation(req.LegacyBranchNo, uniqueRefNo, req.SequenceNo))
        {
          id = batchImpl.CreateBatchItem((long)batchId, uniqueRefNo, Risk.BatchTransactionType.Registration, Risk.BatchSubTransactionType.Client,
                                          Compression.Compress(Xml.Serialize<CSREG_CLIENT>
                                          (new CSREG_CLIENT()
                                          {
                                            Client_no = req.ClientNo,
                                            Name = req.Name,
                                            Surname = req.Lastname,
                                            Comments = req.Comment,
                                            Country_of_origin = req.CountryOfOrigin,
                                            Identity_number = req.IDNo,
                                            Reference_no = uniqueRefNo,
                                            Trans_created_date = req.TransactionCreateDate,//DateTime.Now.ToString("yyyymmdd"),
                                            Trans_created_time = req.TransactionCreateTime,//DateTime.Now.ToString("HHmmss")
                                          })));
        }
				_bus.Publish(new ResponseBatchX(req.CorrelationId)
        {
          CreatedAt = DateTime.Now,
          Result = id == 0 ? -1 : 1
        });
      }
    }


    public void RegisterLoan(RegisterLoan req)
    {
      long? batchId = null;
      using (var batchImpl = new BatchServletImpl())
      {
        batchId = batchImpl.CreateBatch(req.LegacyBranchNo);

        Int64 id = 0;
        string uniqueRefNo = StringUtils.RandomString(10);
        if (batchImpl.CreateAssociation(req.LegacyBranchNo, uniqueRefNo, req.SequenceNo))
        {

          id = batchImpl.CreateBatchItem((long)batchId, uniqueRefNo, Risk.BatchTransactionType.Registration, Risk.BatchSubTransactionType.Loan,
           Compression.Compress(Xml.Serialize<CSREG_LOAN>(new CSREG_LOAN()
           {
             Identity_number = req.IDNo,
             Country_of_origin = req.CountryOfOrigin,
             Loan_due_date = req.DueDate,
             Loan_install_amt = req.InstallmentAmount.ToString(),
             Loan_issued_date = req.IssueDate,
             Loan_ref_no = req.LoanReferenceNo,
             Loan_tot_amt_repayable = req.TotalAmountRepayable.ToString(),
             Reference_no = uniqueRefNo,
             Trans_created_date = req.TransactionCreateDate,//DateTime.Now.ToString("yyyymmdd"),
             Trans_created_time = req.TransactionCreateTime//DateTime.Now.ToString("HHmmss")
           })));
        }

				_bus.Publish<ResponseBatchX>(new ResponseBatchX(req.CorrelationId)
        {
          CreatedAt = DateTime.Now,
          Result = id == 0 ? -1 : 1
        });
      }
    }


    public void EnqGlobal(ENQGlobal req)
    {
      long? batchId = null;
      using (var batchImpl = new BatchServletImpl())
      {
        batchId = batchImpl.CreateBatch(req.LegacyBranchNo);

        Int64 id = 0;
        string uniqueRefNo = StringUtils.RandomString(10);
        if (batchImpl.CreateAssociation(req.LegacyBranchNo, uniqueRefNo, req.SequenceNo))
        {

          id = batchImpl.CreateBatchItem((long)batchId, uniqueRefNo, Risk.BatchTransactionType.Enquiry, Risk.BatchSubTransactionType.Global,
           Compression.Compress(Xml.Serialize<CSENQ_GLOBAL2>(new CSENQ_GLOBAL2()
           {
             Identity_number = req.IdentityNo,
             Country_of_origin = req.CountryOfOrigin,
             Cc_enquiry = req.CcEnquiry,
             Con_Cell_tel_no = req.ConCellTelNo,
             Con_Curr_add_1 = req.ConCurrAdd1,
             Con_Curr_add_2 = req.ConCurrAdd2,
             Con_Curr_add_3 = req.ConCurrAdd3,
             Con_Curr_add_4 = req.ConCurrAdd4,
             Con_Curr_add_post_code = req.ConCurrAddPostCode,
             Con_Date_of_birth = req.ConDateOfBirth,
             Con_enquiry = req.ConEnquiry,
             Con_Home_tel_code = req.ConHomeTelCode,
             Con_Home_tel_no = req.ConHomeTelNo,
             Con_Work_tel_code = req.ConWorkTelCode,
             Con_Work_tel_no = req.ConWorkTelNo,
             Forename1 = req.Forename1,
             Forename2 = req.Forename2,
             Forename3 = req.Forename3,
             Gender = req.Gender,
             Nlr_enquiry = req.NlrEnquiry,
             Nlr_Loan_amt = req.NlrLoanAmount,
             Passport_flag = req.PassportFlag,
             Surname = req.Surname,
             Reference_no = uniqueRefNo,
             Trans_created_date = req.TransactionCreateDate,//DateTime.Now.ToString("yyyymmdd"),
             Trans_created_time = req.TransactionCreateTime//DateTime.Now.ToString("HHmmss")
           })));
        }

				_bus.Publish<ResponseBatchX>(new ResponseBatchX(req.CorrelationId)
        {
          CreatedAt = DateTime.Now,
          Result = id == 0 ? -1 : 1
        });
      }
    }


    public void RegisterPayment(RegisterPayment req)
    {
      using (var batchImpl = new BatchServletImpl())
      {
        long? batchId = null;
        batchId = batchImpl.CreateBatch(req.LegacyBranchNo);
        Int64 id = 0;
        string uniqueRefNo = StringUtils.RandomString(10);
        if (batchImpl.CreateAssociation(req.LegacyBranchNo, uniqueRefNo, req.SequenceNo))
        {

          id = batchImpl.CreateBatchItem((long)batchId, uniqueRefNo, Risk.BatchTransactionType.Registration, Risk.BatchSubTransactionType.Payment,
            Compression.Compress(Xml.Serialize<CSREG_PAYMENT>(new CSREG_PAYMENT()
            {
              Identity_number = req.IDNo,
              Country_of_origin = req.CountryOfOrigin,
              Loan_ref_no = req.LoanReferenceNo,
              Payment_amount = req.PaymentAmount.ToString(),
              Payment_date = req.PaymentDate,
              Reference_no = uniqueRefNo,
              Payment_ref_no = req.PaymentReferenceNo,
              Payment_type = req.PaymentType,
              Trans_created_date = req.TransactionCreateDate,//DateTime.Now.ToString("yyyymmdd"),
              Trans_created_time = req.TransactionCreateTime //DateTime.Now.ToString("HHmmss")
            })));
        }

				_bus.Publish<ResponseBatchX>(new ResponseBatchX(req.CorrelationId)
        {
          CreatedAt = DateTime.Now,
          Result = id == 0 ? -1 : 1
        });
      }
    }


    public void RegisterAddress(RegisterAddress req)
    {
      using (var batchImpl = new BatchServletImpl())
      {
        long? batchId = null;
        batchId = batchImpl.CreateBatch(req.LegacyBranchNo);

        Int64 id = 0;
        string uniqueRefNo = StringUtils.RandomString(10);
        if (batchImpl.CreateAssociation(req.LegacyBranchNo, uniqueRefNo, req.SequenceNo))
        {
          id = batchImpl.CreateBatchItem((long)batchId, uniqueRefNo, Risk.BatchTransactionType.Registration, Risk.BatchSubTransactionType.Address,
          Compression.Compress(Xml.Serialize<CSREG_ADDRESS>(new CSREG_ADDRESS()
          {
            Address_line_1 = req.AddressLine1,
            Address_line_2 = req.AddressLine2,
            Address_line_3 = req.AddressLine3,
            Address_line_4 = req.AddressLine4,
            Address_postal_code = req.AddressPostalCode,
            Address_type = req.AddressType,
            Country_of_origin = req.CountryOfOrigin,
            Identity_number = req.IDNo,
            Reference_no = uniqueRefNo,
            Trans_created_date = req.TransactionCreateDate,//DateTime.Now.ToString("yyyymmdd"),
            Trans_created_time = req.TransactionCreateTime//DateTime.Now.ToString("HHmmss")
          })));
        }

				_bus.Publish<ResponseBatchX>(new ResponseBatchX(req.CorrelationId)
        {
          CreatedAt = DateTime.Now,
          Result = id == 0 ? -1 : 1
        });
      }
    }


    public void RegisterTelephone(RegisterTelephone req)
    {
      using (var batchImpl = new BatchServletImpl())
      {
        long? batchId = null;
        batchId = batchImpl.CreateBatch(req.LegacyBranchNo);

        Int64 id = 0;
        string uniqueRefNo = StringUtils.RandomString(10);
        if (batchImpl.CreateAssociation(req.LegacyBranchNo, uniqueRefNo, req.SequenceNo))
        {

          id = batchImpl.CreateBatchItem((long)batchId, uniqueRefNo, Risk.BatchTransactionType.Registration, Risk.BatchSubTransactionType.Telephone,
          Compression.Compress(Xml.Serialize<CSREG_TELEPHONE>(new CSREG_TELEPHONE()
          {
            Telephone_no = req.TelephoneNo,
            Telephone_type = req.TelephoneType,
            Country_of_origin = req.CountryOfOrigin,
            Identity_number = req.IDNo,
            Reference_no = uniqueRefNo,
            Trans_created_date = req.TransactionCreateDate,//DateTime.Now.ToString("yyyymmdd"),
            Trans_created_time = req.TransactionCreateTime//DateTime.Now.ToString("HHmmss")
          })));
        }

				_bus.Publish<ResponseBatchX>(new ResponseBatchX(req.CorrelationId)
        {
          CreatedAt = DateTime.Now,
          Result = id == 0 ? -1 : 1
        });
      }
    }


    public void RegisterEmployer(RegisterEmployer req)
    {
      using (var batchImpl = new BatchServletImpl())
      {
        long? batchId = null;
        batchId = batchImpl.CreateBatch(req.LegacyBranchNo);

        Int64 id = 0;
        string uniqueRefNo = StringUtils.RandomString(10);
        if (batchImpl.CreateAssociation(req.LegacyBranchNo, uniqueRefNo, req.SequenceNo))
        {
          id = batchImpl.CreateBatchItem((long)batchId, uniqueRefNo, Risk.BatchTransactionType.Registration, Risk.BatchSubTransactionType.Employer,
          Compression.Compress(Xml.Serialize<CSREG_EMPLOYER>(new CSREG_EMPLOYER()
          {
            Employee_number = req.EmployeeNo,
            Employee_occupation = req.EmployeeOccupation,
            Employee_payslip_reference = req.EmployeePayslipReference,
            Employee_salary_frequency = req.EmployeeSalaryFrequency,
            Employer_name = req.EmployerName,
            Employment_type = req.EmploymentType,
            Country_of_origin = req.CountryOfOrigin,
            Identity_number = req.IDNo,
            Reference_no = uniqueRefNo,
            Trans_created_date = req.TransactionCreateDate,//DateTime.Now.ToString("yyyymmdd"),
            Trans_created_time = req.TransactionCreateTime//DateTime.Now.ToString("HHmmss")
          })));
        }

				_bus.Publish<ResponseBatchX>(new ResponseBatchX(req.CorrelationId)
        {
          CreatedAt = DateTime.Now,
          Result = id == 0 ? -1 : 1
        });
      }
    }


    public void UpdateClient(UpdateClient req)
    {
      using (var batchImpl = new BatchServletImpl())
      {
        long? batchId = null;
        Int64 id = 0;
        batchId = batchImpl.CreateBatch(req.LegacyBranchNo);

        string uniqueRefNo = StringUtils.RandomString(10);
        if (batchImpl.CreateAssociation(req.LegacyBranchNo, uniqueRefNo, req.SequenceNo))
        {
          id = batchImpl.CreateBatchItem((long)batchId, uniqueRefNo, Risk.BatchTransactionType.Update, Risk.BatchSubTransactionType.Client,
           Compression.Compress(Xml.Serialize<CSUPD_CLIENT>(new CSUPD_CLIENT()
           {
             Client_no = req.ClientNo,
             Name = req.Name,
             Surname = req.Lastname,
             Comments = req.Comment,
             Country_of_origin = req.CountryOfOrigin,
             Identity_number = req.IDNo,
             Client_status = req.ClientStatus,
             Reference_no = uniqueRefNo,
             Trans_created_date = req.TransactionCreateDate,//DateTime.Now.ToString("yyyymmdd"),
             Trans_created_time = req.TransactionCreateTime//DateTime.Now.ToString("HHmmss")
           })));
        }

				_bus.Publish<ResponseBatchX>(new ResponseBatchX(req.CorrelationId)
        {
          CreatedAt = DateTime.Now,
          Result = id == 0 ? -1 : 1
        });
      }
    }


    public void UpdateLoan(UpdateLoan req)
    {
      using (var batchImpl = new BatchServletImpl())
      {
        long? batchId = null;
        Int64 id = 0;
        batchId = batchImpl.CreateBatch(req.LegacyBranchNo);

        string uniqueRefNo = StringUtils.RandomString(10);
        if (batchImpl.CreateAssociation(req.LegacyBranchNo, uniqueRefNo, req.SequenceNo))
        {
          id = batchImpl.CreateBatchItem((long)batchId, uniqueRefNo, Risk.BatchTransactionType.Update, Risk.BatchSubTransactionType.Loan,
           Compression.Compress(Xml.Serialize<CSUPD_LOAN>(new CSUPD_LOAN()
           {
             Loan_ref_no = req.LoanRefNo,
             Loan_status = req.LoanStatus,
             Country_of_origin = req.CountryOfOrigin,
             Identity_number = req.IDNo,
             Reference_no = uniqueRefNo,
             Trans_created_date = req.TransactionCreateDate,//DateTime.Now.ToString("yyyymmdd"),
             Trans_created_time = req.TransactionCreateTime//DateTime.Now.ToString("HHmmss")
           })));
        }
				_bus.Publish<ResponseBatchX>(new ResponseBatchX(req.CorrelationId)
        {
          CreatedAt = DateTime.Now,
          Result = id == 0 ? -1 : 1
        });
      }
    }


    public void NLRRegisterLoan(RegisterNLRLoan req)
    {
      BatchServletImpl batchImpl = new BatchServletImpl();
      long? batchId = null;
      batchId = batchImpl.CreateBatch(req.LegacyBranchNo);

      Int64 id = 0;
      string uniqueRefNo = StringUtils.RandomString(10);
      if (batchImpl.CreateAssociation(req.LegacyBranchNo, uniqueRefNo, req.SequenceNo))
      {
        id = batchImpl.CreateBatchItem((long)batchId, uniqueRefNo, Risk.BatchTransactionType.NLR, Risk.BatchSubTransactionType.Loan,
         Compression.Compress(Xml.Serialize<NLR_LOANREG>(new NLR_LOANREG()
         {
           Account_no = req.AccountNo,
           Annual_rate_for_tot_charge_of_credit = req.AnnaulRateForTotalChargeOfCredit.ToString(),
           Country_of_origin = req.CountryOfOrigin,
           Current_balance = req.CurrentBalance.ToString(),
           Current_balance_indicator = req.CurrentBalanceIndicator,
           Date_loan_disbursed = req.LoanDisbursed,
           Interest_rate_type = req.InterestRateType,
           Loan_amount = req.LoanAmount.ToString(),
           Loan_amount_indicator = req.LoanAmountIndicator,
           Loan_purpose = req.LoanPurpose,
           Loan_type = req.LoanType,
           Monthly_instalment = req.MonthlyInstalment.ToString(),
           Nlr_enq_ref_no = req.NLREnquiryReferenceNo,
           Nlr_loan_reg_no = req.NLRLoanRegistrationNo,
           Rand_value_interest_charges = req.InterestCharges.ToString(),
           Rand_value_tot_charge_of_credit = req.TotalChargeOfCredit.ToString(),
           Repayment_period = req.RepaymentPeriod,
           Settlement_amount = req.SettlementAmount.ToString(),
           Sub_account_no = req.SubAccountNo,
           Total_amount_repayable = req.TotalAmountRepayable.ToString(),
           Reference_no = uniqueRefNo,
           Trans_created_date = req.TransactionCreateDate,//DateTime.Now.ToString("yyyymmdd"),
           Trans_created_time = req.TransactionCreateTime//DateTime.Now.ToString("HHmmss")
         })));
      }


			_bus.Publish<ResponseBatchX>(new ResponseBatchX(req.CorrelationId)
      {
        CreatedAt = DateTime.Now,
        Result = id == 0 ? -1 : 1
      });
    }


    public void NLRRegisterLoan2(RegisterNLRLoan2 req)
    {
      BatchServletImpl batchImpl = new BatchServletImpl();
      long? batchId = null;
      batchId = batchImpl.CreateBatch(req.LegacyBranchNo);

      Int64 id = 0;
      string uniqueRefNo = StringUtils.RandomString(10);
      if (batchImpl.CreateAssociation(req.LegacyBranchNo, uniqueRefNo, req.SequenceNo))
      {
        id = batchImpl.CreateBatchItem((long)batchId, uniqueRefNo, Risk.BatchTransactionType.NLR, Risk.BatchSubTransactionType.Loan2,
         Compression.Compress(Xml.Serialize<NLR_LOANREG2>(new NLR_LOANREG2()
         {
           Account_no = req.AccountNo,
           Annual_rate_for_tot_charge_of_credit = req.AnnaulRateForTotalChargeOfCredit.ToString(),
           Country_of_origin = req.CountryOfOrigin,
           Current_balance = req.CurrentBalance.ToString(),
           Current_balance_indicator = req.CurrentBalanceIndicator,
           Date_loan_disbursed = req.LoanDisbursed,
           Interest_rate_type = req.InterestRateType,
           Loan_amount = req.LoanAmount.ToString(),
           Loan_amount_indicator = req.LoanAmountIndicator,
           Loan_purpose = req.LoanPurpose,
           Loan_type = req.LoanType,
           Monthly_instalment = req.MonthlyInstalment.ToString(),
           Rand_value_interest_charges = req.InterestCharges.ToString(),
           Rand_value_tot_charge_of_credit = req.TotalChargeOfCredit.ToString(),
           Repayment_period = req.RepaymentPeriod,
           Settlement_amount = req.SettlementAmount.ToString(),
           Sub_account_no = req.SubAccountNo,
           Total_amount_repayable = req.TotalAmountRepayable.ToString(),
           Reference_no = uniqueRefNo,
           Trans_created_date = req.TransactionCreateDate,//DateTime.Now.ToString("yyyymmdd"),
           Trans_created_time = req.TransactionCreateTime, //DateTime.Now.ToString("HHmmss"),
           Identity_number = req.IdentityNo
         })));
      }


			_bus.Publish<ResponseBatchX>(new ResponseBatchX(req.CorrelationId)
      {
        CreatedAt = DateTime.Now,
        Result = id == 0 ? -1 : 1
      });
    }


    public void NLRLoanClose(NLRLoanClose req)
    {
      using (var batchImpl = new BatchServletImpl())
      {
        long? batchId = null;
        {
          batchId = batchImpl.CreateBatch(req.LegacyBranchNo);

          var UnitOfWork = new UnitOfWork();

          string uniqueRefNo = StringUtils.RandomString(10);

          Int64 id = 0;

          if (batchImpl.CreateAssociation(req.LegacyBranchNo, uniqueRefNo, req.SequenceNo))
          {
            id = batchImpl.CreateBatchItem((long)batchId, uniqueRefNo, Risk.BatchTransactionType.NLR, Risk.BatchSubTransactionType.LoanClose,
            Compression.Compress(Xml.Serialize<NLR_LOANCLOSE>(new NLR_LOANCLOSE()
            {
              Nlr_loan_close_code = req.NLRLoanCloseCode,
              Nlr_loan_reg_no = req.NLRLoanRegistrationNo,
              Country_of_origin = req.CountryOfOrigin,
              Reference_no = uniqueRefNo,
              Trans_created_date = req.TransactionCreateDate,//DateTime.Now.ToString("yyyymmdd"),
              Trans_created_time = req.TransactionCreateTime//DateTime.Now.ToString("HHmmss")
            })));
          }

					_bus.Publish<ResponseBatchX>(new ResponseBatchX(req.CorrelationId)
          {
            CreatedAt = DateTime.Now,
            Result = id == 0 ? -1 : 1
          });
        }
      }
    }


    public void NLRBatb2(BATB2 req)
    {
      BatchServletImpl batchImpl = new BatchServletImpl();
      long? batchId = null;
      batchId = batchImpl.CreateBatch(req.LegacyBranchNo);

      string uniqueRefNo = StringUtils.RandomString(10);
      Int64 id = 0;

      if (batchImpl.CreateAssociation(req.LegacyBranchNo, uniqueRefNo, req.SequenceNo))
      {
        id = batchImpl.CreateBatchItem((long)batchId, uniqueRefNo, Risk.BatchTransactionType.NLR, Risk.BatchSubTransactionType.BatB2,
          Compression.Compress(Xml.Serialize<NLR_BATB2>(new NLR_BATB2()
          {
            Account_no = req.AccountNo,
            Annual_rate_for_tot_charge_of_credit = req.AnnualRateForTotalChargeOfCredit.ToString(),
            Balance_overdue = req.BalanceOverdue.ToString(),
            Balance_overdue_indicator = req.BalanceOverdueIndicator,
            Branch_code = req.BranchCode,
            Current_balance = req.CurrentBalance.ToString(),
            Current_balance_indicator = req.CurentBalanceIndicator,
            Date_account_opened = req.AccountOpened,
            Date_last_payment = req.LastPayment,
            Date_of_birth = req.DateOfBirth,
            Employee_no = req.EmployeeNo,
            Employer_name = req.EmployerName,
            Employer_payslip_ref = req.EmployerPayslipReferenceNo,
            Employment_type = req.EmploymentType,
            End_use_code = req.EndUseCode,
            Forename1 = req.Forename1,
            Forename2 = req.Forename2,
            Forename3 = req.Forename3,
            Gender = req.Gender,
            Home_tel_cell_no = req.HomeTelCellNo,
            Identity_number = req.IDNo,
            Interest_rate_type = req.InterestRateType,
            Load_indicator = req.LoanIndicator,
            Loan_amount = req.LoanAmount.ToString(),
            Loan_type = req.LoanType,
            Monthly_instalment = req.MonthlyInstalment.ToString(),
            Months_in_arrears = req.MonthsInArrears.ToString(),
            Nlr_enq_ref_no = req.NLREnquiryReferenceNo,
            Nlr_loan_reg_no = req.NLRLoanRegistrationNo,
            Non_sa_identity_no = req.NonSAIDNo,
            Old_account_no = req.OldAccountNo,
            Old_sub_account_no = req.OldSubAccountNo,
            Old_supplier_branch_code = req.OldSupplierBranchCode,
            Old_supplier_ref_no = req.OldSupplierReferenceNo,
            Opening_balance_indicator = req.OpeneingBalanceIndicator,
            Owner_tenant = req.OwnerTenant,
            Postal_address_line1 = req.PostalAddressLine1,
            Postal_address_line2 = req.PostalAddressLine2,
            Postal_address_line3 = req.PostalAddressLine3,
            Postal_address_line4 = req.PostalAddressLine4,
            Postal_address_post_code = req.PostalAddressPostCode,
            Rand_value_interest_charges = req.InterestCharges.ToString(),
            Rand_value_tot_charge_of_credit = req.TotalChargeOfCredit.ToString(),
            Repayment_period = req.RepaymentPeriod,
            Residential_address_line1 = req.ResidentialAddressLine1,
            Residential_address_line2 = req.ResidentialAddressLine2,
            Residential_address_line3 = req.ResidentialAddressLine3,
            Residential_address_line4 = req.ResidentialAddressLine4,
            Residential_address_postal_code = req.ResidentialAddressPostalCode,
            Salary_frequency = req.SalaryFrequency,
            Settlement_amount = req.SettlementAmount.ToString(),
            Status_code = req.StatusCode,
            Status_date = req.StatusDate,
            Sub_account_no = req.SubAccountNo,
            Surname = req.Lastname,
            Title = req.Title,
            Total_amount_repayable = req.TotalAmountRepayable.ToString(),
            Work_tel_cell_no = req.WorkTelCellNo,
            Country_of_origin = req.CountryOfOrigin,
            Reference_no = uniqueRefNo,
            Trans_created_date = req.TransactionCreateDate,//DateTime.Now.ToString("yyyymmdd"),
            Trans_created_time = req.TransactionCreateTime//DateTime.Now.ToString("HHmmss")
          })));
      }

			_bus.Publish<ResponseBatchX>(new ResponseBatchX(req.CorrelationId)
      {
        CreatedAt = DateTime.Now,
        Result = id == 0 ? -1 : 1
      });
    }


    public void RequestReport(ReportRequest req)
    {
      ReportResponse responseMsg = null;
      using (var uow = new UnitOfWork())
      {
        _log.Information("[RequestReport][{0}] - Retrieving credit report for enquiry {0}", req.EnquiryId);

        var enquiry = new XPQuery<BUR_Enquiry>(uow).FirstOrDefault(p => p.EnquiryId == req.EnquiryId);

        if (enquiry == null)
        {
          _log.Fatal("[RequestReport][{0}] - Unable to locate enquiry {0} in the db.", req.EnquiryId);
          return;
        }

        if (enquiry.EnquiryType == Risk.RiskEnquiryType.Fraud)
        {
          _log.Warning("[RequestReport][{0}] - Request type invalid for supplied enquiry {0}", req.EnquiryId);
          return;
        }

        if (!enquiry.IsSucess)
        {
          _log.Warning("[RequestReport][{0}] - Unable to complete request, enquiry {0} was unsuccessful.", req.EnquiryId);
          return;
        }

        var storage = new XPQuery<BUR_Storage>(uow).FirstOrDefault(s => s.Enquiry.EnquiryId == enquiry.EnquiryId);

        if (storage == null)
        {
          _log.Fatal(string.Format("[RequestReport][{0}] - The risk enquiry storage record with Id {0} appears to be missing", enquiry.EnquiryId));
          return;
        }

        IResponseResult result = null;
        if (enquiry.ObjectVersion > 1)
          result = ((ResponseResultV2)Xml.DeSerialize<ResponseResultV2>(Compression.Decompress(storage.ResponseMessage)));
        else
          result = ((ResponseResult)Xml.DeSerialize<ResponseResult>(Compression.Decompress(storage.ResponseMessage)));

        responseMsg = new ReportResponse(req.CorrelationId);
        responseMsg.EnquiryId = req.EnquiryId;
        responseMsg.Report = Base64.EncodeString(result.SummaryFile);
      }

			_bus.Publish<ReportResponse>(responseMsg);
    }

    #endregion


    #region Internal Methods

    /// <summary>
    /// Return destination environment based on environment indicator.
    /// </summary>
    internal Risk.CreditCheckDestination GetCreditDestination(char environment)
    {
      return environment == 'L' ? Risk.CreditCheckDestination.LIVE : Risk.CreditCheckDestination.TEST;
    }


    /// <summary>
    /// Retry function to retry calls when timeout occurrs.
    /// </summary>
    internal IResponseResult Retry(Func<IResponseResult> func, int retry, int timeOut, Guid correlationId, long? accountId)
    {
      if (func == null)
        throw new ArgumentNullException("func");
      do
      {
        try
        {
          if (retry == 3)
            _log.Information("First request started {0} - Started @ {1} for Account {2}", correlationId, DateTime.Now.ToLongTimeString(), accountId);
          else if (retry == 2)
            _log.Warning("Second request attempt started {0} - Started @ {1} for Account {2}", correlationId, DateTime.Now.ToLongTimeString(), accountId);
          else
            _log.Warning("Third request attempt started {0} - Started @ {1} for Account {2}", correlationId, DateTime.Now.ToLongTimeString(), accountId);

          return func();
        }
        catch
        {
          if (retry <= 0) throw;
          else Thread.Sleep(timeOut);
        }
      } while (retry-- > 0);

      return null;
    }


    internal void SaveSAFPS(long enquiryId, IResponseResult result)
    {
      using (var uow = new UnitOfWork())
      {
        var enquiry = new FPM_SAFPS_Enquiry(uow) { Enquiry = new XPQuery<BUR_Enquiry>(uow).FirstOrDefault(z => z.EnquiryId == enquiryId), CreateDate = DateTime.Now };

        uow.CommitChanges();

        if (result.FPM != null)
        {
          result.FPM.ForEach((p) =>
            {
              var safps = new FPM_SAFPS_Subject(uow)
              {

                Category = p.Category,
                CategoryNo = p.CategoryNo,
                IncidentDate = p.IncidentDate,
                Passport = p.Passport,
                SubCategory = p.SubCategory,
                Subject = p.Subject,
                SubjectNo = p.SubjectNo,
                Victim = p.Victim,
                SAFPS = enquiry
              };
              safps.Save();
            });
        }

        if (result.FPMAddressDetails != null)
        {
          result.FPMAddressDetails.ForEach((p) =>
          {

            var addressSafps = new FPM_SAFPS_AddressDetail(uow)
            {
              SAFPS = enquiry,
              Address = p.Address,
              City = p.City,
              From = p.From,
              PostalCode = p.PostalCode,
              Street = p.Street,
              To = p.To,
              Type = p.Type
            };

            addressSafps.Save();
          });
        }

        if (result.FPMAliasDetails != null)
        {
          result.FPMAliasDetails.ForEach((p) =>
          {
            var aliasDetail = new FPM_SAFPS_AliasDetail(uow)
            {
              SAFPS = enquiry,
              FirstName = p.FirstName,
              Surname = p.Surname,
            };
            aliasDetail.Save();
          });
        }

        if (result.FPMBankDetails != null)
        {
          result.FPMBankDetails.ForEach((p) =>
          {
            var bankDetail = new FPM_SAFPS_BankDetail(uow)
            {
              SAFPS = enquiry,
              AccountNo = p.AccountNo,
              AccountType = p.AccountType,
              Bank = p.Bank,
              From = p.From,
              To = p.To
            };
            bankDetail.Save();
          });
        }

        if (result.FPMCaseDetails != null)
        {
          result.FPMCaseDetails.ForEach((p) =>
          {
            var caseDetail = new FPM_SAFPS_CaseDetail(uow)
            {
              SAFPS = enquiry,
              CaseNo = p.CaseNo,
              ContactNo = p.ContactNo,
              CreatedBy = p.CreatedBy,
              Details = p.Details,
              Email = p.Email,
              Fax = p.Fax,
              Officer = p.Officer,
              Reason = p.Reason,
              ReasonExtension = p.ReasonExtension,
              ReportDate = p.ReportDate,
              Station = p.Station,
              Status = p.Status,
              Type = p.Type
            };

            caseDetail.Save();
          });
        }

        if (result.FPMEmploymentDetails != null)
        {
          result.FPMEmploymentDetails.ForEach((p) =>
          {
            var employmentDetail = new FPM_SAFPS_EmploymentDetail(uow)
            {
              SAFPS = enquiry,
              CompanyNo = p.CompanyNo,
              From = p.From,
              Name = p.Name,
              Occupation = p.Occupation,
              RegisteredName = p.RegisteredName,
              Telephone = p.Telephone,
              To = p.To
            };
          });
        }

        if (result.FPMIncidentDetails != null)
        {
          result.FPMIncidentDetails.ForEach((p) =>
          {
            var incidentDetail = new FPM_SAFPS_IncidentDetail(uow)
            {
              SAFPS = enquiry,
              Category = p.Category,
              City = p.City,
              Detail = p.Detail,
              Forensic = p.Forensic,
              IncidentDate = p.IncidentDate,
              MembersReference = p.MembersReference,
              SubCategory = p.SubCategory,
              SubRole = p.SubRole,
              Victim = p.Victim
            };

            incidentDetail.Save();
          });
        }

        if (result.FPMOtherIdDetails != null)
        {
          result.FPMOtherIdDetails.ForEach((p) =>
          {
            var otherIdDetail = new FPM_SAFPS_OtherIdDetail(uow)
            {
              SAFPS = enquiry,
              Country = p.Country,
              IDNo = p.IDNo,
              IssueDate = p.IssueDate,
              Type = p.Type
            };

            otherIdDetail.Save();
          });
        }

        if (result.FPMPersonalDetails != null)
        {
          result.FPMPersonalDetails.ForEach((p) =>
          {
            var personalDetail = new FPM_SAFPS_PersonDetail(uow)
            {
              SAFPS = enquiry,
              DateOfBirth = p.DateOfBirth,
              Email = p.Email,
              Firstname = p.Firstname,
              Gender = p.Gender,
              ID = p.ID,
              Passport = p.Passport,
              Surname = p.Surname,
              Title = p.Title
            };

            personalDetail.Save();
          });
        }

        if (result.FPMTelephoneDetails != null)
        {
          result.FPMTelephoneDetails.ForEach((p) =>
        {
          var telephoneDetail = new FPM_SAFPS_TelephoneDetail(uow)
          {
            SAFPS = enquiry,
            City = p.City,
            Country = p.Country,
            No = p.No,
            Type = p.Type
          };

          telephoneDetail.Save();
        });
        }
        uow.CommitChanges();
      }
    }


    internal void UpdateAccount(ACC_AccountDTO account, Enumerators.Account.AccountStatus status, Enumerators.Account.AccountStatusReason statusReason, Atlas.Enumerators.Account.AccountStatusSubReason? subReason)
    {
      using (var uow = new UnitOfWork())
      {
        var accountRec = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == account.AccountId);
        if (accountRec == null)
        {
          _log.Fatal("Account [{0}] was not found in the database.", account.AccountId);
          return;
        }
        accountRec.StatusReason = new XPQuery<ACC_StatusReason>(uow).FirstOrDefault(a => a.StatusReasonId == (int)statusReason);

        Default helper = new Default();
        helper.SetAccountStatus(accountRec, status, statusReason, subReason, uow);

        uow.CommitChanges();
      }
    }


    internal void UpdateNLREnquiryNo(ACC_AccountDTO account, string referenceNo)
    {
      using (var uow = new UnitOfWork())
      {
        var acc = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == account.AccountId);

        if (account == null)
        {
          _log.Fatal("UpdateNLREnquiryNo failed, account [{0}] was not found", account.AccountId);
          return;
        }
        else
        {
          acc.NLREnquiryReferenceNo = referenceNo;
          acc.Save();
        }
        uow.CommitChanges();
      }
    }


    internal BUR_BandRangeDTO ProcessBands(int score)
    {
      using (var uow = new UnitOfWork())
      {
        var band = new XPQuery<BUR_BandRange>(uow).FirstOrDefault(b => score >= b.Start && score <= b.End);
        if (band == null)
        {
          _log.Fatal("Band for [{0}] was not found.", score);
          return null;
        }
        return AutoMapper.Mapper.Map<BUR_BandRange, BUR_BandRangeDTO>(band);
      }
    }


    /// <summary>
    /// Process the company policies
    /// </summary>
    /// <param name="result"></param>
    internal List<Risk.Policy> ProcessPolicies(ACC_AccountDTO account, List<Risk.Policy> fraudPolicies, IResponseResult result)
    {
      var cachedPolicies = PolicyCache.GetCache();

      List<Risk.Policy> containedPolicies = new List<Risk.Policy>();

      cachedPolicies.ToList().ForEach((itm) =>
      {
        if (result.Policies != null)
        {
          if (result.Policies.Contains(itm))
            containedPolicies.Add(itm);
        }
        if (fraudPolicies.Count > 0)
        {
          if (fraudPolicies.Contains(itm))
            containedPolicies.Add(itm);
        }
      });

      // Save policies to DB
      using (var uow = new UnitOfWork())
      {
        var accountRec = new XPQuery<ACC_Account>(uow).FirstOrDefault(a => a.AccountId == account.AccountId);

        foreach (var itm in containedPolicies)
        {
          var policy = new BUR_AccountPolicy(uow) { Account = accountRec, Policy = new XPQuery<BUR_Policy>(uow).FirstOrDefault(p => p.Type == itm) };
          policy.Save();
        }

        uow.CommitChanges();
      }

      return containedPolicies;
    }


    /// <summary>
    /// Prints out trace information
    /// </summary>
    internal void Trace(List<string> errors, dynamic result, decimal totalCPAAccount, decimal totalNLRAccount, List<string> reasons, long transNo)
    {
      #region Tracing

      if (errors != null)
      {
        if (errors.Count > 0)
        {
          for (int i = 0; i < errors.ToArray().Length; i++)
          {
            _log.Fatal(errors.ToArray()[i]);
          }
          _log.Information("Enquiry Unsuccessful. - Ended @ {0} Enquiry {1}", DateTime.Now.ToLongTimeString(), transNo);
          return;
        }
      }

      _log.Information("Enquiry {0} Score : {1}", transNo, result.Score);
      _log.Information("Enquiry {0} Risk Type : {1}", transNo, result.RiskType);
      if (reasons != null)
      {
        if (reasons.Count > 0)
        {
          for (int i = 0; i < reasons.ToArray().Length; i++)
          {
            _log.Information("Enquiry {0} Reason [{1}]: {2}", transNo, i.ToString(), reasons.ToArray()[i]);
          }
        }
      }

      if (result.Policies != null)
      {
        foreach (var policy in result.Policies)
        {
          _log.Information("Enquiry {0} Policy : {1}", transNo, ((Risk.Policy)policy).ToStringEnum());
        }
      }

      if (_objectVersion > 1)
      {
        if (result.Products != null)
        {
          _log.Information("Products:");
          foreach (var product in result.Products)
          {
            _log.Information("Enquiry {0} Product : {1}", transNo, product.Description);
            _log.Information("Enquiry {0} Product Outcome: {1}", transNo, product.Outcome);
            if (product.Reasons != null)
            {
              foreach (var reason in product.Reasons)
              {
                _log.Information("Enquiry {0} Product Reason: {1}", transNo, reason.Description);
              }
            }
          }
        }
      }

      _log.Information("Enquiry {0} Total CPA : {1}", transNo, totalCPAAccount.ToString());
      _log.Information("Enquiry {0} Total NLR : {1}", transNo, totalNLRAccount.ToString());

      _log.Information("Enquiry {0} Enquiry Successful. - Ended @ Enquiry {1}", transNo, DateTime.Now.ToLongTimeString());

      #endregion
    }


    /// <summary>
    /// Returns a risk enquiry record for the required amount of days.
    /// </summary>
    /// <param name="idNumber">Identity number to search on.</param>
    /// <param name="days">Days elapsed since last enquiry</param>
    /// <returns>Object containing risk record, or null if none exists.</returns>
    internal BUR_EnquiryDTO GetRiskRecord(string idNumber, Risk.RiskTransactionType transactionType, int? days = null)
    {
      using (var UoW = new UnitOfWork())
      {

        var recentEnquiry = new XPQuery<BUR_Enquiry>(UoW).Where(o => o.IdentityNum == idNumber && o.IsSucess && o.TransactionType == transactionType
                                                                                        && o.PreviousEnquiry == null && o.EnquiryType == Risk.RiskEnquiryType.Credit)
                                                                                                        .OrderByDescending(o => o.EnquiryDate).FirstOrDefault();

        if (recentEnquiry == null)
          return null;
        else
        {
          if (days.HasValue && (DateTime.Now - (DateTime)recentEnquiry.CreateDate).TotalDays > days.Value)
          {
            return null;
          }
          else
          {
            return AutoMapper.Mapper.Map<BUR_Enquiry, BUR_EnquiryDTO>(recentEnquiry);
          }
        }
      }
    }

    internal BUR_StorageDTO GetFirstStorage(BUR_EnquiryDTO enquiry)
    {
      using (var uow = new UnitOfWork())
      {
        var storage = new XPQuery<BUR_Storage>(uow).FirstOrDefault(b => b.Enquiry.EnquiryId == enquiry.EnquiryId);

        return AutoMapper.Mapper.Map<BUR_Storage, BUR_StorageDTO>(storage);
      }
    }
    internal int GetStorageCount(BUR_EnquiryDTO enquiry)
    {
      using (var uow = new UnitOfWork())
      {
        var storageCount = new XPQuery<BUR_Storage>(uow).Count(b => b.Enquiry.EnquiryId == enquiry.EnquiryId);

        return storageCount;
      }
    }


    /// <summary>
    /// Saves all the required account CPA / NLR data
    /// </summary>
    internal void SaveAccountData(IResponseResult result, ACC_AccountDTO account, long? transNo, long createUser, ref Decimal totalCPAAccount, ref Decimal totalNLRAccount, ref List<string> errorArr)
    {
      #region Save NLR / CPA Account Data

      using (var UoW = new UnitOfWork())
      {
        try
        {
          if (result.Accounts != null)
          {
            // NLR / CPA Accounts
            foreach (var ac in result.Accounts)
            {
              decimal number;
              var acc = new BUR_Accounts(UoW) { Account = account == null ? null : new XPQuery<ACC_Account>(UoW).FirstOrDefault(a => a.AccountId == account.AccountId), Type = ac.AccountType, AccountNo = ac.AccountNo != string.Empty ? ac.AccountNo.Replace(" ", "") : ac.AccountNo, SubAccountNo = ac.SubAccountNo, AccountStatusCode = new XPQuery<BUR_AccountStatusCode>(UoW).FirstOrDefault(o => o.ShortCode == ac.StatusCode), AccountType = new XPQuery<BUR_AccountTypeCode>(UoW).FirstOrDefault(o => o.ShortCode == ac.AccountTypeCode) };

              if (Decimal.TryParse(ac.CurrentBalance, out number))
              {
                acc.CurrentBalance = number;
              }
              else
              {
                Console.WriteLine("Unable to parse CurrentBalance '{0}'.", ac.CurrentBalance);
              }

              acc.Enquiry = new XPQuery<BUR_Enquiry>(UoW).FirstOrDefault(o => o.EnquiryId == (Int64)transNo);
              acc.CreatedDate = DateTime.Now;

              if (Decimal.TryParse(ac.Installment, out number))
              {
                acc.Instalment = number;

                if (ac.AccountType == Risk.BureauAccountType.CPA)
                {
                  totalCPAAccount += number;
                }
                else
                {
                  if (Decimal.TryParse(ac.Installment, out number))
                  {
                    totalNLRAccount += number;
                  }
                }
              }
              else
              {
                Console.WriteLine("Unable to parse Installment '{0}'.", ac.Installment);
              }

              acc.LastPayDate = ac.LastPaymentDate;

              if (Decimal.TryParse(ac.OpenBalance, out number))
              {
                acc.OpenBalance = number;
              }
              else
              {
                Console.WriteLine("Unable to parse OpenBalance '{0}'.", ac.OpenBalance);
              }

              acc.OpenDate = ac.OpenDate;

              if (Decimal.TryParse(ac.OverdueAmount, out number))
              {
                acc.OverdueAmount = number;
              }
              else
              {
                Console.WriteLine("Unable to parse OverdueAmount '{0}'.", ac.OverdueAmount);
              }

              acc.Subscriber = ac.Subscriber;
              acc.JointParticipants = ac.JoinLoanParticpants;
              acc.Status = ac.Status;
              acc.StatusDate = ac.StatusDate;
              acc.PeriodType = acc.PeriodType;
              acc.Period = ac.RepaymentPeriod;
              acc.Enabled = true;
              acc.CreateUser = new XPQuery<PER_Person>(UoW).FirstOrDefault(p => p.PersonId == createUser);

              acc.Save();
            }
          }
          UoW.CommitChanges();
        }
        catch (Exception ex)
        {
          UoW.RollbackTransaction();

          using (var unit = new UnitOfWork())
          {
            var riskEnquiry = new XPQuery<BUR_Enquiry>(UoW).FirstOrDefault(o => o.EnquiryId == (Int64)transNo);
            riskEnquiry.IsSucess = false;
            unit.CommitChanges();
          }
          string refNo = StringUtils.RandomString(15);

          _log.Fatal(string.Format("Processing of enquiry response failed - {0} {1} {2} - Rolling back transaction.", ex.Message, ex.StackTrace, refNo));
          errorArr.Add(string.Format("Processing of enquiry response failed - {0} {1} - Rolling back transaction", ex.Message, refNo));
          throw;
        }
      }

      #endregion
    }


    /// <summary>
    /// Processes a new credit request or returns and old one.
    /// </summary>
    internal IResponseResult DoProcessOrDecode(bool reEnquire, long? transNo, BUR_EnquiryDTO riskRec, Request doRequest, string userName, string password, Risk.CreditCheckDestination destination)
    {
      if (reEnquire)
      {
        return new EnquiryServletImpl().DoRequest(destination, doRequest, transNo, userName, password);
      }
      else
      {
        var storage = GetFirstStorage(riskRec);
        if (storage != null)
        {
          if (riskRec.ObjectVersion == 1)
            return
              (IResponseResult)
                Xml.DeSerialize<ResponseResult>(Compression.Decompress(storage.ResponseMessage));
          return
            (IResponseResult)
              Xml.DeSerialize<ResponseResultV2>(
                Compression.Decompress(storage.ResponseMessage));
        }
        return null;
      }
    }


    /// <summary>
    /// Updates a risk record.
    /// </summary>
    internal void UpdateRiskRecord(IResponseResult result, long? transNo, ref List<string> errorArr)
    {
      if (!result.WasSucess)
      {
        errorArr.Add(string.Format("{0} - {1} ", result.Error, result.ErrorDescription));
      }
      else
      {
        using (var UoW = new UnitOfWork())
        {
          // Update the risk enquiry record
          var riskEnquiry = new XPQuery<BUR_Enquiry>(UoW).FirstOrDefault(o => o.EnquiryId == (Int64)transNo);

          if (riskEnquiry == null)
          {
            _log.Fatal(string.Format("The risk enquiry record with Id {0} appears to be missing", transNo));
            errorArr.Add(string.Format("The risk enquiry record with Id {0} appears to be missing", transNo));
          }

          riskEnquiry.EnquiryDate = DateTime.Now;
          riskEnquiry.IsSucess = true;

          riskEnquiry.Save();

          var riskStorage = new XPQuery<BUR_Storage>(UoW).FirstOrDefault(s => s.Enquiry.EnquiryId == (Int64)transNo);

          if (riskStorage == null)
          {
            _log.Fatal(string.Format("The risk enquiry storage record with Id {0} appears to be missing", transNo));
            errorArr.Add(string.Format("The risk enquiry storage record with Id {0} appears to be missing", transNo));
          }

          if (riskEnquiry.ObjectVersion > 1)
            riskStorage.ResponseMessage = Compression.Compress(Xml.Serialize<ResponseResultV2>((ResponseResultV2)result));
          else
            riskStorage.ResponseMessage = Compression.Compress(Xml.Serialize<ResponseResult>((ResponseResult)result));

          riskStorage.OriginalResponse = result.CompuscanResponse;


          UoW.CommitChanges();
        }
      }
    }


    /// <summary>
    /// Returns a request object to be sent to compuscan impl
    /// </summary>
    internal Request CreateRequest(string firstName, string surname, string IdNumber, string gender, DateTime dateOfBirth,
                                               string addressLine1, string addressLine2, string suburb, string city, string postalCode,
                                               string homeTelCode, string homeTelNo, string workTelCode, string workTelNo, string cellNo, bool isIDPassportNo,
                                               bool isExistingClient, long requestUser, string CC_enquiry, string CCPlusCPA_enquiry, string NLR_enquiry, string RunCodix,
                                               string Run_CompuScore)
    {
      return new Request()
      {
        Firstname = firstName,
        Surname = surname,
        IdentityNo = IdNumber,
        AddressLine1 = addressLine1,
        AddressLine2 = addressLine2,
        Suburb = suburb,
        City = city,
        PostalCode = postalCode,
        CellTelNo = cellNo,
        WorkTelNo = workTelNo,
        WorkTelCode = workTelCode,
        HomeTelNo = homeTelNo,
        HomeTelCode = homeTelCode,
        ResponseFormat = Risk.ResponseFormat.XMHT,
        DateOfBirth = dateOfBirth,
        Destination = Risk.CreditCheckDestination.LIVE,
        Gender = gender == "M" ? General.Gender.Male : General.Gender.Female,
        IsIDPassportNo = isIDPassportNo ? "Y" : "N",
        AddressMandatory = "Y",
        CCEnquiry = CC_enquiry,
        CCPlusCPAEnquiry = CCPlusCPA_enquiry,
        NLREnquiry = NLR_enquiry,
        EnquiryPurpose = Risk.EnquiryPurpose.Affordability_Assessment,
        RunCompuScore = Run_CompuScore,
        RunCodix = RunCodix
      };
    }


    /// <summary>
    /// Creates a cord risk enquiry record
    /// </summary>
    internal void CreateRiskRecord(long? accountId, Guid correlationId, BUR_EnquiryDTO enquiry, long? serviceId, string firstName, string surname, string idNumber, Request doRequest, long? requestUser, Risk.RiskTransactionType transactionType, long? branchId, out long? transNo)
    {
      using (var UoW = new UnitOfWork())
      {

        BUR_Enquiry riskEnquiry = new BUR_Enquiry(UoW)
        {
          PreviousEnquiry = enquiry == null ? null : new XPQuery<BUR_Enquiry>(UoW).FirstOrDefault(e => e.EnquiryId == enquiry.EnquiryId),
          CorrelationId = correlationId,
          Account = accountId == null ? null : new XPQuery<ACC_Account>(UoW).FirstOrDefault(b => b.AccountId == accountId),
          FirstName = firstName,
          LastName = surname,
          IdentityNum = idNumber,
          CreateDate = DateTime.Now,
          Service = new XPQuery<BUR_Service>(UoW).FirstOrDefault(b => b.ServiceId == serviceId),
          CreatedUser = requestUser == null ? null : new XPQuery<PER_Person>(UoW).FirstOrDefault(p => p.PersonId == requestUser),
          EnquiryType = Risk.RiskEnquiryType.Credit,
          EnquiryDate = DateTime.Now,
          TransactionType = transactionType,
          Branch = new XPQuery<BRN_Branch>(UoW).FirstOrDefault(p => p.BranchId == branchId),
          ObjectVersion = enquiry == null ? 2 : new XPQuery<BUR_Enquiry>(UoW).FirstOrDefault(e => e.EnquiryId == enquiry.EnquiryId).ObjectVersion
        };

        riskEnquiry.Save();

        var riskStorage = new BUR_Storage(UoW)
        {
          Enquiry = riskEnquiry,
          RequestMessage = Compression.Compress(Xml.Serialize<Request>(doRequest))
        };

        UoW.CommitChanges();
        transNo = riskEnquiry.EnquiryId;
      }
    }

    #endregion


    #region Private Methods

    private void SaveColour(long transNo, IResponseResult result)
    {
      using (var uow = new UnitOfWork())
      {
        var colour = new BUR_Colour(uow)
        {
          EnquiryId = new XPQuery<BUR_Enquiry>(uow).FirstOrDefault(e => e.EnquiryId == (Int64)transNo),
          R = result.R,
          G = result.G,
          B = result.B,
          Score = string.IsNullOrEmpty(result.Score) ? "0" : result.Score
        };

        uow.CommitChanges();
      }
    }


    /// <summary>
    /// Publishes response message to queue
    /// </summary>
    private void Respond(CreditRequest req, CreditResponse resp)
    {
      if (EnquiryCache.CheckItem(req.CorrelationId) != null)
        EnquiryCache.Update(req.CorrelationId, req, resp);
    }

    #endregion


    #region IDisposable

    public void Dispose()
    {

    }

    #endregion

  }
}