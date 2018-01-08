using Atlas.Online.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Atlas.Online.Web.Controllers
{
  [AllowAnonymous]
  public class MessageController : AppController
  {
    public ActionResult Index(MessageBoxModel.MessageType t)
    {
      var model = MessageModelFromType(t);
      if (model == null)
      {
        return RedirectToAction("Index", "Home");
      }

      return View("MessageBox", model);
    }    

    private MessageBoxModel MessageModelFromType(MessageBoxModel.MessageType type)
    {
      var obj = new MessageBoxModel();

      var message = Resources.Messages.ResourceManager.GetString(type.ToString());

      if (message == null)
      {
        return null;
      }

      var parts = message.Split(new char[] { '|' }, 2);
      if (parts.Length > 0)
      {
        obj.Heading = parts[0];
      }

      if (parts.Length == 2)
      {
        obj.HtmlMessage = new MvcHtmlString(parts[1]);
      }

      switch (type)
      {
        case MessageBoxModel.MessageType.PasswordReset_Sent:
          obj.Title = "Password Reset";
          obj.Icon = MessageBoxModel.IconType.Clock;
          break;

        case MessageBoxModel.MessageType.PasswordReset_Success:
          obj.Title = "Password Reset";
          obj.Icon = MessageBoxModel.IconType.TickGreen;
          obj.ButtonRight = new MessageBoxModel.Button() { Text = "Login", Url = Url.Action("Login", "Account") };
          break;

        case MessageBoxModel.MessageType.Contact_MessageSent:
        case MessageBoxModel.MessageType.Question_MessageSent:
          obj.Title = "Message sent";
          obj.Icon = MessageBoxModel.IconType.TickGreen;
          obj.ButtonRight = new MessageBoxModel.Button() { Text = "Home", Url = Url.Action("Index", "Home") };
          break;
      }

      return obj;
    }
  }
}
