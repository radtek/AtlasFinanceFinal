using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atlas.Online.Web.Models
{
  public class MessageBoxModel
  {
    public enum IconType
    {
      Clock,
      Cross,
      Face,
      Lock,
      Tick,
      TickGreen
    }

    public enum AlertType
    {
      None,
      Success,
      Danger,
      Error,
      Info
    }

    public enum MessageType
    {
      PasswordReset_Sent,
      PasswordReset_Success,

      Application_Declined,
      Application_Review,
      Application_Complete,
      Application_Pending,
      Application_TechnicalError,

      Application_PendingBankCutOff,
      Application_PendingNextWorkingDay,
      Application_PendingHoliday,

      Contact_MessageSent,
      Question_MessageSent,
    }

    public class Button
    {
      public string Text { get; set; }
      public string Url { get; set; }

      public MvcHtmlString CustomHtml { get; set; }
      public string Classes { get; set; }
    }

    private string _headingClasses = null;

    public IconType Icon { get; set; }

    public AlertType? Alert { get; set; }

    public string Title { get; set; }

    public string Heading { get; set; }
    public string HeadingClasses { get
    {
      if (_headingClasses == null)
      {
        return "fs-zeta";
      }

      return _headingClasses;
    }
      set 
      {
        _headingClasses = value;
      }
    }

    public string Message { get; set; }

    public System.Web.Mvc.MvcHtmlString HtmlMessage { get; set; }

    public Button ButtonLeft { get; set; }
    public Button ButtonRight { get; set; }

    public string MessageClasses { get; set; }

  }
}