#region Using

using Atlas.Enumerators;
using Atlas.Online.Web.Models.Dto;
using Atlas.Online.Web.Service.AccountService;
using Atlas.Online.Web.Service.Entities.App;
using Atlas.Online.Web.Service.Entities.Otp;
using Atlas.Online.Web.Service.Entities.Verification;
using System;
using System.Collections.Generic;
using System.ServiceModel;

#endregion

namespace Atlas.Online.Web.Service.WCF.Interface
{
  [ServiceContract]
  public interface IWebService
  {
    #region OTP

    [OperationContract]
    OtpSendResult OTP_Send(long clientId, bool sendFirst);

    [OperationContract]
    bool OTP_Verify(long clientId, int otp);

    #endregion

    #region AVS
    [OperationContract]
    long? AVS_Submit(long clientId);

    [OperationContract]
    AVS.Result? AVS_GetResponse(long? personId, long transactionId);

    #endregion

    #region CDV

		[OperationContract]
		bool CDV_VerifyAccount(General.BankName bank, General.BankAccountType bankAccountType, string accountNo);

    #endregion

		#region Application

		[OperationContract]
		string APP_Submit(long clientId);

    [OperationContract]
    ApplicationAffordability APP_GetAffordability(long applicationId);

    [OperationContract]
    bool APP_AcceptAffordability(long optionId);

    [OperationContract]
    ApplicationSettlementResult APP_SubmitSettlement(ApplicationSettlementSubmission settlement);

    [OperationContract]
    int APP_ApplyIn(long clientId);

    [OperationContract]
    ApplicationDeclinedReason APP_GetDeclinedReason(long applicationId);


    [OperationContract]
    LoanRulesDto APP_SliderRules(long? clientId);

    #endregion

    #region Quote

    /// <summary>
    /// To be called on quote page load
    /// </summary>
    [OperationContract]
    bool QTE_PreApprove(long applicationId);

    /// <summary>
    /// To be called when one clicks accept
    /// </summary>
    [OperationContract]
    bool QTE_AcceptQuote(long applicationId);

    /// <summary>
    /// To be caleld when one rejects the quotation.
    /// </summary>
    [OperationContract]
    bool QTE_RejectQuote(long applicationId);

    [OperationContract]
    Quotation QTE_GetQuote(long applicationId);

    #endregion

    #region Verification

    [OperationContract]
    IEnumerable<Questions> VER_GetQuestions(long clientId);

    [OperationContract]
    VerificationResult VER_SubmitQuestions(long applicationId, long clientId, List<Questions> questions);

    [OperationContract]
    VerificationResult VER_CheckStatus(long applicationId, long clientId);

    #endregion

    #region My Account

    [OperationContract]
    AccountStatement MYC_GetClientStatement(long applicationId);

    [OperationContract]
    ApplicationSettlementResponse MYC_CheckSettlement(long applicationId);

    [OperationContract]
    Settlement MYC_GetSettlementQuotation(long applicationId);

    [OperationContract]
    Quotation MYC_GetQuote(long applicationId);

    [OperationContract]
    PaidUpLetter MYC_GetPaidUpLetter(long applicationId);

    #endregion

    #region Loan Rules


    #endregion

    #region NTF

    [OperationContract]
    bool NTF_Registration(string firstName, string lastName, string cellNo, string url, string username);

    [OperationContract]
    bool NTF_ForgotPassword(string firstName, string lastName, string token, string userName);

    #endregion

    #region General

    [OperationContract]
    List<DateTime> UTL_GetHolidays();

    #endregion

  }
}
