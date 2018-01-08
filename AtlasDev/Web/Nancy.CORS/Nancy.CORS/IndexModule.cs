namespace Nancy.CORS
{
  using Nancy;
  using System;
  using Nancy.ModelBinding;

  public class IndexModule : NancyModule
  {
    
    public class Request1
    {
      public string Status { get; set; }
    }

    public class FP
    {
      public string Password { get; set; }
      public string Hash { get; set; }

      public Guid TrackingId { get; set; }
    }

    public IndexModule()
    {
      Post["/"] = _ =>
      {
        FP fp = this.Bind<FP>();

        Guid correlationGuid = Magnum.CombGuid.Generate();
        Global.ServiceBus.Publish<Nancy.Cors.Rabbit.Messages.FingerPrintRequest>(new Nancy.Cors.Rabbit.Messages.FingerPrintRequest(correlationGuid)
        {
          TrackingId = fp.TrackingId.ToString()
        });


        return Response.AsJson<Request1>(new Request1() { Status = "Processing..." });
      };
    }
  }
}