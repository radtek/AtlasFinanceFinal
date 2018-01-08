using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Data
{
  public class WebEnumerators
  {
    public enum Province
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Eastern Cape")]
      EC = 1,
      [Description("Gauteng")]
      GAU = 2,
      [Description("KwaZulu-Natal")]
      KZN = 3,
      [Description("Limpopo")]
      LIM = 4,
      [Description("Mpumalanga")]
      MPL = 5,
      [Description("Northern Cape")]
      NC = 6,
      [Description("North West")]
      NW = 7,
      [Description("Western Cape")]
      WC = 8,
      [Description("Free State")]
      FS = 9
    }

    public enum BankName
    {
      [Description("Not Set")]
      NotSet = 0, // Required change from 0, postgres does not allow 0 id's,
      [Description("Standard Bank")]
      STD = 1,
      [Description("First National Bank")]
      FNB = 2,
      [Description("Nedbank")]
      NED = 3,
      [Description("ABSA")]
      ABS = 10,
      [Description("Capitec")]
      CAP = 16,
    }

    // These can be removed once system-wide enums have been created for these.
    public enum MaritalStatus
    {
      [Description("Not Set")]
      NotSet = 0,
      Single = 1,
      Married = 2,
      Divorced = 3,
      Widowed = 4
    }

    public enum MaritalAgreement
    {
      [Description("In Community")]
      InCommunity = 1,
      [Description("Out of Community")]
      OutCommunity = 2
    }

    public enum EthnicGroup
    {
      [Description("Not Set")]
      NotSet = 0,
      Asian = 1,
      Black = 2,
      Coloured = 3,
      Indian = 4,
      White = 5,
      Other = 6
    }

    public enum Industry
    {
      // TODO: COmplete
      [Description("Not Set")]
      NotSet,
      Mining,
      Accounting,
      InformationTechnology,
      Other
    }

    public enum LoanReason
    {
      [Description("Not Set")]
      NotSet,
      Car,
      House,
      Jet,
      Other
    }

  }
}
