using System;
using Atlas.RabbitMQ.Messages.Credit;
using AutoMapper;
using Falcon.TBR.Bureau.Interfaces;
using MassTransit;

namespace Falcon.TBR.Bureau.Service
{
  public class BureauService : IBureauService
  {
    private readonly IServiceBus _bus;
    public BureauService(IServiceBus bus)
    {
      _bus = bus;
      Mapper.CreateMap<CreditStreamResponse, ICreditResponse>();
      Mapper.CreateMap<Product, IProduct>();
      Mapper.CreateMap<Reason, IReason>();

    }

    /// <summary>
    /// Gets the latest score, or performs one if its completely empty for this client
    /// </summary>
    public ICreditResponse GetScore(CreditRequestLegacy request)
    {
      request.IsQueryOnly = false;
      ICreditResponse response = null;

      _bus.PublishRequest(request, x =>
      {
        x.Handle<CreditStreamResponse>(a =>
        {
          response = Mapper.Map<CreditStreamResponse, ICreditResponse>(a);
        }); x.SetTimeout(TimeSpan.FromSeconds(89));
      });
      return response;
    }
    
    /// <summary>
    /// Only lookups the existing one.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public ICreditResponse RequestScore(CreditRequestLegacy request)
    {
      request.IsQueryOnly = true;
      ICreditResponse response = null;
      _bus.PublishRequest(request, x =>
      {
        x.Handle<CreditStreamResponse>(a =>
        {
          response = Mapper.Map<CreditStreamResponse, ICreditResponse>(a);
        }); x.SetTimeout(TimeSpan.FromSeconds(89));
      });
      return response;
    }
  }
}
