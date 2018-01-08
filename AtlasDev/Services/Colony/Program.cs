using System.IO;
using Atlas.RabbitMQ.Messages.Coupon;
using Atlas.RabbitMQ.Messages.Notification;
using Ninject;
using System;
using Serilog;
using Topshelf;

namespace Atlas.Colony.Integration.Service
{
  class Program
  {
    [STAThread]
    static void Main()
    {
      Log.Logger = new LoggerConfiguration()
        .WriteTo.ColoredConsole()
        .WriteTo.RollingFile(string.Format("{0}/{1}", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs"), "CouponService.txt"))
        .CreateLogger();

      #region AutoMapper

      AutoMapper.Mapper.CreateMap<CouponIssueRequestMessage, CouponIssueStartRequestMessage>();
      AutoMapper.Mapper.CreateMap<CouponIssueStartRequestMessage, CouponIssueCompletedRequestMessage>();
      AutoMapper.Mapper.CreateMap<CouponIssueStartRequestMessage, CouponIssueIgnoreRequestMessage>();

      AutoMapper.Mapper.CreateMap<EurocomSMSRequestMessage, EurocomSMSStartRequestMessage>();
      AutoMapper.Mapper.CreateMap<EurocomSMSStartRequestMessage, EurocomSMSCompletedRequestMessage>();

      #endregion

      HostFactory.Run(c =>
      {

        c.UseAssemblyInfoForServiceInfo();
        c.UseSerilog();
        c.RunAsLocalSystem();
        var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
        var module = new CouponRegistry();
        kernel.Load(module);

        c.Service<Engine>(s =>
        {
          s.ConstructUsing(builder => kernel.Get<Engine>());
          s.WhenStarted(o => o.Start());
          s.WhenStopped(o => o.Stop());
        });
      });
    }
  }
}