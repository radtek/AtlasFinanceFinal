using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Atlas.Common.Extensions;
using Falcon.Common.Interfaces.Structures;
using Falcon.Gyrkin.Controllers.Api.Models;
using Serilog;
using Stream.Framework.Repository;
using Stream.Framework.Services;
using Stream.Framework.Structures;

namespace Falcon.Gyrkin.Controllers.Api
{
  public class StreamController : ApiController
  {
    #region Injections

    // TODO: remove repository access from controller and use stream service
    private readonly IStreamRepository _streamRepository;
    private readonly IStreamService _streamService;
    private readonly ILogger _logger;

    #endregion

    public StreamController(IStreamRepository streamRepository, IStreamService streamService, ILogger logger)
    {
      _streamRepository = streamRepository;
      _streamService = streamService;
      _logger = logger;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> GetStaticData(StreamModel.GetStaticDataModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          return Request.CreateResponse(System.Net.HttpStatusCode.OK,
            new
            {
              caseStatuses =
                _streamRepository.GetCaseStatuses()
                  .Select(c => new { CaseStatusId = (int)c, Description = c.ToStringEnum() }),
              streamTypes = _streamRepository.GetStreamTypes(model.GroupType),
              comments = _streamRepository.GetCommentsByStreamGroup(model.GroupType)
            });
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - GetStaticData: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> GetWorkItems(StreamModel.GetWorkItemsQueryModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          var workItems = _streamRepository.GetWorkItems(
            groupTypes: new[] {model.GroupType},
            caseStatuses: model.CaseStatus.HasValue ? new[] {model.CaseStatus} : null, branchId: model.BranchId,
            allocatedPersonId: model.AllocatedPersonId,
            allocatedUserId: model.AllocatedUserId, bufferTime: null, completeDate: model.CompleteDate,
            caseId: model.CaseId, idNumber: model.IdNumber, accountReference: model.AccountReferenceNo,
            actionDateStartRange: model.ActionStartDate, actionDateEndRange: model.ActionEndDate, limitResults: 100,
            createDateStartRange: model.CreateStartDate, createDateEndRange: model.CreateEndDate,
            streamTypes: model.StreamType).Select(c => new
            {
              c.CaseId,
              c.CaseStreamId,
              c.CaseStreamActionId,
              c.AccountReference,
              c.DebtorIdNumber,
              c.DebtorFullName,
              c.Priority,
              c.CaseStatus,
              c.AllocatedUserId,
              c.AllocatedUserFullName,
              c.StreamId,
              c.ActionTypeId,
              c.Category,
              c.SubCategory,
              c.LoanAmount,
              c.ArrearsAmount,
              c.Balance,
              c.ActionDate,
              c.NoActionCount,
              Warning = c.ActionDate < DateTime.Today,
              Danger = c.ActionDate < DateTime.Today.AddDays(-1)
            });
          return Request.CreateResponse(System.Net.HttpStatusCode.OK, workItems);
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - GetWorkItems: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> GetWorkItem(StreamModel.GetWorkItemModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          var workItem = _streamRepository.GetWorkItem(model.CaseStreamActionId, model.UserId);
          return Request.CreateResponse(System.Net.HttpStatusCode.OK,
            new { Case = workItem.Item1, Notes = workItem.Item2, PreviousCaseCompleteNote = workItem.Item3 });
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - GetWorkItem: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> GetNoteHistory(StreamModel.GetNoteHistoryModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          var workItem = _streamRepository.GetNoteHistory(model.CaseId);
          return Request.CreateResponse(System.Net.HttpStatusCode.OK,
            new { Notes = workItem.Item1, PreviousCaseCompleteNote = workItem.Item2 });
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - GetNoteHistory: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> GetNextWorkItem(StreamModel.GetNextWorkItemModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          var workItem = _streamRepository.GetNextWorkItem(model.GroupType, model.PersonId, model.StreamType);
          if (workItem == null)
            return Request.CreateResponse(System.Net.HttpStatusCode.OK,
              (Tuple<IWorkItem, List<INote>, string>)null);

          return Request.CreateResponse(System.Net.HttpStatusCode.OK,
            new { Case = workItem.Item1, Notes = workItem.Item2, PreviousCaseCompleteNote = workItem.Item3 });
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - GetNextWorkItem: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> Save(StreamModel.SaveModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          switch (model.StreamType)
          {
            case StreamModel.StreamAction.PTP:
              _streamRepository.MoveCaseToPtpStream(model.CaseStreamId, model.UserId, model.Comment, model.Note,
                decimal.Parse(model.Amount, CultureInfo.InvariantCulture), new[] { model.Date });
              break;
            case StreamModel.StreamAction.PTPReschedule:
              _streamRepository.MoveCaseToPtpStream(model.CaseStreamId, model.UserId, model.Comment, model.Note,
                decimal.Parse(model.Amount, CultureInfo.InvariantCulture), new[] { model.Date });
              break;
            case StreamModel.StreamAction.FollowUp:
              _streamRepository.MoveCaseToFollowUpStream(model.CaseStreamId, model.UserId, model.Comment, model.Note,
                model.Date);
              break;
            case StreamModel.StreamAction.PTC:
              _streamRepository.MoveCaseToPtcStream(model.CaseStreamId, model.UserId, model.Comment, model.Note,
                model.Date);
              break;
            case StreamModel.StreamAction.NoAction:
              _streamRepository.NoActionOnCaseStream(model.CaseStreamId, model.ActionId, model.UserId, model.Comment);
              break;
            case StreamModel.StreamAction.ForceClose:
              _streamRepository.ForceCloseCase(model.CaseStreamId, model.Comment);
              break;
            case StreamModel.StreamAction.PTCUnchanged:
            case StreamModel.StreamAction.PTPUnchanged:
              _streamRepository.MarkActionReminderAsComplete(model.ActionId, model.UserId, model.Comment);
              break;
          }

          return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Saved");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - Save: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }

      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> SaveOutcome(StreamModel.OutcomeModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          switch (model.Outcome)
          {
            case StreamModel.Outcome.Yes:
              _streamRepository.MovePtcCaseToComplete(model.CaseStreamId, model.UserId, model.Comment);
              break;
            case StreamModel.Outcome.No:
              _streamRepository.MovePtcCaseToPtcBrokenStream(model.CaseStreamId, model.UserId, model.Comment, model.Note);
              break;
          }
          return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Saved");

        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - SaveOutcome: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> Escalate(StreamModel.EscalateModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          if (_streamRepository.EscalateCaseStream(model.CaseStreamId, model.UserId, model.Comment, model.EscalationType))
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Saved");
          else
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Error");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - Escalate: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> SaveContact(StreamModel.SaveContactModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          _streamRepository.AddContact(model.DebitorId, model.UserId, model.ContactType, model.No);
          return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Saved");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - SaveContact: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> SyncPayment(StreamModel.SyncPaymentModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          if (_streamService.CheckForPtpPayment(model.CaseStreamId))
          {
            return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Paid");

          }

          return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Pending");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - SyncPayment: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    public async Task<HttpResponseMessage> SaveNotInterested(StreamModel.SaveNotInterested model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          _streamRepository.PtcClientNotInterested(model.CaseStreamId, model.UserId, model.Note);
          return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Saved");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - SaveNotInterested: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> AddCaseComment(StreamModel.CaseCommentModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          var note = _streamRepository.AddAccountNote(model.UserId, model.Note, model.CaseId);
          return Request.CreateResponse(System.Net.HttpStatusCode.OK, note);
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - AddCaseComment: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> GetLetterOfDemandPDF(StreamModel.GetLetterOfDemandModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          var letterOfDemandPdf = _streamRepository.GetFinalLetterOfDemandPdf(model.CaseStreamId);
          return Request.CreateResponse(System.Net.HttpStatusCode.OK,
            new { response = Convert.ToBase64String(letterOfDemandPdf) });
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - GetLetterOfDemandPDF: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> GetEscalatedItems(StreamModel.EscalatedItemsModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          var escalatedWorkItems = _streamRepository.GetEscalatedWorkItemsOnly(model.AllocatedUserId, model.BranchId);
          foreach (var escalatedWorkItem in escalatedWorkItems)
          {
            if (escalatedWorkItem.CaseStreamAllocations.Count > 0)
            {
              escalatedWorkItem.AllocatedUserId = escalatedWorkItem.CaseStreamAllocations[0].AllocatedUserId;
              escalatedWorkItem.AllocatedUserFullName = escalatedWorkItem.CaseStreamAllocations[0].AllocatedUser;
            }
            escalatedWorkItem.Warning = escalatedWorkItem.ActionDate < DateTime.Today;
            escalatedWorkItem.Danger = escalatedWorkItem.ActionDate < DateTime.Today.AddDays(-1);

          }
          return Request.CreateResponse(System.Net.HttpStatusCode.OK,
            escalatedWorkItems);
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - GetEscalatedItems: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> EscalationReturn(StreamModel.EscalatedTransferBackModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          _streamRepository.MarkEscalationAsComplete(model.CaseStreamId, model.AllocatedUserId, model.CommentId);
          return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Complete");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - EscalationReturn: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> TransferToUser(StreamModel.ChangeAllocatedUserModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          _streamRepository.ChangeCaseStreamAllocatedUser(model.CaseStreamId, model.UserId, model.CurrentUserId, model.NewUserId);
          return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Changed");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - TransferToUser: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }

    [HttpPost]
    public async Task<HttpResponseMessage> TransferMultipleCaseStreamsToUser(
      StreamModel.ChangeMultipleCaseStreamAllocatedUserModel model)
    {
      var result = await Task.Run(() =>
      {
        try
        {
          foreach (var changeAllocatedUserModel in model.ChangeAllocatedUserModels)
          {
            try
            {
              //_streamRepository.MarkEscalationAsComplete(changeAllocatedUserModel.CaseStreamId, changeAllocatedUserModel.UserId);
            }
            catch (Exception exception)
            {
              _logger.Error(string.Format("StreamController - TransferMultipleCaseStreamsToUser Remove Escalation: {0}, {1}", exception.Message, exception.StackTrace));
            }
            _streamRepository.ChangeCaseStreamAllocatedUser(changeAllocatedUserModel.CaseStreamId,
              changeAllocatedUserModel.UserId, changeAllocatedUserModel.CurrentUserId,
              changeAllocatedUserModel.NewUserId);
          }
          return Request.CreateResponse(System.Net.HttpStatusCode.OK, "Changed");
        }
        catch (Exception ex)
        {
          _logger.Error(string.Format("StreamController - TransferMultipleCaseStreamsToUser: {0}, {1}", ex.Message, ex.StackTrace));
          return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
        }
      });
      return result;
    }
  }
}