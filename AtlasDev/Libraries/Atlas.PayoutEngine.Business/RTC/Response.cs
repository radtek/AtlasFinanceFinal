using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.PayoutEngine.Business.RTC
{
  public class Response
  {
    [StringLength(16, MinimumLength = 1)]
    public int TransmissionId { get; set; }
    [StringLength(16, MinimumLength = 6)]
    public int PayoutId { get; set; }
    [StringLength(4, MinimumLength = 4)]
    public string ReplyCode { get; set; }
    [StringLength(210, MinimumLength = 1)]
    public string Reason { get; set; }
    [StringLength(6, MinimumLength = 6)]
    public string DateSent { get; set; }
    public bool Accepted { get; set; }
  }
}
