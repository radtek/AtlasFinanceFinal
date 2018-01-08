using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nancy.Cors.Rabbit.Messages
{
 [Serializable]
  public sealed class FingerPrintRequest 
  {
   public FingerPrintRequest(Guid correlationId)
    {
      CorrelationId = correlationId;
      CreatedAt = DateTime.UtcNow;
    }

    public Guid CorrelationId { get; set; }
    private DateTime CreatedAt { get; set; }
    public string TrackingId { get; set; }
    public bool HasError { get; set; }
    public string Error { get; set; }
  }
}
