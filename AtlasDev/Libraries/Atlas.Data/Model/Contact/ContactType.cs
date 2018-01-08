using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Atlas.Domain.Model
{
  public class ContactType : XPLiteObject
  {

    private long _contactTypeId;
    [Key(AutoGenerate = false)]
    public long ContactTypeId
    {
      get
      {
        return _contactTypeId;
      }
      set
      {
        SetPropertyValue("ContactTypeId", ref _contactTypeId, value);
      }
    }


    private string _description;
    [Persistent]
    [Indexed(Name = "IX_CONTACTYPE_DESC")]
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

    public ContactType()
    { }
    public ContactType(Session session) : base(session) { }

    #endregion
  }
}
