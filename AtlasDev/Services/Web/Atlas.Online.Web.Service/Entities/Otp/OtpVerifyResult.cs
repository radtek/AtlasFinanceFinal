using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Online.Web.Service.Entities.Otp
{
  public class OtpVerifyResult
  {
    public bool Success { get; set; }
    public int RemainingVerifyRetries { get; set; }
  }
}
