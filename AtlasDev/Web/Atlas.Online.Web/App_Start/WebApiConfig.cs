using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace Atlas.Online.Web
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      config.Routes.MapHttpRoute(
        name: "MyAccountApi",
        routeTemplate: "api/MyAccount/{action}/{clientId}",
        defaults: new { controller = "MyAccount", clientId = RouteParameter.Optional }
      );   

      config.Routes.MapHttpRoute(
        name: "VerificationQuestionsApi",
        routeTemplate: "api/VerificationQuestions/{id}",        
        defaults: new { controller = "VerificationQuestions" }
      );

      config.Routes.MapHttpRoute(
        name: "ApplicationApi",
        routeTemplate: "api/Application/{action}/{id}",
        defaults: new { controller = "Application", action = "Get", stepId = RouteParameter.Optional }
      );

      config.Routes.MapHttpRoute(
        name: "RedirectApplicationStepApi",
        routeTemplate: "api/ApplicationStep/ProcessingRedirect/{id}",
        defaults: new { controller = "ApplicationStep", action = "ProcessingRedirect" }
      );
  
      config.Routes.MapHttpRoute(
        name: "ApplicationStepApi",
        routeTemplate: "api/ApplicationStep/{action}/{stepId}",
        defaults: new { controller = "ApplicationStep", action = "Get", stepId = RouteParameter.Optional }
      );

      config.Routes.MapHttpRoute(
        name: "ActionApi",
        routeTemplate: "api/{controller}/{action}/{id}",
        defaults: new { id = RouteParameter.Optional }
      );
      
      config.Routes.MapHttpRoute(
        name: "DefaultApi",
        routeTemplate: "api/{controller}/{id}",
        defaults: new { id = RouteParameter.Optional }
      );

      // Json Formatting
      var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
      config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);
      config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
      ((DefaultContractResolver)config.Formatters.JsonFormatter.SerializerSettings.ContractResolver).IgnoreSerializableAttribute = true;

      JsonMediaTypeFormatter jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
      JsonSerializerSettings jSettings = new Newtonsoft.Json.JsonSerializerSettings()
      {
        Formatting = Formatting.Indented,
        DateTimeZoneHandling = DateTimeZoneHandling.Local
      };

      // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
      // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
      // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
      // config.EnableQuerySupport();

      // To disable tracing in your application, please comment out or remove the following line of code
      // For more information, refer to: http://www.asp.net/web-api
      //config.EnableSystemDiagnosticsTracing();
    }
  }
}
