using Atlas.Enumerators;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public class BUR_BandRange : XPLiteObject
  {
    private int _bandRangeId;
    [Key(AutoGenerate = true)]
    public int BandRangeId
    {
      get
      {
        return _bandRangeId;
      }
      set
      {
        SetPropertyValue("BandRangeId", ref _bandRangeId, value);
      }
    }

    private BUR_Band _band;
    [Persistent("BandId")]
    public BUR_Band Band
    {
      get
      {
        return _band;
      }
      set
      {
        SetPropertyValue("Band", ref _band, value);
      }
    }

    private int _start;
    [Persistent]
    public int Start
    {
      get
      {
        return _start;
      }
      set
      {
        SetPropertyValue("Start", ref _start, value);
      }
    }

    private int _end;
    [Persistent]
    public int End
    {
      get
      {
        return _end;
      }
      set
      {
        SetPropertyValue("End", ref _end, value);
      }
    }


    #region Constructors

    public BUR_BandRange() : base() { }
    public BUR_BandRange(Session session) : base(session) { }

    #endregion
  }
}
