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
  public partial class AccountVerification
  {

    private string statusField;

    private int referenceNoField;

    /// <remarks/>
    public string Status
    {
      get
      {
        return this.statusField;
      }
      set
      {
        this.statusField = value;
      }
    }

    /// <remarks/>
    public int ReferenceNo
    {
      get
      {
        return this.referenceNoField;
      }
      set
      {
        this.referenceNoField = value;
      }
    }
  }
}
