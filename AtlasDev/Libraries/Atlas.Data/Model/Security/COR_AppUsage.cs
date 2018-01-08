/*
 * 
 *  A combination of the Atlas machine, Atlas software & user.
 * 
 * 
 * 
 * 
 * 
 * */


using System;

using DevExpress.Xpo;

using Atlas.Domain.Model;


namespace Atlas.Domain.Security
{
  public class COR_AppUsage: XPCustomObject
  {  
    [Key(AutoGenerate = true)]
    public Int64 AppUsageId { get; set; }

    private COR_Machine _Machine;
    [Persistent("MachineId"), Indexed]
    public COR_Machine Machine
    {
      get { return _Machine; }
      set { SetPropertyValue(nameof(Machine), ref _Machine, value); }
    }

    private COR_Software _Application;
    [Persistent("SoftwareId")]
    public COR_Software Application
    {
      get { return _Application; }
      set { SetPropertyValue(nameof(Application), ref _Application, value); }
    }
        
    private PER_Security _User;
    [Persistent("SecurityId")]
    public PER_Security User
    {
      get { return _User; }
      set { SetPropertyValue(nameof(User), ref _User, value); }
    }

    private BRN_Branch _BranchCode;
    [Persistent("BranchId")]
    public BRN_Branch BranchCode
    {
      get { return _BranchCode; }
      set { SetPropertyValue(nameof(BranchCode), ref _BranchCode, value); }
    }


    #region Constructors

    public COR_AppUsage(Session session) : base(session) { }
    public COR_AppUsage() : base() { }

    #endregion

  }
}
