using System;
using System.Linq;
using System.IO;
using System.Net.Mail;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Collections.Concurrent;

using OpenPop.Mime;
using HtmlAgilityPack;
using OpenPop.Pop3;

using Atlas.Common.Interface;
using Atlas.ThirdParty.CS.Bureau.Client.ClientProxies;


namespace Atlas.Integration.Scheduler.QuartzTasks.CCEMails
{
  internal static class EMailUtils
  {
    /// <summary>
    /// Downloads and queues messages for processing
    /// </summary>
    internal static void POP3QueueNewMessages(ILogging log, IConfigSettings config)
    {
      try
      {
        using (var client = new Pop3Client())
        {
          log.Information("Connecting...");
          client.Connect(config.GetCustomSetting(null, "popServer", false), 110, false);

          log.Information("Logging in...");
          client.Authenticate(config.GetCustomSetting(null, "popUser", false), config.GetCustomSetting(null, "popPassword", false));

          log.Information("Checking messages...");
          var messageCount = client.GetMessageCount();

          if (messageCount > 0)
          {
            var maxMessages = config.GetCustomSettingInt(null, "popGetMax", false, 10);
            messageCount = Math.Min(messageCount, maxMessages);
            log.Information("Messages: {messageCount}", messageCount);

            // Messages are numbered in the interval: [1 - messageCount. Most servers give the latest message the highest number
            for (var i = 1; i <= messageCount; i++)
            {
              log.Information("Downloading message {message}", i);
              var message = client.GetMessage(i);

              log.Information("Queuing message {message}", i);
              QueueMessage(message);
            }
          }
          else
          {
            log.Information("  No messages");
          }
        }
      }
      catch (Exception err)
      {
        log.Error(err, "POP3QueueNewMessages");
      }
    }


    /// <summary>
    /// Processes queued messages
    /// </summary>
    /// <returns>List of messageIds successfully processed</returns>
    internal static List<string> ProcessQueued(ILogging log, IConfigSettings config)
    {
      var successful = new List<string>();
      foreach (var pending in _queued)
      {
        if (ProcessMessage(log, config, pending))
        {
          successful.Add(pending.Headers.MessageId);
        }
      }
      _queued.Clear();

      return successful;
    }


    /// <summary>
    /// Send all pending OutBox messages
    /// </summary>
    internal static void SendOutBox(ILogging log, IConfigSettings config)
    {
      if (_outBox.Any())
      {
        var failed = new List<MailMessage>();

        for (var i = 0; i < _outBox.Count; i++)
        {
          var message = _outBox[i];
          try
          {
            using (var smtp = new SmtpClient())
            {
              smtp.Host = config.GetCustomSetting(null, "smtpMailServer", false);// AppConfig.SMTPServerHost;
              smtp.UseDefaultCredentials = false;
              smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
              log.Information("Sending message {@to}...", message.To);

              // !!remove!!
              //message.To.Clear();
              //message.To.Add(new MailAddress("keith@atcorp.co.za"));

              smtp.Send(message);
              log.Information("  Sent");
            }
            message.Dispose(); // MailMessage implements IDisposable- must free any resources
          }
          catch (Exception err)
          {
            log.Error(err, "SendOutBox");
            failed.Add(message);
          }
        }

        _outBox.RemoveAll(s => !failed.Contains(s));
      }
    }


    /// <summary>
    /// Delete messages from POP3 with specific messageIds
    /// </summary>
    /// <param name="messageIds"></param>
    internal static void POP3DeleteMessages(ILogging log, IConfigSettings config, List<string> messageIds)
    {
      try
      {
        if (!messageIds.Any())
        {
          return;
        }

        using (var client = new Pop3Client())
        {
          client.Connect(config.GetCustomSetting(null, "popServer", false), 110, false);
          client.Authenticate(config.GetCustomSetting(null, "popUser", false), config.GetCustomSetting(null, "popPassword", false));
          var messageCount = client.GetMessageCount();

          // Run trough each of these messages and download the headers
          for (var messageItem = messageCount; messageItem > 0; messageItem--)
          {
            var messageId = client.GetMessageHeaders(messageItem).MessageId;
            if (messageIds.Contains(messageId))
            {
              log.Information("Deleting messageId: '{0}'", messageId);
              client.DeleteMessage(messageItem);
            }
          }
        }
      }
      catch (Exception err)
      {
        log.Error(err, "POP3DeleteMessages");
      }
    }


