using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class Province : XPLiteObject
  {
    [Key(AutoGenerate = false)]
    public int ProvinceId { get; set; }
    [Persistent, Size(7)]
    public string ShortCode { get; set; }
    public string Description { get; set; }

    public Province() : base() { }
    public Province(Session session) : base(session) { }
  }
}