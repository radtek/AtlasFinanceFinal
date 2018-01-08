using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class RouteHistory : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public int RouteHistoryId { get; set; }     
    [Persistent("MessageId")]
    public Message Message { get; set; }
    public string Source { get; set; }
    public string Destination { get; set; }
    public byte[] MsgData { get; set; }
    public DateTime CreateDate { get; set; }

    public RouteHistory() : base() { }
    public RouteHistory(Session session) : base(session) { }
  }
}