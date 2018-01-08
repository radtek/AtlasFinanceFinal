using System;

using DevExpress.Xpo;

using EvolutionEnums = Atlas.Enumerators.Evolution;


namespace Atlas.Domain.Model.Evolution
{
  /// <summary>
  /// A snapshot of a loan's status, at a specific point-in-time
  /// </summary>
  public class EVO_LoanTrackSnapshot : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public Int64 LoanTrackSnapshotId { get; set; }

    private EVO_LoanTrack _loanTrack;
    [Association("EVO_LoanTrack_Snapshot", UseAssociationNameAsIntermediateTableName = true)]
    public EVO_LoanTrack LoanTrack
    {
      get { return _loanTrack; }
      set { SetPropertyValue<EVO_LoanTrack>("LoanTrack", ref _loanTrack, value); }
    }
    
    private EvolutionEnums.SnapshotReasonTypes _reason;
    [Persistent]
    public EvolutionEnums.SnapshotReasonTypes Reason
    {
      get { return _reason; }
      set { SetPropertyValue<EvolutionEnums.SnapshotReasonTypes>("Reason", ref _reason, value); }
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    // FROM ASS

    /// <summary>
    /// loan.status
    /// </summary>
    private string _loanStatus;
    [Persistent, Size(1)]
    public string LoanStatus
    {
      get { return _loanStatus; }
      set { SetPropertyValue<string>("LoanStatus", ref _loanStatus, value); }
    }

    // Need to get from trans.reason -> see reason 'rn' col FUNCTION -> loan.status
    //private string _assReasonCode;
    //[Persistent]
    //public string AssReasonCode
    //{
    //  get { return _assReasonCode; }
    //  set { SetPropertyValue<string>("AssReasonCode", ref _assReasonCode, value); }
    //}

    /// <summary>
    /// trans calc...
    /// </summary>
    private decimal _currentBalance;
    [Persistent]
    public decimal CurrentBalance
    {
      get { return _currentBalance; }
      set { SetPropertyValue<decimal>("CurrentBalance", ref _currentBalance, value); }
    }


    /// <summary>
    /// trans caclc...
    /// </summary>
    private DateTime? _lastReceipt;
    [Persistent]
    public DateTime? LastReceipt
    {
      get { return _lastReceipt; }
      set { SetPropertyValue<DateTime?>("LastReceipt", ref _lastReceipt, value); }
    }


    /// <summary>
    /// trans caclc...
    /// </summary>
    private DateTime? _overdueSince;
    [Persistent]
    public DateTime? OverdueSince
    {
      get { return _overdueSince; }
      set { SetPropertyValue<DateTime?>("OverdueSince", ref _overdueSince, value); }
    }

    /// <summary>
    /// trans caclc...
    /// </summary>
    private decimal _overdueAmount;
    [Persistent]
    public decimal OverdueAmount
    {
      get { return _overdueAmount; }
      set { SetPropertyValue<decimal>("OverdueAmount", ref _overdueAmount, value); }
    }


    /// <summary>
    /// loan.tramount
    /// </summary>
    private decimal _instalmentAmount;
    [Persistent]
    public decimal InstalmentAmount
    {
      get { return _instalmentAmount; }
      set { SetPropertyValue<Decimal>("InstalmentAmount", ref _instalmentAmount, value); }
    }


    private string _payFrequency;
    [Persistent, Size(2)]
    public string PayFrequency
    {
      get { return _payFrequency; }
      set { SetPropertyValue<string>("PayFrequency", ref _payFrequency, value); }
    }


    private int _loanPeriod;
    [Persistent]
    public int LoanPeriod
    {
      get { return _loanPeriod; }
      set { SetPropertyValue<int>("LoanPeriod", ref _loanPeriod, value); }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////

    private DateTime _snapshotDate;
    [Persistent, Indexed]
    public DateTime SnapshotDate
    {
      get { return _snapshotDate; }
      set { SetPropertyValue<DateTime>("SnapshotDate", ref _snapshotDate, value); }
    }

    private DateTime _created;
    [Persistent]
    public DateTime Created
    {
      get { return _created; }
      set { SetPropertyValue<DateTime>("Created", ref _created, value); }
    }
    
    private decimal _grossSalary;
    public decimal GrossSalary
    {
      get { return _grossSalary; }
      set { SetPropertyValue<decimal>("GrossSalary", ref _grossSalary, value); }
    }

    /// <summary>
    /// Many-2-many- batches this snapshot is part of
    /// </summary>
    [Association("EVO_LoanTrackSnapshot_UploadBatch", UseAssociationNameAsIntermediateTableName = true)]
    public XPCollection<EVO_UploadBatch> UploadBatches
    {
      get { return GetCollection<EVO_UploadBatch>("UploadBatches"); }
    }

    public EVO_LoanTrackSnapshot() : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public EVO_LoanTrackSnapshot(Session session) : base(session)
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