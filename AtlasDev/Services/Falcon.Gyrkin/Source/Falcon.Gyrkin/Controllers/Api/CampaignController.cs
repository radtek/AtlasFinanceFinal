using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Web.Http;
using Atlas.Domain.Model;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Common.Interfaces.Services;
using Falcon.Gyrkin.Controllers.Api.Models;

namespace Falcon.Gyrkin.Controllers.Api
{
  public sealed class CampaignController : ApiController
  {

    #region Injections

    readonly INotificationRepository _notificationRepository;
    readonly ICampaignService _campaignService;

    #endregion
    public CampaignController(INotificationRepository notificationRepository, ICampaignService campaignService)
    {
      _notificationRepository = notificationRepository;
      _campaignService = campaignService;
    }

    [HttpPost]
    public HttpResponseMessage Resend([FromBody] CampaignModels.ResendModel model)
    {
      try
      {
        var sms = _notificationRepository.GetSms(a => a.NotificationId == model.NotificationId).FirstOrDefault();
        if(sms != null)
        {
        _campaignService.EnqueueSms(sms.To, sms.Body, Notification.NotificationPriority.High);
        }
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Queued");
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetSms([FromBody] CampaignModels.SMSQueryModel model)
    {
      try
      {
        Expression<Func<NTF_Notification, bool>> func = c => c.Type.Type == Notification.NotificationType.SMS;

        if (model.NotificationId != null)
        {
          var funct0R = func.Compile();
          func = a => funct0R(a) && a.NotificationId == model.NotificationId;
        }

        if (model.StartDate != null)
        {
          var funct0R = func.Compile();
          if (model.EndDate != null && model.EndDate.Value == model.StartDate.Value)
            func = a => funct0R(a) && a.CreateDate >= model.StartDate && a.CreateDate <= model.EndDate.Value.AddDays(+1);
          else if (model.EndDate != null && model.EndDate.Value != model.StartDate.Value)
            func = a => funct0R(a) && a.CreateDate >= model.StartDate && a.CreateDate <= model.EndDate.Value;
          else
            func = a => funct0R(a) && a.CreateDate >= model.StartDate && a.CreateDate <= model.StartDate.Value.AddDays(+1);
        }

        return Request.CreateResponse(_notificationRepository.GetSms(func.Compile()));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }
  }
}
