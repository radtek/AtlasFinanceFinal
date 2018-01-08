using System;

using DevExpress.Xpo;

using Atlas.Common.Extensions;


namespace Atlas.Domain.Model
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
    public Enumerators.Stream.Frequency Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.Frequency>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.Frequency>();
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

    public STR_Frequency() : base() { }
    public STR_Frequency(Session session) : base(session) { }

    #endregion

  }
}
