using System;


namespace Atlas.Common.Attributes
{
  public class SearchValueAttribute : Attribute
  {
    public int Num;
    public SearchValueAttribute(int num)
    {
      Num = num;
    }
  }

}
