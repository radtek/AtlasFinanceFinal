using System;
using System.Net.Http;
using System.Web.Http;
using Falcon.Common.Interfaces.Repositories;
using Falcon.Gyrkin.Controllers.Api.Models;

namespace Falcon.Gyrkin.Controllers.Api
{
  public sealed class DebitOrderController : ApiController
  {
    #region Injections

    readonly IDebitOrderRepository _debitOrderRepository;

    #endregion
    public DebitOrderController(IDebitOrderRepository debitOrderRepository)
    {
      _debitOrderRepository = debitOrderRepository;
    }

    [HttpPost]
    public HttpResponseMessage Batch(DebitOrderModel.BatchQueryModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _debitOrderRepository.GetBatches(model.BatchId, model.BranchId, model.BatchStatus, model.StartRange, model.EndRange, model.QueryBatchOnly));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }
    
    [HttpPost]
    public HttpResponseMessage Control([FromBody] DebitOrderModel.ControlQueryModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _debitOrderRepository.GetControl(model.controlId, model.specificRepetition));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    public HttpResponseMessage Controls([FromBody] DebitOrderModel.ControlsQueryModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _debitOrderRepository.GetControls(model.Host, model.BranchId, model.StartRange, model.EndRange, model.ControlOnly));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage AdditionalDebitOrder([FromBody] DebitOrderModel.AdditionalDebitOrderModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _debitOrderRepository.AddAdditionalDebitOrder(model.ControlId, model.Instalment, model.ActionDate, model.InstalmentDate));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }

    [HttpPost]
    public HttpResponseMessage AdditionalDebitOrder([FromBody] DebitOrderModel.CancelAdditionalDebitOrderModel model)
    {
      try
      {
        return Request.CreateResponse(System.Net.HttpStatusCode.OK, _debitOrderRepository.CancelAdditionalDebitOrder(model.ControlId,model.TransactionId));
      }
      catch (Exception ex)
      {
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }
  }
}
