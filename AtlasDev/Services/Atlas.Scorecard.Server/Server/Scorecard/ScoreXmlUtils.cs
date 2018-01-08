using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Web;
using System.Text;

using Atlas.Common.Extensions;
using Atlas.Common.Utils;
using Atlas.Enumerators;
using Atlas.ThirdParty.CS.Bureau;
using Atlas.Common.Interface;


namespace Atlas.ThirdParty.CS.Enquiry
{
  /// <summary>
  /// CompuScan scorecard XML utilities
  /// </summary>
  internal static class ScoreXmlUtils
  {
    /// <summary>
    /// Requests a new Version 2 scorecard from CompuScan via SOAP
    /// </summary>
    /// <param name="log"></param>   
    /// <param name="errorMessage"></param>
    /// <returns>ZIP file, null if error</returns>
    internal static byte[] GetScorecardV2(ILogging log, string firstName, string surname, string IdNumber, string gender,
      DateTime dateOfBirth, string addressLine1, string addressLine2, string addressLine3, string addressLine4,
      string postalCode, string homeTelCode, string homeTelNo, string workTelCode, string workTelNo, string cellNo,
      bool isIDPassportNo, string userName, string password, out string errorMessage)
    {
      var methodName = "GetScorecardV2";
      byte[] result = null;
      errorMessage = null;

      #region Create XML request
      // The posted XML is pretty static, there is no need to get fancy...
      var pTransactionXml = GetTransactionXmlV2(gender, IdNumber, firstName, surname, isIDPassportNo, dateOfBirth,
        addressLine1, addressLine2, addressLine3, addressLine4, postalCode,
        homeTelCode, homeTelNo, workTelCode, workTelNo, cellNo);

      #endregion

      log.Information("SOAP {pTransaction} request", pTransactionXml);
      try
      {
        string error = null;
        new NormalSearchServiceClient("NormalSearchServicePort").Using(client =>
            {
              client.PingServer();
              var enquiryResult = client.DoNormalEnquiry(new NormalEnqRequestParamsType()
              {
                pInput_Format = "XML",          // Input type: XML
                pOrigin = "Atlas",              // Name of the originating application
                pOrigin_Version = "1.0",        // Version of originating application
                pPasswrd = password,            // Compuscan user password
                pUsrnme = userName,             // Compuscan user name  
                pVersion = "1.0",               // Will be used to define the version of the input and output types. Current version = 1.0
                pTransaction = pTransactionXml  // Input string
              });

              if (enquiryResult != null)
              {
                if (enquiryResult.transactionCompleted)
                {
                  result = Convert.FromBase64String(enquiryResult.retData);
                }
                else
                {
                  error = string.Format("[{0}]- {1}", enquiryResult.errorCode, enquiryResult.errorString);
                }
              }
              else
              {
                error = "Empty response from CompuScan";
              }
            });

        errorMessage = error;
      }
      catch (Exception err)
      {
        log.Error(err, "{MethodName}", methodName);
        errorMessage = err.Message;
      }

      return result;
    }


    /// <summary>
    /// Decodes a CompuScan scorecard to the useless 'ResponseResultV2', for storage in BUR_Storage.ResponseMessage
    /// </summary>
    /// <param name="log">Logging system</param>
    /// <param name="xml">The CompuScan XML</param>
    /// <returns>An equivalent ResponseResultV2</returns>
    internal static ResponseResultV2 Deserialize(ILogging log, byte[] xml)
    {
      try
      {
        var result = new ResponseResultV2();
        //result.CompuscanResponse = xml;
        var doc = new XmlDocument();
        doc.LoadXml(ASCIIEncoding.ASCII.GetString(xml));

        var reference = doc.SelectSingleNode("/ROOT/ConsumerEnquiry/ReturnData/NLR_ReferenceNo") ?? doc.SelectSingleNode("/ROOT/Enquiry_ID");
        result.NLREnquiryReferenceNo = reference != null ? reference.InnerText : null;

        #region Score
        var score = doc.SelectSingleNode("/ROOT/EnqCC_CompuSCORE/ROW");
        if (score != null)
        {
          result.Score = score["SCORE"].InnerText;

          if (!string.IsNullOrEmpty(score["RISK_COLOUR_R"].InnerText))
          {
            result.R = Convert.ToInt32(score["RISK_COLOUR_R"].InnerText);
          }
          if (!string.IsNullOrEmpty(score["RISK_COLOUR_G"].InnerText))
          {
            result.B = Convert.ToInt32(score["RISK_COLOUR_G"].InnerText);
          }
          if (!string.IsNullOrEmpty(score["RISK_COLOUR_B"].InnerText))
          {
            result.G = Convert.ToInt32(score["RISK_COLOUR_B"].InnerText);
          }

          result.Reasons = new List<string> { score["DECLINE_R_1"].InnerText, score["DECLINE_R_2"].InnerText, score["DECLINE_R_3"].InnerText };
          result.RiskType = score["RISK_TYPE"].InnerText;
        }
        else
        {
          result.Policies.Add(Risk.Policy.InsufficientDataForScoringPurposes);
        }

        #endregion

        #region Policies
        result.Policies = new List<Risk.Policy>();

        #region Under Administration
        var underAdministration = doc.SelectNodes("/ROOT/EnqCC_ADMINORD");
        if (underAdministration != null)
        {
          if (underAdministration.Count > 0)
          {
            result.Policies.Add(Risk.Policy.ApplicantIsUnderAdministration);
          }
        }
        #endregion

        #region Status of client
        var status = doc.SelectNodes("/ROOT/EnqCC_DMATCHES/ROW");
        foreach (XmlNode node in status)
        {
          var comparison = node["STATUS"].InnerText;
          if (comparison.ToLower() == "deceased")
          {
            result.Policies.Add(Risk.Policy.ApplicantIsDeceased);
          }
          else if (comparison.ToLower() == "unverified")
          {
            result.Policies.Add(Risk.Policy.ApplicantStatusIsNotVerified);
          }
        }
        #endregion

        #region Under debt review
        var underDebtReview = doc.SelectNodes("/ROOT/EnqCC_DEBT_RESTRUCT");
        if (underDebtReview != null && underDebtReview.Count > 0)
        {
          result.Policies.Add(Risk.Policy.ApplicantIsUnderDebtReview);
        }
        #endregion

        if (!result.Policies.Contains(Risk.Policy.OneOrMoreJudgmentsInLastTwelveMonths) && CheckJudgement(doc))
        {
          result.Policies.Add(Risk.Policy.OneOrMoreJudgmentsInLastTwelveMonths);
        }

        if (!result.Policies.Contains(Risk.Policy.OneOrMoreAdverseRecordsLastTwelveMonths) && CheckAdverse(doc))
        {
          result.Policies.Add(Risk.Policy.OneOrMoreAdverseRecordsLastTwelveMonths);
        }

        if (!result.Policies.Contains(Risk.Policy.ApplicantIsUnderSequestration) && CheckSequestration(doc))
        {
          result.Policies.Add(Risk.Policy.ApplicantIsUnderSequestration);
        }
        #endregion

        result.Accounts = new List<Account>();
        GetNLRAccounts(doc, result.Accounts, result.Policies);
        GetCPAAccounts(doc, result.Accounts, result.Policies);
        GetFPM(doc, ref result);
        result.Products = GetProducts(doc);
        result.Telephone = GetTelephones(doc);
        result.WasSucess = true;

        return result;
      }
      catch (Exception err)
      {
        log.Error(err, "ScorecardXmlDecoder.Deserialize");
        return null;
      }
    }


