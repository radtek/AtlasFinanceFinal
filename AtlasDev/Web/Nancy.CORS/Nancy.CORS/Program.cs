namespace Nancy.CORS
{
  using System;
  using Nancy.Hosting.Self;
  using System.Configuration;
  using MassTransit;

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
      var uri =
          new Uri("http://127.0.0.1:3579");

      using (var host = new NancyHost(uri))
      {
        host.Start();

        Console.WriteLine("Your application is running on " + uri);
        Console.WriteLine("Press any [Enter] to close the host.");

       Global.ServiceBus =  ServiceBusFactory.New(config =>
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
        Console.ReadLine();
      }
    }
  }
}
