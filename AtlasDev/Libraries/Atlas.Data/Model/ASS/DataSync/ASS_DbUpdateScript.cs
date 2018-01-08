/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    SQL scripts to execute, to upgrade branch SQL to a specific, expected version
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
  /// SQL scripts to execute, to upgrade local branch SQL to a specific version
  /// </summary>
  public class ASS_DbUpdateScript : XPCustomObject
  { 
    [Key(AutoGenerate = true)]
    public Int64 DbUpdateScriptId { get; set; }

    private string _DbVersion;
    [Persistent]
    [Indexed(Unique = true)]
    public string DbVersion
    {
      get { return _DbVersion; }
      set { SetPropertyValue("DbVersion", ref _DbVersion, value); }
    }

    private ASS_DbUpdateScript _PreviousVersion;
    [Persistent]
    [Indexed]
    public ASS_DbUpdateScript PreviousVersion
    {
      get { return _PreviousVersion; }
      set { SetPropertyValue("PreviousVersion", ref _PreviousVersion, value); }
    }

    private string _UpdateScript;
    [Persistent, Size(int.MaxValue)]
    public string UpdateScript
    {
      get { return _UpdateScript; }
      set { SetPropertyValue("UpdateScript", ref _UpdateScript, value); }
    }

    private string _Description;
    [Persistent, Size(500)]
    public string Description
    {
      get { return _Description; }
      set { SetPropertyValue("Description", ref _Description, value); }
    }

    private DateTime _CreatedDT;
    [Persistent]
    public DateTime CreatedDT
    {
      get { return _CreatedDT; }
      set { SetPropertyValue("CreatedDT", ref _CreatedDT, value); }
    }


    #region Constructors

    public ASS_DbUpdateScript()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public ASS_DbUpdateScript(Session session)
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

    #endregion

  }

}