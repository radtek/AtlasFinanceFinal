using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Models;
using Atlas.Online.Web.WebService;
using Mvc.Mailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Atlas.Common.Extensions;

namespace Atlas.Online.Web.Mailers
{
  public class AccountMailer : MailerBase
  {
    public AccountMailer()
    {
      MasterName = "_MailerLayout";
    }

    public virtual MvcMailMessage ForgotPassword(string email, Client client, string tokenUrl)
    {

      using (var result = new WebServiceClient("WebServer.NET"))
      {
        result.NTF_ForgotPassword(client.Firstname, client.Surname, tokenUrl, email);
      }
      return null;
      //ViewBag.Client = client;
      //ViewBag.TokenUrl = tokenUrl;
      //return Populate(x =>
      //{
      //  x.Subject = "Atlas Online - Password Reset";
      //  x.From = new MailAddress("no-reply@atlasonline.co.za", "Atlas Online");
      //  x.ViewName = "ForgotPassword";

      //  x.To.Add(email);
      //});
    }

    public virtual MvcMailMessage NewRegistration(string email, Client client, string password)
    {

      using (var result = new WebServiceClient("WebServer.NET"))
      {
        result.NTF_Registration(client.Firstname, client.Surname, client.Contacts.FirstOrDefault(p => p.ContactType.ContactTypeId == Enumerators.General.ContactType.CellNo.ToInt()).Value,
          "http://www.atlasonline.co.za/blah", email);

      }
      return null;
      //ViewBag.Client = client;
      //ViewBag.Password = password;

      //return Populate(x =>
      //{
      //  x.Subject = "Welcome to Atlas Online";
      //  x.From = new MailAddress("no-reply@atlasonline.co.za", "Atlas Online");
      //  x.ViewName = "NewRegistration";

      //  x.To.Add(email);
      //});
    }
  }
}