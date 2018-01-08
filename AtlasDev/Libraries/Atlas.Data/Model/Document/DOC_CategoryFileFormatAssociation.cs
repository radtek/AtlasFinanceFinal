using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public sealed class DOC_CategoryFileFormatAssociation : XPLiteObject
  {
    private Int64 _categoryFileFormatAssociationId;
    [Key(AutoGenerate = true)]
    [Persistent("CategoryFileFormatAssociationId")]
    public Int64 CategoryFileFormatAssociationId
    {
      get { return _categoryFileFormatAssociationId; }
      set { SetPropertyValue("CategoryFileFormatAssociationId", ref _categoryFileFormatAssociationId, value); }
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
    [Persistent("FileFormatType")]
    public DOC_FileFormatType FileFormatType
    {
      get { return _fileFormatType; }
      set { SetPropertyValue("FileFormatType", ref _fileFormatType, value); }
    }

    private bool _enabled;
    [Indexed]
    public bool Enabled
    {
      get { return _enabled; }
      set { SetPropertyValue("Enabled", ref _enabled, value); }
    }

    private DateTime _createDate;
    [Indexed]
    public DateTime CreateDate
    {
      get { return _createDate; }
      set { SetPropertyValue("CreateDate", ref _createDate, value); }
    }


    #region Constructors

    public DOC_CategoryFileFormatAssociation() : base() { }
    public DOC_CategoryFileFormatAssociation(Session session) : base(session) { }

    #endregion

  }
}
