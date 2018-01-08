using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Atlas.Common.Extensions;
using Atlas.Common.Utils;
using Atlas.RabbitMQ.Messages.Notification;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Falcon.Common.Structures;
using Falcon.Gyrkin.Controllers.Api.Models.CiReport;
using Magnum;
using MassTransit;
using Serilog;
using Notification = Atlas.Enumerators.Notification;

namespace Falcon.Gyrkin.Controllers.Api
{
  public class CiReportController : ApiController
  {
    #region injections

    private readonly ICiReportService _ciReportService;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger _logger;
    private readonly IServiceBus _bus;

    #endregion

    public CiReportController(ICiReportService ciReportService, ICompanyRepository companyRepository, IUserRepository userRepository, ILogger logger, IServiceBus bus)
    {
      _ciReportService = ciReportService;
      _companyRepository = companyRepository;
      _userRepository = userRepository;
      _logger = logger;
      _bus = bus;
    }

    [HttpGet]
    public async Task<HttpResponseMessage> GetFilterData()
    {
      var result = await Task.Run(() =>
      {
        try
        {
          var groupTypes =
            EnumUtil.GetValues<Stream.Framework.Enumerators.Stream.GroupType>()
              .Select(a => new {GroupTypeId = (int) a, Description = a.ToStringEnum()});
          var branches = _companyRepository.GetActiveBranches();
          var regions = branches.Select(a => new {a.RegionId, Description = a.Region})
              .Distinct()
              .OrderBy(r => r.Description)
              .ToList();
          return Request.CreateResponse(HttpStatusCode.OK,
            new {regionBranches = _companyRepository.GetPersonRegionBranches(), branches, regions, groupTypes});
        }
        catch (Exception ex)
        {
          return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> GetCiReport(CiReportFilterModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          var excelFile = _ciReportService.GetCiReport(model.StartDate, model.EndDate, model.BranchIds.ToList());

          return Request.CreateResponse(HttpStatusCode.OK, new { response = Convert.ToBase64String(excelFile) });
        }
        catch (Exception ex)
        {
          _logger.Error(
            string.Format(
              "[CiReportController][GetCiReport] model.BranchIds: {0} | model.StartDate: {1} | model.EndDate: {2}",
              string.Join(",", model.BranchIds), model.StartDate, model.EndDate));
          _logger.Error(string.Format("[CiReportController][GetCiReport] {0} - {1}, {2}", ex.Message, ex.InnerException,
            ex.StackTrace));
          return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> EmailCiReport(EmailCiReportFilterModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          var email = model.Email;
          if (string.IsNullOrEmpty(email))
          {
            var person = _userRepository.GetPerson(model.UserId);
            if (person != null)
            {
              email = person.Email;
            }
          }
          if (string.IsNullOrEmpty(email))
          {
            throw new Exception("There is no email address for the given person");
          }
          var excelFile = _ciReportService.GetCiReport(model.StartDate, model.EndDate, model.BranchIds.ToList());

          var attachment = new List<Tuple<string, string, string>>();
          var data = Base64.EncodeString(excelFile);
          attachment.Add(
            new Tuple<string, string, string>(
              $"CIReport_EmailFromFalcon_{model.StartDate.ToString("ddMMyyyy")}_{model.EndDate.ToString("ddMMyyyy")}",
              ".xlsx", data));

          _bus.Publish(new EmailNotifyMessage(CombGuid.Generate())
          {
            CreatedAt = DateTime.Now,
            ActionDate = DateTime.Today,
            Body =
              string.Format("Hi, {0}{0}Please see attached CI Report.{0}{0}Regards,{0}Falcon", Environment.NewLine),
            From = "falcon@atcorp.co.za",
            IsHTML = false,
            Attachments = attachment,
            Priority = Notification.NotificationPriority.High,
            Subject =
              $"CI Report dated: {model.StartDate.ToString("dd MMM yyyy")} to {model.EndDate.ToString("dd MMM yyyy")}",
            To = email
          });

          return Request.CreateResponse(HttpStatusCode.OK, new {message = "To be Emailed"});
        }
        catch (Exception ex)
        {
          _logger.Error(
            string.Format(
              "[CiReportController][GetCiReport] model.BranchIds: {0} | model.StartDate: {1} | model.EndDate: {2}",
              string.Join(",", model.BranchIds), model.StartDate, model.EndDate));
          _logger.Error(string.Format("[CiReportController][GetCiReport] {0} - {1}, {2}", ex.Message, ex.InnerException,
            ex.StackTrace));
          return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }
  }
}