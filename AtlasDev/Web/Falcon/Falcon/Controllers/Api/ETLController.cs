
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web.Http;
using Atlas.Common.Utils;
using Atlas.Enumerators;
using Falcon.Gyrkin.Library.Security.Attributes;
using Falcon.Gyrkin.Library.Service;


namespace Falcon.Controllers.Api
{
  // [AllowAnonymous]
  // [ValidateHttpAntiForgeryToken]
  [Authorize]
  public sealed class ETLController : AppApiController
  {
    // GET api/<controller>
    [HttpGet]
    public HttpResponseMessage Get(long? id)
    {
      if (!id.HasValue)
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Id parameter required.");

      List<ETLDebitOrderBatch> eltDebitOrderBatch;

      try
      {

        eltDebitOrderBatch =  Services.WebServiceClient.Naedo_GetETLBatch(General.Host.SDC, id, null, true);
      }
      catch (Exception)
      {
        return Request.CreateResponse(HttpStatusCode.InternalServerError, "Unable to communicate with services.");
      }

      return Request.CreateResponse(HttpStatusCode.OK, new { eltDebitOrderBatch });
    }


    [HttpGet]
    public HttpResponseMessage GetItem(long? id, bool b)
    {
      if (!id.HasValue)
        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Id parameter required.");

      var eltDebitOrderBatch = Services.WebServiceClient.Naedo_GetETLBatch(General.Host.SDC, null, id, b);

      return Request.CreateResponse(HttpStatusCode.OK, new { eltDebitOrderBatch });
    }


    [HttpPost]
    [WebApiAntiForgeryToken]
    [Authorize]
    public HttpResponseMessage Deliver(string p)
    {
      long? stagingBatchId;
      var file = Base64.Base64Decode(p);
      if (File.Exists(file))
      {
        var fileContent = File.ReadAllBytes(file);
        var compressContent = Compression.InMemoryCompress(fileContent);
        var baseStringfy = Base64.EncodeString(compressContent);

        using (var md5Hash = MD5.Create())
        {
          var hash = Compression.MD5Hash(md5Hash, baseStringfy);

          stagingBatchId = Services.WebServiceClient.Naedo_ImportFile(General.Host.Falcon, 1, baseStringfy, hash);

          if (stagingBatchId == 0)
            return Request.CreateResponse(HttpStatusCode.InternalServerError, "There was an error processing your batch file.");
          else if (stagingBatchId == -1)
            return Request.CreateResponse(HttpStatusCode.BadRequest, "File has to many transactions.");
        }
      }
      else
      {
        return Request.CreateResponse(HttpStatusCode.NotFound, "File was not found in storage");
      }
      return Request.CreateResponse(HttpStatusCode.OK, new { id = stagingBatchId });
    }
  }
}