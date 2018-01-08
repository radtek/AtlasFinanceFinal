using System;

using DevExpress.Xpo;

using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
{
  public class STR_Category : XPLiteObject
  {
    private int _categoryId;
    [Key]
    public int CategoryId
    {
      get
      {
        return _categoryId;
      }
      set
      {
        SetPropertyValue("CategoryId", ref _categoryId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Stream.Category CategoryType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.Category>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.Category>();
      }
    }

    private string _description;
    [Persistent, Size(40)]
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        SetPropertyValue("Description", ref _description, value);
      }
    }


    #region Constructors

    public STR_Category() : base() { }
    public STR_Category(Session session) : base(session) { }

    #endregion

  }
}