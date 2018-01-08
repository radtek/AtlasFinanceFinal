using System;
using System.Collections.Generic;
using Atlas.Domain.Model;
using Falcon.Common.Interfaces.Structures;

namespace Falcon.Common.Interfaces.Repositories
{
  public interface INotificationRepository
  {
    // TODO: change to interface
    List<ISmsNotification> GetSms(Func<NTF_Notification, bool> func);
  }
}
