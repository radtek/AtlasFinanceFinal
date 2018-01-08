using Atlas.Enumerators;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Online.Web.Configuration;
using Mvc.Mailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Atlas.Online.Web.Mailers
{
  public class ApplicationMailer : MailerBase
  {
    public ApplicationMailer()
    {
      MasterName = "_MailerLayout";
    }

    public virtual MvcMailMessage DeclinedReasonReview(Client client, string refno, string review)
    {
      ViewBag.Client = client;
      ViewBag.Review = review;
      ViewBag.RefNo = refno;
      ViewBag.Name = String.Concat(client.Firstname, " ", client.Surname);

      var contact = client.Contacts.FirstOrDefault(x => x.ContactType.ContactTypeId == Convert.ToInt32(General.ContactType.Email));
      var fromEmail = contact != null ? contact.Value : string.Empty;

      return Populate(x =>
      {
        x.Subject = "Atlas Online - Declined Application Review";
        x.From = new MailAddress(fromEmail, ViewBag.Name);
        x.ViewName = "DeclinedReview";

        x.To.Add(AtlasConfig.General["SupportEmail"]);
      });
    }
  }
}