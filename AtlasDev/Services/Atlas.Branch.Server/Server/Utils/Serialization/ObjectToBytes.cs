using System;
using System.Text;

using fastJSON;


namespace Atlas.Utils.Serialization
{
  static class ObjectToBytes
  {
    /// <summary>
    /// Uses fastJson to serialize an object to json and then optionally compress
    /// </summary>
    /// <param name="theObject"></param>
    /// <param name="compress"></param>
    /// <returns>byte[] of ASCII json, with optional compression</returns>
    public static byte[] SerializeToBytesJson(object theObject, bool compress = true)
    {
      JSON.Parameters.UseOptimizedDatasetSchema = true;
      var json = JSON.ToJSON(theObject);
      var bytes = ASCIIEncoding.ASCII.GetBytes(json);
      return compress ? Ionic.Zlib.DeflateStream.CompressBuffer(bytes) : bytes;
    }


    /// <summary>
    /// Uses fastJson to deserialize a byte[] back to object
    /// </summary>
    /// <param name="serializedBytes"></param>
    /// <param name="isCompressed"></param>
    /// <returns>Deserialized </returns>
    public static T DeserializeFromBytesJson<T>(byte[] serializedBytes, bool isCompressed = true)
    {
      if (isCompressed)
      {
        var uncompressed = Ionic.Zlib.DeflateStream.UncompressBuffer(serializedBytes);
        var json = Encoding.ASCII.GetString(uncompressed);
        return JSON.ToObject<T>(json);
      }
      else
      {
        var json = Encoding.ASCII.GetString(serializedBytes);
        return JSON.ToObject<T>(json);
      }
    }

  }
}
