using Autofac;

namespace Falcon.Service.Interface
{
  public interface IService
  {
    void Start(IContainer container);
    void Stop();
  }
}
