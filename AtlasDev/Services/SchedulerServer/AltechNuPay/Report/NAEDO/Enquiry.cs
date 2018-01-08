using System;
using System.Xml.Linq;

using Atlas.Common.Extensions;
using Atlas.Domain.DTO;
using Atlas.Common.Interface;


namespace SchedulerServer.AltechNuPay.Report.NAEDO
{
  public static class Enquiry
  {
    public static XElement GetReport(ILogging log, NAEDOLoginDTO naedoLogin, int reportTypeId, string reportTypeTag, int serviceTypeId, DateTime startDate,
        DateTime endDate, ref bool isSuccess, ref string errorMessage, long tokenId = 0, int blockId = 0, int allowedRetryAttempts = 3)
    {
      using (var client = new wsNaedoSoapClient())
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
                               naedoLogin.MerchantId,
                               naedoLogin.Username,
                               naedoLogin.Password,
                               serviceTypeId,
                               reportTypeId,
                               startDate,
                               endDate,
                               int.Parse(tokenId.ToString()), blockId);  // BlockId = 0 & TokenId = 0: Fresh Report

            xmlReport = xmlNode.GetXElement();
          }
          catch (Exception err)
          {
            log.Error(err, "[NAEDO]- getReport()");
            errorMessage = err.Message;
            retryAttempt++;
            isError = true;
            continue;
          }

          if (FileStructureHelper.IsError(xmlReport, out errorMessage))
          {
            log.Error("[NAEDO]- IsError: {error}", errorMessage);
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
            log.Warning("[NAEDO]- Empty result");
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
