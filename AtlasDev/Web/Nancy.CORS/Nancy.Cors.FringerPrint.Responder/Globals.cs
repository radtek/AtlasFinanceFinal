using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nancy.CORS
{
  public static class Global
  {
    public static IServiceBus ServiceBus { get; set; }
  }
}
