using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.DTO
{
  public sealed class BankDto
  {
    public int BankId { get; set; }
    public Enumerators.General.BankName Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.BankName>(); }
      set { Description = value.ToStringEnum(); }
    }
    public string Code { get; set; }
    public string Description { get; set; }
    public bool Enabled { get; set; }

  }
}