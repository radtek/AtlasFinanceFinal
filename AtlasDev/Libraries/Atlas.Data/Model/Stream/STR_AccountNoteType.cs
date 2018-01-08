using System;

using DevExpress.Xpo;

using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
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
    public Enumerators.Stream.AccountNoteType AccountNoteType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.AccountNoteType>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.AccountNoteType>();
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

    public STR_AccountNoteType() : base() { }
    public STR_AccountNoteType(Session session) : base(session) { }

    #endregion

  }
}