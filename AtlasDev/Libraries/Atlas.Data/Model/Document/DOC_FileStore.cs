using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  /// <summary>
  /// Storage information for document- actual document data to be stored in mongodb for replication/local access
  /// </summary>
  public sealed class DOC_FileStore : XPLiteObject
  { 
    [Key(AutoGenerate = true)]
    [Persistent("StorageId")]
    public Int64 StorageId { get; set; }

    private PER_Person _client;
    [Persistent("ClientId")]
    [Indexed]
    public PER_Person Client
    {
      get { return _client; }
      set { SetPropertyValue("Client", ref _client, value); }
    }

    private byte[] _storageSystemRef;
    [Indexed] // _id field in mongodb is 12 bytes
    public byte[] StorageSystemRef
    {
      get { return _storageSystemRef; }
      set { SetPropertyValue("StorageSystemRef", ref _storageSystemRef, value); }
    }

    private string _reference;
    [Indexed, Size(500)]
    public string Reference
    {
      get { return _reference; }
      set { SetPropertyValue("Reference", ref _reference, value); }
    }

    private DOC_Category _category;
    [Indexed]
    [Persistent("CategoryId")]
    public DOC_Category Category
    {
      get { return _category; }
      set { SetPropertyValue("Category", ref _category, value); }
    }
    
    private DOC_FileFormatType _fileFormatType;
    [Indexed]
    [Persistent("FileFormatTypeId")]
    public DOC_FileFormatType FileFormatType
    {
      get { return _fileFormatType; }
      set { SetPropertyValue("FileFormatType", ref _fileFormatType, value); }
    }

    private int _revision;
    [Indexed]
    [Persistent("Revision")]
    public int Revision
    {
      get { return _revision; }
      set { SetPropertyValue("Revision", ref _revision, value); }
    }
       

    private int _size;
    [Persistent("Size")]
    public int Size
    {
      get { return _size; }
      set { SetPropertyValue("Size", ref _size, value); }
    }

    private string _hash;
    [Size(128)]    // Provide storage for up to SHA-512 HEX (84 bytes base64 encoded)
    [Persistent("Hash")] 
    public string Hash
    {
      get { return _hash; }
      set { SetPropertyValue("Hash", ref _hash, value); }
    }

    private DOC_TemplateStore _sourceTemplate;
    [Persistent("TemplateId")]
    public DOC_TemplateStore SourceTemplate
    {
      get { return _sourceTemplate; }
      set { SetPropertyValue("SourceTemplate", ref _sourceTemplate, value); }
    }

    private DOC_FileStore _sourceDocument;
    [Persistent("SourceDocumentId")] // If a scanned document, link this scan document to the original system generated document
    public DOC_FileStore SourceDocument
    {
      get { return _sourceDocument; }
      set { SetPropertyValue("SourceDocumentId", ref _sourceDocument, value); }
    }

    private Enumerators.Document.Generator _generator;
    [Persistent]
    public Enumerators.Document.Generator Generator
    {
      get { return _generator; }
      set { SetPropertyValue("Generator", ref _generator, value); }
    }

    private Byte[] _sourceData;
    [Persistent]
    public Byte[] SourceData
    {
      get { return _sourceData; }
      set { SetPropertyValue("SourceData", ref _sourceData, value); }
    }

    private PER_Person _createdBy;
    [Persistent("CreatedBy")]
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

    private string _comment;
    [Size(int.MaxValue)]
    [Persistent("Comment")]
    public string Comment
    {
      get { return _comment; }
      set { SetPropertyValue("Comment", ref _comment, value); }
    }

    private string _keywords;
    [Persistent("Keywords")]
    [Size(500)]
    public string Keywords
    {
      get { return _keywords; }
      set { SetPropertyValue("Keywords", ref _keywords, value); }
    }


    #region Constructors

    public DOC_FileStore() : base() { }
    public DOC_FileStore(Session session) : base(session) { }

    #endregion
  }
}
