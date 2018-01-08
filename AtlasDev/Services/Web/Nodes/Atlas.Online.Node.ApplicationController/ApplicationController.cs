using Atlas.Online.Node.Core;
using MassTransit;
using Ninject;
using Ninject.Extensions.Logging;
using System;
using Atlas.Online.Data.Models.Definitions;
using Atlas.Enumerators;
using DevExpress.Xpo;
using System.Linq;
using Atlas.Common.Extensions;
using Atlas.Domain.DTO;
using Atlas.Domain.Model;
using System.Collections.Generic;
using Atlas.RabbitMQ.Messages.Notification;
using Magnum;
using Atlas.RabbitMQ.Messages.Online;


namespace Atlas.Online.Node.ApplicationController
{
  public sealed class ApplicationControllerNode
  {
    private IKernel _kernel;
    private static Object obj = new Object();
    public ApplicationControllerNode(ILogger ilogger, IServiceBus ibus, IKernel kernel)
    {
      this._kernel = kernel;
    }

    public void Start()
    {
    }

    public void Stop()
    {
    }

  }
}