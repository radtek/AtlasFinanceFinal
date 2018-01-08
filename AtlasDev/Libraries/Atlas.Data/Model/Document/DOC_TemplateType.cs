using System;
using DevExpress.Xpo;

using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
{
  /// <summary>
  /// Stores enumerator 'Enumerators.Document.DocumentTemplate'
  /// </summary>
  public class DOC_TemplateType : XPLiteObject
  {   
    [Persistent("TemplateTypeId")]
    [Key(AutoGenerate = false)]
    public int TemplateTypeId { get; set; }

    private Enumerators.Document.DocumentTemplate _templateType;
    [Persistent("DocTemplateType")]
    public Enumerators.Document.DocumentTemplate DocTemplateType
    {
      get { return _templateType; }
      set { SetPropertyValue<Enumerators.Document.DocumentTemplate>("DocTemplateType", ref _templateType, value); }
    }

    private string _description;
    [Persistent, Size(int.MaxValue)]
    public string Description
    {
      get { return _description; }
      set { SetPropertyValue("Description", ref _description, value); }
    }

    private DOC_Category _category;
    [Persistent("CategoryId")]
    public DOC_Category Category
    {
      get { return _category; }
      set { SetPropertyValue("Category", ref _category, value); }
    }

    private DateTime _createDate;
    [Persistent("CreateDate")]
    public DateTime CreateDate
    {
      get { return _createDate; }
      set { SetPropertyValue("CreateDate", ref _createDate, value); }
    }


    public DOC_TemplateType()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public DOC_TemplateType(Session session)
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