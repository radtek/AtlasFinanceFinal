using System;

using DevExpress.Xpo;

using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
{
  public class STR_TransactionStatus : XPLiteObject
  {
    private int _transactionStatusId;
    [Key]
    public int TransactionStatusId
    {
      get
      {
        return _transactionStatusId;
      }
      set
      {
        SetPropertyValue("TransactionStatusId", ref _transactionStatusId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Stream.TransactionStatus Status
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.TransactionStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.TransactionStatus>();
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

    public STR_TransactionStatus() : base() { }
    public STR_TransactionStatus(Session session) : base(session) { }

    #endregion

  }
}