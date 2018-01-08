using Atlas.Online.Web.Models;
using Atlas.Online.Web.Extensions.Flash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Atlas.Online.Web.Security;
using Atlas.Online.Data.Models.Definitions;
using DevExpress.Xpo;
using WebMatrix.WebData;
using Atlas.Enumerators;
using Atlas.Online.Web.Models.Dto;
using Atlas.Online.Web.Models.Steps;
using Atlas.Online.Web.Resources;
using System.Globalization;
using Atlas.Online.Web.Configuration;
using Atlas.Online.Web.Mailers;

namespace Atlas.Online.Web.Controllers
{
  [Authorize]
  public class ApplicationController : AppController
  {
    private Application _application = null;

    public Application CurrentApplication
    {
      get
      {
        if (_application == null)
        {
          _application = Application.GetFirstBy(
            Services.XpoUnitOfWork,
            x => x.IsCurrent &&
              x.Client.UserId == WebSecurity.CurrentUserId &&
              x.Status == Account.AccountStatus.Inactive
        );
        }

        return _application;
      }
    }

    // GET: /Application/
    [AllowApiActions(typeof(Api.ApplicationStepController))]
    [AllowApiActions(typeof(Api.LoanController), "Get")]
    [AllowApiActions(typeof(Api.AvsController))]
    public ActionResult Index()
    {
      if (!CurrentClient.OTPVerified)
      {
        return RedirectToAction("OTP", "Account");
      }

      var sliderResult = LoanDto.FromCookie(Request);

      // Check for existing application
      var application = CurrentClient.CurrentApplication;

      // If it doesn't exist or has expired, create a new one
      if (application == null || application.Expired)
      {
        if (Services.WebServiceClient.APP_ApplyIn(CurrentClient.ClientId) > 0)
        {
          return RedirectToAction("Index", "Home").WithFlash(new { notice = ErrorMessages.Application_CannotApply });
        }

        if (sliderResult == null)
        {
          var rules = Services.WebServiceClient.APP_SliderRules(CurrentClient.ClientId);
          sliderResult = new LoanDto()
          {
            Amount = rules.MaxLoanAmount,
            Period = rules.MaxLoanPeriod            
          };
        }

        application = new Application(Services.XpoUnitOfWork);
        sliderResult.SetApplication(ref application);

        application.Client = CurrentClient;
        application.IsCurrent = true;
        application.Status = Account.AccountStatus.Inactive;
        application.Step = 1;
        application.CreateDate = DateTime.Now;
        application.BankDetail = CurrentClient.BankDetails.FirstOrDefault(x => x.IsEnabled);

        application.Save();
        Services.XpoUnitOfWork.CommitChanges();
      }
      else if (sliderResult != null)
      {
        if (application.IsFinal)
        {
          return RedirectToAction("Index", "MyAccount").WithFlash(new { notice = ErrorMessages.Application_AlreadyPending });
        }

        // If one exists, update it with the new slider values if able
        if (application.Affordability == null && application.Step < Convert.ToInt32(ApplicationStep.Verify))
        {
          sliderResult.SetApplication(ref application);
          application.Save();
          Services.XpoUnitOfWork.CommitChanges();
        }        
      }

      if (application.CurrentStep == ApplicationStep.QuoteAcceptance)
      {
        return RedirectToAction("QuoteAcceptance", "Application", new { id = application.ApplicationId });
      }

      ViewBag.StepId = application.Step;

      return View();
    }

    [AllowApiActions(typeof(Api.QuoteAcceptanceController))]
    public ActionResult QuoteAcceptance(long? id)
    {
      if (!id.HasValue)
      {
        return HttpNotFound();
      }

      var application = CurrentClient.CurrentApplication;

      if (application == null)
      {
        return RedirectToAction("Index", "MyAccount").WithFlash(new { error = ErrorMessages.Application_NotFound });
      }

      if (application.IsFinal)
      {
        return RedirectToAction("Index", "MyAccount").WithFlash(new { error = ErrorMessages.Application_AlreadyComplete });
      }

      if (application.CurrentStep != ApplicationStep.QuoteAcceptance)
      {
        return RedirectToAction("Index", "Application");
      }
      
      if (application.Affordability == null) 
      {
        return RedirectToAction("Index", "Application").WithFlash(new { error = ErrorMessages.Application_CompleteFirst });
      }

      if (application.Status == Account.AccountStatus.Inactive && !Services.WebServiceClient.QTE_PreApprove(id.Value))
      {
        return RedirectToAction("Index", "Message", new { t = MessageBoxModel.MessageType.Application_TechnicalError });
      }

      ViewBag.ApplicationId = application.ApplicationId;

      var quote = Services.WebServiceClient.QTE_GetQuote(id.Value);
      
      var model = new QuoteAcceptance();
      model.Populate(quote);

      return View(model);
    }

