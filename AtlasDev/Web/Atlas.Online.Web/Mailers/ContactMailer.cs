using Atlas.Online.Web.Models;
using Mvc.Mailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Atlas.Online.Web.Mailers
{
  public class ContactMailer : MailerBase
  {
    public ContactMailer()
    {
      MasterName = "_MailerLayout";
    }

    public virtual MvcMailMessage Contact(string to, ContactModel contact, string viewName = "Contact")
    {
      ViewData = new System.Web.Mvc.ViewDataDictionary(contact);

      return Populate(x =>
      {
        x.Subject = String.Format("Atlas Online - Contact from '{0}'", contact.Email);
        x.From = new MailAddress("no-reply@atlasonline.co.za", "Atlas Online");
        x.ViewName = viewName;

        x.To.Add(to);
      });
    }
  }
}