using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Atlas.Common.Extensions;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Gyrkin.Controllers.Api.Models;
using Serilog;
using Stream.Framework.Repository;

namespace Falcon.Gyrkin.Controllers.Api
{
  public class StreamReportController : ApiController
  {
    #region injections

    private readonly IStreamReportRepository _streamReportRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILogger _logger;

    #endregion

    public StreamReportController(IStreamReportRepository streamReportRepository, ICompanyRepository companyRepository,
      ILogger logger)
    {
      _streamReportRepository = streamReportRepository;
      _companyRepository = companyRepository;
      _logger = logger;
    }

    [HttpGet]
    public HttpResponseMessage GetFilterData()
    {
      try
      {
        var groupTypes =
          EnumUtil.GetValues<Stream.Framework.Enumerators.Stream.GroupType>()
            .Select(a => new {GroupTypeId = a.ToInt(), Description = a.ToStringEnum()});
        var caseStatuses =
          EnumUtil.GetValues<Stream.Framework.Enumerators.CaseStatus.Type>()
            .Select(a => new {CaseStatusId = a.ToInt(), Description = a.ToStringEnum()});
        var streamTypes =
          EnumUtil.GetValues<Stream.Framework.Enumerators.Stream.StreamType>()
            .Select(a => new {StreamId= a.ToInt(), Description = a.ToStringEnum()});
        var branches = _companyRepository.GetActiveBranches();
        var regions = branches.Select(a => new { a.RegionId, Description = a.Region })
            .Distinct()
            .OrderBy(r => r.Description)
            .ToList();
        return Request.CreateResponse(HttpStatusCode.OK,
          new {regions, branches, groupTypes, caseStatuses, streamTypes });
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("StreamController - GetEscalatedItems: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetPerformanceReport(StreamReportModel.StreamFilterModel model)
    {
      try
      {
        var excelFile =
          _streamReportRepository.GetPerformanceReport(
            (Stream.Framework.Enumerators.Stream.GroupType)model.GroupTypeId, model.StartDate, model.EndDate, model.BranchIds);

        return Request.CreateResponse(HttpStatusCode.OK, new {response = Convert.ToBase64String(excelFile)});
      }
      catch (Exception ex)
      {
        _logger.Error(
          string.Format(
            "[StreamReportController][GetPerformanceReport] model.GroupTypeId: {0} | model.BranchIds: {1} | model.StartDate: {2} | model.EndDate: {3}",
            model.GroupTypeId, string.Join(",", model.BranchIds), model.StartDate, model.EndDate));
        _logger.Error(string.Format("[StreamReportController][GetPerformanceReport] {0} - {1}", ex.Message,
          ex.InnerException));
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetDetailReport(StreamReportModel.StreamDetailFilterModel model)
    {
      try
      {
        var excelFile =
          _streamReportRepository.GetDetailReport((Stream.Framework.Enumerators.Stream.GroupType) model.GroupTypeId,
            model.BranchId, model.StreamIds, model.CaseStatusIds);

        return Request.CreateResponse(HttpStatusCode.OK, new {response = Convert.ToBase64String(excelFile)});
      }
      catch (Exception ex)
      {
        _logger.Error(
          string.Format(
            "[StreamReportController][GetDetailReport] model.GroupTypeId: {0} | model.BranchId: {1} | model.StreamIds: {2} | model.CaseStatusIds: {3}",
            model.GroupTypeId, string.Join(",", model.BranchId), model.StreamIds, model.CaseStatusIds));
        _logger.Error(string.Format("[StreamReportController][GetDetailReport] {0} - {1}", ex.Message,
          ex.InnerException));
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetAccountsExport(StreamReportModel.StreamAccounts model)
    {
      try
      {
        var excelFile =
          _streamReportRepository.GetAccountsExport((Stream.Framework.Enumerators.Stream.GroupType) model.GroupTypeId,
            model.StartDate, model.EndDate, model.RegionId, model.CategoryId, model.BranchIds, model.AllocatedUserId,
            model.Column);

        return Request.CreateResponse(HttpStatusCode.OK, new {response = Convert.ToBase64String(excelFile)});
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format(
          "[StreamReportController][GetAccountData] model.GroupTypeId: {0} | model.BranchIds: {1} | model.StartDate: {2} | model.EndDate: {3} | model.AllocatedUserId: {4} | model.Column: {5} || {6} - {7}",
          model.GroupTypeId, string.Join(",", model.BranchIds), model.StartDate, model.EndDate, model.AllocatedUserId,
          model.Column, ex.Message, ex.InnerException));
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetAccountData(StreamReportModel.StreamOverviewModel model)
    {
      try
      {
        var accountData =
          _streamReportRepository.GetOverview((Stream.Framework.Enumerators.Stream.GroupType) model.GroupTypeId,
            model.StartDate, model.EndDate, model.RegionId, model.CategoryId, model.SubCategoryId, model.BranchIds, model.DrillDownLevel);

        return Request.CreateResponse(HttpStatusCode.OK, new {accountData});
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format(
          "[StreamReportController][GetAccountData] model.GroupTypeId: {0} | model.BranchIds: {1} | model.StartDate: {2} | model.EndDate: {3} | model.DrillDownLevel: {4} || {5} - {6}",
          model.GroupTypeId, string.Join(",", model.BranchIds), model.StartDate, model.EndDate, model.DrillDownLevel,
          ex.Message, ex.InnerException));
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetAccounts(StreamReportModel.StreamAccounts model)
    {
      try
      {
        var streamData =
          _streamReportRepository.GetAccounts((Stream.Framework.Enumerators.Stream.GroupType) model.GroupTypeId,
            model.StartDate, model.EndDate, model.CategoryId, model.BranchIds, model.AllocatedUserId, model.Column);

        return Request.CreateResponse(HttpStatusCode.OK, new {streamData});
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format(
          "[StreamReportController][GetAccountData] model.GroupTypeId: {0} | model.BranchIds: {1} | model.StartDate: {2} | model.EndDate: {3} | model.AllocatedUserId: {4} | model.Column: {5} || {6} - {7}",
          model.GroupTypeId, string.Join(",", model.BranchIds), model.StartDate, model.EndDate, model.AllocatedUserId,
          model.Column, ex.Message, ex.InnerException));
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    private Tuple<DateTime, DateTime> GetDateRange(int month, int year)
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