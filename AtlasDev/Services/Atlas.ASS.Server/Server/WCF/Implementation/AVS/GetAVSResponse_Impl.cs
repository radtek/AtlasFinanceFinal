using System;

using Atlas.Common.Interface;
using Atlas.WCF.Interface;
using Atlas.Server.MessageBus.Avs;
using Atlas.Cache.Interfaces;


namespace Atlas.Server.WCF.Implementation.AVS
{
  internal static class GetAVSResponse_Impl
  {
    internal static AVSResponse Execute(ILogging log, long transactionId)
    {
      var methodName = "GetAVSResponse";
      try
      {
        var avsResponse = AvsDistCommUtils.CheckAVS(transactionId);
        if (avsResponse == null)
        {
          log.Error("{MethodName}- GetAVSResponse() returned null result", methodName);
          return null;
        }

        return ConvertUtils.ToAVSResponse(avsResponse);
      }
      catch (Exception err)
      {
        log.Error(err, methodName);
        throw new Exception("Unexpected server error");
      }
    }
  }
}
