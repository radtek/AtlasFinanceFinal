using System;

namespace Atlas.Enumerators
{
  public class NodeType
  {
    public enum Nodes
    {
      ApplicationStart = 0,
      AccountVerification = 1,
      AccountCreation = 2,
      Affordability = 3,
      ApplicationDecision = 4,
      Client = 5,
      CreditVerification = 6,
      Email = 7,
      FraudPrevention = 8,
      SMS = 9,
      Sink = 41
    }
  }
}
