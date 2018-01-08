using System;
using System.Net.Http;
using System.Web.Http;
using Atlas.Enumerators;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Gyrkin.Controllers.Api.Models;
using Serilog;

namespace Falcon.Gyrkin.Controllers.Api
{
  public sealed class UserController : ApiController
  {
    #region Injections

    IUserRepository _userRepository;
    private readonly ILogger _logger;

    #endregion
    public UserController(IUserRepository userRepository, ILogger logger)
    {
      _userRepository = userRepository;
      _logger = logger;
    }

    [HttpPost]
    public HttpResponseMessage Get()
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _userRepository.GetUsers(General.PersonType.Employee));
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("UserController - Get: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    public HttpResponseMessage AssignUser(UserModel.AssignModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _userRepository.GetUsers(General.PersonType.Employee));
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("UserController - AssignUser: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }

    }

    [HttpPost]
    public HttpResponseMessage GetActiveUsers()
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _userRepository.GetActiveUsers());
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("UserController - GetActiveUsers: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }


    [HttpPost]
    public HttpResponseMessage CheckLink(UserModel.CheckLinkModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, (_userRepository.CheckLink(model.UserId)));
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("UserController - CheckLink: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage LinkUser(UserModel.LinkUserModel model)
    {
      try
      {
        _logger.Debug(string.Format("UserController - LinkUser [entry] : {0}, {1}", model.IDNo, model.UserId));
        if (_userRepository.LinkUser(model.IDNo, model.UserId))
          return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Linked");

        return Request.CreateResponse(System.Net.HttpStatusCode.NotFound, "NotLinked");
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("UserController - LinkUser: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage UnlinkUser(UserModel.LinkUserV2Model model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _userRepository.UnLinkUser(model.PersonId, model.UserId));
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("UserController - UnlinkUser: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage LinkUserV2(UserModel.LinkUserV2Model model)
    {
      try
      {
        _logger.Debug(string.Format("UserController - LinkUserV2 [entry] : {0}, {1}", model.PersonId, model.UserId));
        _userRepository.LinkUser(model.PersonId, model.UserId);
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Linked");

      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("UserController - LinkUserV2: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetConsultants(UserModel.ConsulantQueryModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _userRepository.GetConsultants(model.BranchId));
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("UserController - GetConsultants: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetLinkedBranches(UserModel.UserLinkedBranchesQueryModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _userRepository.GetLinkedBranches(model.UserId));
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("UserController - GetLinkedBranches: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }
    [HttpPost]
    public HttpResponseMessage List(UserModel.UserListQueryModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _userRepository.List(model.BranchId, model.FirstName, model.LastName, model.IdNo));
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("UserController - List: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage GetUserDetail(UserModel.CheckLinkModel model)
    {
      Console.WriteLine(DateTime.Now + " Entered login:" + model.UserId);
      try
      {
        var person = _userRepository.GetPerson(model.UserId);
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, person);
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("UserController - GetUserDetail: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
      finally
      {
        Console.WriteLine(DateTime.Now + " Left login:" + model.UserId);
      }
    }
  }
}