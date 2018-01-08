using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class Message : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public int MessageId { get; set; }    
    public Guid CorrelationId { get; set; }
    [Persistent("ClientId")]
    public Client Client { get; set; }
    public string CurrNode { get; set; }
    public byte[] MsgData { get; set; }
    public DateTime CreateDate { get; set; }

    public Message() : base() { }
    public Message(Session session) : base(session) { }
  }
}
  