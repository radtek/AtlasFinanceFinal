/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *   Stores version information about server maintained tables, i.e. ASBRANCH/ASSTMAST
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
  /// Stores version information about server maintained tables, i.e. ASBRANCH/ASSTMAST
  /// </summary>
  public class ASS_ServerTable : XPCustomObject
  {  
    [Key(AutoGenerate = true)]
    public Int64 ServerTableId { get; set; }

    private string _TableName;
    [Persistent, Indexed]
    public string TableName
    {
      get { return _TableName; }
      set { SetPropertyValue("TableName", ref _TableName, value); }
    }

    private string _Description;
    [Persistent, Size(500)]
    public string Description
    {
      get { return _Description; }
      set { SetPropertyValue("Description", ref _Description, value); }
    }

    private DateTime _LastUpdatedDT;
    [Persistent]
    public DateTime LastUpdatedDT
    {
      get { return _LastUpdatedDT; }
      set { SetPropertyValue("LastUpdatedDT", ref _LastUpdatedDT, value); }
    }

    private bool _LiveUpdates;
    [Persistent]
    public bool LiveUpdates
    {
      get { return _LiveUpdates; }
      set { SetPropertyValue("LiveUpdates", ref _LiveUpdates, value); }
    }


    #region Constructors

    public ASS_ServerTable()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public ASS_ServerTable(Session session)
      : base(session)
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    #endregion

  }
}