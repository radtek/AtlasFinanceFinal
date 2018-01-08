using System;

using DevExpress.Xpo;

using AccountEnums = Atlas.Enumerators.Account;


namespace Atlas.Domain.Model.Evolution
{
  /// <summary>
  /// A single loan for tracking- put *fixed* loan details into this class
  /// </summary>  
  public class EVO_LoanTrack : XPLiteObject
  {
    [Key(AutoGenerate = true)]
    public Int64 LoanTrackId { get; set; }

    private BRN_Branch _branch;
    [Persistent, Indexed]
    public BRN_Branch Branch
    {
      get { return _branch; }
      set { SetPropertyValue<BRN_Branch>("Branch", ref _branch, value); }
    }

    private Int64 _loanRecId;
    [Persistent, Indexed]
    public Int64 LoanRecId
    {
      get { return _loanRecId; }
      set { SetPropertyValue<Int64>("LoanRecId", ref _loanRecId, value); }
    }

    private Int64 _clientRecId;
    [Persistent, Indexed]
    public Int64 ClientRecId
    {
      get { return _clientRecId; }
      set { SetPropertyValue<Int64>("ClientRecId", ref _clientRecId, value); }
    }

    private string _assClient;
    [Persistent, Indexed, Size(5)]
    public string AssClient
    {
      get { return _assClient; }
      set { SetPropertyValue<string>("AssClient", ref _assClient, value); }
    }

    private string _assLoan;
    [Persistent, Indexed, Size(4)]
    public string AssLoan
    {
      get { return _assLoan; }
      set { SetPropertyValue<string>("AssLoan", ref _assLoan, value); }
    }

    private DateTime _assLoanStartDate;
    [Persistent]
    public DateTime AssLoanStartDate
    {
      get { return _assLoanStartDate; }
      set { SetPropertyValue<DateTime>("AssLoanStartDate", ref _assLoanStartDate, value); }
    }

    private Decimal _loanAmount;
    [Persistent]
    public decimal LoanAmount
    {
      get { return _loanAmount; }
      set { SetPropertyValue<decimal>("LoanAmount", ref _loanAmount, value); }
    }

    private string _assLoanReason;
    [Persistent, Size(3)]
    public string AssLoanReason
    {
      get { return _assLoanReason; }
      set { SetPropertyValue<string>("AssLoanReason", ref _assLoanReason, value); }
    }
    
    [Association("EVO_LoanTrack_Snapshot", UseAssociationNameAsIntermediateTableName =true)]
    public XPCollection<EVO_LoanTrackSnapshot> LoanTrackSnapshots
    {
      get { return GetCollection<EVO_LoanTrackSnapshot>("LoanTrackSnapshots"); }
    }
    

    public EVO_LoanTrack() : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public EVO_LoanTrack(Session session) : base(session)
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