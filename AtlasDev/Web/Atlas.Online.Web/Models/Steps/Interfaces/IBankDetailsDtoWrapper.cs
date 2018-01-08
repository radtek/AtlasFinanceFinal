using Atlas.Online.Web.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atlas.Online.Web.Models.Steps.Interfaces
{
  public interface IBankDetailsDtoWrapper
  {
    BankDetailDto BankDetail { get; set; }
  }
}