using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Falcon.Common.Structures.Branch;
using Falcon.Gyrkin.Controllers.Api.Models.Target;

namespace Falcon.Gyrkin.Controllers.Api
{
  public class TargetController : ApiController
  {
    private readonly ITargetService _targetService;
    private readonly ICompanyRepository _companyRepository;

    public TargetController(ITargetService targetService, ICompanyRepository companyRepository)
    {
      _targetService = targetService;
      _companyRepository = companyRepository;
    }

    [HttpPost]
    public HttpResponseMessage AddBranchCiMonthly(BranchCiMonthlyModel model)
    {
      try
      {
        _targetService.AddBranchCiMonthly(model.BranchId, model.HostId, model.TargetDate, model.Amount, model.Percent,
          model.UserId);
        return Request.CreateResponse(HttpStatusCode.OK, "Saved");
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage AddDailySale(DailySaleModel model)
    {
      try
      {
        _targetService.AddDailySale(model.BranchId, model.HostId, model.TargetDate, model.Amount, model.Percent,
          model.UserId);
        return Request.CreateResponse(HttpStatusCode.OK, "Saved");
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage AddHandoverTarget(HandoverTargetModel model)
    {
      try
      {
        _targetService.AddHandoverTarget(model.BranchId, model.HostId, model.TargetDate, model.HandoverBudget, model.ArrearTarget, model.UserId);
        return Request.CreateResponse(HttpStatusCode.OK, "Saved");
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage AddLoanMix(LoanMixModel model)
    {
      try
      {
        _targetService.AddLoanMix(model.TargetDate, model.PayNo, model.Percent);
        return Request.CreateResponse(HttpStatusCode.OK, "Saved");
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpGet]
    public HttpResponseMessage GetMonthlyCiFilterData()
    {
      try
      {
        var branches = _companyRepository.GetActiveBranches().ToList();

        branches.Insert(0, new Branch
        {
          BranchId = 0,
          RegionId = 0,
          Region = string.Empty,
          Name = "Select All",
          LegacyBranchNum = string.Empty
        });

        var hosts = _companyRepository.GetAllHosts();

        return Request.CreateResponse(HttpStatusCode.OK,
          new { hosts, branches });
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpGet]
    public HttpResponseMessage GetHandoverTargetFilterData()
    {
      try
      {
        var branches = _companyRepository.GetActiveBranches().ToList();

        branches.Insert(0, new Branch
        {
          BranchId = 0,
          RegionId = 0,
          Region = string.Empty,
          Name = "Select All",
          LegacyBranchNum = string.Empty
        });

        var hosts = _companyRepository.GetAllHosts();

        return Request.CreateResponse(HttpStatusCode.OK,
          new { hosts, branches });
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetDailySales(GetDailySalesModel model)
    {
      try
      {
        return Request.CreateResponse(HttpStatusCode.OK,
          _targetService.GetDailySales(GetEndDate(model.TargetMonth, model.TargetYear)));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetLoanMix(GetLoanMixModel model)
    {
      try
      {
        return Request.CreateResponse(HttpStatusCode.OK,
          _targetService.GetLoanMixes(GetEndDate(model.TargetMonth, model.TargetYear)));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetBranchCiMonthly(GetBranchCiMonthly model)
    {
      try
      {
        return Request.CreateResponse(HttpStatusCode.OK,
          _targetService.GetBranchCiMonthlys(model.BranchId, model.HostId,
            GetEndDate(model.TargetMonth, model.TargetYear)));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetHandoverTarget(GetHandoverTarget model)
    {
      try
      {
        return Request.CreateResponse(HttpStatusCode.OK,
          _targetService.GetHandoverTargets(model.BranchId, model.HostId,
            GetEndDate(model.TargetMonth, model.TargetYear)));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
      }
    }

    private DateTime GetEndDate(int month, int year)
    {
      return new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
    }
  }
}