using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PRD_Product : XPCustomObject
  {
    private Int64 _productId;
    [Key(AutoGenerate = true)]
    public Int64 ProductId
    {
      get
      {
        return _productId;
      }
      set
      {
        SetPropertyValue("ProductId", ref _productId, value);
      }
    }

    private PRD_ProductBatch _productBatch;
    [Persistent("ProductBatchId")]
    public PRD_ProductBatch ProductBatch
    {
      get
      {
        return _productBatch;
      }
      set
      {
        SetPropertyValue("ProductBatch", ref _productBatch, value);
      }
    }

    private PER_Person _AllocatedPerson;
    [Persistent("PersonId")]
    public PER_Person AllocatedPerson
    {
      get
      {
        return _AllocatedPerson;
      }
      set
      {
        SetPropertyValue("AllocatedPerson", ref _AllocatedPerson, value);
      }
    }


    private string _xmlObject;
    [Persistent, Size(2000)]
    public string XmlObject
    {
      get
      {
        return _xmlObject;
      }
      set
      {
        SetPropertyValue("XmlObject", ref _xmlObject, value);
      }
    }

    private string _searchValue1;
    [Persistent, Size(80)]
    [Indexed]
    public string SearchValue1
    {
      get
      {
        return _searchValue1;
      }
      set
      {
        SetPropertyValue("SearchValue1", ref _searchValue1, value);
      }
    }

    private string _searchValue2;
    [Persistent, Size(80)]
    [Indexed]
    public string SearchValue2
    {
      get
      {
        return _searchValue2;
      }
      set
      {
        SetPropertyValue("SearchValue2", ref _searchValue2, value);
      }
    }

    private PER_Person _createdBy;
    [Persistent]
    public PER_Person CreatedBy
    {
      get
      {
        return _createdBy;
      }
      set
      {
        SetPropertyValue("CreatedBy", ref _createdBy, value);
      }
    }

    private PER_Person _receivedBy;
    [Persistent]
    public PER_Person ReceivedBy
    {
      get
      {
        return _receivedBy;
      }
      set
      {
        SetPropertyValue("ReceivedBy", ref _receivedBy, value);
      }
    }

    private PER_Person _allocatedBy;
    [Persistent]
    public PER_Person AllocatedBy
    {
      get
      {
        return _allocatedBy;
      }
      set
      {
        SetPropertyValue("AllocatedBy", ref _allocatedBy, value);
      }
    }

    private PER_Person _lastEditedBy;
    [Persistent]
    public PER_Person LastEditedBy
    {
      get
      {
        return _lastEditedBy;
      }
      set
      {
        SetPropertyValue("LastEditedBy", ref _lastEditedBy, value);
      }
    }

    private DateTime _createdDT;
    [Persistent]
    public DateTime CreatedDT
    {
      get
      {
        return _createdDT;
      }
      set
      {
        SetPropertyValue("CreatedDT", ref _createdDT, value);
      }
    }

    private DateTime? _receivedDT;
    [Persistent]
    public DateTime? ReceivedDT
    {
      get
      {
        return _receivedDT;
      }
      set
      {
        SetPropertyValue("ReceivedDT", ref _receivedDT, value);
      }
    }

    private DateTime? _allocatedDT;
    [Persistent]
    public DateTime? AllocatedDT
    {
      get
      {
        return _allocatedDT;
      }
      set
      {
        SetPropertyValue("AllocatedDT", ref _allocatedDT, value);
      }
    }

    private DateTime? _lastEditedDT;
    [Persistent]
    public DateTime? LastEditedDT
    {
      get
      {
        return _lastEditedDT;
      }
      set
      {
        SetPropertyValue("LastEditedDT", ref _lastEditedDT, value);
      }
    }

    public PRD_Product() : base() { }
    public PRD_Product(Session session) : base(session) { }
  }
}