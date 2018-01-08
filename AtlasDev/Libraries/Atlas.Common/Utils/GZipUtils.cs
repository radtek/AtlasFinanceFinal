/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *    Useful GZip utilities
 * 
 * 
 *  Author:
 *  ------------------
 *     Keith Blows
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *       
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using System;
using System.IO;
using System.IO.Compression;
using System.Text;


namespace Atlas.Common.Utils
{
  public static class GZipUtils
  {
    //public static Encoding Encoding = System.Text.Encoding.Unicode;
    public static byte[] CompressString(string text, Encoding Encoding = null)
    {
      if (text == null) return null;
      Encoding = Encoding ?? System.Text.Encoding.Unicode;            // If the encoding is not specified use the Unicode
      var textBytes = Encoding.GetBytes(text);                        // Get the bytes according to the encoding
      using (var textStream = new MemoryStream())                            // Make a stream of to be feeded by zip stream
      {
        using (var zip = new GZipStream(textStream, CompressionMode.Compress)) // Create a zip stream to receive zipped content in textStream
        {
          zip.Write(textBytes, 0, textBytes.Length);                      // Write textBytes into zip stream, then zip will populate textStream
          zip.Close();
        }
        return textStream.ToArray();                                    // Get the bytes from the text stream
      }
    }


    public static string DecompressString(byte[] value, Encoding Encoding = null)
    {
      if (value == null) return null;
      Encoding = Encoding ?? System.Text.Encoding.Unicode;                // If the encoding is not specified use the Uncide
      using (var inputStream = new MemoryStream(value))                          // Create a stream based on input value
      using (var outputStream = new MemoryStream())                              // Create a stream to recieve output
      {
        using (var zip = new GZipStream(inputStream, CompressionMode.Decompress))  // Create a stream to decompress inputStream into outputStream
        {
          var bytes = new byte[4096];
          int n;
          while ((n = zip.Read(bytes, 0, bytes.Length)) != 0)                 // While zip results output bytes from input stream
          {
            outputStream.Write(bytes, 0, n);                                // Write the unzipped bytes into output stream
          }
          zip.Close();
        }
        return Encoding.GetString(outputStream.ToArray());                  // Get the string from unzipped bytes
      }
    }


    // http://www.codestash.co.uk/CodeSnippet/DisplaySnippetsForCategory?category=C%23%20Compression
    /// <summary>
    /// Compresses byte data
    /// </summary>
    /// <param name="bytes">Byte array to be compressed</param>
    /// <returns>A byte array of compressed data</returns>
    public static byte[] CompressToByte(byte[] bytes)
    {
      if (bytes == null)
      {
        return null;
      }
      using (var stream = new MemoryStream())
      {
        using (var zipStream = new GZipStream(stream, CompressionMode.Compress, true))
        {
          zipStream.Write(bytes, 0, bytes.Length);
          zipStream.Close();
        }
        return stream.ToArray();
      }
    }


    // http://www.codestash.co.uk/CodeSnippet/DisplaySnippetsForCategory?category=C%23%20Compression
    /// <summary>
    /// Decompresses byte data
    /// </summary>
    /// <param name="bytes">Byte array to be decompressed</param>
    /// <returns>A byte array of decompressed data</returns>
    public static byte[] DecompressToByte(byte[] bytes)
    {
      if (bytes == null || bytes.Length == 0)
      {
        return null;
      }

      using (var stream = new MemoryStream())
      {
        using (var zipStream = new GZipStream(new MemoryStream(bytes), CompressionMode.Decompress, true))
        {
          var buffer = new byte[4096];
          while (true)
          {
            var size = zipStream.Read(buffer, 0, buffer.Length);
            if (size > 0)
            {
              stream.Write(buffer, 0, size);
            }
            else
            {
              break;
            }
          }
          zipStream.Close();
        }

        return stream.ToArray();
      }
    }
  }

}
