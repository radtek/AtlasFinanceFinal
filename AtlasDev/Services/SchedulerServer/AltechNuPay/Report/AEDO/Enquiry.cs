using System;
using System.Xml.Linq;

using Atlas.Domain.DTO;
using Atlas.Common.Extensions;
using Atlas.Common.Interface;


namespace SchedulerServer.AltechNuPay.Report.AEDO
{
  public static class Enquiry
  {
    // blockId = 0, gets a fresh report
    public static XElement GetReport(ILogging log, AEDOLoginDTO aedoLogin, int reportType, string reportTypeTag, string userType,
         DateTime startDate, DateTime endDate, ref bool isSuccess, ref string errorMessage, long tokenId = 0, int blockId = 0, int allowedRetryAttempts = 3)
    {
      using (var client = new wsNupayReportSoapClient())
      {
        var retryAttempt = 1;
        var isError = true;
        XElement xmlReport = null;

        // Get Report from webservice and if error persists for <allowedRetryAttempts> retries, then throw exception
        while (isError && retryAttempt <= allowedRetryAttempts)
        {
          try
          {
            var xmlNode = client.getReport(
                                aedoLogin.MerchantNum,
                                aedoLogin.Password,
                                userType,
                                reportType.ToString(),
                                startDate,
                                endDate,
                                tokenId.ToString(), blockId.ToString());  // BlockId = 0 & TokenId = 0: Fresh Report
            xmlReport = xmlNode.GetXElement();
          }
          catch (Exception err)
          {
            log.Error(err, "[AEDO]- getReport()");
            errorMessage = err.Message;
            retryAttempt++;
            isError = true;
            continue;
          }

          if (FileStructureHelper.IsError(xmlReport, out errorMessage))
          {
            log.Error("[AEDO]- IsError: {error}", errorMessage);
            retryAttempt++;
            isError = true;
          }
          else
          {
            isError = false;
          }
        }
        if (!isError)
        {
          isSuccess = true;
          if (!FileStructureHelper.IsEmpty(xmlReport.Element(reportTypeTag)))
          {           
            errorMessage = string.Empty;
            return xmlReport;
          }
          else
          {
            log.Warning("[AEDO]- Empty result");
            errorMessage = "Empty Result";
            return null;
          }
        }
        else
        {
          isSuccess = false;
          return null;
        }
      }
    }
  }
}
