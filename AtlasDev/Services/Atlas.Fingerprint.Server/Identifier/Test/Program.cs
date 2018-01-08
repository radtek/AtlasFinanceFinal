using System;
using System.Configuration;
using System.Collections.Generic;

using EasyNetQ;
using Atlas.FP.Identifier.MessageTypes.RequestResponse;
using Atlas.FP.Identifier.MessageTypes.PubSub;


namespace TestFP
{
  class Program
  {
    private static IBus _bus;


    static void Main()
    {
      #region Distributed identification: Request/receive via RabbitMQ
      var address = ConfigurationManager.AppSettings["fp-rabbitmq-address"];
      var virtualHost = ConfigurationManager.AppSettings["fp-rabbitmq-virtualhost"];
      var userName = ConfigurationManager.AppSettings["fp-rabbitmq-username"];
      var password = ConfigurationManager.AppSettings["fp-rabbitmq-password"];

      var connectionString = string.Format("host={0};virtualHost={1};username={2};password={3};persistentMessages=false", address, virtualHost, userName, password);

      _bus = RabbitHutch.CreateBus(connectionString);
      #endregion

      try
      {
        var customerId = string.Empty;
        while (customerId != "quit")
        {
          Console.Write("quit exits: ");
          customerId = Console.ReadLine();

          for (var i = 0; i < 50; i++)
          {
            var x = _bus.Request<IdentifyFromImageRequest, IdentifyFromImageResponse>(new IdentifyFromImageRequest(DateTime.Now, new List<byte[]> { new byte[1000] }, 6));
          }
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine("Exception!!! OMG!!! {0}", ex);
      }
      finally
      {
        _bus.Dispose();
      }

      Console.ReadKey();
    }

  }
}
