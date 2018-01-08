using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Falcon.Controllers.Api
{
  public sealed class DocumentController : AppApiController
  {
    [HttpPost] // This is from System.Web.Http, and not from System.Web.Mvc
    public async Task<HttpResponseMessage> Upload()
    {
      if (!Request.Content.IsMimeMultipartContent())
      {
        Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
      }

      try
      {
        var provider = GetMultipartProvider();
        await Request.Content.ReadAsMultipartAsync(provider);        
        return Request.CreateResponse(HttpStatusCode.OK, "Uploaded");
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.BadRequest, "Failed to upload");
      }
    }


    // You could extract these two private methods to a separate utility class since
    // they do not really belong to a controller class but that is up to you
    private static MultipartFormDataStreamProvider GetMultipartProvider()
    {      
      // you could put this to web.config
      var root = HttpContext.Current.Server.MapPath("~/App_Data/Tmp/FileUploads");
      Directory.CreateDirectory(root);
      return new MultipartFormDataStreamProvider(root);
    }


    // Extracts Request FormatData as a strongly typed model
    private static object GetFormData<T>(MultipartFormDataStreamProvider result)
    {
      if (result.FormData.HasKeys())
      {
        var unescapedFormData = Uri.UnescapeDataString(result.FormData
            .GetValues(0).FirstOrDefault() ?? String.Empty);
        if (!String.IsNullOrEmpty(unescapedFormData))
          return JsonConvert.DeserializeObject<T>(unescapedFormData);
      }

      return null;
    }

    private string GetDeserializedFileName(MultipartFileData fileData)
    {
      var fileName = GetFileName(fileData);
      return JsonConvert.DeserializeObject(fileName).ToString();
    }

    public static string GetFileName(MultipartFileData fileData)
    {
      return fileData.Headers.ContentDisposition.FileName;
    }
  }
}