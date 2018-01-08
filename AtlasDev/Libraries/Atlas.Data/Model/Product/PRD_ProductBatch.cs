using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class PRD_ProductBatch : XPCustomObject
  {
    private Int64 _productBatchId;
    [Key(AutoGenerate = true)]
    public Int64 ProductBatchId
    {
      get
      {
        return _productBatchId;
      }
      set
      {
        SetPropertyValue("ProductBatchId", ref _productBatchId, value);
      }
    }

    private PRD_ProductType _productType;
    [Persistent("ProductTypeId")]
    public PRD_ProductType ProductType
    {
      get
      {
        return _productType;
      }
      set
      {
        SetPropertyValue("ProductType", ref _productType, value);
      }
    }

    private Enumerators.General.ProductBatchStatus _Status;
    [Persistent]
    [Indexed]
    public Enumerators.General.ProductBatchStatus Status
    {
      get
      {
        return _Status;
      }
      set
      {
        SetPropertyValue("Status", ref _Status, value);
      }
    }

    private CPY_Company _courier;
    [Persistent]
    [Indexed]
    public CPY_Company Courier
    {
      get
      {
        return _courier;
      }
      set
      {
        SetPropertyValue("Courier", ref _courier, value);
      }

    }

    private string _trackingNum;
    [Persistent]
    public string TrackingNum
    {
      get
      {
        return _trackingNum;
      }
      set
      {
        SetPropertyValue("TrackingNum", ref _trackingNum, value);
      }
    }

    private BRN_Branch _deliverToBranch;
    [Persistent]
    [Indexed]
    public BRN_Branch DeliverToBranch
    {
      get
      {
        return _deliverToBranch;
      }
      set
      {
        SetPropertyValue("DeliverToBranch", ref _deliverToBranch, value);
      }
    }

    private DateTime _capturedDT;
    [Persistent]
    [Indexed]
    public DateTime CapturedDT
    {
      get
      {
        return _capturedDT;
      }
      set
      {
        SetPropertyValue("CapturedDT", ref _capturedDT, value);
      }
    }

    private DateTime? _deliveryDT;
    [Persistent]
    public DateTime? DeliveryDT
    {
      get
      {
        return _deliveryDT;
      }
      set
      {
        SetPropertyValue("DeliveryDT", ref _deliveryDT, value);
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

    private PER_Person _capturedBy;
    [Persistent]
    public PER_Person CapturedBy
    {
      get
      {
        return _capturedBy;
      }
      set
      {
        SetPropertyValue("CapturedBy", ref _capturedBy, value);
      }
    }

    private PER_Person _deliverySetBy;
    [Persistent]
    public PER_Person DeliverySetBy
    {
      get
      {
        return _deliverySetBy;
      }
      set
      {
        SetPropertyValue("DeliverySetBy", ref _deliverySetBy, value);
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

    private int _quantity;
    [Persistent]
    public int Quantity
    {
      get
      {
        return _quantity;
      }
      set
      {
        SetPropertyValue("Quantity", ref _quantity, value);
      }
    }

    private string _comment;
    [Persistent]
    public string Comment
    {

      get
      {
        return _comment;
      }
      set
      {
        SetPropertyValue("Comment", ref _comment, value);
      }
    }

    public PRD_ProductBatch() : base() { }
    public PRD_ProductBatch(Session session) : base(session) { }
  }
}