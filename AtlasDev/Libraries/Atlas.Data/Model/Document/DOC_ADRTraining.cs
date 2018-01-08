using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model.Document
{
  /// <summary>
  /// Storage of GdPicture templates for Automatic Document Recognition (ADR) template training
  /// </summary>
  public class DOC_ADRTraining : XPLiteObject
  {     
    [Key(AutoGenerate = true)]
    public Int64 ADRTrainingId { get; set; }

    private byte[] _fileBytes;
    [Persistent]
    public byte[] FileBytes
    {
      get { return _fileBytes; }
      set { SetPropertyValue<byte[]>("FileBytes", ref _fileBytes, value); }
    }
       
    private Atlas.Enumerators.Document.DocumentTemplate _docTemplate;
    [Persistent]
    public Atlas.Enumerators.Document.DocumentTemplate DocTemplate
    {
      get { return _docTemplate; }
      set { SetPropertyValue<Atlas.Enumerators.Document.DocumentTemplate>("DocTemplate", ref _docTemplate, value); }
    }

    private DateTime _createDate;
    [Persistent("CreateDate")]
    public DateTime CreateDate
    {
      get { return _createDate; }
      set { SetPropertyValue<DateTime>("CreateDate", ref _createDate, value); }
    }

    private PER_Person _createdBy;
    public PER_Person CreatedBy
    {
      get { return _createdBy; }
      set { SetPropertyValue<PER_Person>("CreatedBy", ref _createdBy, value); }
    }

    private string _comment;
    [Size(int.MaxValue)]
    [Persistent("Comment")]
    public string Comment
    {
      get { return _comment; }
      set { SetPropertyValue<string>("Comment", ref _comment, value); }
    }


    #region Constructors

    public DOC_ADRTraining()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public DOC_ADRTraining(Session session)
      : base(session)
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    #endregion


    public override void AfterConstruction()
    {
      base.AfterConstruction();
      // Place here your initialization code.
    }

  }
}