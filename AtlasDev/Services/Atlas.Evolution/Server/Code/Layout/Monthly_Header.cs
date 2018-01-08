using System;

using FileHelpers;


namespace Atlas.Evolution.Server.Code.Layout
{
  [FixedLengthRecord()]
  public class Monthly_Header
  {
    [FieldFixedLength(1)]
    public string HEADER = "H";

    [FieldFixedLength(10)]
    [FieldAlign(AlignMode.Right, ' ')]
    public string SUPPLIER_REFERENCE_NUMBER;

    [FieldFixedLength(8)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint MONTH_END_DATE;

    [FieldFixedLength(2)]
    public string VERSION_NUMBER = "06";

    [FieldFixedLength(8)]
    [FieldAlign(AlignMode.Right, '0')]
    public uint FILE_CREATION_DATE;

    [FieldFixedLength(60)]
    public string TRADING_NAME_BRAND_NAME = "Atlas Finance";

    [FieldFixedLength(611)]
    public string FILLER;
  }
}
