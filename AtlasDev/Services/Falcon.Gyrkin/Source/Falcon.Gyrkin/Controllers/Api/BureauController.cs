using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Atlas.Enumerators;
using Falcon.Gyrkin.Controllers.Api.Models;
using Falcon.TBR.Bureau.Interfaces;
using Serilog;
using Stream.Framework.Repository;

namespace Falcon.Gyrkin.Controllers.Api
{
  public sealed class BureauController : ApiController
  {
    #region Injections

    readonly IBureauRepository _bureauRepository;
    private readonly IStreamRepository _streamRepository;
    readonly ILogger _logger;

    #endregion

    public BureauController(IBureauRepository bureauRepository, IStreamRepository streamRepository, ILogger logger)
    {
      _bureauRepository = bureauRepository;
      _streamRepository = streamRepository;
      _logger = logger;
    }

    [HttpPost]
    public HttpResponseMessage GetRecentScore(BureauModels.ScoreModel model)
    {
      try
      {
        var debtor = _streamRepository.GetDebtorById(model.DebtorId);
        if (debtor == null)
        {
          return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest,
            "Cannot get score for debtor - does not exist.");
        }

        var residentialAddress =
          debtor.Addresses.FirstOrDefault(
            p => p.AddressType != null && p.AddressType.Type == General.AddressType.Residential);

        var result = _bureauRepository.GetScore(debtor.FirstName, debtor.LastName, debtor.IdNumber, residentialAddress,
          debtor.Contacts.FirstOrDefault(
            p => p.ContactType != null && p.ContactType.Type == General.ContactType.CellNo),
          debtor.Contacts.FirstOrDefault(
            p => p.ContactType != null && p.ContactType.Type == General.ContactType.TelNoHome),
          debtor.Contacts.FirstOrDefault(
            p => p.ContactType != null && p.ContactType.Type == General.ContactType.TelNoWork),
          model.BranchId,
          model.NewScore &&
          _streamRepository.DoesBudgetAllow(Stream.Framework.Enumerators.Stream.BudgetType.CompuscanEnquiries));

        if (result == null)
        {
          return Request.CreateResponse(System.Net.HttpStatusCode.BadRequest,
            "Either an error ocurred, or budget has been exceeded.");
        }

        _streamRepository.IncBudget(Stream.Framework.Enumerators.Stream.BudgetType.CompuscanEnquiries);

        return Request.CreateResponse(System.Net.HttpStatusCode.OK, result);
      }
      catch (Exception ex)
      {
        _logger.Error(string.Format("BureauController - GetRecentScore: {0}, {1}", ex.Message, ex.StackTrace));
        return Request.CreateErrorResponse(System.Net.HttpStatusCode.InternalServerError, ex);
      }
    }
  }
}
