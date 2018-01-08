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
  public sealed class Ethnicity : XPLiteObject
  {
    [Key(AutoGenerate = false)]
    public int EthnicityId { get; set; }
    [NonPersistent]
    public WebEnumerators.EthnicGroup Type
    {
      get { return Description.FromStringToEnum<WebEnumerators.EthnicGroup>(); }
      set { this.Description = value.ToStringEnum(); }
    }
    public string Description { get; set; }

    public Ethnicity() : base() { }
    public Ethnicity(Session session) : base(session) { }
  }
}