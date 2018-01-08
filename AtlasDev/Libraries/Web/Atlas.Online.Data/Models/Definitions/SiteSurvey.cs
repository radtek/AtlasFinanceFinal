using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class SiteSurvey: XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public int SiteSurveyId { get; set; }
    
    public string Email { get; set; }
    public string Name { get; set; }
    
    [Persistent("ClientId")]
    [Indexed]
    public Client Client { get; set; }

    public int Q1Option { get; set; }
    public int Q2Option { get; set; }
    public int Q3Option { get; set; }
    public int Q4Option { get; set; }

    public string Comments { get; set; }

    public DateTime CreateDate { get; set; }

    public SiteSurvey() : base() { }
    public SiteSurvey(Session session) : base(session) { }

    public static bool Exists(Session session, long clientId)
    {
      return new XPQuery<SiteSurvey>(session).Any(x => x.Client.ClientId == clientId);
    }
  }
}
