using Atlas.Enumerators;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Data.Models.DTO;
using Atlas.Online.Web.Common.Serializers;
using Atlas.Online.Web.Models;
using Atlas.Online.Web.Models.Steps;
using Atlas.Online.Web.Resources;
using Atlas.Online.Web.Security;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace Atlas.Online.Web.Controllers
{
  [Authorize]
  public class MyAccountController : AppController
  {
    [AllowApiActions(typeof(Api.MyAccountController))]
    [AllowApiActions(typeof(Api.AccountsController), "EmailInUse", "CellInUse")]
    [AllowApiActions(typeof(Api.AvsController), "Status")]
    [AllowApiActions(typeof(Api.ApplicationController), "Get", "Current", "Repayment", "SubmitSettlement")]
    public ActionResult Index()
    {
      ViewBag.Client = CurrentClient;
      var application = CurrentClient.CurrentApplication;

      ViewBag.CanApply = application == null &&
        Services.WebServiceClient.APP_ApplyIn(CurrentClient.ClientId) <= 0;

      return View();
    }

    public ActionResult CurrentLoan()
    {
      var application = Application.GetFirstBy(Services.XpoUnitOfWork, x => x.IsCurrent && x.Client.UserId == WebSecurity.CurrentUserId);

      ViewBag.CurrentLoan = AutoMapper.Mapper.Map<Application, ApplicationDto>(application);
      ViewBag.Client = CurrentClient;
      ViewBag.Status = application != null ? (int)application.Status : 0;
      ViewBag.ApplicationId = application != null ? application.ApplicationId : 0;

      ViewBag.CanContinue = application != null && !application.IsFinal;

      return PartialView("_CurrentLoan");
    }

    public ActionResult LoanHistory()
    {
      var result = new XPQuery<Application>(Services.XpoUnitOfWork)
        .Where(x => x.Client.UserId == WebSecurity.CurrentUserId && !x.IsCurrent)
        .OrderByDescending(x => x.CreateDate).Take(20).ToList();

      ViewBag.Loans = AutoMapper.Mapper.Map<List<Application>, List<ApplicationDto>>(result);

      return PartialView("_LoanHistory");
    }

    public ActionResult LoanSettlements()
    {
      return PartialView("_LoanSettlements");
    }

    public ActionResult PersonalDetails()
    {
      ViewBag.Client = CurrentClient;

      return PartialView("_PersonalDetails", Atlas.Online.Web.Models.Dto.PersonalDetailsDto.Create(CurrentClient));
    }

    public ActionResult BankDetails()
    {
      var model = CurrentClient.BankDetails.FirstOrDefault(x => x.IsEnabled);
      ViewBag.AVS_BankDetailsProcessing = Messages.AVS_BankDetailsProcessing;
      return PartialView("_BankDetails", model != null ? Atlas.Online.Web.Models.Dto.BankDetailDto.Create(model) : null);
    }

    public ActionResult LoginDetails()
    {
      return PartialView("_LoginDetails", new LoginDetailsModel() { Email = User.Identity.Name });
    }

  }
}
