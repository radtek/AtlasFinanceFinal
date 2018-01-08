using Atlas.Enumerators;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Atlas.Common.Extensions;

namespace Atlas.Domain.Model
{
  public class BUR_Band : XPLiteObject
  {
    private int _bandId;
    [Key(AutoGenerate = true)]
    public int BandId
    {
      get
      {
        return _bandId;
      }
      set
      {
        SetPropertyValue("BandId", ref _bandId, value);
      }
    }

    [NonPersistent]
    public Enumerators.Risk.Band Type
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Risk.Band>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Risk.Band>();
      }
    }

    private string _description;
    [Persistent]
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

    private bool _pass;
    [Persistent]
    public bool Pass
    {
      get
      {
        return _pass;
      }
      set
      {
        SetPropertyValue("Pass", ref _pass, value);
      }
    }

    #region Constructors

    public BUR_Band() : base() { }
    public BUR_Band(Session session) : base(session) { }

    #endregion
  }
}
