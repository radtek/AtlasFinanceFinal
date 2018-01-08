using System;

using DevExpress.Xpo;


namespace Atlas.Domain.Model
{
  public class STR_GroupStream : XPLiteObject
  {
    private int _groupStreamId;
    [Key(AutoGenerate = true)]
    public int GroupStreamId
    {
      get
      {
        return _groupStreamId;
      }
      set
      {
        SetPropertyValue("GroupStreamId", ref _groupStreamId, value);
      }
    }

    private STR_Group _group;
    [Persistent("GroupId")]
    public STR_Group Group
    {
      get
      {
        return _group;
      }
      set
      {
        SetPropertyValue("Group", ref _group, value);
      }
    }

    private STR_Stream _stream;
    [Persistent("StreamId")]
    public STR_Stream Stream
    {
      get
      {
        return _stream;
      }
      set
      {
        SetPropertyValue("Stream", ref _stream, value);
      }
    }

    private int _ordinal;
    public int Ordinal
    {
      get
      {
        return _ordinal;
      }
      set
      {
        SetPropertyValue("Ordinal", ref _ordinal, value);
      }
    }


    #region Constructors

    public STR_GroupStream() : base() { }
    public STR_GroupStream(Session session) : base(session) { }

    #endregion

  }
}