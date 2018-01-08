using System;
using System.Xml.Serialization;
using System.IO;
using System.Xml;


namespace Atlas.Common.Utils
{
  /// <summary>
  /// Helper functions relating to XML
  /// </summary>
  public static class Xml
  {
    public static object DeSerialize<T>(string xml) where T : class
    {
      return DeSerialize(typeof(T), xml);
    }

    public static object DeSerialize(Type type, string xml)
    {
      var xmlDeserialize = new XmlSerializer(type);

      var xmlDoc = XmlReader.Create(new StringReader(xml));
     
      var obj = xmlDeserialize.Deserialize(xmlDoc);

      xmlDeserialize = null;

      return obj;
    }


    /// <summary>
    /// Returns a string based value on an input object
    /// </summary>
    /// <typeparam name="T">Object to serialize</typeparam>
    public static string Serialize<T>(T value) where T : class
    {
      try
      {
        // Create instance of Serialize based on input type
        var xmlSerial = new XmlSerializer(typeof(T));

        return SerializeMemoryStreamToString<T>(value, xmlSerial, false);
      }
      catch (Exception)
      {
        throw;
      }
    }


    /// <summary>
    /// Returns a string based value on an input object
    /// </summary>
    public static string Serialize(object value)
    {
      try
      {
        // Create instance of Serialize based on input type
        var xmlSerial = new XmlSerializer(value.GetType());

        return SerializeMemoryStreamToString(value, xmlSerial, false);
      }
      catch (Exception)
      {
        throw;
      }
    }


    /// <summary>
    /// Returns a string based value on an input object
    /// </summary>
    /// <typeparam name="T">Object to serialize</typeparam>
    /// <param name="value">Instance of object</param>
    /// <param name="stripNamespaces">Remove XML namespaces</param>
    public static string Serialize<T>(T value, bool stripNamespaces) where T : class
    {
      try
      {
        // Create instance of Serialize based on input type
        var xmlSerial = new XmlSerializer(typeof(T));

        return SerializeMemoryStreamToString<T>(value, xmlSerial, stripNamespaces);
      }
      catch
      {
        throw;
      }
    }


    /// <summary>
    /// Internal method used to do serialization work
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="xmlSerial"></param>
    /// <param name="stripNamespaces"></param>
    /// <returns></returns>
    internal static string SerializeMemoryStreamToString<T>(T value, XmlSerializer xmlSerial, bool stripNamespaces) where T : class
    {
      XmlSerializerNamespaces xns = null;

      if (stripNamespaces)
      {
        // Remove namespaces
        xns = new XmlSerializerNamespaces();
        xns.Add(string.Empty, string.Empty);
      }

      // open new stream
      using (var stream = new MemoryStream())
      {
        xmlSerial.Serialize(stream, value, xns);

        // Reset stream to start
        stream.Position = 0;

        // Read into stream
        var strmRead = new StreamReader(stream);

        // return result
        return strmRead.ReadToEnd();
      }
    }


    /// <summary>
    /// Internal method used to do serialization work
    /// </summary>
    /// <param name="value"></param>
    /// <param name="xmlSerial"></param>
    /// <param name="stripNamespaces"></param>
    /// <returns></returns>
    internal static string SerializeMemoryStreamToString(object value, XmlSerializer xmlSerial, bool stripNamespaces)
    {
      XmlSerializerNamespaces xns = null;

      if (stripNamespaces)
      {
        // Remove namespaces
        xns = new XmlSerializerNamespaces();
        xns.Add(string.Empty, string.Empty);
      }

      // open new stream
      using (var stream = new MemoryStream())
      {
        xmlSerial.Serialize(stream, value, xns);

        // Reset stream to start
        stream.Position = 0;

        // Read into stream
        var strmRead = new StreamReader(stream);

        // return result
        return strmRead.ReadToEnd();
      }
    }


    public static string Serialize(Type type, object product, bool stripNamespaces)
    {
      var xmlSerial = new XmlSerializer(type);
      XmlSerializerNamespaces xns = null;

      if (stripNamespaces)
      {
        // Remove namespaces
        xns = new XmlSerializerNamespaces();
        xns.Add(string.Empty, string.Empty);
      }

      // open new stream
      using (var stream = new MemoryStream())
      {
        xmlSerial.Serialize(stream, product, xns);

        // Reset stream to start
        stream.Position = 0;

        // Read into stream
        var strmRead = new StreamReader(stream);

        // return result
        return strmRead.ReadToEnd();
      }
    }


    /// <summary>
    /// Serialize object to file
    /// </summary>
    /// <typeparam name="T">Type of C</typeparam>
    /// <param name="value">Type of C</param>
    /// <param name="C">Object to serialize</param>
    /// <param name="fileName">File to save</param>
    public static void Serialize<T>(object C, Type[] types, string fileName) where T : class
    {
      // Create instance of serializer for type T
      var serializer = new XmlSerializer(typeof(T), types);

      // Remove namespaces
      var xns = new XmlSerializerNamespaces();
      xns.Add(string.Empty, string.Empty);

      // Write file out
      using (var writer = new StreamWriter(fileName))
      {
        // Serial
        serializer.Serialize(writer, C, xns);
        writer.Close();
      }
    }


    /// <summary>
    /// Performs a serialization zips to temp file then base64 encodes the file
    /// </summary>
    public static string SerializeToZipToBase64<T>(T value)
    {
      // Create instance of Serialize based on input type
      var serializer = new XmlSerializer(typeof(T));

      var xns = new XmlSerializerNamespaces();

      // Remove namespaces
      xns.Add(string.Empty, string.Empty);

      var str = string.Empty;
      // open new stream
      using (var stream = new MemoryStream())
      {
        serializer.Serialize(stream, value, xns);

        // Reset stream to start
        stream.Position = 0;

        // Read into stream
        var strmRead = new StreamReader(stream);

        str = strmRead.ReadToEnd();
        var xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(str);

        foreach (XmlNode node in xmlDocument)
        {
          if (node.NodeType == XmlNodeType.XmlDeclaration)
          {
            xmlDocument.RemoveChild(node);
            str = xmlDocument.InnerXml.ToString();
            break;
          }
        }
      }

      // Generate temporary file.
      var filename = Guid.NewGuid().ToString().Replace("-", "").ToLower();

      // Write contents to temp file
      using(TextWriter tw = new StreamWriter(filename, false))
      {
        tw.Write(str);
      }

      if (Compression.Compress(filename, string.Format("{0}.zip", filename)))
        File.Delete(filename);

      var base64Encoded = Base64.Encode(string.Format("{0}.zip", filename));
      File.Delete(string.Format("{0}.zip",filename));

      return base64Encoded;
    }
  }
}

