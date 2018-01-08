using Atlas.Common.Extensions;
using Atlas.Enumerators;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class PYT_Validation: XPLiteObject
  {
    private int _validationId;
    [Key]
    public int ValidationId
    {
      get
      {
        return _validationId;
      }
      set
      {
        SetPropertyValue("ValidationId", ref _validationId, value);
      }
    }

    [NonPersistent]
    public Payout.Validation Type
    {
      get
      {
        return Description.FromStringToEnum<Payout.Validation>();
      }
      set
      {
        value = Description.FromStringToEnum<Payout.Validation>();
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

    #region Constructors

    public PYT_Validation() : base() { }
    public PYT_Validation(Session session) : base(session) { }

    #endregion
  }
}
