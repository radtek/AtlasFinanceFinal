using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public sealed class ACC_PeriodFrequency : XPLiteObject
  {
    private Int32 _periodFrequencyId;
    [Key]
    public Int32 PeriodFrequencyId
    {
      get
      {
        return _periodFrequencyId;
      }
      set
      {
        SetPropertyValue("PeriodFrequencyId", ref _periodFrequencyId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Account.PeriodFrequency Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Account.PeriodFrequency>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Account.PeriodFrequency>();
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

    public ACC_PeriodFrequency() : base() { }
    public ACC_PeriodFrequency(Session session) : base(session) { }

    #endregion
  }
}
