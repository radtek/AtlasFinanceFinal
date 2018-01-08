using System;

using FileHelpers;


namespace Atlas.Evolution.Server.Code.Layout
{
  [FixedLengthRecord()]
  public class Monthly_Trailer
  {
    [FieldFixedLength(1)]
    public string TRAILER = "T";

    [FieldFixedLength(9)]
    [FieldAlign(AlignMode.Right, ' ')]
    public uint NUMBER_OF_RECORDS;

    [FieldFixedLength(690)]
    public string FILLER;
  }
}
