using System;
using System.Xml;
using System.Xml.Linq;


namespace Atlas.Common.Extensions
{
  public static class XML
  {
    public static XElement GetXElement(this XmlNode node)
    {
      var xDoc = new XDocument();
      using (var xmlWriter = xDoc.CreateWriter())
        node.WriteTo(xmlWriter);
      return xDoc.Root;
    }


    public static XmlNode GetXmlNode(this XElement element)
    {
      var xmlDoc = new XmlDocument();
      using (var xmlReader = element.CreateReader())
        xmlDoc.Load(xmlReader);
      return xmlDoc;
    }

  }
}
