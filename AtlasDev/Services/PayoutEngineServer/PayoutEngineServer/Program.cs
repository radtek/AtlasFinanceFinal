using Atlas.Domain;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Ninject;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Topshelf;

namespace Atlas.Payout.Engine
{
	class Program
	{
		[STAThread]
		static void Main(string[] args)
		{
			var path = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			if (path != null)
			{
				var checkFile = Path.Combine(path, $"{Assembly.GetExecutingAssembly().ManifestModule.Name}{".Config"}");
				if (!File.Exists(checkFile))
				{
					throw new FileNotFoundException("Log configuration file was not found");
				}
			}
			log4net.Config.XmlConfigurator.Configure();

			var connStr = ConfigurationManager.ConnectionStrings["Atlas"].ConnectionString;
			var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);
			using (var dataLayer = new SimpleDataLayer(dataStore))
			{
				using (var session = new Session(dataLayer))
				{
					XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
				}
			}
			XpoDefault.Session = null;
			DomainMapper.Map();

			HostFactory.Run(c =>
			{
				c.SetServiceName("PayoutEngine");
				c.SetDisplayName("PayoutEngine Server");
				c.SetDescription("An engine that performs payouts to designated accounts.");

				c.RunAsLocalSystem();
				var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
				var module = new PayoutEngineRegistry();
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
