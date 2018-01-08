/* -----------------------------------------------------------------------------------------------------------------
 *  Copyright (C) 2012 Atlas Finance (Pty() Ltd.
 * 
 *  Description:
 *  ------------------
 *  Holds calls for queries against core DB
 * 
 *  Author:
 *  ------------------
 *     Fabian Franco-Roldan
 * 
 * 
 *  Revision history: 
 *  ------------------ 
 *     2012/11/01 - Initial Version
 * 
 * 
 * ----------------------------------------------------------------------------------------------------------------- */

using Atlas.Domain;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using Ninject;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using Atlas.Domain.Model;
using Topshelf;

namespace Atlas.Orchestration.Server
{
  class Program
  {
    static void Main(string[] args)
    {
      var path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

      var checkFile = Path.Combine(path, string.Format("{0}{1}", Assembly.GetExecutingAssembly().ManifestModule.Name, ".Config"));
      if (!File.Exists(checkFile))
      {
        throw new FileNotFoundException("Log configuration file was not found");
      }
      log4net.Config.XmlConfigurator.Configure();

      #region Start XPO- Create a thread-safe data layer
      try
      {
        // Create thread-safe- load and build the domain!
        var connStr = ConfigurationManager.ConnectionStrings["Atlas"].ConnectionString;
        var dataStore = XpoDefault.GetConnectionProvider(connStr, AutoCreateOption.None);
        using (var dataLayer = new SimpleDataLayer(dataStore))
        {
          using (var session = new Session(dataLayer))
          {
            session.Dictionary.GetDataStoreSchema(typeof(BRN_Branch).Assembly);  
            XpoDefault.DataLayer = new ThreadSafeDataLayer(session.Dictionary, dataStore);
          }
        }
        XpoDefault.Session = null;
      }
      catch (Exception err)
      {
        throw new Exception("Error with XPO domain", err);
      }

      DomainMapper.Map();
      #endregion


      HostFactory.Run(c =>
      {
        c.SetServiceName("AtlasOrchestrationService");
        c.SetDisplayName("Atlas Orchestration Service");
        c.SetDescription("Orchestrate calls between backend services");

        c.RunAsLocalSystem();
        var kernel = new StandardKernel(new NinjectSettings() { LoadExtensions = true });
        var module = new OrchestrationRegistry();
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