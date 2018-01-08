using System;

using DevExpress.Xpo;

using EvolutionEnums = Atlas.Enumerators.Evolution;


namespace Atlas.Domain.Model.Evolution
{
  public class EVO_UploadBatch : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public Int64 UploadBatchId { get; set; }
    
    private EvolutionEnums.BatchTypes _batchType;
    /// <summary>
    /// The kind of batch- i.e. daily, monthly, ad-hoc
    /// </summary>
    [Persistent, Indexed]
    public EvolutionEnums.BatchTypes BatchType
    {
      get { return _batchType; }
      set { SetPropertyValue<EvolutionEnums.BatchTypes>("BatchType", ref _batchType, value); }
    }
    
    private bool _batchIsLive;
    /// <summary>
    /// Is the batch a test (false), or a live batch (true)
    /// </summary>
    [Persistent]
    public bool BatchIsLive
    {
      get { return _batchIsLive; }
      set { SetPropertyValue<bool>("BatchIsLive", ref _batchIsLive, value); }
    }


    private DateTime _collateDate;
    /// <summary>
    /// Start date/time when data was collated for the batch
    /// </summary>
    [Persistent, Indexed]
    public DateTime CollateDate
    {
      get { return _collateDate; }
      set { SetPropertyValue<DateTime>("CollateDate", ref _collateDate, value); }
    }


    /// <summary>
    /// Period covered by batch- start date
    /// </summary>
    private DateTime _batchPeriodStartDate;
    [Persistent, Indexed]
    public DateTime BatchPeriodStartDate
    {
      get { return _batchPeriodStartDate; }
      set { SetPropertyValue<DateTime>("BatchPeriodStartDate", ref _batchPeriodStartDate, value); }
    }

    /// <summary>
    /// Period covered by batch- end date
    /// </summary>
    private DateTime _batchPeriodEndDate;
    [Persistent]
    public DateTime BatchPeriodEndDate
    {
      get { return _batchPeriodEndDate; }
      set { SetPropertyValue<DateTime>("BatchPeriodEndDate", ref _batchPeriodEndDate, value); }
    }


    private DateTime _uploadedDate;
    /// <summary>
    /// Date/time when batch was successfully uploaded
    /// </summary>
    [Persistent]
    public DateTime UploadedDate
    {
      get { return _uploadedDate; }
      set { SetPropertyValue<DateTime>("UploadedDate", ref _uploadedDate, value); }
    }

    private DateTime _lastUploadAttempt;
    /// <summary>
    /// Date/time of last failed attempt
    /// </summary>
    [Persistent]
    public DateTime LastUploadAttempt
    {
      get { return _lastUploadAttempt; }
      set { SetPropertyValue<DateTime>("LastUploadAttempt", ref _lastUploadAttempt, value); }
    }

    private int _uploadAttemptCount;
    /// <summary>
    /// How many times have we tried to upoad
    /// </summary>
    [Persistent]
    public int UploadAttemptCount
    {
      get { return _uploadAttemptCount; }
      set { SetPropertyValue<int>("UploadAttemptCount", ref _uploadAttemptCount, value); }
    }

    private string _lastError;
    [Persistent, Size(int.MaxValue)]
    public string LastError
    {
      get { return _lastError; }
      set { SetPropertyValue<string>("LastError", ref _lastError, value); }
    }

    private byte[] _storageSystemRef;
    /// <summary>
    /// mongodb ZIP file storage id
    /// </summary>
    [Indexed] // _id field in mongodb is 12 bytes
    public byte[] StorageSystemRef
    {
      get { return _storageSystemRef; }
      set { SetPropertyValue("StorageSystemRef", ref _storageSystemRef, value); }
    }


    private string _storageFileName;
    [Persistent, Size(100)]
    public string StorageFileName
    {
      get { return _storageFileName; }
      set { SetPropertyValue<string>("StorageFileName", ref _storageFileName, value); }
    }


    /// <summary>
    /// Which system created?
    /// </summary>
    private Enumerators.General.Host _host;
    [Persistent]
    public Enumerators.General.Host Host
    {
      get { return _host; }
      set { SetPropertyValue<Enumerators.General.Host>("HostId", ref _host, value); }
    }


    /// <summary>
    /// What is the state of the staging process
    /// </summary>
    private Enumerators.Evolution.StagingState _stagingState;
    [Persistent]
    public Enumerators.Evolution.StagingState StagingState
    {
      get { return _stagingState; }
      set { SetPropertyValue<Enumerators.Evolution.StagingState>("StagingState", ref _stagingState, value); }
    }

    /// <summary>
    /// Many-2-many- snapshots included in this batch
    /// </summary>
    [Association("EVO_LoanTrackSnapshot_UploadBatch", UseAssociationNameAsIntermediateTableName = true)]
    public XPCollection<EVO_LoanTrackSnapshot> LoanTrackSnapshots
    {
      get { return GetCollection<EVO_LoanTrackSnapshot>("LoanTrackSnapshots"); }
    }


    public EVO_UploadBatch() : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public EVO_UploadBatch(Session session) : base(session)
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