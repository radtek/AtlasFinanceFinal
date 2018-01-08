using System;
using System.Net.Http;
using System.Web.Http;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Gyrkin.Controllers.Api.Models;

namespace Falcon.Gyrkin.Controllers.Api
{
  public sealed class UserTrackingController : ApiController
  {

    #region Injections

    IUserTrackingRepository _userTrackingRepository;

    #endregion
    public UserTrackingController(IUserTrackingRepository userTrackingRepository)
    {
      _userTrackingRepository = userTrackingRepository;
    }

    [HttpPost]
    public HttpResponseMessage TrackUser([FromBody] UserTrackingModel.TrackUserModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _userTrackingRepository.GetUserMovements(model.UserId, model.BranchId, model.StartDate, model.EndDate));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage TrackBranch([FromBody] UserTrackingModel.TrackBranchModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _userTrackingRepository.GetBranchMovements(model.BranchId, model.StartDate, model.EndDate));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage SavePin([FromBody] UserTrackingModel.TrackUserSavePinModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _userTrackingRepository.SavePin(model.UserId, (Tracking.AlertType)model.AlertType, (Tracking.SeverityClassification)model.Severity,
           (Tracking.Elapse)model.Elapse, model.Value, model.Notify));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetPinned([FromBody] UserTrackingModel.PinnedUserQueryModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _userTrackingRepository.GetPinned(model.Active));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }
    [HttpPost]
    public HttpResponseMessage RemovePin([FromBody] UserTrackingModel.RemovePinnedQueryModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _userTrackingRepository.RemovePin(model.PinnedUserId));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }
  }
}
