
namespace Atlas.Domain.Model
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using DevExpress.Xpo;

  /// <summary>
  /// TODO: Update summary.
  /// </summary>
  public class BUR_Batch : XPLiteObject
  {
    private Int64 _batchId;
    [Key(AutoGenerate = true)]
    public Int64 BatchId
    {
      get
      {
        return _batchId;
      }
      set
      {
        SetPropertyValue("BatchId", ref _batchId, value);
      }
    }

    private BRN_Branch _branch;
    [Persistent("BranchId"), Indexed]
    public BRN_Branch Branch
    {
      get
      {
        return _branch;
      }
      set
      {
        SetPropertyValue("Branch", ref _branch, value);
      }
    }

    private DateTime? _deliveryDate;
    [Persistent, Indexed]
    public DateTime? DeliveryDate
    {
      get
      {
        return _deliveryDate;
      }
      set
      {
        SetPropertyValue("DeliveryDate", ref _deliveryDate, value);
      }
    }

    private bool _locked;
    [Persistent]
    public bool Locked
    {
      get
      {
        return _locked;
      }
      set
      {
        SetPropertyValue("Locked", ref _locked, value);
      }
    }

    private PER_Person _createUser;
    [Persistent]
    public PER_Person CreateUser
    {
      get
      {
        return _createUser;
      }
      set
      {
        SetPropertyValue("CreateUser", ref _createUser, value);
      }
    }

    private DateTime? _createdDate;
    [Persistent]
    public DateTime? CreatedDate
    {
      get
      {
        return _createdDate;
      }
      set
      {
        SetPropertyValue("CreatedDate", ref _createdDate, value);
      }
    }


    #region Constructors

    public BUR_Batch() : base() { }
    public BUR_Batch(Session session) : base(session) { }

    #endregion
  }
}
