/*
 *  Used to log application usage/info
 * 
 * 
 * 
 * 
 * 
 * 
 * */

using System;

using DevExpress.Xpo;

using Atlas.Domain.Model;
using Atlas.Enumerators;


namespace Atlas.Domain.Security
{
  public class COR_LogAppUsage : XPCustomObject
  {   
    [Persistent, Key(AutoGenerate = true)]
    public Int64 LogAppUsageId { get; set; }

    private COR_AppUsage _AppUsageId;
    [Persistent, Indexed]
    public COR_AppUsage AppUsageId
    {
      get { return _AppUsageId; }
      set { SetPropertyValue(nameof(AppUsageId), ref _AppUsageId, value); }
    }
    
    private DateTime _EventDT;
    [Persistent]
    [Indexed]
    public DateTime EventDT
    {
      get { return _EventDT; }
      set { SetPropertyValue(nameof(EventDT), ref _EventDT, value); }
    }

    private General.AuditStatusType _AtlasEvent;
    [Persistent]
    public General.AuditStatusType AtlasEvent
    {
      get { return _AtlasEvent; }
      set { SetPropertyValue(nameof(AtlasEvent), ref _AtlasEvent, value); }
    }

    private string _Comments;
    [Persistent, Size(4096)]
    public string Comments
    {
      get { return _Comments; }
      set { SetPropertyValue(nameof(Comments), ref _Comments, value); }
    }


    #region public constructors
          
    public COR_LogAppUsage(Session session) : base(session) { }
    public COR_LogAppUsage() : base() { }

    #endregion
  }
}