    /// <summary>
    /// process a pending message
    /// </summary>
    /// <param name="message"></param>
    private static bool ProcessMessage(ILogging log, IConfigSettings config, Message message)
    {
      log.Information("Processing: {subject} {from}", message.Headers.Subject, message.Headers.From.Address);

      var wasSentToInDox = false;
      var score = 0;
      string id = null;

      try
      {
        if (_lastDate.Date != DateTime.Now.Date)
        {
          LogProcessedAndClear(log);

          _todaySentCount = 0;
          _lastDate = DateTime.Now;
        }

        var maxPerDay = config.GetCustomSettingInt(null, "maxInDoxForwardsPerDay", false, int.MaxValue);

        if (_todaySentCount >= maxPerDay) // limit InDox forwarding per day...
        {
          throw new Exception("InDox daily limit reached");
        }

        #region Find and parse HTML message part
        var html = message.FindFirstHtmlVersion();
        if (html == null)
        {
          throw new Exception("Missing HTML body message part");
        }

        var parser = new HtmlDocument();
        using (var ms = new MemoryStream(html.Body))
        {
          parser.Load(ms, html.BodyEncoding);
        }
        if (parser.ParseErrors != null && parser.ParseErrors.Any())
        {
          throw new Exception("Failed to decode HTML message part");
        }
        #endregion

        #region Get name/id from HTML message part
        var personDetails = parser.DocumentNode.SelectSingleNode("//div[@id='page4']");
        if (personDetails == null)
        {
          throw new Exception("HTML Missing div id = 'page4' tag");
        }
        var nodes = personDetails.SelectNodes(".//div[@class='form_element cf_textbox']");
        if (nodes == null || !nodes.Any())
        {
          throw new Exception("page4 missing nodes");
        }

        var firstName = string.Empty;
        var initials = string.Empty;
        var surname = string.Empty;
        var email = string.Empty;
        foreach (var node in nodes)
        {
          var text = node.InnerHtml.ToLower();
          if (text.Contains("<strong>name</strong>"))
          {
            firstName = GetLastTextBit(text);
            if (!string.IsNullOrWhiteSpace(firstName))
            {
              firstName = firstName.Split(' ')[0];
            }
          }
          else if (text.Contains("<strong>initials</strong>"))
          {
            initials = GetLastTextBit(text);
          }
          else if (text.Contains("<strong>surname</strong>"))
          {
            surname = GetLastTextBit(text);
          }
          else if (text.Contains("<strong>id</strong>"))
          {
            id = Regex.Replace(GetLastTextBit(text), "[^0-9]", string.Empty);
            if (id.Length < 5 || id.Length > 13)
            {
              throw new Exception($"Invalid id number: '{id}'");
            }
          }
          else if (text.Contains("<strong>email</strong>"))
          {
            email = GetLastTextBit(text);
            try
            {
              new MailAddress(email);
            }
            catch
            {
              throw new Exception($"Invalid e-mail: {email}");
            }
          }
        }
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(surname))
        {
          throw new Exception($"Missing key fields- ID/surname: '{id}' / '{surname}'");
        }
        #endregion

        #region Pull scorecard
        Atlas.ThirdParty.CS.WCF.Interface.ScorecardSimpleResult scorecard = null;
        var attempts = 0;
        while (scorecard == null && ++attempts <= 3)
        {
          using (var client = new ScorecardClient())
          {
            scorecard = client.GetSimpleScorecard("0L7" /* KZN CC */, firstName, surname, id, false);
          }
        }

        if (scorecard == null)
        {
          // CompuScam issues- try later...
          log.Error("null response to scorecard request");
          return false;
        }

        if (!scorecard.Successful)
        {
          throw new Exception($"Invalid response to scorecard request: '{scorecard.Error}'");
        }

        score = scorecard.Score;
        if (scorecard.Score < 600)
        {
          throw new Exception($"Did not meet score minimum of 600: {scorecard.Score}");
        }

        var fourMonth = scorecard.AtlasProducts.FirstOrDefault(s => s.ProductType == "PROD4"); // 2-4 month
        {
          if (fourMonth == null || !fourMonth.Outcome)
          {
            throw new Exception("'2-4 month' product missing or failed");
          }
        }
        #endregion

        #region Telephone details
        var telDetails = parser.DocumentNode.SelectSingleNode("//div[@id='page4i']");
        if (telDetails == null)
        {
          throw new Exception("HTML Missing div id = 'page4i' tag");
        }
        var telNodes = telDetails.SelectNodes(".//div[@class='form_element cf_textbox']");
        if (telNodes == null || !telNodes.Any())
        {
          throw new Exception("page4i missing nodes");
        }

        var cellNum = string.Empty;
        var workTel = string.Empty;
        var homeTel = string.Empty;
        var faxTel = string.Empty;
        var telPreferred = string.Empty;
        foreach (var node in telNodes)
        {
          var text = node.InnerHtml.ToLower();
          if (text.Contains("<strong>daytime number you prefer</strong>"))
          {
            telPreferred = CleanTel(GetLastTextBit(text));
          }
          else if (text.Contains("<strong>tel w</strong>"))
          {
            workTel = CleanTel(GetLastTextBit(text));
          }
          else if (text.Contains("<strong>tel h</strong>"))
          {
            homeTel = CleanTel(GetLastTextBit(text));
          }
          else if (text.Contains("<strong>fax no.</strong>"))
          {
            faxTel = CleanTel(GetLastTextBit(text));
          }
          else if (text.Contains("<strong>cell no.</strong>"))
          {
            cellNum = CleanTel(GetLastTextBit(text));
          }
        }
        #endregion

        #region Get salary
        var payDetails = parser.DocumentNode.SelectSingleNode("//div[@id='page1']");
        string netSalaryVal = null;
        if (payDetails == null)
        {
          throw new Exception("HTML Missing div id = 'page1' tag");
        }
        var payNodes = payDetails.SelectNodes(".//div[@class='form_element cf_textbox']");
        foreach (var node in payNodes)
        {
          var text = node.InnerHtml.ToLower();
          if (text.Contains("<strong>basic salary (nett) as per payslip</strong>"))
          {
            netSalaryVal = GetLastTextBit(text);
          }
        }
        var netSalary = 0M;
        if (!string.IsNullOrEmpty(netSalaryVal))
        {
          if (!decimal.TryParse(netSalaryVal, NumberStyles.Number, CultureInfo.InvariantCulture, out netSalary) || netSalary < 1000 | netSalary > 150000)
          {
            throw new Exception($"Salary contains invalid value: {netSalaryVal}");
          }
        }
        #endregion

        #region Get employer
        var employerName = string.Empty;
        var employerDetails = parser.DocumentNode.SelectSingleNode("//div[@id='page7']");
        if (employerDetails == null)
        {
          throw new Exception("HTML Missing div id = 'page7' tag");
        }
        var employerNodes = employerDetails.SelectNodes(".//div[@class='form_element cf_textbox']");
        foreach (var node in employerNodes)
        {
          var text = node.InnerHtml.ToLower();
          if (text.Contains("<strong>employer</strong>"))
          {
            employerName = Regex.Replace(GetLastTextBit(text), @"[^A-Za-z0-9 \(\)\.\-]", string.Empty); // keep safe chars
          }
        }
        #endregion

        #region Add to InDox outbox
        var outMessage = new MailMessage();
        foreach (var attachment in message.FindAllAttachments())
        {
          using (var ms = new MemoryStream())
          {
            attachment.Save(ms);
            ms.Position = 0;
            outMessage.Attachments.Add(new Attachment(ms, attachment.FileName));
          }
        }

        outMessage.From = new MailAddress(config.GetCustomSetting(null, "docFromAddress", false)); // AppConfig.DocFromAddress);
        outMessage.To.Add(new MailAddress(config.GetCustomSetting(null, "docSendToAddress", false))); // AppConfig.DocSendToAddress));
        // (Contact number = preferred contact number according to the client per your website.)    
        // Normal|Title|Initials|Surname | FirstName| IDNumber    | Contact1 | Contact2 | Contact3   | Contact4 | ContactNo | Email            | Cell     | Work     | Home | E | Employer Name |Net Salary
        // Normal|Miss |D       |Hansen  |Davina    |8606191234567|0830000000|0830000000|0830000000  |0830000000|           |davina@indox.co.za|0830000000|0830000000|      | E | Indox         |9000.00
        //   0      1      2        3         4            5            6          7          8            9            10            11              12       13       14    15      16            17
        //
        outMessage.Subject = string.Format(CultureInfo.InvariantCulture, "{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17:F2}",
          "Normal",       // 0  'Normal' - > Lead type * DO NOT LOCALIZE *
          string.Empty,   // 1  No title
          initials,       // 2  initials
          surname,        // 3  surname
          firstName,      // 4  firstname
          id,             // 5  IDNumber,
          string.Empty,   // 6  Contact1
          string.Empty,   // 7  Contact2 
          string.Empty,   // 8  Contact3
          string.Empty,   // 9  Contact4
          telPreferred,   // 10 ContactNo
          email,          // 11 email address     
          cellNum,        // 12 Cell
          workTel,        // 13 Work
          homeTel,        // 14 Home
          "E",            // 15 "E" * DO NOT LOCALIZE * 
          employerName,   // 16 Employer Name
          netSalary);     // 17 Nett Salary

        AddEvent(id, "OK", scorecard.Score);
        log.Information("Added to InDox queue: {Subject}, {@Content}", outMessage.Subject, outMessage);
        _outBox.Add(outMessage);
        wasSentToInDox = true;
        _todaySentCount++;
        #endregion
      }
      catch (Exception err)
      {
        AddEvent(id, err.Message, score);
        log.Warning(err, "ProcessPendingMessage");
      }