    [AllowApiActions(typeof(Api.VerificationQuestionsController))]
    public ActionResult Verification(long? id)
    {
      var application = CurrentClient.CurrentApplication;
      if (application == null || application.ApplicationId != id)
      {
        throw new InvalidOperationException("Client or Application not found.");
      }

      if (application.IsFinal)
      {
        return RedirectToAction("Index", "MyAccount").WithFlash(new { Info = "Your application is already complete." });
      }

      if (application.CurrentStep == ApplicationStep.QuoteAcceptance)
      {
        return RedirectToAction("QuoteAcceptance", "Application", new { id = id });
      }

      ViewBag.Verification_QuestionsFailed = ErrorMessages.Verification_QuestionsFailed;

      ViewBag.ApplicationId = id;
      
      return View();
    }

    [AllowApiActions(typeof(Api.AffordabilityController))]
    public ActionResult Affordability(long? id)
    {
      if (!id.HasValue)
      {
        return HttpNotFound();
      }

      // Check that id matches application for the current user
      var application = Application.GetFirstBy(Services.XpoUnitOfWork, x => x.ApplicationId == id && x.Client.UserId == WebSecurity.CurrentUserId);
      if (application == null || application.Expired || !application.IsCurrent || application.Status != Account.AccountStatus.Inactive)
      {
        return RedirectToAction("Index", "MyAccount").WithFlash(new { error = String.Format(ErrorMessages.Application_Expired, 7) });
      }

      if (application.Affordability != null && application.Affordability.Accepted)
      {
        return RedirectToAction("Verification", "Application", new { id = id });
      }      

      // Get affordability
      var affordability = Services.WebServiceClient.APP_GetAffordability(application.ApplicationId);

      ViewBag.Data = new { AppId = application.ApplicationId };

      return View(new AffordabilityDto()
      {
        Loan = new LoanDto(application),
        Afford = affordability
      });
    }

    public ActionResult Declined(long? id)
    {
      var application = Application.GetFirstBy(Services.XpoUnitOfWork,
        x => x.ApplicationId == id &&
          x.Client.UserId == WebSecurity.CurrentUserId
      );

      if (application == null)
      {
        return RedirectToAction("Index", "Home").WithFlash(new { Error = ErrorMessages.Application_NotFound });
      }

      var obj = new MessageBoxModel()
      {
        HtmlMessage = new MvcHtmlString(
          String.Format(CultureInfo.CurrentCulture,
            Resources.Messages.ResourceManager.GetString(MessageBoxModel.MessageType.Application_Declined.ToString()),
            application.AccountNo
          )
        ),
        Title = "Declined",
        Icon = MessageBoxModel.IconType.Cross,

        MessageClasses = "fs-zeta fw-b",

        ButtonRight = new MessageBoxModel.Button()
        {
          Text = "View possible reason",
          Url = Url.Action("DeclinedReasons", "Application", new { id = application.ApplicationId})
        }
      };

      return View("MessageBox", obj);
    }

    public ActionResult Review(long? id)
    {
      var application = Application.GetFirstBy(Services.XpoUnitOfWork,
        x => x.ApplicationId == id &&
          x.Client.UserId == WebSecurity.CurrentUserId
      );

      if (application == null)
      {
        return RedirectToAction("Index", "Home").WithFlash(new { Error = ErrorMessages.Application_NotFound });
      }

      var obj = new MessageBoxModel()
      {
        HtmlMessage = new MvcHtmlString(
          String.Format(CultureInfo.CurrentCulture,
            Resources.Messages.ResourceManager.GetString(MessageBoxModel.MessageType.Application_Review.ToString()),
            application.AccountNo
          )
        ),
        Title = "Review",
        Icon = MessageBoxModel.IconType.Lock,

        MessageClasses = "fs-zeta fw-b",

        //ButtonRight = new MessageBoxModel.Button()
        //{
        //  Text = "View possible reason",
        //  Url = Url.Action("DeclinedReasons", "Application", new { id = application.ApplicationId })
        //}
      };

      return View("MessageBox", obj);
    }

    public ActionResult PendingBankCutOff(long? id)
    {
      var application = Application.GetFirstBy(Services.XpoUnitOfWork,
        x => x.ApplicationId == id &&
          x.Client.UserId == WebSecurity.CurrentUserId
      );

      if (application == null)
      {
        return RedirectToAction("Index", "Home").WithFlash(new { Error = ErrorMessages.Application_NotFound });
      }

      var obj = new MessageBoxModel()
      {
        HtmlMessage = new MvcHtmlString(
          String.Format(CultureInfo.CurrentCulture,
            Resources.Messages.ResourceManager.GetString(MessageBoxModel.MessageType.Application_PendingBankCutOff.ToString()),
            application.AccountNo
          )
        ),
        Title = "Pending",
        Icon = MessageBoxModel.IconType.Clock,

        MessageClasses = "fs-zeta fw-b",

        //ButtonRight = new MessageBoxModel.Button()
        //{
        //  Text = "View possible reason",
        //  Url = Url.Action("DeclinedReasons", "Application", new { id = application.ApplicationId })
        //}
      };

      return View("MessageBox", obj);
    }


