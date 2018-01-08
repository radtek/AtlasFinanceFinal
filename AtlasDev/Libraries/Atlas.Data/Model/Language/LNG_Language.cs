using System;
using DevExpress.Xpo;

using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
{
  public class LNG_Language : XPLiteObject
  {
    private Int64 _LanguageId;
    [Key(AutoGenerate = false)]
    public Int64 LanguageId
    {
      get { return _LanguageId; }
      set { SetPropertyValue("LanguageId", ref _LanguageId, value); }
    }

    [NonPersistent]
    public Enumerators.General.Language Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.Language>(); }
      set { value = Description.FromStringToEnum<Enumerators.General.Language>(); }
    }

    private string _Description;
    [Persistent, Size(100)]
    public string Description
    {
      get { return _Description; }
      set { SetPropertyValue("Description", ref _Description, value); }
    }

    public LNG_Language()
      : base()
    {
      // This constructor is used when an object is loaded from a persistent storage.
      // Do not place any code here.
    }

    public LNG_Language(Session session)
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