namespace Nancy.CORS
{
  using Nancy;
  using Nancy.Bootstrapper;
  using Nancy.TinyIoc;

  public class Bootstrapper : DefaultNancyBootstrapper
  {
    // The bootstrapper enables you to reconfigure the composition of the framework,
    // by overriding the various methods and properties.
    // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper
    protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context)
    {
      pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
      {
        ctx.Response.WithHeader("Access-Control-Allow-Origin", "*")
                        .WithHeader("Access-Control-Allow-Methods", "POST,GET")
                        .WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type");

      });
    }
  }
}