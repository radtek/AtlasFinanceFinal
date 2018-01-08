using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Colony.Integration.Service.Saga;
using FluentNHibernate.Mapping;
using MassTransit.NHibernateIntegration;

namespace Atlas.Colony.Integration.Service.Map
{
  public sealed class SmsSendSagaMap : ClassMap<SmsSendSaga>
  {
    public SmsSendSagaMap()
    {
      Not.LazyLoad();

      Id(x => x.CorrelationId).GeneratedBy.Assigned();

      Map(x => x.CurrentState)
        .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
        .CustomType<StateMachineUserType>();
    }
  }
}