    public ActionResult PendingNextWorkingDay(long? id)
    {
      var application = Application.GetFirstBy(Services.XpoUnitOfWork,
        x => x.ApplicationId == id &&
          x.Client.UserId == WebSecurity.CurrentUserId
      );

      if (application == null)
      {
        return RedirectToAction("Index", "Home").WithFlash(new { Error = ErrorMessages.Application_NotFound });
      }

      var obj = new MessageBoxModel()
      {
        HtmlMessage = new MvcHtmlString(
          String.Format(CultureInfo.CurrentCulture,
            Resources.Messages.ResourceManager.GetString(MessageBoxModel.MessageType.Application_PendingNextWorkingDay.ToString()),
            application.AccountNo
          )
        ),
        Title = "Pending",
        Icon = MessageBoxModel.IconType.Clock,

        MessageClasses = "fs-zeta fw-b",

        //ButtonRight = new MessageBoxModel.Button()
        //{
        //  Text = "View possible reason",
        //  Url = Url.Action("DeclinedReasons", "Application", new { id = application.ApplicationId })
        //}
      };

      return View("MessageBox", obj);
    }

    public ActionResult PendingHoliday(long? id)
    {
      var application = Application.GetFirstBy(Services.XpoUnitOfWork,
        x => x.ApplicationId == id &&
          x.Client.UserId == WebSecurity.CurrentUserId
      );

      if (application == null)
      {
        return RedirectToAction("Index", "Home").WithFlash(new { Error = ErrorMessages.Application_NotFound });
      }

      var obj = new MessageBoxModel()
      {
        HtmlMessage = new MvcHtmlString(
          String.Format(CultureInfo.CurrentCulture,
            Resources.Messages.ResourceManager.GetString(MessageBoxModel.MessageType.Application_PendingHoliday.ToString()),
            application.AccountNo
          )
        ),
        Title = "Pending",
        Icon = MessageBoxModel.IconType.Clock,

        MessageClasses = "fs-zeta fw-b",

        //ButtonRight = new MessageBoxModel.Button()
        //{
        //  Text = "View possible reason",
        //  Url = Url.Action("DeclinedReasons", "Application", new { id = application.ApplicationId })
        //}
      };

      return View("MessageBox", obj);
    }

    public ActionResult TechnicalError(long? id)
    {
      var application = Application.GetFirstBy(Services.XpoUnitOfWork,
        x => x.ApplicationId == id &&
          x.Client.UserId == WebSecurity.CurrentUserId
      );

      if (application == null)
      {
        return RedirectToAction("Index", "Home").WithFlash(new { Error = ErrorMessages.Application_TechnicalError });
      }

      var obj = new MessageBoxModel()
      {
        HtmlMessage = new MvcHtmlString(
          String.Format(CultureInfo.CurrentCulture,
            ErrorMessages.Application_TechnicalError,
            application.AccountNo
          )
        ),
        Title = "Technical Error",
        Icon = MessageBoxModel.IconType.Cross,
        MessageClasses = "fs-zeta fw-b",

        ButtonRight = new MessageBoxModel.Button()
        {
          Text = "My Account",
          Url = Url.Action("Index", "MyAccount")
        }
      };

      return View("MessageBox", obj);
    }

    public ActionResult Complete(long? id)
    {
      var application = Application.GetFirstBy(Services.XpoUnitOfWork,
        x => x.ApplicationId == id &&
          x.Client.UserId == WebSecurity.CurrentUserId
      );

      if (application == null)
      {
        return RedirectToAction("Index", "Home").WithFlash(new { Error = ErrorMessages.Application_NotFound });
      }

      var obj = new MessageBoxModel()
      {
        HtmlMessage = new MvcHtmlString(
          String.Format(CultureInfo.CurrentCulture,
            Resources.Messages.ResourceManager.GetString(MessageBoxModel.MessageType.Application_Complete.ToString()),
            application.ApplicationId
          )
        ),
        Title = "Complete",
        Icon = MessageBoxModel.IconType.TickGreen,

        MessageClasses = "fs-zeta fw-b",

        ButtonLeft = new MessageBoxModel.Button()
        {
          Text = "Logout",
          CustomHtml = MvcHtmlString.Create(RenderRazorViewToString("_LogoutPartial", new MessageBoxModel.Button() { Classes = "btn btn-left d-block" }))
        },

        ButtonRight = new MessageBoxModel.Button()
        {
          Text = "My Account",
          Url = Url.Action("Index", "MyAccount")
        }
      };

      return View("MessageBox", obj);
    }

