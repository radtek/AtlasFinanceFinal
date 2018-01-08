using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Domain.DTO
{
  [Serializable]
  [DataContract(IsReference = true)]
  public class STR_CaseNotificationDTO
  {
    [DataMember]
    public Int64 CaseNotificationId { get; set; }
    [DataMember]
    public STR_CaseDTO Case { get; set; }
    [DataMember]
    public NTF_HistoryDTO Notification { get; set; }
    [DataMember]
    public Guid NotificationReference { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}