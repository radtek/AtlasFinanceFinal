using System;
using FluentNHibernate.Mapping;
using MassTransit.NHibernateIntegration;
using Falcon.Gyrkin.ESB.Saga;


namespace Falcon.Gyrkin.ESB.Maps
{
  public sealed class FingerPrintEventMessageSagaMap : ClassMap<FingerPrintEventMessageSaga>
  {
    public FingerPrintEventMessageSagaMap()
    {
      Not.LazyLoad();

      Id(x => x.CorrelationId).GeneratedBy.Assigned();

      Map(x => x.CurrentState)
        .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
        .CustomType<StateMachineUserType>();

      Map(x => x.CreatDate);
    }
  }
}
