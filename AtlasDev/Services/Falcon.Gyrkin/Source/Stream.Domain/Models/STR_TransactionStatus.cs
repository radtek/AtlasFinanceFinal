using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Stream.Domain.Models
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
    public Framework.Enumerators.Stream.TransactionStatus Status
    {
      get
      {
        return Description.FromStringToEnum<Framework.Enumerators.Stream.TransactionStatus>();
      }
      set
      {
        value = Description.FromStringToEnum<Framework.Enumerators.Stream.TransactionStatus>();
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

    public STR_TransactionStatus()
    { }
    public STR_TransactionStatus(Session session) : base(session) { }

    #endregion

  }
}