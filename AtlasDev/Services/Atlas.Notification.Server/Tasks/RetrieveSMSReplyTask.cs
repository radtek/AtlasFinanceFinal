using System;
using System.Configuration;
using System.Linq;
using Atlas.Domain.Model;
using Atlas.Notification.Server.Structures;
using DevExpress.Xpo;
using log4net;
using Quartz;

namespace Atlas.Notification.Server.Tasks
{
  [DisallowConcurrentExecution]
  public sealed class RetrieveSMSReplyTask : IJob
  {
    private static readonly ILog _log = LogManager.GetLogger(typeof(RetrieveSMSReplyTask));

    public void Execute(IJobExecutionContext context)
    {
      try
      {
        using (var uow = new UnitOfWork())
        {
          // Get only the items that are stored in the cache.
          var maxReply = new XPQuery<NTF_Notification>(uow).Where(o => o.EventId != null && o.ReplyId != null).Max(p => p.ReplyId);

          var construct = string.Format("<reply><settings><id>{0}</id>" +
                                       "<cols_returned>sentcustomerid,eventid,receiveddata,numfrom</cols_returned>" +
                                       "<max_recs>1000</max_recs></settings></reply>", maxReply);

          using (var client = new SMSPortal.APISoapClient())
          {
            var result = client.Reply_STR_STR(ConfigurationManager.AppSettings["smsportal.user"], ConfigurationManager.AppSettings["smsportal.password"], construct);
            var replyObject = ((api_result)Atlas.Common.Utils.Xml.DeSerialize<api_result>(result));
            
            foreach(var itm in replyObject.Items)
            {
              if(itm.GetType() == typeof(api_resultData))
              {
                var data = ((api_resultData)itm);

                var notificationItem = new XPQuery<NTF_Notification>(uow).FirstOrDefault(o => o.EventId == Int64.Parse(data.eventid) && o.ReplyId == 0);
                if(notificationItem != null)
                {
                  _log.Info(string.Format("Got response for NotificationId: {0}, EventId: {1}, ReplyId: {2}, Reply: {3}",
                     notificationItem.NotificationId, data.eventid, data.replyid, data.receiveddata));
                  notificationItem.ReplyId = Int64.Parse(data.replyid);
                  notificationItem.Reply = data.receiveddata;
                  notificationItem.Save();
                }
              }
            }

            uow.CommitChanges();
          }
        }
      }
      catch (Exception err)
      {
        _log.Error("RetrieveSMSReplyTask - Execute", err);
      }
    }
  }
}