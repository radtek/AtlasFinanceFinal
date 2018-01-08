using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_AccountNoteType : XPLiteObject
  {
    private int _accountNoteTypeId;
    [Key]
    public int AccountNoteTypeId
    {
      get
      {
        return _accountNoteTypeId;
      }
      set
      {
        SetPropertyValue("AccountTypeId", ref _accountNoteTypeId, value);
      }
    }

    [NonPersistent]
    public Framework.Enumerators.Stream.AccountNoteType AccountNoteType
    {
      get
      {
        return Description.FromStringToEnum<Framework.Enumerators.Stream.AccountNoteType>();
      }
      set
      {
        value = Description.FromStringToEnum<Framework.Enumerators.Stream.AccountNoteType>();
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

    public STR_AccountNoteType()
    { }
    public STR_AccountNoteType(Session session) : base(session) { }

    #endregion

  }
}