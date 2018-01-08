using System;

using Atlas.Common.Extensions;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class STR_SubCategory : XPLiteObject
  {
    private int _subCategoryId;
    [Key]
    public int SubCategoryId
    {
      get
      {
        return _subCategoryId;
      }
      set
      {
        SetPropertyValue("SubCategoryId", ref _subCategoryId, value);
      }
    }

    private STR_Category _category;
    [Persistent("CategoryId")]
    public STR_Category Category
    {
      get
      {
        return _category;
      }
      set
      {
        SetPropertyValue("Category", ref _category, value);
      }
    }

    [NonPersistent]
    public Enumerators.Stream.SubCategory SubCategoryType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.SubCategory>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.SubCategory>();
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

    private DateTime? _disableDate;
    [Persistent]
    public DateTime? DisableDate
    {
      get
      {
        return _disableDate;
      }
      set
      {
        SetPropertyValue("DisableDate", ref _disableDate, value);
      }
    }


    #region Constructors

    public STR_SubCategory() : base() { }
    public STR_SubCategory(Session session) : base(session) { }

    #endregion

  }
}