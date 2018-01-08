/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty) Ltd.
 * 
 *  Description:
 *  ------------------
 *    Useful serialization utilities
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

using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace Atlas.Common.Utils
{
  public static class Serialization
  {
    /// <summary>
    /// Serializes an object to byte[], with optional compression
    /// </summary>
    /// <param name="theObject"></param>
    /// <param name="compress"></param>
    /// <returns>Serialized byte array of object</returns>    
    public static byte[] SerializeToBytes(object theObject, bool compress = true)
    {
      if (theObject is DataSet)
      {
        ((DataSet)theObject).RemotingFormat = SerializationFormat.Binary;
      }
      else if (theObject is DataTable)
      {
        ((DataTable)theObject).RemotingFormat = SerializationFormat.Binary;
      }

      var formatter = new BinaryFormatter();
      using (var serializedMem = new MemoryStream())
      {
        formatter.Serialize(serializedMem, theObject);

        return compress ? Ionic.Zlib.DeflateStream.CompressBuffer(serializedMem.ToArray()) : serializedMem.ToArray();
      }
    }


    /// <summary>
    /// Deserializes a object from a byte[]
    /// </summary>
    /// <param name="serializedBytes"></param>
    /// <param name="isCompressed"></param>
    /// <returns>The deserialized object</returns>
    public static object DeserializeFromBytes(byte[] serializedBytes, bool isCompressed)
    {
      var formatter = new BinaryFormatter();
      if (isCompressed)
      {
        using (var uncompressed = new MemoryStream(Ionic.Zlib.DeflateStream.UncompressBuffer(serializedBytes)))
        {
          return formatter.Deserialize(uncompressed);
        }
      }
      else
      {
        using (var compressed = new MemoryStream(serializedBytes))
        {
          return formatter.Deserialize(compressed);

        }
      }
    }
  }
}
