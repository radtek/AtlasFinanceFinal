using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;

using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Atlas.Common.Utils;
using Atlas.DocServer.Utils;
using Atlas.DocServer.WCF.Interface;
using Atlas.Common.Interface;


namespace Atlas.DocServer.WCF.Implementation.Generator
{
  class RenderDocumentWithJSon_Impl
  {
    internal static byte[] Execute(ILogging log, long templateId, Int64 storageId, string json,
      FileFormatEnums.FormatType fileFormat,
      DocOptions docOptions, RenderOptions renderOptions, bool destCompress)
    {
      var methodName = "RenderDocumentWithJSon";
      log.Information("{MethodName} started, {TemplateId}, {DataLength}, {FileFormat}, {@DocOptions}, {@RenderOptions} ", methodName,
        templateId, json != null ? json.Length : 0, fileFormat, docOptions, renderOptions);
      var sw = Stopwatch.StartNew();

      if (templateId <= 0 || string.IsNullOrEmpty(json) || fileFormat == FileFormatEnums.FormatType.NotSet || docOptions == null)
      {
        log.Error("{MethodName}- Missing key parameters: {TemplateId}, {FileFormat}, {@DocOptions}, {@RenderOptions}",
            methodName, templateId, fileFormat, docOptions, renderOptions);          
        return null;
      }

      Stream result = null;

      try
      {
        #region Convert json to DataTable/DataSet
        var jsonParse = JObject.Parse(json);
        DataTable data = null;
        var dataSet = new DataSet();
        if (jsonParse.Type == JTokenType.Array) // Multiple json rows: {items:["Name":"AAA","Age":22,"Job":"PPP"},{"Name":"BBB"","Age":25,"Job":"QQQ"}]}
        {
          data = JsonConvert.DeserializeObject<ReportData>(json).Items;
        }
        else // Single json row: {"Name":"AAA","Age":22,"Job":"PPP"}
        {
          data = new DataTable();
          var values = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
          foreach(var key in values.Keys)
          {
            data.Columns.Add(key, values[key].GetType());            
          }
          var row = data.NewRow();
          foreach (var key in values.Keys)
          {
            row[key] = values[key];
          }
          data.Rows.Add(row);
        }
        dataSet.Tables.Add(data);
        #endregion

        var fileFormatEnum = Mapper.Map<Enumerators.Document.FileFormat>(fileFormat);
        result = Utils.TemplateUtils.RenderTemplate(templateId, storageId, dataSet, fileFormatEnum, docOptions, renderOptions);
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
      }

      log.Information("{MethodName} completed in {ElapsedMS}ms with {ResultLength} bytes", methodName,
        sw.ElapsedMilliseconds, result != null ? result.Length : 0);

      return (destCompress && result != null && result.Length > 0) ? Compression.InMemoryCompress(result).ToArray() : result.StreamToByte();
    }


    class ReportData
    {
      [JsonProperty(PropertyName = "items")]
      public DataTable Items { get; set; }
    }
  }
}
