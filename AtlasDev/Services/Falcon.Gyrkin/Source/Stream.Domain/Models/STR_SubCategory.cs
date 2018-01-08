using System;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Stream.Domain.Models
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

    public STR_SubCategory()
    { }
    public STR_SubCategory(Session session) : base(session) { }

    #endregion

  }
}