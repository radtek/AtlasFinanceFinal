using Atlas.Enumerators;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Model
{
  public sealed class BUR_Colour : XPLiteObject
  {
    private Int64 _colourId;
    [Key(AutoGenerate = true)]
    public Int64 ColourId
    {
      get
      {
        return _colourId;
      }
      set
      {
        SetPropertyValue("ColourId", ref _colourId, value);
      }
    }

    private BUR_Enquiry _enquiryId;
    [Persistent("EnquiryId")]
    public BUR_Enquiry EnquiryId
    {
      get
      {
        return _enquiryId;
      }
      set
      {
        SetPropertyValue("EnquiryId", ref _enquiryId, value);
      }
    }

    private string _score;
    [Persistent, Size(10)]
    public string Score
    {
      get
      {
        return _score;
      }
      set
      {
        SetPropertyValue("Score", ref _score, value);
      }
    }

    private int _r;
    [Persistent]
    public int R
    {
      get
      {
        return _r;
      }
      set
      {
        SetPropertyValue("R", ref _r, value);
      }
    }

    private int _g;
    [Persistent]
    public int G
    {
      get
      {
        return _g;
      }
      set
      {
        SetPropertyValue("G", ref _g, value);
      }
    }

    private int _b;
    [Persistent]
    public int B
    {
      get
      {
        return _b;
      }
      set
      {
        SetPropertyValue("B", ref _b, value);
      }
    }


    #region Constructors

    public BUR_Colour() : base() { }
    public BUR_Colour(Session session) : base(session) { }

    #endregion
  }
}
