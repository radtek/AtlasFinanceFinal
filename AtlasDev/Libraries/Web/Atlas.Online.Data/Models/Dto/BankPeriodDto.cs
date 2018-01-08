using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.DTO
{
  public sealed class BankPeriodDto 
  {
    public int PeriodId { get; set; }
    public Enumerators.General.BankPeriod Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.BankPeriod>(); }
      set { value = Description.FromStringToEnum<Enumerators.General.BankPeriod>(); }
    }
    public string Description { get; set; }

  }
}