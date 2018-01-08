using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PRD_ProductType : XPCustomObject
  {
    private Int64 _productTypeId;
    [Key(AutoGenerate = true)]
    public Int64 ProductTypeId
    {
      get
      {
        return _productTypeId;
      }
      set
      {
        SetPropertyValue("ProductTypeId", ref _productTypeId, value);
      }
    }

    private string _description;
    [Persistent, Size(25)]
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

    private string _assemblyName;
    [Persistent, Size(100)]
    public string AssemblyName
    {
      get
      {
        return _assemblyName;
      }
      set
      {
        SetPropertyValue("AssemblyName", ref _assemblyName, value);
      }
    }

    private string _searchField1;
    [Persistent, Size(80)]
    public string SearchField1
    {
      get
      {
        return _searchField1;
      }
      set
      {
        SetPropertyValue("SearchField1", ref _searchField1, value);
      }
    }

    private string _searchField2;
    [Persistent, Size(80)]
    public string SearchField2
    {
      get
      {
        return _searchField2;
      }
      set
      {
        SetPropertyValue("SearchField2", ref _searchField2, value);
      }
    }

    public PRD_ProductType() : base() { }
    public PRD_ProductType(Session session) : base(session) { }
  }
}