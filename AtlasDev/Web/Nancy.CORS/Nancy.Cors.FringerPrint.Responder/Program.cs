using Atlas.RabbitMQ.Messages.Push;
using MassTransit;
using Nancy.Cors.Rabbit.Messages;
using Nancy.CORS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.Cors.FringerPrint.Responder
{
  class Program
  {
    private static string RABBITMQ_ADDRESS = string.Empty;
    private static string RABBITMQ_BINDING = string.Empty;
    private static string RABBITMQ_USERNAME = string.Empty;
    private static string RABBITMQ_PASSWORD = string.Empty;


    static void Main(string[] args)
    {

      RABBITMQ_ADDRESS = ConfigurationManager.AppSettings["rabbitmq-address"];
      RABBITMQ_BINDING = ConfigurationManager.AppSettings["rabbitmq-binding"];
      RABBITMQ_USERNAME = ConfigurationManager.AppSettings["rabbitmq-username"];
      RABBITMQ_PASSWORD = ConfigurationManager.AppSettings["rabbitmq-password"];

      Global.ServiceBus = ServiceBusFactory.New(config =>
      {
        config.UseRabbitMq(r => r.ConfigureHost(new Uri(string.Format("rabbitmq://{0}/{1}", RABBITMQ_ADDRESS, RABBITMQ_BINDING)), h =>
        {
          h.SetUsername(RABBITMQ_USERNAME);
          h.SetPassword(RABBITMQ_PASSWORD);
          h.SetRequestedHeartbeat(60);
          
        }));
        config.ReceiveFrom(string.Format("rabbitmq://{0}/{1}", RABBITMQ_ADDRESS, RABBITMQ_BINDING));
        config.UseControlBus();
        config.EnableMessageTracing();
        config.EnableRemoteIntrospection();
       
      });

      Global.ServiceBus.SubscribeHandler<FingerPrintRequest>(Handle);

      Console.ReadLine();
    }
    public static string Generate(string var)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(var);
      byte[] buffer = new SHA256Managed().ComputeHash(bytes);
      string str = string.Empty;

      foreach (byte num in buffer)
        str = str + string.Format("{0:x2}", num);

      return str;
    }
    static void Handle(FingerPrintRequest x)
    {
      PushMessage msg = new PushMessage(Magnum.CombGuid.Generate());
      msg.Type = PushMessage.PushType.FingerPrint;
      msg.Parameters.Add("Authenticated", true);
      msg.Parameters.Add("TrackingId", x.TrackingId);
      msg.Parameters.Add("HasError", false);
      msg.Parameters.Add("Error", string.Empty);
      msg.Parameters.Add("Checksum", Generate(string.Format("{0}{1}{2}{3}{4}", true, x.TrackingId, false, string.Empty, "12312312312")).ToString());

      Global.ServiceBus.Publish<PushMessage>(msg);
      Console.WriteLine(x.CorrelationId);        
    }
  }
}
