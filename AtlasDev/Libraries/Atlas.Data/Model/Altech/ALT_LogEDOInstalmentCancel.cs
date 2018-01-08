using System;
using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class ALT_LogEDOInstalmentCancel : XPBaseObject
  {
    [Key(AutoGenerate = true)]
    public Int64 LogEDOInstalmentCancelId { get; set; }

    [Persistent, Indexed]
    public ALT_EDOContractCancel EDOContractCancelId
    {
      get { return GetPropertyValue<ALT_EDOContractCancel>(nameof(EDOContractCancelId)); }
      set { SetPropertyValue<ALT_EDOContractCancel>(nameof(EDOContractCancelId), value); }
    }

    /// <summary>
    /// Date of cancellation creation request
    /// </summary>    
    [Persistent, Indexed]
    public DateTime CreatedDT
    {
      get { return GetPropertyValue<DateTime>(nameof(CreatedDT)); }
      set { SetPropertyValue<DateTime>(nameof(CreatedDT), value); }
    }

    [Persistent]
    public int InstalmentNum
    {
      get { return GetPropertyValue<int>(nameof(InstalmentNum)); }
      set { SetPropertyValue<int>(nameof(InstalmentNum), value); }
    }


    [Persistent]
    public string Result
    {
      get { return GetPropertyValue<string>(nameof(Result)); }
      set { SetDelayedPropertyValue<string>(nameof(Result), value); }
    }


    public ALT_LogEDOInstalmentCancel()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public ALT_LogEDOInstalmentCancel(Session session)
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