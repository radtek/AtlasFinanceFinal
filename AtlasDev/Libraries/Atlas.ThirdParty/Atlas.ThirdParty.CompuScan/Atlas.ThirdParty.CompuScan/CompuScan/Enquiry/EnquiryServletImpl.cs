using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Net;

using Atlas.Enumerators;
using Atlas.Common.Extensions;
using Atlas.Common.Utils;


namespace Atlas.ThirdParty.CompuScan.Enquiry
{
  public sealed class EnquiryServletImpl
  {
    #region Private Properties

    private Enumerators.Risk.CreditCheckDestination _url { get; set; }

    #region Core Servlet Parameters

    private string pTransType { get; set; }
    private string pUsrnme { get; set; }
    private string pPasswrd { get; set; }
    private string pDLL_Version { get; set; }
    private string pMyOrigin { get; set; }
    private string pTransaction { get; set; }

    #endregion

    #region Xml Builder Parameters

    public string CC_enquiry { get; set; }
    public string CCPlusCPA_enquiry { get; set; }
    public string NLR_enquiry { get; set; }
    public string CON_enquiry { get; set; }
    public string Identity_number { get; set; }
    public string Surname { get; set; }
    public string Forename { get; set; }
    public string Forename2 { get; set; }
    public string Forename3 { get; set; }
    public string Gender { get; set; }
    public string Passport_flag { get; set; }
    public string DateOfBirth { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string Address3 { get; set; }
    public string Address4 { get; set; }
    public string PostalCode { get; set; }
    public string HomeTelCode { get; set; }
    public string HomeTelNo { get; set; }
    public string WorkTelCode { get; set; }
    public string WorkTelNo { get; set; }
    public string CellTelNo { get; set; }
    public Enumerators.Risk.ResponseFormat ResultType { get; set; }
    public string RunCodix { get; set; }
    public string CodixParams { get; set; }
    public string Adrs_Mandatory { get; set; }
    public string Enq_Purpose { get; set; }
    public string Run_CompuScore { get; set; }

    #endregion

    #endregion

    #region Error Codes

    internal Dictionary<int, string>
       _errorDict = new Dictionary<int, string>(){
          {-301, "Invalid Username or Password"},
          {-302, "Parameter Error"},
          {-303, "Error getting Enquiry IDs"},
          {-304, "Could not find properties for Search Servlet"},
          {-305, "Error zipping and encoding file"},
          {-307, "Found no results, contact Compuscan"},
          {-308, "Web session not valid"},
          {-310, "Problems inserting Enquiry"},
          {-311, "Error looking for Country Code / Branch Code"},
          {-312, "Error combining results"},
          {-313, "Error starting search threads"},
          {-314, "Error retrieving IDs from threads"},
          {-315, "No more Experian Consumer Enquiry Credits left. Please contact Compuscan"},
          {-316, "Problems looking for Experian Credits"},
          {-317, "Experian link is down, please try again later. Compuscan CreditCheck searches only for now"},
          {-318, "Trail CreditCheck branch has no more credits"},
          {-319, "Client not registered as an Experian client"},
          {-320, "Search Name on Normal NLR Search longer than 15 characters"},
          {-321, "Search Surname on Normal NLR Search longer than 25 characters"},
          {-322, "Gender incorrect only M or F"},
          {-323, "Passport flag can only take YES or NO"},
          {-324, "Parameter is longer than it should be"},
          {-325, "Date of Birth not valid"},
          {-326, "Error on Experian side: Could not get accounts"},
          {-327, "Error on Experian side: System error during get report"},
          {-328, "Experian Server returned HTTP response code: 502"},
          {-329, "Experian Server is refusing the connection"},
          {-330, "Search Name can not be longer than 15 characters for NLR Searches"},
          {-331, "Search Surname can not be longer than 25 characters for NLR Searches"},
          {-403, "Username or Password incorrect" },
          {-520, "XML Malformed"},
					{-565, "AddressLine 2 must be populated"},
          {-525, "DOB not a valid date" },
          {-524, "DOB must be 8 characters"},
          {-589, "CelTelNo cannot be bigger than 16 digits"}
       };
    #endregion

    #region Private Methods

    /// <summary>
    /// Perform the request against the compuscan servers
    /// </summary>
    /// <returns>ICompuScanResult</returns>
    public ResponseResultV2 DoRequest(Risk.CreditCheckDestination destination, Request doRequest, Int64? transNo, string  userName, string passWord)
    {
      #region Authentication Parameters

      this.pUsrnme = userName;
      this.pPasswrd = passWord;
      
      /*this.pUsrnme = "1949-1";
      this.pPasswrd = "ATLAS1";*/
      
      #endregion

      #region Setup Parameters

      var request = ((Request)doRequest);
      this.CC_enquiry = request.CCEnquiry;
      this.NLR_enquiry = request.NLREnquiry;
      this.CCPlusCPA_enquiry = request.CCPlusCPAEnquiry;
      this.CON_enquiry = "N";
      this.Identity_number = request.IdentityNo;
      this.Surname = request.Surname;
      this.Forename = request.Firstname;
      this.Gender = request.Gender.ToStringEnum();
      this.Passport_flag = request.IsIDPassportNo;
      this.DateOfBirth = request.DateOfBirth.ToString("yyyyMMdd");
      this.Address1 = request.AddressLine1;
      this.Address2 = request.AddressLine2;
      this.Address3 = request.Suburb;
      this.Address4 = request.City;
      this.PostalCode = request.PostalCode;
      this.HomeTelCode = request.HomeTelCode;
      this.HomeTelNo = request.HomeTelNo;
      this.WorkTelCode = request.WorkTelCode;
      this.WorkTelNo = request.WorkTelNo;
      this.CellTelNo = request.CellTelNo;
      this.Adrs_Mandatory = request.AddressMandatory;
      this.ResultType = request.ResponseFormat;
      this._url = destination;
      this.Enq_Purpose = ((int)request.EnquiryPurpose).ToString();
      this.Run_CompuScore = request.RunCompuScore;
      this.RunCodix = request.RunCodix;

      #endregion

      // Get all the properties of the current class
      var propertyInfos =
          this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

      // Run through the properties ensuring we only add ones that are populated
      // also ignore _url
      var requestDict = propertyInfos.Where(propertyInfo => propertyInfo.CanRead && propertyInfo.GetValue(this, null) != null && propertyInfo.Name != "_url" &&
              propertyInfo.Name.StartsWith("p")).ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => Convert.ToBase64String(
              System.Text.ASCIIEncoding.ASCII.GetBytes(propertyInfo.GetValue(this, null).ToString())));

      requestDict.Add("pTransType",
                      Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("Normal_Search")));
      requestDict.Add("pDLL_Version", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("Direct01")));
      requestDict.Add("pMyOrigin", Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes("AtlasFinance")));

      // New instance of the poster class 
			var post = new Http { Url = this._url.ToStringEnum(), TimeOut = 200000 };

      requestDict.Add("pTransaction", GenerateXML(propertyInfos));

      // Add the required post variables into the poster
      requestDict.ToList().ForEach(i => post.PostItems.Add(i.Key, i.Value));

      // We know we are posting so set num
      post.Type = Http.PostTypeEnum.Post;

      // Do the actual post against the url
			string postResult = string.Empty;
			try
			{
				postResult = post.Post();
			}
			catch (WebException ex)
			{
				throw ex;
			}

      // Check to ensure we did not receive any of the possible error codes
      ResponseResultV2 result = new ResponseResultV2();

      if (postResult.Trim().Length <= 5)
      {
        result.WasSucess = false;
        if (string.IsNullOrEmpty(postResult.Trim()))
        {
          result.Error = "Unknown";
          result.ErrorDescription = "Unknown compuscan error occurred";
        }
        else
        {

          Console.WriteLine(postResult.Trim());
          result.Error = postResult.Trim();
          result.ErrorDescription = this._errorDict[System.Convert.ToInt32(postResult.Trim())];
        }
        return result;
      }

      result.WasSucess = true;
      result.Files = new List<string>();

      // if we did not receive error codes we can then write to disk
      WriteToDisk(doRequest, postResult, transNo, ref result);

      // returning the results
      return result;
    }

    #endregion


    #region XML Generation

    private string GenerateXML(PropertyInfo[] propertyInfos)
    {
      var xmlStub = new XElement("Transactions");
      var requestDict = propertyInfos.Where(propertyInfo => propertyInfo.Name != "_url" && !propertyInfo.Name.StartsWith("p"))
              .ToDictionary(propertyInfo => propertyInfo.Name, propertyInfo => propertyInfo.GetValue(this, null) ?? string.Empty);

      var searchCriteria = new List<XElement>();

      requestDict.ToList().ForEach(i => searchCriteria.Add(new XElement(i.Key, i.Value)));
      xmlStub.Add(new XElement("Search_Criteria", searchCriteria.ToArray()));

      return Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(xmlStub.ToString()));
    }

    #endregion


    #region Zip Writing

    /// <summary>
    /// Write the file to disc after decoding it
    /// </summary>
    /// <param name="encodedData"></param>
    internal void WriteToDisk(Request doRequest, string encodedData, Int64? transNo, ref ResponseResultV2 result)
    {
      // Decode the string from base64 into byte array
      byte[] encodedDataAsBytes = System.Convert.FromBase64String(encodedData);

      result.CompuscanResponse = encodedDataAsBytes;

      // create new guid, and just replace the -
      string folderName = string.Empty;

      if (transNo == null)
        folderName = Guid.NewGuid().ToString().Replace("-", "");
      else
        folderName = transNo.ToString();

      // Create zip storage path if required
      if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ZipStorage")))
        Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ZipStorage"));

      // Write all bytes to disk with guid as file name
      string zipFile = string.Format("{0}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"ZipStorage\{0}.zip", Guid.NewGuid().ToString())));

      using (var fs = new FileStream(zipFile, FileMode.Create, FileAccess.Write))
      {
        // Writes a block of bytes to this stream using data from a byte array.
        fs.Write(encodedDataAsBytes, 0, encodedDataAsBytes.Length);
        fs.Flush();
        // close file stream
        fs.Close();
      }

      // Combine the applications base path and zip extraction
      string extractionDirectory = string.Empty;

      string combinedZipPath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "Temp");

      // Check to see if it exists, else create
      if (!Directory.Exists(combinedZipPath))
        Directory.CreateDirectory(combinedZipPath);

      // Create a path for the received file with its own name
      string zipExtractionPath = Path.Combine(combinedZipPath, string.Format(@"{0}\{1}\{2}\{3}\", DateTime.Now.Year.ToString(), DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"), folderName));

      // Remove the directory if it exists, this could happen if the loan failed for some reason and they are re-attempting an enquiry.
      if (Directory.Exists(zipExtractionPath))
        Directory.Delete(zipExtractionPath, true);

      Directory.CreateDirectory(zipExtractionPath);

      // Unzip the file to specified path
      using (var zip = new Ionic.Zip.ZipFile(zipFile))
      {
        zip.ExtractAll(zipExtractionPath);
      }

      // Remove Zip file
      //File.Delete(zipFile);

      // Add the files we just extracted to the response object
      var di = new DirectoryInfo(zipExtractionPath);
      string summaryFile = string.Empty;

      foreach (FileInfo fi in di.GetFiles("*.*", SearchOption.TopDirectoryOnly))
      {
        int underscoreIndex = fi.Name.LastIndexOf("_");
        string fileName = String.Format("{0}{1}", fi.Name.Substring(0, underscoreIndex), fi.Extension);
        File.Copy(fi.FullName, string.Format(@"{0}\{1}", di.FullName, fileName));
        File.Delete(fi.FullName);
        result.Files.Add(string.Format(@"{0}\{1}", di.FullName, fileName));
        if (string.Format(@"{0}\{1}", di.FullName, fileName).Contains("SUMM"))
        {
          summaryFile = string.Format(@"{0}\{1}", di.FullName, fileName);
        }
      }

      // XPDF only!
      if (doRequest.ResponseFormat == Enumerators.Risk.ResponseFormat.XPDF)
      {
        XmlDocument XmlDocument = new XmlDocument();
        XmlDocument.Load(string.Format("{0}{1}", di.FullName, string.Format("Enq_{0}.xml", doRequest.IdentityNo)));

        var ccPDFXML = XmlDocument.SelectNodes("ROOT/CC_PDF_RESULTS");
        if (ccPDFXML.Count == 1)
        {
          byte[] ccPdfBytes = System.Convert.FromBase64String(ccPDFXML[0].InnerXml);
          File.WriteAllBytes(string.Format("{0}{1}", di.FullName, string.Format("Enq_CC_{0}.pdf", doRequest.IdentityNo)), ccPdfBytes);
        }

        var nlrPDFXML = XmlDocument.SelectNodes("ROOT/NLR_PDF_RESULTS");
        if (nlrPDFXML.Count == 1)
        {
          byte[] nlrPdfBytes = System.Convert.FromBase64String(nlrPDFXML[0].InnerXml);
          File.WriteAllBytes(string.Format("{0}{1}", di.FullName, string.Format("Enq_NLR_{0}.pdf", doRequest.IdentityNo)), nlrPdfBytes);
        }

        var summPDFXML = XmlDocument.SelectNodes("ROOT/SUMM_PDF_RESULTS");
        if (summPDFXML.Count == 1)
        {
          byte[] summPdfBytes = System.Convert.FromBase64String(summPDFXML[0].InnerXml);
          File.WriteAllBytes(string.Format("{0}{1}", di.FullName, string.Format("Enq_SUMM_{0}.pdf", doRequest.IdentityNo)), summPdfBytes);
        }
      }

      // Only fix image space if in HTML or XHML format.
      if (doRequest.ResponseFormat == Enumerators.Risk.ResponseFormat.HTML || doRequest.ResponseFormat == Enumerators.Risk.ResponseFormat.XHML)
      {
        if (!string.IsNullOrEmpty(summaryFile))
        {
          // Read all string contents into memory
          string fileContents = File.ReadAllText(summaryFile);

          // Replace /images with ./images
          fileContents = fileContents.Replace("/images/", "./images/");

          // Write file to disk
          using (var sw = new StreamWriter(summaryFile, false))
          {
            sw.Write(fileContents);
          }
          // Clear contents of string, most likely not required
          fileContents = null;
        }
      }

      // Read summary file response
      if (!string.IsNullOrEmpty(summaryFile))
        result.SummaryFile = File.ReadAllBytes(summaryFile);


      // Build Response structures
      BuildResponse(zipExtractionPath, ref result);

      // Finally remove the files.
      //Directory.Delete(zipExtractionPath, true);

      // Remove Zip

      //File.Delete(zipFile);

    }

    #endregion


    #region Response Builder

    /// <summary>
    /// Build response structure
    /// </summary>
    /// <param name="extractionPath"></param>
    /// <param name="file"></param>
    public void BuildResponse(string extractionPath, ref ResponseResultV2 result)
    {
      List<string> files = result.Files.Where(x => x.EndsWith(".xml") == true).ToList();
      string fileName;

      if (files.Count > 0)
      {
        fileName = files.FirstOrDefault();
        XmlDocument Document = new XmlDocument();
        Document.LoadXml(File.ReadAllText(Path.Combine(extractionPath, fileName)));

        result.OriginalResponse = Document;
        result.Policies = new List<Atlas.Enumerators.Risk.Policy>();

        #region Under Administration

        var underAdministration = Document.SelectNodes("ROOT/EnqCC_ADMINORD");

        if (underAdministration != null)
        {
          if (underAdministration.Count > 0)
            result.Policies.Add(Atlas.Enumerators.Risk.Policy.ApplicantIsUnderAdministration);

        }

        #endregion

        #region Status of client

        var Status = Document.SelectNodes("ROOT/EnqCC_DMATCHES");

        if (Status != null)
        {
          if (Status.Count > 0)
          {
            foreach (var nodes in Status[0].SelectNodes("ROW"))
            {
              // There should only be 1
              var Xml = nodes as XmlNode;

              var comparison = Xml.SelectSingleNode("STATUS").InnerText;
              if (comparison.ToLower() == "deceased")
                result.Policies.Add(Atlas.Enumerators.Risk.Policy.ApplicantIsDeceased);
              else if (comparison.ToLower() == "unverified")
                result.Policies.Add(Atlas.Enumerators.Risk.Policy.ApplicantStatusIsNotVerified);
            }
          }
        }

        #endregion

        #region Under Debt Review


        var underDebtReview = Document.SelectNodes("ROOT/EnqCC_DEBT_RESTRUCT");
        if (underDebtReview != null)
        {
          if (underDebtReview.Count > 0)
            result.Policies.Add(Atlas.Enumerators.Risk.Policy.ApplicantIsUnderDebtReview);
        }

        #endregion

        #region Telephone Numbers

        var telephoneNumbers = Document.SelectNodes("ROOT/EnqCC_TELEPHONE");
        result.Telephone = new List<Telephone>();
        foreach (var node in telephoneNumbers)
        {
          var Xml = node as XmlNode;
          if (Xml.HasChildNodes)
          {
            foreach (var tel in telephoneNumbers[0].SelectNodes("ROW"))
            {
              var telNode = tel as XmlNode;

              var type = string.IsNullOrEmpty(telNode.SelectSingleNode("TEL_NUMBER_TYPE").InnerText) ? string.Empty : telNode.SelectSingleNode("TEL_NUMBER_TYPE").InnerText;
              var number = string.IsNullOrEmpty(telNode.SelectSingleNode("TEL_NUMBER").InnerText) ? string.Empty : telNode.SelectSingleNode("TEL_NUMBER").InnerText;

              DateTime? telephoneDate = null;
              try
              {
                telephoneDate = string.IsNullOrEmpty(telNode.SelectSingleNode("TEL_DATE_CREATED").InnerText) ? (DateTime?)null : DateTime.ParseExact(telNode.SelectSingleNode("TEL_DATE_CREATED").InnerText, "dd-MM-yyyy", null);
              }
              catch (Exception)
              {
                try
                {
                  telephoneDate = string.IsNullOrEmpty(telNode.SelectSingleNode("TEL_DATE_CREATED").InnerText) ? (DateTime?)null : DateTime.ParseExact(telNode.SelectSingleNode("TEL_DATE_CREATED").InnerText, "dd-MM-yyyy", null);
                }
                catch (Exception)
                {
                  telephoneDate = (DateTime?)null;
                }
              }
              result.Telephone.Add(new Telephone { Number = number, Type = type, CreateDate = telephoneDate });
            }
          }
        }

        telephoneNumbers = Document.SelectNodes("ROOT/ConsumerEnquiry/Telephones");

        foreach (var node in telephoneNumbers)
        {
          var Xml = node as XmlNode;
          if (Xml.HasChildNodes)
          {
            foreach (var tel in telephoneNumbers[0])
            {
              var telNode = tel as XmlNode;

              var prefix = string.IsNullOrEmpty(telNode.SelectSingleNode("PhonePrefix").InnerText) ? string.Empty : telNode.SelectSingleNode("PhonePrefix").InnerText;
              var number = string.IsNullOrEmpty(telNode.SelectSingleNode("PhoneNumber").InnerText) ? string.Empty : telNode.SelectSingleNode("PhoneNumber").InnerText;
              var type = string.IsNullOrEmpty(telNode.SelectSingleNode("PhoneType").InnerText) ? string.Empty : telNode.SelectSingleNode("PhoneType").InnerText;
              DateTime? telephoneDate = null;
              try
              {
                telephoneDate = string.IsNullOrEmpty(telNode.SelectSingleNode("FirstReportedDate").InnerText) ? (DateTime?)null : DateTime.ParseExact(telNode.SelectSingleNode("FirstReportedDate").InnerText, "dd MMM yyyy", null);
              }
              catch (Exception)
              {
                telephoneDate = (DateTime?)null;
              }
              string combined = string.Empty;

              if (!string.IsNullOrEmpty(prefix))
                combined = string.Format("{0}{1}", prefix, number);
              else
                combined = number;

              result.Telephone.Add(new Telephone { Number = combined, Type = type, CreateDate = telephoneDate });
            }
          }
        }
        #endregion

        #region Score

        // Get Score
        var Score = Document.SelectNodes("ROOT/EnqCC_CompuSCORE");

        if (Score.Count > 0)
        {
          if (Score != null)
          {
            foreach (var nodes in Score[0].SelectNodes("ROW"))
            {
              // There should only be 1
              var Xml = nodes as XmlNode;

              result.Score = Xml.SelectSingleNode("SCORE").InnerText;


              if (result.Reasons == null)
                result.Reasons = new List<string>();

              if (!string.IsNullOrEmpty(Xml.SelectSingleNode("RISK_COLOUR_R").InnerText))
                result.R = Convert.ToInt32(Xml.SelectSingleNode("RISK_COLOUR_R").InnerText);

              if (!string.IsNullOrEmpty(Xml.SelectSingleNode("RISK_COLOUR_G").InnerText))
                result.B = Convert.ToInt32(Xml.SelectSingleNode("RISK_COLOUR_G").InnerText);

              if (!string.IsNullOrEmpty(Xml.SelectSingleNode("RISK_COLOUR_B").InnerText))
                result.G = Convert.ToInt32(Xml.SelectSingleNode("RISK_COLOUR_B").InnerText);

              result.Reasons.Add(Xml.SelectSingleNode("DECLINE_R_1").InnerText);
              result.Reasons.Add(Xml.SelectSingleNode("DECLINE_R_2").InnerText);
              result.Reasons.Add(Xml.SelectSingleNode("DECLINE_R_3").InnerText);

              result.RiskType = Xml.SelectSingleNode("RISK_TYPE").InnerText;
            }
          }
        }
        else
        {
          result.Policies.Add(Atlas.Enumerators.Risk.Policy.InsufficientDataForScoringPurposes);
        }

        #endregion

        #region NLR Accounts

        result.NLREnquiryReferenceNo = Document.SelectNodes("ROOT/ConsumerEnquiry/ReturnData/NLR_ReferenceNo")[0].InnerText;

        // Initialize the list
        result.Accounts = new List<Account>();

        // Get node with accounts
        var AccountList = Document.SelectNodes("ROOT/ConsumerEnquiry/Accounts");
        if (AccountList != null)
        {
          if (AccountList.Count != 0)
          {
            // There should be only one, but lets just be safe
            foreach (var node in AccountList[0].SelectNodes("Account"))
            {
              var Xml = node as XmlNode;
              // Set account variables
              var Account = new Account();
              Account.Subscriber = Xml.SelectSingleNode("Subscriber").InnerText;
              Account.AccountNo = Xml.SelectSingleNode("AccountNo").InnerText;
              Account.AccountTypeCode = Xml.SelectSingleNode("AccountType/@code").InnerText;
              Account.StatusCode = Xml.SelectSingleNode("Status/@code").InnerText;
              Account.Status = Xml.SelectSingleNode("Status").InnerText;
              Account.StatusDate = string.IsNullOrEmpty(Xml.SelectSingleNode("StatusDate").InnerText) ? (DateTime?)null : Convert.ToDateTime(Xml.SelectSingleNode("StatusDate").InnerText);
              Account.OpenDate = string.IsNullOrEmpty(Xml.SelectSingleNode("OpenDate").InnerText) ? (DateTime?)null : Convert.ToDateTime(Xml.SelectSingleNode("OpenDate").InnerText);
              Account.OpenBalance = string.IsNullOrEmpty(Xml.SelectSingleNode("OpenBalance").InnerText.Replace("R", "")) ? "0.00" : Xml.SelectSingleNode("OpenBalance").InnerText.Replace("R", "");
              Account.Subscriber = Xml.SelectSingleNode("Subscriber").InnerText;
              Account.CurrentBalance = string.IsNullOrEmpty(Xml.SelectSingleNode("CurrentBalance").InnerText.Replace("R", "")) ? "0.00" : Xml.SelectSingleNode("CurrentBalance").InnerText.Replace("R", "");
              Account.BalanceDate = string.IsNullOrEmpty(Xml.SelectSingleNode("BalanceDate").InnerText) ? (DateTime?)null : Convert.ToDateTime(Xml.SelectSingleNode("BalanceDate").InnerText);
              Account.LastPayment = string.IsNullOrEmpty(Xml.SelectSingleNode("LastPayment").InnerText.Replace("R", "")) ? "0.00" : Xml.SelectSingleNode("LastPayment").InnerText.Replace("R", "");
              Account.LastPaymentDate = string.IsNullOrEmpty(Xml.SelectSingleNode("LastPaymentDate").InnerText) ? (DateTime?)null : Convert.ToDateTime(Xml.SelectSingleNode("LastPaymentDate").InnerText);
              Account.Installment = string.IsNullOrEmpty(Xml.SelectSingleNode("Installment").InnerText.Replace("R", "")) ? "0.00" : Xml.SelectSingleNode("Installment").InnerText.Replace("R", "");
              Account.OverdueAmount = string.IsNullOrEmpty(Xml.SelectSingleNode("OverdueAmount").InnerText.Replace("R", "")) ? "0.00" : Xml.SelectSingleNode("OverdueAmount").InnerText.Replace("R", "");
              Account.SettlementDate = string.IsNullOrEmpty(Xml.SelectSingleNode("SettlementDate").InnerText) ? (DateTime?)null : Convert.ToDateTime(Xml.SelectSingleNode("SettlementDate").InnerText);
              Account.RepaymentPeriodType = Xml.SelectSingleNode("RepaymentPeriodType").InnerText;
              Account.RepaymentPeriod = Xml.SelectSingleNode("RepaymentPeriod").InnerText;
              Account.AccountType = Atlas.Enumerators.Risk.BureauAccountType.NLR;

              if (string.IsNullOrEmpty(Xml.SelectSingleNode("BalanceDate").InnerText))
                Account.LastUpdateDate = (DateTime?)null;
              else
                Account.LastUpdateDate = Convert.ToDateTime(Xml.SelectSingleNode("BalanceDate").InnerText);
              // Add account
              result.Accounts.Add(Account);

              if (Account.LastUpdateDate != null)
              {
                DateTime d1;
                DateTime d2;

                if (Account.LastUpdateDate.Value > DateTime.Now)
                {
                  d1 = Account.LastUpdateDate.Value;
                  d2 = DateTime.Now;
                }
                else
                {
                  d1 = DateTime.Now;
                  d2 = Account.LastUpdateDate.Value;
                }
                var totalDays = (d1 - d2).TotalDays;
                // Check to ensure the payment history is not blank
                if (totalDays <= 90)
                {
                  if (!result.Policies.Contains(Atlas.Enumerators.Risk.Policy.OneOrMoreAccountsInNinetyDaysArrears))
                  {
                    var paymentNode = Xml.SelectSingleNode("PaymentCycle");
                    if (paymentNode.HasChildNodes)
                    {
                      var cycle = paymentNode.SelectSingleNode("Summary");
                      if (cycle != null)
                      {
                        if (!string.IsNullOrEmpty(cycle.InnerText))
                        {
                          if (cycle.InnerText.Length >= 1)
                          {
                            string digiCheck = cycle.InnerText.Substring(0, 1);
                            if (Validation.IsNumeric(digiCheck))
                            {
                              if (Convert.ToInt32(digiCheck) >= 3)
                              {
                                result.Policies.Add(Atlas.Enumerators.Risk.Policy.OneOrMoreAccountsInNinetyDaysArrears);
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }

              if (!result.Policies.Contains(Atlas.Enumerators.Risk.Policy.ApplicantHasDisputeIndicator))
              {
                if (!string.IsNullOrEmpty(Account.StatusCode))
                {
                  if (Account.StatusCode.Trim() == "30")
                    result.Policies.Add(Atlas.Enumerators.Risk.Policy.ApplicantHasDisputeIndicator);
                }
              }
            }
          }
        }

        #endregion

        #region Judgements

        // Check judgments
        var JudgementsList = Document.SelectNodes("ROOT/ConsumerEnquiry/Judgements");
        if (JudgementsList != null)
        {
          if (JudgementsList.Count != 0)
          {
            // There should be only one, but lets just be safe
            foreach (var node in JudgementsList)
            {
              var Xml = node as XmlNode;
              if (Xml.HasChildNodes)
              {
                foreach (var judge in JudgementsList[0].SelectNodes("ROW"))
                {
                  var XmlJudge = judge as XmlNode;
                  var reason = string.IsNullOrEmpty(XmlJudge.SelectSingleNode("REASON").InnerText) ? string.Empty : XmlJudge.SelectSingleNode("REASON").InnerText;
                  //var status = string.IsNullOrEmpty(XmlJudge.SelectSingleNode("STATUS").InnerText) ? string.Empty : XmlJudge.SelectSingleNode("STATUS").InnerText;
                  if (reason.ToLower() == "sequestration")
                  {
                    result.Policies.Add(Atlas.Enumerators.Risk.Policy.ApplicantIsUnderSequestration);
                  }


                  DateTime? judgeDate = string.IsNullOrEmpty(Xml.SelectSingleNode("JudgementDate").InnerText) ? (DateTime?)null : DateTime.ParseExact(Xml.SelectSingleNode("JudgementDate").InnerText, "dd-MM-yyyy", null);
                  if (judgeDate != null)
                  {
                    if (!result.Policies.Contains(Atlas.Enumerators.Risk.Policy.OneOrMoreJudgmentsInLastTwelveMonths))
                    {
                      DateDifference df = new DateDifference(DateTime.Now, (DateTime)judgeDate);
                      if (df.Months <= 12 && df.Years == 0)
                        result.Policies.Add(Atlas.Enumerators.Risk.Policy.OneOrMoreJudgmentsInLastTwelveMonths);
                    }
                  }
                }
              }
            }
          }
        }

        #endregion

        #region CPA Accounts

        var CPAAccountList = Document.SelectNodes("ROOT/EnqCC_CPA_ACCOUNTS");
        if (CPAAccountList != null)
        {
          if (CPAAccountList.Count != 0)
          {
            // There should be only one, but lets just be safe
            foreach (var node in CPAAccountList[0].SelectNodes("ROW"))
            {
              var Xml = node as XmlNode;
              // Set account variables
              var Account = new Account();
              Account.Subscriber = Xml.SelectSingleNode("SUBSCRIBER_NAME").InnerText;
              Account.AccountNo = Xml.SelectSingleNode("ACCOUNT_NO").InnerText;
              Account.SubAccountNo = Xml.SelectSingleNode("SUB_ACCOUNT_NO").InnerText;
              Account.OwnershipTypeCode = string.IsNullOrEmpty(Xml.SelectSingleNode("OWNERSHIP_TYPE").InnerText) ? string.Empty : Xml.SelectSingleNode("OWNERSHIP_TYPE").InnerText;
              Account.JoinLoanParticpants = string.IsNullOrEmpty(Xml.SelectSingleNode("JOINT_LOAN_PARTICIPANTS").InnerText) ? (int?)null : Convert.ToInt32(Xml.SelectSingleNode("JOINT_LOAN_PARTICIPANTS").InnerText);
              Account.AccountTypeCode = Xml.SelectSingleNode("ACCOUNT_TYPE").InnerText;
              Account.OpenDate = string.IsNullOrEmpty(Xml.SelectSingleNode("OPEN_DATE").InnerText) ? (DateTime?)null : Convert.ToDateTime(Xml.SelectSingleNode("OPEN_DATE").InnerText);
              Account.LastPaymentDate = string.IsNullOrEmpty(Xml.SelectSingleNode("LAST_PAYMENT_DATE").InnerText) ? (DateTime?)null : Convert.ToDateTime(Xml.SelectSingleNode("LAST_PAYMENT_DATE").InnerText);
              Account.OpenBalance = string.IsNullOrEmpty(Xml.SelectSingleNode("OPEN_BAL").InnerText.Replace("R", "")) ? "0.00" : Xml.SelectSingleNode("OPEN_BAL").InnerText.Replace("R", "");
              Account.CurrentBalance = string.IsNullOrEmpty(Xml.SelectSingleNode("CURRENT_BAL").InnerText.Replace("R", "")) ? "0.00" : Xml.SelectSingleNode("CURRENT_BAL").InnerText.Replace("R", "");
              Account.OverdueAmount = string.IsNullOrEmpty(Xml.SelectSingleNode("OVERDUE_AMOUNT").InnerText.Replace("R", "")) ? "0.00" : Xml.SelectSingleNode("OVERDUE_AMOUNT").InnerText.Replace("R", "");
              Account.Installment = string.IsNullOrEmpty(Xml.SelectSingleNode("INSTALMENT_AMOUNT").InnerText.Replace("R", "")) ? "0.00" : Xml.SelectSingleNode("INSTALMENT_AMOUNT").InnerText.Replace("R", "");
              Account.RepaymentPeriodType = Xml.SelectSingleNode("REPAYMENT_FREQ_DESC").InnerText;
              Account.RepaymentPeriod = Xml.SelectSingleNode("TERMS").InnerText;
              Account.StatusCode = Xml.SelectSingleNode("STATUS_CODE").InnerText;
              Account.Status = Xml.SelectSingleNode("STATUS_CODE_DESC").InnerText;
              Account.StatusDate = string.IsNullOrEmpty(Xml.SelectSingleNode("STATUS_DATE").InnerText) ? (DateTime?)null : Convert.ToDateTime(Xml.SelectSingleNode("STATUS_DATE").InnerText);
              Account.AccountType = Atlas.Enumerators.Risk.BureauAccountType.CPA;

              if (string.IsNullOrEmpty(Xml.SelectSingleNode("MONTH_END_DATE").InnerText))
                Account.LastUpdateDate = (DateTime?)null;
              else
                Account.LastUpdateDate = Convert.ToDateTime(Xml.SelectSingleNode("MONTH_END_DATE").InnerText);

              Account.LastPayment = "0.00"; // Hard coded because XML doesnt support it
              // Add account
              result.Accounts.Add(Account);

              if (Account.LastUpdateDate != null)
              {
                DateTime d1;
                DateTime d2;

                if (Account.LastUpdateDate.Value > DateTime.Now)
                {
                  d1 = Account.LastUpdateDate.Value;
                  d2 = DateTime.Now;
                }
                else
                {
                  d1 = DateTime.Now;
                  d2 = Account.LastUpdateDate.Value;
                }
                var totalDays = (d1 - d2).TotalDays;
                // Check to ensure the payment history is not blank
                if (totalDays <= 90)
                {
                  if (!result.Policies.Contains(Atlas.Enumerators.Risk.Policy.OneOrMoreAccountsInNinetyDaysArrears))
                  {
                    string paymentHistory = string.IsNullOrEmpty(Xml.SelectSingleNode("PAYMENT_HISTORY").InnerText) ? string.Empty : Xml.SelectSingleNode("PAYMENT_HISTORY").InnerText;
                    if (!string.IsNullOrEmpty(paymentHistory))
                    {
                      if (paymentHistory.Length >= 1)
                      {
                        string digitCheck = paymentHistory.Substring(0, 1);
                        if (Validation.IsNumeric(digitCheck))
                        {
                          if (Convert.ToInt32(digitCheck) >= 3)
                          {
                            result.Policies.Add(Atlas.Enumerators.Risk.Policy.OneOrMoreAccountsInNinetyDaysArrears);
                          }
                        }
                      }
                    }
                  }
                }
              }

              // Ensure that if the dispute was set to true, make sure it does not revert state
              if (!result.Policies.Contains(Atlas.Enumerators.Risk.Policy.ApplicantHasDisputeIndicator))
              {
                if (!string.IsNullOrEmpty(Account.StatusCode))
                {
                  if (Account.StatusCode.Trim() == "D")
                    result.Policies.Add(Atlas.Enumerators.Risk.Policy.ApplicantHasDisputeIndicator);
                }
              }
            }
          }
        }

        #endregion

        #region Judgements

        if (!result.Policies.Contains(Atlas.Enumerators.Risk.Policy.OneOrMoreJudgmentsInLastTwelveMonths))
        {
          // Check judgments
          JudgementsList = Document.SelectNodes("ROOT/EnqCC_JUDGEMENTS");
          if (JudgementsList != null)
          {
            if (JudgementsList.Count != 0)
            {
              // There should be only one, but lets just be safe
              foreach (var node in JudgementsList)
              {
                var Xml = node as XmlNode;
                if (Xml.HasChildNodes)
                {
                  foreach (var judge in JudgementsList[0].SelectNodes("ROW"))
                  {
                    var XmlJudge = judge as XmlNode;
                    var reason = string.IsNullOrEmpty(XmlJudge.SelectSingleNode("REASON").InnerText) ? string.Empty : XmlJudge.SelectSingleNode("REASON").InnerText;
                    //var status = string.IsNullOrEmpty(XmlJudge.SelectSingleNode("STATUS").InnerText) ? string.Empty : XmlJudge.SelectSingleNode("STATUS").InnerText;
                    if (reason.ToLower() == "sequestration")
                    {
                      result.Policies.Add(Atlas.Enumerators.Risk.Policy.ApplicantIsUnderSequestration);
                    }

                    DateTime? judgeDate = string.IsNullOrEmpty(XmlJudge.SelectSingleNode("DATE_ISSUED").InnerText) ? (DateTime?)null : DateTime.ParseExact(XmlJudge.SelectSingleNode("DATE_ISSUED").InnerText, "dd-MM-yyyy", null);
                    if (judgeDate != null)
                    {
                      if (!result.Policies.Contains(Atlas.Enumerators.Risk.Policy.OneOrMoreJudgmentsInLastTwelveMonths))
                      {
                        DateDifference df = new DateDifference(DateTime.Now, (DateTime)judgeDate);
                        if (df.Months <= 12 && df.Years == 0)
                          result.Policies.Add(Atlas.Enumerators.Risk.Policy.OneOrMoreJudgmentsInLastTwelveMonths);
                      }
                    }
                  }
                }
              }
            }
          }
        }

        #endregion

        #region Adverse

        XmlNodeList AdveresesList = null;
        if (result.Policies.Contains(Atlas.Enumerators.Risk.Policy.OneOrMoreAdverseRecordsLastTwelveMonths))
        {
          AdveresesList = Document.SelectNodes("ROOT/EnqCC_ADVERSE");
          if (AdveresesList != null)
          {
            if (AdveresesList.Count != 0)
            {
              // There should be only one, but lets just be safe
              foreach (var node in AdveresesList)
              {
                var Xml = node as XmlNode;
                if (Xml.HasChildNodes)
                {
                  var isRowFormat = AdveresesList[0].SelectNodes("ROW");
                  if (isRowFormat != null)
                  {
                    if (isRowFormat.Count > 0)
                    {
                      foreach (var adverse in AdveresesList[0].SelectNodes("ROW"))
                      {
                        var adverseXML = adverse as XmlNode;
                        DateTime? adverseDate = string.IsNullOrEmpty(adverseXML.SelectSingleNode("ADVERSE_DATE").InnerText) ? (DateTime?)null : DateTime.ParseExact(adverseXML.SelectSingleNode("ADVERSE_DATE").InnerText, "yyyy-MM-dd", null);
                        if (adverseDate != null)
                        {
                          if (result.Policies.Contains(Atlas.Enumerators.Risk.Policy.OneOrMoreAdverseRecordsLastTwelveMonths))
                          {
                            DateDifference df = new DateDifference(DateTime.Now, (DateTime)adverseDate);
                            if (df.Months <= 12 && df.Years == 0)
                              result.Policies.Add(Atlas.Enumerators.Risk.Policy.OneOrMoreAdverseRecordsLastTwelveMonths);
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


        AdveresesList = Document.SelectNodes("ROOT/BankDefaults");
        if (AdveresesList != null)
        {
          if (AdveresesList.Count != 0)
          {
            // There should be only one, but lets just be safe
            foreach (var node in AdveresesList)
            {
              var isRowFormat = AdveresesList[0].SelectNodes("ROW");
              if (isRowFormat != null)
              {
                if (isRowFormat.Count > 0)
                {
                  foreach (var adverse in AdveresesList[0].SelectNodes("ROW"))
                  {
                    var Xml = node as XmlNode;
                    DateTime? adverseDate = string.IsNullOrEmpty(Xml.SelectSingleNode("DATE_CREATED").InnerText) ? (DateTime?)null : DateTime.ParseExact(Xml.SelectSingleNode("DATE_CREATED").InnerText, "yyyy-MM-dd", null);
                    if (adverseDate != null)
                    {
                      if (!result.Policies.Contains(Atlas.Enumerators.Risk.Policy.OneOrMoreAdverseRecordsLastTwelveMonths))
                      {
                        DateDifference df = new DateDifference(DateTime.Now, (DateTime)adverseDate);
                        if (df.Months <= 12 && df.Years == 0)
                          result.Policies.Add(Atlas.Enumerators.Risk.Policy.OneOrMoreAdverseRecordsLastTwelveMonths);
                      }
                    }
                  }
                }
              }
            }
          }
        }

        AdveresesList = Document.SelectNodes("ROOT/Defaults");
        if (AdveresesList != null)
        {
          if (AdveresesList.Count != 0)
          {
            // There should be only one, but lets just be safe
            foreach (var node in AdveresesList)
            {
              var isRowFormat = AdveresesList[0].SelectNodes("ROW");
              if (isRowFormat != null)
              {
                if (isRowFormat.Count > 0)
                {
                  foreach (var adverse in AdveresesList[0].SelectNodes("ROW"))
                  {
                    var Xml = node as XmlNode;
                    DateTime? adverseDate = string.IsNullOrEmpty(Xml.SelectSingleNode("StatusDate").InnerText) ? (DateTime?)null : DateTime.ParseExact(Xml.SelectSingleNode("StatusDate").InnerText, "yyyy-MM-dd", null);
                    if (adverseDate != null)
                    {
                      if (!result.Policies.Contains(Atlas.Enumerators.Risk.Policy.OneOrMoreAdverseRecordsLastTwelveMonths))
                      {
                        DateDifference df = new DateDifference(DateTime.Now, (DateTime)adverseDate);
                        if (df.Months <= 12 && df.Years == 0)
                          result.Policies.Add(Atlas.Enumerators.Risk.Policy.OneOrMoreAdverseRecordsLastTwelveMonths);
                      }
                    }
                  }
                }
              }
            }
          }
        }
        #endregion

        #region Fraud

        XmlNodeList fraud = null;

        result.FPM = new List<FPM>();

        fraud = Document.SelectNodes("ROOT/EnqCC_Fraud");

        if (fraud != null)
        {
          if (fraud.Count > 0)
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

                    fpm.SubjectNo = string.IsNullOrEmpty(summaryItem.SelectSingleNode("SUBJECTNO").InnerText) ? string.Empty : summaryItem.SelectSingleNode("SUBJECTNO").InnerText;
                    fpm.Category = string.IsNullOrEmpty(summaryItem.SelectSingleNode("CATEGORY").InnerText) ? string.Empty : summaryItem.SelectSingleNode("CATEGORY").InnerText;
                    fpm.CategoryNo = string.IsNullOrEmpty(summaryItem.SelectSingleNode("CAT_NUM").InnerText) ? string.Empty : summaryItem.SelectSingleNode("CAT_NUM").InnerText;
                    fpm.SubCategory = string.IsNullOrEmpty(summaryItem.SelectSingleNode("SUBCAT").InnerText) ? string.Empty : summaryItem.SelectSingleNode("SUBCAT").InnerText;
                    fpm.Subject = string.IsNullOrEmpty(summaryItem.SelectSingleNode("SUBJECT").InnerText) ? string.Empty : summaryItem.SelectSingleNode("SUBJECT").InnerText;
                    fpm.Passport = string.IsNullOrEmpty(summaryItem.SelectSingleNode("PASSPORT").InnerText) ? string.Empty : summaryItem.SelectSingleNode("PASSPORT").InnerText;
                    fpm.IncidentDate = string.IsNullOrEmpty(summaryItem.SelectSingleNode("INCIDENTDATE").InnerText) ? string.Empty : summaryItem.SelectSingleNode("INCIDENTDATE").InnerText;
                    fpm.Victim = string.IsNullOrEmpty(summaryItem.SelectSingleNode("VICTIM").InnerText) ? false : summaryItem.SelectSingleNode("VICTIM").InnerText == "NO" ? false : true;

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
                          FPM_PersonalDetail personal = new FPM_PersonalDetail();

                          personal.Title = string.IsNullOrEmpty(personDetail.SelectSingleNode("FRAUD_PERSONAL_TITLE").InnerText) ? string.Empty : personDetail.SelectSingleNode("FRAUD_PERSONAL_TITLE").InnerText;
                          personal.Surname = string.IsNullOrEmpty(personDetail.SelectSingleNode("FRAUD_PERSONAL_SURNAME").InnerText) ? string.Empty : personDetail.SelectSingleNode("FRAUD_PERSONAL_SURNAME").InnerText;
                          personal.Firstname = string.IsNullOrEmpty(personDetail.SelectSingleNode("FRAUD_PERSONAL_FIRSTNAME").InnerText) ? string.Empty : personDetail.SelectSingleNode("FRAUD_PERSONAL_FIRSTNAME").InnerText;
                          personal.ID = string.IsNullOrEmpty(personDetail.SelectSingleNode("FRAUD_PERSONAL_ID").InnerText) ? string.Empty : personDetail.SelectSingleNode("FRAUD_PERSONAL_ID").InnerText;
                          personal.Passport = string.IsNullOrEmpty(personDetail.SelectSingleNode("FRAUD_PERSONAL_PASSPORT").InnerText) ? string.Empty : personDetail.SelectSingleNode("FRAUD_PERSONAL_PASSPORT").InnerText;
                          personal.DateOfBirth = string.IsNullOrEmpty(personDetail.SelectSingleNode("FRAUD_PERSONAL_DATEOFBIRTH").InnerText) ? string.Empty : personDetail.SelectSingleNode("FRAUD_PERSONAL_DATEOFBIRTH").InnerText;
                          personal.Gender = string.IsNullOrEmpty(personDetail.SelectSingleNode("FRAUD_PERSONAL_GENDER").InnerText) ? string.Empty : personDetail.SelectSingleNode("FRAUD_PERSONAL_GENDER").InnerText;
                          personal.Email = string.IsNullOrEmpty(personDetail.SelectSingleNode("FRAUD_PERSONAL_EMAIL").InnerText) ? string.Empty : personDetail.SelectSingleNode("FRAUD_PERSONAL_EMAIL").InnerText;

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
                          FPM_IncidentDetail incident = new FPM_IncidentDetail();

                          incident.Victim = string.IsNullOrEmpty(incidentDetail.SelectSingleNode("FRAUD_INCIDENT_VICTIM").InnerText) ? false : incidentDetail.SelectSingleNode("FRAUD_INCIDENT_VICTIM").InnerText == "NO" ? false : true;
                          incident.MembersReference = string.IsNullOrEmpty(incidentDetail.SelectSingleNode("FRAUD_INCIDENT_MEMBERSREF").InnerText) ? string.Empty : incidentDetail.SelectSingleNode("FRAUD_INCIDENT_MEMBERSREF").InnerText;
                          incident.Category = string.IsNullOrEmpty(incidentDetail.SelectSingleNode("FRAUD_INCIDENT_CATEGORY").InnerText) ? string.Empty : incidentDetail.SelectSingleNode("FRAUD_INCIDENT_CATEGORY").InnerText;
                          incident.SubCategory = string.IsNullOrEmpty(incidentDetail.SelectSingleNode("FRAUD_INCIDENT_SUBCAT").InnerText) ? string.Empty : incidentDetail.SelectSingleNode("FRAUD_INCIDENT_SUBCAT").InnerText;
                          incident.IncidentDate = string.IsNullOrEmpty(incidentDetail.SelectSingleNode("FRAUD_INCIDENT_DATE").InnerText) ? string.Empty : incidentDetail.SelectSingleNode("FRAUD_INCIDENT_DATE").InnerText;
                          incident.SubRole = string.IsNullOrEmpty(incidentDetail.SelectSingleNode("FRAUD_INCIDENT_SUBROLE").InnerText) ? string.Empty : incidentDetail.SelectSingleNode("FRAUD_INCIDENT_SUBROLE").InnerText;
                          incident.City = string.IsNullOrEmpty(incidentDetail.SelectSingleNode("FRAUD_INCIDENT_CITY").InnerText) ? string.Empty : incidentDetail.SelectSingleNode("FRAUD_INCIDENT_CITY").InnerText;
                          incident.Detail = string.IsNullOrEmpty(incidentDetail.SelectSingleNode("FRAUD_INCIDENT_DETAIL").InnerText) ? string.Empty : incidentDetail.SelectSingleNode("FRAUD_INCIDENT_DETAIL").InnerText;
                          incident.Forensic = string.IsNullOrEmpty(incidentDetail.SelectSingleNode("FRAUD_INCIDENT_FORENSIC").InnerText) ? string.Empty : incidentDetail.SelectSingleNode("FRAUD_INCIDENT_FORENSIC").InnerText;

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
                          FPM_AddressDetail address = new FPM_AddressDetail();

                          address.Type = string.IsNullOrEmpty(addressDetail.SelectSingleNode("FRAUD_ADDRESSES_TYPE").InnerText) ? string.Empty : addressDetail.SelectSingleNode("FRAUD_ADDRESSES_TYPE").InnerText;
                          address.Street = string.IsNullOrEmpty(addressDetail.SelectSingleNode("FRAUD_ADDRESSES_STREET").InnerText) ? string.Empty : addressDetail.SelectSingleNode("FRAUD_ADDRESSES_STREET").InnerText;
                          address.Address = string.IsNullOrEmpty(addressDetail.SelectSingleNode("FRAUD_ADDRESSES_ADDRESS").InnerText) ? string.Empty : addressDetail.SelectSingleNode("FRAUD_ADDRESSES_ADDRESS").InnerText;
                          address.City = string.IsNullOrEmpty(addressDetail.SelectSingleNode("FRAUD_ADDRESSES_CITY").InnerText) ? string.Empty : addressDetail.SelectSingleNode("FRAUD_ADDRESSES_CITY").InnerText;
                          address.PostalCode = string.IsNullOrEmpty(addressDetail.SelectSingleNode("FRAUD_ADDRESSES_PCODE").InnerText) ? string.Empty : addressDetail.SelectSingleNode("FRAUD_ADDRESSES_PCODE").InnerText;
                          address.From = string.IsNullOrEmpty(addressDetail.SelectSingleNode("FRAUD_ADDRESSES_FROM").InnerText) ? string.Empty : addressDetail.SelectSingleNode("FRAUD_ADDRESSES_FROM").InnerText;
                          address.To = string.IsNullOrEmpty(addressDetail.SelectSingleNode("FRAUD_ADDRESSES_TO").InnerText) ? string.Empty : addressDetail.SelectSingleNode("FRAUD_ADDRESSES_TO").InnerText;

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
                          FPM_TelephoneDetail telephone = new FPM_TelephoneDetail();

                          telephone.Type = string.IsNullOrEmpty(telephoneDetail.SelectSingleNode("FRAUD_OTHERTEL_TYPE").InnerText) ? string.Empty : telephoneDetail.SelectSingleNode("FRAUD_OTHERTEL_TYPE").InnerText;
                          telephone.No = string.IsNullOrEmpty(telephoneDetail.SelectSingleNode("FRAUD_OTHERTEL_NUMBER").InnerText) ? string.Empty : telephoneDetail.SelectSingleNode("FRAUD_OTHERTEL_NUMBER").InnerText;
                          telephone.City = string.IsNullOrEmpty(telephoneDetail.SelectSingleNode("FRAUD_OTHERTEL_CITY").InnerText) ? string.Empty : telephoneDetail.SelectSingleNode("FRAUD_OTHERTEL_CITY").InnerText;
                          telephone.Country = string.IsNullOrEmpty(telephoneDetail.SelectSingleNode("FRAUD_OTHERTEL_COUNTRY").InnerText) ? string.Empty : telephoneDetail.SelectSingleNode("FRAUD_OTHERTEL_COUNTRY").InnerText;

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
                          FPM_AliasDetail alias = new FPM_AliasDetail();

                          alias.FirstName = string.IsNullOrEmpty(aliasDetail.SelectSingleNode("FRAUD_ALIAS_FIRSTNAME").InnerText) ? string.Empty : aliasDetail.SelectSingleNode("FRAUD_ALIAS_FIRSTNAME").InnerText;
                          alias.Surname = string.IsNullOrEmpty(aliasDetail.SelectSingleNode("FRAUD_ALIAS_SURNAME").InnerText) ? string.Empty : aliasDetail.SelectSingleNode("FRAUD_ALIAS_SURNAME").InnerText;

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
                          FPM_OtherIdDetail other = new FPM_OtherIdDetail();

                          other.IDNo = string.IsNullOrEmpty(otherIdDetail.SelectSingleNode("FRAUD_OTHERID_ID").InnerText) ? string.Empty : otherIdDetail.SelectSingleNode("FRAUD_OTHERID_ID").InnerText;
                          other.Type = string.IsNullOrEmpty(otherIdDetail.SelectSingleNode("FRAUD_OTHERID_TYPE").InnerText) ? string.Empty : otherIdDetail.SelectSingleNode("FRAUD_OTHERID_TYPE").InnerText;
                          other.IssueDate = string.IsNullOrEmpty(otherIdDetail.SelectSingleNode("FRAUD_OTHERID_ISSUEDATE").InnerText) ? string.Empty : otherIdDetail.SelectSingleNode("FRAUD_OTHERID_ISSUEDATE").InnerText;
                          other.Country = string.IsNullOrEmpty(otherIdDetail.SelectSingleNode("FRAUD_OTHERID_COUNTRY").InnerText) ? string.Empty : otherIdDetail.SelectSingleNode("FRAUD_OTHERID_COUNTRY").InnerText;

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
                          FPM_EmploymentDetail employer = new FPM_EmploymentDetail();

                          employer.Name = string.IsNullOrEmpty(employerDetail.SelectSingleNode("FRAUD_EMPLOYER_NAME").InnerText) ? string.Empty : employerDetail.SelectSingleNode("FRAUD_EMPLOYER_NAME").InnerText;
                          employer.Telephone = string.IsNullOrEmpty(employerDetail.SelectSingleNode("FRAUD_EMPLOYER_TEL").InnerText) ? string.Empty : employerDetail.SelectSingleNode("FRAUD_EMPLOYER_TEL").InnerText;
                          employer.RegisteredName = string.IsNullOrEmpty(employerDetail.SelectSingleNode("FRAUD_EMPLOYER_COMPANYNAME").InnerText) ? string.Empty : employerDetail.SelectSingleNode("FRAUD_EMPLOYER_COMPANYNAME").InnerText;
                          employer.CompanyNo = string.IsNullOrEmpty(employerDetail.SelectSingleNode("FRAUD_EMPLOYER_COMPANYNO").InnerText) ? string.Empty : employerDetail.SelectSingleNode("FRAUD_EMPLOYER_COMPANYNO").InnerText;
                          employer.Occupation = string.IsNullOrEmpty(employerDetail.SelectSingleNode("FRAUD_EMPLOYER_OCCUPATION").InnerText) ? string.Empty : employerDetail.SelectSingleNode("FRAUD_EMPLOYER_OCCUPATION").InnerText;
                          employer.From = string.IsNullOrEmpty(employerDetail.SelectSingleNode("FRAUD_EMPLOYER_DATEFROM").InnerText) ? string.Empty : employerDetail.SelectSingleNode("FRAUD_EMPLOYER_DATEFROM").InnerText;
                          employer.To = string.IsNullOrEmpty(employerDetail.SelectSingleNode("FRAUD_EMPLOYER_DATETO").InnerText) ? string.Empty : employerDetail.SelectSingleNode("FRAUD_EMPLOYER_DATETO").InnerText;

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
                          FPM_BankDetail bank = new FPM_BankDetail();

                          bank.AccountNo = string.IsNullOrEmpty(bankDetail.SelectSingleNode("FRAUD_BANK_NO").InnerText) ? string.Empty : bankDetail.SelectSingleNode("FRAUD_BANK_NO").InnerText;
                          bank.AccountType = string.IsNullOrEmpty(bankDetail.SelectSingleNode("FRAUD_BANK_TYPE").InnerText) ? string.Empty : bankDetail.SelectSingleNode("FRAUD_BANK_TYPE").InnerText;
                          bank.Bank = string.IsNullOrEmpty(bankDetail.SelectSingleNode("FRAUD_BANK_BANK").InnerText) ? string.Empty : bankDetail.SelectSingleNode("FRAUD_BANK_BANK").InnerText;
                          bank.From = string.IsNullOrEmpty(bankDetail.SelectSingleNode("FRAUD_BANK_DATEFROM").InnerText) ? string.Empty : bankDetail.SelectSingleNode("FRAUD_BANK_DATEFROM").InnerText;
                          bank.To = string.IsNullOrEmpty(bankDetail.SelectSingleNode("FRAUD_BANK_DATETO").InnerText) ? string.Empty : bankDetail.SelectSingleNode("FRAUD_BANK_DATETO").InnerText;

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
                          FPM_CaseDetail @case = new FPM_CaseDetail();

                          @case.CaseNo = string.IsNullOrEmpty(caseDetail.SelectSingleNode("FRAUD_CASE_NO").InnerText) ? string.Empty : caseDetail.SelectSingleNode("FRAUD_CASE_NO").InnerText;
                          @case.ReportDate = string.IsNullOrEmpty(caseDetail.SelectSingleNode("FRAUD_CASE_REPORTDATE").InnerText) ? string.Empty : caseDetail.SelectSingleNode("FRAUD_CASE_REPORTDATE").InnerText;
                          @case.Officer = string.IsNullOrEmpty(caseDetail.SelectSingleNode("FRAUD_CASE_OFFICER").InnerText) ? string.Empty : caseDetail.SelectSingleNode("FRAUD_CASE_OFFICER").InnerText;
                          @case.CreatedBy = string.IsNullOrEmpty(caseDetail.SelectSingleNode("FRAUD_CASE_CREATEDBY").InnerText) ? string.Empty : caseDetail.SelectSingleNode("FRAUD_CASE_CREATEDBY").InnerText;
                          @case.Station = string.IsNullOrEmpty(caseDetail.SelectSingleNode("FRAUD_CASE_STATION").InnerText) ? string.Empty : caseDetail.SelectSingleNode("FRAUD_CASE_STATION").InnerText;
                          @case.Type = string.IsNullOrEmpty(caseDetail.SelectSingleNode("FRAUD_CASE_TYPE").InnerText) ? string.Empty : caseDetail.SelectSingleNode("FRAUD_CASE_TYPE").InnerText;
                          @case.Status = string.IsNullOrEmpty(caseDetail.SelectSingleNode("FRAUD_CASE_STATUS").InnerText) ? string.Empty : caseDetail.SelectSingleNode("FRAUD_CASE_STATUS").InnerText;
                          @case.Reason = string.IsNullOrEmpty(caseDetail.SelectSingleNode("FRAUD_CASE_REASON").InnerText) ? string.Empty : caseDetail.SelectSingleNode("FRAUD_CASE_REASON").InnerText;
                          @case.ReasonExtension = string.IsNullOrEmpty(caseDetail.SelectSingleNode("FRAUD_CASE_REASONEXTENSION").InnerText) ? string.Empty : caseDetail.SelectSingleNode("FRAUD_CASE_REASONEXTENSION").InnerText;
                          @case.ContactNo = string.IsNullOrEmpty(caseDetail.SelectSingleNode("FRAUD_CASE_CONTACTNO").InnerText) ? string.Empty : caseDetail.SelectSingleNode("FRAUD_CASE_CONTACTNO").InnerText;
                          @case.Email = string.IsNullOrEmpty(caseDetail.SelectSingleNode("FRAUD_CASE_EMAIL").InnerText) ? string.Empty : caseDetail.SelectSingleNode("FRAUD_CASE_EMAIL").InnerText;
                          @case.Fax = string.IsNullOrEmpty(caseDetail.SelectSingleNode("FRAUD_CASE_FAX").InnerText) ? string.Empty : caseDetail.SelectSingleNode("FRAUD_CASE_FAX").InnerText;
                          @case.Details = string.IsNullOrEmpty(caseDetail.SelectSingleNode("FRAUD_CASE_DETAILS").InnerText) ? string.Empty : caseDetail.SelectSingleNode("FRAUD_CASE_DETAILS").InnerText;

                          result.FPMCaseDetails.Add(@case);
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

        #region Products

        var products = Document.SelectNodes("ROOT/ATLAS_CODIX/PRODUCTS");

        if (products != null)
        {
          if (products.Count > 0)
          {

            result.Products = new List<Product>();

            foreach (XmlNode detail in products)
            {
              var productCollection = detail.SelectNodes("PRODUCT");
              foreach (XmlNode productDetail in productCollection)
              {
                Product product = new Product();

                product.Description = productDetail.SelectSingleNode("product_description").InnerText;
                product.Outcome = productDetail.SelectSingleNode("outcome").InnerText;

                var reasonNodeCollection = productDetail.SelectNodes("reasons");
                if (reasonNodeCollection != null)
                {
                  if (reasonNodeCollection.Count > 0)
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
                }
                result.Products.Add(product);
              }
            }
          }
        }
        #endregion

        Document = null;
      }
    }
  }
    #endregion

}
