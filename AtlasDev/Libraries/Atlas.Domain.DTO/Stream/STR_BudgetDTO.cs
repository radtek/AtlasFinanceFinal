using Atlas.Common.Extensions;
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
  public class STR_BudgetDTO
  {
    [DataMember]
    public int BudgetId { get; set; }
    [DataMember]
    public Enumerators.Stream.Budget BudgetType
    {
      get
      {
        return Description.FromStringToEnum<Enumerators.Stream.Budget>();
      }
      set
      {
        value = Description.FromStringToEnum<Enumerators.Stream.Budget>();
      }
    }
    [DataMember]
    public string Description { get; set; }
    [DataMember]
    public DateTime RangeStart { get; set; }
    [DataMember]
    public DateTime RangeEnd { get; set; }
    [DataMember]
    public long MaxValue { get; set; }
    [DataMember]
    public long CurrentValue { get; set; }
    [DataMember]
    public bool IsActive { get; set; }
    [DataMember]
    public DateTime CreateDate { get; set; }
  }
}