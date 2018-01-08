using Atlas.Common.Interface;


namespace Atlas.Servers.Common.WCF
{
  internal static class WcfUtils
  {
    /// <summary>
    /// Enumerates details of WCF endpoint
    /// </summary>
    /// <param name="service"></param>
    public static void EnumerateEndpointsActive(System.ServiceModel.Description.ServiceDescription service, ILogging log)
    {
      log.Information("WCF endpoints active: {EndpointCount}", service.Endpoints.Count);
      foreach (var endpoint in service.Endpoints)
      {
        log.Information("WCF endpoint active- {WCFAddress}, {WCFContractName}, {WCFBindingName}",
            endpoint.Address, endpoint.Contract.Name, endpoint.Binding.Name);
      }
    }
  }

}
