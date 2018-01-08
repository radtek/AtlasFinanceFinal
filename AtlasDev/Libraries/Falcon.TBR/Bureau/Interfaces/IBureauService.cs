using Atlas.RabbitMQ.Messages.Credit;

namespace Falcon.TBR.Bureau.Interfaces
{
  public interface IBureauService
  {
    ICreditResponse GetScore(CreditRequestLegacy request);
    ICreditResponse RequestScore(CreditRequestLegacy request);
  }
}
