using System;
using Atlas.Common.Extensions;
using DevExpress.Xpo;

namespace Stream.Domain.Models
{
  public class STR_Frequency : XPLiteObject
  {
    private Int32 _frequencyId;
    [Key]
    public Int32 FrequencyId
    {
      get
      {
        return _frequencyId;
      }
      set
      {
        SetPropertyValue("FrequencyId", ref _frequencyId, value);
      }
    }

    [NonPersistent]
    public Framework.Enumerators.Stream.FrequencyType Type
    {
      get
      {
        return Description.FromStringToEnum<Framework.Enumerators.Stream.FrequencyType>();
      }
      set
      {
        value = Description.FromStringToEnum<Framework.Enumerators.Stream.FrequencyType>();
      }
    }

    private string _description;
    [Persistent, Size(20)]
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

    public STR_Frequency()
    { }
    public STR_Frequency(Session session) : base(session) { }

    #endregion

  }
}
