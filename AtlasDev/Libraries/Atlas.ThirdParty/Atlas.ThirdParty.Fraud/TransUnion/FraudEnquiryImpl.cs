using Atlas.Common.ExceptionBase;
using Atlas.Common.Utils;
using Atlas.Domain.Model;
using DevExpress.Xpo;
//using Ninject.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.ThirdParty.Fraud.TransUnion
{
  public sealed class FraudEnquiryImpl
  {
		public FraudResult FraudCheck(BureauEnquiry42 transaction, long riskEnquiryId, long? previousEnquiryId)
		{
			var uow = new UnitOfWork();
			ConsumerSoapClient _client = null;
			var fraudResult = new FraudResult();
			// Get the enquiry record to update
			var riskEnquiry = new XPQuery<BUR_Enquiry>(uow).FirstOrDefault(o => o.EnquiryId == riskEnquiryId);

			try
			{
        if (_client == null)
          _client = new ConsumerSoapClient();

				var result = _client.ProcessRequestTrans42(transaction, Destination.Live);

				if (riskEnquiry == null)
					throw new RecordNotFoundException("Risk enquiry record was not found in the database.");

				// Save the initial request payload

				var riskStorage = new BUR_Storage(uow);
				riskStorage.Enquiry = riskEnquiry;
				riskStorage.RequestMessage = Compression.Compress(Xml.Serialize<BureauEnquiry42>(transaction));

				if (result.ResponseStatus == ResponseStatus.Success && result.FraudScoreFS01 != null)
				{
					var fpm = FraudScore(ref uow, result.FraudScoreFS01, ref fraudResult, riskEnquiryId, transaction.IdentityNo1, transaction.BankAccountNumber, transaction.CellNo);

					#region Segments

					AddressFrequency(ref uow, ref fpm, ref fraudResult, result.AddressVerificationNR01);
					ConsumerNumberFrequency(ref uow, ref fpm, ref fraudResult, result.ConsumerNumberFrequencyNY01);
					HawkAlert(ref uow, ref fpm, ref fraudResult, result.HawkNH05);
					HawkIdv(ref uow, ref fpm, ref  fraudResult, result.IdvNI01);
					CellPhoneValidation(ref uow, ref fpm, ref fraudResult, result.CellphoneValidationNZ01);
					ConsumerTelephoneHistory(ref uow, ref fpm, ref fraudResult, result.ConsumerTelephoneHistoryNW01);

					#endregion

					// It was a sucess
					riskEnquiry.IsSucess = true;
					fraudResult.EnquiryStatus = EnquiryStatus.Success;
					fraudResult.FraudScoreId = fpm.FraudScoreId;
					// Save the original response from bureau
					riskStorage.OriginalResponse = Compression.Compress(Xml.Serialize<BureauResponse>(result));
				}
				else
        {
					fraudResult.EnquiryStatus = EnquiryStatus.Error;
					fraudResult.Status = Enumerators.Account.AccountStatus.Technical_Error;
					riskEnquiry.IsSucess = false;
				}

				uow.CommitChanges();
				uow.Dispose();

				_client.Close();
				_client = null;
				result = null;
			}
			catch (Exception ex)
			{
				fraudResult.EnquiryStatus = EnquiryStatus.Error;
				fraudResult.Status = Enumerators.Account.AccountStatus.Technical_Error;
				riskEnquiry.IsSucess = false;
        throw new Exception("FraudCheck()", ex);
			}

			return fraudResult;
		}

    #region Internal Methods 

    internal FPM_FraudScore FraudScore(ref UnitOfWork uow, FraudScoreFS01 fraudScore, ref FraudResult result, long enquiryId, string idNo, string bankAccountNo, string cellNo)
    {
      var fraudscore = new FPM_FraudScore(uow);
      fraudscore.Enquiry = new XPQuery<BUR_Enquiry>(uow).FirstOrDefault(_ => _.EnquiryId == enquiryId);
      fraudscore.RecordSeq = fraudScore.RecordSequence.Trim();
      fraudscore.Part = fraudScore.Part.Trim();
      fraudscore.PartSeq = fraudScore.PartSequence.Trim();
      fraudscore.Rating = fraudScore.Rating.Trim();
      fraudscore.RatingDescription = fraudScore.RatingDescription.Trim();
      fraudscore.IDNumber = idNo;
      fraudscore.BankAccountNo = bankAccountNo;
      fraudscore.CellNo = cellNo;
      fraudscore.CreatedDate = DateTime.Now;

      int reasonCodeCount = fraudScore.ReasonCode.Count();

      if (result.ReasonCodes == null)
        result.ReasonCodes = new List<string>();

      for (int i = 0; i < reasonCodeCount; i++)
      {
        var fraudScoreReason = new FPM_FraudScore_Reason(uow);

        fraudScoreReason.FraudScore = fraudscore;
        fraudScoreReason.ReasonCode = fraudScore.ReasonCode[i];
        fraudScoreReason.Description = fraudScore.ReasonDescription[i];

        // Add reason codes to response
        result.ReasonCodes.Add(fraudScoreReason.ReasonCode);
      }

      // Set the rating to the response
      result.Rating = Convert.ToInt32(fraudScore.Rating);

      uow.CommitChanges();

      return fraudscore;
    }

    internal void AddressFrequency(ref UnitOfWork uow, ref FPM_FraudScore fraudscore, ref FraudResult result, AddressVerificationNR01 addressverification)
    {
      if (addressverification != null)
      {
        var addressVerification = new FPM_AddressFrequency(uow);
        addressVerification.FraudScore = fraudscore;
        addressVerification.AddressMessage = addressverification.AddressMessage.Trim();
        addressVerification.Last24Hours = Int32.Parse(addressverification.Last24Hours.Trim());
        addressVerification.Last30Days = Int32.Parse(addressverification.Last30Days.Trim());
        addressVerification.Last48Hours = Int32.Parse(addressverification.Last48Hours.Trim());
        addressVerification.Last96Hours = Int32.Parse(addressverification.Last96Hours.Trim());
      }
    }

    internal void ConsumerNumberFrequency(ref UnitOfWork uow, ref FPM_FraudScore fraudscore, ref FraudResult result, ConsumerNumberFrequencyNY01[] numberfrequency)
    {
      if (numberfrequency != null)
      {
				foreach (var number in numberfrequency)
				{
					var consumerNumberFrequency = new FPM_ConsumerTelephone(uow);
					consumerNumberFrequency.FraudScore = fraudscore;
					consumerNumberFrequency.CellPhoneNumber = number.CellNo.Trim();
					consumerNumberFrequency.CellPhoneTotal24Hours = Int32.Parse(number.CellNoTotal24Hrs.Trim());
					consumerNumberFrequency.CellPhoneTotal30Days = Int32.Parse(number.CellNoTotal30Days.Trim());
					consumerNumberFrequency.CellPhoneTotal48Hours = Int32.Parse(number.CellNoTotal96Hrs.Trim());
					consumerNumberFrequency.TelephoneNumberDial = number.TelephoneCode.Trim();
					consumerNumberFrequency.TelephoneNumber = number.TelephoneNo.Trim();
					consumerNumberFrequency.TelephoneTotal24Hours = Int32.Parse(number.TelephoneTotal24Hrs.Trim());
					consumerNumberFrequency.TelephoneTotal30Days = Int32.Parse(number.TelephoneTotal30Days.Trim());
					consumerNumberFrequency.TelephoneTotal48Hours = Int32.Parse(number.TelephoneTotal48Hrs.Trim());
					consumerNumberFrequency.TelephoneTotal96Hours = Int32.Parse(number.TelephoneTotal96Hrs.Trim());
				}
      }
    }

    internal void HawkAlert(ref UnitOfWork uow, ref FPM_FraudScore fraudscore, ref FraudResult result, HawkNH05 hawk)
    {
      if (hawk != null)
      {
        var hawkAlert = new FPM_HawkAlert(uow);
        hawkAlert.ContactName = hawk.ContactName;
        hawkAlert.ContactTelCode = hawk.ContactTelCode;
        hawkAlert.ContactTelNo = hawk.ContactTelNo;
        hawkAlert.DeceasedDate = hawk.DeceasedDate;
        hawkAlert.FraudScore = fraudscore;
        hawkAlert.HawkCode = hawk.HawkCode;
        hawkAlert.HawkDescription = hawk.HawkDesc;
        hawkAlert.HawkFoundFor = hawk.HawkFound;
        hawkAlert.HawkNo = hawk.HawkNo;
        hawkAlert.SubscriberName = hawk.SubscriberName;
        hawkAlert.SubscriberReference = hawk.SubscriberRef;
        hawkAlert.VictimReference = hawk.VictimReference;
        hawkAlert.VictimTelCode = hawk.VictimTelCode;
        hawkAlert.VictimTelNo = hawk.VictimTelNo;
      }
    }

    internal void HawkIdv(ref UnitOfWork uow, ref FPM_FraudScore fraudscore, ref FraudResult result, IdvNI01 idv)
    {
      if (idv != null)
      {
        var idvAlert = new FPM_HawkIDV(uow);
				idvAlert.DeceasedDate = idv.DeceasedDate;
        idvAlert.FraudScore = fraudscore;
        idvAlert.IDVerifiedCode = idv.IDVerifiedCode;
        idvAlert.IDVerifiedDescription = idv.IDVerifiedDesc;
        idvAlert.VerifiedForeName1 = idv.VerifiedForename1;
        idvAlert.VerifiedForeName2 = idv.VerifiedForename2;
        idvAlert.VerifiedSurname = idv.VerifiedSurname;
      }
    }

    internal void CellPhoneValidation(ref UnitOfWork uow, ref FPM_FraudScore fraudscore, ref FraudResult result, CellphoneValidationNZ01[] cellphonevalidation)
    {
      if (cellphonevalidation != null)
      {
        foreach (var cellphoneItem in cellphonevalidation)
        {
          var cellphoneValidation = new FPM_ConsumerCellphoneValidation(uow);
          cellphoneValidation.CellularNumber = cellphoneItem.CellNo;
          cellphoneValidation.CellularFirstUsed = cellphoneItem.CellDateFirstUsed;
          cellphoneValidation.CellularVerification = cellphoneItem.CellVerificationDesc;
          cellphoneValidation.FraudScore = fraudscore;
        }
      }
    }

		internal void ConsumerTelephoneHistory(ref UnitOfWork uow, ref FPM_FraudScore fraudscore, ref FraudResult result, ConsumerTelephoneHistoryNW01 telephoneHistory)
		{
			if (telephoneHistory != null)
			{
				foreach (var workPhone in telephoneHistory.WorkNumbers)
				{
					if (!string.IsNullOrEmpty(workPhone.Number))
					{
						var telephoneItem = new FPM_ConsumerTelephoneHistory(uow);
						telephoneItem.AreaCode = !string.IsNullOrEmpty(workPhone.AreaCode) ? workPhone.AreaCode : string.Empty;
						telephoneItem.Date = !string.IsNullOrEmpty(workPhone.Date) ? workPhone.Date : string.Empty;
						telephoneItem.FraudScore = fraudscore;
						telephoneItem.Number = !string.IsNullOrEmpty(workPhone.Number) ? workPhone.Number : string.Empty;
						telephoneItem.Years = !string.IsNullOrEmpty(workPhone.Years) ? workPhone.Years : string.Empty;
					}
				}

				foreach (var cellNo in telephoneHistory.CellNumbers)
				{
					if (!string.IsNullOrEmpty(cellNo.Number))
					{
						var telephoneItem = new FPM_ConsumerTelephoneHistory(uow);
						telephoneItem.AreaCode = !string.IsNullOrEmpty(cellNo.AreaCode) ? cellNo.AreaCode : string.Empty;
						telephoneItem.Date = !string.IsNullOrEmpty(cellNo.Date) ? cellNo.Date : string.Empty;
						telephoneItem.FraudScore = fraudscore;
						telephoneItem.Number = !string.IsNullOrEmpty(cellNo.Number) ? cellNo.Number : string.Empty;
						telephoneItem.Years = !string.IsNullOrEmpty(cellNo.Years) ? cellNo.Years : string.Empty;
					}
				}

				foreach (var homeNo in telephoneHistory.HomeNumbers)
				{
					if (!string.IsNullOrEmpty(homeNo.Number))
					{
						var telephoneItem = new FPM_ConsumerTelephoneHistory(uow);
						telephoneItem.AreaCode = !string.IsNullOrEmpty(homeNo.AreaCode) ? homeNo.AreaCode : string.Empty;
						telephoneItem.Date = !string.IsNullOrEmpty(homeNo.Date) ? homeNo.Date : string.Empty;
						telephoneItem.FraudScore = fraudscore;
						telephoneItem.Number = !string.IsNullOrEmpty(homeNo.Number) ? homeNo.Number : string.Empty;
						telephoneItem.Years = !string.IsNullOrEmpty(homeNo.Years) ? homeNo.Years : string.Empty;
					}
				}
			}
		}
    #endregion
  }
}
