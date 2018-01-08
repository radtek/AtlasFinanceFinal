namespace Atlas.Server.Implementation.MessageBus
{
  public interface IMessageBusHandler
  { 
    void SendSMS(string cellularNumber, string message);
  }
}