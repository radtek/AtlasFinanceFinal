/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2015 Atlas Finance Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for: Link fraud case to a PER_Person
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
  /// Link fraud case to a PER_Person
  /// </summary>
  public class FRD_CasePerson : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public Int64 CasePersonId { get; set; }
        
    [Persistent, Indexed, Association]
    public FRD_Case CaseId
    {
      get { return GetPropertyValue<FRD_Case>("CaseId"); }
      set { SetPropertyValue<FRD_Case>("CaseId", value); }
    }

    [Persistent, Indexed]
    public PER_Person PersonId
    {
      get { return GetPropertyValue<PER_Person>("PersonId"); }
      set { SetPropertyValue<PER_Person>("PersonId", value); }
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

    [Persistent, Size(int.MaxValue)]
    public string Comment
    {
      get { return GetPropertyValue<string>("Comment"); }
      set { SetPropertyValue<string>("Comment", value); }
    }

    #region Constructors

    public FRD_CasePerson()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public FRD_CasePerson(Session session)
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