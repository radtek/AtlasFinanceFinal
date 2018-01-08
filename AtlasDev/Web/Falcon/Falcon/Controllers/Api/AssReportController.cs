using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web.Hosting;
using System.Web.Http;

using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Security.Role;


namespace Falcon.Controllers.Api
{
  [Authorize]
  public sealed class AssReportController : AppApiController
  {
    //// GET api/<controller>
    //[HttpGet]
    //[WebApiAntiForgeryToken]
    //public HttpResponseMessage GetRegions(long personId)
    //{
    //  var response = Services.WebServiceClient.AssReporting_GetPersonRegions(personId);

    //  return Request.CreateResponse(HttpStatusCode.OK, new { regions = response });
    //}

    //// GET api/<controller>
    //[HttpGet]
    //[WebApiAntiForgeryToken]
    //public HttpResponseMessage GetBranches(long regionId)
    //{
    //  var response = Services.WebServiceClient.AssReporting_GetRegionBranches(regionId);

    //  return Request.CreateResponse(HttpStatusCode.OK, new { branches = response });
    //}

    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetRegionBranches(long personId)
    {
      var response = Services.WebServiceClient.AssReporting_GetPersonRegionBranches(personId);

      return Request.CreateResponse(HttpStatusCode.OK, new { regionBranches = response });
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetCheque(int getCheque, string branchIds, int month, int year)
    {
      var dateRange = GetDateRange(month, year);
      var branchId = FixIds(branchIds);

      var response = Services.WebServiceClient.AssReporting_GetCheque(branchId, dateRange.Item1, dateRange.Item2);

      return Request.CreateResponse(HttpStatusCode.OK, new { response = response });
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetInterestPercentile(int getInterestPercentile, string branchIds, int month, int year)
    {
      var dateRange = GetDateRange(month, year);
      var branchId = FixIds(branchIds);

      var response = Services.WebServiceClient.AssReporting_GetInterestPercentiles(branchId, dateRange.Item1, dateRange.Item2);

      return Request.CreateResponse(HttpStatusCode.OK, new { response = response });
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetInsurancePercentile(int getInsurancePercentile, string branchIds, int month, int year)
    {
      var dateRange = GetDateRange(month, year);
      var branchId = FixIds(branchIds);

      var response = Services.WebServiceClient.AssReporting_GetInsurancePercentiles(branchId, dateRange.Item1, dateRange.Item2);

      return Request.CreateResponse(HttpStatusCode.OK, new { response = response });
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetInsurance(int getInsurance, string branchIds, int month, int year)
    {
      var dateRange = GetDateRange(month, year);
      var branchId = FixIds(branchIds);

      var response = Services.WebServiceClient.AssReporting_GetInsuranceFees(branchId, dateRange.Item1, dateRange.Item2);

      return Request.CreateResponse(HttpStatusCode.OK, new { response = response });
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetInterest(int getInterest, string branchIds, int month, int year)
    {
      var dateRange = GetDateRange(month, year);
      var branchId = FixIds(branchIds);

      var response = Services.WebServiceClient.AssReporting_GetInterestFees(branchId, dateRange.Item1, dateRange.Item2);

      return Request.CreateResponse(HttpStatusCode.OK, new { response = response });
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetLoanMix(int getLoanMix, string branchIds, int month, int year)
    {
      var dateRange = GetDateRange(month, year);
      var branchId = FixIds(branchIds);

      var response = Services.WebServiceClient.AssReporting_GetLoanMix(branchId, dateRange.Item1, dateRange.Item2);

      return Request.CreateResponse(HttpStatusCode.OK, new { response = response });
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetAverageNewClientLoan(int getAverageNewClientLoan, string branchIds, int month, int year)
    {
      var dateRange = GetDateRange(month, year);
      var branchId = FixIds(branchIds);

      var response = Services.WebServiceClient.AssReporting_GetAverageNewClientLoan(branchId, dateRange.Item1, dateRange.Item2);

      return Request.CreateResponse(HttpStatusCode.OK, new { response = response });
    }


    // GET api/<controller>
    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetAverageLoan(int getAverageLoan, string branchIds, int month, int year)
    {
      var dateRange = GetDateRange(month, year);
      var branchId = FixIds(branchIds);

      var response = Services.WebServiceClient.AssReporting_GetAverageLoan(branchId, dateRange.Item1, dateRange.Item2);

      return Request.CreateResponse(HttpStatusCode.OK, new { response = response });
    }


    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetXls(int getXls, string branchIds, int month, int year)
    {
      var dateRange = GetDateRange(month, year);
      var branchId = FixIds(branchIds);

      var file = Services.WebServiceClient.AssReporting_ExportCIReport(branchId, dateRange.Item1, dateRange.Item2, ClaimsPrincipal.Current.HasClaim(ClaimTypes.Role, RoleType.POSSIBLE_HANDOVERS));

      var result = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StreamContent(new MemoryStream(file)) };

      result.Content.Headers.ContentType =
          new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
      return result;
    }


    [HttpGet]
    [WebApiAntiForgeryToken]
    public HttpResponseMessage GetXlsPath(int getXlsPath, string branchIds, int month, int year)
    {
      try
      {
        var dateRange = GetDateRange(month, year);
        var branchId = FixIds(branchIds);
        try
        {
          var excelFile1 = Services.WebServiceClient.AssReporting_ExportCIReport(branchId, dateRange.Item1,
            dateRange.Item2,
            ClaimsPrincipal.Current.HasClaim(ClaimTypes.Role, RoleType.POSSIBLE_HANDOVERS));
        }
        catch (Exception exception)
        {
          return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "error calling service:"+exception.Message +": "+ exception.InnerException});
        }
        var excelFile = Services.WebServiceClient.AssReporting_ExportCIReport(branchId, dateRange.Item1,
          dateRange.Item2,
          ClaimsPrincipal.Current.HasClaim(ClaimTypes.Role, RoleType.POSSIBLE_HANDOVERS));

        foreach (var file in Directory.GetFiles(HostingEnvironment.MapPath("~/FileServer")))
        {
          var fi = new FileInfo(file);
          if (fi.CreationTime < DateTime.Now.AddMinutes(5))
          {
            try
            {
              fi.Delete();
            }
            catch (Exception exception)
            {
              // app pool locks file. when it is released, it will be deleted 
            }
          }
        }

        var filename = string.Format("export_{0}.xlsx", Guid.NewGuid());
        var filePath = Path.Combine(HostingEnvironment.MapPath("~/FileServer"), filename);
        var fileStream = new FileStream(filePath, FileMode.Create,
          FileAccess.Write);

        fileStream.Write(excelFile, 0, excelFile.Length);

        fileStream.Close();

        return Request.CreateResponse(HttpStatusCode.OK, new {response = string.Format("/FileServer/{0}", filename)});
      }
      catch (Exception exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = exception.Message + ": " + exception.InnerException });
      }
    }

    private static List<long> FixIds(string ids)
    {
      return string.IsNullOrEmpty(ids) || ids == "undefined" ? new List<long>() : ids.Split(',').Select(c => long.Parse(c)).ToList();
    }


    private static Tuple<DateTime, DateTime> GetDateRange(int month, int year)
    {
      if (month == 0)
      {
        var startDate = DateTime.Today.Date;
        var endDate = startDate;
        return new Tuple<DateTime, DateTime>(startDate, endDate);
      }
      else
      {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        return new Tuple<DateTime, DateTime>(startDate, endDate);
      }
    }
  }
}