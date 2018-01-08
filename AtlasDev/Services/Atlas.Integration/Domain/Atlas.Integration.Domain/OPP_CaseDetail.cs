/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2015 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *    Sales opportunity- Model for new sales opportunity
  * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2015-08-28- Initial creation
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using DevExpress.Xpo;
using Atlas.Enumerators;
using AtlasEnums = Atlas.Enumerators;


namespace Atlas.Domain.Model.Opportunity
{
  /// <summary>
  /// Opportunity- case details
  /// </summary>
  public class OPP_CaseDetail : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public Int64 CaseDetailId { get; set; }

    [Persistent, Size(100), Indexed]
    public string CallerSysRefId
    {
      get { return GetPropertyValue<string>("CallerSysRefId"); }
      set { SetPropertyValue<string>("CallerSysRefId", value); }
    }

    [Persistent]
    public DateTime Started
    {
      get { return GetPropertyValue<DateTime>("Started"); }
      set { SetPropertyValue<DateTime>("Started", value); }
    }

    [Persistent]
    public DateTime Completed
    {
      get { return GetPropertyValue<DateTime>("Completed"); }
      set { SetPropertyValue<DateTime>("Completed", value); }
    }

    [Persistent]
    public DateTime FollowUp
    {
      get { return GetPropertyValue<DateTime>("FollowUp"); }
      set { SetPropertyValue<DateTime>("FollowUp", value); }
    }

    [Persistent]
    public DateTime Created
    {
      get { return GetPropertyValue<DateTime>("Created"); }
      set { SetPropertyValue<DateTime>("Created", value); }
    }

    [Persistent("UserPersonId")]
    public PER_Person UserPerson
    {
      get { return GetPropertyValue<PER_Person>("UserPerson"); }
      set { SetPropertyValue<PER_Person>("UserPerson", value); }
    }

    [Persistent("BranchId")]
    public BRN_Branch Branch
    {
      get { return GetPropertyValue<BRN_Branch>("Branch"); }
      set { SetPropertyValue<BRN_Branch>("Branch", value); }
    }

    [Persistent]
    public string GPSCoords
    {
      get { return GetPropertyValue<string>("GPSCoords"); }
      set { SetPropertyValue<string>("GPSCoords", value); }
    }

    [Persistent, Size(10)]
    public string ClientCellularNum
    {
      get { return GetPropertyValue<string>("ClientCellularNum"); }
      set { SetPropertyValue<string>("ClientCellularNum", value); }
    }

    [Persistent, Size(20)]
    public string ClientIdNum
    {
      get { return GetPropertyValue<string>("ClientIdNum"); }
      set { SetPropertyValue<string>("ClientIdNum", value); }
    }

    [Persistent, Size(50)]
    public string ClientFirstname
    {
      get { return GetPropertyValue<string>("ClientFirstname"); }
      set { SetPropertyValue<string>("ClientFirstname", value); }
    }

    [Persistent, Size(50)]
    public string ClientLastname
    {
      get { return GetPropertyValue<string>("ClientLastname"); }
      set { SetPropertyValue<string>("ClientLastname", value); }
    }

    [Persistent]
    public DateTime ClientDateOfBirth
    {
      get { return GetPropertyValue<DateTime>("ClientDateOfBirth"); }
      set { SetPropertyValue<DateTime>("ClientDateOfBirth", value); }
    }

    [Persistent("EnquiryId")]
    public BUR_Enquiry Enquiry
    {
      get { return GetPropertyValue<BUR_Enquiry>("Enquiry"); }
      set { SetPropertyValue<BUR_Enquiry>("Enquiry", value); }
    }


    [Persistent]
    public bool PassedVetting
    {
      get { return GetPropertyValue<bool>("PassedVetting"); }
      set { SetPropertyValue<bool>("PassedVetting", value); }
    }


    [Persistent]
    public AtlasEnums.Opportunity.OpportunityStatus OpportunityState
    {
      get { return GetPropertyValue<AtlasEnums.Opportunity.OpportunityStatus>("OpportunityState"); }
      set { SetPropertyValue<AtlasEnums.Opportunity.OpportunityStatus>("OpportunityState", value); }
    }

    [Persistent]
    public BRN_Branch GrantedBranch
    {
      get { return GetPropertyValue<BRN_Branch>("GrantedBranch"); }
      set { SetPropertyValue<BRN_Branch>("GrantedBranch", value); }
    }

    [Persistent]
    public DateTime GrantedDate
    {
      get { return GetPropertyValue<DateTime>("GrantedDate"); }
      set { SetPropertyValue<DateTime>("GrantedDate", value); }
    }

    [Persistent]
    public decimal GrantedLoanAmount
    {
      get { return GetPropertyValue<decimal>("GrantedLoanAmount"); }
      set { SetPropertyValue<decimal>("GrantedLoanAmount", value); }
    }

    [Persistent]
    public AtlasEnums.Account.PeriodFrequency GrantedPeriodType
    {
      get { return GetPropertyValue<AtlasEnums.Account.PeriodFrequency>("GrantedPeriodType"); }
      set { SetPropertyValue<AtlasEnums.Account.PeriodFrequency>("GrantedPeriodType", value); }
    }

    [Persistent]
    public int GrantedPeriodVal
    {
      get { return GetPropertyValue<int>("GrantedPeriodVal"); }
      set { SetPropertyValue<int>("GrantedPeriodVal", value); }
    }


    public OPP_CaseDetail()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public OPP_CaseDetail(Session session)
      : base(session)
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public override void AfterConstruction()
    {
      base.AfterConstruction();
      // Place here your initialization code.
    }
  }

}