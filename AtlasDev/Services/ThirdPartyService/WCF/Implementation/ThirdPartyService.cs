using log4net;
using System.ServiceModel;
using Atlas.ThirdParty.Service.Interface;
using DevExpress.Xpo;
using Atlas.Domain.Model;
using System.Linq;
using System;
using System.Collections.Generic;
using Atlas.Common.Utils;
using MassTransit;
using System.Dynamic;
using System.Configuration;
using Atlas.Common.Extensions;
using Atlas.Enumerators;
using Atlas.Common.ExceptionBase;
using Atlas.ThirdParty.Service.OrchestrationService;
using Atlas.RabbitMQ.Messages.Notification;
using Magnum;
//using Atlas.ThirdParty.Service.NaedoService;

namespace Atlas.ThirdParty.Service.Implementation
{
  /// <summary>
  /// Service relating to exposure of services for outside parties
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
  public sealed class ThirdPartyService : IThirdPartyService
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(ThirdPartyService));
    private static string BULK_MAX_INSERT = ConfigurationManager.AppSettings["naedo.maximum.bulk.insert"];

    #region Authentication
    /// <summary>
    /// Retrieve authentication ticket
    /// </summary>
    /// <param name="username">username stored in security store</param>
    /// <param name="password">password associated with username</param>
    /// <returns>authentication ticket</returns>
    public string Authenticate(string username, string password)
    {
      using (var uow = new UnitOfWork())
      {
        var credCheck = new XPQuery<BUR_Service>(uow).FirstOrDefault(p => p.ServiceType == Enumerators.Risk.ServiceType.Atlas_Naedo &&
          p.Username == username && p.Password == password && p.Enabled);

        if (credCheck == null)
        {
          _log.Error(string.Format("[Authenticate] - Credentials {0}:{1} where found in the security store", username, password));
          throw new Exception("Credentials not present in security store");
        }
      }

      string _authenticationTicket = Hash.Generate(username, password);

      var db = RedisConnection.Current.GetDatabase();

      string _redisKey = string.Format("{0}|{1}", Cryptography.Encrypt(string.Format("{0}|{1}", username, password),
        _authenticationTicket, password), DateTime.Now.ToString());

      db.StringSet(_authenticationTicket, _redisKey);
      db.KeyExpire(_authenticationTicket, new TimeSpan(0, 15, 0)); // 900 = 15mins

      _log.Info(string.Format("[Authenticate] - Generated ticket {0} for user {1} and password {2}", _authenticationTicket, username, password));

      return _authenticationTicket;
    }

    /// <summary>
    /// Determines if passed in authentication ticket is valid
    /// </summary>
    /// <param name="username">username stored in security store</param>
    /// <param name="password">password associated with username</param>
    /// <param name="authenticationTicket">authentication ticket received from Authenticate call</param>
    /// <returns>true/false</returns>
    public bool IsValid(string username, string password, string authenticationTicket)
    {
      var result = IsValid(authenticationTicket);

      if (result.Item1)
      {
        var _ticket = result.Item2.ToString().Split('|');

        #region authentication against username and password

        if (_ticket.Length >= 2)
        {
          var _authName = Cryptography.Decrypt(_ticket[0], authenticationTicket, password);

          if (_authName.Contains('|'))
          {
            var _split = _authName.Split('|');
            if ((_split[0] == username && _split[1] != password) || (_split[0] != username && _split[1] == password))
            {
              _log.Info(string.Format("[IsValid] - AuthenticationTicket {0} is invalid", authenticationTicket));
              return false;
            }
          }
        }

        #endregion

        DateTime ticketDate;
        DateTime.TryParse(_ticket[1], out ticketDate);

        return IsValid(authenticationTicket, ticketDate, null);
      }
      return false;
    }

    /// <summary>
    /// Determine if ticket exists in backing store.
    /// </summary>
    /// <param name="authenticationTicket">Ticket to determine if exists in backing store.</param>
    /// <returns>Tuple <True, AuthenticationTicket></returns>
    internal Tuple<bool, string> IsValid(string authenticationTicket)
    {
      var db = RedisConnection.Current.GetDatabase();

      var result = db.StringGet(authenticationTicket);

      if (string.IsNullOrEmpty(result))
      {
        _log.Info(string.Format("[IsValid] - AuthenticationTicket {0} is invalid", authenticationTicket));
        return new Tuple<bool, string>(false, authenticationTicket);
      }

      return new Tuple<bool, string>(true, result);
    }

    /// <summary>
    /// Used internally to determine life time of ticket, this should be used when a transaction is processed.
    /// </summary>
    /// <param name="authenticationTicket">Ticket to determine if valid</param>
    /// <param name="tolerance">Tolerance time for expiration, i.e. expired 20 seconds ago.</param>
    /// <returns>true/false</returns>
    internal bool IsValid(string authenticationTicket, DateTime ticketCreationTime, TimeSpan? tolerance)
    {
      TimeSpan ts = (DateTime.Now - ticketCreationTime);

      if (tolerance != null)
      {
        // Combine timespan with tolerance to determine if ts has overshot tolerance
        TimeSpan toleranceTs = (ts + (TimeSpan)tolerance);

        // equality match against two ts's
        if (ts.TotalSeconds > toleranceTs.TotalSeconds)
        {
          _log.Info(string.Format("[IsValid] - AuthenticationTicket {0} is invalid", authenticationTicket));
          return false;
        }
        else
        {
          _log.Info(string.Format("[IsValid] - AuthenticationTicket {0} is valid", authenticationTicket));
          return true;
        }
      }
      else
      {
        // if tolerance is null default to 15mins, and ignore tolerance, is invalid immediatly regardless of seconds
        if (ts.TotalSeconds >= 900)
        {
          _log.Info(string.Format("[IsValid] - AuthenticationTicket {0} is invalid", authenticationTicket));
          return false;
        }

        _log.Info(string.Format("[IsValid] - AuthenticationTicket {0} is valid", authenticationTicket));
        return true;
      }
    }

    /// <summary>
    /// Determines if a ticket is valid
    /// </summary>
    internal dynamic TransactionPreVerification(string authenticationTicket)
    {
      var ticket = IsValid(authenticationTicket);
      if (!ticket.Item1)
      {
        return new { TicketMissing = true, ToleranceFailure = false };
      }
      else
      {
        var _splitTicket = ticket.Item2.ToString().Split('|');

        if (_splitTicket.Length >= 2)
        {
          DateTime ticketTime;
          DateTime.TryParse(_splitTicket[1], out ticketTime);
          if (!IsValid(authenticationTicket, ticketTime, new TimeSpan(0, 0, 30)))
          {
            return new { TicketMissing = false, ToleranceFailure = true };
          }
        }
      }
      return new { TicketMissing = false, ToleranceFailure = false };
    }

    /// <summary>
    /// Immediately invalidates a authentication ticket.
    /// </summary>
    /// <param name="authenticationTicket">authentication ticket to invalidate</param>
    /// <returns>true/false</returns>
    public bool InValidate(string authenticationTicket)
    {
      var db = RedisConnection.Current.GetDatabase();

      _log.Info(string.Format("[InValidate] - AuthenticationTicket {0} has been invalidated", authenticationTicket));
      return db.KeyDelete(authenticationTicket);
    }

    /// <summary>
    /// Generate a checksum based on paassed parameters
    /// </summary>
    /// <param name="transaction">Parameters to generate a checksum against</param>
    /// <returns>Generated checksum</returns>
    public string GenerateCheckSum(Structure.Transaction transaction)
    {
      return Verify.GenerateCheckSum<Structure.Transaction>(transaction);
    }

    #endregion

    #region Transactions

    ///// <summary>
    ///// Submit a bulk naedo transaction set.
    ///// </summary>
    ///// <param name="transactions">
    ///// Type of List with dictionaries which contain a guid identifier field and another dictionary
    ///// with a string for checksum and the transaction structure. The Guid field must be supplied by 
    ///// the third party in order for them to be able to correlate the record in the submission on their 
    ///// side    /// 
    ///// </param>
    //public Dictionary<Guid, long> NAEDO_SubmitBulk(Dictionary<Guid, Dictionary<string, Structure.Transaction>> transactions)
    //{
    //  Dictionary<Guid, long> responseDictionary = new Dictionary<Guid, long>();

    //  if (transactions.Count == 0)
    //    throw new Exception("No results contained in the transaction set.");

    //  if (transactions.Count > Convert.ToInt32(BULK_MAX_INSERT))
    //    throw new Exception("Transaction count exceeds the maximum allowed submission total.");

    //  bool isValidatedForSubmission = false;

    //  foreach (KeyValuePair<Guid, Dictionary<string, Structure.Transaction>> dicts in transactions)
    //  {
    //    foreach (KeyValuePair<string, Structure.Transaction> item in dicts.Value)
    //    {
    //      if (!isValidatedForSubmission)
    //      {
    //        // Determine if ticket exists and also if it has not expired against tolerance
    //        var preVal = TransactionPreVerification(item.Value.AuthenticationTicket);

    //        if (preVal.TicketMissing && !preVal.ToleranceFailure)
    //          throw new Exception("Authentication ticket does not exist in backing store.");
    //        else if (preVal.TicketMissing && preVal.ToleranceFailure)
    //          throw new Exception("Authentication ticket has expired the alloted time.");
    //        else if (!preVal.TicketMissing && !preVal.ToleranceFailure)
    //        {
    //          isValidatedForSubmission = true;
    //          responseDictionary.Add(dicts.Key, Submit(item.Key, item.Value));
    //        }
    //      }
    //      else
    //      {
    //        responseDictionary.Add(dicts.Key, Submit(item.Key, item.Value));
    //      }
    //    }
    //  }
    //  return responseDictionary;
    //}


    ///// <summary>
    ///// Prevlidate naedo transaction prior to submission, although they will be run through validation again,
    ///// this is to help the third party in order to determine quickly if there are any possible faulty transactions.
    ///// </summary>
    ///// <param name="transactions"></param>
    ///// <returns></returns>
    //public Dictionary<Guid, List<string>> NAEDO_PreValidate(Dictionary<Guid, Structure.Transaction> transactions)
    //{
    //  Dictionary<Guid, List<string>> responseContainer = new Dictionary<Guid, List<string>>();

    //  try
    //  {
    //    if (transactions.Count == 0)
    //      throw new Exception("Collection does not contain any transactions to prevalidate.");      

    //    foreach (KeyValuePair<Guid, Structure.Transaction> keyPair in transactions)
    //    {
    //      _log.Info(string.Format("[NAEDO_PreValidate] - Prevalidating transaction {0}", keyPair.Value.ThirdPartyReference));

    //      List<string> validationErrors = IsValid(keyPair.Value);

    //      if (validationErrors.Count > 0)
    //      {
    //        responseContainer.Add(keyPair.Key, validationErrors);
    //        _log.Warn(string.Format("[NAEDO_PreValidate] - Prevalidating transaction {0} failed with follow errors {1}", keyPair.Value.ThirdPartyReference, string.Join(",", validationErrors.ToArray())));
    //      }
    //      else
    //      {
    //        _log.Info(string.Format("[NAEDO_PreValidate] - Prevalidating transaction {0} passed.", keyPair.Value.ThirdPartyReference));
    //      }
    //    }
    //  }catch(Exception ex)
    //  {
    //    _log.Fatal(ex);
    //    throw ex;
    //  }
    //  return responseContainer;
    //}

    ///// <summary>
    ///// Submit a naedo transaction
    ///// </summary>
    ///// <param name="checkSum">checksum</param>
    ///// <param name="transaction">Naedo structure</param>
    ///// <returns>Unique reference in order to query naedo result</returns>
    //public long NAEDO_Submit(string checkSum, Structure.Transaction transaction)
    //{
    //  // Determine if ticket exists and also if it has not expired against tolerance
    //  var preVal = TransactionPreVerification(transaction.AuthenticationTicket);

    //  if (preVal.TicketMissing && !preVal.ToleranceFailure)
    //    throw new Exception("Authentication ticket does not exist in backing store.");
    //  else if (preVal.TicketMissing && preVal.ToleranceFailure)
    //    throw new Exception("Authentication ticket has expired the alloted time.");

    //  return Submit(checkSum, transaction);
    //}

    ///// <summary>
    ///// Get control id for transaction
    ///// </summary>
    ///// <param name="checkSum">checksum</param>
    ///// <param name="transaction">Naedo structure</param>
    ///// <returns>Unique reference in order to query naedo result</returns>
    //public long? NAEDO_GetControlId(Structure.Transaction transaction)
    //{
    //    // Add Validation to structure
    //    var validationErrors = IsValid(transaction);
    //    if (validationErrors.Count == 0)
    //    {
    //      Atlas.Enumerators.General.Host host = Enumerators.General.Host.SDC;

    //      using (var uow = new UnitOfWork())
    //      {
    //        var control = new XPQuery<DBT_Control>(uow).FirstOrDefault(c => c.BankStatementReference == transaction.BankStatementReference && c.ThirdPartyReference == transaction.ThirdPartyReference && c.Host.Type == host
    //                                    && (c.ControlStatus.Type == Debit.ControlStatus.New || c.ControlStatus.Type == Debit.ControlStatus.InProcess));
    //        if (control != null)
    //        {
    //          _log.Info(string.Format("[NAEDO_GetControlId] - Getting control id for third party reference {0} control id is {1}", transaction.ThirdPartyReference, control.ControlId));
    //          return control.ControlId;
    //        }
    //        else
    //          return null;
    //      }
    //    }
    //  return null;
    //}

    ///// <summary>
    ///// Get control id for a transction that could have occurred during error/slow connection.
    ///// </summary>
    ///// <param name="thirdPartyReferenceNo">Reference no which to lookup control for.</param>
    //public long? NAEDO_LinkTransactionToControl(string thirdPartyReferenceNo)
    //{
    //  if (string.IsNullOrEmpty(thirdPartyReferenceNo))
    //    throw new Exception("ThirdPartyReferenceNo may not be blank");

    //  General.Host host = Enumerators.General.Host.SDC;

    //  using (var uow = new UnitOfWork())
    //  {
    //    var control = new XPQuery<DBT_Control>(uow).FirstOrDefault(c => c.ThirdPartyReference == thirdPartyReferenceNo.Trim() && c.Host.Type == host);
    //    if (control != null)
    //    {
    //      _log.Info(string.Format("[NAEDO_LinkTransactionToControl] - Getting control id for third party reference {0} control id is {1}", thirdPartyReferenceNo, control.ControlId));
    //      return control.ControlId;
    //    }
    //    else
    //      return null;
    //  }
    //}

    ///// <summary>
    ///// Submit NAEDO
    ///// </summary>
    //internal long Submit(string checkSum, Structure.Transaction transaction)
    //{
    //  if (Verify.VerifyCheckSum<Structure.Transaction>(checkSum, transaction, "NAEDO_Submit"))
    //  {
    //    // Add Validation to structure
    //    var validationErrors = IsValid(transaction);
    //    if (validationErrors.Count == 0)
    //    {
    //      try
    //      {
    //        Atlas.Enumerators.General.Host host = Enumerators.General.Host.SDC;
    //        long? branchId = (long?)null;
    //        Atlas.Enumerators.General.BankName bank = (Atlas.Enumerators.General.BankName)(int)transaction.Bank;
    //        Atlas.Enumerators.General.BankAccountType bankAccountType = (Atlas.Enumerators.General.BankAccountType)(int)transaction.BankAccountType;
    //        Atlas.Enumerators.Debit.TrackingDay trackingDays = (Atlas.Enumerators.Debit.TrackingDay)(int)transaction.TrackingDay;
    //        Atlas.Enumerators.Account.PayRule payRule = Enumerators.Account.PayRule.Sun_To_Mon;

    //        using (var uow = new UnitOfWork())
    //        {
    //          var control = new XPQuery<DBT_Control>(uow).FirstOrDefault(c => c.BankStatementReference == transaction.BankStatementReference && c.ThirdPartyReference == transaction.ThirdPartyReference && c.Host.Type == host
    //                                      && (c.ControlStatus.Type == Debit.ControlStatus.New || c.ControlStatus.Type == Debit.ControlStatus.InProcess));
    //          if (control != null)
    //          {
    //            var error = string.Format("Control with ThirdPartyReference: {0} and Contract Reference: {1} already exists for Host {2}", transaction.ThirdPartyReference, transaction.BankStatementReference, host.ToStringEnum());
    //            _log.Error(error);
    //            return -50;
    //          }
    //        }

    //        _log.Info(string.Format("[Submit] - Submit naedo transaction : Host: {0}, Bank: {1}, BankAccountType: {2}, TrackingDays: {3}, PayRule: {4}, BankStatementRef: {5}, ThirdPartyReference: {6}, BankAccountNo: {7}, BankAccountName: {8}, IdNumber: {9}, Amount: {10}, InstalmentDate: {11}",
    //          host.ToStringEnum(), bank.ToStringEnum(), bankAccountType.ToStringEnum(), trackingDays.ToStringEnum(),
    //          payRule.ToStringEnum(), transaction.BankStatementReference, transaction.ThirdPartyReference, transaction.BankAccountNo,
    //          transaction.BankAccountName, transaction.IdNumber, transaction.Amount, transaction.InstalmentDate));

    //        long controlId = 0;
    //        new NaedoService.NaedoServiceClient("NaedoService.NET").Using((client) =>
    //        {              
    //          controlId = client.AddNewDebitOrder(host, branchId, transaction.ThirdPartyReference, transaction.BankStatementReference.ToUpper(), bank, transaction.BankBranchCode, transaction.BankAccountNo,
    //           bankAccountType, transaction.BankAccountName, transaction.IdNumber, Enumerators.Debit.FailureType.Continue, trackingDays, Enumerators.Debit.AVSCheckType.ControlCreation, 1, transaction.Amount,
    //           Enumerators.Account.PeriodFrequency.Monthly, transaction.InstalmentDate, payRule, Enumerators.Account.PayDateType.DayOfMonth, null, transaction.InstalmentDate.Day, null);

    //          _log.Info(string.Format("[Submit] - Submitt naedo transaction ThirdPartyReference: {0} returned with ControlId {1}", transaction.ThirdPartyReference, controlId));
    //        });

    //        return controlId;
    //      }
    //      catch (Exception exception)
    //      {
    //        _log.Error(string.Format("[NAEDO_Submit] {0}, {1}", exception.Message, exception.StackTrace));
    //        throw exception;
    //      }
    //    }
    //    else
    //    {
    //      _log.Fatal(string.Format("Error Submittiing Transaction: Parameters not valid - {0}", validationErrors.ToArray()));

    //      throw new Exception(string.Format("Error Submittiing Transaction: Parameters not valid - {0}", validationErrors.ToArray()));
    //    }
    //  }
    //  else
    //  {
    //    throw new UnauthorizedAccessException("Checksum invalid");
    //  }
    //}
     

    ///// <summary>
    ///// Gets response for transaction - successfully/unsuccessfully collected/still waiting for a response
    ///// </summary>
    //public Dictionary<Guid, Structure.Response> NAEDO_GetResponseBulk(Dictionary<Guid, long> transactions)
    //{
    //  Dictionary<Guid, Structure.Response> responseContainer = new Dictionary<Guid,Structure.Response>();
    //  foreach(var keyPair in transactions)
    //  {
    //    responseContainer.Add(keyPair.Key, NAEDO_GetResponse(keyPair.Value));
    //  }
    //  return responseContainer;
    //}

    ///// <summary>
    ///// Gets response for transaction - successfully/unsuccessfully collected/still waiting for a response
    ///// </summary>
    ///// <param name="referenceId">Unique reference received from Submit Method</param>
    ///// <returns>naedo response structure</returns>
    //public Structure.Response NAEDO_GetResponse(long referenceId)
    //{
    //  try
    //  {
    //    _log.Info(string.Format("[NAEDO_GetResponse] - Retrieving response for naedo transaction with reference {0}", referenceId));

    //    var response = new Structure.Response();

    //    using(var uow = new UnitOfWork())
    //    {
    //      var control = new XPQuery<DBT_Control>(uow).FirstOrDefault(p => p.ControlId == referenceId);
    //      if (control == null)
    //      {
    //        _log.Warn(string.Format("[NAEDO_GetResponse] - No control record found for reference {0}", referenceId));
    //        return null;
    //      }
    //    }

    //    using (var naedoService = new NaedoService.NaedoServiceClient("NaedoService.NET"))
    //    {
    //      var debitOrderResult = naedoService.GetDebitOrderResults(referenceId, null);
    //      response.Amount = debitOrderResult.Instalment;
    //      response.ControlStatus = (Structure.Enums.ControlStatus)(int)debitOrderResult.ControlStatus;
    //      response.ReferenceId = debitOrderResult.ControlId;
    //      response.ThirdPartyReference = debitOrderResult.ThirdPartyReference;

    //      response.ValidationErrors = new List<Structure.Enums.ValidationError>();
    //      foreach (var validationError in debitOrderResult.ValidationErrors)
    //      {
    //        response.ValidationErrors.Add((Structure.Enums.ValidationError)(int)validationError);
    //      }

    //      if (debitOrderResult.ResponseTransactions != null && debitOrderResult.ResponseTransactions.Length == 1)
    //      {
    //        var responseTransaction = debitOrderResult.ResponseTransactions.FirstOrDefault();
    //        response.InstalmentDate = responseTransaction.ActionDate;
    //        response.ResponseCode = responseTransaction.ResponseCode;
    //        response.ResponseDescription = responseTransaction.ResponseCodeDescription;
    //        response.Status = (Structure.Enums.Status)(int)responseTransaction.Status;
    //      }
    //      else
    //      {
    //        response.ResponseCode = string.Empty;
    //        response.InstalmentDate = debitOrderResult.FirstInstalmentDate;
    //        if (response.ControlStatus == Structure.Enums.ControlStatus.New)
    //          response.Status = Structure.Enums.Status.New;
    //        else if (response.ControlStatus == Structure.Enums.ControlStatus.InProcess)
    //          response.Status = Structure.Enums.Status.New;
    //        else if (response.ControlStatus == Structure.Enums.ControlStatus.Completed)
    //          response.Status = Structure.Enums.Status.Failed;
    //        else if (response.ControlStatus == Structure.Enums.ControlStatus.Cancelled || response.ControlStatus == Structure.Enums.ControlStatus.Cancelled_ValidationErrors)
    //          response.Status = Structure.Enums.Status.Cancelled;
    //      }
    //    }
    //    return response;
    //  }
    //  catch (Exception exception)
    //  {
    //    _log.Error(string.Format("[NAEDO_GetResponse] - Error getting response for transaction with reference {0} - {1}, {2}", referenceId, exception.Message, exception.StackTrace));
    //    throw exception;
    //  }
    //}

    ///// <summary>
    ///// Generates a cancellation request token for a particular control
    ///// </summary>
    ///// <param name="idNo">Identity number of person requesting cancellation</param>
    ///// <param name="cellNo">Cell No of person requesting cancellation</param>
    ///// <param name="controlId"></param>
    ///// <returns></returns>
    //public string NAEDO_RequestCancellationToken(string idNo, string cellNo, long controlId)
    //{
    //  using(var uow = new UnitOfWork())
    //  {
    //    _log.Info(string.Format("[RequestCancellationToken] - Cancellation token requested for Id No {0}, Cell No {1} and ControlId {2}", idNo, cellNo, controlId));
    //    var requestPerson = new XPQuery<PER_Person>(uow).FirstOrDefault(p => p.IdNum == idNo);

    //    if (requestPerson == null)
    //      throw new Exception("Requested user was not found in datastore.");

    //    var requestCellNo = requestPerson.Contacts.FirstOrDefault(p => p.Value == cellNo);

    //    if (requestCellNo == null)
    //      throw new Exception(string.Format("Cell No for user {0} does not match what is on the datastore.", requestPerson.PersonId));

    //    var db = RedisConnection.Current.GetDatabase();

    //    var hash = Hash.Generate(requestPerson.PersonId, requestCellNo.Value, controlId);

    //    TupleOfintstring otp = null;

    //    using (var orchestration = new OrchestrationService.OrchestrationServiceClient("OrchestrationService.NET"))
    //    {
    //      otp = orchestration.GenerateOTP();          
    //    }

    //    Bus.Instance().Publish<SMSNotifyMessage>(new SMSNotifyMessage(CombGuid.Generate())
    //    {
    //      ActionDate = DateTime.Now,
    //      Body = string.Format("Naedo cancellation OTP :{0}", otp.m_Item1),
    //      CreatedAt = DateTime.Now,
    //      Priority = Notification.NotificationPriority.High,
    //      To = requestCellNo.Value
    //    });
    //    db.StringSet(hash, string.Format("{0}:{1}", otp.m_Item2, controlId));
    //    _log.Info(string.Format("[RequestCancellationToken] - Cancellation token request finished for Id No {0}, Cell No {1}, ControlId {2} with checksum {3}", idNo, cellNo, controlId, hash));

    //    return hash;
    //  }
    //}

    ///// <summary>
    ///// Verifies cancellation token and OTP and cancels naedo
    ///// </summary>
    ///// <param name="checkSum">Checksum generated via RequestCancellationToken</param>
    ///// <param name="OTP">OTP delivered to the requesters cell phone.</param>
    //public bool NAEDO_VerifyAndCancel(string checkSum, int OTP)
    //{

    //  _log.Info(string.Format("[VerifyAndCancel] - Request with checksum {0} and OTP {1}", checkSum,OTP));

    //  var db = RedisConnection.Current.GetDatabase();

    //  var result = db.StringGet(checkSum);

    //  if (result.IsNull)
    //    throw new Exception("Checksum appears to be invalid.");

    //  string[] key = result.ToString().Split(':');

    //  string securityHash = key[0];
    //  string controlId = key[1];

    //  bool validation = false;
    //  using (var orchestration = new OrchestrationService.OrchestrationServiceClient("OrchestrationService.NET"))
    //  {
    //    validation = orchestration.VerifyOTP(securityHash, Convert.ToInt32(OTP));
    //  }

    //  _log.Info(string.Format("[VerifyAndCancel] - Request with checksum {0} and OTP {1}, OTP verification result of {2}", checkSum, OTP, validation));
    //  if (validation)
    //    return false;

    //  if (validation)
    //  {
    //    Response response = null;

    //    using (var naedo = new NaedoService.NaedoServiceClient("NaedoService.NET"))
    //    {
    //      response = naedo.StopDebitOrder(Convert.ToInt64(controlId), true);
    //    }
    //    _log.Info(string.Format("[VerifyAndCancel] - Request with checksum {0} and OTP {1}, OTP verification result of {2}, and cancellation result of {3}", checkSum, OTP, validation, response.ControlStatus.Value.ToStringEnum()));
    //    if (response.ControlStatus.HasValue && response.ControlStatus.Value == Debit.ControlStatus.Cancelled)
    //      return true;
    //  }
    //  return false;
    //}

    #endregion

    private List<string> IsValid(Structure.Transaction transaction)
    {
      List<string> validationErrors = new List<string>();
      if (transaction.Amount < 100)
        validationErrors.Add("Validation: Amount may not be less than 100");
      if (transaction.Bank == 0)
        validationErrors.Add("Validation: Bank supplied is invalid");
      if (string.IsNullOrEmpty(transaction.BankAccountName) || transaction.BankAccountName.Length > 30)
        validationErrors.Add("Validation: BankAccountName cannot be blank or greater than 30 characters");
      if (string.IsNullOrEmpty(transaction.BankAccountNo) || transaction.BankAccountNo.Length > 11)
        validationErrors.Add("Validation: BankAccountNo cannot be blank or greater than 11 characters");
      if (transaction.BankAccountType == 0)
        validationErrors.Add("Validation: BankAccountType supplied is invalid");
      if (string.IsNullOrEmpty(transaction.BankBranchCode))
        validationErrors.Add("Validation: BankBranchCode cannot be blank");
      if (string.IsNullOrEmpty(transaction.BankStatementReference) || transaction.BankStatementReference.Length > 10)
        validationErrors.Add("Validation: BankStatementReference cannot be blank or greater than 10 characters");
      if (string.IsNullOrEmpty(transaction.IdNumber))
        validationErrors.Add("Validation: IdNumber may not be blank");
      if (transaction.InstalmentDate <= DateTime.Today)
        validationErrors.Add("Validation: InstalmentDate cannot be before todays date");
      if (transaction.InstalmentDate.Date == DateTime.Now.Date)
        validationErrors.Add("Validation: InstalmentDate cannot be todays date");
      if (transaction.InstalmentDate.Date <= DateTime.Today.AddDays(+2))
        validationErrors.Add("Validation: InstalmentDate must be at least 3 days prior to strike date");
      if (transaction.TrackingDay == 0)
        validationErrors.Add("Validation: TrackingDay supplied is not valid");
      return validationErrors;
    }
  }
}