using Atlas.RabbitMQ.Messages.Credit;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Interfaces.Services
{
  public interface IBureauService
  {
    ICreditResponse GetScore(CreditRequestLegacy request);
    ICreditResponse RequestScore(CreditRequestLegacy request);
  }
}
