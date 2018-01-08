/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2015 Atlas Finance Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for: A fraud case notation
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
  /// A fraud case notation
  /// </summary>
  public class FRD_CaseNote : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public Int64 CaseNoteId { get; set; }

    [Persistent, Indexed, Association]
    public FRD_Case CaseId
    {
      get { return GetPropertyValue<FRD_Case>("CaseId"); }
      set { SetPropertyValue<FRD_Case>("CaseId", value); }
    }

    [Persistent, Size(int.MaxValue)]
    public string Comment
    {
      get { return GetPropertyValue<string>("Comment"); }
      set { SetPropertyValue<string>("Comment", value); }
    }

    [Persistent]
    public PER_Person CreatedBy
    {
      get { return GetPropertyValue<PER_Person>("CreatedBy"); }
      set { SetPropertyValue<PER_Person>("CreatedBy", value); }
    }

    [Persistent]
    public DateTime CreatedDT
    {
      get { return GetPropertyValue<DateTime>("CreatedDT"); }
      set { SetPropertyValue<DateTime>("CreatedDT", value); }
    }
        

    #region Constructors

    public FRD_CaseNote()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public FRD_CaseNote(Session session)
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