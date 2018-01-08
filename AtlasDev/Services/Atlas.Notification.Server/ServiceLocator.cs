using System.Linq;
using Ninject;

namespace Atlas.Notification.Server
{
  public static class ServiceLocator
  {
    private static IKernel _serviceLocator;

    public static void SetServiceLocator(IKernel serviceLocator)
    {
      _serviceLocator = serviceLocator;
    }

    public static T Get<T>()
    {
      return _serviceLocator.GetAll<T>().FirstOrDefault();
    }
  }
}
