using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.Definitions
{
  public sealed class Holidays : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public int HolidayId { get; set; }
    public DateTime Date { get; set; }

    public Holidays() : base() { }
    public Holidays(Session session) : base(session) { }
  }
}