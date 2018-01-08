using System;
using DevExpress.Xpo;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  /// <summary>
  /// Stores enumerator 'Enumerators.Document.FileFormat'
  /// </summary>
  public sealed class DOC_FileFormatType : XPLiteObject
  {
    private Int64 _fileFormatTypeId;
    [Key(AutoGenerate = false)]
    [Persistent("FileFormatTypeId")]
    public Int64 FileFormatTypeId
    {
      get { return _fileFormatTypeId; }
      set { SetPropertyValue("FileFormatTypeId", ref _fileFormatTypeId, value); }
    }

    [NonPersistent]
    public Enumerators.Document.FileFormat Type
    {
      get { return Description.FromStringToEnum<Enumerators.Document.FileFormat>(); }
      set { value = Description.FromStringToEnum<Enumerators.Document.FileFormat>(); }
    }

    private string _description;
    [Persistent,Size(500)]
    public string Description
    {
      get { return _description; }
      set { SetPropertyValue("Description", ref _description, value); }
    }

    private DateTime _createDate;
    [Persistent("CreateDate")]
    public DateTime CreateDate
    {
      get { return _createDate; }
      set { SetPropertyValue("CreateDate", ref _createDate, value); }
    }

    #region Constructors

    public DOC_FileFormatType() : base() { }
    public DOC_FileFormatType(Session session) : base(session) { }

    #endregion

  }
}
