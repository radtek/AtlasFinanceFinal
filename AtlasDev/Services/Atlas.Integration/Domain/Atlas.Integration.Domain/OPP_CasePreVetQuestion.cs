/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2015 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *    Sales opportunity- Model of result of a single pre-vetting question
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


namespace Atlas.Domain.Model.Opportunity
{  
  public class OPP_CasePreVetQuestion : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public Int64 CasePreVetQuestionId { get; set; }

    [Persistent("CaseDetailId"), Indexed]
    public OPP_CaseDetail CaseDetail
    {
      get { return GetPropertyValue<OPP_CaseDetail>("CaseDetail"); }
      set { SetPropertyValue<OPP_CaseDetail>("CaseDetail", value); }
    }
    
    [Persistent, Indexed, Size(500)]
    public string Parameter
    {
      get { return GetPropertyValue<string>("Parameter"); }
      set { SetPropertyValue<string>("Parameter", value); }
    }

    [Persistent, Size(500)]
    public string Value
    {
      get { return GetPropertyValue<string>("Value"); }
      set { SetPropertyValue<string>("Value", value); }
    }

    [Persistent]
    public bool? PositiveOutcome
    {
      get { return GetPropertyValue<bool?>("PositiveOutcome"); }
      set { SetPropertyValue<bool?>("PositiveOutcome", value); }
    }
    
    [Persistent]
    public string Comment
    {
      get { return GetPropertyValue<string>("Comment"); }
      set { SetPropertyValue<string>("Comment", value); }
    }


    public OPP_CasePreVetQuestion()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public OPP_CasePreVetQuestion(Session session)
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