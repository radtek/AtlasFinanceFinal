using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class OTPRequest : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public long RequestId { get; set; }
    [Persistent("ClientId")]
    [Association]
    [Indexed]
    public Client Client { get;set;}
    public int OTP { get; set; }
    public string Key { get; set; }
    public DateTime RequestDate { get;set;}

    public OTPRequest() : base() { }
    public OTPRequest(Session session) : base(session) { }
  }
}