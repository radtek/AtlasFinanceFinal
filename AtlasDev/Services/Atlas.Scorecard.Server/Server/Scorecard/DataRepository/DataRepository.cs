using System;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Xml;

using DevExpress.Xpo;

using Atlas.Common.Utils;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using Atlas.Common.Interface;


namespace Atlas.ThirdParty.CS.Enquiry
{
  internal class DataRepository
  {
    // Check if we have a recent scorecard and use that
    internal static byte[] CheckForRecentScorecard(string legacyBranchNum, string IdNumber,
      out string userName, out string password, out Int64 serviceId, out Int64 branchId, out Int64 enquiryId)
    {
      byte[] zipFile = null;
      userName = null;
      password = null;
      serviceId = 0;
      branchId = 1;
      enquiryId = 0;
     
      using (var unitOfWork = new UnitOfWork())
      {
        #region Get Bureau details
        var creditService = ConfigurationManager.AppSettings["CompuScanDevSite"] != null ?
          unitOfWork.Query<BUR_Service>()
            .FirstOrDefault(s =>
              s.Enabled &&
              s.ServiceType == Risk.ServiceType.Atlas_Online_Credit) :
          unitOfWork.Query<BUR_Service>()
            .FirstOrDefault(s => s.Enabled && s.ServiceType == Risk.ServiceType.Credit &&
              s.Branch.LegacyBranchNum.PadLeft(3, '0') == legacyBranchNum.PadLeft(3, '0'));
        if (creditService == null)
        {
          throw new Exception("No service profile found for credit");
        }

        userName = creditService.Username;
        password = creditService.Password;
        serviceId = creditService.ServiceId;
        var keepForDays = creditService.Days;
        branchId = creditService.Branch != null ? creditService.Branch.BranchId : 1;
        #endregion

        var lastEnquiry = unitOfWork.Query<BUR_Enquiry>()
          .Where(s => s.IdentityNum == IdNumber
            && s.IsSucess
            && s.TransactionType == Risk.RiskTransactionType.Score
            && s.PreviousEnquiry == null
            && s.EnquiryType == Risk.RiskEnquiryType.Credit)
          .OrderByDescending(s => s.EnquiryDate)
          .Select(s => new { EnquiryId = s.EnquiryId, CreateDate = s.CreateDate })
          .FirstOrDefault();

        if (lastEnquiry != null && DateTime.Now.Subtract((DateTime)lastEnquiry.CreateDate).TotalHours < (keepForDays * 24))
        {
          enquiryId = lastEnquiry.EnquiryId;

          zipFile = unitOfWork.Query<BUR_Storage>()
            .Where(s => s.Enquiry.EnquiryId == lastEnquiry.EnquiryId)
            .Select(s => s.OriginalResponse)
            .FirstOrDefault();
        }
      }

      return zipFile;
    }


    /// <summary>
    /// SAve a scorecard to BUR_Enquiry and BUR_Storage
    /// </summary>   
    internal static void SaveScorecard(ILogging log, byte[] scoreCard, byte[] zipFile, long branchId,
      DateTime enquiryDate, string IdNumber, string firstName, string surname, long serviceId, out Int64 enquiryId)
    {
      #region Get the CS enquiry ID from the XML
      enquiryId = 0;
      var doc = new XmlDocument();
      doc.LoadXml(ASCIIEncoding.ASCII.GetString(scoreCard));

      var id = doc.DocumentElement.SelectSingleNode("/ROOT/Enquiry_ID") ?? doc.SelectSingleNode("/ROOT/ConsumerEnquiry/ReturnData/NLR_ReferenceNo");
      if (id == null || !Int64.TryParse(id.InnerText, out enquiryId))
      {
        throw new Exception("Failed to parse/locate the CompuScan enquiry ID");
      }
      #endregion

      #region Save the enquiry to DB
      using (var unitOfWork = new UnitOfWork())
      {
        var enquiry = new BUR_Enquiry(unitOfWork)
          {
            Branch = unitOfWork.Query<BRN_Branch>().First(s => s.BranchId == branchId),
            CreateDate = enquiryDate,
            EnquiryDate = DateTime.Now,
            IdentityNum = IdNumber,
            IsSucess = true,
            FirstName = firstName,
            LastName = surname,
            TransactionType = Risk.RiskTransactionType.Score,
            EnquiryType = Risk.RiskEnquiryType.Credit,
            EnquiryId = enquiryId,
            Service = unitOfWork.Query<BUR_Service>().First(s => s.ServiceId == serviceId),
            CorrelationId = Guid.NewGuid(),
            ObjectVersion = 2
          };

        var response = ScoreXmlUtils.Deserialize(log, scoreCard); // TODO: Kill this...! Extract necessary items to BUR_... tables
        new BUR_Storage(unitOfWork)
          {
            Enquiry = enquiry,
            OriginalResponse = zipFile, // TODO: ?? mongo ??
            //RequestMessage = Atlas.Common.Utils.Compression.Compress() // no point really?
            ResponseMessage = Compression.Compress(Xml.Serialize<ResponseResultV2>(response))// TODO: PLEASE kill this!
          };

        unitOfWork.CommitChanges();

        enquiryId = enquiry.EnquiryId;
      }
      #endregion
    }

  }
}
