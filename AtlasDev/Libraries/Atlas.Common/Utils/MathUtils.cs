using System;


namespace Atlas.Common.Utils
{
  public static class MathUtils
  {
    /// <summary>
    /// Returns a specified decimal raised to the specified power.
    /// </summary>
    /// <param name="value">A number to be raised to a power</param>
    /// <param name="power">A number that specifies a power</param>
    /// <returns></returns>
    public static decimal Pow(decimal value, int power)
    {
      decimal result = 1;
      for (; power > 0; power--)
      {
        result *= value;
      }
      return result;
    }

  
  }
}
