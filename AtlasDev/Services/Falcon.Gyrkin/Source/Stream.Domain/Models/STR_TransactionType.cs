using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_TransactionType : XPLiteObject
  {
    private int _transactionTypeId;
    [Key]
    public int TransactionTypeId
    {
      get
      {
        return _transactionTypeId;
      }
      set
      {
        SetPropertyValue("TransactionTypeId", ref _transactionTypeId, value);
      }
    }

    [NonPersistent]
    public Framework.Enumerators.Stream.TransactionType Type
    {
      get
      {
          return Description.FromStringToEnum<Framework.Enumerators.Stream.TransactionType>();
      }
      set
      {
        value = Description.FromStringToEnum<Framework.Enumerators.Stream.TransactionType>();
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

    public STR_TransactionType()
    { }
    public STR_TransactionType(Session session) : base(session) { }

    #endregion

  }
}