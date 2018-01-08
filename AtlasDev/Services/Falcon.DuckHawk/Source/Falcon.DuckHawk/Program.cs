using Autofac;
using Topshelf;
using Topshelf.Autofac;

namespace Falcon.DuckHawk
{
  class Program
  {
    static void Main(string[] args)
    {
      var builder = new ContainerBuilder();

      builder.RegisterType<Engine>();

      var container = builder.Build();

      HostFactory.Run(c =>
      {
        c.UseAssemblyInfoForServiceInfo();

        c.RunAsLocalSystem();
        c.UseAutofacContainer(container);

        c.Service<Engine>(s =>
        {
          s.ConstructUsing(() => container.Resolve<Engine>());
          s.WhenStarted(o => o.Start());
          s.WhenStopped(o => o.Stop());
        });
      });
    }
  }
}