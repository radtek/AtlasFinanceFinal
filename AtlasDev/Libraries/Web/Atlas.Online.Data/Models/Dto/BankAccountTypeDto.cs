using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Atlas.Online.Web.Common.Extensions;

namespace Atlas.Online.Data.Models.DTO
{
  public sealed class BankAccountTypeDto
  {
    public int AccountTypeId { get; set; }
    public Enumerators.General.BankAccountType Type
    {
      get { return Description.FromStringToEnum<Enumerators.General.BankAccountType>(); }
      set { value = Description.FromStringToEnum<Enumerators.General.BankAccountType>(); }
    }
    public string Description { get; set; }

  }
}