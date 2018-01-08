/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *    Compression helper functions   
 * 
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012-09-03 - Initial revision
 *     
 * ----------------------------------------------------------------------------------------------------------------- */
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

using Ionic.Zip;


namespace Atlas.Common.Utils
{
  /// <summary>
  /// Houses core compression functionality
  /// </summary>
  public static class Compression
  {
    /// <summary>
    /// Compress a single file
    /// </summary>
    /// <param name="fileIn">File to compress</param>
    /// <param name="fileOut">Archout output file</param>
    public static bool Compress(string fileIn, string fileOut)
    {
      using (var zip = new ZipFile())
      {
        zip.AddFile(fileIn);
        zip.Save(fileOut);
      }

      return true;
    }


    /// <summary>
    /// Compress a single file, creates new zipped file with same name and appends .zip
    /// </summary>
    /// <param name="fileIn">File to compress</param>
    /// <param name="level">Specify compression level</param>
    public static string Compress(string fileIn, Ionic.Zlib.CompressionLevel level)
    {
      using (var zip = new ZipFile())
      {
        zip.CompressionLevel = level;
        zip.CompressionMethod = CompressionMethod.BZip2; // Better than deflate        
        zip.AddFile(fileIn);
        zip.Save(string.Format("{0}.zip", fileIn));
      }

      return string.Format("{0}.zip", fileIn);
    }


    /// <summary>
    /// Compresses a string returning a byte array
    /// </summary>
    /// <param name="str">String to compress</param>
    /// <returns></returns>
    public static byte[] Compress(string str)
    {
      return Ionic.Zlib.ZlibStream.CompressString(str);
    }


    /// <summary>
    /// Decompresses a compressed byte array
    /// </summary>
    /// <param name="bytes">Byte array to decompress</param>
    /// <returns></returns>
    public static string Decompress(byte[] bytes)
    {
      return Ionic.Zlib.ZlibStream.UncompressString(bytes);
    }


    /// <summary>
    /// Compress byte array
    /// </summary>
    /// <param name="plainData">Data to be compressed</param>
    /// <returns>Compressed byte array</returns>
    public static byte[] InMemoryCompress(byte[] plainData)
    {
      byte[] compressesData = null;
      using (var outputStream = new MemoryStream())
      {
        using (var zip = new GZipStream(outputStream, CompressionMode.Compress))
        {
          zip.Write(plainData, 0, plainData.Length);
        }
        compressesData = outputStream.ToArray();
      }

      return compressesData;
    }


    /// <summary>
    /// Decompress byte array
    /// </summary>
    /// <param name="zippedData">Compressed byte array</param>
    /// <returns>Uncompressed byte array</returns>
    public static byte[] InMemoryDecompress(byte[] zippedData)
    {
      byte[] decompressedData = null;
      using (var outputStream = new MemoryStream())
      {
        using (var inputStream = new MemoryStream(zippedData))
        {
          using (var zip = new GZipStream(inputStream, CompressionMode.Decompress))
          {
            zip.CopyTo(outputStream);
          }
        }
        decompressedData = outputStream.ToArray();
      }
      return decompressedData;
    }


    /// <summary>
    /// Generate MD5 hash of input
    /// </summary>
    /// <param name="md5Hash"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string MD5Hash(MD5 md5Hash, string input)
    {
      var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
      var sBuilder = new StringBuilder();
      for (var i = 0; i < data.Length; i++)
      {
        sBuilder.Append(data[i].ToString("x2"));
      }
      return sBuilder.ToString();
    }


    /// <summary>
    ///  Verify a hash against a string. 
    /// </summary>
    public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
    {
      var hashOfInput = MD5Hash(md5Hash, input);
      var comparer = StringComparer.OrdinalIgnoreCase;
      return (0 == comparer.Compare(hashOfInput, hash));
    }


    /// Performs GZip compression on the source stream- caller to dispose stream (use this within a using... statement)
    /// </summary>
    /// <param name="source">Source Stream to be compressed</param>
    /// <returns>Stream with GZip compressed data</returns>
    public static MemoryStream InMemoryCompress(Stream source)
    {
      if (source == null || source.Length == 0)
      {
        return null;
      }

      source.Position = 0;
      var result = new MemoryStream();
      using (var dest = new GZipStream(result, CompressionMode.Compress, true))
      {
        source.CopyTo(dest);
      }

      return result;
    }


    /// <summary>
    /// Performs GZip decompression on the source stream- caller to dispose stream (use this within a using... statement)
    /// </summary>
    /// <param name="source">Source Stream to be decompressed</param>
    /// <returns>Stream with GZip decompressed data</returns>
    public static MemoryStream InMemoryDecompress(Stream source)
    {
      if (source == null || source.Length == 0)
      {
        return null;
      }

      source.Position = 0;
      var result = new MemoryStream();
      using (var dest = new GZipStream(source, CompressionMode.Decompress, true))
      {
        dest.CopyTo(result);
      }

      return result;
    }
  }
}