using Atlas.Enumerators;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.Definitions
{
	public sealed class Industry : XPLiteObject
  {
    [Key(AutoGenerate = false)]
    public int IndustryId { get; set; }
    [NonPersistent]
    public WebEnumerators.Industry Type
    {
      get { return Description.FromStringToEnum<WebEnumerators.Industry>(); }
      set { this.Description = value.ToStringEnum(); }
    }
    public string Description { get; set; }

    public Industry() : base() { }
		public Industry(Session session) : base(session) { }
  }
}