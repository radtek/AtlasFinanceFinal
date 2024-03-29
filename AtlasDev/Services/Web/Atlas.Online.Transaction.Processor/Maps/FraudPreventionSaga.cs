﻿using Atlas.Online.Transaction.Processor.Sagas;
using FluentNHibernate.Mapping;
using MassTransit.NHibernateIntegration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Transaction.Processor.Maps
{
  public sealed class FraudPreventionSagaMap : ClassMap<FraudPreventionSaga>
  {
    public FraudPreventionSagaMap()
    {
      Not.LazyLoad();

      Id(x => x.CorrelationId).GeneratedBy.Assigned();

      Map(x => x.CurrentState)
        .Access.ReadOnlyPropertyThroughCamelCaseField(Prefix.Underscore)
        .CustomType<StateMachineUserType>();
    }
  }
}