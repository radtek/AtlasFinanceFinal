using System;
using System.Net.Http;
using System.Web.Http;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Gyrkin.Controllers.Api.Models;
using Serilog;

namespace Falcon.Gyrkin.Controllers.Api
{
  public sealed class BranchController : ApiController
  {
    #region Injections

    readonly ICompanyRepository _companyRepository;
      private readonly ILogger _logger;

      #endregion
    public BranchController(ICompanyRepository companyRepository, ILogger logger)
    {
        _companyRepository = companyRepository;
        _logger = logger;
    }

      [HttpPost]
    public HttpResponseMessage Get()
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _companyRepository.GetActiveBranches());
      }
      catch (Exception ex)
      {
          _logger.Error(string.Format("BranchController - Get: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage AssociateUser(BranchModels.AssociateUserModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _companyRepository.AssociateUser(model.BranchId,model.PersonId));
      }
      catch (Exception ex)
      {
          _logger.Error(string.Format("BranchController - AssociateUser: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }
  }
}
