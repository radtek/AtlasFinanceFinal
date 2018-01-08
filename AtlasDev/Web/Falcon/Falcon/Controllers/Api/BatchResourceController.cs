using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Falcon.Controllers.Api
{
  // [AllowAnonymous]
  // [ValidateHttpAntiForgeryToken]
  public sealed class BatchResourceController : AppApiController
  {
    public sealed class Job
    {
      public long BatchId { get;set;}
      public long BranchId { get;set;}
      public string Status { get; set; }
      public DateTime? DeliveryDate { get;set;}
      public DateTime CreatedDate { get; set; }
    }


    // GET api/<controller>
    [HttpGet]
    public HttpResponseMessage GetJobs()
    {
     // var jobCollection = new XPQuery<BUR_Batch>(Services.UnitOfwork).ToList();

      List<Job> enquiryCollection = new List<Job>();

      //foreach (var item in jobCollection)
      //{
      //  enquiryCollection.Add(new Job()
      //  {
      //    BatchId = item.BatchId,
      //    BranchId = item.Branch.BranchId,
      //    Status = item.DeliveryDate == null ? "Pending" : "Delivered",
      //    CreatedDate = (DateTime)item.CreatedDate,
      //    DeliveryDate = item.DeliveryDate
      //  });
      //}

      return Request.CreateResponse(HttpStatusCode.OK, new { enquiryCollection });
    }


    public bool Post([FromBody] object value)
    {
      return true;
    }
  }
}