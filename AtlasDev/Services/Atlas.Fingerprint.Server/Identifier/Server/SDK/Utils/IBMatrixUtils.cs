using System;


namespace Atlas.FP.Identifier.SDK.Utils
{
  /// <summary>
  /// Useful matrix utilities
  /// </summary>
  internal static class IBMatrixUtils
  { 
    /// <summary>
    /// Rotate an image 180 degrees- basically just reverse the 1D array to achieve this
    /// </summary>
    /// <param name="source">1D array</param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns>Rotated 1D array</returns>
    internal static byte[] Rotate180(byte[] source)
    {
      var result = new byte[source.Length];
      Array.Copy(source, result, source.Length);
      Array.Reverse(result);
      return result;
    }
    
  }
}
