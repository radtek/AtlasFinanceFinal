using System;
using System.IO;


namespace Atlas.DocServer.Utils
{
  public static class StreamUtils
  {
    /// <summary>
    /// Converts a Stream to a byte array
    /// </summary>
    /// <param name="source">The source Stream</param>
    /// <returns>A byte array of the stream</returns>
    public static byte[] StreamToByte(this Stream source)
    {
      if (source == null)
      {
        return null;
      }

      using (var ms = new MemoryStream())
      {
        source.Position = 0;
        source.CopyTo(ms);
        return ms.ToArray();
      }
    }
  }
}
