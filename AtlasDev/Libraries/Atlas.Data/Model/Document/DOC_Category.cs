using System;

using DevExpress.Xpo;

using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
{
  /// <summary>
  /// Stores enumerator 'Enumerators.Document.Category'
  /// </summary>
  public sealed class DOC_Category : XPLiteObject
  {  
    [Key(AutoGenerate = false)]   
    public Int64 CategoryId { get; set; }

    private Enumerators.Document.Category _category;
    [Persistent("DocCategoryEnum")]
    public Enumerators.Document.Category Category
    {
      get { return _category; }
      set { SetPropertyValue<Enumerators.Document.Category>("Category", ref _category, value); }
    }

    private DOC_Category _parentCategory;
    [Indexed]
    [Persistent("ParentCategoryId")]
    public DOC_Category ParentCategory
    {
      get { return _parentCategory; }
      set { SetPropertyValue("ParentCategory", ref _parentCategory, value); }
    }

    private string _description;
    [Persistent, Size(500)]
    public string Description
    {
      get { return _description; }
      set { SetPropertyValue("Description", ref _description, value); }
    }

    private bool _enabled;
    [Indexed]
    public bool Enabled
    {
      get { return _enabled; }
      set { SetPropertyValue("Enabled", ref _enabled, value); }
    }

    private DateTime _createDate;
    [Persistent("CreateDate"), Indexed]
    public DateTime CreateDate
    {
      get { return _createDate; }
      set { SetPropertyValue("CreateDate", ref _createDate, value); }
    }


    #region Constructors

    public DOC_Category() : base() { }
    public DOC_Category(Session session) : base(session) { }

    #endregion

  }
}
