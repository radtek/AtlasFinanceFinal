
/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2015 Atlas Finance Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *    Model for Altech NAEDO/AEDO cancellations
  * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2015-03-20- Initial creation
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  /// <summary>
  /// Altech NAEDO/AEDO contract cancel requests- 'HandledOK' indicates handled with/without problems
  /// </summary>
  public class ALT_EDOContractCancel : XPBaseObject
  {    
    [Key(AutoGenerate = true)]
    public Int64 EDOContractCancelId { get; set; }

    /// <summary>
    /// The EDO type- 'N'- NAEDO, 'A'- AEDO, 'T'- TSP
    /// </summary>
    [Persistent, Size(1), Indexed]
    public string EDOType
    {
      get { return GetPropertyValue<string>(nameof(EDOType)); }
      set { SetPropertyValue<string>(nameof(EDOType), value); }
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

    /// <summary>
    /// The Altech contract number
    /// </summary>
    [Persistent, Size(100)]
    public string ContractNum
    {
      get { return GetPropertyValue<string>(nameof(ContractNum)); }
      set { SetPropertyValue<string>(nameof(ContractNum), value); }
    }
    
    /// <summary>
    /// The branch that originated the cancel request
    /// </summary>
    [Persistent]
    public BRN_Branch Branch
    {
      get { return GetPropertyValue<BRN_Branch>(nameof(Branch)); }
      set { SetPropertyValue<BRN_Branch>(nameof(Branch), value); }
    }

    /// <summary>
    /// The person who requested the cancellation
    /// </summary>
    [Persistent]
    public PER_Person UserPerson
    {
      get { return GetPropertyValue<PER_Person>(nameof(UserPerson)); }
      set { SetPropertyValue<PER_Person>(nameof(UserPerson), value); }
    }

    /// <summary>
    /// Altech's EDO transaction id
    /// </summary>
    [Persistent]
    public Int64 AltechTransactionId
    {
      get { return GetPropertyValue<Int64>(nameof(AltechTransactionId)); }
      set { SetPropertyValue<Int64>(nameof(AltechTransactionId), value); }
    }

    /// <summary>
    /// Date of last cancellation attempt
    /// </summary>    
    [Persistent, Indexed]
    public DateTime? LastAttemptDT
    {
      get { return GetPropertyValue<DateTime?>(nameof(LastAttemptDT)); }
      set { SetPropertyValue<DateTime?>(nameof(LastAttemptDT), value); }
    }

    /// <summary>
    /// Date of completion/final failure attempt
    /// </summary>    
    [Persistent, Indexed]
    public DateTime? CompletedDT
    {
      get { return GetPropertyValue<DateTime?>(nameof(CompletedDT)); }
      set { SetPropertyValue<DateTime?>(nameof(CompletedDT), value); }
    }

    [Persistent, Size(500)]
    public string LastError
    {
      get { return GetPropertyValue<string>(nameof(LastError)); }
      set { SetPropertyValue<string>(nameof(LastError), value); }
    }

    /// <summary>
    /// null if not yet attempted, true if successful, false if all attempts failed
    /// </summary>    
    [Persistent, Indexed]
    public bool? HandledOK
    {
      get { return GetPropertyValue<bool?>(nameof(HandledOK)); }
      set { SetPropertyValue<bool?>(nameof(HandledOK), value); }
    }

    [Persistent]
    public AEDOLogin AEDOLogin
    {
      get { return GetPropertyValue<AEDOLogin>(nameof(AEDOLogin)); }
      set { SetPropertyValue<AEDOLogin>(nameof(AEDOLogin), value); }
    }

    [Persistent]
    public NAEDOLogin NAEDOLogin
    {
      get { return GetPropertyValue<NAEDOLogin>(nameof(NAEDOLogin)); }
      set { SetPropertyValue<NAEDOLogin>(nameof(NAEDOLogin), value); }
    }

    [Persistent]
    public BRN_Branch BranchId
    {
      get { return GetPropertyValue<BRN_Branch>(nameof(BranchId)); }
      set { SetPropertyValue<BRN_Branch>(nameof(BranchId), value); }
    }


    public ALT_EDOContractCancel()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public ALT_EDOContractCancel(Session session)
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