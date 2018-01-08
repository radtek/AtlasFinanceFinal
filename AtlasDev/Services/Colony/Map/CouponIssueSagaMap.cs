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
  public sealed class CouponIssueSagaMap : ClassMap<CouponIssueSaga>
  {
    public CouponIssueSagaMap()
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
