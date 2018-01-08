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
      [Description("Single")]
      Single = 1,
      [Description("Married")]
      Married = 2,
      [Description("Divorced")]
      Divorced = 3,
      [Description("Widowed")]
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
      [Description("Asian")]
      Asian = 1,
      [Description("Black")]
      Black = 2,
      [Description("Coloured")]
      Coloured = 3,
      [Description("Indian")]
      Indian = 4,
      [Description("White")]
      White = 5,
      [Description("Other")]
      Other = 6
    }

    public enum Industry
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Academic")]
      Academic = 1,
      [Description("Accounting")]
      Accounting = 2,
      [Description("Advertising")]
      Advertising = 3,
      [Description("Finance")]
      Finance = 4,
      [Description("CallCentre")]
      CallCentre = 5,
      [Description("Engineering")]
      Engineering = 6,
      [Description("Building")]
      Building = 7,
      [Description("Government")]
      Government = 8,
      [Description("Hospitality")]
      Hospitality = 9,
      [Description("IT")]
      IT = 10,
      [Description("Insurance")]
      Insurance = 11,
      [Description("Logistics")]
      Logistics = 12,
      [Description("Legal")]
      Legal = 13,
      [Description("Manufacturing")]
      Manufacturing = 14,
      [Description("Medical")]
      Medical = 15,
      [Description("Motor")]
      Motor = 16,
      [Description("NGO")]
      NGO = 17,
      [Description("Retail")]
      Retail = 18,
      [Description("Sales")]
      Sales = 19,
      [Description("Tourism")]
      Tourism = 20,
      [Description("Transport")]
      Transport = 21,
      [Description("Other")]
      Other = 23
    }

    public enum LoanReason
    {
      [Description("Not Set")]
      NotSet = 0,
      [Description("Debt Consolidation")]
      DebtConsolidation = 1,
      [Description("Death/Funeral")]
      DeathFuneral = 2,
      [Description("Education")]
      Education = 3,
      [Description("Emergency")]
      Emergency = 4,
      [Description("Furniture")]
      Furniture = 5,
      [Description("Housing")]
      Housing = 6,
      [Description("Income Loss")]
      IncomeLoss = 7,
      [Description("Medical")]
      Medical = 9,
      [Description("Service")]
      Service = 10,
      [Description("Small Business")]
      SmallBusiness = 11,
      [Description("Theft or Fire")]
      TheftOrFire = 12,
      [Description("Other Reason")]
      OtherReason = 13
    }

  }
}
