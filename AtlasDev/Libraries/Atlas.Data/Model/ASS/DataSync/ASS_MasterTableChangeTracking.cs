/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2013 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *   Tracks changes made to ASS lookup table rows, which need to be replicated to all branches
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
  public class ASS_MasterTableChangeTracking : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public Int64 RecId { get; set; }

    private string _TableName;
    [Persistent, Size(20), Indexed]
    public string TableName
    {
      get { return _TableName; }
      set { SetPropertyValue("TableName", ref _TableName, value); }
    }

    private string _KeyFieldName;
    [Persistent, Size(20)]
    public string KeyFieldName
    {
      get { return _KeyFieldName; }
      set { SetPropertyValue("KeyFieldName", ref _KeyFieldName, value); }
    }

    private string _KeyFieldValue;
    [Persistent, Size(50)]
    public string KeyFieldValue
    {
      get { return _KeyFieldValue; }
      set { SetPropertyValue("KeyFieldValue", ref _KeyFieldValue, value); }
    }

    private DateTime _ChangedTS;
    [Persistent]
    public DateTime ChangedTS
    {
      get { return _ChangedTS; }
      set { SetPropertyValue("ChangedTS", ref _ChangedTS, value); }
    }


    #region Constructors

    public ASS_MasterTableChangeTracking()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public ASS_MasterTableChangeTracking(Session session)
      : base(session)
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    #endregion

  }
}
