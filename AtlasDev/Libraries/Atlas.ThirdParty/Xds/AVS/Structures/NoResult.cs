using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.ThirdParty.Xds.AVS.Structures
{
  /// <remarks/>
  [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
  [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
  public partial class NoResult
  {

    private string notFoundField;

    /// <remarks/>
    public string NotFound
    {
      get
      {
        return this.notFoundField;
      }
      set
      {
        this.notFoundField = value;
      }
    }

    private string errorField;

    /// <remarks/>
    public string Error
    {
      get
      {
        return this.errorField;
      }
      set
      {
        this.errorField = value;
      }
    }
  }
}
