/*
 * 
 *  List Atlas software releases
 * 
 * 
 * 
 * */

using System;
using System.Collections.Generic;
using System.Linq;

using DevExpress.Xpo;


namespace Atlas.Domain.Security
{
  public class COR_Software: XPCustomObject
  {       
    [Persistent("AtlasSoftwareId")]
    [Key(AutoGenerate=true)]
    public Int64 AtlasSoftwareId { get; set; }

    private Enumerators.General.ApplicationIdentifiers _AtlasApplication;
    [Persistent, Indexed]
    public Enumerators.General.ApplicationIdentifiers AtlasApplication
    {
      get { return _AtlasApplication; }
      set { SetPropertyValue(nameof(AtlasApplication), ref _AtlasApplication, value); }
    }

    private string _AppVersion;
    [Persistent, Size(25)]
    public string AppVersion
    {
      get { return _AppVersion; }
      set { SetPropertyValue(nameof(AppVersion), ref _AppVersion, value); }      
    }
        
    private DateTime _ReleaseDT;
    [Persistent("ReleaseDT"), Indexed]
    public DateTime ReleasedDT
    {
      get { return _ReleaseDT; }
      set { SetPropertyValue(nameof(ReleasedDT), ref _ReleaseDT, value); }      
    }

    private string _FileHash;
    [Persistent, Size(32)]
    public string FileHash
    {
      get { return _FileHash; }
      set { SetPropertyValue(nameof(FileHash), ref _FileHash, value); }
    }

    private string _Comments;
    [Persistent, Size(int.MaxValue)]
    public string Comments
    {
      get { return _Comments; }
      set { SetPropertyValue(nameof(Comments), ref _Comments, value); }
    }


    #region Constructors

    public COR_Software(Session session) : base(session) { }
    public COR_Software() : base() { }

    #endregion
  }
}
