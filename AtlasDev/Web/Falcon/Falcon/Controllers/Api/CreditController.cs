using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Atlas.Common.Utils;
using Falcon.Gyrkin.Library.Security.Attributes;

namespace Falcon.Controllers.Api
{
  public sealed class CreditController : AppApiController
  {
    [HttpPost]
    [Authorize]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage Resubmit(long enquiryId)
    {
      try
      {
        return Request.CreateResponse(HttpStatusCode.OK, new { result = Services.WebServiceClient.Credit_ReSubmit(enquiryId) });
      }
      catch (Exception ex)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { result = ex.Message.ToString() });
      }
    }


    [HttpGet]
    public async Task<HttpResponseMessage> Report(long reportId)
    {
      try
      {
        var report = await Services.WebServiceClient.Credit_FetchCreditReportAsync(reportId);

        var dataDirectory = string.Format(string.Format(@"{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "Static"));

        //var x = Services.WebServiceClient.ConvertMHTMLToPDF(report);

        if (!string.IsNullOrEmpty(dataDirectory))
        {
          var dir = new DirectoryInfo(string.Format(@"{0}\{1}\{2}\{3}", dataDirectory, "Credit", "Reports", reportId));

          if (!dir.Exists)
            dir.Create();

          if (!File.Exists(DateTime.Now.ToString("yyyyMMdd")))
            File.WriteAllBytes(string.Format(@"{0}\{1}.mht", dir.FullName, DateTime.Now.ToString("yyyyMMdd")), Base64.DecodeString(report));

        }
        string url = "ff";// Request.BaseURL();

        if (!string.IsNullOrEmpty(report))
          return Request.CreateResponse(HttpStatusCode.OK, string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}.mht", url, "Static", "Credit", "Reports", reportId.ToString(), DateTime.Now.ToString("yyyyMMdd")));
        else
          return Request.CreateResponse(HttpStatusCode.NoContent, "Report stream returned an empty value.");

      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to load credit report view.");
      }
    }
  }
}