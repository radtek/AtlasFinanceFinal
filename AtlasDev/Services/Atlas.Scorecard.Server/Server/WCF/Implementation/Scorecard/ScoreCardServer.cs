using System;
using System.ServiceModel;
using System.Xml;
using System.Collections.Generic;

using Atlas.ThirdParty.CS.Enquiry;
using Atlas.ThirdParty.CS.WCF.Interface;
using Atlas.Common.Interface;

namespace Atlas.ThirdParty.CS.WCF.Implementation
{
  /// <summary>
  /// Credit Server Implementation
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]

  public class ScorecardServer : IScorecardServer
  {
    public ScorecardServer(ILogging log)
    {
      _log = log;
    }

    public ScorecardV2Result GetScorecardV2(string legacyBranchNum, string firstName, string surname, string idNumber, string gender, DateTime dateOfBirth,
                      string addressLine1, string addressLine2, string addressLine3, string addressLine4, string postalCode,
                      string homeTelCode, string homeTelNo, string workTelCode, string workTelNo, string cellNo, bool isIdPassportNo)
    {
      var result = new ScorecardV2Result();

      var methodName = "ScorecardServer.GetScorecardV2";
      var started = DateTime.Now;

      var request = new
      {
        legacyBranchNum,
        firstName,
        surname,
        idNumber,
        gender,
        dateOfBirth,
        addressLine1,
        addressLine2,
        addressLine3,
        addressLine4,
        postalCode,
        homeTelCode,
        homeTelNo,
        workTelCode,
        workTelNo,
        cellNo,
        isIdPassportNo
      };
      _log.Information("{MethodName}- Starting with: {@Request}", methodName, request);

      try
      {
        string errorMessage;
        string userName;
        string password;
        Int64 serviceId;
        Int64 branchId;
        Int64 enquiryId;
        var needNewScorecard = false;

        // Get recent scorecard, as well as scorecard branch credentials (in case we need to pull a new scorecard)
        var csZipFile = DataRepository.CheckForRecentScorecard(legacyBranchNum, idNumber, out userName, out password, out serviceId, out branchId, out enquiryId);
        if (csZipFile == null) // no recent scorecard, we need to request one...
        {
          needNewScorecard = true;

          #region Request a new scorecard from service provider
          _log.Information("{MethodName}- Getting scorecard: {userName}, {password}, {ServiceId}", methodName, userName, password, serviceId);
          csZipFile = ScoreXmlUtils.GetScorecardV2(_log, firstName, surname, idNumber, gender, dateOfBirth,
            addressLine1, addressLine2, addressLine3, addressLine4, postalCode, homeTelCode, homeTelNo, workTelCode, workTelNo,
            cellNo, isIdPassportNo, userName, password, out errorMessage);

          if (csZipFile == null) // scorecard failed
          {
            errorMessage = string.Format("Unexpected vendor error: {0}", errorMessage);
            _log.Error("{MethodName}- {Error}", methodName, errorMessage);
            result.ErrorMessage = errorMessage;
            return result;
          }

          _log.Information("{MethodName}- Successfully received and decoded scorecard ZIP- {zipBytesLength}", methodName, csZipFile.Length);
          #endregion
        }
        else
        {
          _log.Information("{MethodName}- Found recent scorecard- using stored", methodName);
        }

        #region Unzip the MHT and XML files from the ZIP file
        byte[] byteMhtFile;
        byte[] byteXmlFile;
        ScoreXmlUtils.UnzipScorecard(_log, csZipFile, out byteMhtFile, out byteXmlFile);
        if (byteMhtFile == null || byteMhtFile.Length == 0 || byteXmlFile == null || byteXmlFile.Length == 0)
        {
          result.ErrorMessage = "Invalid scorecard/MHT";
          return result;
        }
        #endregion

        if (needNewScorecard)
        {
          DataRepository.SaveScorecard(_log, byteXmlFile, csZipFile, branchId, started, idNumber, firstName, surname, serviceId, out enquiryId);
        }

        result.ScorecardXmlFileBase64 = Convert.ToBase64String(byteXmlFile);
        result.UserDisplayFileBase64 = Convert.ToBase64String(byteMhtFile);
        result.Successful = true;
        result.EnquiryId = enquiryId;
      }
      catch (Exception err)
      {
        _log.Error(err, "{MethodName}", methodName);
        result.ErrorMessage = "Unexpected server error";
      }

      _log.Information("{MethodName}- Completed with: Success: {Success}, Error: {Error}, ScoreBytesBase64: {ScoreCardBytes}, MHTBytesBase64: {MHTBytes}", methodName,
        result.Successful, result.ErrorMessage,
        result.ScorecardXmlFileBase64 != null ? result.ScorecardXmlFileBase64.Length : 0,
        result.UserDisplayFileBase64 != null ? result.UserDisplayFileBase64.Length : 0);

      return result;
    }
    

    public ScorecardSimpleResult GetSimpleScorecard(string legacyBranchNum, string firstName, string surname, string idNumber, bool isPassport)
    {
      var methodName = "ScorecardServer.GetSimpleScorecard";
      var started = DateTime.Now;
      ScorecardSimpleResult result;

      var request = new
      {
        legacyBranchNum,
        firstName,
        surname,
        idNumber,
        isPassport
      };
      _log.Information("{MethodName}- Starting with: {@Request}", methodName, request);

      var id = new Atlas.Common.Utils.IDValidator(idNumber);
      var scoreBase64 = GetScorecardV2(legacyBranchNum, firstName, surname, idNumber, id.isValid() ? id.IsFemale() ? "F" : "M" : "",
        id.isValid() ? id.GetDateOfBirthAsDateTime() : DateTime.Now.Subtract(TimeSpan.FromDays(36 * 365)),
        "", "", "", "", "", "", "", "", "", "", id.isValid());

      if (scoreBase64.Successful)
      {
        #region Parse the XML
        var scoreXml = System.Text.ASCIIEncoding.ASCII.GetString(System.Convert.FromBase64String(scoreBase64.ScorecardXmlFileBase64));
        var doc = new XmlDocument();
        doc.LoadXml(scoreXml);

        var reference = doc.SelectSingleNode("/ROOT/ConsumerEnquiry/ReturnData/NLR_ReferenceNo") ?? doc.SelectSingleNode("/ROOT/Enquiry_ID");
        var score = doc.SelectSingleNode("/ROOT/EnqCC_CompuSCORE/ROW");
        if (score == null)
        {
          return new ScorecardSimpleResult() { Error = "Scorecard invalid" };
        }

        var scoreVal = int.Parse(score["SCORE"].InnerText);
        _log.Information("{reference}", reference.InnerText);
        //var riskType = score["RISK_TYPE"].InnerText;

        #region Products
        var products = new List<AtlasProduct>();
        var productNodes = doc.SelectNodes("/ROOT/ATLAS_CODIX/PRODUCTS/PRODUCT");
        foreach (XmlNode productDetail in productNodes)
        {
          var reasons = new List<string>();
          var reasonNodeCollection = productDetail.SelectNodes("reasons/reason");
          if (reasonNodeCollection != null && reasonNodeCollection.Count > 0)
          {
            foreach (XmlNode reason in reasonNodeCollection)
            {
              reasons.Add(reason.InnerText);
            }
          }

          products.Add(new AtlasProduct()
            {
              ProductType = productDetail["product_type"].InnerText,
              ProductDescription = productDetail["product_description"].InnerText,
              Outcome = productDetail["outcome"].InnerText == "Y",
              Reasons = reasons
            });          
        }
        #endregion

        #endregion
                       
        result = new ScorecardSimpleResult() { Successful = true, AtlasProducts = products, Score = scoreVal, EnquiryId = scoreBase64.EnquiryId };
      }
      else
      {
        result = new ScorecardSimpleResult() { Successful = false, Error = scoreBase64.ErrorMessage };
      }

      return result;
    }

    #region Private vars

    // Log4net
    private readonly ILogging _log;

    #endregion
    
  }
}