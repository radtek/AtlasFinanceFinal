using Atlas.Common.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Atlas.Domain.Serializable.Product
{
  /// <summary>
  /// Unique column is IMEI. 
  /// </summary>
  [Serializable]
  public class SimCard
  {
    [Description("Cell Number")]
    [SearchValue(1)]
    public string Number { get; set; }
    [UseScanner]
    [Description("IMEI")]
    [SearchValue(2)]
    [IsUnique]
    public string IMEI { get; set; }
    [Description("Service Provider")]
    public string Provider { get; set; }
    [Description("Issue Date")]
    public DateTime? IssueDate { get; set; }
  }
}
