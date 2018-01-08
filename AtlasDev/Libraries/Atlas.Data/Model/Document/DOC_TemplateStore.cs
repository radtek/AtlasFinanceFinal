using System;
using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  /// <summary>
  /// Stores template definition used to generate other documents (i.e. report, mail-merge source document, etc.)
  /// </summary>
  public sealed class DOC_TemplateStore : XPLiteObject
  {   
    [Key(AutoGenerate = true)]  
    public Int64 TemplateId { get; set; }

    private DOC_TemplateType _templateType;
    [Persistent("TemplateTypeId")]
    [Indexed]
    public DOC_TemplateType TemplateType
    {
      get { return _templateType; }
      set { SetPropertyValue("TemplateTypeId", ref _templateType, value); }
    }

    private byte[] _fileBytes;
    [Persistent("FileBytes")]
    public byte[] FileBytes
    {
      get { return _fileBytes; }
      set { SetPropertyValue("FileBytes", ref _fileBytes, value); }
    }


    private string _comment;
    [Persistent]
    [Size(int.MaxValue)]
    public string Comment
    {
      get { return _comment; }
      set { SetPropertyValue("Comment", ref _comment, value); }
    }


    private DOC_FileFormatType _templateFileFormat;
    [Persistent("TemplateFileFormatTypeId")]
    public DOC_FileFormatType TemplateFileFormat
    {
      get { return _templateFileFormat; }
      set { SetPropertyValue("TemplateFileFormat", ref _templateFileFormat, value); }
    }

    private int _revision;
    [Persistent]
    public int Revision
    {
      get { return _revision; }
      set { SetPropertyValue("Revision", ref _revision, value); }
    }

    private LNG_Language _language;
    [Persistent]
    public LNG_Language Language
    {
      get { return _language; }
      set { SetPropertyValue("Language", ref _language, value); }
    }

    private PER_Person _createdBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get { return _createdBy; }
      set { SetPropertyValue("CreatedBy", ref _createdBy, value); }
    }

    private DateTime _createDate;
    [Persistent("CreateDate")]
    public DateTime CreateDate
    {
      get { return _createDate; }
      set { SetPropertyValue("CreateDate", ref _createDate, value); }
    }


    #region Constructors

    public DOC_TemplateStore() : base() { }
    public DOC_TemplateStore(Session session) : base(session) { }

    #endregion

  }
}