    /// <summary>
    /// Extracts the XML and MHT files from ZIPped byte content (zippedScorecard)
    /// </summary>
    /// <param name="log"></param>
    /// <param name="zippedScorecard"></param>
    /// <param name="file"></param>
    /// <param name="scorecard"></param>
    internal static void UnzipScorecard(ILogging log, byte[] zippedScorecard, out byte[] file, out byte[] scorecard)
    {
      var methodName = "UnzipScorecard";
      file = null;
      scorecard = null;
      var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));

      try
      {
        #region Extract the ZIP file to temp location
        using (var ms = new MemoryStream(zippedScorecard))
        {
          using (var zipFile = new ZipArchive(ms, ZipArchiveMode.Read))
          {
            zipFile.ExtractToDirectory(tempDir);
          }
        }
        #endregion

        #region Load result
        var mhtFileName = Directory.GetFiles(tempDir, "Enq_SUMM_*.MHT"); // old V1 scorecard contains multiple MHT files- Summary provides most details
        if (mhtFileName.Length == 0)
        {
          mhtFileName = Directory.GetFiles(tempDir, "*.MHT"); // New V2 scorecard only contains a single MHT file...
          if (mhtFileName.Length == 0)
          {
            throw new Exception("MHT file missing from response");
          }
        }

        file = File.ReadAllBytes(mhtFileName[0]);

        var xmlFileName = Directory.GetFiles(tempDir, "*.XML");
        if (xmlFileName.Length == 0)
        {
          throw new Exception("XML file missing from response");
        }
        scorecard = File.ReadAllBytes(xmlFileName[0]);
        #endregion
      }
      finally
      {
        try
        {
          Directory.Delete(tempDir, true);
        }
        catch (Exception err)
        {
          log.Error(err, "{MethodName}", methodName);
        }
      }
    }


    #region private methods

    /// <summary>
    /// Gets pTransaction for V2 scorecard
    /// </summary>   
    /// <returns>XML string for V2 scorecard</returns>
    private static string GetTransactionXmlV2(string gender, string IdNumber, string firstName, string surname, bool isIDPassportNo,
      DateTime dateOfBirth, string addressLine1, string addressLine2, string addressLine3, string addressLine4, string postalCode,
      string homeTelCode, string homeTelNo, string workTelCode, string workTelNo, string cellNo)
    {
      return string.Format(
        "<Transactions>" +
        "<Search_Criteria>" +
        "<CS_Data>Y</CS_Data>" +
        "<CPA_Plus_NLR_Data>Y</CPA_Plus_NLR_Data>" +
        "<Deeds_Data>N</Deeds_Data>" +
        "<Directors_Data>N</Directors_Data>" +
        "<Identity_number>{0}</Identity_number>" +
        "<Surname>{1}</Surname>" +
        "<Forename>{2}</Forename>" +
        "<Forename2></Forename2>" +
        "<Forename3></Forename3>" +
        "<Gender>{3}</Gender>" +
        "<Passport_flag>{4}</Passport_flag>" +
        "<DateOfBirth>{5:yyyyMMdd}</DateOfBirth>" +
        "<Address1>{6}</Address1>" +
        "<Address2>{7}</Address2>" +
        "<Address3>{8}</Address3>" +
        "<Address4>{9}</Address4>" +
        "<PostalCode>{10}</PostalCode>" +
        "<HomeTelCode>{11}</HomeTelCode>" +
        "<HomeTelNo>{12}</HomeTelNo>" +
        "<WorkTelCode>{13}</WorkTelCode>" +
        "<WorkTelNo>{14}</WorkTelNo>" +
        "<CellTelNo>{15}</CellTelNo>" +
        "<ResultType>XMHT</ResultType>" +
        "<RunCodix>Y</RunCodix>" +
        "<CodixParams />" +
        "<Adrs_Mandatory>N</Adrs_Mandatory>" +
        "<Enq_Purpose>11</Enq_Purpose>" + // 11- Affordability assessment
        "<Run_CompuScore>Y</Run_CompuScore>" +
        "<ClientConsent>Y</ClientConsent>" +
        "</Search_Criteria>" +
        "</Transactions>",
          HttpUtility.HtmlEncode(IdNumber), HttpUtility.HtmlEncode(surname), HttpUtility.HtmlEncode(firstName),
          HttpUtility.HtmlEncode(gender), isIDPassportNo ? "Y" : "N", dateOfBirth, HttpUtility.HtmlEncode(addressLine1),
          HttpUtility.HtmlEncode(addressLine2), HttpUtility.HtmlEncode(addressLine3), HttpUtility.HtmlEncode(addressLine4),
          HttpUtility.HtmlEncode(postalCode), HttpUtility.HtmlEncode(homeTelCode), HttpUtility.HtmlEncode(homeTelNo),
          HttpUtility.HtmlEncode(workTelCode), HttpUtility.HtmlEncode(workTelNo), HttpUtility.HtmlEncode(cellNo));
    }



    /// <summary>
    /// Check for Adverse records
    /// </summary>
    /// <param name="doc"></param>
    /// <returns>true if any adverse records found</returns>
    private static bool CheckAdverse(XmlDocument doc)
    {
      var adverses = doc.SelectNodes("/ROOT/EnqCC_ADVERSE/ROW");
      foreach (XmlNode node in adverses)
      {
        var adverseDate = string.IsNullOrEmpty(node["ADVERSE_DATE"].InnerText) ? (DateTime?)null :
                  DateTime.ParseExact(node["ADVERSE_DATE"].InnerText, "yyyy-MM-dd", null);
        var df = new DateDifference(DateTime.Now, (DateTime)adverseDate);
        if (df.Months <= 12 && df.Years == 0)
        {
          return true;
        }
      }

      adverses = doc.SelectNodes("/ROOT/BankDefaults/ROW");
      foreach (XmlNode node in adverses)
      {
        var adverseDate = string.IsNullOrEmpty(node["DATE_CREATED"].InnerText) ?
                  (DateTime?)null : DateTime.ParseExact(node["DATE_CREATED"].InnerText, "yyyy-MM-dd", null);
        if (adverseDate != null)
        {
          var df = new DateDifference(DateTime.Now, (DateTime)adverseDate);
          if (df.Months <= 12 && df.Years == 0)
          {
            return true;
          }
        }
      }

      adverses = doc.SelectNodes("/ROOT/Defaults/ROW");
      foreach (XmlNode node in adverses)
      {
        var adverseDate = string.IsNullOrEmpty(node["StatusDate"].InnerText) ? (DateTime?)null :
                  DateTime.ParseExact(node["StatusDate"].InnerText, "yyyy-MM-dd", null);
        if (adverseDate != null)
        {
          var df = new DateDifference(DateTime.Now, (DateTime)adverseDate);
          if (df.Months <= 12 && df.Years == 0)
          {
            return true;
          }
        }
      }

      return false;
    }


    /// <summary>
    /// Check for sequestration
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    private static bool CheckSequestration(XmlDocument doc)
    {
      var judgements = doc.SelectNodes("/ROOT/EnqCC_JUDGEMENTS/ROW");
      foreach (XmlNode node in judgements)
      {
        var reason = string.IsNullOrEmpty(node["REASON"].InnerText) ? string.Empty : node["REASON"].InnerText;
        //var status = string.IsNullOrEmpty(XmlJudge["STATUS"].InnerText) ? string.Empty : XmlJudge["STATUS"].InnerText;
        if (reason.ToLower() == "sequestration")
        {
          return true;
        }
      }

      judgements = doc.SelectNodes("/ROOT/ConsumerEnquiry/Judgements/ROW");
      foreach (XmlNode judge in judgements)
      {
        var reason = string.IsNullOrEmpty(judge["REASON"].InnerText) ? string.Empty : judge["REASON"].InnerText;
        //var status = string.IsNullOrEmpty(judge["STATUS"].InnerText) ? string.Empty : judge["STATUS"].InnerText;
        if (reason.ToLower() == "sequestration")
        {
          return true;
        }
      }

      return false;
    }


    /// <summary>
    /// Check for jusdgements
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    private static bool CheckJudgement(XmlDocument doc)
    {
      var judgements = doc.SelectNodes("/ROOT/ConsumerEnquiry/Judgements/ROW");
      foreach (XmlNode judge in judgements)
      {
        var judgeDate = string.IsNullOrEmpty(judge["JudgementDate"].InnerText) ? (DateTime?)null :
                  DateTime.ParseExact(judge["JudgementDate"].InnerText, "dd-MM-yyyy", null);
        if (judgeDate != null)
        {
          var df = new DateDifference(DateTime.Now, (DateTime)judgeDate);
          if (df.Months <= 12 && df.Years == 0)
          {
            return true;
          }
        }
      }

      judgements = doc.SelectNodes("/ROOT/EnqCC_JUDGEMENTS/ROW");
      // There should be only one, but lets just be safe
      foreach (XmlNode node in judgements)
      {
        var judgeDate = string.IsNullOrEmpty(node["DATE_ISSUED"].InnerText) ?
                  (DateTime?)null : DateTime.ParseExact(node["DATE_ISSUED"].InnerText, "dd-MM-yyyy", null);
        if (judgeDate != null)
        {
          var df = new DateDifference(DateTime.Now, (DateTime)judgeDate);
          if (df.Months <= 12 && df.Years == 0)
          {
            return true;
          }
        }
      }

      return false;
    }


    /// <summary>
    /// Get NLR accounts
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="allAccounts"></param>
    /// <param name="policies"></param>
    private static void GetNLRAccounts(XmlDocument doc, List<Account> allAccounts, List<Risk.Policy> policies)
    {
      // Get node with accounts
      var v1 = true;
      var nlrAccounts = doc.SelectNodes("/ROOT/ConsumerEnquiry/Accounts/Account");
      if (nlrAccounts.Count == 0)
      {
        v1 = false;
        nlrAccounts = doc.SelectNodes("/ROOT/EnqCC_NLR_ACCOUNTS/ROW");
      }

      foreach (XmlNode node in nlrAccounts)
      {
        // Set account variables
        var account = new Account()
        {
          Subscriber = node[v1 ? "Subscriber" : "SUBSCRIBER_NAME"].InnerText,
          AccountNo = node[v1 ? "AccountNo" : "ACCOUNT_NO"].InnerText,
          AccountTypeCode = node.SelectSingleNode(v1 ? "AccountType/@code" : "ACCOUNT_TYPE").InnerText,
          StatusCode = node.SelectSingleNode(v1 ? "Status/@code" : "STATUS_CODE").InnerText,
          Status = node[v1 ? "Status" : "STATUS_CODE_DESC"].InnerText,
          StatusDate = string.IsNullOrEmpty(node[v1 ? "StatusDate" : "STATUS_DATE"].InnerText) ? (DateTime?)null : Convert.ToDateTime(node[v1 ? "StatusDate" : "STATUS_DATE"].InnerText),
          OpenDate = string.IsNullOrEmpty(node[v1 ? "OpenDate" : "OPEN_DATE"].InnerText) ? (DateTime?)null : Convert.ToDateTime(node[v1 ? "OpenDate" : "OPEN_DATE"].InnerText),
          OpenBalance = string.IsNullOrEmpty(node[v1 ? "OpenBalance" : "OPEN_BAL"].InnerText.Replace("R", "")) ? "0.00" : node[v1 ? "OpenBalance" : "OPEN_BAL"].InnerText.Replace("R", ""),
          CurrentBalance = string.IsNullOrEmpty(node[v1 ? "CurrentBalance" : "CURRENT_BAL"].InnerText.Replace("R", "")) ? "0.00" : node[v1 ? "CurrentBalance" : "CURRENT_BAL"].InnerText.Replace("R", ""),
          Installment = string.IsNullOrEmpty(node[v1 ? "Installment" : "INSTALMENT_AMOUNT"].InnerText.Replace("R", "")) ? "0.00" : node[v1 ? "Installment" : "INSTALMENT_AMOUNT"].InnerText.Replace("R", ""),
          LastPaymentDate = string.IsNullOrEmpty(node[v1 ? "LastPaymentDate" : "LAST_PAYMENT_DATE"].InnerText) ? (DateTime?)null : Convert.ToDateTime(node[v1 ? "LastPaymentDate" : "LAST_PAYMENT_DATE"].InnerText),
          //                                                                    ????????????                                                          ?????????????
          BalanceDate = string.IsNullOrEmpty(node[v1 ? "BalanceDate" : "STATUS_DATE"].InnerText) ? (DateTime?)null : Convert.ToDateTime(node[v1 ? "BalanceDate" : "STATUS_DATE"].InnerText),
          OverdueAmount = string.IsNullOrEmpty(node[v1 ? "OverdueAmount" : "OVERDUE_AMOUNT"].InnerText.Replace("R", "")) ? "0.00" : node[v1 ? "OverdueAmount" : "OVERDUE_AMOUNT"].InnerText.Replace("R", ""),
        };
        if (v1)
        {
          account.LastPayment = string.IsNullOrEmpty(node["LastPayment"].InnerText.Replace("R", "")) ? "0.00" : node["LastPayment"].InnerText.Replace("R", "");
          account.SettlementDate = string.IsNullOrEmpty(node["SettlementDate"].InnerText) ? (DateTime?)null : Convert.ToDateTime(node["SettlementDate"].InnerText);
          account.RepaymentPeriodType = node["RepaymentPeriodType"].InnerText;
          account.RepaymentPeriod = node["RepaymentPeriod"].InnerText;
        }

        account.LastUpdateDate = (string.IsNullOrEmpty(node[v1 ? "BalanceDate" : "DATE_CREATED"].InnerText)) ? (DateTime?)null : Convert.ToDateTime(node[v1 ? "BalanceDate" : "DATE_CREATED"].InnerText);

        account.AccountType = Risk.BureauAccountType.NLR;

        // Add account
        allAccounts.Add(account);

        if (v1)
        {
          if (account.LastUpdateDate != null)
          {
            DateTime d1;
            DateTime d2;

            if (account.LastUpdateDate.Value > DateTime.Now)
            {
              d1 = account.LastUpdateDate.Value;
              d2 = DateTime.Now;
            }
            else
            {
              d1 = DateTime.Now;
              d2 = account.LastUpdateDate.Value;
            }
            var totalDays = (d1 - d2).TotalDays;
            // Check to ensure the payment history is not blank
            if (totalDays <= 90)
            {
              if (!policies.Contains(Risk.Policy.OneOrMoreAccountsInNinetyDaysArrears))
              {
                var paymentNode = node[v1 ? "PaymentCycle" : ""];
                if (paymentNode.HasChildNodes)
                {
                  var cycle = paymentNode[v1 ? "Summary" : ""];
                  if (cycle != null)
                  {
                    if (!string.IsNullOrEmpty(cycle.InnerText))
                    {
                      if (cycle.InnerText.Length >= 1)
                      {
                        var digiCheck = cycle.InnerText.Substring(0, 1);
                        if (Validation.IsNumeric(digiCheck))
                        {
                          if (Convert.ToInt32(digiCheck) >= 3)
                          {
                            policies.Add(Risk.Policy.OneOrMoreAccountsInNinetyDaysArrears);
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }

        if (!policies.Contains(Risk.Policy.ApplicantHasDisputeIndicator))
        {
          if (!string.IsNullOrEmpty(account.StatusCode))
          {
            if (account.StatusCode.Trim() == "30")
              policies.Add(Risk.Policy.ApplicantHasDisputeIndicator);
          }
        }
      }
    }


    /// <summary>
    /// Get CPA accounts
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="allAccounts"></param>
    /// <param name="policies"></param>
    private static void GetCPAAccounts(XmlDocument doc, List<Account> allAccounts, List<Risk.Policy> policies)
    {
      var cpaAccounts = doc.SelectNodes("/ROOT/EnqCC_CPA_ACCOUNTS/ROW");
      // There should be only one, but lets just be safe
      foreach (XmlNode node in cpaAccounts)
      {
        // Set account variables
        var account = new Account()
        {
          Subscriber = node["SUBSCRIBER_NAME"].InnerText,
          AccountNo = node["ACCOUNT_NO"].InnerText,
          SubAccountNo = node["SUB_ACCOUNT_NO"].InnerText,
          OwnershipTypeCode = string.IsNullOrEmpty(node["OWNERSHIP_TYPE"].InnerText) ? string.Empty : node["OWNERSHIP_TYPE"].InnerText,
          JoinLoanParticpants = string.IsNullOrEmpty(node["JOINT_LOAN_PARTICIPANTS"].InnerText) ? (int?)null : Convert.ToInt32(node["JOINT_LOAN_PARTICIPANTS"].InnerText),
          AccountTypeCode = node["ACCOUNT_TYPE"].InnerText,
          OpenDate = string.IsNullOrEmpty(node["OPEN_DATE"].InnerText) ? (DateTime?)null : Convert.ToDateTime(node["OPEN_DATE"].InnerText),
          LastPaymentDate = string.IsNullOrEmpty(node["LAST_PAYMENT_DATE"].InnerText) ? (DateTime?)null : Convert.ToDateTime(node["LAST_PAYMENT_DATE"].InnerText),
          OpenBalance = string.IsNullOrEmpty(node["OPEN_BAL"].InnerText.Replace("R", "")) ? "0.00" : node["OPEN_BAL"].InnerText.Replace("R", ""),
          CurrentBalance = string.IsNullOrEmpty(node["CURRENT_BAL"].InnerText.Replace("R", "")) ? "0.00" : node["CURRENT_BAL"].InnerText.Replace("R", ""),
          OverdueAmount = string.IsNullOrEmpty(node["OVERDUE_AMOUNT"].InnerText.Replace("R", "")) ? "0.00" : node["OVERDUE_AMOUNT"].InnerText.Replace("R", ""),
          Installment = string.IsNullOrEmpty(node["INSTALMENT_AMOUNT"].InnerText.Replace("R", "")) ? "0.00" : node["INSTALMENT_AMOUNT"].InnerText.Replace("R", ""),
          RepaymentPeriodType = node["REPAYMENT_FREQ_DESC"].InnerText,
          RepaymentPeriod = node["TERMS"].InnerText,
          StatusCode = node["STATUS_CODE"].InnerText,
          Status = node["STATUS_CODE_DESC"].InnerText,
          StatusDate = string.IsNullOrEmpty(node["STATUS_DATE"].InnerText) ? (DateTime?)null : Convert.ToDateTime(node["STATUS_DATE"].InnerText),
          AccountType = Risk.BureauAccountType.CPA,
          LastUpdateDate = string.IsNullOrEmpty(node["MONTH_END_DATE"].InnerText) ? (DateTime?)null : Convert.ToDateTime(node["MONTH_END_DATE"].InnerText),
          LastPayment = "0.00" // Hard coded because XML doesnt support it
        };

        // Add account
        allAccounts.Add(account);

        if (!policies.Contains(Risk.Policy.OneOrMoreAccountsInNinetyDaysArrears))
        {
          if (account.LastUpdateDate != null)
          {
            DateTime d1;
            DateTime d2;

            if (account.LastUpdateDate.Value > DateTime.Now)
            {
              d1 = account.LastUpdateDate.Value;
              d2 = DateTime.Now;
            }
            else
            {
              d1 = DateTime.Now;
              d2 = account.LastUpdateDate.Value;
            }
            var totalDays = (d1 - d2).TotalDays;
            // Check to ensure the payment history is not blank
            if (totalDays <= 90)
            {
              var paymentHistory = string.IsNullOrEmpty(node["PAYMENT_HISTORY"].InnerText) ? string.Empty : node["PAYMENT_HISTORY"].InnerText;
              if (!string.IsNullOrEmpty(paymentHistory))
              {
                if (paymentHistory.Length >= 1)
                {
                  var digitCheck = paymentHistory.Substring(0, 1);
                  if (Validation.IsNumeric(digitCheck))
                  {
                    if (Convert.ToInt32(digitCheck) >= 3)
                    {
                      policies.Add(Risk.Policy.OneOrMoreAccountsInNinetyDaysArrears);
                    }
                  }
                }
              }
            }
          }

          // Ensure that if the dispute was set to true, make sure it does not revert state
          if (!policies.Contains(Risk.Policy.ApplicantHasDisputeIndicator))
          {
            if (!string.IsNullOrEmpty(account.StatusCode))
            {
              if (account.StatusCode.Trim() == "D")
                policies.Add(Risk.Policy.ApplicantHasDisputeIndicator);
            }
          }
        }
      }
    }


    /// <summary>
    /// Get telephones
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    private static List<Telephone> GetTelephones(XmlDocument doc)
    {
      var result = new List<Telephone>();
      var telephoneNumbers = doc.SelectNodes("/ROOT/EnqCC_TELEPHONE/ROW");

      foreach (XmlNode telNode in telephoneNumbers)
      {
        var type = string.IsNullOrEmpty(telNode["TEL_NUMBER_TYPE"].InnerText) ? string.Empty : telNode["TEL_NUMBER_TYPE"].InnerText;
        var number = string.IsNullOrEmpty(telNode["TEL_NUMBER"].InnerText) ? string.Empty : telNode["TEL_NUMBER"].InnerText;

        DateTime? telephoneDate = null;
        try
        {
          telephoneDate = string.IsNullOrEmpty(telNode["TEL_DATE_CREATED"].InnerText) ? (DateTime?)null : DateTime.ParseExact(telNode["TEL_DATE_CREATED"].InnerText, "dd-MM-yyyy", null);
        }
        catch (Exception)
        {
          try
          {
            telephoneDate = string.IsNullOrEmpty(telNode["TEL_DATE_CREATED"].InnerText) ? (DateTime?)null : DateTime.ParseExact(telNode["TEL_DATE_CREATED"].InnerText, "dd-MM-yyyy", null);
          }
          catch (Exception)
          {
            telephoneDate = (DateTime?)null;
          }
        }
        result.Add(new Telephone { Number = number, Type = type, CreateDate = telephoneDate });
      }

      telephoneNumbers = doc.SelectNodes("/ROOT/ConsumerEnquiry/Telephones/Telephone");
      foreach (XmlNode telNode in telephoneNumbers)
      {
        var prefix = string.IsNullOrEmpty(telNode["PhonePrefix"].InnerText) ? string.Empty : telNode["PhonePrefix"].InnerText;
        var number = string.IsNullOrEmpty(telNode["PhoneNumber"].InnerText) ? string.Empty : telNode["PhoneNumber"].InnerText;
        var type = string.IsNullOrEmpty(telNode["PhoneType"].InnerText) ? string.Empty : telNode["PhoneType"].InnerText;
        DateTime? telephoneDate = null;
        try
        {
          telephoneDate = string.IsNullOrEmpty(telNode["FirstReportedDate"].InnerText) ?
            (DateTime?)null : DateTime.ParseExact(telNode["FirstReportedDate"].InnerText, "dd MMM yyyy", null);
        }
        catch (Exception)
        {
          telephoneDate = (DateTime?)null;
        }

        var combined = string.Format("{0}{1}", !string.IsNullOrEmpty(prefix) ? prefix : string.Empty, number);
        result.Add(new Telephone { Number = combined, Type = type, CreateDate = telephoneDate });
      }

      return result;
    }


    /// <summary>
    /// Get products
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
    private static List<Product> GetProducts(XmlDocument doc)
    {
      var result = new List<Product>();
      var products = doc.SelectNodes("/ROOT/ATLAS_CODIX/PRODUCTS/PRODUCT");

      foreach (XmlNode productDetail in products)
      {
        var product = new Product()
        {
          Description = productDetail["product_description"].InnerText,
          Outcome = productDetail["outcome"].InnerText
        };

        var reasonNodeCollection = productDetail.SelectNodes("reasons/reason");
        if (reasonNodeCollection != null && reasonNodeCollection.Count > 0)
        {
          product.Reasons = new List<Reason>();

          foreach (XmlNode reason in reasonNodeCollection)
          {
            var reasonNodeItemCollection = reason.SelectNodes("reason");
            foreach (XmlNode reasonItem in reasonNodeItemCollection)
            {
              product.Reasons.Add(new Reason() { Description = reasonItem.InnerText });
            }
          }
        }
        result.Add(product);
      }

      return result;
    }


    /// <summary>
    /// Get fraud prevention module
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="result"></param>
    private static void GetFPM(XmlDocument doc, ref ResponseResultV2 result)
    {
      result.FPM = new List<FPM>();
      var fraud = doc.SelectNodes("/ROOT/EnqCC_Fraud");
      if (fraud != null && fraud.Count > 0)
      {
        foreach (XmlNode fraudItem in fraud)
        {
          var summaryNodeCollection = fraudItem.SelectNodes("EnqCC_Fraud_SUMM");
          if (summaryNodeCollection.Count > 0)
          {
            FPM fpm = null;
            foreach (XmlNode summaryNode in summaryNodeCollection)
            {
              foreach (XmlNode summaryItem in summaryNode.SelectNodes("ROW"))
              {
                fpm = new FPM();
                fpm.SubjectNo = string.IsNullOrEmpty(summaryItem["SUBJECTNO"].InnerText) ? string.Empty : summaryItem["SUBJECTNO"].InnerText;
                fpm.Category = string.IsNullOrEmpty(summaryItem["CATEGORY"].InnerText) ? string.Empty : summaryItem["CATEGORY"].InnerText;
                fpm.CategoryNo = string.IsNullOrEmpty(summaryItem["CAT_NUM"].InnerText) ? string.Empty : summaryItem["CAT_NUM"].InnerText;
                fpm.SubCategory = string.IsNullOrEmpty(summaryItem["SUBCAT"].InnerText) ? string.Empty : summaryItem["SUBCAT"].InnerText;
                fpm.Subject = string.IsNullOrEmpty(summaryItem["SUBJECT"].InnerText) ? string.Empty : summaryItem["SUBJECT"].InnerText;
                fpm.Passport = string.IsNullOrEmpty(summaryItem["PASSPORT"].InnerText) ? string.Empty : summaryItem["PASSPORT"].InnerText;
                fpm.IncidentDate = string.IsNullOrEmpty(summaryItem["INCIDENTDATE"].InnerText) ? string.Empty : summaryItem["INCIDENTDATE"].InnerText;
                fpm.Victim = string.IsNullOrEmpty(summaryItem["VICTIM"].InnerText) ? false : summaryItem["VICTIM"].InnerText == "NO" ? false : true;

                result.FPM.Add(fpm);
              }
            }
          }

          var fraudDetail = fraudItem.SelectNodes("Detail");
          if (fraudDetail != null)
          {
            if (fraudDetail.Count > 0)
            {
              foreach (XmlNode detail in fraudDetail)
              {
                var personalDetailCollection = detail.SelectNodes("EnqCC_Fraud_PersonalDetail");

                if (personalDetailCollection != null)
                {
                  result.FPMPersonalDetails = new List<FPM_PersonalDetail>();
                  foreach (XmlNode pd in personalDetailCollection)
                  {
                    foreach (XmlNode personDetail in pd.SelectNodes("ROW"))
                    {
                      var personal = new FPM_PersonalDetail()
                      {
                        Title = string.IsNullOrEmpty(personDetail["FRAUD_PERSONAL_TITLE"].InnerText) ? string.Empty : personDetail["FRAUD_PERSONAL_TITLE"].InnerText,
                        Surname = string.IsNullOrEmpty(personDetail["FRAUD_PERSONAL_SURNAME"].InnerText) ? string.Empty : personDetail["FRAUD_PERSONAL_SURNAME"].InnerText,
                        Firstname = string.IsNullOrEmpty(personDetail["FRAUD_PERSONAL_FIRSTNAME"].InnerText) ? string.Empty : personDetail["FRAUD_PERSONAL_FIRSTNAME"].InnerText,
                        ID = string.IsNullOrEmpty(personDetail["FRAUD_PERSONAL_ID"].InnerText) ? string.Empty : personDetail["FRAUD_PERSONAL_ID"].InnerText,
                        Passport = string.IsNullOrEmpty(personDetail["FRAUD_PERSONAL_PASSPORT"].InnerText) ? string.Empty : personDetail["FRAUD_PERSONAL_PASSPORT"].InnerText,
                        DateOfBirth = string.IsNullOrEmpty(personDetail["FRAUD_PERSONAL_DATEOFBIRTH"].InnerText) ? string.Empty : personDetail["FRAUD_PERSONAL_DATEOFBIRTH"].InnerText,
                        Gender = string.IsNullOrEmpty(personDetail["FRAUD_PERSONAL_GENDER"].InnerText) ? string.Empty : personDetail["FRAUD_PERSONAL_GENDER"].InnerText,
                        Email = string.IsNullOrEmpty(personDetail["FRAUD_PERSONAL_EMAIL"].InnerText) ? string.Empty : personDetail["FRAUD_PERSONAL_EMAIL"].InnerText
                      };

                      result.FPMPersonalDetails.Add(personal);
                    }
                  }
                }

                var incidentDetailCollection = detail.SelectNodes("EnqCC_Fraud_IncidentDetail");
                if (incidentDetailCollection != null)
                {
                  result.FPMIncidentDetails = new List<FPM_IncidentDetail>();
                  foreach (XmlNode id in incidentDetailCollection)
                  {
                    foreach (XmlNode incidentDetail in id.SelectNodes("ROW"))
                    {
                      var incident = new FPM_IncidentDetail()
                      {
                        Victim = string.IsNullOrEmpty(incidentDetail["FRAUD_INCIDENT_VICTIM"].InnerText) ? false : incidentDetail["FRAUD_INCIDENT_VICTIM"].InnerText == "NO" ? false : true,
                        MembersReference = string.IsNullOrEmpty(incidentDetail["FRAUD_INCIDENT_MEMBERSREF"].InnerText) ? string.Empty : incidentDetail["FRAUD_INCIDENT_MEMBERSREF"].InnerText,
                        Category = string.IsNullOrEmpty(incidentDetail["FRAUD_INCIDENT_CATEGORY"].InnerText) ? string.Empty : incidentDetail["FRAUD_INCIDENT_CATEGORY"].InnerText,
                        SubCategory = string.IsNullOrEmpty(incidentDetail["FRAUD_INCIDENT_SUBCAT"].InnerText) ? string.Empty : incidentDetail["FRAUD_INCIDENT_SUBCAT"].InnerText,
                        IncidentDate = string.IsNullOrEmpty(incidentDetail["FRAUD_INCIDENT_DATE"].InnerText) ? string.Empty : incidentDetail["FRAUD_INCIDENT_DATE"].InnerText,
                        SubRole = string.IsNullOrEmpty(incidentDetail["FRAUD_INCIDENT_SUBROLE"].InnerText) ? string.Empty : incidentDetail["FRAUD_INCIDENT_SUBROLE"].InnerText,
                        City = string.IsNullOrEmpty(incidentDetail["FRAUD_INCIDENT_CITY"].InnerText) ? string.Empty : incidentDetail["FRAUD_INCIDENT_CITY"].InnerText,
                        Detail = string.IsNullOrEmpty(incidentDetail["FRAUD_INCIDENT_DETAIL"].InnerText) ? string.Empty : incidentDetail["FRAUD_INCIDENT_DETAIL"].InnerText,
                        Forensic = string.IsNullOrEmpty(incidentDetail["FRAUD_INCIDENT_FORENSIC"].InnerText) ? string.Empty : incidentDetail["FRAUD_INCIDENT_FORENSIC"].InnerText
                      };

                      result.FPMIncidentDetails.Add(incident);
                    }
                  }
                }

                var addressDetailsCollection = detail.SelectNodes("EnqCC_Fraud_AddressDetail");
                if (addressDetailsCollection != null)
                {
                  result.FPMAddressDetails = new List<FPM_AddressDetail>();

                  foreach (XmlNode ad in addressDetailsCollection)
                  {
                    foreach (XmlNode addressDetail in ad.SelectNodes("ROW"))
                    {
                      var address = new FPM_AddressDetail()
                      {
                        Type = string.IsNullOrEmpty(addressDetail["FRAUD_ADDRESSES_TYPE"].InnerText) ? string.Empty : addressDetail["FRAUD_ADDRESSES_TYPE"].InnerText,
                        Street = string.IsNullOrEmpty(addressDetail["FRAUD_ADDRESSES_STREET"].InnerText) ? string.Empty : addressDetail["FRAUD_ADDRESSES_STREET"].InnerText,
                        Address = string.IsNullOrEmpty(addressDetail["FRAUD_ADDRESSES_ADDRESS"].InnerText) ? string.Empty : addressDetail["FRAUD_ADDRESSES_ADDRESS"].InnerText,
                        City = string.IsNullOrEmpty(addressDetail["FRAUD_ADDRESSES_CITY"].InnerText) ? string.Empty : addressDetail["FRAUD_ADDRESSES_CITY"].InnerText,
                        PostalCode = string.IsNullOrEmpty(addressDetail["FRAUD_ADDRESSES_PCODE"].InnerText) ? string.Empty : addressDetail["FRAUD_ADDRESSES_PCODE"].InnerText,
                        From = string.IsNullOrEmpty(addressDetail["FRAUD_ADDRESSES_FROM"].InnerText) ? string.Empty : addressDetail["FRAUD_ADDRESSES_FROM"].InnerText,
                        To = string.IsNullOrEmpty(addressDetail["FRAUD_ADDRESSES_TO"].InnerText) ? string.Empty : addressDetail["FRAUD_ADDRESSES_TO"].InnerText
                      };

                      result.FPMAddressDetails.Add(address);
                    }
                  }
                }

                var telephoneDetailsCollection = detail.SelectNodes("EnqCC_Fraud_TelDetail");

                if (telephoneDetailsCollection != null)
                {
                  result.FPMTelephoneDetails = new List<FPM_TelephoneDetail>();
                  foreach (XmlNode td in telephoneDetailsCollection)
                  {
                    foreach (XmlNode telephoneDetail in td.SelectNodes("ROW"))
                    {
                      var telephone = new FPM_TelephoneDetail()
                      {
                        Type = string.IsNullOrEmpty(telephoneDetail["FRAUD_OTHERTEL_TYPE"].InnerText) ? string.Empty : telephoneDetail["FRAUD_OTHERTEL_TYPE"].InnerText,
                        No = string.IsNullOrEmpty(telephoneDetail["FRAUD_OTHERTEL_NUMBER"].InnerText) ? string.Empty : telephoneDetail["FRAUD_OTHERTEL_NUMBER"].InnerText,
                        City = string.IsNullOrEmpty(telephoneDetail["FRAUD_OTHERTEL_CITY"].InnerText) ? string.Empty : telephoneDetail["FRAUD_OTHERTEL_CITY"].InnerText,
                        Country = string.IsNullOrEmpty(telephoneDetail["FRAUD_OTHERTEL_COUNTRY"].InnerText) ? string.Empty : telephoneDetail["FRAUD_OTHERTEL_COUNTRY"].InnerText
                      };

                      result.FPMTelephoneDetails.Add(telephone);
                    }
                  }
                }

                var aliasDetailsCollection = detail.SelectNodes("EnqCC_Fraud_AliasDetail");
                if (aliasDetailsCollection != null)
                {
                  result.FPMAliasDetails = new List<FPM_AliasDetail>();

                  foreach (XmlNode adc in aliasDetailsCollection)
                  {
                    foreach (XmlNode aliasDetail in adc.SelectNodes("ROW"))
                    {
                      var alias = new FPM_AliasDetail()
                      {
                        FirstName = string.IsNullOrEmpty(aliasDetail["FRAUD_ALIAS_FIRSTNAME"].InnerText) ? string.Empty : aliasDetail["FRAUD_ALIAS_FIRSTNAME"].InnerText,
                        Surname = string.IsNullOrEmpty(aliasDetail["FRAUD_ALIAS_SURNAME"].InnerText) ? string.Empty : aliasDetail["FRAUD_ALIAS_SURNAME"].InnerText
                      };

                      result.FPMAliasDetails.Add(alias);
                    }
                  }
                }

                var otherIdDetailsCollection = detail.SelectNodes("EnqCC_Fraud_OtherIDDetail");
                if (otherIdDetailsCollection != null)
                {
                  result.FPMOtherIdDetails = new List<FPM_OtherIdDetail>();

                  foreach (XmlNode oid in otherIdDetailsCollection)
                  {
                    foreach (XmlNode otherIdDetail in oid.SelectNodes("ROW"))
                    {
                      var other = new FPM_OtherIdDetail()
                      {
                        IDNo = string.IsNullOrEmpty(otherIdDetail["FRAUD_OTHERID_ID"].InnerText) ? string.Empty : otherIdDetail["FRAUD_OTHERID_ID"].InnerText,
                        Type = string.IsNullOrEmpty(otherIdDetail["FRAUD_OTHERID_TYPE"].InnerText) ? string.Empty : otherIdDetail["FRAUD_OTHERID_TYPE"].InnerText,
                        IssueDate = string.IsNullOrEmpty(otherIdDetail["FRAUD_OTHERID_ISSUEDATE"].InnerText) ? string.Empty : otherIdDetail["FRAUD_OTHERID_ISSUEDATE"].InnerText,
                        Country = string.IsNullOrEmpty(otherIdDetail["FRAUD_OTHERID_COUNTRY"].InnerText) ? string.Empty : otherIdDetail["FRAUD_OTHERID_COUNTRY"].InnerText
                      };

                      result.FPMOtherIdDetails.Add(other);
                    }
                  }
                }

                var employerDetailCollection = detail.SelectNodes("EnqCC_Fraud_EmpDetail");
                if (employerDetailCollection != null)
                {
                  result.FPMEmploymentDetails = new List<FPM_EmploymentDetail>();

                  foreach (XmlNode ed in employerDetailCollection)
                  {
                    foreach (XmlNode employerDetail in ed.SelectNodes("ROW"))
                    {
                      var employer = new FPM_EmploymentDetail()
                      {
                        Name = string.IsNullOrEmpty(employerDetail["FRAUD_EMPLOYER_NAME"].InnerText) ? string.Empty : employerDetail["FRAUD_EMPLOYER_NAME"].InnerText,
                        Telephone = string.IsNullOrEmpty(employerDetail["FRAUD_EMPLOYER_TEL"].InnerText) ? string.Empty : employerDetail["FRAUD_EMPLOYER_TEL"].InnerText,
                        RegisteredName = string.IsNullOrEmpty(employerDetail["FRAUD_EMPLOYER_COMPANYNAME"].InnerText) ? string.Empty : employerDetail["FRAUD_EMPLOYER_COMPANYNAME"].InnerText,
                        CompanyNo = string.IsNullOrEmpty(employerDetail["FRAUD_EMPLOYER_COMPANYNO"].InnerText) ? string.Empty : employerDetail["FRAUD_EMPLOYER_COMPANYNO"].InnerText,
                        Occupation = string.IsNullOrEmpty(employerDetail["FRAUD_EMPLOYER_OCCUPATION"].InnerText) ? string.Empty : employerDetail["FRAUD_EMPLOYER_OCCUPATION"].InnerText,
                        From = string.IsNullOrEmpty(employerDetail["FRAUD_EMPLOYER_DATEFROM"].InnerText) ? string.Empty : employerDetail["FRAUD_EMPLOYER_DATEFROM"].InnerText,
                        To = string.IsNullOrEmpty(employerDetail["FRAUD_EMPLOYER_DATETO"].InnerText) ? string.Empty : employerDetail["FRAUD_EMPLOYER_DATETO"].InnerText
                      };

                      result.FPMEmploymentDetails.Add(employer);
                    }
                  }
                }

                var bankDetailsCollection = detail.SelectNodes("EnqCC_Fraud_BankDetail");

                if (bankDetailsCollection != null)
                {
                  result.FPMBankDetails = new List<FPM_BankDetail>();
                  foreach (XmlNode bd in bankDetailsCollection)
                  {
                    foreach (XmlNode bankDetail in bd.SelectNodes("ROW"))
                    {
                      var bank = new FPM_BankDetail()
                      {
                        AccountNo = string.IsNullOrEmpty(bankDetail["FRAUD_BANK_NO"].InnerText) ? string.Empty : bankDetail["FRAUD_BANK_NO"].InnerText,
                        AccountType = string.IsNullOrEmpty(bankDetail["FRAUD_BANK_TYPE"].InnerText) ? string.Empty : bankDetail["FRAUD_BANK_TYPE"].InnerText,
                        Bank = string.IsNullOrEmpty(bankDetail["FRAUD_BANK_BANK"].InnerText) ? string.Empty : bankDetail["FRAUD_BANK_BANK"].InnerText,
                        From = string.IsNullOrEmpty(bankDetail["FRAUD_BANK_DATEFROM"].InnerText) ? string.Empty : bankDetail["FRAUD_BANK_DATEFROM"].InnerText,
                        To = string.IsNullOrEmpty(bankDetail["FRAUD_BANK_DATETO"].InnerText) ? string.Empty : bankDetail["FRAUD_BANK_DATETO"].InnerText
                      };

                      result.FPMBankDetails.Add(bank);
                    }
                  }
                }

                var caseDetailsCollection = detail.SelectNodes("EnqCC_Fraud_CaseDetail");
                if (caseDetailsCollection != null)
                {
                  result.FPMCaseDetails = new List<FPM_CaseDetail>();
                  foreach (XmlNode cd in caseDetailsCollection)
                  {
                    foreach (XmlNode caseDetail in cd.SelectNodes("ROW"))
                    {
                      var fpmCaseDetail = new FPM_CaseDetail()
                      {
                        CaseNo = string.IsNullOrEmpty(caseDetail["FRAUD_CASE_NO"].InnerText) ? string.Empty : caseDetail["FRAUD_CASE_NO"].InnerText,
                        ReportDate = string.IsNullOrEmpty(caseDetail["FRAUD_CASE_REPORTDATE"].InnerText) ? string.Empty : caseDetail["FRAUD_CASE_REPORTDATE"].InnerText,
                        Officer = string.IsNullOrEmpty(caseDetail["FRAUD_CASE_OFFICER"].InnerText) ? string.Empty : caseDetail["FRAUD_CASE_OFFICER"].InnerText,
                        CreatedBy = string.IsNullOrEmpty(caseDetail["FRAUD_CASE_CREATEDBY"].InnerText) ? string.Empty : caseDetail["FRAUD_CASE_CREATEDBY"].InnerText,
                        Station = string.IsNullOrEmpty(caseDetail["FRAUD_CASE_STATION"].InnerText) ? string.Empty : caseDetail["FRAUD_CASE_STATION"].InnerText,
                        Type = string.IsNullOrEmpty(caseDetail["FRAUD_CASE_TYPE"].InnerText) ? string.Empty : caseDetail["FRAUD_CASE_TYPE"].InnerText,
                        Status = string.IsNullOrEmpty(caseDetail["FRAUD_CASE_STATUS"].InnerText) ? string.Empty : caseDetail["FRAUD_CASE_STATUS"].InnerText,
                        Reason = string.IsNullOrEmpty(caseDetail["FRAUD_CASE_REASON"].InnerText) ? string.Empty : caseDetail["FRAUD_CASE_REASON"].InnerText,
                        ReasonExtension = string.IsNullOrEmpty(caseDetail["FRAUD_CASE_REASONEXTENSION"].InnerText) ? string.Empty : caseDetail["FRAUD_CASE_REASONEXTENSION"].InnerText,
                        ContactNo = string.IsNullOrEmpty(caseDetail["FRAUD_CASE_CONTACTNO"].InnerText) ? string.Empty : caseDetail["FRAUD_CASE_CONTACTNO"].InnerText,
                        Email = string.IsNullOrEmpty(caseDetail["FRAUD_CASE_EMAIL"].InnerText) ? string.Empty : caseDetail["FRAUD_CASE_EMAIL"].InnerText,
                        Fax = string.IsNullOrEmpty(caseDetail["FRAUD_CASE_FAX"].InnerText) ? string.Empty : caseDetail["FRAUD_CASE_FAX"].InnerText,
                        Details = string.IsNullOrEmpty(caseDetail["FRAUD_CASE_DETAILS"].InnerText) ? string.Empty : caseDetail["FRAUD_CASE_DETAILS"].InnerText
                      };

                      result.FPMCaseDetails.Add(fpmCaseDetail);
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    #endregion



    private readonly static Dictionary<int, string> _scoreErrors = new Dictionary<int, string>() 
    { { -301, "Invalid Username or Password" }, 
      { -302, "Parameter Error" }, 
      { -303, "Error getting Enquiry IDs" }, 
      { -304, "Could not find properties for Search Servlet" }, 
      { -305, "Error zipping and encoding file" }, 
      { -307, "Found no results, contact Compuscan" }, 
      { -308, "Web session not valid" }, 
      { -310, "Problems inserting Enquiry" }, 
      { -311, "Error looking for Country Code / Branch Code" }, 
      { -312, "Error combining results" },
      { -313, "Error starting search threads" }, 
      { -314, "Error retrieving IDs from threads" }, 
      { -315, "No more Experian Consumer Enquiry Credits left. Please contact Compuscan" }, 
      { -316, "Problems looking for Experian Credits" }, 
      { -317, "Experian link is down, please try again later. Compuscan CreditCheck searches only for now" }, 
      { -318, "Trail CreditCheck branch has no more credits" }, 
      { -319, "Client not registered as an Experian client" }, 
      { -320, "Search Name on Normal NLR Search longer than 15 characters" }, 
      { -321, "Search Surname on Normal NLR Search longer than 25 characters" },
      { -322, "Gender incorrect only M or F" }, 
      { -323, "Passport flag can only take YES or NO" },
      { -324, "Parameter is longer than it should be" }, 
      { -325, "Date of Birth not valid" }, 
      { -326, "Error on Experian side: Could not get accounts" }, 
      { -327, "Error on Experian side: System error during get report" }, 
      { -328, "Experian Server returned HTTP response code: 502" }, 
      { -329, "Experian Server is refusing the connection" }, 
      { -330, "Search Name can not be longer than 15 characters for NLR Searches" },
      { -331, "Search Surname can not be longer than 25 characters for NLR Searches" },
      { -403, "Username or Password incorrect" }, { -520, "XML Malformed" }, 
      { -521, "Problem parsing transaction" }, { -524, "DOB must be 8 characters" },
      { -525, "DOB not a valid date" }, { -528, "Experian line down" }, 
      { -529, "Not Experian registered" }, { -530, "No more Consumer Credits" }, 
      { -532, "Not CC enabled" },
      { -535, "Error on Experian side: Could not get accounts" },
      { -536, "Error on Experian side: System error during get report" },
      { -537, "Experian Server returned HTTP response code: 502" }, 
      { -538, "Experian Server is refusing the connection" }, 
      { -539, "NCA minimum data (enquiry) requirements not met" }, 
      { -540, "Unknown error occurred while performing the search" },
      { -541, "Problems inserting Enquiry" }, 
      { -542, "Error building results" }, 
      { -543, "Problems zipping and encoding results" }, 
      { -544, "Returned result from Experian not in correct format" },
      { -545, "Error on Experian side: Could not get session ID" },
      { -546, "Experian Processing Error" },
      { -550, "Error getting Enquiry IDs" }, 
      { -551, "Error generating results" }, 
      { -565, "AddressLine 2 must be populated" }, 
      { -580, "Cannot run Codix as your branch is not switched on for Codix" }, 
      { -589, "CelTelNo cannot be bigger than 16 digits" },
      { -999, "Unknown error" } };
  }
}
