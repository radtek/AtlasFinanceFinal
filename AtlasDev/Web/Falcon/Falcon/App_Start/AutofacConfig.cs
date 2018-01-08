using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.Nh;
using BrockAllen.MembershipReboot.Nh.Repository;
using BrockAllen.MembershipReboot.WebHost;
using NHibernate;

namespace Falcon.App_Start
{
  public class AutofacConfig
  {
    public static void ConfigureContainer()
    {
      var builder = new ContainerBuilder();
      builder.RegisterControllers(Assembly.GetAssembly(typeof(AutofacConfig)));
      builder.RegisterApiControllers(Assembly.GetAssembly(typeof(AutofacConfig)));
      
      builder.Register(context => NhibernateConfig.GetSessionFactory()).As<ISessionFactory>().SingleInstance();

      // Use a session per web request.
      builder.Register(context => context.Resolve<ISessionFactory>().OpenSession())
          .As<ISession>()
          .InstancePerHttpRequest();

      builder.RegisterGeneric(typeof(UserAccountService<>)).AsSelf();
      builder.RegisterType(typeof(SamAuthenticationService<NhUserAccount>))
          .AsSelf()
          .As(typeof(AuthenticationService<NhUserAccount>));
      builder.RegisterGeneric(typeof(MembershipRebootConfiguration<>)).AsSelf();
      builder.RegisterGeneric(typeof(NhRepository<>)).As(typeof(IRepository<>));
      builder.RegisterGeneric(typeof(NhUserAccountRepository<>)).As(typeof(IUserAccountRepository<>));
      builder.RegisterGeneric(typeof(NhGroupRepository<>))
          .As(typeof(IGroupRepository<>), typeof(QueryableGroupRepository<>));
      builder.Register(
          context => new GroupService<NhGroup>("default", context.Resolve<IGroupRepository<NhGroup>>())).AsSelf();
      var container = builder.Build();
      DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
      var resolver = new AutofacWebApiDependencyResolver(container);
      GlobalConfiguration.Configuration.DependencyResolver = resolver;
    }
  }
}