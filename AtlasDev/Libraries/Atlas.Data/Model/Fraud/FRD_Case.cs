/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2015 Atlas Finance Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for: A fraud case
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  /// <summary>
  /// A fraud case
  /// </summary>
  public class FRD_Case : XPBaseObject
  {
    [Key(AutoGenerate = true)]
    public Int64 CaseId { get; set; }

    [Persistent]
    public PER_Person CreatedBy
    {
      get { return GetPropertyValue<PER_Person>("CreatedBy"); }
      set { SetPropertyValue<PER_Person>("CreatedBy", value); }
    }

    [Persistent, Indexed]
    public DateTime CreatedDT
    {
      get { return GetPropertyValue<DateTime>("CreatedDT"); }
      set { SetPropertyValue<DateTime>("CreatedDT", value); }
    }

    [Persistent, Indexed, Size(1000)]
    public string CaseReference
    {
      get { return GetPropertyValue<string>("CaseReference"); }
      set { SetPropertyValue<string>("CaseReference", value); }
    }

    [Persistent, Size(int.MaxValue)]
    public string Comment
    {
      get { return GetPropertyValue<string>("Comment"); }
      set { SetPropertyValue<string>("Comment", value); }
    }

    [Persistent]
    public DateTime? LastEditedDT
    {
      get { return GetPropertyValue<DateTime?>("LastEditedDT"); }
      set { SetPropertyValue<DateTime?>("LastEditedDT", value); }
    }


    [Association]
    public XPCollection<FRD_CaseNote> CaseNotes { get { return GetCollection<FRD_CaseNote>("CaseNotes"); } }

    [Association]
    public XPCollection<FRD_CaseBankDetail> CaseBanks { get { return GetCollection<FRD_CaseBankDetail>("CaseBanks"); } }

    [Association]
    public XPCollection<FRD_CaseCompany> CaseCompanies { get { return GetCollection<FRD_CaseCompany>("CaseCompanies"); } }

    [Association]
    public XPCollection<FRD_CaseContact> CaseContacts { get { return GetCollection<FRD_CaseContact>("CaseContacts"); } }

    [Association]
    public XPCollection<FRD_CasePerson> CasePersons { get { return GetCollection<FRD_CasePerson>("CasePersons"); } }


    #region Constructors

    public FRD_Case()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public FRD_Case(Session session)
      : base(session)
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    #endregion


    public override void AfterConstruction()
    {
      base.AfterConstruction();
      // Place here your initialization code.
    }

  }

}