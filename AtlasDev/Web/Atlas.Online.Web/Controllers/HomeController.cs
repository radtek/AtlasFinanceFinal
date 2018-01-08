using Atlas.Online.Web.Configuration;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Filters;
using Atlas.Online.Web.Extensions.Flash;
using Atlas.Online.Web.Models;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Atlas.Online.Web.Common.Extensions;
using Atlas.Online.Web.Security;
using Atlas.Online.Web.Mailers;
using Atlas.Online.Web.Resources;
using Atlas.Online.Data.Models.DTO;
using WebMatrix.WebData;

namespace Atlas.Online.Web.Controllers
{
  [AllowAnonymous]
  [CustomHandleError]
  public class HomeController : AppController
  {
    [AllowApiActions(typeof(Api.LoanController))]
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult About()
    {
      return View();
    }

    public ActionResult Questions()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Questions(ContactModel model)
    {
      if (ModelState.IsValid)
      {
        new ContactMailer().Contact(AtlasConfig.General["QuestionsEmail"], model, "Questions").SendAsync();

        return RedirectToAction("Index", "Message", new { t = MessageBoxModel.MessageType.Question_MessageSent });
      }

      return View();
    }

    public ActionResult TermsAndConditions()
    {
      return View();
    }

    public ActionResult PrivacyPolicy()
    {
      return View();
    }

    public ActionResult Contact()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Contact(ContactModel model)
    {
      if (ModelState.IsValid)
      {
        new ContactMailer().Contact(AtlasConfig.General["ContactEmail"], model).SendAsync();

        return RedirectToAction("Index", "Message", new { t = MessageBoxModel.MessageType.Contact_MessageSent });
      }

      return View();
    }

    public ActionResult SiteSurvey()
    {
      if (HasClientVoted())
      {
        return View("SiteSurveyVoted");
      }

      var model = new SiteSurveyDto();
      if (CurrentClient != null)
      {
        model.Name = CurrentClient.FullName;
        model.Email = WebSecurity.CurrentUserName;

      }

      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult SiteSurvey(SiteSurveyDto survey)
    {
      ViewBag.HasClientVoted = HasClientVoted();

      if (ModelState.IsValid && !ViewBag.HasClientVoted)
      {
        if (CurrentClient != null)
        {
          AutoMapper.Mapper.CreateMap<Client, ClientDto>();
          AutoMapper.Mapper.Map(CurrentClient, survey.Client);
        }

        SiteSurvey surveyModel = new SiteSurvey(Services.XpoUnitOfWork);
        AutoMapper.Mapper.CreateMap<SiteSurveyDto, SiteSurvey>()
          .ForMember(x => x.Client, opt => opt.Ignore());
        AutoMapper.Mapper.Map(survey, surveyModel);

        surveyModel.CreateDate = DateTime.Now;
        surveyModel.Client = CurrentClient;

        surveyModel.Save();

        Response.SetCookie(new HttpCookie(SiteSurveyDto.VOTED_COOKIE, surveyModel.SiteSurveyId.ToString()));

        ViewBag.HasClientVoted = true;

        Services.XpoUnitOfWork.CommitChanges();
      }

      return SiteSurvey();
    }

    private bool HasClientVoted()
    {
      var cookie = Request.Cookies[SiteSurveyDto.VOTED_COOKIE];

      return cookie != null || (
        CurrentClient != null &&
        Atlas.Online.Data.Models.Definitions.SiteSurvey.Exists(Services.XpoUnitOfWork, CurrentClient.ClientId)
      );
    }
  }
}