    public ActionResult Pending(long? id)
    {
      var application = Application.GetFirstBy(Services.XpoUnitOfWork,
         x => x.ApplicationId == id &&
           x.Client.UserId == WebSecurity.CurrentUserId
       );

      if (application == null)
      {
        return RedirectToAction("Index", "Home").WithFlash(new { Error = ErrorMessages.Application_NotFound });
      }

      ViewBag.LogoutButtonClasses = "btn btn-left";

      var obj = new MessageBoxModel()
      {
        HtmlMessage = new MvcHtmlString(
          String.Format(CultureInfo.CurrentCulture,
            Resources.Messages.ResourceManager.GetString(MessageBoxModel.MessageType.Application_Pending.ToString()),
            application.ApplicationId
          )
        ),
        Title = "Pending",
        Icon = MessageBoxModel.IconType.Clock,

        MessageClasses = "fs-zeta fw-b",

        ButtonLeft = new MessageBoxModel.Button()
        {
          Text = "Logout",
          CustomHtml = MvcHtmlString.Create(RenderRazorViewToString("_LogoutPartial", new MessageBoxModel.Button() { Classes = "btn btn-left d-block" }))
        },

        ButtonRight = new MessageBoxModel.Button()
        {
          Text = "My Account",
          Url = Url.Action("Index", "MyAccount")
        }
      };

      return View("MessageBox", obj);
    }

    [HttpGet]
    public ActionResult DeclinedReasons(long? id)
    {
      var reason = Services.WebServiceClient.APP_GetDeclinedReason(id.Value);
      if (reason == null)
      {
        return RedirectToAction("Index", "Home").WithFlash(new { error = "Invalid Application." });
      }

      var application = Application.GetFirstBy(Services.XpoUnitOfWork,
        x => x.ApplicationId == id &&
          x.Client.UserId == WebSecurity.CurrentUserId
      );

      ViewBag.Reason = reason.Reason;
      ViewBag.RefNo = application != null ? application.AccountNo : string.Empty;
      ViewBag.ApplicationId = id.Value;

      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult DeclinedReasons(long? id, string review)
    {
      var application = Application.GetFirstBy(Services.XpoUnitOfWork,
        x => x.ApplicationId == id &&
          x.Client.UserId == WebSecurity.CurrentUserId
      );
      ViewBag.Submitted = true;
      ViewBag.RefNo = application != null ? application.AccountNo : string.Empty;

      if (ModelState.IsValid)
      {
        new ApplicationMailer().DeclinedReasonReview(CurrentClient, ViewBag.RefNo, review).Send();
      }

      return DeclinedReasons(id);
    }

    public ActionResult PersonalDetails()
    {
      SetViewMessages();

      ViewBag.Client = CurrentClient;

      ViewBag.CurrentStep = CurrentApplication.Step;

      return PartialView("_PersonalDetails");
    }

    public ActionResult EmployerDetails()
    {
      return PartialView("_EmployerDetails");
    }

    public ActionResult IncomeExpenses()
    {
      return PartialView("_IncomeExpenses");
    }

    public ActionResult ConfirmVerify()
    {
      var application = CurrentApplication;
      if (application == null)
      {
        throw new InvalidOperationException(ErrorMessages.Application_NotFound);
      }
      
      SetViewMessages();

      ViewBag.Application_PassedCutOff = Messages.Application_PassedCutOff;

      ViewBag.PassedCutOff = DateTime.Now.Hour >= 15;

      return PartialView("_ConfirmVerify", ApplicationStepFactory.Create(typeof(ConfirmVerifyStep), application));
    }

    public ActionResult Verify()
    {
      var application = Application.GetForUser(
        Services.XpoUnitOfWork,
        WebSecurity.CurrentUserId,
        isCurrent: true
      );
      
      if (application == null)
      {
        return RedirectToAction("Index", "Home").WithFlash(new { Error = ErrorMessages.Application_NotFound });
      }

      ViewBag.TimeoutRedirect = string.Format("/Application/TechnicalError/{0}", application.ApplicationId);
      //{
      //  TimeoutRedirect = Url.Action("TechnicalError", "Application", new { id = application.ApplicationId })
      //};

      ViewBag.Application = application;

      return PartialView("_Verify");
    }

    public ActionResult QA_AdditionalInformation()
    {
      return View();
    }

    public ActionResult QA_TermsAndConditions()
    {
      return View();
    }

    public ActionResult QA_DirectDebitAuthorisation()
    {
      return View();
    }

    private void SetViewMessages()
    {
      ViewBag.AVS_BankDetailsProcessing = ActionName.Equals("ConfirmVerify")
        ? Messages.AVS_BankDetailsProcessingConfirmVerify
        : Messages.AVS_BankDetailsProcessing;
    }

  }
}