      //if (!wasSentToInDox) // Simply forward the message as-is to the power/atlas CC...
      {
        var outMessage = message.ToMailMessage();
        outMessage.To.Clear();
        outMessage.To.Add(new MailAddress("documentskzn@atcorp.co.za"));  // TODO: Config
        _outBox.Add(outMessage);
      }

      return true;
    }


    private static void AddEvent(string idNumber, string message, int score)
    {
      _processedToday.Push(new Tuple<DateTime, string, string, int>(DateTime.Now, idNumber, message, score));
    }


    /// <summary>
    /// Gets last text bit after the final '>' character
    /// </summary>
    /// <param name="html">HTML text</param>
    /// <returns>Trailing text</returns>
    private static string GetLastTextBit(string html)
    {
      var pos = html.LastIndexOf(">");
      return pos > 0 && pos < html.Length - 1 ? html.Substring(pos + 1).Trim() : string.Empty;
    }


    private static string CleanTel(string telNo)
    {
      return !string.IsNullOrEmpty(telNo) ? Regex.Replace(telNo, @"[^\d]", string.Empty) : string.Empty;
    }


    /// <summary>
    /// Log statistics
    /// </summary>
    private static void LogProcessedAndClear(ILogging log)
    {
      log.Information("Daily statistics:\r\n========================================");
      Tuple<DateTime, string, string, int> processed;
      while (_processedToday.TryPop(out processed))
      {
        log.Information("{DateTime:yyyy-MM-dd HH:mm:ss},{ID},{Result},{Score}", processed.Item1, processed.Item2, processed.Item3, processed.Item4);
      }
    }


    /// <summary>
    /// Add to pending messages
    /// </summary>    
    private static void QueueMessage(Message message)
    {
      _queued.Add(message);
    }


    /// <summary>
    /// Last date sent count was reset
    /// </summary>
    private static DateTime _lastDate = DateTime.Now;

    /// <summary>
    /// How many have we sent today
    /// </summary>
    private static int _todaySentCount = 0;


    private static readonly ConcurrentStack<Tuple<DateTime, string, string, int>> _processedToday = new ConcurrentStack<Tuple<DateTime, string, string, int>>();


    private static readonly List<Message> _queued = new List<Message>();

    /// <summary>
    /// e-mail outbox
    /// </summary>
    private static readonly List<MailMessage> _outBox = new List<MailMessage>();

  }
}
